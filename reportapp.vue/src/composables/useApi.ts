import axios from "axios";

const api = axios.create({ baseURL: "https://localhost:7008/api" });

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

export async function fetchSurveys() {
  const res = await api.get("/reports/surveys");
  return res.data;
}

export async function fetchQuestions(surveyId: number) {
  const res = await api.get(`/reports/surveys/${surveyId}/questions`);
  return res.data;
}

export async function exportReport(params: ExportParams): Promise<ExportResult> {
  const startPerf = performance.now();

  const cleanParams: Record<string, string> = {};

  if (params.start) cleanParams.start = params.start;
  if (params.end) cleanParams.end = params.end;

  // FIX: Handle both number[] and string for questionIds
  if (params.questionIds != null) {
    if (Array.isArray(params.questionIds) && params.questionIds.length > 0) {
      cleanParams.questionIds = params.questionIds.join(",");
    } else if (typeof params.questionIds === "string" && params.questionIds.trim()) {
      cleanParams.questionIds = params.questionIds;
    }
  }

  const res = await api.post(
    `/reports/export/${params.provider}/${params.format}/${params.surveyId}`,
    { htmlTemplate: params.htmlTemplate ?? "" },
    {
      params: cleanParams,
      responseType: "blob",
    }
  );

  const endPerf = performance.now();

  // Detect format for extension
  const fmt = params.format.toLowerCase();
  const ext = fmt === "excel" ? "xlsx" : fmt === "ppt" ? "pptx" : fmt;
  const generationMs = Math.round(endPerf - startPerf);

  // Try to read server-side generation time from header
  const serverMs = res.headers["x-generation-time-ms"];
  const finalMs = serverMs ? parseInt(serverMs) : generationMs;

  return {
    blob: res.data,
    filename: `Rapport_${params.provider}_${new Date().toLocaleDateString("sv-SE").replace(/-/g, "")}.${ext}`,
    generationMs: finalMs,
    fileSizeBytes: res.data.size,
  };
}

export function downloadBlob(blob: Blob, filename: string) {
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.setAttribute("download", filename);
  document.body.appendChild(link);
  link.click();
  link.remove();
  setTimeout(() => window.URL.revokeObjectURL(url), 100);
}

// ─── Rich HTML Templates ────────────────────────────────────────────────────
// These are the actual templates sent to jsreport/Playwright for PDF generation.
// They use Handlebars syntax (jsreport) or token replacement (playwright).

