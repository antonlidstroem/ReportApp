<template>
  <div class="dashboard">
    <header class="hero">
      <div class="hero-bg"><div class="hbg-grid"></div><div class="hbg-glow"></div></div>
      <div class="hero-inner">
        <div class="hero-tag">Proof of Concept · .NET 8 Reporting Engine</div>
        <h1>Modern Reporting<br>Engine</h1>
        <div class="hero-pills">
          <div class="pill" v-for="m in meta" :key="m.label">
            <span class="pill-val">{{ m.val }}</span>
            <span class="pill-lbl">{{ m.label }}</span>
          </div>
        </div>
      </div>
    </header>

    <section class="section">
      <h2 class="section-title">Technical Tracks</h2>
      <div class="provider-grid">
        <router-link v-for="p in providers" :key="p.route" :to="p.route" class="pcard" :style="`--c:${p.color}`">
          <div class="pcard-top">
            <span class="pcard-icon">{{ p.icon }}</span>
            <span class="pcard-track">Track {{ p.track }}</span>
          </div>
          <h3>{{ p.name }}</h3>
          <p>{{ p.description }}</p>
          <div class="pcard-tags">
            <span v-for="t in p.tags" :key="t" class="tag">{{ t }}</span>
          </div>
          <div class="pcard-scores">
            <div v-for="s in p.scores" :key="s.l" class="ps-row">
              <span class="ps-l">{{ s.l }}</span>
              <div class="ps-bar"><div class="ps-fill" :style="`width:${s.v*10}%`"></div></div>
              <span class="ps-v">{{ s.v }}/10</span>
            </div>
          </div>
          <div class="pcard-foot">
            <span :class="['pcard-cost', p.costClass]">{{ p.cost }}</span>
            <span v-if="p.isStub" class="stub-badge">🚧 Stub</span>
            <span class="pcard-arrow">→</span>
          </div>
        </router-link>
      </div>
    </section>

    <section class="section">
      <h2 class="section-title">Feature Comparison</h2>
      <div class="matrix-tabs">
        <button v-for="f in matrixFilters" :key="f.id" :class="['mtab', { active: activeFilter===f.id }]" @click="activeFilter=f.id">{{ f.label }}</button>
      </div>
      <div class="matrix-wrap">
        <table class="matrix">
          <thead>
            <tr>
              <th class="feat-col">Feature</th>
              <th v-for="p in providers" :key="p.name" :style="`color:${p.color}`">
                <div class="mth-inner">{{ p.icon }} {{ p.shortName }}</div>
              </th>
            </tr>
          </thead>
          <tbody>
            <template v-for="group in filteredMatrix" :key="group.category">
              <tr><td :colspan="providers.length + 1" class="cat-row">{{ group.category }}</td></tr>
              <tr v-for="(row, ri) in group.rows" :key="ri">
                <td class="feat-name">{{ row.feature }}</td>
                <td v-for="(val, vi) in row.values" :key="vi" class="feat-cell">
                  <span v-if="val === true" class="v-yes">✓</span>
                  <span v-else-if="val === false" class="v-no">✗</span>
                  <span v-else class="v-partial">{{ val }}</span>
                </td>
              </tr>
            </template>
          </tbody>
        </table>
      </div>
    </section>

    <section class="section">
      <h2 class="section-title">Capability Radar</h2>
      <div class="radar-grid">
        <div v-for="p in providers" :key="p.name" class="radar-card">
          <div class="rc-title" :style="`color:${p.color}`">{{ p.icon }} {{ p.shortName }}</div>
          <canvas :ref="(el) => setRadarRef(p.shortName, el as HTMLCanvasElement)" height="200"></canvas>
          <div v-if="p.isStub" class="rc-stub">* Estimerade värden</div>
        </div>
      </div>
    </section>

    <section class="section">
      <div class="section-header">
        <h2 class="section-title">Benchmark History</h2>
        <button v-if="benchmarks.results.length" class="clear-btn" @click="benchmarks.clear()">🗑 Clear</button>
      </div>
      <div v-if="!benchmarks.results.length" class="bench-empty">
        <div class="be-icon">⚡</div>
        <div class="be-title">No benchmarks yet</div>
        <div class="be-sub">Go to any provider page, select a survey, and export to record data here.</div>
      </div>
      <div v-else class="bench-layout">
        <div class="bench-chart-wrap">
          <div class="bcw-label">Generation time per export (ms)</div>
          <canvas ref="benchBarRef" height="180"></canvas>
        </div>
        <div class="bench-table-wrap">
          <div class="btw-label">Last 10 exports</div>
          <div class="btw-rows">
            <div class="btw-row btw-header"><span>Provider</span><span>Format</span><span>Time</span><span>Size</span><span>Q</span></div>
            <div v-for="r in benchmarks.results.slice(0, 10)" :key="r.timestamp" class="btw-row">
              <span class="br-provider">{{ r.provider }}</span>
              <span class="br-format">{{ r.format.toUpperCase() }}</span>
              <span :class="['br-time', speedClass(r.generationMs)]">{{ r.generationMs }}ms</span>
              <span class="br-size">{{ formatSize(r.fileSizeBytes) }}</span>
              <span class="br-q">{{ r.questionCount }}</span>
            </div>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useBenchmarkStore } from '../stores/benchmarks'
