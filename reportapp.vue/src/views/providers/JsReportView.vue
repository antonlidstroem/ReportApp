<template>
  <div class="page">
    <div class="hero">
      <div class="hero-rings"><div v-for="i in 4" :key="i" class="ring" :style="`--i:${i}`"></div></div>
      <div class="hero-content">
        <router-link to="/" class="back">← Dashboard</router-link>
        <div class="eyebrow">Track 1 · Web-Standard Engine</div>
        <h1>jsreport</h1>
        <div class="tagline">HTML + CSS + JavaScript → PDF</div>
        <p>The only provider that runs real Chart.js inside reports. Chromium renders your HTML — every gradient, every chart — and captures it as a pixel-perfect PDF.</p>
        <div class="verdicts">
          <div class="verdict ok">✓ Best for: Designer-owned HTML/CSS templates with live charts</div>
          <div class="verdict warn">⚠ Avoid if: You need live Excel formulas or editable PPT charts</div>
        </div>
        <div class="badges">
          <span class="b purple">★ Chart.js in PDF</span>
          <span class="b purple">✓ Handlebars templating</span>
          <span class="b green">✓ Preview = PDF output</span>
          <span class="b red">✗ No native PPT</span>
          <span class="b red">✗ 2–5s cold start</span>
        </div>
      </div>
      <div class="hero-browser">
        <div class="bc"><div class="bc-dots"><span></span><span></span><span></span></div><span class="bc-url">{{SurveyTitle}} → Chromium → PDF</span></div>
        <div class="bb">
          <canvas ref="heroRef" height="110"></canvas>
          <div class="mini-rows">
            <div v-for="q in heroRows" :key="q.t" class="mr"><span>{{ q.t }}</span><div class="mrb"><div :style="`width:${q.v*20}%;background:${sc(q.v)}`"></div></div><span :style="`color:${sc(q.v)}`">{{ q.v }}</span></div>
          </div>
        </div>
      </div>
    </div>

    <section class="section">
      <div class="eyebrow-sm">CAPABILITY 1 — UNIQUE TO jsreport</div>
      <h2>Edit data → chart updates → PDF is this exact view</h2>
      <p class="sub">Chromium waits for <code>networkidle</code> — all JS executed, all charts painted — then screenshots. Move the sliders. The chart updates live. <strong>That is exactly what the PDF will contain.</strong></p>
      <div class="chartlab">
        <div class="cl-left">
          <div class="cl-header"><span>Survey data</span><button class="btn-sm" @click="randomize">↻ Randomize</button></div>
          <div class="cl-sliders">
            <div v-for="(r, i) in chartRows" :key="i" class="cl-row">
              <span class="cl-lbl">{{ r.label }}</span>
              <input type="range" min="1" max="5" step="0.1" v-model.number="r.value" class="cl-range" :style="`--c:${sc(r.value)}`" @input="updateLiveChart" />
              <span :style="`color:${sc(r.value)}`" class="cl-val">{{ r.value.toFixed(1) }}</span>
            </div>
          </div>
          <div class="cl-types">
            <button v-for="ct in chartTypes" :key="ct.id" :class="['ct-btn', { active: activeCT === ct.id }]" @click="activeCT = ct.id; rebuildLiveChart()">{{ ct.icon }} {{ ct.label }}</button>
          </div>
          <div class="cl-note"><strong>How it works:</strong> Template has <code>&lt;canvas&gt;</code> + <code>new Chart(...)</code>. Chromium executes JS, waits for idle, screenshots → PDF.</div>
        </div>
        <div class="cl-right">
          <div class="cl-plabel">Live Chart.js → identical to PDF</div>
          <canvas ref="liveRef" height="280"></canvas>
        </div>
      </div>
    </section>

    <section class="section">
      <div class="eyebrow-sm">CAPABILITY 2</div>
      <h2>Live PDF Preview — exact HTML sent to Chromium</h2>
      <p class="sub">This iframe renders the actual HTML template sent to jsreport. Switch between templates. The PDF is a pixel-perfect capture of this view.</p>
      <LiveReportPreview provider="jsreport" :show-sliders="true" />
    </section>

    <section class="section">
      <div class="eyebrow-sm">CAPABILITY 3</div>
      <h2>Handlebars — report logic in HTML, not C#</h2>
      <p class="sub">Toggle features and watch the template update live. A designer controls this — zero C# changes needed.</p>
      <div class="hb-demo">
        <div class="hbd-left">
          <label v-for="t in hbToggles" :key="t.id" class="hbd-toggle">
            <input type="checkbox" v-model="t.enabled" />
            <div class="track"><div class="thumb"></div></div>
            <div><span class="tt">{{ t.title }}</span><span class="td">{{ t.desc }}</span></div>
          </label>
        </div>
        <div class="hbd-right">
          <div class="hbd-header"><span>Generated template fragment</span><span class="hbd-lang">Handlebars</span></div>
          <pre class="hbd-pre"><code>{{ generatedHb }}</code></pre>
        </div>
      </div>
    </section>

    <section class="section">
      <div class="eyebrow-sm">HONEST ASSESSMENT</div>
      <h2>Strengths and weaknesses</h2>
      <div class="sw-grid">
        <div class="sw-col sw-purple">
          <h3>🌐 Strengths</h3>
          <div v-for="s in strengths" :key="s.t" class="sw-item"><div class="sw-t">{{ s.t }}</div><div class="sw-d">{{ s.d }}</div></div>
        </div>
        <div class="sw-col sw-yellow">
          <h3>⚠ Weaknesses</h3>
          <div v-for="w in weaknesses" :key="w.t" class="sw-item"><div class="sw-t">{{ w.t }}</div><div class="sw-d">{{ w.d }}</div></div>
        </div>
      </div>
    </section>

    <StressTestPanel provider="jsreport" provider-color="#a855f7" />

    <section class="section">
      <div class="eyebrow-sm">EXPORT WORKSPACE</div>
      <h2>Export with real data</h2>
      <p class="sub">The "Full Report" and "Executive" templates include Chart.js charts rendered by Chromium. Select a survey and export.</p>
      <div class="workspace">
        <div class="ws-left"><SurveyPicker /><ModuleList /></div>
        <div class="ws-right"><ExportButton provider="jsreport" :supports-html-template="true" /><TemplateDesigner provider-name="jsreport" :supports-html-template="true" /></div>
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

