<template>
  <div class="page">
    <div class="hero">
      <div class="hero-bg"></div>
      <div class="hero-content">
        <router-link to="/" class="back">← Dashboard</router-link>
        <div class="eyebrow">Track 2 · Mature Enterprise Platform</div>
        <h1>Syncfusion</h1>
        <div class="tagline">Native Office engine. 20 years of Excel expertise.</div>
        <p>Not a PDF engine that also does Excel — a native Office engine that also does PDF. Live formulas, editable charts, AES-256 encryption, PDF/UA. Built for systems that need more than "it works."</p>
        <div class="verdicts">
          <div class="verdict ok">✓ Best for: Native Office docs with live formulas and editable charts</div>
          <div class="verdict warn">⚠ Avoid if: A designer needs to own the report layout in HTML/CSS</div>
        </div>
        <div class="badges">
          <span class="b teal">★ Live =AVERAGE() formulas</span>
          <span class="b teal">✓ Editable PPT charts</span>
          <span class="b teal">✓ PDF/UA accessibility</span>
          <span class="b teal">✓ Community Free</span>
          <span class="b yellow">⚠ No HTML → PDF</span>
        </div>
      </div>
      <div class="hero-products">
        <div v-for="p in products" :key="p.name" class="prod-card">
          <span class="pc-icon">{{ p.icon }}</span>
          <div class="pc-name">{{ p.name }}</div>
          <div class="pc-desc">{{ p.desc }}</div>
          <div class="pc-stars"><span v-for="i in 5" :key="i" :class="['star', i<=p.stars?'on':'']">★</span></div>
        </div>
      </div>
    </div>

    <!-- FORMULA SANDBOX -->
    <section class="section">
      <div class="eyebrow-sm">CAPABILITY 1 — UNIQUE TO SYNCFUSION</div>
      <h2>Live Excel formulas — the spreadsheet recalculates when opened</h2>
      <p class="sub">XlsIO writes real <code>=AVERAGE()</code> formulas, not static numbers. Click cells, edit values — the formula bar updates. This is exactly what an exported .xlsx file contains.</p>
      <div class="formula-sandbox">
        <div class="fs-sheet">
          <div class="fs-toolbar">
            <span class="fs-app">📗 Excel Simulation</span>
            <span class="fs-fbar">{{ activeFx }}</span>
          </div>
          <div class="fs-head">
            <div class="fsh-cell corner"></div>
            <div class="fsh-cell">A — Category</div>
            <div class="fsh-cell">B — Q1</div>
            <div class="fsh-cell">C — Q2</div>
            <div class="fsh-cell">D — Q3</div>
            <div class="fsh-cell formula-col">E — =AVERAGE(B:D)</div>
          </div>
          <div v-for="(row, i) in fRows" :key="i" class="fs-row" :class="{ selected: selectedRow===i }" @click="selectedRow=i; activeFx=`=AVERAGE(B${i+2}:D${i+2})`">
            <div class="fs-rnum">{{ i+2 }}</div>
            <div class="fs-cell fs-label">{{ row.label }}</div>
            <div class="fs-cell" v-for="k in ['q1','q2','q3']" :key="k">
              <input v-model.number="(row as Record<string,number|string>)[k]" type="number" min="1" max="5" step="0.1" class="fs-input" @focus="activeFx=`=${k==='q1'?'B':k==='q2'?'C':'D'}${i+2}`" />
            </div>
            <div class="fs-cell fs-result" :style="`color:${avgColor(rowAvg(row))}`">
              <span class="fsr-fx">=AVERAGE(B{{ i+2 }}:D{{ i+2 }})</span>
              <span class="fsr-val">{{ rowAvg(row).toFixed(2) }}</span>
            </div>
          </div>
          <div class="fs-row fs-total">
            <div class="fs-rnum">—</div>
            <div class="fs-cell fs-tlabel">=AVERAGE(col)</div>
            <div class="fs-cell" v-for="k in ['q1','q2','q3']" :key="k">
              <span :style="`color:${avgColor(colAvg(k as 'q1'|'q2'|'q3'))}`">{{ colAvg(k as 'q1'|'q2'|'q3').toFixed(2) }}</span>
            </div>
            <div class="fs-cell fs-grand" :style="`color:${avgColor(overallAvg)}`">{{ overallAvg.toFixed(2) }}</div>
          </div>
          <div class="fs-note">↑ Click a row to see its formula. Edit values — =AVERAGE() recalculates live, just like in exported Excel.</div>
        </div>
        <div class="fs-code">
          <div class="code-hdr"><span>SyncfusionExcelGenerator.cs</span><span class="code-lang">C#</span></div>
          <pre class="code-body"><code>{{ formulaCode }}</code></pre>
        </div>
      </div>
    </section>

    <!-- NATIVE CHARTS -->
    <section class="section">
      <div class="eyebrow-sm">CAPABILITY 2 — UNIQUE TO SYNCFUSION</div>
      <h2>Native editable Office charts — not images</h2>
      <p class="sub">Syncfusion creates <code>OfficeChart</code> objects. The recipient opens the PPT or Excel file and can click <strong>"Edit Data"</strong> — change chart type, add series. One enum change in C# switches the chart type.</p>
      <div class="chart-demo">
        <div class="cd-picker">
          <button v-for="ct in nativeChartTypes" :key="ct.id" :class="['nct-btn', { active: activeNCT===ct.id }]" @click="switchNativeChart(ct.id)">
            <span class="nct-icon">{{ ct.icon }}</span>
            <span class="nct-label">{{ ct.label }}</span>
            <code class="nct-code">OfficeChartType.{{ ct.enum }}</code>
          </button>
          <div class="cd-pnote">↑ In C# this is literally one line: <code>chart.ChartType = OfficeChartType.{{ activeNativeChartType?.enum }}</code></div>
        </div>
        <div class="cd-canvas-wrap">
          <canvas ref="nativeChartRef" height="240"></canvas>
          <div class="cd-edit-bar">
            <span>⋮⋮ PowerPoint — Chart Design</span>
            <div class="cdb-btns"><span>Edit Data</span><span>Change Chart Type</span><span>Select Data</span></div>
          </div>
        </div>
        <div class="fs-code">
          <div class="code-hdr"><span>SyncfusionPptGenerator.cs</span><span class="code-lang">C#</span></div>
          <pre class="code-body"><code>{{ nativeChartCode }}</code></pre>
        </div>
      </div>
    </section>

    <!-- PREVIEW (placeholder showing what code-first produces) -->
    <section class="section">
      <div class="eyebrow-sm">CAPABILITY 3</div>
      <h2>What Syncfusion actually builds — code-first, no HTML</h2>
      <p class="sub">Unlike jsreport, there is no HTML template. The document structure is defined entirely in C# object models. The preview below shows what the approach looks like.</p>
      <LiveReportPreview provider="Syncfusion" :show-sliders="false" />
    </section>

    <!-- ENTERPRISE SANDBOX -->
    <section class="section">
      <div class="eyebrow-sm">CAPABILITY 4</div>
      <h2>Enterprise features — toggle to activate on next export</h2>
      <div class="sandbox-grid">
        <div v-for="f in sandboxFeatures" :key="f.id" class="sc" :class="{ active: (config as Record<string,boolean>)[f.id] }" @click="(config as Record<string,boolean>)[f.id] = !(config as Record<string,boolean>)[f.id]">
          <div class="sc-top"><span class="sc-icon">{{ f.icon }}</span><div class="sc-toggle" :class="{ on: (config as Record<string,boolean>)[f.id] }"><div class="sc-thumb"></div></div></div>
          <div class="sc-title">{{ f.title }}</div>
          <div class="sc-desc">{{ f.desc }}</div>
          <div class="sc-snippet"><code>{{ f.snippet }}</code></div>
          <div v-if="(config as Record<string,boolean>)[f.id]" class="sc-badge">✓ Active on next export</div>
        </div>
      </div>
    </section>

    <!-- SW -->
    <section class="section">
      <div class="eyebrow-sm">HONEST ASSESSMENT</div>
      <h2>The real Syncfusion tradeoffs</h2>
      <div class="sw-grid">
        <div class="sw-col sw-teal"><h3>💎 Strengths</h3><div v-for="s in strengths" :key="s.t" class="sw-item"><div class="sw-t">{{ s.t }}</div><div class="sw-d">{{ s.d }}</div></div></div>
        <div class="sw-col sw-yellow"><h3>⚠ Weaknesses</h3><div v-for="w in weaknesses" :key="w.t" class="sw-item"><div class="sw-t">{{ w.t }}</div><div class="sw-d">{{ w.d }}</div></div></div>
      </div>
    </section>

    <StressTestPanel provider="Syncfusion" provider-color="#14b8a6" />

    <section class="section">
      <div class="eyebrow-sm">EXPORT WORKSPACE</div>
      <h2>Test Syncfusion exports</h2>
      <p class="sub">Excel exports contain live <code>=AVERAGE()</code> formulas. PowerPoint exports contain editable OfficeChart objects. Try opening the files in Microsoft Office.</p>
      <div class="workspace">
        <div class="ws-left"><SurveyPicker /><ModuleList /></div>
        <div class="ws-right"><ExportButton provider="Syncfusion" :supports-html-template="false" /><TemplateDesigner provider-name="Syncfusion" :supports-html-template="false" /></div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import SurveyPicker from '../../components/SurveyPicker.vue'