export const RICH_TEMPLATES = {
  /** Full-featured jsreport template with Chart.js bar + line charts */
  jsreportFull: `<!DOCTYPE html>
<html lang="sv">
<head>
<meta charset="UTF-8">
<style>
  @import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;600;700;800&display=swap');
  * { margin:0; padding:0; box-sizing:border-box; }
  body { font-family: 'Inter', sans-serif; background: #ffffff; color: #1e293b; }
  .cover { background: linear-gradient(135deg, #1e3a5f 0%, #0f2d4a 100%); color:white; padding: 60px; min-height: 280px; display:flex; flex-direction:column; justify-content:space-between; }
  .cover-title { font-size: 36px; font-weight: 800; margin-bottom: 8px; }
  .cover-sub { font-size: 16px; opacity: 0.7; }
  .cover-meta { font-size: 13px; opacity: 0.5; margin-top: 40px; }
  .kpi-strip { display:grid; grid-template-columns: repeat(4,1fr); gap:0; border-bottom: 2px solid #e2e8f0; }
  .kpi { padding: 24px; border-right: 1px solid #e2e8f0; }
  .kpi:last-child { border-right: none; }
  .kpi-val { font-size: 32px; font-weight: 800; color: #1e3a5f; }
  .kpi-lbl { font-size: 11px; color: #94a3b8; text-transform: uppercase; letter-spacing: 1px; margin-top: 4px; }
  .section { padding: 40px; }
  .section-title { font-size: 18px; font-weight: 700; color: #1e3a5f; margin-bottom: 24px; padding-bottom: 8px; border-bottom: 2px solid #e2e8f0; }
  .chart-wrap { position: relative; height: 300px; margin-bottom: 40px; }
  table { width: 100%; border-collapse: collapse; font-size: 13px; }
  thead tr { background: #1e3a5f; color: white; }
  thead th { padding: 10px 14px; text-align: left; font-weight: 600; }
  tbody tr:nth-child(even) { background: #f8fafc; }
  tbody tr:hover { background: #eff6ff; }
  td { padding: 9px 14px; border-bottom: 1px solid #e2e8f0; }
  .score { font-weight: 700; }
  .score-high { color: #16a34a; }
  .score-med { color: #d97706; }
  .score-low { color: #dc2626; }
  .bar-cell { display: flex; align-items: center; gap: 8px; }
  .bar-bg { flex:1; height: 8px; background: #e2e8f0; border-radius: 4px; overflow: hidden; }
  .bar-fill { height: 100%; border-radius: 4px; }
  .cat-badge { display:inline-block; font-size:10px; padding: 2px 8px; border-radius: 100px; background: #eff6ff; color: #1e3a5f; }
  .footer { padding: 24px 40px; border-top: 1px solid #e2e8f0; display:flex; justify-content:space-between; font-size:11px; color: #94a3b8; }
</style>
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
</head>
<body>

<div class="cover">
  <div>
    <div class="cover-title">{{SurveyTitle}}</div>
    <div class="cover-sub">{{CompanyName}}</div>
  </div>
  <div class="cover-meta">Generated {{GeneratedAt}} {{#if StartDate}}· Period: {{StartDate}} – {{EndDate}}{{/if}}</div>
</div>

<div class="kpi-strip">
  <div class="kpi">
    <div class="kpi-val">{{QuestionCount}}</div>
    <div class="kpi-lbl">Questions</div>
  </div>
  <div class="kpi">
    <div class="kpi-val">{{TotalResponses}}</div>
    <div class="kpi-lbl">Responses</div>
  </div>
  <div class="kpi">
    <div class="kpi-val">{{OverallAverage}}</div>
    <div class="kpi-lbl">Overall Average</div>
  </div>
  <div class="kpi">
    <div class="kpi-val">{{CategoryCount}}</div>
    <div class="kpi-lbl">Categories</div>
  </div>
</div>

<div class="section">
  <div class="section-title">Average Score per Question</div>
  <div class="chart-wrap">
    <canvas id="barChart"></canvas>
  </div>
</div>

{{#if Trends}}
<div class="section">
  <div class="section-title">Monthly Trend</div>
  <div class="chart-wrap">
    <canvas id="lineChart"></canvas>
  </div>
</div>
{{/if}}

<div class="section">
  <div class="section-title">Question Details</div>
  <table>
    <thead>
      <tr>
        <th style="width:40%">Question</th>
        <th>Category</th>
        <th>Average</th>
        <th>Responses</th>
        <th style="width:20%">Score</th>
      </tr>
    </thead>
    <tbody>
      {{#each QuestionSummaries}}
      <tr>
        <td>{{Text}}</td>
        <td><span class="cat-badge">{{Category}}</span></td>
        <td class="score {{#if (gt AverageValue 4)}}score-high{{else}}{{#if (gt AverageValue 3)}}score-med{{else}}score-low{{/if}}{{/if}}">
          {{AverageValue}}
        </td>
        <td>{{TotalResponses}}</td>
        <td>
          <div class="bar-cell">
            <div class="bar-bg">
              <div class="bar-fill" style="width:{{AverageValue20Pct}}%; background:{{#if (gt AverageValue 4)}}#16a34a{{else}}{{#if (gt AverageValue 3)}}#d97706{{else}}#dc2626{{/if}}{{/if}}"></div>
            </div>
          </div>
        </td>
      </tr>
      {{/each}}
    </tbody>
  </table>
</div>

<div class="footer">
  <span>{{CompanyName}} · {{SurveyTitle}}</span>
  <span>Confidential — {{GeneratedAt}}</span>
</div>

<script>
  // Bar chart - average per question
  const barLabels = [{{#each QuestionSummaries}}'Q{{@index}}'{{#unless @last}},{{/unless}}{{/each}}];
  const barData   = [{{#each QuestionSummaries}}{{AverageValue}}{{#unless @last}},{{/unless}}{{/each}}];
  const barColors = barData.map(v => v >= 4 ? '#16a34a' : v >= 3 ? '#d97706' : '#dc2626');

  new Chart(document.getElementById('barChart'), {
    type: 'bar',
    data: {
      labels: barLabels,
      datasets: [{ data: barData, backgroundColor: barColors, borderRadius: 4, borderSkipped: false }]
    },
    options: {
      responsive: true, maintainAspectRatio: false,
      plugins: { legend: { display: false } },
      scales: {
        y: { min:0, max:5, ticks: { stepSize:1 }, grid: { color: '#f1f5f9' } },
        x: { grid: { display: false } }
      }
    }
  });

  {{#if Trends}}
  const lineLabels = [{{#each Trends}}'{{MonthName}} {{Year}}'{{#unless @last}},{{/unless}}{{/each}}];
  const lineData   = [{{#each Trends}}{{AverageValue}}{{#unless @last}},{{/unless}}{{/each}}];

  new Chart(document.getElementById('lineChart'), {
    type: 'line',
    data: {
      labels: lineLabels,
      datasets: [{
        data: lineData, label: 'Average',
        borderColor: '#1e3a5f', backgroundColor: 'rgba(30,58,95,0.08)',
        fill: true, tension: 0.4, pointRadius: 5, pointBackgroundColor: '#1e3a5f'
      }]
    },
    options: {
      responsive: true, maintainAspectRatio: false,
      plugins: { legend: { display: false } },
      scales: {
        y: { min:0, max:5, grid: { color: '#f1f5f9' } },
        x: { grid: { display: false } }
      }
    }
  });
  {{/if}}
</script>
</body>
</html>`,

  /** Executive summary template - dark theme, radar chart */
  jsreportExecutive: `<!DOCTYPE html>
<html lang="sv">
<head>
<meta charset="UTF-8">
<style>
  * { margin:0; padding:0; box-sizing:border-box; }
  body { font-family: 'Segoe UI', sans-serif; background: #0f172a; color: #e2e8f0; }
  .hero { background: linear-gradient(135deg, #1e3a5f 0%, #0a0f1e 100%); padding: 56px; }
  .hero-badge { font-size:10px; text-transform:uppercase; letter-spacing:3px; color:#60a5fa; margin-bottom:12px; }
  .hero-title { font-size:42px; font-weight:800; color:white; letter-spacing:-1px; margin-bottom:8px; }
  .hero-company { font-size:16px; color:rgba(255,255,255,0.5); }
  .hero-meta { margin-top:32px; font-size:12px; color:rgba(255,255,255,0.3); }
  .metrics { display:grid; grid-template-columns:repeat(3,1fr); gap:1px; background:#1e293b; }
  .metric { padding:32px; background:#0f172a; }
  .metric-val { font-size:40px; font-weight:800; color:#60a5fa; }
  .metric-lbl { font-size:12px; color:#475569; text-transform:uppercase; letter-spacing:1px; margin-top:6px; }
  .body-grid { display:grid; grid-template-columns:1fr 1fr; gap:1px; background:#1e293b; }
  .panel { background:#0f172a; padding:32px; }
  .panel-title { font-size:14px; font-weight:700; color:#94a3b8; text-transform:uppercase; letter-spacing:1px; margin-bottom:20px; }
  .chart-area { height:280px; position:relative; }
  .score-list { display:flex; flex-direction:column; gap:10px; }
  .score-row { display:flex; align-items:center; gap:12px; }
  .score-label { font-size:12px; color:#94a3b8; flex:1; white-space:nowrap; overflow:hidden; text-overflow:ellipsis; }
  .score-bar { width:100px; height:6px; background:#1e293b; border-radius:3px; overflow:hidden; }
  .score-bar-fill { height:100%; border-radius:3px; }
  .score-num { font-size:13px; font-weight:700; width:32px; text-align:right; }
  .footer { padding:24px 32px; border-top:1px solid #1e293b; font-size:11px; color:#334155; display:flex; justify-content:space-between; }
</style>
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
</head>
<body>
<div class="hero">
  <div class="hero-badge">Executive Report</div>
  <div class="hero-title">{{SurveyTitle}}</div>
  <div class="hero-company">{{CompanyName}}</div>
  <div class="hero-meta">{{GeneratedAt}}</div>
</div>
<div class="metrics">
  <div class="metric"><div class="metric-val">{{OverallAverage}}</div><div class="metric-lbl">Overall Score</div></div>
  <div class="metric"><div class="metric-val">{{QuestionCount}}</div><div class="metric-lbl">Questions</div></div>
  <div class="metric"><div class="metric-val">{{TotalResponses}}</div><div class="metric-lbl">Total Responses</div></div>
</div>
<div class="body-grid">
  <div class="panel">
    <div class="panel-title">Category Radar</div>
    <div class="chart-area"><canvas id="radar"></canvas></div>
  </div>
  <div class="panel">
    <div class="panel-title">Top & Bottom Questions</div>
    <div class="score-list">
      {{#each QuestionSummaries}}
      <div class="score-row">
        <div class="score-label" title="{{Text}}">{{Text}}</div>
        <div class="score-bar">
          <div class="score-bar-fill" style="width:{{AverageValue20Pct}}%;background:{{#if (gt AverageValue 4)}}#22c55e{{else}}{{#if (gt AverageValue 3)}}#f59e0b{{else}}#ef4444{{/if}}{{/if}}"></div>
        </div>
        <div class="score-num" style="color:{{#if (gt AverageValue 4)}}#22c55e{{else}}{{#if (gt AverageValue 3)}}#f59e0b{{else}}#ef4444{{/if}}{{/if}}">{{AverageValue}}</div>
      </div>
      {{/each}}
    </div>
  </div>
</div>
<div class="footer">
  <span>{{CompanyName}}</span><span>Confidential · {{GeneratedAt}}</span>
</div>
<script>
  const cats = [...new Set([{{#each QuestionSummaries}}'{{Category}}'{{#unless @last}},{{/unless}}{{/each}}])];
  const catAvgs = cats.map(cat => {
    const qs = [{{#each QuestionSummaries}}{cat:'{{Category}}',val:{{AverageValue}}}{{#unless @last}},{{/unless}}{{/each}}].filter(q=>q.cat===cat);
    return qs.length ? qs.reduce((s,q)=>s+q.val,0)/qs.length : 0;
  });
  new Chart(document.getElementById('radar'),{
    type:'radar',
    data:{ labels:cats, datasets:[{ data:catAvgs, backgroundColor:'rgba(96,165,250,0.15)', borderColor:'#60a5fa', pointBackgroundColor:'#60a5fa', borderWidth:2, pointRadius:4 }] },
    options:{ responsive:true, maintainAspectRatio:false, plugins:{legend:{display:false}}, scales:{r:{min:0,max:5,ticks:{color:'#334155',stepSize:1,font:{size:8}},grid:{color:'#1e293b'},pointLabels:{color:'#94a3b8',font:{size:10}}}} }
  });
</script>
</body>
</html>`,

  /** Playwright template - simple token replacement, no Handlebars logic */
  playwrightSimple: `<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<style>
  * { margin:0; padding:0; box-sizing:border-box; }
  body { font-family: 'Segoe UI', sans-serif; padding: 40px; color: #1e293b; }
  h1 { font-size: 28px; font-weight: 800; color: #0f2d4a; margin-bottom: 4px; }
  .sub { color: #64748b; margin-bottom: 32px; font-size: 14px; }
  .meta { font-size: 12px; color: #94a3b8; margin-bottom: 40px; }
  .note { padding: 16px; background: #fffbeb; border: 1px solid #fcd34d; border-radius: 8px; font-size: 12px; color: #92400e; margin-top: 32px; }
</style>
</head>
<body>
  <h1>{{SurveyTitle}}</h1>
  <div class="sub">{{CompanyName}}</div>
  <div class="meta">Generated: {{GeneratedAt}}</div>

  {{QuestionsTableHtml}}

  <div class="note">
    ℹ This PDF was generated by Playwright with token replacement only.
    For Chart.js visualizations and Handlebars logic, use jsreport (Hybrid A).
  </div>
</body>
</html>`,
};

