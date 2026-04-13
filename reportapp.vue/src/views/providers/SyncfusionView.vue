<template>
  <div class="sf-page">
    <!-- ── HERO ── -->
    <div class="hero">
      <div class="hero-bg">
        <div class="diamond-grid"></div>
        <div class="glow-teal"></div>
      </div>
      <div class="hero-inner">
        <router-link to="/" class="back-link">← Dashboard</router-link>
        <div class="hero-eyebrow">
          <span class="track-dot"></span>
          Spår 4 — Mogen Enterprise Platform
        </div>
        <h1>Syncfusion<br/><span class="sf-sub">Essential Studio</span></h1>
        <p class="hero-sub">
          20+ år av Excel-expertis. Native Office-dokumentmodeller.
          Redigerbara grafer, live formler, PDF/UA accessibility —
          byggt för system som kräver mer än "det fungerar".
        </p>
        <div class="hero-badges">
          <span class="badge teal">★ Native redigerbara grafer</span>
          <span class="badge teal">✓ 400+ Excel-formler</span>
          <span class="badge teal">✓ PDF/UA accessibility</span>
          <span class="badge teal">✓ Community gratis</span>
          <span class="badge yellow">⚠ Ingen HTML → PDF</span>
        </div>
      </div>
      <div class="hero-product-grid">
        <div v-for="p in products" :key="p.name" class="product-tile">
          <div class="pt-icon">{{ p.icon }}</div>
          <div class="pt-name">{{ p.name }}</div>
          <div class="pt-tagline">{{ p.tagline }}</div>
          <div class="pt-rating">
            <span v-for="i in 5" :key="i" :class="['star', i <= p.stars ? 'filled' : '']">★</span>
          </div>
        </div>
      </div>
    </div>

    <!-- ── KILLER FEATURE: FORMULA ENGINE ── -->
    <section class="section">
      <div class="feature-spotlight formula">
        <div class="fs-badge">🎯 UNIK FUNKTION</div>
        <h2>Live Excel-formler i exporterade rapporter</h2>
        <div class="fs-body">
          <div class="fs-left">
            <p>
              Syncfusion XlsIO kan skriva <strong>riktiga Excel-formler</strong> (=AVERAGE, =SUM, =IF)
              i exporterade filer. Öppnar användaren Excel och ändrar ett värde — grafen uppdateras automatiskt.
              Ingen annan provider i detta test kan detta.
            </p>
            <div class="formula-demo">
              <div class="fd-header">Interaktivt formelexempel</div>
              <div class="fd-cells">
                <div class="fd-row header">
                  <span>Fråga</span><span>Q1</span><span>Q2</span><span>Q3</span><span>=AVERAGE</span>
                </div>
                <div v-for="(q, i) in formulaDemo" :key="q.label" class="fd-row">
                  <span>{{ q.label }}</span>
                  <input v-model.number="q.q1" class="fd-input" type="number" min="1" max="5" step="0.1" />
                  <input v-model.number="q.q2" class="fd-input" type="number" min="1" max="5" step="0.1" />
                  <input v-model.number="q.q3" class="fd-input" type="number" min="1" max="5" step="0.1" />
                  <span class="fd-avg" :style="`color:${avgColor((q.q1+q.q2+q.q3)/3)}`">
                    {{ ((q.q1+q.q2+q.q3)/3).toFixed(2) }}
                  </span>
                </div>
                <div class="fd-row formula-row">
                  <span>=AVERAGE(col)</span>
                  <span>{{ colAvg(0).toFixed(2) }}</span>
                  <span>{{ colAvg(1).toFixed(2) }}</span>
                  <span>{{ colAvg(2).toFixed(2) }}</span>
                  <span class="fd-total">{{ overallAvg.toFixed(2) }}</span>
                </div>
              </div>
              <div class="fd-note">↑ Redigera värdena — formeln räknar live (precis som i exporterade Excel)</div>
            </div>
          </div>
          <div class="fs-right">
            <div class="code-block">
              <div class="cb-header"><span>SyncfusionProvider.cs — XlsIO Formler</span><span class="cb-lang">C#</span></div>
              <pre><code>{{ formulaCode }}</code></pre>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ── KILLER FEATURE: NATIVE CHARTS ── -->
    <section class="section">
      <div class="feature-spotlight charts">
        <div class="fs-badge">🎯 UNIK FUNKTION</div>
        <h2>Native redigerbara Office-grafer</h2>
        <div class="fs-body">
          <div class="fs-left">
            <p>
              Syncfusion skapar <strong>native OfficeChart-objekt</strong> — inte bilder.
              PPT-grafen kan öppnas i PowerPoint och redigeras: byt charttyp, lägg till dataserier,
              ändra färger. Exportera till Excel och få samma editerbara graf.
            </p>
            <div class="chart-type-picker">
              <button
                v-for="ct in chartTypes"
                :key="ct.id"
                class="ct-btn"
                :class="{ active: selectedChartType === ct.id }"
                @click="selectChartType(ct.id)"
              >{{ ct.icon }} {{ ct.label }}</button>
            </div>
            <div class="chart-container">
              <canvas ref="nativeChart" height="200"></canvas>
            </div>
            <div class="chart-note">↑ Klicka charttyp — i Syncfusion ändras bara <code>OfficeChartType</code>-enum</div>
          </div>
          <div class="fs-right">
            <div class="code-block">
              <div class="cb-header"><span>SyncfusionProvider.cs — PPT Chart</span><span class="cb-lang">C#</span></div>
              <pre><code>{{ nativeChartCode }}</code></pre>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ── SANDBOX ── -->
    <section class="section">
      <h2 class="section-title">Enterprise Feature Sandbox</h2>
      <p class="section-sub">Aktivera avancerade Syncfusion-funktioner för att se exakt vad som ingår i exporten.</p>
      <div class="sandbox-grid">
        <div v-for="f in sandboxFeatures" :key="f.id" class="sandbox-card" :class="{ active: sfConfig[f.id as keyof typeof sfConfig] }">
          <label class="sandbox-toggle">
            <input type="checkbox" v-model="sfConfig[f.id as keyof typeof sfConfig]" />
            <span class="toggle-track"><span class="toggle-thumb"></span></span>
          </label>
          <div class="sc-icon">{{ f.icon }}</div>
          <div class="sc-title">{{ f.title }}</div>
          <div class="sc-desc">{{ f.desc }}</div>
          <div class="sc-code">
            <code>{{ f.code }}</code>
          </div>
          <div class="sc-status" v-if="sfConfig[f.id as keyof typeof sfConfig]">
            <span class="active-badge">✓ Aktiverad i nästa export</span>
          </div>
        </div>
      </div>
    </section>

    <!-- ── STRENGTHS & WEAKNESSES ── -->
    <section class="section">
      <div class="sw-grid">
        <div class="sw-panel strengths">
          <h3>💎 Styrkor</h3>
          <div v-for="s in strengthsList" :key="s.title" class="sw-item">
            <div class="sw-title">{{ s.title }}</div>
            <div class="sw-desc">{{ s.desc }}</div>
          </div>
        </div>
        <div class="sw-panel weaknesses">
          <h3>⚠️ Svagheter</h3>
          <div v-for="w in weaknessList" :key="w.title" class="sw-item">
            <div class="sw-title">{{ w.title }}</div>
            <div class="sw-desc">{{ w.desc }}</div>
          </div>
        </div>
      </div>
    </section>

    <!-- ── WORKSPACE ── -->
    <section class="section">
      <h2 class="section-title">Testa Syncfusion-exporter</h2>
      <div class="workspace">
        <div class="workspace-left">
          <SurveyPicker />
          <ModuleList />
        </div>
        <div class="workspace-right">
          <TemplateDesigner provider-name="Syncfusion" :supports-html-template="false" />
          <ExportButton provider="Syncfusion" :supports-html-template="false" />
        </div>
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
import Chart from 'chart.js/auto'