import ModuleList from '../../components/ModuleList.vue'
import TemplateDesigner from '../../components/TemplateDesigner.vue'
import ExportButton from '../../components/ExportButton.vue'
import StressTestPanel from '../../components/StressTestPanel.vue'
import LiveReportPreview from '../../components/LiveReportPreview.vue'
import Chart from 'chart.js/auto'

const nativeChartRef = ref<HTMLCanvasElement>()
let chartInst: Chart | null = null
const selectedRow = ref(0)
const activeFx = ref('=AVERAGE(B2:D2)')

const products = [
  { icon: '📊', name: 'XlsIO', desc: 'Excel engine — 400+ live formulas', stars: 5 },
  { icon: '📄', name: 'Essential PDF', desc: 'AES-256 encryption & PDF/UA', stars: 4 },
  { icon: '📋', name: 'Presentation', desc: 'Native editable PPT charts', stars: 4 },
]

const fRows = ref([
  { label: 'Ledarskap', q1: 4.2, q2: 3.8, q3: 4.5 },
  { label: 'Psykosocialt', q1: 3.1, q2: 3.4, q3: 2.9 },
  { label: 'Hälsa', q1: 3.8, q2: 4.0, q3: 3.6 },
  { label: 'Säkerhet', q1: 4.5, q2: 4.2, q3: 4.8 },
])

