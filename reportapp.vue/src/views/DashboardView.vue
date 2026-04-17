<template>
  <div class="dashboard">
    <!-- HERO -->
    <header class="hero">
      <div class="hero-bg"><div class="hbg-grid"></div><div class="hbg-glow"></div></div>
      <div class="hero-inner">
        <div class="hero-tag">Proof of Concept · .NET 8 Reporting Engine</div>
        <h1>Modern Reporting<br>Engine</h1>
        <p>Four technical tracks. One database. Compare PDF, Excel, and PowerPoint generation across jsreport, Syncfusion, and two hybrid configurations.</p>
        <div class="hero-pills">
          <div class="pill" v-for="m in meta" :key="m.label">
            <span class="pill-val">{{ m.val }}</span>
            <span class="pill-lbl">{{ m.label }}</span>
          </div>
        </div>
      </div>
    </header>

    <!-- PROVIDER CARDS -->
    <section class="section">
      <h2 class="section-title">Technical Tracks</h2>
      <div class="provider-grid">
        <router-link v-for="p in providers"
                     :key="p.route"
                     :to="p.route"
                     class="pcard"
                     :style="`--c:${p.color}`">
          <div class="pcard-top">
            <span class="pcard-icon">{{ p.icon }}</span>
            <span class="pcard-track">Track {{ p.track }}</span>
          </div>
          <h3>{{ p.name }}</h3>
          <p>{{ p.description }}</p>
          <div class="pcard-verdict">
            <span class="pv-lbl">Best for</span>
            <span class="pv-val">{{ p.bestFor }}</span>
          </div>
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
            <span class="pcard-arrow">→</span>
          </div>
        </router-link>
      </div>
    </section>

    <!-- COMPARISON MATRIX -->
    <section class="section">
      <h2 class="section-title">Full Feature Comparison</h2>
      <div class="matrix-tabs">
        <button v-for="f in matrixFilters" :key="f.id" :class="['mtab', { active: activeFilter===f.id }]" @click="activeFilter=f.id">{{ f.label }}</button>
      </div>
      <div class="matrix-wrap">
        <table class="matrix">
          <thead>
            <tr>
              <th class="feat-col">Feature</th>
              <th v-for="p in providers" :key="p.name" :style="`color:${p.color}`">
                <div class="mth-inner"><span>{{ p.icon }}</span><span>{{ p.shortName }}</span></div>
              </th>
            </tr>
          </thead>
          <tbody>
            <template v-for="group in filteredMatrix" :key="group.category">
              <tr><td colspan="5" class="cat-row">{{ group.category }}</td></tr>
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

    <!-- RADAR CHARTS -->
    <section class="section">
      <h2 class="section-title">Capability Radar</h2>
      <div class="radar-grid">
        <div v-for="p in providers" :key="p.name" class="radar-card">
          <div class="rc-title" :style="`color:${p.color}`">{{ p.icon }} {{ p.shortName }}</div>
          <canvas :ref="(el) => (radarRefs[p.shortName] = el as HTMLCanvasElement)" height="200"></canvas>
        </div>
      </div>
    </section>

    <!--STRESSTEST-->
    <section class="section">
      <h2 class="section-title">⚡ Live Performance Stress Test</h2>
      <div class="stress-test-wrapper">
        <div class="stress-controls">
          <button v-for="p in providers"
                  :key="p.id"
                  :class="['st-btn', { active: activeStressProvider === p.id }]"
                  :style="activeStressProvider === p.id ? `--c:${p.color}` : ''"
                  @click="activeStressProvider = p.id">
            {{ p.icon }} {{ p.shortName }}
          </button>
        </div>

        <StressTestPanel :provider="activeStressProvider"
                         :provider-color="providers.find(p => p.id === activeStressProvider)?.color || '#6366f1'" />
      </div>
    </section>

    <!-- BENCHMARK HISTORY -->
    <section class="section">
      <div class="section-header">
        <h2 class="section-title">Benchmark History</h2>
        <button v-if="benchmarks.results.length" class="clear-btn" @click="benchmarks.clear()">🗑 Clear</button>
      </div>
      <div v-if="!benchmarks.results.length" class="bench-empty">
        <div class="be-icon">⚡</div>
        <div class="be-title">No benchmarks yet</div>
        <div class="be-sub">Go to any provider page and run the Stress Test to see comparative data here.</div>
      </div>
      <div v-else class="bench-layout">
        <div class="bench-chart-wrap">
          <div class="bcw-label">Generation time per export (ms)</div>
          <canvas ref="benchBarRef" height="200"></canvas>
        </div>
        <div class="bench-stats">
          <div v-for="stat in benchStats" :key="stat.label" class="bstat">
            <div class="bstat-val" :style="`color:${stat.color}`">{{ stat.val }}</div>
            <div class="bstat-lbl">{{ stat.label }}</div>
          </div>
        </div>
        <div class="bench-table-wrap">
          <div class="btw-label">Last 10 exports</div>
          <div class="btw-rows">
            <div class="btw-row btw-header">
              <span>Provider</span><span>Format</span><span>Time</span><span>Size</span><span>Questions</span>
            </div>
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

    <!-- DECISION GUIDE -->
    <section class="section">
      <h2 class="section-title">Choose the Right Provider</h2>
      <div class="decision-grid">
        <div v-for="d in decisionGuide" :key="d.scenario" class="dcard" :style="`--dc:${d.color}`">
          <div class="dcard-icon">{{ d.icon }}</div>
          <div class="dcard-scenario">{{ d.scenario }}</div>
          <div class="dcard-winner">→ {{ d.winner }}</div>
          <div class="dcard-reason">{{ d.reason }}</div>
        </div>
      </div>
    </section>

    <!-- STRESS TEST CTA -->
    <section class="stress-cta">
      <div class="sc-inner">
        <div>
          <h2>⚡ Stress test all four providers</h2>
          <p>Export the 500-question survey (100,000 rows) from each provider and compare performance.</p>
        </div>
        <div class="sc-btns">
          <router-link v-for="p in providers" :key="p.route" :to="p.route" class="sc-btn" :style="`--c:${p.color}`">
            {{ p.icon }} {{ p.shortName }}
          </router-link>
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
  import StressTestPanel from '../components/StressTestPanel.vue'

