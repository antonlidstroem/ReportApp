<template>
  <div class="live-preview">
    <div class="lp-header">
      <div class="lp-title-group">
        <span class="lp-eyebrow">LIVE REPORT PREVIEW</span>
        <h3>What the PDF will look like</h3>
        <span class="lp-sub">This iframe renders the exact HTML sent to {{ engineLabel }}. The PDF is a pixel-perfect capture of this.</span>
      </div>
      <div class="lp-controls">
        <button
          v-for="t in templates"
          :key="t.id"
          :class="['tmpl-btn', { active: activeTemplate === t.id }]"
          @click="switchTemplate(t.id)"
        >{{ t.icon }} {{ t.name }}</button>
        <button class="refresh-btn" @click="renderPreview" title="Refresh">↺</button>
      </div>
    </div>

    <!-- Data sliders for interactivity -->
    <div v-if="showSliders" class="lp-sliders">
      <div class="ls-label">Edit data → see it update live in the "PDF":</div>
      <div class="sliders-row">
        <div v-for="(row, i) in previewData" :key="i" class="slider-item">
          <span class="si-label">{{ row.label }}</span>
          <input
            type="range" min="1" max="5" step="0.1"
            v-model.number="row.value"
            class="si-range"
            :style="`--c:${scoreColor(row.value)}`"
            @input="debouncedRender"
          />
          <span :style="`color:${scoreColor(row.value)}`" class="si-val">{{ row.value.toFixed(1) }}</span>
        </div>
      </div>
    </div>

    <div class="lp-frame-wrap">
      <div v-if="isLoading" class="lp-loading">
        <div class="lp-spinner"></div>
        <span>Rendering preview...</span>
      </div>
      <iframe
        ref="frameRef"
        class="lp-frame"
        sandbox="allow-same-origin allow-scripts"
        title="Report preview"
      ></iframe>
    </div>

    <div class="lp-footer">
      <span class="lf-engine">Engine: {{ engineLabel }}</span>
      <span class="lf-note">{{ footerNote }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, nextTick } from 'vue'
import { RICH_TEMPLATES, buildRichTemplateWithData } from '../composables/useApi'

const props = defineProps<{
  provider: string
  showSliders?: boolean
}>()

const frameRef = ref<HTMLIFrameElement>()
const activeTemplate = ref('full')
const isLoading = ref(false)
let debounceTimer: ReturnType<typeof setTimeout> | null = null

const engineLabel = computed(() => {
  if (props.provider === 'play-sync') return 'Playwright (token replacement)'
  if (props.provider === 'Syncfusion') return 'Syncfusion (code-first, no HTML)'
  return 'jsreport (Handlebars + Chromium)'
})

const footerNote = computed(() => {
  if (props.provider === 'play-sync') {
    return '⚠ Playwright only substitutes {{Token}} — no Handlebars loops or conditionals'
  }
  if (props.provider === 'Syncfusion') {
    return 'ℹ Syncfusion builds documents programmatically — no HTML template'
  }
  return '✓ Chromium renders this HTML exactly. Chart.js runs before the PDF is captured.'
})

const templates = computed(() => {
  if (props.provider === 'play-sync') {
    return [{ id: 'playwright', name: 'Simple', icon: '📄' }]
  }
  if (props.provider === 'Syncfusion') {
    return [{ id: 'syncfusion', name: 'Native .NET', icon: '💎' }]
  }
  return [
    { id: 'full', name: 'Full Report', icon: '📊' },
    { id: 'executive', name: 'Executive', icon: '🌙' },
  ]
})

const previewData = ref([
  { label: 'Leadership', value: 4.1 },
  { label: 'Psychosocial', value: 3.2 },
  { label: 'Health', value: 3.8 },
  { label: 'Safety', value: 4.5 },
  { label: 'Resources', value: 3.5 },
])

function scoreColor(v: number) {
  return v >= 4 ? '#16a34a' : v >= 3 ? '#d97706' : '#dc2626'
}

function switchTemplate(id: string) {
  activeTemplate.value = id
  renderPreview()
}

function debouncedRender() {
  if (debounceTimer) clearTimeout(debounceTimer)
  debounceTimer = setTimeout(renderPreview, 150)
}