const rowAvg = (r: { q1: number; q2: number; q3: number }) => (r.q1 + r.q2 + r.q3) / 3
const colAvg = (k: 'q1' | 'q2' | 'q3') => fRows.value.reduce((s, r) => s + r[k], 0) / fRows.value.length
const overallAvg = computed(() => fRows.value.reduce((s, r) => s + r.q1 + r.q2 + r.q3, 0) / (fRows.value.length * 3))
const avgColor = (v: number) => v >= 4 ? '#22c55e' : v >= 3 ? '#f59e0b' : '#ef4444'

const formulaCode = `// Write helper value column (J)
sheet.Range[r, 10].Number = q.AverageValue;

// Write real FORMULA in column C
sheet.Range[r, 3].Formula = $"=AVERAGE(B{r}:D{r})";

// Conditional formatting — red if < 3
var cond = sheet.Range[r, 3]
    .ConditionalFormats.AddCondition();
cond.FormatType = ExcelCFType.CellValue;
cond.Operator = ExcelComparisonOperator.Less;
cond.FirstFormula = "3";
cond.BackColorRGB = Color.FromArgb(255, 239, 68, 68);

// Auto-fit all columns
sheet.UsedRange.AutofitColumns();`

const activeNCT = ref('bar')
const nativeChartTypes = [
  { id: 'bar', icon: '📊', label: 'Clustered Bar', enum: 'Column_Clustered' },
  { id: 'line', icon: '📈', label: 'Line', enum: 'Line' },
  { id: 'radar', icon: '🎯', label: 'Radar', enum: 'Radar' },
  { id: 'doughnut', icon: '🍩', label: 'Doughnut', enum: 'Doughnut' },
]
const activeNativeChartType = computed(() => nativeChartTypes.find(t => t.id === activeNCT.value))