const nativeChart = ref<HTMLCanvasElement>()
let chartInstance: Chart | null = null
const selectedChartType = ref('bar')

const products = [
  { icon: '📊', name: 'XlsIO', tagline: 'Excel engine med formler', stars: 5 },
  { icon: '📄', name: 'Essential PDF', tagline: 'AES-256 & PDF/UA', stars: 4 },
  { icon: '📋', name: 'Presentation', tagline: 'Native PPT charts', stars: 4 },
]

const formulaDemo = ref([
  { label: 'Ledarskap', q1: 4.2, q2: 3.8, q3: 4.5 },
  { label: 'Psykosocialt', q1: 3.1, q2: 3.4, q3: 2.9 },
  { label: 'Hälsa', q1: 3.8, q2: 4.0, q3: 3.6 },
])

function colAvg(col: number) {
  const vals = [formulaDemo.value[0], formulaDemo.value[1], formulaDemo.value[2]]
  const key = col === 0 ? 'q1' : col === 1 ? 'q2' : 'q3'
  return vals.reduce((s, r) => s + r[key as keyof typeof r] as unknown as number, 0) / vals.length
}
const overallAvg = computed(() => {
  const total = formulaDemo.value.reduce((s, r) => s + (r.q1 + r.q2 + r.q3), 0)
  return total / (formulaDemo.value.length * 3)
})
function avgColor(v: number) { return v >= 4 ? '#22c55e' : v >= 3 ? '#f59e0b' : '#ef4444' }