import { fetchSurveys } from '../composables/useApi'
import { useReportBuilderStore } from '../stores/reportBuilder'
import Chart from 'chart.js/auto'

const benchmarks = useBenchmarkStore()
const store = useReportBuilderStore()
const benchBarRef = ref<HTMLCanvasElement>()
const activeFilter = ref('all')
let benchBarChart: Chart | null = null
const radarRefs: Record<string, HTMLCanvasElement | null> = {}
const radarCharts: Record<string, Chart> = {}

function setRadarRef(name: string, el: HTMLCanvasElement | null) {
  radarRefs[name] = el
}

const meta = [
  { val: '6',     label: 'Providers' },
  { val: '4',     label: 'Surveys'   },
  { val: '100k+', label: 'Responses' },
  { val: '3',     label: 'Formats'   },
]

const providers = [
  { route: '/providers/jsreport',   track: 1, icon: '🌐', name: 'jsreport',             shortName: 'jsreport',  description: 'HTML + CSS + JS → PDF. Real Chart.js charts in exported PDFs.', tags: ['Chart.js PDF', 'Handlebars'], cost: 'OSS/Pro',       costClass: 'freemium', color: '#a855f7', isStub: false, scores: [{ l: 'PDF', v: 10 }, { l: 'Excel', v: 4 }, { l: 'Speed', v: 6 }] },
  { route: '/providers/syncfusion', track: 2, icon: '💎', name: 'Syncfusion',            shortName: 'Syncfusion', description: 'Native Office engine. Live formulas, editable PPT charts, PDF/UA.',  tags: ['Excel Formulas', 'PDF/UA'],   cost: 'Community Free', costClass: 'free',     color: '#14b8a6', isStub: false, scores: [{ l: 'Excel', v: 10 }, { l: 'PPT', v: 9 }, { l: 'Speed', v: 9 }] },
  { route: '/providers/js-sync',    track: 3, icon: '⚒️', name: 'jsreport + Syncfusion', shortName: 'Hybrid A',  description: 'jsreport for PDF (Chart.js). Syncfusion for Excel + PPT (native).', tags: ['Designer PDF', 'Native Excel'], cost: 'Enterprise',    costClass: 'paid',     color: '#8b5cf6', isStub: false, scores: [{ l: 'PDF', v: 10 }, { l: 'Excel', v: 10 }, { l: 'Speed', v: 7 }] },
  { route: '/providers/play-sync',  track: 4, icon: '🎭', name: 'Playwright + Syncfusion',shortName: 'Hybrid B',  description: 'Playwright PDF — faster cold start, no Node.js runtime layer.',     tags: ['Fast PDF', 'CI-friendly'],    cost: 'Enterprise',    costClass: 'paid',     color: '#10b981', isStub: false, scores: [{ l: 'PDF', v: 8 }, { l: 'Excel', v: 10 }, { l: 'Speed', v: 9 }] },
  { route: '/providers/telerik',    track: 5, icon: '⚡', name: 'Telerik',               shortName: 'Telerik',   description: 'Telerik Document Processing — free NuGet, pure .NET, no Chromium.', tags: ['Free NuGet', 'Pure .NET'],    cost: 'Gratis',         costClass: 'free',     color: '#e4572e', isStub: false, scores: [{ l: 'PDF', v: 7 }, { l: 'Excel', v: 7 }, { l: 'Speed', v: 9 }] },
  { route: '/providers/devexpress', track: 6, icon: '🔷', name: 'DevExpress',            shortName: 'DevExpress', description: 'Enterprise document processing. Requires paid Universal license.',  tags: ['Enterprise', 'Stub'],         cost: 'Betald licens',  costClass: 'paid',     color: '#f05a28', isStub: true,  scores: [{ l: 'PDF', v: 9 }, { l: 'Excel', v: 9 }, { l: 'Speed', v: 8 }] },
]