const nativeChartCode = computed(() => `// Creates an EDITABLE chart (not image!)
IPresentationChart chart =
    slide.Shapes.AddChart(100, 100, 600, 400);

// One line to change chart type:
chart.ChartType =
    OfficeChartType.${activeNativeChartType.value?.enum ?? 'Column_Clustered'};

for (int i = 0; i < data.Count; i++) {
    chart.ChartData.SetValue(i + 2, 1, data[i].Label);
    chart.ChartData.SetValue(i + 2, 2, data[i].Value);
}
// Recipient can click "Edit Data" in PowerPoint!`)

function switchNativeChart(id: string) {
  activeNCT.value = id
  buildNativeChart()
}

const chartLabels = ['Ledarskap', 'Psykosocialt', 'Hälsa', 'Säkerhet', 'Resurser']
const chartValues = [4.1, 3.2, 3.8, 4.5, 3.5]

function buildNativeChart() {
  if (!nativeChartRef.value) return
  if (chartInst) chartInst.destroy()
  const teal = '#14b8a6'
  const isRound = activeNCT.value === 'doughnut'
  chartInst = new Chart(nativeChartRef.value, {
    type: activeNCT.value as 'bar' | 'line' | 'radar' | 'doughnut',
    data: {
      labels: chartLabels,
      datasets: [{
        data: chartValues,
        backgroundColor: isRound ? ['#14b8a6','#0d9488','#0f766e','#115e59','#134e4a'] : 'rgba(20,184,166,0.65)',
        borderColor: isRound ? 'transparent' : teal,
        borderWidth: 2,
        borderRadius: activeNCT.value === 'bar' ? 4 : undefined,
        fill: activeNCT.value === 'line' ? true : undefined,
        tension: 0.4,
        pointBackgroundColor: teal,
      }],
    },
    options: {
      animation: { duration: 350 },
      plugins: { legend: { display: isRound, labels: { color: '#94a3b8', font: { size: 10 } } } },
      scales: activeNCT.value === 'radar'
        ? { r: { min: 0, max: 5, ticks: { color: '#666', stepSize: 1, font: { size: 8 } }, grid: { color: '#1e293b' }, pointLabels: { color: '#94a3b8', font: { size: 9 } } } }
        : isRound ? {}
        : { y: { min: 0, max: 5, ticks: { color: '#888', font: { size: 10 } }, grid: { color: '#1e2a3a' } }, x: { ticks: { color: '#888', font: { size: 10 } }, grid: { color: '#1e2a3a' } } },
    } as import('chart.js').ChartOptions,
  })
}

const config = ref({ formulas: true, encrypt: false, nativeCharts: true, accessibility: false, watermark: false })
const sandboxFeatures = [
  { id: 'formulas', icon: '🧮', title: 'Live Excel Formulas', desc: '=AVERAGE() instead of static numbers. Change data in Excel and it recalculates.', snippet: 'sheet.Range[r,3].Formula = $"=AVERAGE(B{r}:D{r})";' },
  { id: 'encrypt', icon: '🔐', title: 'AES-256 PDF Encryption', desc: 'Password-protect the report at generation time. Password: 1234', snippet: 'doc.Security.KeySize = Key256Bit;' },
  { id: 'nativeCharts', icon: '📊', title: 'Editable PPT Charts', desc: 'OfficeChart objects — not images. Right-click "Edit Data" in PowerPoint.', snippet: 'chart.ChartType = OfficeChartType.Column_Clustered;' },
  { id: 'accessibility', icon: '♿', title: 'PDF/UA Accessibility', desc: 'Tagged PDF, WCAG 2.1 AA, screen reader compatible.', snippet: 'doc.ViewerPreferences.DisplayDocTitle = true;' },
  { id: 'watermark', icon: '💧', title: 'Confidential Watermark', desc: 'Diagonal stamp across all pages.', snippet: 'graphics.DrawString("CONFIDENTIAL", font, brush);' },
]

