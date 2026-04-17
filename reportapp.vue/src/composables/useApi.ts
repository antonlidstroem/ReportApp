import axios, { AxiosError } from "axios";

// Use http (not https) to avoid mixed-content errors in dev.
// Vite dev server is HTTP; backend http profile runs on 5207.
const api = axios.create({
  baseURL: "http://localhost:5207/api",
  headers: { "Content-Type": "application/json" },
});

// Chart.js is served as a static file from the .NET app (wwwroot/chart.umd.min.js).
// jsreport's headless Chrome can reach localhost, avoiding external CDN blocks.
export const CHART_JS_URL = "http://localhost:5207/chart.umd.min.js";

export interface ExportParams {
  provider: string;
  format: string;
  surveyId: number;
  htmlTemplate?: string;
  start?: string;
  end?: string;
  questionIds?: number[] | string | null;
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
  type: string; // "Scale" | "YesNo"
  responseCount: number;
}

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
    } else if (typeof params.questionIds === "string" && params.questionIds.trim()) {
      qp.questionIds = params.questionIds;
    }
  }

  let res;
  try {
    res = await api.post(
      `/reports/export/${params.provider}/${params.format}/${params.surveyId}`,
      { htmlTemplate: params.htmlTemplate ?? "" },
      { params: qp, responseType: "blob" }
    );
  } catch (err) {
    if (err instanceof AxiosError && err.response?.data instanceof Blob) {
      const t = await err.response.data.text();
      throw new Error(`Export failed (${err.response.status}): ${t.substring(0, 300)}`);
    }
    throw err;
  }

  const serverMs = res.headers["x-generation-time-ms"];
  const generationMs = serverMs ? parseInt(serverMs, 10) : Math.round(performance.now() - t0);
  const serverBytes = res.headers["x-file-size-bytes"];
  const fileSizeBytes = serverBytes ? parseInt(serverBytes, 10) : res.data.size;

  const fmt = params.format.toLowerCase();
  const ext = fmt === "excel" ? "xlsx" : fmt === "ppt" ? "pptx" : fmt;

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