const chartTypes = [
  { id: 'bar', icon: '📊', label: 'Stapel' },
  { id: 'line', icon: '📈', label: 'Linje' },
  { id: 'radar', icon: '🎯', label: 'Radar' },
  { id: 'doughnut', icon: '🍩', label: 'Donut' },
]

const chartData = {
  labels: ['Ledarskap', 'Psykosocialt', 'Hälsa', 'Säkerhet', 'Resurser'],
  values: [4.1, 3.2, 3.8, 4.5, 3.5]
}

function selectChartType(id: string) {
  selectedChartType.value = id
  renderNativeChart(id)
}

function renderNativeChart(type: string) {
  if (!nativeChart.value) return
  if (chartInstance) chartInstance.destroy()

  const teal = 'rgba(20,184,166,'
  const baseConfig = {
    labels: chartData.labels,
    datasets: [{
      data: chartData.values,
      backgroundColor: type === 'doughnut' ? ['#14b8a6','#0d9488','#0f766e','#115e59','#134e4a'] : `${teal}0.7)`,
      borderColor: type === 'doughnut' ? 'transparent' : '#14b8a6',
      borderWidth: 2,
      borderRadius: type === 'bar' ? 4 : undefined,
      fill: type === 'line' ? true : undefined,
      tension: 0.4,
      pointBackgroundColor: '#14b8a6',
    }]
  }
  const opts: Record<string, unknown> = {
    plugins: { legend: { display: type === 'doughnut', labels: { color: '#94a3b8', font: { size: 10 } } } },
    animation: { duration: 400 },
  }
  if (type !== 'doughnut' && type !== 'radar') {
    (opts as Record<string, unknown>).scales = {
      y: { min: 0, max: 5, ticks: { color: '#94a3b8', font: { size: 9 } }, grid: { color: '#1e293b' } },
      x: { ticks: { color: '#94a3b8', font: { size: 9 } }, grid: { color: '#1e293b' } },
    }
  }
  if (type === 'radar') {
    (opts as Record<string, unknown>).scales = {
      r: { min: 0, max: 5, ticks: { color: '#666', stepSize: 1 }, grid: { color: '#1e293b' }, pointLabels: { color: '#94a3b8', font: { size: 9 } } }
    }
  }

  chartInstance = new Chart(nativeChart.value, {
    type: type as 'bar' | 'line' | 'radar' | 'doughnut',
    data: baseConfig as unknown as import('chart.js').ChartData,
    options: opts as unknown as import('chart.js').ChartOptions,
  })
}

const sfConfig = ref({ useFormulas: true, encrypt: false, nativeCharts: true, accessibility: false, watermark: false })

const sandboxFeatures = [
  { id: 'useFormulas', icon: '🧮', title: 'Live Excel Formulas', desc: '=AVERAGE(J2:J10) istället för statiska siffror. Ändra data i Excel och kalkylen uppdateras.', code: 'sheet.Range[r, 3].Formula = $"=J{r}";' },
  { id: 'encrypt', icon: '🔐', title: 'PDF Kryptering (AES-256)', desc: 'Lösenordsskydda rapporten direkt vid generering. Lösenord: 1234', code: 'document.Security.KeySize = PdfEncryptionKeySize.Key256Bit;' },
  { id: 'nativeCharts', icon: '📊', title: 'Native Redigerbara Grafer', desc: 'PPT-grafer som OfficeChart-objekt, redigerbara i PowerPoint (inte bilder).', code: 'chart.ChartType = OfficeChartType.Column_Clustered;' },
  { id: 'accessibility', icon: '♿', title: 'PDF/UA Accessibility', desc: 'Taggad PDF som fungerar med skärmläsare. WCAG 2.1 AA-kompatibel.', code: 'document.ViewerPreferences.DisplayDocTitle = true;' },
  { id: 'watermark', icon: '💧', title: 'Konfidentialitetsstämpel', desc: 'Diagonalt vattenmärke "KONFIDENTIELLT" på alla sidor.', code: 'page.Graphics.DrawString("KONFIDENTIELLT", font, brush);' },
]