const heroRef = ref<HTMLCanvasElement>()
const liveRef = ref<HTMLCanvasElement>()
let liveChart: Chart | null = null
const heroRows = [{ t: 'Utvilad på morgonen?', v: 3.1 }, { t: 'Återhämtningstid?', v: 3.5 }, { t: 'Stress kring deadlines?', v: 4.2 }]
const sc = (v: number) => v >= 4 ? '#22c55e' : v >= 3 ? '#f59e0b' : '#ef4444'
const chartRows = ref([{ label: 'Ledarskap', value: 4.1 }, { label: 'Psykosocialt', value: 3.2 }, { label: 'Hälsa', value: 3.8 }, { label: 'Säkerhet', value: 4.5 }, { label: 'Resurser', value: 3.5 }])
const chartTypes = [{ id: 'bar', icon: '📊', label: 'Bar' }, { id: 'line', icon: '📈', label: 'Line' }, { id: 'radar', icon: '🎯', label: 'Radar' }, { id: 'doughnut', icon: '🍩', label: 'Donut' }]
const activeCT = ref('bar')

function randomize() { chartRows.value.forEach(r => { r.value = Math.round((1 + Math.random() * 4) * 10) / 10 }); rebuildLiveChart() }
function updateLiveChart() {
  if (!liveChart) return
  liveChart.data.datasets[0].data = chartRows.value.map(r => r.value)
  if (activeCT.value === 'bar') (liveChart.data.datasets[0] as import('chart.js').BarControllerDatasetOptions & {backgroundColor: string[]}).backgroundColor = chartRows.value.map(r => sc(r.value) + 'cc')
  liveChart.update('none')
}
function rebuildLiveChart() {
  if (!liveRef.value) return
  if (liveChart) liveChart.destroy()
  const isRound = activeCT.value === 'doughnut'
  liveChart = new Chart(liveRef.value, {
    type: activeCT.value as 'bar' | 'line' | 'radar' | 'doughnut',
    data: { labels: chartRows.value.map(r => r.label), datasets: [{ data: chartRows.value.map(r => r.value), backgroundColor: isRound ? ['#a855f7','#7c3aed','#6d28d9','#5b21b6','#4c1d95'] : chartRows.value.map(r => sc(r.value) + 'cc'), borderColor: isRound ? 'transparent' : '#a855f7', borderWidth: 2, borderRadius: activeCT.value === 'bar' ? 4 : undefined, fill: activeCT.value === 'line' ? true : undefined, tension: 0.4, pointBackgroundColor: '#a855f7' }] },
    options: { animation: { duration: 300 }, plugins: { legend: { display: isRound, labels: { color: '#94a3b8', font: { size: 10 } } } }, scales: activeCT.value === 'radar' ? { r: { min: 0, max: 5, ticks: { color: '#666', stepSize: 1, font: { size: 8 } }, grid: { color: '#2d1060' }, pointLabels: { color: '#94a3b8', font: { size: 9 } } } } : isRound ? {} : { y: { min: 0, max: 5, ticks: { color: '#888', font: { size: 10 } }, grid: { color: '#1e1e3f' } }, x: { ticks: { color: '#888', font: { size: 10 } }, grid: { color: '#1e1e3f' } } } } as import('chart.js').ChartOptions
  })
}