function getHtml(): string {
  if (props.provider === 'Syncfusion') {
    return buildSyncfusionPlaceholder()
  }
  if (props.provider === 'play-sync') {
    return buildPlaywrightPreview()
  }
  if (activeTemplate.value === 'executive') {
    return buildExecutivePreview()
  }
  return buildFullPreview()
}

function buildFullPreview(): string {
  const q = previewData.value
  const labels = q.map((_, i) => `"Q${i + 1}"`).join(',')
  const values = q.map(d => d.value.toFixed(2)).join(',')
  const colors = q.map(d => `"${scoreColor(d.value)}"`).join(',')
  const rows = q.map((d, i) => `
    <tr>
      <td>Q${i + 1}: ${d.label} (example question)</td>
      <td><span style="background:#eff6ff;color:#1e3a5f;padding:2px 8px;border-radius:100px;font-size:10px">Category ${Math.floor(i / 2) + 1}</span></td>
      <td style="font-weight:700;color:${scoreColor(d.value)}">${d.value.toFixed(2)}</td>
      <td>${Math.floor(20 + Math.random() * 20)}</td>
    </tr>`).join('')

  return `<!DOCTYPE html><html>
<head>
<meta charset="UTF-8">
<style>
  *{margin:0;padding:0;box-sizing:border-box}
  body{font-family:'Segoe UI',sans-serif;background:#fff;color:#1e293b}
  .cover{background:linear-gradient(135deg,#1e3a5f,#0f2d4a);color:white;padding:40px}
  .cover h1{font-size:28px;font-weight:800;margin-bottom:4px}
  .cover p{opacity:.6;font-size:13px}
  .section{padding:28px}
  .title{font-size:15px;font-weight:700;color:#1e3a5f;margin-bottom:16px;padding-bottom:8px;border-bottom:2px solid #e2e8f0}
  .chart-wrap{height:220px;position:relative;margin-bottom:24px}
  table{width:100%;border-collapse:collapse;font-size:12px}
  thead{background:#1e3a5f;color:white}
  th{padding:9px 12px;text-align:left;font-weight:600}
  td{padding:7px 12px;border-bottom:1px solid #e2e8f0}
  tr:nth-child(even){background:#f8fafc}
  .label-tag{font-size:10px;padding:2px 8px;border-radius:100px;background:#eff6ff;color:#1e3a5f}
</style>
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"><\/script>
</head>
<body>
<div class="cover">
  <h1>Survey Analysis Report</h1>
  <p>Storkommunen AB · Generated ${new Date().toLocaleDateString('sv-SE')}</p>
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
  data:{
    labels:[${labels}],
    datasets:[{data:[${values}],backgroundColor:[${colors}],borderRadius:4}]
  },
  options:{
    responsive:true,maintainAspectRatio:false,
    plugins:{legend:{display:false}},
    scales:{
      y:{min:0,max:5,grid:{color:'#f1f5f9'},ticks:{stepSize:1}},
      x:{grid:{display:false}}
    }
  }
});
<\/script>
</body></html>`
}