const matrixFilters = [
  { id: 'all', label: 'Alla' }, { id: 'output', label: '📄 Format' },
  { id: 'charts', label: '📊 Diagram' }, { id: 'enterprise', label: '🔐 Enterprise' },
  { id: 'cost', label: '💰 Kostnad' },
]

const fullMatrix = [
  { category: '📄 Format', filter: 'output', rows: [
    { feature: 'PDF',           values: [true, true, true, true, true, true] },
    { feature: 'Excel (.xlsx)', values: ['HTML', true, true, true, true, true] },
    { feature: 'PPT (.pptx)',   values: ['Basic', true, true, true, true, true] },
    { feature: 'HTML → PDF',    values: [true, false, true, 'Tokens', false, false] },
  ]},
  { category: '📊 Diagram', filter: 'charts', rows: [
    { feature: 'Chart.js i PDF',          values: [true, false, true, false, false, false] },
    { feature: 'Redigerbara Excel-diagram', values: [false, true, true, true, false, true] },
    { feature: 'Redigerbara PPT-diagram',   values: [false, true, true, true, false, true] },
  ]},
  { category: '🔐 Enterprise', filter: 'enterprise', rows: [
    { feature: 'AES-256-kryptering', values: [false, true, false, true, false, true] },
    { feature: 'PDF/UA',             values: [false, true, false, true, false, true] },
    { feature: 'Live Excel-formler', values: [false, true, true, true, false, true] },
  ]},
  { category: '💰 Kostnad', filter: 'cost', rows: [
    { feature: 'Licens',       values: ['OSS/Pro', 'Community', '2 lic.', '2 lic.', 'Gratis', 'Betald'] },
    { feature: 'Chromium',     values: [true, false, true, true, false, false] },
    { feature: 'Cold start',   values: ['2–5s', '<100ms', '2–5s', '~200ms', '<50ms', '<80ms*'] },
  ]},
]

const filteredMatrix = computed(() =>
  activeFilter.value === 'all' ? fullMatrix : fullMatrix.filter(g => g.filter === activeFilter.value)
)

const radarDims = ['PDF', 'Excel', 'PPT', 'Charts', 'Cost', 'Templates', 'Enterprise', 'Speed']
const radarData: Record<string, number[]> = {
  jsreport:  [10, 4,  2, 10, 7, 10,  4, 6],
  Syncfusion:[7, 10,  9,  6, 9,  0, 10, 9],
  'Hybrid A':[10,10,  9, 10, 6, 10,  8, 7],
  'Hybrid B':[9, 10,  9,  5, 7,  3,  8, 9],
  Telerik:   [7,  7,  6,  3, 9,  0,  5, 9],
  DevExpress:[9,  9,  7,  3, 3,  0,  9, 8],
}

function drawRadarCharts() {
  providers.forEach(p => {
    const canvas = radarRefs[p.shortName]
    if (!canvas) return
    if (radarCharts[p.shortName]) radarCharts[p.shortName].destroy()
    radarCharts[p.shortName] = new Chart(canvas, {
      type: 'radar',
      data: {
        labels: radarDims,
        datasets: [{ data: radarData[p.shortName] ?? [], backgroundColor: `${p.color}22`, borderColor: p.color, pointBackgroundColor: p.color, borderWidth: 2, pointRadius: 3 }],
      },
      options: {
        plugins: { legend: { display: false } },
        scales: { r: { min: 0, max: 10, ticks: { color: '#475569', stepSize: 2, font: { size: 8 } }, grid: { color: '#1e293b' }, pointLabels: { color: '#94a3b8', font: { size: 9 } } } },
      },
    })
  })
}

function drawBenchChart() {
  const canvas = benchBarRef.value
  if (!canvas || !benchmarks.results.length) return
  if (benchBarChart) benchBarChart.destroy()
  const recent = benchmarks.results.slice(0, 8).reverse()
  benchBarChart = new Chart(canvas, {
    type: 'bar',
    data: {
      labels: recent.map(r => `${r.provider} ${(r.format ?? '').toUpperCase()}`),
      datasets: [{ data: recent.map(r => r.generationMs), backgroundColor: recent.map(r => { const p = providers.find(p => p.shortName === r.provider || p.name === r.provider); return (p?.color ?? '#6b7280') + 'cc' }), borderRadius: 4 }],
    },
    options: { plugins: { legend: { display: false } }, scales: { y: { ticks: { color: '#94a3b8', font: { size: 10 } }, grid: { color: '#1e293b' } }, x: { ticks: { color: '#94a3b8', font: { size: 9 }, maxRotation: 30 }, grid: { display: false } } } },
  })
}

