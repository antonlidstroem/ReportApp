<template>
  <div class="page">

    <!-- ── HERO ─────────────────────────────────────────────────────── -->
    <div class="hero">
      <div class="hero-rings"><div v-for="i in 4" :key="i" class="ring" :style="`--i:${i}`"></div></div>
      <div class="hero-content">
        <router-link to="/" class="back">← Dashboard</router-link>
        <div class="eyebrow">Track 1 · Web-Standard Engine</div>
        <h1>jsreport</h1>
        <div class="tagline">HTML + CSS + JavaScript → PDF</div>
        <p>Den enda providern som kör riktig Chart.js inuti rapporten. Chromium renderar din HTML exakt som en webbläsare — varje gradient, varje diagram — och tar en PDF-skärmbild.</p>
        <div class="verdicts">
          <div class="verdict ok">✓ Bäst för: Designer-ägda HTML/CSS-mallar</div>
          <div class="verdict warn">⚠ Undvik om: Du behöver live Excel-formler</div>
        </div>
        <div class="badges">
          <span class="b purple">★ Chart.js i PDF</span>
          <span class="b purple">✓ Handlebars</span>
          <span class="b green">✓ Preview = PDF</span>
          <span class="b red">✗ Ingen native PPT</span>
          <span class="b red">✗ 2–5s cold start</span>
        </div>
      </div>
      <div class="hero-preview">
        <div class="browser">
          <div class="browser-bar">
            <div class="dots"><span></span><span></span><span></span></div>
            <span class="browser-url">{{SurveyTitle}} → report.pdf</span>
          </div>
          <div class="browser-body" v-if="heroData">
            <div class="prev-title">{{ heroData.title }}</div>
            <canvas ref="heroCanvas" height="100" class="prev-canvas"></canvas>
            <div class="prev-rows">
              <div v-for="(q, i) in heroData.questions.slice(0, 4)" :key="i" class="prev-row">
                <span class="pr-t">{{ q.text.substring(0, 32) }}</span>
                <div class="pr-bar"><div :style="`width:${q.avg*20}%;background:${sc(q.avg)}`"></div></div>
                <span :style="`color:${sc(q.avg)}`" class="pr-v">{{ q.avg.toFixed(1) }}</span>
              </div>
            </div>
          </div>
          <div class="browser-body" v-else>
            <div class="prev-loading">Laddar data...</div>
          </div>
        </div>
      </div>
    </div>

    <!-- ── CAP 1: CHART.JS I PDF ─────────────────────────────────── -->
    <section class="section">
      <div class="eyebrow-sm">CAPABILITY 1 — UNIKT FÖR jsreport</div>
      <h2>Chart.js körs i din rapport — redigera data, se PDF-resultatet</h2>
      <p class="sub">Chromium väntar på <code>networkidle</code> — all JS har körts, alla diagram är målade — och tar sedan en skärmbild som PDF. Flytta reglagen nedan och se diagrammet uppdateras. Det är exakt vad PDF:en kommer innehålla.</p>

      <div class="demo-card">
        <div class="demo-controls">
          <div class="dc-header"><span>Enkätdata (redigera)</span><button class="btn-sm" @click="randomize">↻ Slumpa</button></div>
          <div class="dc-sliders">
            <div v-for="(row, i) in chartRows" :key="i" class="dc-row">
              <span class="dc-lbl">{{ row.label }}</span>
              <input type="range" min="1" max="5" step="0.1" v-model.number="row.value" class="dc-range" :style="`--c:${sc(row.value)}`" @input="updateChart" />
              <span :style="`color:${sc(row.value)}`" class="dc-val">{{ row.value.toFixed(1) }}</span>
            </div>
          </div>
          <div class="dc-chart-types">
            <button v-for="ct in chartTypes" :key="ct.id" :class="['ct-btn', { active: activeCT === ct.id }]" @click="activeCT = ct.id; rebuildChart()">{{ ct.icon }} {{ ct.label }}</button>
          </div>
          <!-- EXPORT BUTTON FOR THIS DEMO -->
          <div class="demo-export-box">
            <div class="deb-title">Exportera det du ser ovan som PDF</div>
            <div class="deb-note">Diagrammet renderas av Chromium — den exporterade PDF:en är identisk med vyn till höger.</div>
            <InlineExport
              provider="jsreport"
              :supports-html-template="true"
              :html-template="fullTemplate"
              :survey-id-override="demoSurveyId"
            />
          </div>
        </div>
        <div class="demo-chart">
          <div class="dc-chart-label">Live Chart.js (identisk med PDF)</div>
          <canvas ref="liveChartCanvas" height="260"></canvas>
        </div>
      </div>
    </section>

    <!-- ── CAP 2: LIVE PDF PREVIEW ──────────────────────────────── -->
    <section class="section">
      <div class="eyebrow-sm">CAPABILITY 2</div>
      <h2>Live PDF-förhandsgranskning — exakt den HTML som skickas till Chromium</h2>
      <p class="sub">Iframe:n nedan renderar den exakta HTML-mallen som skickas till jsreport. Byt mall och se hur PDF:en förändras. Exportknapparna till höger genererar den visade mallen med riktig enkätdata.</p>

      <div class="preview-section">
        <div class="preview-controls">
          <div class="pc-header">Mall-väljare</div>
          <button v-for="t in previewTemplates" :key="t.id" :class="['tmpl-btn', { active: activePreviewTmpl === t.id }]" @click="switchPreview(t.id as 'full' | 'executive')">
            {{ t.icon }} {{ t.name }}
          </button>
          <div class="preview-export-box">
            <div class="peb-title">Exportera mallen med riktig data</div>
            <p class="peb-note">Välj en enkät nedan och exportera. PDF:en genereras av jsreport med Chromium och Chart.js.</p>
            <SurveyPicker />
            <InlineExport
              provider="jsreport"
              :supports-html-template="true"
              :html-template="activeRichTemplate"
            />
          </div>
        </div>
        <div class="preview-frame-wrap">
          <div class="preview-label">{{ previewLabel }}</div>
          <iframe ref="previewFrame" class="preview-iframe" sandbox="allow-same-origin allow-scripts" title="PDF-förhandsgranskning"></iframe>
        </div>
      </div>
    </section>

    <!-- ── CAP 3: HANDLEBARS ──────────────────────────────────────── -->
    <section class="section">
      <div class="eyebrow-sm">CAPABILITY 3</div>
      <h2>Handlebars — rapportlogik i HTML, inte i C#</h2>
      <p class="sub">Aktivera funktioner nedan och se mallen uppdateras i realtid. Det är det designers kontrollerar — inga C#-ändringar krävs.</p>

      <div class="hb-demo">
        <div class="hbd-toggles">
          <label v-for="t in hbToggles" :key="t.id" class="hbt">
            <input type="checkbox" v-model="t.enabled" />
            <div class="hbt-track"><div class="hbt-thumb"></div></div>
            <div><span class="hbt-title">{{ t.title }}</span><span class="hbt-desc">{{ t.desc }}</span></div>
          </label>
        </div>
        <div class="hbd-code">
          <div class="hbd-code-hdr"><span>Handlebars-mall (uppdateras automatiskt)</span><span class="hbd-lang">HTML</span></div>
          <pre class="hbd-pre"><code>{{ generatedTemplate }}</code></pre>
        </div>
      </div>
    </section>

    <!-- ── SW ──────────────────────────────────────────────────────── -->
    <section class="section">
      <div class="eyebrow-sm">ÄRLIG BEDÖMNING</div>
      <h2>Styrkor och svagheter</h2>
      <div class="sw-grid">
        <div class="sw-col sw-purple">
          <h3>🌐 Styrkor</h3>
          <div v-for="s in strengths" :key="s.t" class="sw-item"><div class="sw-t">{{ s.t }}</div><div class="sw-d">{{ s.d }}</div></div>
        </div>
        <div class="sw-col sw-yellow">
          <h3>⚠ Svagheter</h3>
          <div v-for="w in weaknesses" :key="w.t" class="sw-item"><div class="sw-t">{{ w.t }}</div><div class="sw-d">{{ w.d }}</div></div>
        </div>
      </div>
    </section>

    <StressTestPanel provider="jsreport" provider-color="#a855f7" />

    <!-- ── WORKSPACE ──────────────────────────────────────────────── -->
    <section class="section">
      <div class="eyebrow-sm">EXPORT-ARBETSYTA</div>
      <h2>Exportera med din data</h2>
      <p class="sub">Välj enkät, välj en rik mall med Chart.js-diagram, och exportera. "Fullständig" och "Executive" mallarna inkluderar diagram renderade av Chromium.</p>
      <div class="workspace">
        <div><SurveyPicker /><ModuleList /></div>
        <div><ExportButton provider="jsreport" :supports-html-template="true" /><TemplateDesigner provider-name="jsreport" :supports-html-template="true" /></div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, nextTick } from 'vue'