const benchmarks = useBenchmarkStore()
const store = useReportBuilderStore()
const radarRefs = ref<Record<string, HTMLCanvasElement>>({})
const benchBarRef = ref<HTMLCanvasElement>()
let benchBarChart: Chart | null = null
  const activeFilter = ref('all')
  const activeStressProvider = ref('jsreport')

const meta = [
  { val: '4', label: 'Providers' },
  { val: '4', label: 'Surveys' },
  { val: '100k+', label: 'Responses' },
  { val: '3', label: 'Formats' },
]

const providers = [
  {
    id: 'jsreport',
    route: '/providers/jsreport', track: 1, icon: '🌐',
    name: 'jsreport', shortName: 'jsreport',
    description: 'HTML + CSS + JavaScript → PDF. The only provider with real Chart.js charts in exported PDFs.',
    bestFor: 'Designer-owned HTML/CSS templates',
    tags: ['Chart.js in PDF', 'Handlebars', 'HTML Preview'],
    cost: 'OSS / Pro', costClass: 'freemium', color: '#a855f7',
    scores: [{ l: 'PDF Quality', v: 10 }, { l: 'Excel', v: 4 }, { l: 'Performance', v: 6 }],
  },
  {
    id: 'Syncfusion',
    route: '/providers/syncfusion', track: 2, icon: '💎',
    name: 'Syncfusion', shortName: 'Syncfusion',
    description: 'Native Office engine. Live Excel formulas, editable PPT charts, AES-256 encryption, PDF/UA.',
    bestFor: 'Native Office docs with live formulas',
    tags: ['Excel Formulas', 'Native Charts', 'PDF/UA'],
    cost: 'Community Free', costClass: 'free', color: '#14b8a6',
    scores: [{ l: 'Excel', v: 10 }, { l: 'PPT', v: 9 }, { l: 'Performance', v: 9 }],
  },
  {
    id: 'js-sync',
    route: '/providers/js-sync', track: 3, icon: '⚒️',
    name: 'jsreport + Syncfusion', shortName: 'Hybrid A',
    description: 'jsreport handles PDF (HTML templates, Chart.js). Syncfusion handles Excel and PPT (native quality).',
    bestFor: 'Teams needing HTML templates AND Excel formulas',
    tags: ['Designer PDF', 'Native Excel', 'Full Stack'],
    cost: 'Enterprise', costClass: 'paid', color: '#8b5cf6',
    scores: [{ l: 'PDF', v: 10 }, { l: 'Excel', v: 10 }, { l: 'Performance', v: 7 }],
  },
  {
    id: 'play-sync',
    route: '/providers/play-sync', track: 4, icon: '🎭',
    name: 'Playwright + Syncfusion', shortName: 'Hybrid B',
    description: 'Playwright replaces jsreport for PDF — faster cold start, zero extra Node.js runtime. Syncfusion handles Office.',
    bestFor: 'Teams already using Playwright for E2E tests',
    tags: ['Fast PDF', 'Native Excel', 'CI-friendly'],
    cost: 'Enterprise', costClass: 'paid', color: '#10b981',
    scores: [{ l: 'PDF', v: 9 }, { l: 'Excel', v: 10 }, { l: 'Performance', v: 9 }],
  },
]