// ─── Rich Handlebars templates ───────────────────────────────────────────────
// Chart.js is loaded from localhost (served by .NET UseStaticFiles).
// The jsreport headless Chrome can reach localhost:5207 without internet access.

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
.score{font-weight:700}
.bar-bg{height:7px;background:#e2e8f0;border-radius:4px;overflow:hidden}
.bar-fill{height:100%;border-radius:4px}
.cat-badge{display:inline-block;font-size:9px;padding:2px 7px;border-radius:100px;background:#eff6ff;color:#1e3a5f}
.footer{padding:18px 60px;border-top:1px solid #e2e8f0;display:flex;justify-content:space-between;font-size:10px;color:#94a3b8}
</style>
<script src="${CHART_JS_URL}"></script>
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
        <td class="score">{{AverageValue}}</td>
        <td>{{TotalResponses}}</td>
        <td><div class="bar-bg"><div class="bar-fill" style="width:{{AverageValue20Pct}}%;background:#1e3a5f"></div></div></td>
      </tr>
      {{/each}}
    </tbody>
  </table>
</div>
<div class="footer"><span>{{CompanyName}} · {{SurveyTitle}}</span><span>Konfidentiellt — {{GeneratedAt}}</span></div>
<script>
var bLabels=[{{#each QuestionSummaries}}'Q{{@index}}'{{#unless @last}},{{/unless}}{{/each}}];
var bData=[{{#each QuestionSummaries}}{{AverageValue}}{{#unless @last}},{{/unless}}{{/each}}];
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
.meta{margin-top:24px;font-size:10px;color:rgba(255,255,255,.28)}
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
<script src="${CHART_JS_URL}"></script>
</head><body>
<div class="hero">
  <div class="badge">Executive Report</div>
  <div class="title">{{SurveyTitle}}</div>
  <div class="co">{{CompanyName}}</div>
  <div class="meta">{{GeneratedAt}}</div>
</div>
<div class="metrics">
  <div class="m"><div class="m-val">{{OverallAverage}}</div><div class="m-lbl">Totalpoäng</div></div>
  <div class="m"><div class="m-val">{{QuestionCount}}</div><div class="m-lbl">Frågor</div></div>
  <div class="m"><div class="m-val">{{TotalResponses}}</div><div class="m-lbl">Totalt svar</div></div>
</div>
<div class="grid">
  <div class="panel">
    <div class="panel-title">Medelvärde per kategori</div>
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
var allQ=[{{#each QuestionSummaries}}{c:'{{Category}}',v:{{AverageValue}}}{{#unless @last}},{{/unless}}{{/each}}];
var cats=[...new Set(allQ.map(function(q){return q.c;}))];
var avgs=cats.map(function(c){var a=allQ.filter(function(q){return q.c===c;});return+(a.reduce(function(s,q){return s+q.v;},0)/a.length).toFixed(2);});
new Chart(document.getElementById('r'),{type:'bar',data:{labels:cats,datasets:[{data:avgs,backgroundColor:'rgba(96,165,250,0.7)',borderColor:'#60a5fa',borderWidth:1,borderRadius:4}]},options:{responsive:true,maintainAspectRatio:false,plugins:{legend:{display:false}},scales:{y:{min:0,max:5,ticks:{color:'#334155',stepSize:1,font:{size:8}},grid:{color:'#1e293b'}},x:{ticks:{color:'#94a3b8',font:{size:9}},grid:{display:false}}}}});
</script></body></html>`,

  playwrightSimple: `<!DOCTYPE html>
<html><head><meta charset="UTF-8">
<style>*{margin:0;padding:0;box-sizing:border-box}body{font-family:'Segoe UI',sans-serif;padding:40px;color:#1e293b}
h1{font-size:26px;font-weight:800;color:#0f2d4a;margin-bottom:4px}
.sub{color:#64748b;font-size:14px;margin-bottom:6px}.meta{font-size:11px;color:#94a3b8;margin-bottom:30px}
.notice{padding:13px;background:#fffbeb;border:1px solid #fcd34d;border-radius:7px;font-size:11px;color:#92400e;margin-top:20px}
code{background:#fef3c7;padding:1px 4px;border-radius:3px;font-family:monospace;font-size:10px}
</style></head><body>
<h1>{{SurveyTitle}}</h1>
<div class="sub">{{CompanyName}}</div>
<div class="meta">Genererad: {{GeneratedAt}}</div>
{{QuestionsTableHtml}}
<div class="notice">ℹ Playwright-mall: Stödjer <code>{{SurveyTitle}}</code>, <code>{{CompanyName}}</code>, <code>{{GeneratedAt}}</code>, <code>{{QuestionsTableHtml}}</code>.</div>
</body></html>`,
};

// ─── Build a self-contained demo template with inline chart data ──────────────
// Used by Card 1 (Chart.js demo) so the exported PDF matches the live sliders exactly.
// Data is serialized as JSON directly into the JS — no Handlebars needed, no API call.
export function buildDemoTemplate(
  chartType: string,
  labels: string[],
  values: number[],
  colors: string[]
): string {
  const labelsJson = JSON.stringify(labels);
  const valuesJson = JSON.stringify(values);
  const colorsJson = JSON.stringify(colors.map((c) => c + "cc"));

  const isRound = chartType === "doughnut";
  const bgColors = isRound
    ? JSON.stringify(["#a855f7","#7c3aed","#6d28d9","#5b21b6","#4c1d95"])
    : colorsJson;

  const scalesConfig = chartType === "radar"
    ? `r:{min:0,max:5,ticks:{color:'#666',stepSize:1},grid:{color:'#2d1060'},pointLabels:{color:'#94a3b8'}}`
    : isRound
    ? ""
    : `y:{min:0,max:5,ticks:{color:'#888'},grid:{color:'#1e1e3f'}},x:{ticks:{color:'#888'},grid:{color:'#1e1e3f'}}`;

  const scalesStr = scalesConfig ? `scales:{${scalesConfig}}` : "";

  const date = new Date().toLocaleDateString("sv-SE");

  // Build a mini table of the data
  const tableRows = labels.map((lbl, i) => {
    const v = values[i] ?? 0;
    const color = v >= 4 ? "#22c55e" : v >= 3 ? "#f59e0b" : "#ef4444";
    const pct = Math.round(v * 20);
    return `<tr>
      <td style="padding:7px 12px">${lbl}</td>
      <td style="padding:7px 12px;font-weight:700;color:${color}">${v.toFixed(1)}</td>
      <td style="padding:7px 12px">
        <div style="height:6px;background:#e2e8f0;border-radius:3px;overflow:hidden">
          <div style="width:${pct}%;height:100%;background:${color};border-radius:3px"></div>
        </div>
      </td>
    </tr>`;
  }).join("");

  return `<!DOCTYPE html>
<html><head><meta charset="UTF-8">
<style>
*{margin:0;padding:0;box-sizing:border-box}
body{font-family:'Segoe UI',sans-serif;padding:40px;color:#1e293b;background:#fff}
.cover{background:linear-gradient(135deg,#0d0018,#1a0533);color:white;padding:32px 40px;border-radius:8px;margin-bottom:28px}
.cover h1{font-size:22px;font-weight:800;margin-bottom:4px;letter-spacing:-0.5px}
.cover p{font-size:12px;opacity:.6}
.chart-wrap{height:300px;position:relative;margin-bottom:28px;padding:20px;background:#f8fafc;border-radius:8px;border:1px solid #e2e8f0}
.section-title{font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:1px;color:#94a3b8;margin-bottom:12px}
table{width:100%;border-collapse:collapse;font-size:12px}
thead{background:#0d0018;color:white}
th{padding:8px 12px;text-align:left;font-weight:600}
td{border-bottom:1px solid #f1f5f9;vertical-align:middle}
tr:nth-child(even) td{background:#fafafa}
</style>
<script src="${CHART_JS_URL}"></script>
</head>
<body>
  <div class="cover">
    <h1>Chart.js Demo — ${chartType.charAt(0).toUpperCase() + chartType.slice(1)}-diagram</h1>
    <p>Genererad ${date} · Renderad av jsreport + Chromium</p>
  </div>
  <div class="section-title">Live Chart.js-diagram (identisk med förhandsgranskning)</div>
  <div class="chart-wrap"><canvas id="c"></canvas></div>
  <div class="section-title" style="margin-top:8px">Datatabell</div>
  <table>
    <thead><tr><th>Kategori</th><th>Värde</th><th>Bar</th></tr></thead>
    <tbody>${tableRows}</tbody>
  </table>
<script>
new Chart(document.getElementById('c'),{
  type:${JSON.stringify(chartType)},
  data:{
    labels:${labelsJson},
    datasets:[{
      data:${valuesJson},
      backgroundColor:${bgColors},
      borderColor:${isRound ? "'transparent'" : "'#a855f7'"},
      borderWidth:2,
      borderRadius:${chartType === "bar" ? 4 : 0},
      fill:${chartType === "line" ? "true" : "false"},
      tension:0.4,
      pointBackgroundColor:'#a855f7'
    }]
  },
  options:{
    responsive:true,maintainAspectRatio:false,
    plugins:{legend:{display:${isRound ? "true" : "false"},labels:{color:'#64748b'}}},
    ${scalesStr}
  }
});
</script>
</body></html>`;
}

// Build a concrete HTML preview for the iframe (no Handlebars, real data injected as JS)
export function buildPreviewHtml(
  template: "full" | "executive" | "playwright",
  data: {
    title: string;
    company: string;
    questions: Array<{ text: string; category: string; avg: number; responses: number }>;
  }
): string {
  const date = new Date().toLocaleDateString("sv-SE");
  const overall = (
    data.questions.reduce((s, q) => s + q.avg, 0) / Math.max(data.questions.length, 1)
  ).toFixed(2);

  const labelsJson = JSON.stringify(data.questions.map((_, i) => `Q${i + 1}`));
  const valuesJson = JSON.stringify(data.questions.map((q) => q.avg));

  if (template === "full") {
    const rows = data.questions
      .map(
        (q, i) =>
          `<tr><td>Q${i + 1}: ${q.text.substring(0, 55)}</td>
          <td><span style="background:#eff6ff;color:#1e3a5f;padding:2px 8px;border-radius:100px;font-size:10px">${q.category}</span></td>
          <td style="font-weight:700">${q.avg.toFixed(2)}</td>
          <td>${q.responses}</td></tr>`
      )
      .join("");

    return `<!DOCTYPE html><html><head><meta charset="UTF-8"><style>
*{margin:0;padding:0;box-sizing:border-box}
body{font-family:'Segoe UI',sans-serif;background:#fff;color:#1e293b}
.cover{background:linear-gradient(135deg,#1e3a5f,#0f2d4a);color:white;padding:36px 40px}
.cover h1{font-size:26px;font-weight:800;margin-bottom:3px}
.cover p{opacity:.6;font-size:12px}
.section{padding:24px 40px}
.title{font-size:12px;font-weight:700;color:#1e3a5f;margin-bottom:14px;padding-bottom:6px;border-bottom:2px solid #e2e8f0;text-transform:uppercase;letter-spacing:.5px}
.chart-wrap{height:200px;position:relative;margin-bottom:20px}
table{width:100%;border-collapse:collapse;font-size:11px}
thead{background:#1e3a5f;color:white}
th{padding:7px 10px;text-align:left;font-weight:600}
td{padding:6px 10px;border-bottom:1px solid #e2e8f0}
tr:nth-child(even){background:#f8fafc}
</style>
<script src="${CHART_JS_URL}"></script>
</head><body>
<div class="cover"><h1>${data.title}</h1><p>${data.company} · ${date}</p></div>
<div class="section">
<div class="title">Poäng per fråga — Chart.js renderat av Chromium</div>
<div class="chart-wrap"><canvas id="c"></canvas></div>
<table><thead><tr><th>Fråga</th><th>Kategori</th><th>Snitt</th><th>Svar</th></tr></thead><tbody>${rows}</tbody></table>
</div>
<script>new Chart(document.getElementById('c'),{type:'bar',data:{labels:${labelsJson},datasets:[{data:${valuesJson},backgroundColor:'rgba(30,58,95,0.7)',borderColor:'#1e3a5f',borderWidth:1,borderRadius:4}]},options:{responsive:true,maintainAspectRatio:false,plugins:{legend:{display:false}},scales:{y:{min:0,max:5,grid:{color:'#f1f5f9'},ticks:{stepSize:1}},x:{grid:{display:false}}}}})</\\/script>
</body></html>`;
  }

  if (template === "executive") {
    const catMap = new Map<string, number[]>();
    data.questions.forEach((q) => {
      const a = catMap.get(q.category) ?? [];
      a.push(q.avg);
      catMap.set(q.category, a);
    });
    const cats = [...catMap.keys()];
    const catAvgs = cats.map((c) => {
      const a = catMap.get(c)!;
      return a.reduce((s, v) => s + v, 0) / a.length;
    });
    const scoreRows = data.questions
      .map(
        (q) =>
          `<div style="display:flex;align-items:center;gap:10px;padding:5px 0;border-bottom:1px solid #1e293b">
          <div style="flex:1;font-size:10px;color:#94a3b8;white-space:nowrap;overflow:hidden;text-overflow:ellipsis">${q.text.substring(0, 40)}</div>
          <div style="width:70px;height:4px;background:#1e293b;border-radius:2px;overflow:hidden">
            <div style="width:${q.avg * 20}%;height:100%;background:#60a5fa;border-radius:2px"></div>
          </div>
          <div style="font-size:11px;font-weight:700;color:#60a5fa;width:26px;text-align:right">${q.avg.toFixed(1)}</div>
        </div>`
      )
      .join("");

    const catsJson = JSON.stringify(cats);
    const catAvgsJson = JSON.stringify(catAvgs);

    return `<!DOCTYPE html><html><head><meta charset="UTF-8"><style>
*{margin:0;padding:0;box-sizing:border-box}
body{font-family:'Segoe UI',sans-serif;background:#0f172a;color:#e2e8f0}
.hero{background:linear-gradient(135deg,#1e3a5f,#0a0f1e);padding:36px 40px}
.badge{font-size:9px;text-transform:uppercase;letter-spacing:3px;color:#60a5fa;margin-bottom:8px}
.title{font-size:30px;font-weight:800;color:white;margin-bottom:4px}
.co{font-size:12px;color:rgba(255,255,255,.5)}
.metrics{display:grid;grid-template-columns:repeat(3,1fr);gap:1px;background:#1e293b}
.m{padding:18px 22px;background:#0f172a}
.m-val{font-size:28px;font-weight:800;color:#60a5fa}
.m-lbl{font-size:9px;color:#475569;text-transform:uppercase;letter-spacing:1px;margin-top:3px}
.grid{display:grid;grid-template-columns:1fr 1fr;gap:1px;background:#1e293b}
.panel{background:#0f172a;padding:22px}
.panel-title{font-size:9px;font-weight:700;color:#94a3b8;text-transform:uppercase;letter-spacing:1px;margin-bottom:12px}
.chart-area{height:200px;position:relative}
</style>
<script src="${CHART_JS_URL}"></script>
</head><body>
<div class="hero"><div class="badge">Executive Report</div><div class="title">${data.title}</div><div class="co">${data.company}</div></div>
<div class="metrics">
<div class="m"><div class="m-val">${overall}</div><div class="m-lbl">Totalpoäng</div></div>
<div class="m"><div class="m-val">${data.questions.length}</div><div class="m-lbl">Frågor</div></div>
<div class="m"><div class="m-val">${data.questions.reduce((s, q) => s + q.responses, 0)}</div><div class="m-lbl">Svar</div></div>
</div>
<div class="grid">
<div class="panel"><div class="panel-title">Medelvärde per kategori</div><div class="chart-area"><canvas id="r"></canvas></div></div>
<div class="panel"><div class="panel-title">Poäng</div>${scoreRows}</div>
</div>
<script>new Chart(document.getElementById('r'),{type:'bar',data:{labels:${catsJson},datasets:[{data:${catAvgsJson},backgroundColor:'rgba(96,165,250,0.7)',borderColor:'#60a5fa',borderWidth:1,borderRadius:4}]},options:{responsive:true,maintainAspectRatio:false,plugins:{legend:{display:false}},scales:{y:{min:0,max:5,ticks:{color:'#334155',stepSize:1,font:{size:8}},grid:{color:'#1e293b'}},x:{ticks:{color:'#94a3b8',font:{size:9}},grid:{display:false}}}}})<\\/script>
</body></html>`;
  }

  // playwright fallback
  const tableRows = data.questions
    .map(
      (q, i) =>
        `<tr><td>Q${i + 1}: ${q.text.substring(0, 60)}</td><td style="font-weight:700">${q.avg.toFixed(2)}</td><td>${q.responses}</td></tr>`
    )
    .join("");
  return `<!DOCTYPE html><html><head><meta charset="UTF-8"><style>*{margin:0;padding:0;box-sizing:border-box}body{font-family:'Segoe UI',sans-serif;padding:32px;color:#1e293b}h1{font-size:22px;font-weight:800;color:#0f2d4a;margin-bottom:4px}.sub{color:#64748b;font-size:13px}.meta{font-size:10px;color:#94a3b8;margin-bottom:24px;margin-top:4px}table{width:100%;border-collapse:collapse;font-size:11px;margin-bottom:18px}thead{background:#0f2d4a;color:white}th{padding:8px 10px;text-align:left}td{padding:6px 10px;border-bottom:1px solid #e2e8f0}tr:nth-child(even){background:#f8fafc}</style></head><body><h1>${data.title}</h1><div class="sub">${data.company}</div><div class="meta">Genererad: ${date}</div><table><thead><tr><th>Fråga</th><th>Genomsnitt</th><th>Svar</th></tr></thead><tbody>${tableRows}</tbody></table></body></html>`;
}

export function buildRichTemplateWithData(_template: string, _data: unknown): string {
  return "";
}