import SurveyPicker from '../../components/SurveyPicker.vue'
import ModuleList from '../../components/ModuleList.vue'
import TemplateDesigner from '../../components/TemplateDesigner.vue'
import ExportButton from '../../components/ExportButton.vue'
import InlineExport from '../../components/InlineExport.vue'
import StressTestPanel from '../../components/StressTestPanel.vue'
import Chart from 'chart.js/auto'
import { fetchSurveys, fetchQuestions, buildPreviewHtml, RICH_TEMPLATES } from '../../composables/useApi'

// ── Seeded data ────────────────────────────────────────────────────────────
const heroCanvas      = ref<HTMLCanvasElement>()
const liveChartCanvas = ref<HTMLCanvasElement>()
const previewFrame    = ref<HTMLIFrameElement>()
let   liveChart: Chart | null = null

interface QRow { text: string; category: string; avg: number; responses: number }
interface HeroData { title: string; company: string; questions: QRow[] }

const heroData     = ref<HeroData | null>(null)
const demoSurveyId = ref(1)

// Slider rows
const chartRows = ref([
  { label: 'Ledarskap',    value: 4.1 },
  { label: 'Psykosocialt', value: 3.2 },
  { label: 'Hälsa',        value: 3.8 },
  { label: 'Säkerhet',     value: 4.5 },
  { label: 'Resurser',     value: 3.5 },
])