const matrixFilters = [
  { id: 'all', label: 'All' },
  { id: 'output', label: '📄 Formats' },
  { id: 'charts', label: '📊 Charts' },
  { id: 'enterprise', label: '🔐 Enterprise' },
  { id: 'dx', label: '💰 Cost & DX' },
]

const fullMatrix = [
  { category: '📄 File Formats & Quality', filter: 'output', rows: [
    { feature: 'PDF generation', values: [true, true, true, true] },
    { feature: 'Excel (.xlsx)', values: ['HTML only', true, true, true] },
    { feature: 'PowerPoint (.pptx)', values: ['Basic', true, true, true] },
    { feature: 'HTML → PDF', values: [true, false, true, true] },
    { feature: 'Multi-sheet Excel', values: [false, true, true, true] },
  ]},
  { category: '📊 Charts & Visualization', filter: 'charts', rows: [
    { feature: 'Chart.js in PDF', values: [true, false, true, false] },
    { feature: 'Native editable Excel charts', values: [false, true, true, true] },
    { feature: 'Native editable PPT charts', values: [false, true, true, true] },
    { feature: 'Radar / donut / line chart', values: ['Chart.js', 'SF API', 'Chart.js', 'Static'] },
  ]},
  { category: '🔐 Security & Enterprise', filter: 'enterprise', rows: [
    { feature: 'AES-256 PDF encryption', values: [false, true, false, true] },
    { feature: 'Watermark', values: [true, true, true, true] },
    { feature: 'PDF/UA Accessibility', values: [false, true, false, true] },
    { feature: 'Excel live formulas', values: [false, true, true, true] },
    { feature: 'Conditional formatting', values: [false, true, true, true] },
  ]},
  { category: '💰 Cost & Developer Experience', filter: 'dx', rows: [
    { feature: 'License cost', values: ['OSS/Pro', 'Community Free', '2 licenses', '2 licenses'] },
    { feature: 'Chromium process required', values: [true, false, true, true] },
    { feature: 'HTML template support', values: [true, false, true, 'Tokens only'] },
    { feature: 'Cold start (first PDF)', values: ['2–5s', '< 100ms', '2–5s', '~200ms'] },
    { feature: 'Chart.js in PDF', values: [true, false, true, false] },
  ]},
]

const filteredMatrix = computed(() => {
  if (activeFilter.value === 'all') return fullMatrix
  return fullMatrix.filter(g => g.filter === activeFilter.value)
})