function speedClass(ms: number) { return ms < 500 ? 'fast' : ms < 2000 ? 'ok' : 'slow' }
function formatSize(bytes: number) {
  if (!bytes) return '—'
  return bytes > 1_000_000 ? `${(bytes / 1_000_000).toFixed(1)} MB` : `${(bytes / 1024).toFixed(0)} KB`
}

onMounted(async () => {
  if (!store.surveys.length) store.surveys = await fetchSurveys()
  setTimeout(drawRadarCharts, 150)
})
watch(() => benchmarks.results.length, () => setTimeout(drawBenchChart, 100))
</script>

<style scoped>
.dashboard { max-width: 1300px; margin: 0 auto; color: #e2e8f0; }
.hero { position: relative; overflow: hidden; border-radius: 20px; background: #050d1f; margin-bottom: 48px; padding: 56px; }
.hero-bg { position: absolute; inset: 0; }
.hbg-grid { position: absolute; inset: 0; background-image: linear-gradient(rgba(255,255,255,.025) 1px,transparent 1px),linear-gradient(90deg,rgba(255,255,255,.025) 1px,transparent 1px); background-size: 50px 50px; }
.hbg-glow { position: absolute; width: 600px; height: 600px; background: radial-gradient(circle,rgba(99,102,241,.12) 0%,transparent 70%); top: -200px; right: -100px; }
.hero-inner { position: relative; z-index: 1; }
.hero-tag { font-size: 11px; text-transform: uppercase; letter-spacing: 2px; color: rgba(255,255,255,.35); margin-bottom: 14px; }
h1 { font-size: 52px; font-weight: 800; letter-spacing: -2px; margin: 0 0 28px; color: #fff; line-height: 1.05; }
.hero-pills { display: flex; gap: 14px; flex-wrap: wrap; }
.pill { background: rgba(255,255,255,.04); border: 1px solid rgba(255,255,255,.07); border-radius: 12px; padding: 12px 22px; text-align: center; }
.pill-val { display: block; font-size: 26px; font-weight: 800; color: #fff; }
.pill-lbl { font-size: 10px; color: rgba(255,255,255,.35); text-transform: uppercase; letter-spacing: 1px; }
.section { margin-bottom: 52px; }
.section-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.section-title { font-size: 20px; font-weight: 700; margin: 0 0 16px; color: #f1f5f9; }
.provider-grid { display: grid; grid-template-columns: repeat(6, 1fr); gap: 12px; }
.pcard { display: flex; flex-direction: column; background: #050d1f; border: 1px solid rgba(255,255,255,.05); border-top: 3px solid var(--c); border-radius: 14px; padding: 16px; text-decoration: none; color: #e2e8f0; transition: all .2s; }
.pcard:hover { transform: translateY(-2px); box-shadow: 0 12px 32px rgba(0,0,0,.3); border-color: var(--c); }
.pcard-top { display: flex; justify-content: space-between; align-items: center; margin-bottom: 7px; }
.pcard-icon { font-size: 20px; }
.pcard-track { font-size: 9px; text-transform: uppercase; letter-spacing: 1px; color: var(--c); }
.pcard h3 { font-size: 12px; font-weight: 700; margin: 0 0 4px; color: #fff; }
.pcard > p { font-size: 10px; color: #64748b; line-height: 1.5; flex: 1; margin: 0 0 7px; }
.pcard-tags { display: flex; flex-wrap: wrap; gap: 3px; margin-bottom: 7px; }
.tag { font-size: 8px; padding: 2px 5px; background: color-mix(in srgb,var(--c) 10%,transparent); color: var(--c); border-radius: 100px; border: 1px solid color-mix(in srgb,var(--c) 25%,transparent); }
.pcard-scores { display: flex; flex-direction: column; gap: 3px; margin-bottom: 8px; }
.ps-row { display: flex; align-items: center; gap: 5px; }
.ps-l { font-size: 8px; color: #64748b; width: 36px; flex-shrink: 0; }
.ps-bar { flex: 1; height: 3px; background: #1e293b; border-radius: 2px; overflow: hidden; }
.ps-fill { height: 100%; background: var(--c); border-radius: 2px; }
.ps-v { font-size: 8px; color: #475569; width: 26px; text-align: right; }
.pcard-foot { display: flex; justify-content: space-between; align-items: center; padding-top: 7px; border-top: 1px solid rgba(255,255,255,.05); }
.pcard-cost { font-size: 10px; font-weight: 700; }
.pcard-cost.free { color: #22c55e; } .pcard-cost.paid { color: #f59e0b; } .pcard-cost.freemium { color: #a855f7; }
.stub-badge { font-size: 8px; color: #f59e0b; background: rgba(245,158,11,.1); border: 1px solid rgba(245,158,11,.3); padding: 1px 4px; border-radius: 3px; }
.pcard-arrow { color: #475569; transition: transform .15s; font-size: 13px; }
.pcard:hover .pcard-arrow { transform: translateX(3px); color: var(--c); }
.matrix-tabs { display: flex; gap: 6px; margin-bottom: 12px; flex-wrap: wrap; }
.mtab { font-size: 11px; padding: 4px 12px; border-radius: 100px; border: 1px solid #1e293b; background: transparent; color: #64748b; cursor: pointer; transition: all .15s; font-family: inherit; }
.mtab.active { background: #0f172a; border-color: #475569; color: #e2e8f0; }
.matrix-wrap { overflow-x: auto; border-radius: 12px; border: 1px solid #1e293b; }
.matrix { width: 100%; border-collapse: collapse; }
.matrix th { background: #0a1628; padding: 9px 10px; text-align: left; font-size: 10px; font-weight: 700; border-bottom: 2px solid #1e293b; white-space: nowrap; }
.feat-col { width: 180px; color: #475569; }
.mth-inner { display: flex; align-items: center; gap: 4px; }
.cat-row { background: #0a1628; padding: 5px 10px; font-size: 9px; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; color: #94a3b8; }
.matrix tr:hover td { background: rgba(255,255,255,.015); }
.matrix td { padding: 6px 10px; border-bottom: 1px solid #0f172a; font-size: 10px; }
.feat-name { color: #94a3b8; } .feat-cell { text-align: center; }
.v-yes { color: #22c55e; font-size: 12px; font-weight: 700; } .v-no { color: #ef4444; font-size: 12px; } .v-partial { font-size: 9px; color: #94a3b8; }
.radar-grid { display: grid; grid-template-columns: repeat(6, 1fr); gap: 10px; }
.radar-card { background: #050d1f; border-radius: 10px; padding: 12px; border: 1px solid #1e293b; }
.rc-title { font-size: 11px; font-weight: 700; margin-bottom: 5px; }
.rc-stub { font-size: 9px; color: #475569; margin-top: 3px; text-align: center; }
.bench-layout { display: grid; grid-template-columns: 1fr 340px; gap: 16px; }
.bench-chart-wrap { background: #050d1f; border: 1px solid #1e293b; border-radius: 12px; padding: 16px; }
.bcw-label { font-size: 11px; font-weight: 600; color: #94a3b8; margin-bottom: 8px; }
.bench-table-wrap { background: #050d1f; border: 1px solid #1e293b; border-radius: 12px; overflow: hidden; }
.btw-label { font-size: 11px; font-weight: 600; padding: 9px 12px; background: #0a1628; border-bottom: 1px solid #1e293b; color: #94a3b8; }
.btw-row { display: grid; grid-template-columns: 80px 48px 58px 52px 28px; padding: 6px 12px; border-bottom: 1px solid #0f172a; font-size: 10px; align-items: center; }
.btw-row.btw-header { font-size: 9px; color: #475569; font-weight: 700; text-transform: uppercase; }
.br-provider { color: #94a3b8; font-weight: 600; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.br-format { color: #64748b; font-size: 9px; }
.br-time.fast { color: #22c55e; font-weight: 700; } .br-time.ok { color: #f59e0b; font-weight: 700; } .br-time.slow { color: #ef4444; font-weight: 700; }
.br-size { color: #64748b; } .br-q { color: #475569; }
.bench-empty { padding: 40px; text-align: center; background: #050d1f; border: 1px dashed #1e293b; border-radius: 12px; }
.be-icon { font-size: 30px; margin-bottom: 8px; } .be-title { font-size: 14px; font-weight: 700; color: #e2e8f0; margin-bottom: 4px; }
.be-sub { font-size: 11px; color: #475569; max-width: 360px; margin: 0 auto; line-height: 1.5; }
.clear-btn { font-size: 11px; padding: 4px 10px; border-radius: 6px; border: 1px solid #1e293b; background: transparent; color: #64748b; cursor: pointer; font-family: inherit; }
.clear-btn:hover { background: #ef4444; color: white; border-color: #ef4444; }
</style>