const sc = (v: number) => v >= 4 ? '#22c55e' : v >= 3 ? '#f59e0b' : '#ef4444'

const chartTypes = [
  { id: 'bar',      icon: '📊', label: 'Stapel'   },
  { id: 'line',     icon: '📈', label: 'Linje'    },
  { id: 'radar',    icon: '🎯', label: 'Radar'    },
  { id: 'doughnut', icon: '🍩', label: 'Munkkaka' },
]
const activeCT = ref('bar')

function randomize() {
  chartRows.value.forEach(r => { r.value = Math.round((1 + Math.random() * 4) * 10) / 10 })
  rebuildChart()
  renderPreview()
}

function updateChart() {
  if (!liveChart) return
  const ds = liveChart.data.datasets[0]
  if (!ds) return
  // Use any to avoid complex Chart.js generic casting
  ;(ds as { data: number[] }).data = chartRows.value.map(r => r.value)
  if (activeCT.value === 'bar') {
    ;(ds as { backgroundColor: string[] }).backgroundColor =
      chartRows.value.map(r => sc(r.value) + 'cc')
  }
  liveChart.update('none')
}

function rebuildChart() {
  if (!liveChartCanvas.value) return
  if (liveChart) liveChart.destroy()
  const isRound = activeCT.value === 'doughnut'
  liveChart = new Chart(liveChartCanvas.value, {
    type: activeCT.value as 'bar' | 'line' | 'radar' | 'doughnut',
    data: {
      labels: chartRows.value.map(r => r.label),
      datasets: [{
        data: chartRows.value.map(r => r.value),
        backgroundColor: isRound
          ? ['#a855f7','#7c3aed','#6d28d9','#5b21b6','#4c1d95']
          : chartRows.value.map(r => sc(r.value) + 'cc'),
        borderColor: isRound ? 'transparent' : '#a855f7',
        borderWidth: 2,
        borderRadius: activeCT.value === 'bar' ? 4 : undefined,
        fill: activeCT.value === 'line' ? true : undefined,
        tension: 0.4,
        pointBackgroundColor: '#a855f7',
      }],
    },
    options: {
      animation: { duration: 280 },
      plugins: {
        legend: {
          display: isRound,
          labels: { color: '#94a3b8', font: { size: 10 } }
        }
      },
      scales: activeCT.value === 'radar'
        ? { r: { min: 0, max: 5, ticks: { color: '#666', stepSize: 1, font: { size: 8 } }, grid: { color: '#2d1060' }, pointLabels: { color: '#94a3b8', font: { size: 9 } } } }
        : isRound ? {}
        : {
            y: { min: 0, max: 5, ticks: { color: '#888', font: { size: 10 } }, grid: { color: '#1e1e3f' } },
            x: { ticks: { color: '#888', font: { size: 10 } }, grid: { color: '#1e1e3f' } }
          },
    },
  })
}