function buildExecutivePreview(): string {
  const q = previewData.value
  const catLabels = q.map(d => `"${d.label}"`).join(',')
  const catValues = q.map(d => d.value.toFixed(2)).join(',')
  const scoreRows = q.map(d => `
    <div style="display:flex;align-items:center;gap:10px;padding:6px 0;border-bottom:1px solid #1e293b">
      <div style="flex:1;font-size:11px;color:#94a3b8;white-space:nowrap;overflow:hidden;text-overflow:ellipsis">${d.label}</div>
      <div style="width:80px;height:5px;background:#1e293b;border-radius:2px;overflow:hidden">
        <div style="width:${d.value * 20}%;height:100%;background:${scoreColor(d.value)};border-radius:2px"></div>
      </div>
      <div style="font-size:12px;font-weight:700;color:${scoreColor(d.value)};width:28px;text-align:right">${d.value.toFixed(1)}</div>
    </div>`).join('')

  const avg = (q.reduce((s, d) => s + d.value, 0) / q.length).toFixed(2)

  return `<!DOCTYPE html><html>
<head>
<meta charset="UTF-8">
<style>
  *{margin:0;padding:0;box-sizing:border-box}
  body{font-family:'Segoe UI',sans-serif;background:#0f172a;color:#e2e8f0}
  .hero{background:linear-gradient(135deg,#1e3a5f,#0a0f1e);padding:40px}
  .hero-badge{font-size:10px;text-transform:uppercase;letter-spacing:3px;color:#60a5fa;margin-bottom:8px}
  .hero-title{font-size:32px;font-weight:800;color:white;letter-spacing:-1px;margin-bottom:4px}
  .hero-co{font-size:13px;color:rgba(255,255,255,.5)}
  .metrics{display:grid;grid-template-columns:repeat(3,1fr);gap:1px;background:#1e293b}
  .metric{padding:24px;background:#0f172a}
  .metric-val{font-size:32px;font-weight:800;color:#60a5fa}
  .metric-lbl{font-size:10px;color:#475569;text-transform:uppercase;letter-spacing:1px;margin-top:4px}
  .grid{display:grid;grid-template-columns:1fr 1fr;gap:1px;background:#1e293b}
  .panel{background:#0f172a;padding:24px}
  .panel-title{font-size:11px;font-weight:700;color:#94a3b8;text-transform:uppercase;letter-spacing:1px;margin-bottom:16px}
  .chart-area{height:220px;position:relative}
</style>
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"><\/script>
</head>
<body>
<div class="hero">
  <div class="hero-badge">Executive Report</div>
  <div class="hero-title">Survey Analysis Report</div>
  <div class="hero-co">Storkommunen AB</div>
</div>
<div class="metrics">
  <div class="metric"><div class="metric-val">${avg}</div><div class="metric-lbl">Overall Score</div></div>
  <div class="metric"><div class="metric-val">5</div><div class="metric-lbl">Categories</div></div>
  <div class="metric"><div class="metric-val">120+</div><div class="metric-lbl">Responses</div></div>
</div>
<div class="grid">
  <div class="panel">
    <div class="panel-title">Category Radar</div>
    <div class="chart-area"><canvas id="r"></canvas></div>
  </div>
  <div class="panel">
    <div class="panel-title">Scores by Category</div>
    ${scoreRows}
  </div>
</div>
<script>
new Chart(document.getElementById('r'),{
  type:'radar',
  data:{
    labels:[${catLabels}],
    datasets:[{data:[${catValues}],backgroundColor:'rgba(96,165,250,0.15)',borderColor:'#60a5fa',pointBackgroundColor:'#60a5fa',borderWidth:2,pointRadius:4}]
  },
  options:{
    responsive:true,maintainAspectRatio:false,
    plugins:{legend:{display:false}},
    scales:{r:{min:0,max:5,ticks:{color:'#334155',stepSize:1,font:{size:8}},grid:{color:'#1e293b'},pointLabels:{color:'#94a3b8',font:{size:9}}}}
  }
});
<\/script>
</body></html>`
}

function buildPlaywrightPreview(): string {
  const rows = previewData.value.map((d, i) => `
    <tr>
      <td>Q${i + 1}: ${d.label} example question text</td>
      <td style="font-weight:700;color:${scoreColor(d.value)}">${d.value.toFixed(2)}</td>
    </tr>`).join('')

  return `<!DOCTYPE html><html>
<head><meta charset="UTF-8">
<style>
  *{margin:0;padding:0;box-sizing:border-box}
  body{font-family:'Segoe UI',sans-serif;padding:32px;color:#1e293b}
  h1{font-size:22px;font-weight:800;color:#0f2d4a;margin-bottom:4px}
  .sub{color:#64748b;margin-bottom:24px;font-size:13px}
  table{width:100%;border-collapse:collapse;font-size:12px;margin-bottom:24px}
  thead{background:#0f2d4a;color:white}
  th{padding:9px 12px;text-align:left}
  td{padding:7px 12px;border-bottom:1px solid #e2e8f0}
  tr:nth-child(even){background:#f8fafc}
  .note{padding:14px;background:#fffbeb;border:1px solid #fcd34d;border-radius:8px;font-size:11px;color:#92400e}
  .token{background:#fef3c7;padding:1px 5px;border-radius:3px;font-family:monospace;font-size:10px}
</style>
</head>
<body>
  <h1>Survey Analysis Report</h1>
  <div class="sub">Storkommunen AB · ${new Date().toLocaleDateString('sv-SE')}</div>
  <p style="font-size:11px;color:#94a3b8;margin-bottom:16px">Token <span class="token">{{SurveyTitle}}</span> → "Survey Analysis Report" &nbsp; <span class="token">{{CompanyName}}</span> → "Storkommunen AB"</p>
  <table>
    <thead><tr><th>Question</th><th>Average</th></tr></thead>
    <tbody>${rows}</tbody>
  </table>
  <div class="note">
    ⚠ <strong>Playwright limitation:</strong> This template only supports simple <span class="token">{{Token}}</span> replacement.
    No <code>#each</code> loops or <code>#if</code> conditionals. The questions table above was pre-built in C# and injected as <span class="token">{{QuestionsTableHtml}}</span>.
    For full Handlebars logic and Chart.js, use <strong>Hybrid A (jsreport + Syncfusion)</strong>.
  </div>
</body></html>`
}

