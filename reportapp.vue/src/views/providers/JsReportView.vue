<template>
  <div class="jsreport-page">
    <!-- ── HERO ── -->
    <div class="hero">
      <div class="hero-bg">
        <div class="web-rings">
          <div class="ring" v-for="i in 4" :key="i" :style="`--i:${i}`"></div>
        </div>
        <div class="glow-purple"></div>
      </div>
      <div class="hero-inner">
        <router-link to="/" class="back-link">← Dashboard</router-link>
        <div class="hero-eyebrow">
          <span class="track-dot"></span>
          Spår 3 — Web-Standard Engine
        </div>
        <h1>jsreport</h1>
        <div class="hero-tagline">HTML + CSS + JavaScript → PDF</div>
        <p class="hero-sub">
          Den enda providern som kör <strong>riktig Chart.js</strong> inuti rapporten.
          Chromium renderar din HTML precis som webbläsaren — med alla grafer, animationer
          och CSS3-features intakta.
        </p>
        <div class="hero-badges">
          <span class="badge purple">★ Chart.js direkt i PDF</span>
          <span class="badge purple">✓ Handlebars templating</span>
          <span class="badge green">✓ Open Source + Pro</span>
          <span class="badge red">✗ PPT saknas nativt</span>
          <span class="badge red">✗ Kräver Chromium-process</span>
        </div>
      </div>
      <div class="hero-demo-panel">
        <div class="browser-chrome">
          <div class="bc-bar">
            <div class="bc-dots"><span></span><span></span><span></span></div>
            <div class="bc-url">report.html → PDF</div>
          </div>
          <div class="bc-content">
            <div class="report-preview">
              <h3 style="color:#a855f7;margin:0 0 8px;font-size:13px">{{ demoTitle }}</h3>
              <div class="rp-chart">
                <canvas ref="heroChart" height="120"></canvas>
              </div>
              <div class="rp-table">
                <div class="rpt-row header"><span>Fråga</span><span>Snitt</span></div>
                <div v-for="q in demoQs" :key="q.t" class="rpt-row">
                  <span>{{ q.t }}</span>
                  <span :style="`color:${hScoreColor(q.v)}`">{{ q.v.toFixed(1) }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ── THE JSREPORT PIPELINE ── -->
    <section class="section">
      <h2 class="section-title">Hur jsreport fungerar</h2>
      <div class="pipeline">
        <div class="pipe-step" v-for="(step, i) in pipeline" :key="i">
          <div class="ps-num">{{ i + 1 }}</div>
          <div class="ps-icon">{{ step.icon }}</div>
          <div class="ps-label">{{ step.label }}</div>
          <div class="ps-desc">{{ step.desc }}</div>
          <div v-if="i < pipeline.length - 1" class="ps-arrow">→</div>
        </div>
      </div>
    </section>

    <!-- ── CHART.JS SHOWCASE ── -->
    <section class="section">
      <h2 class="section-title">Chart.js direkt i rapporten — unikt för jsreport</h2>
      <div class="chartjs-showcase">
        <div class="cjs-left">
          <div class="feature-highlight">
            <div class="fh-icon">🎯</div>
            <div>
              <div class="fh-title">Varför detta är unikt</div>
              <p class="fh-desc">
                jsreport renderar via Chromium och väntar på <code>networkidle</code> — d.v.s.
                tills alla JavaScript-grafer har ritats klart. Resultatet är en PDF där graferna
                är identiska med din webbvy.
              </p>
            </div>
          </div>
          <div class="handlebars-demo">
            <div class="hb-header">Handlebars-mall med Chart.js</div>
            <pre class="hb-code"><code>{{ handlebarsTemplate }}</code></pre>
          </div>
        </div>
        <div class="cjs-right">
          <div class="live-charts-title">Live-exempelgrafer (identiska med PDF-output)</div>
          <div class="charts-4-grid">
            <div class="chart-card">
              <div class="chart-card-title">📊 Stapeldiagram</div>
              <canvas ref="barRef" height="160"></canvas>
            </div>
            <div class="chart-card">
              <div class="chart-card-title">🍩 Donut-kategori</div>
              <canvas ref="doughnutRef" height="160"></canvas>
            </div>
            <div class="chart-card">
              <div class="chart-card-title">📈 Trendlinje</div>
              <canvas ref="lineRef" height="160"></canvas>
            </div>
            <div class="chart-card">
              <div class="chart-card-title">🎯 Radar</div>
              <canvas ref="radarRef" height="160"></canvas>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ── HANDLEBARS LOGIC ── -->
    <section class="section">
      <h2 class="section-title">Logik direkt i HTML-mallen</h2>
      <div class="hb-features">
        <div class="hbf-card" v-for="f in hbFeatures" :key="f.title" :class="f.status">
          <div class="hbf-icon">{{ f.icon }}</div>
          <div class="hbf-title">{{ f.title }}</div>
          <pre class="hbf-code"><code>{{ f.code }}</code></pre>
          <div class="hbf-desc">{{ f.desc }}</div>
        </div>
      </div>
    </section>

    <!-- ── STRENGTHS & WEAKNESSES ── -->
    <section class="section">
      <div class="sw-grid">
        <div class="sw-panel strengths">
          <h3>🌐 Styrkor</h3>
          <div v-for="s in strengthsList" :key="s.title" class="sw-item">
            <div class="sw-title">{{ s.title }}</div>
            <div class="sw-desc">{{ s.desc }}</div>
          </div>
        </div>
        <div class="sw-panel weaknesses">
          <h3>⚡ Svagheter</h3>
          <div v-for="w in weaknessList" :key="w.title" class="sw-item">
            <div class="sw-title">{{ w.title }}</div>
            <div class="sw-desc">{{ w.desc }}</div>
          </div>
        </div>
      </div>
    </section>

    <!-- ── WORKSPACE ── -->
    <section class="section">
      <h2 class="section-title">Testa med din data</h2>
      <div class="workspace">
        <div class="workspace-left">
          <SurveyPicker />
          <ModuleList />
        </div>
        <div class="workspace-right">
          <TemplateDesigner provider-name="jsreport" :supports-html-template="true" />
          <ExportButton provider="jsreport" :supports-html-template="true" />
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import SurveyPicker from '../../components/SurveyPicker.vue'
import ModuleList from '../../components/ModuleList.vue'
import TemplateDesigner from '../../components/TemplateDesigner.vue'
import ExportButton from '../../components/ExportButton.vue'
import Chart from 'chart.js/auto'

const heroChart = ref<HTMLCanvasElement>()
const barRef = ref<HTMLCanvasElement>()
const doughnutRef = ref<HTMLCanvasElement>()
const lineRef = ref<HTMLCanvasElement>()
const radarRef = ref<HTMLCanvasElement>()

const demoTitle = 'Pulsmätning Stress & Belastning'
const demoQs = [
  { t: 'Utvilad på morgonen?', v: 3.1 },
  { t: 'Återhämtningstid?', v: 3.5 },
  { t: 'Stress kring deadlines?', v: 4.2 },
]

function hScoreColor(v: number) { return v >= 4 ? '#22c55e' : v >= 3 ? '#f59e0b' : '#ef4444' }

const pipeline = [
  { icon: '📝', label: 'HTML + Handlebars', desc: 'Din rapport-mall med {{variabler}} och {{#each}}-loopar', },
  { icon: '⚡', label: 'jsreport Engine', desc: 'Handlebars kompileras, data injiceras', },
  { icon: '🌐', label: 'Chromium', desc: 'Renderar HTML, kör Chart.js, väntar på idle', },
  { icon: '📸', label: 'PDF Screenshot', desc: 'Chrome printar sidan som perfekt PDF', },
  { icon: '📦', label: 'byte[]', desc: 'Returneras till .NET API:et och skickas till klienten', },
]

const handlebarsTemplate = `<html>
<head>
  <script src="https://cdn.chart.js"></scr` + `ipt>
</head>
<body>
  <h1>{{SurveyTitle}}</h1>

  <!-- Chart.js renderas av Chromium -->
  <canvas id="myChart"></canvas>

  {{#each QuestionSummaries}}
  <div class="question {{#if (gt AverageValue 4)}}high{{/if}}">
    <span>{{Text}}</span>
    <strong>{{AverageValue}}</strong>
  </div>
  {{/each}}

  <script>
    new Chart(document.getElementById('myChart'), {
      type: 'bar',
      data: {
        labels: [{{#each QuestionSummaries}}'Q{{@index}}'{{/unless @last}},{{/each}}],
        datasets: [{ data: [{{#each QuestionSummaries}}{{AverageValue}},{{/each}}] }]
      }
    });
  </scr` + `ipt>
</body>
</html>`

const hbFeatures = [
  {
    status: 'ok', icon: '🔁',
    title: '#each — Loopar',
    code: `{{#each QuestionSummaries}}
  <tr><td>{{Text}}</td>
      <td>{{AverageValue}}</td></tr>
{{/each}}`,
    desc: 'Iterera över alla frågor, svar, trender etc. direkt i HTML-mallen.',
  },
  {
    status: 'ok', icon: '🔀',
    title: '#if / unless',
    code: `{{#if (gt AverageValue 4)}}
  <span class="high">★ Högt betyg</span>
{{/if}}`,
    desc: 'Villkorlig rendering baserat på datavärden.',
  },
  {
    status: 'ok', icon: '🎨',
    title: 'Inline CSS-logik',
    code: `<div style="color:
  {{#if (gt AverageValue 4)}}green
  {{else}}red{{/if}}">
  {{AverageValue}}
</div>`,
    desc: 'Dynamisk CSS baserat på data — omöjligt i kod-first providers.',
  },
  {
    status: 'ok', icon: '🔢',
    title: '@index — Räknare',
    code: `{{#each QuestionSummaries}}
  <tr>
    <td>{{@index}}</td>
    <td>{{Text}}</td>
  </tr>
{{/each}}`,
    desc: 'Automatisk radnumrering med @index.',
  },
]

const strengthsList = [
  { title: 'Chart.js i PDF — unikt', desc: 'Enda providern som kan rendera interaktiva JS-grafer och "frysa" dem i en PDF via Chromium.' },
  { title: 'Web-designers kan bygga mallar', desc: 'Hela mallen är HTML/CSS — din frontend-designer kan göra ändringar utan att röra C#-kod.' },
  { title: 'Pixel-perfekt med webbvyn', desc: 'Rapporten ser exakt ut som din webbapp. Ingen diskrepans mellan "förhandsvisning" och PDF.' },
  { title: 'Handlebars logik', desc: 'Villkor, loopar och helpers direkt i mallen. Reducerar backend-kod dramatiskt.' },
  { title: 'Excel via HTML', desc: 'Genererar acceptabla Excel-filer via html-to-xlsx recipe utan extra bibliotek.' },
]

const weaknessList = [
  { title: 'Chromium-process krävs', desc: 'jsreport startar en Node.js-process som hanterar Chromium. ~200MB disk, starttid ca 2-5s.' },
  { title: 'PowerPoint saknas nativt', desc: 'jsreport har inget PPT-recipe. Vi använder ShapeCrawler som fallback, men med begränsad funktionalitet.' },
  { title: 'Slow first render', desc: 'Första anropet startar Chromium-processen. Efterföljande är snabbare men fortfarande 1-3s.' },
  { title: 'Excel-kvalitet begränsad', desc: 'HTML-to-xlsx ger enkla ark utan formler, conditionell formatering eller charts.' },
  { title: 'Deployment-komplexitet', desc: 'Chromium-binaryns beroenden (Linux libs) kan vara en utmaning i containeriserade miljöer.' },
]

onMounted(() => {
  const purple = 'rgba(168,85,247,'
  const categories = ['Ledarskap', 'Psykosocialt', 'Hälsa', 'Säkerhet', 'Resurser']
  const catData = [4.1, 3.2, 3.8, 4.5, 3.5]

  if (heroChart.value) {
    new Chart(heroChart.value, {
      type: 'bar',
      data: {
        labels: demoQs.map((_, i) => `Q${i+1}`),
        datasets: [{ data: demoQs.map(q => q.v), backgroundColor: `${purple}0.7)`, borderColor: '#a855f7', borderWidth: 2, borderRadius: 4 }]
      },
      options: { plugins: { legend: { display: false } }, scales: { y: { min: 0, max: 5, ticks: { color: '#888', font: { size: 9 } }, grid: { color: '#1e1e3f' } }, x: { ticks: { color: '#888', font: { size: 9 } }, grid: { color: '#1e1e3f' } } } }
    })
  }
  if (barRef.value) {
    new Chart(barRef.value, {
      type: 'bar',
      data: { labels: ['Q1','Q2','Q3','Q4','Q5'], datasets: [{ data: [3.2,4.1,3.8,4.3,3.5], backgroundColor: `${purple}0.7)`, borderColor: '#a855f7', borderRadius: 4 }] },
      options: { plugins: { legend: { display: false } }, scales: { y: { min: 0, max: 5, ticks: { color: '#888' }, grid: { color: '#1e1e3f' } }, x: { ticks: { color: '#888' }, grid: { color: '#1e1e3f' } } } }
    })
  }
  if (doughnutRef.value) {
    new Chart(doughnutRef.value, {
      type: 'doughnut',
      data: { labels: categories, datasets: [{ data: catData, backgroundColor: ['#a855f7','#7c3aed','#6d28d9','#4c1d95','#2e1065'] }] },
      options: { plugins: { legend: { labels: { color: '#94a3b8', font: { size: 9 } } } } }
    })
  }
  if (lineRef.value) {
    new Chart(lineRef.value, {
      type: 'line',
      data: { labels: ['Jan','Feb','Mar','Apr','Maj','Jun'], datasets: [{ data: [3.2,3.4,3.7,3.5,3.9,4.1], borderColor: '#a855f7', backgroundColor: `${purple}0.1)`, fill: true, tension: 0.4 }] },
      options: { plugins: { legend: { display: false } }, scales: { y: { min: 2, max: 5, ticks: { color: '#888' }, grid: { color: '#1e1e3f' } }, x: { ticks: { color: '#888' }, grid: { color: '#1e1e3f' } } } }
    })
  }
  if (radarRef.value) {
    new Chart(radarRef.value, {
      type: 'radar',
      data: { labels: categories, datasets: [{ data: catData, backgroundColor: `${purple}0.2)`, borderColor: '#a855f7', pointBackgroundColor: '#a855f7' }] },
      options: { plugins: { legend: { display: false } }, scales: { r: { min: 0, max: 5, ticks: { color: '#666', stepSize: 1 }, grid: { color: '#2e1065' }, pointLabels: { color: '#94a3b8', font: { size: 9 } } } } }
    })
  }
})
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;600;700;900&family=JetBrains+Mono:wght@400;600&display=swap');

.jsreport-page { font-family: 'Outfit', sans-serif; max-width: 1300px; margin: 0 auto; color: #e2e8f0; }

/* Hero */
.hero {
  position: relative; overflow: hidden; border-radius: 20px;
  background: #0d0018;
  margin-bottom: 48px;
  display: grid; grid-template-columns: 1fr 380px;
  min-height: 400px;
}
.hero-bg { position: absolute; inset: 0; pointer-events: none; }
.web-rings { position: absolute; top: 50%; left: 40%; transform: translate(-50%,-50%); }
.ring {
  position: absolute; top: 50%; left: 50%; transform: translate(-50%,-50%);
  border: 1px solid rgba(168,85,247,calc(0.15 - var(--i)*0.03));
  border-radius: 50%;
  width: calc(var(--i) * 140px);
  height: calc(var(--i) * 140px);
}
.glow-purple { position: absolute; width: 500px; height: 500px; background: radial-gradient(circle, rgba(168,85,247,0.15) 0%, transparent 70%); top: -100px; left: -100px; }

.hero-inner { position: relative; z-index: 1; padding: 48px; }
.back-link { color: rgba(255,255,255,0.4); text-decoration: none; font-size: 13px; display: block; margin-bottom: 20px; }
.back-link:hover { color: #a855f7; }
.hero-eyebrow { display: flex; align-items: center; gap: 8px; font-size: 11px; text-transform: uppercase; letter-spacing: 2px; color: #a855f7; margin-bottom: 16px; }
.track-dot { width: 8px; height: 8px; border-radius: 50%; background: #a855f7; box-shadow: 0 0 12px #a855f7; }
h1 { font-size: 52px; font-weight: 900; margin: 0 0 4px; color: #fff; letter-spacing: -2px; }
.hero-tagline { font-size: 16px; color: #a855f7; margin-bottom: 16px; font-weight: 600; font-family: 'JetBrains Mono', monospace; }
.hero-sub { font-size: 15px; color: rgba(255,255,255,0.6); margin: 0 0 24px; max-width: 480px; line-height: 1.6; }
.hero-sub strong { color: #c4b5fd; }
.hero-badges { display: flex; flex-wrap: wrap; gap: 8px; }
.badge { font-size: 11px; padding: 4px 12px; border-radius: 100px; border: 1px solid; font-weight: 500; }
.badge.purple { color: #a855f7; border-color: rgba(168,85,247,0.3); background: rgba(168,85,247,0.08); }
.badge.green { color: #22c55e; border-color: rgba(34,197,94,0.3); background: rgba(34,197,94,0.08); }
.badge.red { color: #ef4444; border-color: rgba(239,68,68,0.3); background: rgba(239,68,68,0.08); }

/* Browser mockup */
.hero-demo-panel { position: relative; z-index: 1; padding: 32px; display: flex; align-items: center; justify-content: center; }
.browser-chrome { background: #1a0533; border-radius: 10px; overflow: hidden; width: 100%; box-shadow: 0 20px 60px rgba(168,85,247,0.2); border: 1px solid rgba(168,85,247,0.2); }
.bc-bar { background: #2e1065; padding: 8px 12px; display: flex; align-items: center; gap: 10px; }
.bc-dots { display: flex; gap: 5px; }
.bc-dots span { width: 10px; height: 10px; border-radius: 50%; background: rgba(255,255,255,0.2); }
.bc-url { flex: 1; font-size: 11px; background: rgba(0,0,0,0.3); border-radius: 4px; padding: 3px 10px; color: #94a3b8; font-family: 'JetBrains Mono', monospace; }
.bc-content { padding: 16px; }
.report-preview h3 { font-family: 'Outfit', sans-serif; }
.rp-chart { margin-bottom: 12px; }
.rp-table { font-size: 10px; }
.rpt-row { display: grid; grid-template-columns: 1fr 50px; padding: 4px 0; border-bottom: 1px solid rgba(255,255,255,0.05); color: #94a3b8; }
.rpt-row.header { font-weight: 700; color: #a855f7; border-bottom-color: rgba(168,85,247,0.3); }

/* Pipeline */
.section { margin-bottom: 48px; }
.section-title { font-family: 'JetBrains Mono', monospace; font-size: 18px; color: #fff; margin: 0 0 24px; }
.pipeline { display: flex; align-items: flex-start; gap: 0; background: #0d0018; border-radius: 12px; padding: 24px; border: 1px solid rgba(168,85,247,0.15); }
.pipe-step { position: relative; flex: 1; text-align: center; }
.ps-num { position: absolute; top: -8px; left: 50%; transform: translateX(-50%); background: #a855f7; color: white; width: 18px; height: 18px; border-radius: 50%; font-size: 10px; font-weight: 700; display: flex; align-items: center; justify-content: center; }
.ps-icon { font-size: 28px; margin-bottom: 8px; padding-top: 12px; }
.ps-label { font-size: 12px; font-weight: 700; color: #e2e8f0; margin-bottom: 4px; }
.ps-desc { font-size: 10px; color: #6b7280; line-height: 1.4; padding: 0 8px; }
.ps-arrow { position: absolute; right: -2px; top: 30px; font-size: 20px; color: rgba(168,85,247,0.5); }

/* Chart.js showcase */
.chartjs-showcase { display: grid; grid-template-columns: 1fr 1fr; gap: 24px; }
.cjs-left { display: flex; flex-direction: column; gap: 16px; }
.feature-highlight { display: flex; gap: 16px; background: rgba(168,85,247,0.06); border: 1px solid rgba(168,85,247,0.2); border-radius: 12px; padding: 20px; }
.fh-icon { font-size: 32px; flex-shrink: 0; }
.fh-title { font-size: 16px; font-weight: 700; color: #e2e8f0; margin-bottom: 6px; }
.fh-desc { font-size: 12px; color: #94a3b8; line-height: 1.6; margin: 0; }
.fh-desc code { background: rgba(168,85,247,0.2); padding: 1px 5px; border-radius: 3px; color: #c4b5fd; }
.handlebars-demo { background: #120020; border-radius: 10px; overflow: hidden; }
.hb-header { background: #2e1065; padding: 10px 16px; font-size: 12px; color: #a855f7; font-weight: 600; }
.hb-code { margin: 0; padding: 16px; overflow-x: auto; }
.hb-code code { font-family: 'JetBrains Mono', monospace; font-size: 10px; color: #c4b5fd; white-space: pre; line-height: 1.7; }

.cjs-right { }
.live-charts-title { font-size: 13px; font-weight: 600; color: #a855f7; margin-bottom: 12px; }
.charts-4-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
.chart-card { background: #120020; border: 1px solid rgba(168,85,247,0.15); border-radius: 8px; padding: 14px; }
.chart-card-title { font-size: 11px; color: #94a3b8; margin-bottom: 8px; }

/* HB Features */
.hb-features { display: grid; grid-template-columns: repeat(4, 1fr); gap: 12px; }
.hbf-card { background: #0d0018; border: 1px solid rgba(168,85,247,0.15); border-radius: 10px; padding: 18px; }
.hbf-card.ok { border-color: rgba(168,85,247,0.3); }
.hbf-icon { font-size: 20px; margin-bottom: 8px; }
.hbf-title { font-size: 12px; font-weight: 700; color: #e2e8f0; margin-bottom: 10px; }
.hbf-code { margin: 0 0 10px; padding: 10px; background: #120020; border-radius: 6px; overflow-x: auto; }
.hbf-code code { font-family: 'JetBrains Mono', monospace; font-size: 9px; color: #c4b5fd; white-space: pre; line-height: 1.5; }
.hbf-desc { font-size: 11px; color: #6b7280; line-height: 1.5; }

/* S&W */
.sw-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
.sw-panel { background: #0d0018; border-radius: 12px; padding: 28px; }
.sw-panel.strengths { border-top: 3px solid #a855f7; }
.sw-panel.weaknesses { border-top: 3px solid #f59e0b; }
.sw-panel h3 { font-size: 16px; font-weight: 700; margin: 0 0 20px; }
.strengths h3 { color: #a855f7; }
.weaknesses h3 { color: #f59e0b; }
.sw-item { padding: 12px 0; border-bottom: 1px solid rgba(255,255,255,0.04); }
.sw-item:last-child { border-bottom: none; }
.sw-title { font-size: 13px; font-weight: 600; color: #e2e8f0; margin-bottom: 4px; }
.sw-desc { font-size: 12px; color: #64748b; line-height: 1.5; }

/* Workspace */
.workspace { display: grid; grid-template-columns: 340px 1fr; gap: 20px; }
.workspace-left, .workspace-right { display: flex; flex-direction: column; gap: 16px; }
</style>