// ── Full template for demo export ──────────────────────────────────────────
const fullTemplate = computed(() => RICH_TEMPLATES.jsreportFull)

// ── Live preview ──────────────────────────────────────────────────────────
const activePreviewTmpl = ref<'full' | 'executive'>('full')

const previewTemplates = [
  { id: 'full',      icon: '📊', name: 'Fullständig rapport' },
  { id: 'executive', icon: '🌙', name: 'Executive (mörkt)'   },
]

const previewLabel = computed(() =>
  activePreviewTmpl.value === 'full'
    ? 'Handlebars-mall renderad av Chromium — inkluderar Chart.js stapeldiagram'
    : 'Executive-mall med kategorimedelvärden — renderad av Chromium'
)

const activeRichTemplate = computed(() =>
  activePreviewTmpl.value === 'executive'
    ? RICH_TEMPLATES.jsreportExecutive
    : RICH_TEMPLATES.jsreportFull
)

function switchPreview(id: 'full' | 'executive') {
  activePreviewTmpl.value = id
  renderPreview()
}

function renderPreview() {
  const frame = previewFrame.value
  if (!frame || !heroData.value) return

  const html = buildPreviewHtml(activePreviewTmpl.value, {
    title:     heroData.value.title,
    company:   heroData.value.company,
    questions: heroData.value.questions,
  })

  nextTick(() => {
    const doc = frame.contentDocument
    if (doc) { doc.open(); doc.write(html); doc.close() }
  })
}

// ── Handlebars demo ───────────────────────────────────────────────────────
const hbToggles = ref([
  { id: 'exec',  title: 'Executive-block',           desc: 'KPI-rad med OverallAverage, QuestionCount',  enabled: true  },
  { id: 'trend', title: 'Månadstrend (Chart.js)',    desc: 'Linjediagram via Chromium JS-körning',       enabled: true  },
  { id: 'high',  title: 'Markera låga poäng',        desc: 'Röd rad när AverageValue < 3.0',             enabled: false },
  { id: 'cats',  title: 'Gruppera per kategori',     desc: 'Sektionsrubriker, nästlad #each',            enabled: false },
])

const generatedTemplate = computed(() => {
  const L: string[] = []
  if (hbToggles.value.find(t => t.id === 'exec')?.enabled) {
    L.push('<!-- Executive block -->')
    L.push('<div class="kpis">')
    L.push('  <b>{{OverallAverage}}</b> snitt · {{QuestionCount}} frågor')
    L.push('</div>')
    L.push('')
  }
  if (hbToggles.value.find(t => t.id === 'cats')?.enabled)
    L.push('{{#each CategoryGroups}}\n<h2>{{name}}</h2>')

  if (hbToggles.value.find(t => t.id === 'high')?.enabled) {
    L.push('{{#each QuestionSummaries}}')
    L.push('<div class="row {{#if (lt AverageValue 3)}}low{{/if}}">')
    L.push('  <span>{{Text}}</span><b>{{AverageValue}}</b>')
    L.push('</div>\n{{/each}}')
  } else {
    L.push('{{#each QuestionSummaries}}')
    L.push('<div class="row">')
    L.push('  <span>Q{{@index}}: {{Text}}</span>')
    L.push('  <b>{{AverageValue}}</b>')
    L.push('</div>\n{{/each}}')
  }

  if (hbToggles.value.find(t => t.id === 'cats')?.enabled)
    L.push('{{/each}}')

  if (hbToggles.value.find(t => t.id === 'trend')?.enabled) {
    L.push('')
    L.push('<!-- Chart.js körs i Chromium -->')
    L.push('<canvas id="trend"></canvas>')
    L.push('<script>')
    L.push("  new Chart('trend', { type:'line', data: {")
    L.push("    labels:[{{#each Trends}}'{{MonthName}}'{{#unless @last}},{{/unless}}{{/each}}],")
    L.push('    datasets:[{data:[{{#each Trends}}{{AverageValue}},{{/each}}]}]')
    L.push('  }});\n<\\/script>')
  }
  return L.join('\n') || '<!-- Aktivera alternativ ovan -->'
})