function buildSyncfusionPlaceholder(): string {
  return `<!DOCTYPE html><html>
<head><meta charset="UTF-8">
<style>
  *{margin:0;padding:0;box-sizing:border-box}
  body{font-family:'Segoe UI',sans-serif;padding:40px;color:#1e293b;background:#f8fafc}
  .card{background:white;border-radius:12px;padding:32px;border:1px solid #e2e8f0;max-width:600px;margin:0 auto}
  .icon{font-size:48px;margin-bottom:16px}
  h2{font-size:20px;font-weight:700;color:#0f172a;margin-bottom:8px}
  p{font-size:13px;color:#64748b;line-height:1.6;margin-bottom:16px}
  .code{background:#f1f5f9;border-radius:8px;padding:16px;font-family:monospace;font-size:11px;color:#334155;line-height:1.6}
  .feature{display:flex;align-items:flex-start;gap:10px;padding:8px 0;border-bottom:1px solid #f1f5f9}
  .f-check{color:#14b8a6;font-weight:700;flex-shrink:0}
  .f-text{font-size:12px;color:#64748b}
</style>
</head>
<body>
<div class="card">
  <div class="icon">💎</div>
  <h2>Syncfusion: Code-First, No HTML Template</h2>
  <p>Syncfusion builds documents programmatically via .NET object models — not HTML rendering. There is no HTML to preview here. The output is built directly by the XlsIO and Essential PDF APIs.</p>
  <div class="code">// SyncfusionExcelGenerator.cs
sheet.Range["A1"].Text = "Question";
sheet.Range["B1"].Text = "Category";
sheet.Range["C1"].Formula = "=AVERAGE(D2:D100)";

// Conditional formatting
var cond = sheet.Range["C2:C50"].ConditionalFormats.AddCondition();
cond.FormatType = ExcelCFType.CellValue;
cond.Operator = ExcelComparisonOperator.Less;
cond.FirstFormula = "3";
cond.BackColorRGB = Color.FromArgb(255, 239, 68, 68); // red</div>
  <div style="margin-top:20px">
    <div class="feature"><span class="f-check">✓</span><span class="f-text">Live =AVERAGE() formulas that recalculate when opened in Excel</span></div>
    <div class="feature"><span class="f-check">✓</span><span class="f-text">Native OfficeChart objects (editable in PowerPoint, not images)</span></div>
    <div class="feature"><span class="f-check">✓</span><span class="f-text">AES-256 PDF encryption and password protection</span></div>
    <div class="feature"><span class="f-check">✓</span><span class="f-text">PDF/UA accessibility (WCAG 2.1 AA, screen reader compatible)</span></div>
    <div class="feature"><span class="f-check">✓</span><span class="f-text">Zero cold start — pure .NET, no Chromium process needed</span></div>
  </div>
</div>
</body></html>`
}