const strengths = [
  { t: 'Native editable charts', d: 'Excel/PPT charts are OfficeChart objects. Recipient can click Edit Data and modify them.' },
  { t: '400+ Excel formulas', d: 'AVERAGE, SUM, IF, VLOOKUP. The exported report "lives" and recalculates when opened.' },
  { t: 'Community license free', d: 'Free up to $1M revenue — major advantage over other enterprise suites.' },
  { t: 'PDF/UA Accessibility', d: 'Auto-generates tagged PDFs compliant with WCAG 2.1 AA.' },
  { t: 'Zero cold start', d: 'No Chromium process. Pure .NET — renders normal surveys in under 100ms.' },
]
const weaknesses = [
  { t: 'No HTML template support', d: 'Syncfusion builds via object model. A designer cannot own the layout in HTML/CSS.' },
  { t: 'Commercial license >$1M', d: 'Community is generous but large companies pay. The full enterprise license is significant.' },
  { t: 'Three separate APIs', d: 'XlsIO, Essential PDF, Presentation — each with its own object model to learn.' },
  { t: 'No Chart.js', d: 'Cannot render JavaScript charts in PDFs. Charts go through the Syncfusion chart API.' },
]

onMounted(() => { setTimeout(() => buildNativeChart(), 100) })
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&family=Source+Code+Pro:wght@400;500&display=swap');
:root{--accent:#14b8a6;--surface:#021314;--surface-2:#0a2228;--border:rgba(20,184,166,0.15);--text:#e2e8f0;--text-muted:#64748b}
.page{font-family:'Plus Jakarta Sans',sans-serif;max-width:1300px;margin:0 auto;color:#e2e8f0}
.hero{position:relative;overflow:hidden;border-radius:20px;background:#021314;margin-bottom:48px;display:grid;grid-template-columns:1fr 280px;min-height:420px}
.hero-bg{position:absolute;inset:0;background:radial-gradient(ellipse at 80% 50%,rgba(20,184,166,.15) 0%,transparent 70%)}
.hero-content{position:relative;z-index:1;padding:48px}
.back{display:inline-block;color:rgba(255,255,255,.3);text-decoration:none;font-size:13px;margin-bottom:20px;transition:color .15s}
.back:hover{color:#14b8a6}
.eyebrow{font-size:11px;text-transform:uppercase;letter-spacing:2px;color:#14b8a6;margin-bottom:12px}
.eyebrow-sm{font-size:10px;text-transform:uppercase;letter-spacing:3px;color:#14b8a6;margin-bottom:6px;font-family:'Source Code Pro',monospace}
h1{font-size:50px;font-weight:800;margin:0 0 4px;color:#fff;letter-spacing:-2px}
.tagline{font-family:'Source Code Pro',monospace;font-size:13px;color:#14b8a6;margin-bottom:16px}
.hero-content>p{font-size:14px;color:rgba(255,255,255,.55);max-width:480px;line-height:1.6;margin:0 0 18px}
.verdicts{display:flex;flex-direction:column;gap:5px;margin-bottom:18px}
.verdict{font-size:11px;padding:5px 10px;border-radius:5px;font-weight:500}
.verdict.ok{background:rgba(34,197,94,.07);border:1px solid rgba(34,197,94,.2);color:#22c55e}
.verdict.warn{background:rgba(245,158,11,.07);border:1px solid rgba(245,158,11,.2);color:#f59e0b}
.badges{display:flex;flex-wrap:wrap;gap:6px}
.b{font-size:10px;padding:3px 10px;border-radius:100px;border:1px solid;font-weight:500}
.b.teal{color:#14b8a6;border-color:rgba(20,184,166,.3);background:rgba(20,184,166,.06)}
.b.yellow{color:#f59e0b;border-color:rgba(245,158,11,.3);background:rgba(245,158,11,.06)}
.hero-products{position:relative;z-index:1;display:flex;flex-direction:column;gap:12px;padding:36px 24px;background:rgba(20,184,166,.03);border-left:1px solid rgba(20,184,166,.1)}
.prod-card{background:rgba(20,184,166,.05);border:1px solid rgba(20,184,166,.15);border-radius:10px;padding:14px}
.pc-icon{font-size:18px;display:block;margin-bottom:4px}
.pc-name{font-size:13px;font-weight:700;color:#fff}
.pc-desc{font-size:11px;color:#6b7280;margin-bottom:6px}
.star{font-size:12px;color:#1e293b}
.star.on{color:#14b8a6}
.section{margin-bottom:52px}
.section>h2{font-size:22px;font-weight:700;color:#fff;margin:0 0 8px}
.sub{font-size:13px;color:#64748b;line-height:1.6;margin:0 0 22px;max-width:700px}
.sub code{background:rgba(20,184,166,.12);color:#2dd4bf;padding:1px 5px;border-radius:3px;font-family:'Source Code Pro',monospace}
.sub strong{color:#e2e8f0}
.formula-sandbox{display:grid;grid-template-columns:1fr 360px;gap:20px}
.fs-sheet{background:#021314;border:1px solid rgba(20,184,166,.2);border-radius:12px;overflow:hidden}
.fs-toolbar{background:#0a2228;padding:8px 14px;display:flex;align-items:center;gap:16px;border-bottom:1px solid rgba(20,184,166,.15)}
.fs-app{font-size:12px;font-weight:600;color:#22c55e}
.fs-fbar{flex:1;font-family:'Source Code Pro',monospace;font-size:12px;color:#14b8a6;background:rgba(0,0,0,.3);padding:3px 10px;border-radius:4px;border:1px solid rgba(20,184,166,.2)}
.fs-head{display:grid;grid-template-columns:36px 130px 1fr 1fr 1fr 160px;background:#0a2228;border-bottom:1px solid rgba(20,184,166,.15)}
.fsh-cell{padding:8px 10px;font-size:10px;font-weight:700;color:#94a3b8;border-right:1px solid rgba(255,255,255,.03)}
.fsh-cell.corner{background:#0d2d35}
.fsh-cell.formula-col{color:#14b8a6}
.fs-row{display:grid;grid-template-columns:36px 130px 1fr 1fr 1fr 160px;border-bottom:1px solid rgba(20,184,166,.06);cursor:pointer;transition:background .1s}
.fs-row:hover{background:rgba(20,184,166,.04)}
.fs-row.selected{background:rgba(20,184,166,.08)}
.fs-row.fs-total{background:rgba(20,184,166,.05);border-top:2px solid rgba(20,184,166,.2)}
.fs-rnum{padding:0 8px;display:flex;align-items:center;justify-content:center;font-size:10px;color:#374151;background:#0a2228;border-right:1px solid rgba(255,255,255,.03)}
.fs-cell{padding:6px 10px;display:flex;align-items:center;font-size:12px;border-right:1px solid rgba(255,255,255,.03)}
.fs-label{color:#94a3b8}
.fs-tlabel{color:#14b8a6;font-size:10px;font-family:'Source Code Pro',monospace}
.fs-input{width:100%;background:rgba(0,0,0,.2);border:1px solid transparent;border-radius:3px;color:#e2e8f0;font-size:12px;padding:2px 6px;font-family:'Source Code Pro',monospace;transition:border-color .15s}
.fs-input:focus{outline:none;border-color:#14b8a6;background:rgba(20,184,166,.08)}
.fs-result{flex-direction:column;align-items:flex-start;gap:1px}
.fsr-fx{font-size:9px;color:#374151;font-family:'Source Code Pro',monospace}
.fsr-val{font-size:13px;font-weight:700}
.fs-grand{font-size:16px;font-weight:800;color:#14b8a6 !important}
.fs-note{padding:8px 14px;font-size:10px;color:#374151;background:rgba(0,0,0,.2);border-top:1px solid rgba(20,184,166,.08)}
.fs-code{background:#021314;border:1px solid rgba(20,184,166,.15);border-radius:12px;overflow:hidden}
.code-hdr{display:flex;justify-content:space-between;padding:10px 16px;background:#0a2228;font-size:11px;color:#6b7280;border-bottom:1px solid rgba(20,184,166,.12)}
.code-lang{color:#14b8a6;font-weight:700}
.code-body{margin:0;padding:16px;overflow-x:auto}
.code-body code{font-family:'Source Code Pro',monospace;font-size:11px;line-height:1.7;color:#94a3b8;white-space:pre}
.chart-demo{display:grid;grid-template-columns:220px 1fr 360px;gap:20px;align-items:start}
.cd-picker{display:flex;flex-direction:column;gap:8px}
.nct-btn{display:flex;flex-direction:column;gap:3px;padding:12px 14px;border-radius:8px;border:1px solid rgba(20,184,166,.15);background:transparent;cursor:pointer;text-align:left;transition:all .15s;color:#64748b;font-family:'Plus Jakarta Sans',sans-serif}
.nct-btn:hover{border-color:rgba(20,184,166,.3);color:#94a3b8}
.nct-btn.active{border-color:#14b8a6;background:rgba(20,184,166,.08);color:#e2e8f0}
.nct-icon{font-size:18px}
.nct-label{font-size:12px;font-weight:600}
.nct-code{font-family:'Source Code Pro',monospace;font-size:9px;color:#14b8a6;margin-top:2px}
.cd-pnote{font-size:10px;color:#374151;line-height:1.5;margin-top:8px}
.cd-pnote code{background:rgba(20,184,166,.1);color:#14b8a6;padding:1px 4px;border-radius:3px;font-family:'Source Code Pro',monospace;font-size:9px}
.cd-canvas-wrap{background:#021314;border:1px solid rgba(20,184,166,.15);border-radius:12px;overflow:hidden}
.cd-edit-bar{display:flex;align-items:center;gap:10px;padding:8px 14px;background:#1a3a44;font-size:10px;color:#94a3b8;border-top:1px solid rgba(20,184,166,.1)}
.cdb-btns{display:flex;gap:6px;margin-left:auto}
.cdb-btns span{padding:2px 8px;border:1px solid rgba(255,255,255,.08);border-radius:3px;cursor:pointer;transition:all .1s}
.cdb-btns span:hover{border-color:#14b8a6;color:#14b8a6}
.sandbox-grid{display:grid;grid-template-columns:repeat(5,1fr);gap:12px}
.sc{background:#021314;border:1px solid rgba(20,184,166,.1);border-radius:12px;padding:18px;cursor:pointer;transition:all .2s}
.sc:hover{border-color:rgba(20,184,166,.3)}
.sc.active{border-color:rgba(20,184,166,.5);background:rgba(20,184,166,.04)}
.sc-top{display:flex;justify-content:space-between;align-items:center;margin-bottom:10px}
.sc-icon{font-size:20px}
.sc-toggle{width:36px;height:20px;border-radius:100px;background:#1e293b;position:relative;transition:background .2s}
.sc-toggle.on{background:#14b8a6}
.sc-thumb{position:absolute;left:3px;top:3px;width:14px;height:14px;border-radius:50%;background:white;transition:transform .2s;box-shadow:0 1px 3px rgba(0,0,0,.3)}
.sc-toggle.on .sc-thumb{transform:translateX(16px)}
.sc-title{font-size:12px;font-weight:700;color:#e2e8f0;margin-bottom:6px}
.sc-desc{font-size:11px;color:#6b7280;line-height:1.5;margin-bottom:10px}
.sc-snippet{background:rgba(0,0,0,.3);border-radius:4px;padding:6px 8px;margin-bottom:8px}
.sc-snippet code{font-family:'Source Code Pro',monospace;font-size:9px;color:#14b8a6;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;display:block}
.sc-badge{font-size:10px;color:#14b8a6;font-weight:600}
.sw-grid{display:grid;grid-template-columns:1fr 1fr;gap:20px}
.sw-col{background:#021314;border-radius:12px;padding:24px}
.sw-col.sw-teal{border-top:3px solid #14b8a6}
.sw-col.sw-yellow{border-top:3px solid #f59e0b}
.sw-col h3{font-size:15px;font-weight:700;margin:0 0 16px}
.sw-teal h3{color:#14b8a6}
.sw-yellow h3{color:#f59e0b}
.sw-item{padding:10px 0;border-bottom:1px solid rgba(255,255,255,.04)}
.sw-item:last-child{border-bottom:none}
.sw-t{font-size:12px;font-weight:600;color:#e2e8f0;margin-bottom:3px}
.sw-d{font-size:11px;color:#64748b;line-height:1.5}
.workspace{display:grid;grid-template-columns:340px 1fr;gap:20px}
.ws-left,.ws-right{display:flex;flex-direction:column;gap:16px}
</style>
