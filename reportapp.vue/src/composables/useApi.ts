import axios, { AxiosError } from "axios";

const BASE_URL =
  (import.meta.env.VITE_API_BASE_URL as string | undefined) ??
  "http://localhost:5207/api";

const api = axios.create({
  baseURL: BASE_URL,
  headers: { "Content-Type": "application/json" },
});

export function getBaseUrl(): string {
  return BASE_URL;
}

// ── Health check ─────────────────────────────────────────────────────────────
export async function checkHealth(): Promise<boolean> {
  try {
    await api.get("/reports/surveys", { timeout: 3000 });
    return true;
  } catch {
    return false;
  }
}

// ── Types ─────────────────────────────────────────────────────────────────────
export interface ExportParams {
  provider: string;
  format: string;
  surveyId: number;
  htmlTemplate?: string;
  start?: string;
  end?: string;
  questionIds?: number[] | string | null;
  moduleConfig?: Record<string, unknown>;
}

export interface ExportResult {
  blob: Blob;
  filename: string;
  generationMs: number;
  fileSizeBytes: number;
}

export interface Survey {
  id: number;
  title: string;
  companyName: string;
  questionCount: number;
}

export interface Question {
  id: number;
  text: string;
  category: string;
  type: string;
  responseCount: number;
}

export interface ExportError {
  status: number | null;
  message: string;
  isNetworkError: boolean;
  isNotImplemented: boolean;
}

// ── API calls ─────────────────────────────────────────────────────────────────
export async function fetchSurveys(): Promise<Survey[]> {
  const res = await api.get("/reports/surveys");
  return res.data;
}

export async function fetchQuestions(surveyId: number): Promise<Question[]> {
  const res = await api.get(`/reports/surveys/${surveyId}/questions`);
  return res.data;
}

export async function exportReport(params: ExportParams): Promise<ExportResult> {
  const t0 = performance.now();

  const qp: Record<string, string> = {};
  if (params.start) qp.start = params.start;
  if (params.end) qp.end = params.end;
  if (params.questionIds != null) {
    if (Array.isArray(params.questionIds) && params.questionIds.length > 0) {
      qp.questionIds = params.questionIds.join(",");
    } else if (
      typeof params.questionIds === "string" &&
      params.questionIds.trim()
    ) {
      qp.questionIds = params.questionIds;
    }
  }

  let res;
  try {
    res = await api.post(
      `/reports/export/${params.provider}/${params.format}/${params.surveyId}`,
      {
        htmlTemplate: params.htmlTemplate ?? "",
        moduleConfig: params.moduleConfig ?? {},
      },
      { params: qp, responseType: "blob" }
    );
  } catch (err) {
    // Build a structured error with as much detail as possible
    const exportError: ExportError = {
      status: null,
      message: "Unknown error",
      isNetworkError: false,
      isNotImplemented: false,
    };

    if (err instanceof AxiosError) {
      exportError.status = err.response?.status ?? null;
      exportError.isNetworkError = !err.response; // no response = network/connection error

      if (err.response?.data instanceof Blob) {
        try {
          const text = await err.response.data.text();
          exportError.message = text.substring(0, 400);
          exportError.isNotImplemented =
            text.includes("NotImplementedException") ||
            text.includes("not implemented") ||
            text.includes("requires a paid");
        } catch {
          exportError.message = `HTTP ${err.response.status}`;
        }
      } else if (err.message) {
        exportError.message = err.message;
      }

      if (exportError.isNetworkError) {
        exportError.message = `Cannot reach API at ${BASE_URL}. Is the .NET backend running?`;
      }
    } else if (err instanceof Error) {
      exportError.message = err.message;
    }

    // Throw a typed error that ExportButton can distinguish
    const thrownError = new Error(exportError.message) as Error & {
      exportError: ExportError;
    };
    thrownError.exportError = exportError;
    throw thrownError;
  }

  const serverMs = res.headers["x-generation-time-ms"];
  const generationMs = serverMs
    ? parseInt(serverMs, 10)
    : Math.round(performance.now() - t0);

  const serverBytes = res.headers["x-file-size-bytes"];
  const fileSizeBytes = serverBytes ? parseInt(serverBytes, 10) : res.data.size;

  const fmt = params.format.toLowerCase();
  const ext =
    fmt === "excel" ? "xlsx" : fmt === "ppt" ? "pptx" : fmt;

  return {
    blob: res.data,
    filename: `rapport_${params.provider}_${fmt}_${new Date()
      .toLocaleDateString("sv-SE")
      .replace(/-/g, "")}.${ext}`,
    generationMs,
    fileSizeBytes,
  };
}