const radarDims = ['PDF Quality', 'Excel', 'PPT', 'Charts', 'Cost', 'HTML Templates', 'Enterprise', 'Performance']
const radarData: Record<string, number[]> = {
  jsreport:   [10, 4, 2, 10, 7, 10, 4, 6],
  Syncfusion: [7, 10, 9,  6, 9,  0, 10, 9],
  'Hybrid A': [10, 10, 9, 10, 6, 10, 8, 7],
  'Hybrid B': [9, 10, 9,  5, 7,  3,  8, 9],
}

const decisionGuide = [
  { icon: '🎨', scenario: 'Designer owns the report template', winner: 'jsreport or Hybrid A', reason: 'HTML/CSS templates — designer works in their native medium, no C# changes needed for layout updates.', color: '#a855f7' },
  { icon: '🧮', scenario: 'Excel must recalculate when opened', winner: 'Syncfusion or Hybrid A/B', reason: 'XlsIO writes real =AVERAGE() formulas. Recipient opens Excel, changes a value — chart updates automatically.', color: '#14b8a6' },
  { icon: '📊', scenario: 'Chart.js graphs directly in PDF', winner: 'jsreport or Hybrid A', reason: 'Only providers that run actual JavaScript (Chart.js) inside Chromium during render.', color: '#8b5cf6' },
  { icon: '⚡', scenario: 'Already using Playwright for E2E tests', winner: 'Hybrid B (Playwright + Syncfusion)', reason: 'Your CI already has Chromium. Adding PDF = zero new infrastructure. Faster cold start than jsreport.', color: '#10b981' },
  { icon: '🔐', scenario: 'Need AES-256 encryption or PDF/UA', winner: 'Syncfusion or Hybrid A/B', reason: 'AES-256, password protection, WCAG 2.1 AA accessibility — native to Syncfusion Essential PDF.', color: '#f59e0b' },
  { icon: '💰', scenario: 'Zero license budget', winner: 'jsreport OSS + Syncfusion Community', reason: 'jsreport OSS is free for most uses. Syncfusion Community is free under $1M revenue. Though combined they add complexity.', color: '#ef4444' },
]

const benchStats = computed(() => {
  const rs = benchmarks.results
  if (!rs.length) return []
  const fastest = rs.reduce((a, b) => a.generationMs < b.generationMs ? a : b)
  const avgMs = Math.round(rs.reduce((s, r) => s + r.generationMs, 0) / rs.length)
  const totalSize = rs.reduce((s, r) => s + r.fileSizeBytes, 0)
  return [
    { val: `${fastest.generationMs}ms`, label: `Fastest (${fastest.provider})`, color: '#22c55e' },
    { val: `${avgMs}ms`, label: 'Average', color: '#f59e0b' },
    { val: `${rs.length}`, label: 'Total exports', color: '#3b82f6' },
    { val: formatSize(totalSize), label: 'Total size', color: '#a855f7' },
  ]
})

function speedClass(ms: number) { return ms < 500 ? 'fast' : ms < 2000 ? 'ok' : 'slow' }
function formatSize(bytes: number) {
  if (!bytes) return '—'
  if (bytes > 1_000_000) return `${(bytes / 1_000_000).toFixed(1)} MB`
  return `${(bytes / 1024).toFixed(0)} KB`
}

function drawRadarCharts() {
  providers.forEach(p => {
    const canvas = radarRefs.value[p.shortName]
    if (!canvas) return
    const existing = (canvas as HTMLCanvasElement & { _chart?: Chart })._chart
    if (existing) existing.destroy()
    const inst = new Chart(canvas, {
      type: 'radar',
      data: {
        labels: radarDims,
        datasets: [{ data: (radarData[p.shortName] ?? []) as number[], backgroundColor: `${p.color}22`, borderColor: p.color, pointBackgroundColor: p.color, borderWidth: 2, pointRadius: 3 }],
      },
      options: { plugins: { legend: { display: false } }, scales: { r: { min: 0, max: 10, ticks: { color: '#475569', stepSize: 2, font: { size: 8 } }, grid: { color: '#1e293b' }, pointLabels: { color: '#94a3b8', font: { size: 9 } } } } },
    })
    ;(canvas as HTMLCanvasElement & { _chart?: Chart })._chart = inst
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
      datasets: [{ data: recent.map(r => r.generationMs), backgroundColor: recent.map(r => { const p = providers.find(p => p.shortName === r.provider); return (p?.color ?? '#6b7280') + 'cc' }), borderRadius: 4 }],
    },
    options: { plugins: { legend: { display: false } }, scales: { y: { ticks: { color: '#94a3b8', font: { size: 10 } }, grid: { color: '#1e293b' } }, x: { ticks: { color: '#94a3b8', font: { size: 9 }, maxRotation: 30 }, grid: { display: false } } } },
  })
}