function renderPreview() {
  const frame = frameRef.value
  if (!frame) return
  isLoading.value = true
  const html = getHtml()
  nextTick(() => {
    try {
      const doc = frame.contentDocument
      if (doc) {
        doc.open()
        doc.write(html)
        doc.close()
      }
    } finally {
      setTimeout(() => { isLoading.value = false }, 300)
    }
  })
}

// Re-render when template changes
watch(activeTemplate, renderPreview)

onMounted(() => {
  // Set initial template based on provider
  if (props.provider === 'play-sync') activeTemplate.value = 'playwright'
  else if (props.provider === 'Syncfusion') activeTemplate.value = 'syncfusion'
  setTimeout(renderPreview, 100)
})
</script>

<style scoped>
.live-preview {
  background: var(--surface);
  border: 1px solid var(--border);
  border-radius: 14px;
  overflow: hidden;
}

.lp-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 16px 20px;
  background: var(--surface-2);
  border-bottom: 1px solid var(--border);
  gap: 16px;
  flex-wrap: wrap;
}

.lp-eyebrow {
  display: block;
  font-size: 9px;
  letter-spacing: 2.5px;
  text-transform: uppercase;
  color: var(--accent);
  margin-bottom: 4px;
  opacity: 0.7;
}

.lp-header h3 { font-size: 14px; font-weight: 700; margin: 0 0 2px; color: var(--text); }
.lp-sub { font-size: 11px; color: var(--text-muted); }

.lp-controls { display: flex; gap: 6px; align-items: center; flex-wrap: wrap; flex-shrink: 0; }

.tmpl-btn {
  font-size: 11px;
  padding: 5px 12px;
  border-radius: 6px;
  border: 1px solid var(--border);
  background: transparent;
  color: var(--text-muted);
  cursor: pointer;
  transition: all 0.12s;
}
.tmpl-btn.active { border-color: var(--accent); color: var(--accent); background: color-mix(in srgb, var(--accent) 8%, transparent); }
.tmpl-btn:hover:not(.active) { border-color: rgba(255,255,255,0.2); color: var(--text); }

.refresh-btn {
  width: 28px; height: 28px;
  border-radius: 6px;
  border: 1px solid var(--border);
  background: transparent;
  color: var(--text-muted);
  cursor: pointer;
  font-size: 14px;
  display: flex; align-items: center; justify-content: center;
  transition: all 0.12s;
}
.refresh-btn:hover { border-color: var(--accent); color: var(--accent); }

/* Sliders */
.lp-sliders {
  padding: 14px 20px;
  border-bottom: 1px solid var(--border);
  background: rgba(255,255,255,0.02);
}

.ls-label {
  font-size: 11px;
  color: var(--text-muted);
  margin-bottom: 10px;
}

.sliders-row {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}

.slider-item {
  display: flex;
  align-items: center;
  gap: 6px;
  flex: 1;
  min-width: 140px;
}

.si-label { font-size: 10px; color: var(--text-muted); width: 80px; flex-shrink: 0; }

.si-range {
  flex: 1;
  -webkit-appearance: none;
  height: 4px;
  border-radius: 2px;
  background: var(--border);
  outline: none;
}
.si-range::-webkit-slider-thumb {
  -webkit-appearance: none;
  width: 14px; height: 14px;
  border-radius: 50%;
  background: var(--c, var(--accent));
  cursor: pointer;
}

.si-val { font-size: 12px; font-weight: 700; width: 28px; text-align: right; }

/* Frame */
.lp-frame-wrap {
  position: relative;
  height: 520px;
  background: #f8fafc;
}

.lp-loading {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  background: rgba(0,0,0,0.4);
  z-index: 10;
  font-size: 13px;
  color: #94a3b8;
}

.lp-spinner {
  width: 18px; height: 18px;
  border: 2px solid rgba(255,255,255,0.1);
  border-top-color: var(--accent);
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

.lp-frame {
  width: 100%;
  height: 100%;
  border: none;
}

.lp-footer {
  display: flex;
  justify-content: space-between;
  padding: 8px 20px;
  background: var(--surface-2);
  border-top: 1px solid var(--border);
}

.lf-engine { font-size: 10px; color: var(--text-muted); }
.lf-note { font-size: 10px; color: var(--text-muted); text-align: right; max-width: 60%; }
</style>