/** Build a rich jsreport template with real data pre-rendered for comparison */
export function buildRichTemplateWithData(data: {
  surveyTitle: string;
  companyName: string;
  questions: Array<{ text: string; category: string; avg: number; responses: number }>;
}): string {
  const rows = data.questions.map((q, i) => {
    const pct = Math.round(q.avg * 20);
    const color = q.avg >= 4 ? "#16a34a" : q.avg >= 3 ? "#d97706" : "#dc2626";
    return `<tr>
      <td>Q${i + 1}: ${q.text.substring(0, 60)}</td>
      <td><span style="background:#eff6ff;color:#1e3a5f;padding:2px 8px;border-radius:100px;font-size:10px">${q.category}</span></td>
      <td style="font-weight:700;color:${color}">${q.avg.toFixed(2)}</td>
      <td>${q.responses}</td>
    </tr>`;
  }).join("");

  const labels = data.questions.map((_, i) => `"Q${i + 1}"`).join(",");
  const values = data.questions.map(q => q.avg.toFixed(2)).join(",");
  const colors = data.questions.map(q =>
    q.avg >= 4 ? '"#16a34a"' : q.avg >= 3 ? '"#d97706"' : '"#dc2626"'
  ).join(",");

  return `<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<style>
  * { margin:0;padding:0;box-sizing:border-box; }
  body { font-family:'Segoe UI',sans-serif;background:#fff;color:#1e293b; }
  .cover{background:linear-gradient(135deg,#1e3a5f,#0f2d4a);color:white;padding:48px;margin-bottom:0}
  .cover h1{font-size:32px;font-weight:800;margin-bottom:4px}
  .cover p{opacity:.6;font-size:14px}
  .section{padding:32px}
  .title{font-size:16px;font-weight:700;color:#1e3a5f;margin-bottom:20px;padding-bottom:8px;border-bottom:2px solid #e2e8f0}
  .chart-wrap{height:260px;position:relative;margin-bottom:32px}
  table{width:100%;border-collapse:collapse;font-size:12px}
  thead{background:#1e3a5f;color:white}
  th{padding:10px 12px;text-align:left;font-weight:600}
  td{padding:8px 12px;border-bottom:1px solid #e2e8f0}
  tr:nth-child(even){background:#f8fafc}
</style>
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
</head>
<body>
<div class="cover">
  <h1>${data.surveyTitle}</h1>
  <p>${data.companyName}</p>
</div>
<div class="section">
  <div class="title">Average Score per Question</div>
  <div class="chart-wrap"><canvas id="c"></canvas></div>
  <table>
    <thead><tr><th>Question</th><th>Category</th><th>Average</th><th>Responses</th></tr></thead>
    <tbody>${rows}</tbody>
  </table>
</div>
<script>
new Chart(document.getElementById('c'),{
  type:'bar',
  data:{labels:[${labels}],datasets:[{data:[${values}],backgroundColor:[${colors}],borderRadius:4}]},
  options:{responsive:true,maintainAspectRatio:false,plugins:{legend:{display:false}},scales:{y:{min:0,max:5,grid:{color:'#f1f5f9'}},x:{grid:{display:false}}}}
});
</script>
</body>
</html>`;
}