onMounted(async () => {
  if (!store.surveys.length) store.surveys = await fetchSurveys()
  setTimeout(drawRadarCharts, 100)
})

watch(() => benchmarks.results.length, () => { setTimeout(drawBenchChart, 100) })
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=DM+Sans:wght@300;400;500;600;700;800&family=DM+Mono:wght@400;500&display=swap');

.dashboard { font-family: 'DM Sans', sans-serif; max-width: 1300px; margin: 0 auto; color: #e2e8f0; }

/* HERO */
.hero { position: relative; overflow: hidden; border-radius: 20px; background: #050d1f; margin-bottom: 48px; padding: 64px; }
.hero-bg { position: absolute; inset: 0; }
.hbg-grid { position: absolute; inset: 0; background-image: linear-gradient(rgba(255,255,255,0.025) 1px,transparent 1px),linear-gradient(90deg,rgba(255,255,255,0.025) 1px,transparent 1px); background-size: 50px 50px; }
.hbg-glow { position: absolute; width: 600px; height: 600px; background: radial-gradient(circle,rgba(99,102,241,0.12) 0%,transparent 70%); top: -200px; right: -100px; }
.hero-inner { position: relative; z-index: 1; }
.hero-tag { font-family: 'DM Mono', monospace; font-size: 11px; text-transform: uppercase; letter-spacing: 2px; color: rgba(255,255,255,0.35); margin-bottom: 16px; }
h1 { font-size: 52px; font-weight: 800; letter-spacing: -2px; margin: 0 0 14px; color: #fff; line-height: 1.1; }
.hero > .hero-inner > p { font-size: 17px; color: rgba(255,255,255,0.45); margin: 0 0 36px; max-width: 560px; line-height: 1.6; }
.hero-pills { display: flex; gap: 14px; flex-wrap: wrap; }
.pill { background: rgba(255,255,255,0.04); border: 1px solid rgba(255,255,255,0.07); border-radius: 12px; padding: 12px 20px; text-align: center; }
.pill-val { display: block; font-size: 24px; font-weight: 800; color: #fff; font-family: 'DM Mono', monospace; }
.pill-lbl { font-size: 11px; color: rgba(255,255,255,0.35); text-transform: uppercase; letter-spacing: 1px; }

/* SECTIONS */
.section { margin-bottom: 48px; }
.section-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.section-title { font-size: 22px; font-weight: 700; margin: 0 0 16px; color: #f1f5f9; }

/* PROVIDER CARDS */
.provider-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; }
.pcard { display: flex; flex-direction: column; background: #050d1f; border: 1px solid rgba(255,255,255,0.05); border-top: 3px solid var(--c); border-radius: 14px; padding: 22px; text-decoration: none; color: #e2e8f0; transition: all 0.2s; }
.pcard:hover { transform: translateY(-3px); box-shadow: 0 16px 40px rgba(0,0,0,0.3); border-color: var(--c); }
.pcard-top { display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px; }
.pcard-icon { font-size: 24px; }
.pcard-track { font-size: 10px; text-transform: uppercase; letter-spacing: 1px; color: var(--c); font-family: 'DM Mono', monospace; }
.pcard h3 { font-size: 14px; font-weight: 700; margin: 0 0 7px; color: #fff; }
.pcard > p { font-size: 12px; color: #64748b; line-height: 1.5; flex: 1; margin: 0 0 10px; }
.pcard-verdict { background: rgba(255,255,255,0.03); border-radius: 5px; padding: 5px 9px; margin-bottom: 10px; }
.pv-lbl { font-size: 9px; color: #475569; text-transform: uppercase; letter-spacing: 0.5px; display: block; margin-bottom: 1px; }
.pv-val { font-size: 11px; color: var(--c); font-weight: 600; }
.pcard-tags { display: flex; flex-wrap: wrap; gap: 4px; margin-bottom: 10px; }
.tag { font-size: 9px; padding: 2px 7px; background: color-mix(in srgb,var(--c) 10%,transparent); color: var(--c); border-radius: 100px; border: 1px solid color-mix(in srgb,var(--c) 25%,transparent); }
.pcard-scores { display: flex; flex-direction: column; gap: 5px; margin-bottom: 12px; }
.ps-row { display: flex; align-items: center; gap: 7px; }
.ps-l { font-size: 10px; color: #64748b; width: 80px; flex-shrink: 0; }
.ps-bar { flex: 1; height: 3px; background: #1e293b; border-radius: 2px; overflow: hidden; }
.ps-fill { height: 100%; background: var(--c); border-radius: 2px; }
.ps-v { font-size: 9px; color: #475569; width: 30px; text-align: right; }
.pcard-foot { display: flex; justify-content: space-between; align-items: center; padding-top: 10px; border-top: 1px solid rgba(255,255,255,0.05); }
.pcard-cost { font-size: 12px; font-weight: 700; }
.pcard-cost.free { color: #22c55e; }
.pcard-cost.paid { color: #f59e0b; }
.pcard-cost.freemium { color: #a855f7; }
.pcard-arrow { color: #475569; transition: transform 0.15s; }
.pcard:hover .pcard-arrow { transform: translateX(4px); color: var(--c); }

/* MATRIX */
.matrix-tabs { display: flex; gap: 6px; margin-bottom: 14px; flex-wrap: wrap; }
.mtab { font-size: 12px; padding: 5px 14px; border-radius: 100px; border: 1px solid #1e293b; background: transparent; color: #64748b; cursor: pointer; transition: all 0.15s; font-family: 'DM Sans', sans-serif; }
.mtab.active { background: #0f172a; border-color: #475569; color: #e2e8f0; }
.matrix-wrap { overflow-x: auto; border-radius: 12px; border: 1px solid #1e293b; }
.matrix { width: 100%; border-collapse: collapse; }
.matrix th { background: #0a1628; padding: 12px 14px; text-align: left; font-size: 12px; font-weight: 700; border-bottom: 2px solid #1e293b; white-space: nowrap; }
.feat-col { width: 220px; color: #475569; }
.mth-inner { display: flex; align-items: center; gap: 6px; }
.cat-row { background: #0a1628; padding: 7px 14px; font-size: 11px; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; color: #94a3b8; }
.matrix tr:hover td { background: rgba(255,255,255,0.015); }
.matrix td { padding: 9px 14px; border-bottom: 1px solid #0f172a; font-size: 12px; }
.feat-name { color: #94a3b8; font-weight: 500; }
.feat-cell { text-align: center; }
.v-yes { color: #22c55e; font-size: 14px; font-weight: 700; }
.v-no { color: #ef4444; font-size: 14px; }
.v-partial { font-size: 11px; color: #94a3b8; }

/* RADAR */
.radar-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; }
.radar-card { background: #050d1f; border-radius: 10px; padding: 16px; border: 1px solid #1e293b; }
.rc-title { font-size: 13px; font-weight: 700; margin-bottom: 8px; }

/* BENCHMARK */
.bench-layout { display: grid; grid-template-columns: 1fr 260px; gap: 16px; }
.bench-chart-wrap { background: #050d1f; border: 1px solid #1e293b; border-radius: 12px; padding: 18px; }
.bcw-label { font-size: 12px; font-weight: 600; color: #94a3b8; margin-bottom: 10px; }
.bench-stats { display: grid; grid-template-columns: 1fr 1fr; gap: 8px; align-content: start; }
.bstat { background: #050d1f; border: 1px solid #1e293b; border-radius: 10px; padding: 14px; }
.bstat-val { font-size: 20px; font-weight: 800; font-family: 'DM Mono', monospace; }
.bstat-lbl { font-size: 10px; color: #64748b; margin-top: 3px; }
.bench-table-wrap { background: #050d1f; border: 1px solid #1e293b; border-radius: 12px; overflow: hidden; grid-column: 1; }
.btw-label { font-size: 12px; font-weight: 600; padding: 12px 14px; background: #0a1628; border-bottom: 1px solid #1e293b; color: #94a3b8; }
.btw-rows { }
.btw-row { display: grid; grid-template-columns: 100px 55px 65px 65px 50px; padding: 7px 14px; border-bottom: 1px solid #0f172a; font-size: 11px; }
.btw-row.btw-header { font-size: 10px; color: #475569; font-weight: 700; text-transform: uppercase; background: #050d1f; }
.br-provider { color: #94a3b8; font-weight: 600; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.br-format { color: #64748b; font-family: 'DM Mono', monospace; font-size: 10px; }
.br-time.fast { color: #22c55e; font-weight: 700; }
.br-time.ok { color: #f59e0b; font-weight: 700; }
.br-time.slow { color: #ef4444; font-weight: 700; }
.br-size { color: #64748b; }
.br-q { color: #475569; }
.bench-empty { padding: 48px; text-align: center; background: #050d1f; border: 1px dashed #1e293b; border-radius: 12px; }
.be-icon { font-size: 36px; margin-bottom: 10px; }
.be-title { font-size: 16px; font-weight: 700; color: #e2e8f0; margin-bottom: 5px; }
.be-sub { font-size: 12px; color: #475569; max-width: 380px; margin: 0 auto; line-height: 1.5; }
.clear-btn { font-size: 12px; padding: 5px 12px; border-radius: 6px; border: 1px solid #1e293b; background: transparent; color: #64748b; cursor: pointer; font-family: 'DM Sans', sans-serif; }
.clear-btn:hover { background: #ef4444; color: white; border-color: #ef4444; }

/* DECISION */
.decision-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 14px; }
.dcard { background: #050d1f; border: 1px solid rgba(255,255,255,0.05); border-left: 4px solid var(--dc); border-radius: 12px; padding: 18px; }
.dcard-icon { font-size: 22px; margin-bottom: 6px; }
.dcard-scenario { font-size: 12px; font-weight: 700; color: #e2e8f0; margin-bottom: 4px; }
.dcard-winner { font-size: 12px; font-weight: 700; color: var(--dc); margin-bottom: 4px; }
.dcard-reason { font-size: 11px; color: #64748b; line-height: 1.5; }

/* STRESS CTA */
.stress-cta { background: #050d1f; border: 1px solid #1e293b; border-radius: 16px; padding: 28px; }
.sc-inner { display: flex; justify-content: space-between; align-items: center; gap: 24px; flex-wrap: wrap; }
.stress-cta h2 { font-size: 18px; font-weight: 700; margin: 0 0 6px; }
.stress-cta p { font-size: 13px; color: #64748b; margin: 0; }
.sc-btns { display: flex; gap: 8px; flex-wrap: wrap; }
.sc-btn { padding: 9px 18px; background: color-mix(in srgb,var(--c) 12%,transparent); color: var(--c); border: 1px solid color-mix(in srgb,var(--c) 28%,transparent); border-radius: 8px; text-decoration: none; font-weight: 600; font-size: 13px; transition: all 0.15s; }
.sc-btn:hover { background: var(--c); color: white; }


  .stress-controls {
    display: flex;
    gap: 10px;
    margin-bottom: 20px;
  }

  .st-btn {
    background: #050d1f;
    border: 1px solid #1e293b;
    color: #64748b;
    padding: 8px 16px;
    border-radius: 8px;
    cursor: pointer;
    font-family: 'DM Sans', sans-serif;
    font-weight: 600;
    transition: all 0.2s;
  }

    .st-btn.active {
      border-color: var(--c);
      color: var(--c);
      background: color-mix(in srgb, var(--c) 10%, transparent);
    }

</style>