const hbToggles = ref([
  { id: 'exec', title: 'Executive summary', desc: 'KPI strip with OverallAverage, QuestionCount', enabled: true },
  { id: 'trend', title: 'Monthly trend (Chart.js)', desc: 'Line chart from Trends data via Chromium JS', enabled: true },
  { id: 'highlight', title: 'Highlight low scores', desc: 'Red row when AverageValue < 3.0', enabled: false },
  { id: 'cats', title: 'Group by category', desc: 'Section headers, nested #each loops', enabled: false },
])

const generatedHb = computed(() => {
  const L: string[] = []
  if (hbToggles.value.find(t => t.id === 'exec')?.enabled) { L.push('{{! Executive block }}'); L.push('<div class="kpis">'); L.push('  <b>{{OverallAverage}}</b> avg · {{QuestionCount}} questions'); L.push('</div>'); L.push('') }
  if (hbToggles.value.find(t => t.id === 'cats')?.enabled) L.push('{{#each CategoryGroups}}\n<h2>{{name}}</h2>')
  if (hbToggles.value.find(t => t.id === 'highlight')?.enabled) { L.push('{{#each QuestionSummaries}}'); L.push('<div class="row {{#if (lt AverageValue 3)}}low{{/if}}">'); L.push('  <span>{{Text}}</span><b>{{AverageValue}}</b>'); L.push('</div>\n{{/each}}') }
  else { L.push('{{#each QuestionSummaries}}'); L.push('<div class="row">'); L.push('  <span>Q{{@index}}: {{Text}}</span>'); L.push('  <b>{{AverageValue}}</b>'); L.push('</div>\n{{/each}}') }
  if (hbToggles.value.find(t => t.id === 'cats')?.enabled) L.push('{{/each}}')
  if (hbToggles.value.find(t => t.id === 'trend')?.enabled) { L.push('\n{{! Chart.js runs in Chromium }}'); L.push('<canvas id="trend"></canvas>'); L.push('<script>'); L.push("  new Chart('trend',{type:'line',data:{"); L.push('    labels:[{{#each Trends}}\'{{MonthName}}\'{{#unless @last}},{{/unless}}{{/each}}],'); L.push('    datasets:[{data:[{{#each Trends}}{{AverageValue}},{{/each}}]}]'); L.push('  }});\n<\\/script>') }
  return L.join('\n') || '<!-- Enable options above -->'
})