// ── Strengths / weaknesses ────────────────────────────────────────────────
const strengths = [
  { t: 'Chart.js i PDF — unikt',  d: 'Enda providern som kör JavaScript i Chromium och fångar det renderade diagrammet som PDF.' },
  { t: 'Designer-ägda mallar',    d: 'Hela layouten är HTML/CSS. Frontend-designers kan ändra rapporter utan att röra C#.' },
  { t: 'Preview = PDF-utdata',    d: 'Rapporten ser exakt ut som din webbapp. Ingen diskrepans.' },
  { t: 'Handlebars-logik',        d: 'Villkor, loopar och hjälpare direkt i mallen. Minskar backend-kod drastiskt.' },
]
const weaknesses = [
  { t: 'Chromium-process krävs', d: 'jsreport startar en Node.js-process som hanterar Chromium. ~200MB disk, 2–5s cold start.' },
  { t: 'Ingen native PowerPoint', d: 'jsreport saknar PPT-recept. Fallback till Syncfusion behövs.' },
  { t: 'Begränsad Excel-kvalitet', d: 'HTML-till-xlsx: inga formler, ingen villkorsstyrd formatering, inga native-diagram.' },
  { t: 'Driftsättningskomplexitet', d: "Chromiums Linux-biblioteksberoenden kan vara problematiska i minimala Docker-bilder." },
]

// ── Lifecycle ─────────────────────────────────────────────────────────────
onMounted(async () => {
  try {
    const surveys = await fetchSurveys()
    const survey = surveys[0]  // could be undefined if no surveys
    if (survey) {
      demoSurveyId.value = survey.id
      const questions = await fetchQuestions(survey.id)

      const previewQs = questions.slice(0, 8).map(q => ({
        text: q.text,
        category: q.category,
        avg: parseFloat((2.5 + Math.random() * 2.5).toFixed(2)),
        responses: Math.floor(15 + Math.random() * 30),
      }))

      heroData.value = {
        title: survey.title,
        company: survey.companyName ?? '',
        questions: previewQs,
      }

      // Sync slider labels to real category names
      const cats = [...new Set(questions.map(q => q.category))].slice(0, 5)
      cats.forEach((cat, i) => {
        if (chartRows.value[i]) chartRows.value[i].label = cat
      })
    }
  } catch {
    // Fallback data when API is not available
    heroData.value = {
      title: 'Pulsmätning: Stress & Välmående',
      company: 'Storkommunen AB',
      questions: [
        { text: 'Utvilad på morgonen?',     category: 'Hälsa',        avg: 3.1, responses: 24 },
        { text: 'Återhämtningstid?',        category: 'Hälsa',        avg: 3.5, responses: 24 },
        { text: 'Stress kring deadlines?',  category: 'Psykosocialt', avg: 4.2, responses: 23 },
        { text: 'Stöd från närmaste chef?', category: 'Ledarskap',    avg: 4.0, responses: 22 },
        { text: 'Tydliga förväntningar?',   category: 'Ledarskap',    avg: 3.8, responses: 24 },
        { text: 'Socialt stöd på jobbet?',  category: 'Psykosocialt', avg: 4.3, responses: 23 },
        { text: 'Tillgång till resurser?',  category: 'Resurser',     avg: 3.6, responses: 24 },
        { text: 'Balans arbete/fritid?',    category: 'Hälsa',        avg: 2.9, responses: 22 },
      ],
    }
  }

  // Draw hero chart
  if (heroCanvas.value && heroData.value) {
    const qs = heroData.value.questions.slice(0, 4)
    new Chart(heroCanvas.value, {
      type: 'bar',
      data: {
        labels: qs.map((_, i) => `Q${i + 1}`),
        datasets: [{
          data: qs.map(q => q.avg),
          backgroundColor: qs.map(q => sc(q.avg) + 'aa'),
          borderColor: qs.map(q => sc(q.avg)),
          borderWidth: 2,
          borderRadius: 3,
        }],
      },
      options: {
        plugins: { legend: { display: false } },
        scales: {
          y: { min: 0, max: 5, ticks: { color: '#666', font: { size: 8 } }, grid: { color: '#1e1e2e' } },
          x: { ticks: { color: '#666', font: { size: 8 } }, grid: { display: false } }
        }
      },
    })
  }

  setTimeout(rebuildChart, 80)
  setTimeout(renderPreview, 200)
})
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;600;700;900&family=JetBrains+Mono:wght@400;500&display=swap');