export function downloadBlob(blob: Blob, filename: string) {
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.setAttribute("download", filename);
  document.body.appendChild(a);
  a.click();
  a.remove();
  setTimeout(() => URL.revokeObjectURL(url), 100);
}

// ── Rich Handlebars templates ─────────────────────────────────────────────────
export const RICH_TEMPLATES = {
  jsreportFull: `<!DOCTYPE html>
<html lang="sv"><head><meta charset="UTF-8">
<style>
*{margin:0;padding:0;box-sizing:border-box}
body{font-family:'Segoe UI',sans-serif;background:#fff;color:#1e293b}
.cover{background:linear-gradient(135deg,#1e3a5f,#0f2d4a);color:white;padding:52px 60px}
.cover-eyebrow{font-size:10px;text-transform:uppercase;letter-spacing:3px;color:rgba(255,255,255,.4);margin-bottom:12px}
.cover-title{font-size:36px;font-weight:800;margin-bottom:5px;letter-spacing:-1px}
.cover-company{font-size:15px;opacity:.6}
.cover-meta{margin-top:44px;font-size:11px;opacity:.35}
.kpi-strip{display:grid;grid-template-columns:repeat(4,1fr);border-bottom:2px solid #e2e8f0}
.kpi{padding:18px 22px;border-right:1px solid #e2e8f0}
.kpi:last-child{border-right:none}
.kpi-val{font-size:28px;font-weight:800;color:#1e3a5f}
.kpi-lbl{font-size:10px;color:#94a3b8;text-transform:uppercase;letter-spacing:1px;margin-top:3px}
.section{padding:32px 60px}
.section-title{font-size:13px;font-weight:700;color:#1e3a5f;margin-bottom:18px;padding-bottom:7px;border-bottom:2px solid #e2e8f0;text-transform:uppercase;letter-spacing:.5px}
.chart-wrap{height:260px;position:relative;margin-bottom:32px}
table{width:100%;border-collapse:collapse;font-size:11px}
thead tr{background:#1e3a5f;color:white}
thead th{padding:9px 12px;text-align:left;font-weight:600}
tbody tr:nth-child(even){background:#f8fafc}
td{padding:8px 12px;border-bottom:1px solid #e2e8f0;vertical-align:middle}
.bar-bg{height:7px;background:#e2e8f0;border-radius:4px;overflow:hidden}
.bar-fill{height:100%;border-radius:4px}
.cat-badge{display:inline-block;font-size:9px;padding:2px 7px;border-radius:100px;background:#eff6ff;color:#1e3a5f}
.footer{padding:18px 60px;border-top:1px solid #e2e8f0;display:flex;justify-content:space-between;font-size:10px;color:#94a3b8}
</style>
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
</head><body>
<div class="cover">
  <div class="cover-eyebrow">Analysrapport</div>
  <div class="cover-title">{{SurveyTitle}}</div>
  <div class="cover-company">{{CompanyName}}</div>
  <div class="cover-meta">Genererad {{GeneratedAt}}</div>
</div>
<div class="kpi-strip">
  <div class="kpi"><div class="kpi-val">{{QuestionCount}}</div><div class="kpi-lbl">Frågor</div></div>
  <div class="kpi"><div class="kpi-val">{{TotalResponses}}</div><div class="kpi-lbl">Svar</div></div>
  <div class="kpi"><div class="kpi-val">{{OverallAverage}}</div><div class="kpi-lbl">Genomsnitt</div></div>
  <div class="kpi"><div class="kpi-val">{{CategoryCount}}</div><div class="kpi-lbl">Kategorier</div></div>
</div>
<div class="section">
  <div class="section-title">Poäng per fråga</div>
  <div class="chart-wrap"><canvas id="b"></canvas></div>
  <table>
    <thead><tr><th>Fråga</th><th>Kategori</th><th>Snitt</th><th>Svar</th><th style="width:15%">Bar</th></tr></thead>
    <tbody>
      {{#each QuestionSummaries}}
      <tr>
        <td>{{Text}}</td>
        <td><span class="cat-badge">{{Category}}</span></td>
        <td><strong>{{AverageValue}}</strong></td>
        <td>{{TotalResponses}}</td>
        <td><div class="bar-bg"><div class="bar-fill" style="width:{{AverageValue20Pct}}%;background:#1e3a5f"></div></div></td>
      </tr>
      {{/each}}
    </tbody>
  </table>
</div>
<div class="footer"><span>{{CompanyName}} · {{SurveyTitle}}</span><span>Konfidentiellt — {{GeneratedAt}}</span></div>
<script>
const bLabels=[{{#each QuestionSummaries}}'Q{{@index}}'{{#unless @last}},{{/unless}}{{/each}}];
const bData=[{{#each QuestionSummaries}}{{AverageValue}}{{#unless @last}},{{/unless}}{{/each}}];
new Chart(document.getElementById('b'),{type:'bar',data:{labels:bLabels,datasets:[{data:bData,backgroundColor:'rgba(30,58,95,0.7)',borderColor:'#1e3a5f',borderWidth:1,borderRadius:4}]},options:{responsive:true,maintainAspectRatio:false,plugins:{legend:{display:false}},scales:{y:{min:0,max:5,ticks:{stepSize:1},grid:{color:'#f1f5f9'}},x:{grid:{display:false}}}}});
</script></body></html>`,

  jsreportExecutive: `<!DOCTYPE html>
<html lang="sv"><head><meta charset="UTF-8">
<style>
*{margin:0;padding:0;box-sizing:border-box}
body{font-family:'Segoe UI',sans-serif;background:#0f172a;color:#e2e8f0}
.hero{background:linear-gradient(135deg,#1e3a5f,#0a0f1e);padding:48px 56px}
.badge{font-size:10px;text-transform:uppercase;letter-spacing:3px;color:#60a5fa;margin-bottom:10px}
.title{font-size:40px;font-weight:800;color:white;letter-spacing:-1px;margin-bottom:5px}
.co{font-size:14px;color:rgba(255,255,255,.5)}
.metrics{display:grid;grid-template-columns:repeat(3,1fr);gap:1px;background:#1e293b}
.m{padding:24px 28px;background:#0f172a}
.m-val{font-size:36px;font-weight:800;color:#60a5fa}
.m-lbl{font-size:10px;color:#475569;text-transform:uppercase;letter-spacing:1px;margin-top:4px}
.grid{display:grid;grid-template-columns:1fr 1fr;gap:1px;background:#1e293b}
.panel{background:#0f172a;padding:28px}
.panel-title{font-size:10px;font-weight:700;color:#94a3b8;text-transform:uppercase;letter-spacing:1px;margin-bottom:16px}
.chart-area{height:240px;position:relative}
.score-row{display:flex;align-items:center;gap:10px;padding:5px 0;border-bottom:1px solid #1e293b}
.score-lbl{font-size:10px;color:#94a3b8;flex:1;white-space:nowrap;overflow:hidden;text-overflow:ellipsis}
.score-bar{width:80px;height:4px;background:#1e293b;border-radius:2px;overflow:hidden}
.score-bar-fill{height:100%;background:#60a5fa;border-radius:2px}
.score-num{font-size:11px;font-weight:700;width:28px;text-align:right;color:#60a5fa}
.footer{padding:18px 28px;border-top:1px solid #1e293b;font-size:10px;color:#334155;display:flex;justify-content:space-between}
</style>
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
</head><body>
<div class="hero">
  <div class="badge">Executive Report</div>
  <div class="title">{{SurveyTitle}}</div>
  <div class="co">{{CompanyName}}</div>
</div>
<div class="metrics">
  <div class="m"><div class="m-val">{{OverallAverage}}</div><div class="m-lbl">Totalpoäng</div></div>
  <div class="m"><div class="m-val">{{QuestionCount}}</div><div class="m-lbl">Frågor</div></div>
  <div class="m"><div class="m-val">{{TotalResponses}}</div><div class="m-lbl">Totalt svar</div></div>
</div>
<div class="grid">
  <div class="panel">
    <div class="panel-title">Medelvärde per fråga</div>
    <div class="chart-area"><canvas id="r"></canvas></div>
  </div>
  <div class="panel">
    <div class="panel-title">Poäng per fråga</div>
    {{#each QuestionSummaries}}
    <div class="score-row">
      <div class="score-lbl" title="{{Text}}">{{Text}}</div>
      <div class="score-bar"><div class="score-bar-fill" style="width:{{AverageValue20Pct}}%"></div></div>
      <div class="score-num">{{AverageValue}}</div>
    </div>
    {{/each}}
  </div>
</div>
<div class="footer"><span>{{CompanyName}}</span><span>Konfidentiellt · {{GeneratedAt}}</span></div>
<script>
const allQ=[{{#each QuestionSummaries}}{c:'{{Category}}',v:{{AverageValue}}}{{#unless @last}},{{/unless}}{{/each}}];
const cats=[...new Set(allQ.map(q=>q.c))];
const avgs=cats.map(c=>{const a=allQ.filter(q=>q.c===c);return+(a.reduce((s,q)=>s+q.v,0)/a.length).toFixed(2);});
new Chart(document.getElementById('r'),{type:'bar',data:{labels:cats,datasets:[{data:avgs,backgroundColor:'rgba(96,165,250,0.7)',borderColor:'#60a5fa',borderWidth:1,borderRadius:4}]},options:{responsive:true,maintainAspectRatio:false,plugins:{legend:{display:false}},scales:{y:{min:0,max:5,ticks:{color:'#334155',stepSize:1,font:{size:8}},grid:{color:'#1e293b'}},x:{ticks:{color:'#94a3b8',font:{size:9}},grid:{display:false}}}}});
</script></body></html>`,

  playwrightSimple: `<!DOCTYPE html>
<html><head><meta charset="UTF-8">
<style>*{margin:0;padding:0;box-sizing:border-box}body{font-family:'Segoe UI',sans-serif;padding:40px;color:#1e293b}
h1{font-size:26px;font-weight:800;color:#0f2d4a;margin-bottom:4px}
.sub{color:#64748b;font-size:14px;margin-bottom:6px}.meta{font-size:11px;color:#94a3b8;margin-bottom:30px}
.notice{padding:13px;background:#fffbeb;border:1px solid #fcd34d;border-radius:7px;font-size:11px;color:#92400e;margin-top:20px}
</style></head><body>
<h1>{{SurveyTitle}}</h1>
<div class="sub">{{CompanyName}}</div>
<div class="meta">Genererad: {{GeneratedAt}}</div>
{{QuestionsTableHtml}}
<div class="notice">ℹ Playwright-mall: Token-ersättning utan Handlebars-loopar eller Chart.js.</div>
</body></html>`,
};