const strengths = [{ t: 'Chart.js in PDF — unique', d: 'Only provider running JS inside Chromium and capturing rendered charts as PDF.' }, { t: 'Designer-owned templates', d: 'HTML/CSS. Frontend designers change the layout without touching C#.' }, { t: 'Preview = PDF output', d: 'What the browser shows is what gets captured. Zero discrepancy.' }, { t: 'Handlebars logic', d: 'Conditions, loops, helpers in the template itself.' }]
const weaknesses = [{ t: 'Chromium cold start (2–5s)', d: 'Node.js process + Chromium must initialize. First PDF is always slow.' }, { t: 'No native PowerPoint', d: 'No PPT recipe. This setup falls back to Syncfusion for PPT.' }, { t: 'Excel quality limited', d: 'HTML-to-xlsx: no formulas, no conditional formatting, no native charts.' }, { t: 'Deployment complexity', d: "Chromium's Linux library deps can be painful in minimal containers." }]

onMounted(() => {
  if (heroRef.value) {
    new Chart(heroRef.value, { type: 'bar', data: { labels: heroRows.map((_, i) => `Q${i+1}`), datasets: [{ data: heroRows.map(q => q.v), backgroundColor: heroRows.map(q => sc(q.v) + 'aa'), borderColor: heroRows.map(q => sc(q.v)), borderWidth: 2, borderRadius: 3 }] }, options: { plugins: { legend: { display: false } }, scales: { y: { min: 0, max: 5, ticks: { color: '#666', font: { size: 8 } }, grid: { color: '#1e1e2e' } }, x: { ticks: { color: '#666', font: { size: 8 } }, grid: { display: false } } } } })
  }
  setTimeout(rebuildLiveChart, 120)
})
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;600;700;900&family=JetBrains+Mono:wght@400;500&display=swap');
:root{--accent:#a855f7;--surface:#0d0018;--surface-2:#120020;--border:rgba(168,85,247,0.15);--text:#e2e8f0;--text-muted:#64748b}
.page{font-family:'Outfit',sans-serif;max-width:1300px;margin:0 auto;color:#e2e8f0}
.hero{position:relative;overflow:hidden;border-radius:20px;background:#0d0018;margin-bottom:48px;display:grid;grid-template-columns:1fr 360px;min-height:420px}
.hero-rings{position:absolute;top:50%;left:50%;pointer-events:none}
.ring{position:absolute;top:50%;left:50%;transform:translate(-50%,-50%);border:1px solid rgba(168,85,247,calc(0.14 - var(--i)*0.025));border-radius:50%;width:calc(var(--i)*130px);height:calc(var(--i)*130px)}
.hero-content{position:relative;z-index:1;padding:48px}
.back{display:inline-block;color:rgba(255,255,255,0.3);text-decoration:none;font-size:13px;margin-bottom:20px;transition:color .15s}
.back:hover{color:#a855f7}
.eyebrow{font-size:11px;text-transform:uppercase;letter-spacing:2px;color:#a855f7;margin-bottom:12px}
.eyebrow-sm{font-size:10px;text-transform:uppercase;letter-spacing:3px;color:#a855f7;margin-bottom:6px;font-family:'JetBrains Mono',monospace}
h1{font-size:54px;font-weight:900;margin:0 0 4px;color:#fff;letter-spacing:-2px}
.tagline{font-family:'JetBrains Mono',monospace;font-size:14px;color:#a855f7;margin-bottom:16px}
.hero-content>p{font-size:14px;color:rgba(255,255,255,.55);max-width:460px;line-height:1.6;margin:0 0 18px}
.verdicts{display:flex;flex-direction:column;gap:5px;margin-bottom:18px}
.verdict{font-size:11px;padding:5px 10px;border-radius:5px;font-weight:500}
.verdict.ok{background:rgba(34,197,94,.07);border:1px solid rgba(34,197,94,.2);color:#22c55e}
.verdict.warn{background:rgba(245,158,11,.07);border:1px solid rgba(245,158,11,.2);color:#f59e0b}
.badges{display:flex;flex-wrap:wrap;gap:6px}
.b{font-size:10px;padding:3px 10px;border-radius:100px;border:1px solid;font-weight:500}
.b.purple{color:#a855f7;border-color:rgba(168,85,247,.3);background:rgba(168,85,247,.06)}
.b.green{color:#22c55e;border-color:rgba(34,197,94,.3);background:rgba(34,197,94,.06)}
.b.red{color:#ef4444;border-color:rgba(239,68,68,.3);background:rgba(239,68,68,.06)}
.hero-browser{position:relative;z-index:1;padding:28px;display:flex;flex-direction:column;background:rgba(168,85,247,.03);border-left:1px solid rgba(168,85,247,.12)}
.bc{background:#2e1065;border-radius:8px 8px 0 0;padding:8px 12px;display:flex;align-items:center;gap:8px}
.bc-dots{display:flex;gap:4px}.bc-dots span{width:8px;height:8px;border-radius:50%;background:rgba(255,255,255,.15)}
.bc-url{font-size:9px;font-family:'JetBrains Mono',monospace;color:#94a3b8;flex:1;background:rgba(0,0,0,.25);padding:2px 8px;border-radius:3px}
.bb{background:#1a0533;border-radius:0 0 8px 8px;padding:12px;flex:1;border:1px solid rgba(168,85,247,.2);border-top:none}
.mini-rows{margin-top:10px;display:flex;flex-direction:column;gap:5px}
.mr{display:flex;align-items:center;gap:8px;font-size:9px}
.mr span:first-child{flex:1;color:#94a3b8;white-space:nowrap;overflow:hidden;text-overflow:ellipsis}
.mrb{width:60px;height:4px;background:rgba(255,255,255,.08);border-radius:2px;overflow:hidden}
.mrb div{height:100%;border-radius:2px}
.mr span:last-child{font-size:10px;font-weight:700;width:24px}
.section{margin-bottom:52px}
.section>h2{font-size:22px;font-weight:700;color:#fff;margin:0 0 8px}
.sub{font-size:13px;color:#64748b;line-height:1.6;margin:0 0 22px;max-width:700px}
.sub code{background:rgba(168,85,247,.15);color:#c4b5fd;padding:1px 5px;border-radius:3px;font-family:'JetBrains Mono',monospace}
.sub strong{color:#e2e8f0}
.chartlab{display:grid;grid-template-columns:320px 1fr;gap:24px;background:#0d0018;border:1px solid rgba(168,85,247,.15);border-radius:14px;padding:24px}
.cl-header{display:flex;justify-content:space-between;align-items:center;margin-bottom:16px;font-size:12px;font-weight:600;color:#94a3b8}
.btn-sm{font-size:11px;padding:4px 10px;border-radius:5px;border:1px solid rgba(168,85,247,.3);background:transparent;color:#a855f7;cursor:pointer;font-family:'Outfit',sans-serif;transition:all .12s}
.btn-sm:hover{background:rgba(168,85,247,.1)}
.cl-sliders{display:flex;flex-direction:column;gap:10px;margin-bottom:16px}
.cl-row{display:flex;align-items:center;gap:8px}
.cl-lbl{font-size:11px;color:#64748b;width:96px;flex-shrink:0}
.cl-range{flex:1;-webkit-appearance:none;height:4px;border-radius:2px;background:rgba(168,85,247,.15);outline:none}
.cl-range::-webkit-slider-thumb{-webkit-appearance:none;width:14px;height:14px;border-radius:50%;background:var(--c,#a855f7);cursor:pointer;box-shadow:0 0 6px var(--c,#a855f7)}
.cl-val{font-size:12px;font-weight:700;width:28px;text-align:right}
.cl-types{display:flex;gap:6px;flex-wrap:wrap;margin-bottom:16px}
.ct-btn{font-size:11px;padding:5px 10px;border-radius:6px;border:1px solid rgba(255,255,255,.08);background:transparent;color:#64748b;cursor:pointer;transition:all .12s;font-family:'Outfit',sans-serif}
.ct-btn.active{border-color:#a855f7;color:#a855f7;background:rgba(168,85,247,.08)}
.cl-note{font-size:11px;color:#475569;line-height:1.5;padding:10px;background:rgba(168,85,247,.04);border-radius:8px;border:1px solid rgba(168,85,247,.1)}
.cl-note code{background:rgba(168,85,247,.15);color:#c4b5fd;padding:1px 4px;border-radius:3px;font-family:'JetBrains Mono',monospace;font-size:10px}
.cl-note strong{color:#e2e8f0}
.cl-right{display:flex;flex-direction:column}
.cl-plabel{font-size:10px;color:#475569;margin-bottom:10px;text-transform:uppercase;letter-spacing:1px}
.hb-demo{display:grid;grid-template-columns:280px 1fr;background:#0d0018;border:1px solid rgba(168,85,247,.15);border-radius:14px;overflow:hidden}
.hbd-left{padding:20px;display:flex;flex-direction:column;gap:10px;border-right:1px solid rgba(168,85,247,.1)}
.hbd-toggle{display:flex;align-items:flex-start;gap:10px;cursor:pointer}
.hbd-toggle input{display:none}
.track{width:34px;height:18px;border-radius:100px;background:#1e293b;position:relative;flex-shrink:0;transition:background .2s;margin-top:2px}
.thumb{position:absolute;left:2px;top:2px;width:14px;height:14px;border-radius:50%;background:white;transition:transform .2s}
.hbd-toggle input:checked~.track{background:#a855f7}
.hbd-toggle input:checked~.track .thumb{transform:translateX(16px)}
.tt{display:block;font-size:12px;font-weight:600;color:#e2e8f0;margin-bottom:1px}
.td{font-size:11px;color:#475569;line-height:1.3}
.hbd-right{display:flex;flex-direction:column;background:#060a12}
.hbd-header{display:flex;justify-content:space-between;padding:10px 16px;font-size:10px;color:#475569;border-bottom:1px solid rgba(168,85,247,.08)}
.hbd-lang{color:#a855f7;font-weight:700}
.hbd-pre{flex:1;margin:0;padding:16px;overflow:auto;min-height:200px}
.hbd-pre code{font-family:'JetBrains Mono',monospace;font-size:10px;color:#c4b5fd;white-space:pre;line-height:1.6}
.sw-grid{display:grid;grid-template-columns:1fr 1fr;gap:20px}
.sw-col{background:#0d0018;border-radius:12px;padding:24px}
.sw-col.sw-purple{border-top:3px solid #a855f7}
.sw-col.sw-yellow{border-top:3px solid #f59e0b}
.sw-col h3{font-size:15px;font-weight:700;margin:0 0 16px}
.sw-purple h3{color:#a855f7}
.sw-yellow h3{color:#f59e0b}
.sw-item{padding:10px 0;border-bottom:1px solid rgba(255,255,255,.04)}
.sw-item:last-child{border-bottom:none}
.sw-t{font-size:12px;font-weight:600;color:#e2e8f0;margin-bottom:3px}
.sw-d{font-size:11px;color:#64748b;line-height:1.5}
.workspace{display:grid;grid-template-columns:340px 1fr;gap:20px}
.ws-left,.ws-right{display:flex;flex-direction:column;gap:16px}
</style>