const formulaCode = `// XlsIO: Skriv värde i hjälp-kolumn J
sheet.Range[r, 10].Number = q.AverageValue;

// Skriv FORMEL i kolumn C (refererar till J)
sheet.Range[r, 3].Formula = $"=J{r}";

// Conditionell formatering (röd/gul/grön)
var cond = sheet.Range[r, 3].ConditionalFormats.AddCondition();
cond.FormatType = ExcelCFType.CellValue;
cond.Operator   = ExcelComparisonOperator.LessOrEqual;
cond.FirstFormula = "3";
cond.BackColorRGB = Color.FromArgb(255, 239, 68, 68); // rött`

const nativeChartCode = `// Syncfusion Presentation: Native chart (INTE bild!)
IPresentationChart chart =
    slide.Shapes.AddChart(100, 100, 600, 400);

chart.ChartType = OfficeChartType.Column_Clustered;

// Data kopplas till Office data-range
for (int i = 0; i < questions.Count; i++) {
    chart.ChartData.SetValue(i + 2, 1, "Q" + (i + 1));
    chart.ChartData.SetValue(i + 2, 2,
        questions[i].AverageValue);
}

// Användaren kan REDIGERA grafen i PowerPoint!
var series = chart.Series.Add("Genomsnitt");
series.Values = chart.ChartData[2, 2,
    questions.Count + 1, 2];`

const strengthsList = [
  { title: 'Native redigerbara grafer', desc: 'Excel/PPT-grafer är OfficeChart-objekt, inte bilder. Mottagaren kan redigera dem i Office.' },
  { title: '400+ Excel-formler', desc: 'XlsIO stödjer AVERAGE, SUM, IF, VLOOKUP m.fl. — rapporten "lever" och räknar om sig.' },
  { title: 'Community-licens gratis', desc: 'Gratis upp till $1M omsättning — en stor fördel mot IronSuite som alltid kostar.' },
  { title: 'PDF/UA Accessibility', desc: 'Skapar automatiskt taggade PDF:er kompatibla med WCAG 2.1 AA och skärmläsare.' },
  { title: 'Mogen & dokumenterad', desc: '20+ år på marknaden. Enormt antal examples, dokumentation och community-resurser.' },
  { title: 'Hög prestanda', desc: 'Ingen Chromium-process. Rena .NET-bibliotek med extremt snabb rendering av stora datamängder.' },
]

const weaknessList = [
  { title: 'Ingen HTML-mall support', desc: 'Syncfusion bygger dokument programmatiskt via objektmodell. HTML-mallar från frontend-designern stöds inte.' },
  { title: 'Betallicens för kommersiellt bruk >$1M', desc: 'Community-licensen är generös men stora bolag betalar. Priset är dock lägre än IronSuite.' },
  { title: 'API-komplexitet', desc: 'XlsIO:s objektmodell kan vara komplex. Tre olika API:er (PDF, XlsIO, Presentation) att lära sig.' },
  { title: 'Vendor lock-in', desc: 'Tightly coupled till Syncfusion-objektmodeller — migration är en stor insats.' },
  { title: 'Inga HTML-to-PDF capabilities', desc: 'Kan inte rendera Chart.js-grafer eller komplex CSS-layout i PDF som IronPDF/jsreport kan.' },
]

onMounted(() => {
  setTimeout(() => renderNativeChart('bar'), 100)
})
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&family=Source+Code+Pro:wght@400;500&display=swap');