.page { font-family: 'Outfit', sans-serif; max-width: 1300px; margin: 0 auto; color: #e2e8f0; }

/* HERO */
.hero { position: relative; overflow: hidden; border-radius: 20px; background: #0d0018; margin-bottom: 48px; display: grid; grid-template-columns: 1fr 380px; min-height: 420px; }
.hero-rings { position: absolute; inset: 0; pointer-events: none; }
.ring { position: absolute; top: 50%; left: 50%; transform: translate(-50%,-50%); border: 1px solid rgba(168,85,247,calc(.14 - var(--i)*.025)); border-radius: 50%; width: calc(var(--i)*120px); height: calc(var(--i)*120px); }
.hero-content { position: relative; z-index: 1; padding: 48px; }
.back { display: inline-block; color: rgba(255,255,255,.3); text-decoration: none; font-size: 13px; margin-bottom: 20px; transition: color .15s; }
.back:hover { color: #a855f7; }
.eyebrow { font-size: 11px; text-transform: uppercase; letter-spacing: 2px; color: #a855f7; margin-bottom: 12px; }
.eyebrow-sm { font-size: 10px; text-transform: uppercase; letter-spacing: 3px; color: #a855f7; margin-bottom: 6px; font-family: 'JetBrains Mono', monospace; }
h1 { font-size: 54px; font-weight: 900; margin: 0 0 4px; color: #fff; letter-spacing: -2px; }
.tagline { font-family: 'JetBrains Mono', monospace; font-size: 14px; color: #a855f7; margin-bottom: 16px; }
.hero-content > p { font-size: 14px; color: rgba(255,255,255,.55); max-width: 460px; line-height: 1.6; margin: 0 0 18px; }
.verdicts { display: flex; flex-direction: column; gap: 5px; margin-bottom: 18px; }
.verdict { font-size: 11px; padding: 5px 10px; border-radius: 5px; font-weight: 500; }
.verdict.ok   { background: rgba(34,197,94,.07);  border: 1px solid rgba(34,197,94,.2);  color: #22c55e; }
.verdict.warn { background: rgba(245,158,11,.07); border: 1px solid rgba(245,158,11,.2); color: #f59e0b; }
.badges { display: flex; flex-wrap: wrap; gap: 6px; }
.b { font-size: 10px; padding: 3px 10px; border-radius: 100px; border: 1px solid; font-weight: 500; }
.b.purple { color: #a855f7; border-color: rgba(168,85,247,.3); background: rgba(168,85,247,.06); }
.b.green  { color: #22c55e; border-color: rgba(34,197,94,.3);  background: rgba(34,197,94,.06);  }
.b.red    { color: #ef4444; border-color: rgba(239,68,68,.3);  background: rgba(239,68,68,.06);  }

.hero-preview { position: relative; z-index: 1; padding: 28px; display: flex; align-items: center; justify-content: center; border-left: 1px solid rgba(168,85,247,.1); }
.browser { background: #1a0533; border-radius: 10px; overflow: hidden; width: 100%; border: 1px solid rgba(168,85,247,.2); box-shadow: 0 16px 48px rgba(168,85,247,.15); }
.browser-bar { background: #2e1065; padding: 8px 12px; display: flex; align-items: center; gap: 8px; }
.dots { display: flex; gap: 4px; } .dots span { width: 8px; height: 8px; border-radius: 50%; background: rgba(255,255,255,.15); }
.browser-url { flex: 1; font-size: 9px; background: rgba(0,0,0,.3); border-radius: 4px; padding: 2px 8px; color: #94a3b8; font-family: 'JetBrains Mono', monospace; }
.browser-body { padding: 12px; }
.prev-title { font-size: 10px; font-weight: 700; color: #a855f7; margin-bottom: 8px; }
.prev-canvas { margin-bottom: 10px; width: 100%; }
.prev-rows { display: flex; flex-direction: column; gap: 5px; }
.prev-row { display: grid; grid-template-columns: 1fr 70px 26px; align-items: center; gap: 6px; }
.pr-t { font-size: 9px; color: #94a3b8; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.pr-bar { height: 5px; background: rgba(255,255,255,.08); border-radius: 3px; overflow: hidden; }
.pr-bar div { height: 100%; border-radius: 3px; }
.pr-v { font-size: 10px; font-weight: 700; text-align: right; }
.prev-loading { color: #475569; font-size: 12px; padding: 20px; text-align: center; }

/* SECTIONS */
.section { margin-bottom: 56px; }
.section > h2 { font-size: 22px; font-weight: 700; color: #fff; margin: 0 0 8px; }
.sub { font-size: 13px; color: #64748b; line-height: 1.6; margin: 0 0 22px; max-width: 700px; }
.sub code { background: rgba(168,85,247,.15); color: #c4b5fd; padding: 1px 5px; border-radius: 3px; font-family: 'JetBrains Mono', monospace; }

/* DEMO CARD */
.demo-card { display: grid; grid-template-columns: 340px 1fr; gap: 20px; background: #0d0018; border: 1px solid rgba(168,85,247,.15); border-radius: 14px; padding: 22px; }
.dc-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 14px; font-size: 12px; font-weight: 600; color: #94a3b8; }
.btn-sm { font-size: 11px; padding: 4px 10px; border-radius: 5px; border: 1px solid rgba(168,85,247,.3); background: transparent; color: #a855f7; cursor: pointer; font-family: 'Outfit', sans-serif; transition: all .12s; }
.btn-sm:hover { background: rgba(168,85,247,.1); }
.dc-sliders { display: flex; flex-direction: column; gap: 10px; margin-bottom: 14px; }
.dc-row { display: flex; align-items: center; gap: 8px; }
.dc-lbl { font-size: 11px; color: #64748b; width: 100px; flex-shrink: 0; }
.dc-range { flex: 1; -webkit-appearance: none; height: 4px; border-radius: 2px; background: rgba(168,85,247,.12); outline: none; }
.dc-range::-webkit-slider-thumb { -webkit-appearance: none; width: 14px; height: 14px; border-radius: 50%; background: var(--c,#a855f7); cursor: pointer; box-shadow: 0 0 6px var(--c,#a855f7); }
.dc-val { font-size: 12px; font-weight: 700; width: 28px; text-align: right; }
.dc-chart-types { display: flex; gap: 6px; flex-wrap: wrap; margin-bottom: 14px; }
.ct-btn { font-size: 11px; padding: 5px 9px; border-radius: 6px; border: 1px solid rgba(255,255,255,.08); background: transparent; color: #64748b; cursor: pointer; transition: all .12s; font-family: 'Outfit', sans-serif; }
.ct-btn.active { border-color: #a855f7; color: #a855f7; background: rgba(168,85,247,.08); }
.demo-export-box { background: rgba(168,85,247,.06); border: 1px solid rgba(168,85,247,.2); border-radius: 10px; padding: 14px; }
.deb-title { font-size: 12px; font-weight: 700; color: #c4b5fd; margin-bottom: 4px; }
.deb-note  { font-size: 11px; color: #475569; margin-bottom: 12px; line-height: 1.4; }
.demo-chart { display: flex; flex-direction: column; }
.dc-chart-label { font-size: 10px; color: #475569; margin-bottom: 10px; text-transform: uppercase; letter-spacing: 1px; }

/* PREVIEW SECTION */
.preview-section { display: grid; grid-template-columns: 280px 1fr; gap: 20px; }
.preview-controls { display: flex; flex-direction: column; gap: 10px; }
.pc-header { font-size: 11px; font-weight: 700; color: #94a3b8; text-transform: uppercase; letter-spacing: .5px; margin-bottom: 4px; }
.tmpl-btn { font-size: 12px; padding: 9px 14px; border-radius: 8px; border: 1px solid rgba(168,85,247,.2); background: transparent; color: #64748b; cursor: pointer; text-align: left; transition: all .12s; font-family: 'Outfit', sans-serif; }
.tmpl-btn.active { border-color: #a855f7; color: #e2e8f0; background: rgba(168,85,247,.08); }
.tmpl-btn:hover:not(.active) { border-color: rgba(168,85,247,.3); color: #94a3b8; }
.preview-export-box { background: rgba(168,85,247,.06); border: 1px solid rgba(168,85,247,.15); border-radius: 10px; padding: 14px; display: flex; flex-direction: column; gap: 10px; }
.peb-title { font-size: 12px; font-weight: 700; color: #c4b5fd; }
.peb-note  { font-size: 11px; color: #475569; line-height: 1.4; }
.preview-frame-wrap { display: flex; flex-direction: column; background: #f8fafc; border-radius: 12px; overflow: hidden; border: 1px solid rgba(255,255,255,.1); }
.preview-label { font-size: 10px; color: #475569; padding: 8px 14px; background: #1a0533; border-bottom: 1px solid rgba(168,85,247,.15); }
.preview-iframe { width: 100%; height: 520px; border: none; }

/* HANDLEBARS DEMO */
.hb-demo { display: grid; grid-template-columns: 280px 1fr; background: #0d0018; border: 1px solid rgba(168,85,247,.15); border-radius: 14px; overflow: hidden; }
.hbd-toggles { padding: 20px; display: flex; flex-direction: column; gap: 10px; border-right: 1px solid rgba(168,85,247,.1); }
.hbt { display: flex; align-items: flex-start; gap: 10px; cursor: pointer; }
.hbt input { display: none; }
.hbt-track { width: 34px; height: 18px; border-radius: 100px; background: #1e293b; position: relative; flex-shrink: 0; transition: background .2s; margin-top: 2px; }
.hbt-thumb { position: absolute; left: 2px; top: 2px; width: 14px; height: 14px; border-radius: 50%; background: white; transition: transform .2s; }
.hbt input:checked ~ .hbt-track { background: #a855f7; }
.hbt input:checked ~ .hbt-track .hbt-thumb { transform: translateX(16px); }
.hbt-title { display: block; font-size: 12px; font-weight: 600; color: #e2e8f0; margin-bottom: 1px; }
.hbt-desc  { font-size: 11px; color: #475569; line-height: 1.3; }
.hbd-code { display: flex; flex-direction: column; background: #060a12; }
.hbd-code-hdr { display: flex; justify-content: space-between; padding: 10px 16px; font-size: 10px; color: #475569; border-bottom: 1px solid rgba(168,85,247,.08); }
.hbd-lang { color: #a855f7; font-weight: 700; }
.hbd-pre { flex: 1; margin: 0; padding: 16px; overflow: auto; min-height: 200px; }
.hbd-pre code { font-family: 'JetBrains Mono', monospace; font-size: 10px; color: #c4b5fd; white-space: pre; line-height: 1.6; }

/* SW */
.sw-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
.sw-col { background: #0d0018; border-radius: 12px; padding: 24px; }
.sw-col.sw-purple { border-top: 3px solid #a855f7; }
.sw-col.sw-yellow { border-top: 3px solid #f59e0b; }
.sw-col h3 { font-size: 15px; font-weight: 700; margin: 0 0 16px; }
.sw-purple h3 { color: #a855f7; } .sw-yellow h3 { color: #f59e0b; }
.sw-item { padding: 10px 0; border-bottom: 1px solid rgba(255,255,255,.04); }
.sw-item:last-child { border-bottom: none; }
.sw-t { font-size: 12px; font-weight: 600; color: #e2e8f0; margin-bottom: 3px; }
.sw-d { font-size: 11px; color: #64748b; line-height: 1.5; }

/* WORKSPACE */
.workspace { display: grid; grid-template-columns: 340px 1fr; gap: 20px; }
.workspace > div { display: flex; flex-direction: column; gap: 16px; }
</style>