.sf-page { font-family: 'Plus Jakarta Sans', sans-serif; max-width: 1300px; margin: 0 auto; color: #e2e8f0; }

/* Hero */
.hero {
  position: relative; overflow: hidden; border-radius: 20px;
  background: #021314;
  margin-bottom: 48px;
  display: grid; grid-template-columns: 1fr 340px;
  min-height: 420px;
}
.hero-bg { position: absolute; inset: 0; pointer-events: none; }
.diamond-grid {
  position: absolute; inset: 0;
  background-image: linear-gradient(rgba(20,184,166,0.05) 1px, transparent 1px),
    linear-gradient(90deg, rgba(20,184,166,0.05) 1px, transparent 1px);
  background-size: 48px 48px;
}
.glow-teal { position: absolute; width: 500px; height: 500px; background: radial-gradient(circle, rgba(20,184,166,0.2) 0%, transparent 70%); top: -120px; right: 0; }

.hero-inner { position: relative; z-index: 1; padding: 48px; }
.back-link { color: rgba(255,255,255,0.4); text-decoration: none; font-size: 13px; display: block; margin-bottom: 20px; }
.back-link:hover { color: #14b8a6; }
.hero-eyebrow { display: flex; align-items: center; gap: 8px; font-size: 11px; text-transform: uppercase; letter-spacing: 2px; color: #14b8a6; margin-bottom: 16px; }
.track-dot { width: 8px; height: 8px; border-radius: 50%; background: #14b8a6; box-shadow: 0 0 12px #14b8a6; }
h1 { font-size: 48px; font-weight: 800; margin: 0 0 16px; color: #fff; letter-spacing: -2px; line-height: 1.1; }
.sf-sub { color: #14b8a6; font-size: 28px; }
.hero-sub { font-size: 15px; color: rgba(255,255,255,0.6); margin: 0 0 24px; max-width: 500px; line-height: 1.6; }
.hero-badges { display: flex; flex-wrap: wrap; gap: 8px; }
.badge { font-size: 11px; padding: 4px 12px; border-radius: 100px; border: 1px solid; font-weight: 500; }
.badge.teal { color: #14b8a6; border-color: rgba(20,184,166,0.3); background: rgba(20,184,166,0.08); }
.badge.yellow { color: #f59e0b; border-color: rgba(245,158,11,0.3); background: rgba(245,158,11,0.08); }

/* Product grid */
.hero-product-grid { position: relative; z-index: 1; padding: 40px 28px; display: flex; flex-direction: column; gap: 12px; background: rgba(255,255,255,0.02); border-left: 1px solid rgba(20,184,166,0.15); }
.product-tile { background: rgba(20,184,166,0.05); border: 1px solid rgba(20,184,166,0.15); border-radius: 10px; padding: 16px; }
.pt-icon { font-size: 22px; margin-bottom: 4px; }
.pt-name { font-size: 15px; font-weight: 700; color: #fff; }
.pt-tagline { font-size: 11px; color: #6b7280; margin-bottom: 8px; }
.star { font-size: 12px; color: #374151; }
.star.filled { color: #14b8a6; }

/* Sections */
.section { margin-bottom: 48px; }
.section-title { font-family: 'Source Code Pro', monospace; font-size: 18px; color: #fff; margin: 0 0 8px; }
.section-sub { font-size: 13px; color: #6b7280; margin: 0 0 24px; }

/* Feature spotlight */
.feature-spotlight { background: #021314; border: 1px solid rgba(20,184,166,0.2); border-radius: 16px; padding: 32px; margin-bottom: 0; }
.feature-spotlight.charts { border-color: rgba(20,184,166,0.3); }
.fs-badge { font-size: 10px; font-weight: 700; text-transform: uppercase; letter-spacing: 2px; color: #14b8a6; margin-bottom: 8px; }
.feature-spotlight h2 { font-size: 22px; font-weight: 800; color: #fff; margin: 0 0 24px; }
.fs-body { display: grid; grid-template-columns: 1fr 1fr; gap: 24px; }
.fs-left p { font-size: 13px; color: #94a3b8; line-height: 1.7; margin-bottom: 16px; }
.fs-left p strong { color: #2dd4bf; }

/* Formula demo */
.formula-demo { background: #0a2228; border-radius: 10px; overflow: hidden; border: 1px solid rgba(20,184,166,0.2); }
.fd-header { background: #217346; padding: 8px 14px; font-size: 12px; font-weight: 600; color: white; }
.fd-cells { }
.fd-row { display: grid; grid-template-columns: 120px 1fr 1fr 1fr 1fr; border-bottom: 1px solid rgba(20,184,166,0.1); }
.fd-row.header { background: rgba(20,184,166,0.1); }
.fd-row.header span { padding: 6px 10px; font-size: 10px; font-weight: 700; color: #14b8a6; }
.fd-row.formula-row { background: rgba(20,184,166,0.05); }
.fd-row.formula-row span { padding: 6px 10px; font-size: 11px; color: #14b8a6; font-family: 'Source Code Pro', monospace; }
.fd-row span { padding: 6px 10px; font-size: 11px; color: #94a3b8; display: flex; align-items: center; }
.fd-input { background: #0d3040; border: 1px solid rgba(20,184,166,0.3); color: #e2e8f0; padding: 4px 6px; border-radius: 4px; font-size: 11px; width: 100%; font-family: 'Source Code Pro', monospace; margin: 4px 6px; }
.fd-avg { font-weight: 700; font-size: 13px !important; }
.fd-total { font-weight: 800; font-size: 14px !important; color: #14b8a6 !important; }
.fd-note { padding: 8px 14px; font-size: 10px; color: #6b7280; background: rgba(0,0,0,0.2); }

/* Chart type picker */
.chart-type-picker { display: flex; gap: 8px; margin-bottom: 12px; flex-wrap: wrap; }
.ct-btn { font-size: 12px; padding: 6px 14px; border-radius: 6px; border: 1px solid rgba(20,184,166,0.3); background: transparent; color: #94a3b8; cursor: pointer; transition: all 0.15s; font-family: 'Plus Jakarta Sans', sans-serif; }
.ct-btn.active { background: #14b8a6; color: #021314; border-color: #14b8a6; font-weight: 700; }
.chart-container { background: #0a2228; border-radius: 8px; padding: 12px; }
.chart-note { font-size: 10px; color: #6b7280; margin-top: 8px; }
.chart-note code { background: rgba(20,184,166,0.1); color: #14b8a6; padding: 1px 4px; border-radius: 3px; }

/* Code block */
.code-block { background: #061619; border-radius: 10px; overflow: hidden; }
.cb-header { display: flex; justify-content: space-between; padding: 10px 16px; background: #0a2228; font-size: 12px; color: #aaa; border-bottom: 1px solid rgba(20,184,166,0.15); }
.cb-lang { color: #14b8a6; font-weight: 700; }
.code-block pre { margin: 0; padding: 16px; overflow-x: auto; }
.code-block code { font-family: 'Source Code Pro', monospace; font-size: 11px; line-height: 1.7; color: #d4d4d4; white-space: pre; }

/* Sandbox */
.sandbox-grid { display: grid; grid-template-columns: repeat(5, 1fr); gap: 12px; }
.sandbox-card {
  background: #021314; border: 1px solid rgba(20,184,166,0.1); border-radius: 12px; padding: 18px;
  transition: all 0.2s; position: relative;
}
.sandbox-card.active { border-color: rgba(20,184,166,0.4); background: rgba(20,184,166,0.04); }
.sandbox-toggle { display: flex; align-items: center; margin-bottom: 10px; cursor: pointer; }
.sandbox-toggle input { display: none; }
.toggle-track { width: 36px; height: 20px; background: #1e293b; border-radius: 100px; position: relative; transition: background 0.2s; }
.sandbox-toggle input:checked ~ .toggle-track { background: #14b8a6; }
.toggle-thumb { position: absolute; left: 3px; top: 3px; width: 14px; height: 14px; border-radius: 50%; background: white; transition: transform 0.2s; box-shadow: 0 1px 3px rgba(0,0,0,0.3); }
.sandbox-toggle input:checked ~ .toggle-track .toggle-thumb { transform: translateX(16px); }
.sc-icon { font-size: 22px; margin-bottom: 8px; }
.sc-title { font-size: 13px; font-weight: 700; color: #e2e8f0; margin-bottom: 6px; }
.sc-desc { font-size: 11px; color: #6b7280; line-height: 1.5; margin-bottom: 10px; }
.sc-code { background: rgba(0,0,0,0.3); border-radius: 4px; padding: 6px 8px; }
.sc-code code { font-family: 'Source Code Pro', monospace; font-size: 9px; color: #14b8a6; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; display: block; }
.sc-status { margin-top: 8px; }
.active-badge { font-size: 10px; color: #14b8a6; font-weight: 600; }

/* S&W */
.sw-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
.sw-panel { background: #021314; border-radius: 12px; padding: 28px; }
.sw-panel.strengths { border-top: 3px solid #14b8a6; }
.sw-panel.weaknesses { border-top: 3px solid #f59e0b; }
.sw-panel h3 { font-size: 16px; font-weight: 700; margin: 0 0 20px; }
.strengths h3 { color: #14b8a6; }
.weaknesses h3 { color: #f59e0b; }
.sw-item { padding: 12px 0; border-bottom: 1px solid rgba(255,255,255,0.04); }
.sw-item:last-child { border-bottom: none; }
.sw-title { font-size: 13px; font-weight: 600; color: #e2e8f0; margin-bottom: 4px; }
.sw-desc { font-size: 12px; color: #64748b; line-height: 1.5; }

/* Workspace */
.workspace { display: grid; grid-template-columns: 340px 1fr; gap: 20px; }
.workspace-left, .workspace-right { display: flex; flex-direction: column; gap: 16px; }
</style>
