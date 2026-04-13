<template>
  <div class="iron-page">
    <!-- ── HERO ── -->
    <div class="hero">
      <div class="hero-bg">
        <div class="gear-pattern"></div>
        <div class="glow-blue"></div>
        <div class="glow-blue-2"></div>
      </div>
      <div class="hero-inner">
        <router-link to="/" class="back-link">← Dashboard</router-link>
        <div class="hero-eyebrow">
          <span class="track-dot"></span>
          Spår 1 — Enterprise Suite
        </div>
        <h1>IronSuite</h1>
        <div class="hero-sub-stack">
          <span class="lib-pill">IronPDF</span>
          <span class="separator">+</span>
          <span class="lib-pill">IronXL</span>
          <span class="separator">+</span>
          <span class="lib-pill">IronPPT</span>
        </div>
        <p class="hero-sub">
          En leverantör. Tre kraftfulla bibliotek. HTML-till-PDF via Chromium,
          redigerbara Excel-grafer, och PowerPoint-export — allt med enterprise-grade
          support och SLA.
        </p>
        <div class="hero-badges">
          <span class="badge blue">✓ HTML → PDF (Chromium)</span>
          <span class="badge blue">✓ Redigerbara Excel-grafer</span>
          <span class="badge blue">✓ SLA & support</span>
          <span class="badge yellow">⚠ Betallicens krävs</span>
          <span class="badge yellow">⚠ Stub-läge i denna PoC</span>
        </div>
      </div>
      <div class="hero-price-panel">
        <div class="price-tag">
          <div class="pt-label">Licenskostnad</div>
          <div class="pt-price">$$$</div>
          <div class="pt-sub">per developer/år</div>
        </div>
        <div class="price-features">
          <div class="pf-row" v-for="f in priceFeatures" :key="f">
            <span class="pf-check">✓</span>
            <span>{{ f }}</span>
          </div>
        </div>
        <div class="stub-warning">
          ⚠ Providers är stub-implementerade i denna PoC.
          Aktivera med riktig licens i Program.cs.
        </div>
      </div>
    </div>

    <!-- ── HTML TO PDF POWER ── -->
    <section class="section">
      <h2 class="section-title">IronPDF — HTML → PDF med full Chromium-kraft</h2>
      <div class="html-pdf-demo">
        <div class="hpd-left">
          <div class="approach-selector">
            <button
              v-for="a in approaches"
              :key="a.id"
              class="approach-btn"
              :class="{ active: selectedApproach === a.id }"
              @click="selectedApproach = a.id"
            >{{ a.label }}</button>
          </div>
          <div class="code-block">
            <div class="cb-header">
              <span>{{ currentApproach.file }}</span>
              <span class="cb-lang">C#</span>
            </div>
            <pre><code>{{ currentApproach.code }}</code></pre>
          </div>
          <div class="approach-note">{{ currentApproach.note }}</div>
        </div>
        <div class="hpd-right">
          <!-- Live HTML preview -->
          <div class="html-preview-wrapper">
            <div class="hp-header">
              <span>Interaktiv HTML-mallsförhandsvisning</span>
              <div class="hp-controls">
                <button @click="demoTheme = 'light'" :class="{active: demoTheme==='light'}">☀ Ljus</button>
                <button @click="demoTheme = 'dark'" :class="{active: demoTheme==='dark'}">🌙 Mörk</button>
                <button @click="demoTheme = 'corp'" :class="{active: demoTheme==='corp'}">🏢 Corp</button>
              </div>
            </div>
            <div class="html-preview" :class="`theme-${demoTheme}`">
              <div class="hp-doc">
                <div class="hpd-header">
                  <div class="hpd-logo">{{ demoTheme === 'corp' ? 'ACME Corp' : 'WorkSurvey' }}</div>
                  <div class="hpd-title">Arbetsmiljörapport Q1 2026</div>
                  <div class="hpd-meta">Storkommunen AB · 2026-01-15</div>
                </div>
                <div class="hpd-chart-area">
                  <canvas ref="ironChart" height="120"></canvas>
                </div>
                <table class="hpd-table">
                  <thead>
                    <tr><th>Fråga</th><th>Kategori</th><th>Snitt</th></tr>
                  </thead>
                  <tbody>
                    <tr v-for="q in previewQuestions" :key="q.text">
                      <td>{{ q.text }}</td>
                      <td>{{ q.cat }}</td>
                      <td :class="['score', scoreClass(q.val)]">{{ q.val.toFixed(2) }}</td>
                    </tr>
                  </tbody>
                </table>
                <div class="hpd-footer">Genererad av IronPDF via IronSuite</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ── IRONXL FEATURES ── -->
    <section class="section">
      <h2 class="section-title">IronXL — Excel med redigerbara grafer</h2>
      <div class="ironxl-demo">
        <div class="feature-cards">
          <div v-for="f in xlFeatures" :key="f.title" class="xl-card" :class="f.status">
            <div class="xl-icon">{{ f.icon }}</div>
            <div class="xl-title">{{ f.title }}</div>
            <div class="xl-desc">{{ f.desc }}</div>
            <div class="xl-badge">{{ f.badge }}</div>
          </div>
        </div>
        <div class="excel-mockup">
          <div class="em-header">
            <span class="em-title">📗 Excel-förhandsvisning (simulerad)</span>
          </div>
          <div class="em-tabs">
            <span class="em-tab active">Enkätanalys</span>
            <span class="em-tab">Trenddata</span>
            <span class="em-tab">Graf1 ▶</span>
          </div>
          <div class="em-content">
            <div class="em-row header">
              <span class="em-cell header">A</span>
              <span class="em-cell header">B</span>
              <span class="em-cell header">C</span>
              <span class="em-cell header">D</span>
            </div>
            <div class="em-row row-1">
              <span class="em-cell bold blue-bg">Fråga</span>
              <span class="em-cell bold blue-bg">Kategori</span>
              <span class="em-cell bold blue-bg">Snitt</span>
              <span class="em-cell bold blue-bg">Svar</span>
            </div>
            <div v-for="(q, i) in previewQuestions" :key="q.text" class="em-row" :class="{alt: i%2}">
              <span class="em-cell text-sm">{{ q.text }}</span>
              <span class="em-cell">{{ q.cat }}</span>
              <span class="em-cell formula" :class="scoreClass(q.val)">{{ q.val.toFixed(2) }}</span>
              <span class="em-cell muted">{{ 20 + i*4 }}</span>
            </div>
            <div class="em-chart-area">
              <canvas ref="xlChart" height="100"></canvas>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ── STRENGTHS & WEAKNESSES ── -->
    <section class="section">
      <div class="sw-grid">
        <div class="sw-panel strengths">
          <h3>⚙️ Styrkor</h3>
          <div v-for="s in strengthsList" :key="s.title" class="sw-item">
            <div class="sw-title">{{ s.title }}</div>
            <div class="sw-desc">{{ s.desc }}</div>
          </div>
        </div>
        <div class="sw-panel weaknesses">
          <h3>💳 Svagheter</h3>
          <div v-for="w in weaknessList" :key="w.title" class="sw-item">
            <div class="sw-title">{{ w.title }}</div>
            <div class="sw-desc">{{ w.desc }}</div>
          </div>
        </div>
      </div>
    </section>

    <!-- ── WORKSPACE ── -->
    <section class="section">
      <h2 class="section-title">Testa med riktig licens</h2>
      <div class="workspace">
        <div class="workspace-left">
          <SurveyPicker />
          <ModuleList />
        </div>
        <div class="workspace-right">
          <TemplateDesigner provider-name="IronSuite" :supports-html-template="true" />
          <ExportButton provider="IronSuite" :supports-html-template="true" />
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import SurveyPicker from '../../components/SurveyPicker.vue'
import ModuleList from '../../components/ModuleList.vue'
import TemplateDesigner from '../../components/TemplateDesigner.vue'
import ExportButton from '../../components/ExportButton.vue'
import Chart from 'chart.js/auto'

const ironChart = ref<HTMLCanvasElement>()
const xlChart = ref<HTMLCanvasElement>()
const selectedApproach = ref('html')
const demoTheme = ref('light')
let ironChartInstance: Chart | null = null

const priceFeatures = ['SLA-garanterad support','Enterprise-grade kryptering','HTML-till-PDF (Chromium)','Redigerbara Excel-grafer','PowerPoint-export','Vattenstämpel & signaturer']

const previewQuestions = [
  { text: 'Arbetsbelastning?', cat: 'Psykosocialt', val: 3.2 },
  { text: 'Stöd från chef?', cat: 'Ledarskap', val: 4.1 },
  { text: 'Fysisk miljö OK?', cat: 'Fysisk', val: 3.8 },
  { text: 'Rätt verktyg?', cat: 'Resurser', val: 4.3 },
]

function scoreClass(v: number) { return v >= 4 ? 'high' : v >= 3 ? 'mid' : 'low' }

const approaches = [
  { id: 'html', label: 'HTML → PDF' },
  { id: 'encrypt', label: 'Kryptering' },
  { id: 'watermark', label: 'Vattenstämpel' },
]

const approachContent: Record<string, { file: string; code: string; note: string }> = {
  html: {
    file: 'IronSuiteProvider.cs — HTML-to-PDF',
    code: `// IronPDF: HTML-sträng → PDF
var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PrintBackground = true;
renderer.RenderingOptions.EnableJavaScript = true;
renderer.RenderingOptions.WaitFor.NetworkIdle();

// Rendera din HTML-mall (kan innehålla Chart.js!)
var pdf = await renderer.RenderHtmlAsPdfAsync(htmlTemplate);

// Metadata
pdf.MetaData.Title = data.SurveyTitle;
pdf.MetaData.Author = "ReportApp";

return pdf.BinaryData;`,
    note: '★ Styrka: Full CSS3, Flexbox, CSS Grid — din mall renderas identiskt med webbläsaren.',
  },
  encrypt: {
    file: 'IronSuiteProvider.cs — AES-256 PDF',
    code: `// Kryptera PDF med AES-256
var pdf = await renderer.RenderHtmlAsPdfAsync(html);

pdf.SecuritySettings.OwnerPassword = "admin-secret";
pdf.SecuritySettings.UserPassword   = "viewer-pass";
pdf.SecuritySettings.AllowUserPrinting    = false;
pdf.SecuritySettings.AllowUserAnnotations = false;
pdf.SecuritySettings.EncryptionLevel =
    PdfEncryptionLevel.AES_256_Bit;

return pdf.BinaryData;`,
    note: '★ Enterprise-funktion: PDF:er kan lösenordsskyddas och rättighetsbegränsas direkt i koden.',
  },
  watermark: {
    file: 'IronSuiteProvider.cs — Vattenstämpel',
    code: `var pdf = await renderer.RenderHtmlAsPdfAsync(html);

// Lägg till vattenstämpel på alla sidor
var watermark = new TextWatermark {
    Text = "KONFIDENTIELLT",
    FontSize = 48,
    Rotation = -45,
    Opacity = 0.2f,
    FontColor = Color.Red
};

foreach (var page in pdf.Pages) {
    page.AddWatermark(watermark);
}

return pdf.BinaryData;`,
    note: '★ Vattenstämplar, sidhuvuden, fotnoter — allt via enkla API-anrop.',
  },
}

const currentApproach = computed(() => approachContent[selectedApproach.value])

const xlFeatures = [
  { icon: '📊', title: 'Redigerbara grafer', desc: 'Excel-grafer är native OfficeChart-objekt — inte bilder. Öppna i Excel och redigera direkt.', badge: '★ Unikt vs OSS', status: 'ok' },
  { icon: '🧮', title: 'Formler & beräkningar', desc: 'IronXL stödjer 100+ Excel-formler. Cellen kan räkna om sig när användaren ändrar data.', badge: '✓ Full support', status: 'ok' },
  { icon: '🎨', title: 'Conditionell formatering', desc: 'Automatisk färgkodning baserat på värden — t.ex. röda celler om snitt < 3.', badge: '✓ Enterprise', status: 'ok' },
  { icon: '🔐', title: 'Kryptering', desc: 'Lösenordsskydda Excel-filen och begränsa redigeringsrättigheter.', badge: '✓ AES-256', status: 'ok' },
  { icon: '💰', title: 'Licenskostnad', desc: 'Kräver betald licens. Inga gratis community-alternativ för produktion.', badge: '⚠ Betalning', status: 'warn' },
]

const strengthsList = [
  { title: 'Enhetlig leverantör', desc: 'Ett bolag, ett supportavtal, en uppgradering. Inga kompatibilitetsproblem mellan PDF/Excel/PPT.' },
  { title: 'HTML-mallar stöds fullt ut', desc: 'Designern kan bygga rapporten i HTML/CSS och IronPDF renderar den pixelperfekt.' },
  { title: 'Redigerbara grafer i Excel', desc: 'Enda providern (förutom Syncfusion) som genererar native Excel-grafer som går att redigera.' },
  { title: 'Enterprise-grade säkerhet', desc: 'AES-256 kryptering, lösenordsskydd, digitala signaturer och vattenstämplar out-of-the-box.' },
  { title: 'SLA-support', desc: 'Kommersiell support med SLA — kritiskt för produktionssystem i stora organisationer.' },
]

const weaknessList = [
  { title: 'Betallicens — hög kostnad', desc: 'Iron Software-licenser kan kosta hundratals USD per developer per år. Kan vara avgörande för budgeten.' },
  { title: 'Chromium-beroende för PDF', desc: 'IronPDF startar Chromium-process — samma utmaning som jsreport: RAM-användning, starttid.' },
  { title: 'Stub-läge i denna PoC', desc: 'Utan licens genereras ett NotImplementedException-fel. Testkör kan ej demonstreras fullt ut.' },
  { title: 'Vendor lock-in', desc: 'Att byta från IronSuite kräver att man skriver om alla tre export-implementationer.' },
  { title: 'API-ändringar vid uppgradering', desc: 'Breaking changes förekommer mellan major-versioner och kräver testning vid uppgradering.' },
]

function initCharts() {
  const blue = 'rgba(59,130,246,'
  const qs = previewQuestions

  if (ironChart.value) {
    if (ironChartInstance) ironChartInstance.destroy()
    ironChartInstance = new Chart(ironChart.value, {
      type: 'bar',
      data: {
        labels: qs.map((_, i) => `Q${i+1}`),
        datasets: [{ data: qs.map(q => q.val), backgroundColor: `${blue}0.7)`, borderColor: '#3b82f6', borderRadius: 4 }]
      },
      options: {
        plugins: { legend: { display: false } },
        scales: {
          y: { min: 0, max: 5, ticks: { color: demoTheme.value === 'light' ? '#334155' : '#94a3b8', font: { size: 9 } }, grid: { color: demoTheme.value === 'light' ? '#f1f5f9' : '#1e293b' } },
          x: { ticks: { color: demoTheme.value === 'light' ? '#334155' : '#94a3b8', font: { size: 9 } }, grid: { display: false } },
        }
      }
    })
  }

  if (xlChart.value) {
    new Chart(xlChart.value, {
      type: 'line',
      data: {
        labels: ['Jan','Feb','Mar','Apr'],
        datasets: [{ data: qs.map(q => q.val), borderColor: '#3b82f6', backgroundColor: `${blue}0.1)`, fill: true, tension: 0.4, borderWidth: 2 }]
      },
      options: {
        plugins: { legend: { display: false } },
        scales: { y: { min: 0, max: 5, ticks: { color: '#334155', font: { size: 8 } }, grid: { color: '#f1f5f9' } }, x: { ticks: { color: '#334155', font: { size: 8 } }, grid: { display: false } } }
      }
    })
  }
}

watch(demoTheme, () => { setTimeout(initCharts, 50) })
onMounted(() => { setTimeout(initCharts, 100) })
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Sora:wght@300;400;600;700;800&family=Fira+Code:wght@400;500&display=swap');

.iron-page { font-family: 'Sora', sans-serif; max-width: 1300px; margin: 0 auto; color: #e2e8f0; }

/* Hero */
.hero {
  position: relative; overflow: hidden; border-radius: 20px;
  background: linear-gradient(135deg, #0a0f2e 0%, #0f1a3d 100%);
  margin-bottom: 48px;
  display: grid; grid-template-columns: 1fr 320px;
  min-height: 400px;
}
.hero-bg { position: absolute; inset: 0; pointer-events: none; }
.gear-pattern {
  position: absolute; inset: 0;
  background-image: radial-gradient(rgba(59,130,246,0.08) 1px, transparent 1px);
  background-size: 24px 24px;
}
.glow-blue { position: absolute; width: 400px; height: 400px; background: radial-gradient(circle, rgba(59,130,246,0.2) 0%, transparent 70%); top: -100px; right: -50px; }
.glow-blue-2 { position: absolute; width: 300px; height: 300px; background: radial-gradient(circle, rgba(99,102,241,0.15) 0%, transparent 70%); bottom: -80px; left: 100px; }

.hero-inner { position: relative; z-index: 1; padding: 48px; }
.back-link { color: rgba(255,255,255,0.4); text-decoration: none; font-size: 13px; display: block; margin-bottom: 20px; }
.back-link:hover { color: #3b82f6; }
.hero-eyebrow { display: flex; align-items: center; gap: 8px; font-size: 11px; text-transform: uppercase; letter-spacing: 2px; color: #3b82f6; margin-bottom: 16px; }
.track-dot { width: 8px; height: 8px; border-radius: 50%; background: #3b82f6; box-shadow: 0 0 12px #3b82f6; }
h1 { font-size: 56px; font-weight: 800; margin: 0 0 12px; color: #fff; letter-spacing: -2px; }
.hero-sub-stack { display: flex; align-items: center; gap: 10px; margin-bottom: 16px; }
.lib-pill { background: rgba(59,130,246,0.15); border: 1px solid rgba(59,130,246,0.3); color: #93c5fd; padding: 4px 14px; border-radius: 100px; font-size: 13px; font-weight: 600; font-family: 'Fira Code', monospace; }
.separator { color: #3b82f6; font-weight: 700; font-size: 18px; }
.hero-sub { font-size: 15px; color: rgba(255,255,255,0.6); margin: 0 0 24px; max-width: 500px; line-height: 1.6; }
.hero-badges { display: flex; flex-wrap: wrap; gap: 8px; }
.badge { font-size: 11px; padding: 4px 12px; border-radius: 100px; border: 1px solid; font-weight: 500; }
.badge.blue { color: #3b82f6; border-color: rgba(59,130,246,0.3); background: rgba(59,130,246,0.08); }
.badge.yellow { color: #f59e0b; border-color: rgba(245,158,11,0.3); background: rgba(245,158,11,0.08); }

/* Price panel */
.hero-price-panel { position: relative; z-index: 1; background: rgba(255,255,255,0.03); border-left: 1px solid rgba(59,130,246,0.15); padding: 40px 28px; display: flex; flex-direction: column; gap: 20px; }
.price-tag { text-align: center; }
.pt-label { font-size: 11px; text-transform: uppercase; letter-spacing: 1px; color: #6b7280; margin-bottom: 4px; }
.pt-price { font-size: 40px; font-weight: 800; color: #3b82f6; font-family: 'Fira Code', monospace; }
.pt-sub { font-size: 11px; color: #6b7280; }
.price-features { display: flex; flex-direction: column; gap: 6px; }
.pf-row { display: flex; gap: 8px; font-size: 12px; color: #94a3b8; }
.pf-check { color: #22c55e; flex-shrink: 0; }
.stub-warning { font-size: 10px; color: #f59e0b; background: rgba(245,158,11,0.08); border: 1px solid rgba(245,158,11,0.2); border-radius: 6px; padding: 10px; line-height: 1.5; }

/* Sections */
.section { margin-bottom: 48px; }
.section-title { font-family: 'Fira Code', monospace; font-size: 18px; color: #fff; margin: 0 0 24px; }

/* HTML-PDF Demo */
.html-pdf-demo { display: grid; grid-template-columns: 1fr 1fr; gap: 24px; }
.approach-selector { display: flex; gap: 8px; margin-bottom: 12px; }
.approach-btn { font-size: 12px; padding: 6px 16px; border-radius: 6px; border: 1px solid rgba(59,130,246,0.3); background: transparent; color: #94a3b8; cursor: pointer; transition: all 0.15s; font-family: 'Sora', sans-serif; }
.approach-btn.active { background: #3b82f6; color: white; border-color: #3b82f6; }
.approach-btn:hover:not(.active) { border-color: #3b82f6; color: #3b82f6; }
.code-block { background: #0d1117; border-radius: 10px; overflow: hidden; margin-bottom: 12px; }
.cb-header { display: flex; justify-content: space-between; padding: 10px 16px; background: #161b22; font-size: 12px; color: #aaa; border-bottom: 1px solid #21262d; }
.cb-lang { color: #3b82f6; font-weight: 700; }
.code-block pre { margin: 0; padding: 16px; overflow-x: auto; }
.code-block code { font-family: 'Fira Code', monospace; font-size: 11px; line-height: 1.7; color: #e6edf3; white-space: pre; }
.approach-note { font-size: 12px; color: #93c5fd; background: rgba(59,130,246,0.06); border: 1px solid rgba(59,130,246,0.2); border-radius: 8px; padding: 12px 14px; }

/* HTML Preview */
.html-preview-wrapper { border-radius: 12px; overflow: hidden; background: #0d1117; }
.hp-header { display: flex; justify-content: space-between; align-items: center; padding: 12px 16px; background: #161b22; border-bottom: 1px solid #21262d; font-size: 12px; color: #94a3b8; }
.hp-controls { display: flex; gap: 6px; }
.hp-controls button { font-size: 10px; padding: 3px 10px; border-radius: 4px; border: 1px solid #333; background: transparent; color: #666; cursor: pointer; font-family: 'Sora', sans-serif; }
.hp-controls button.active { background: #3b82f6; color: white; border-color: #3b82f6; }
.html-preview { padding: 16px; transition: all 0.3s; }

/* Theme: light */
.theme-light .hp-doc { background: white; color: #1a202c; }
.theme-light .hpd-header { background: #1e3a5f; color: white; }
.theme-light .hpd-table th { background: #1e3a5f; color: white; }
.theme-light .hpd-table td { border-bottom: 1px solid #e2e8f0; }
.theme-light .hpd-footer { background: #f8fafc; color: #64748b; }

/* Theme: dark */
.theme-dark .hp-doc { background: #1a1a2e; color: #e2e8f0; }
.theme-dark .hpd-header { background: #0f0f23; color: #e2e8f0; border-bottom: 2px solid #3b82f6; }
.theme-dark .hpd-table th { background: #0f0f23; color: #93c5fd; }
.theme-dark .hpd-table td { border-bottom: 1px solid #1e293b; }
.theme-dark .hpd-footer { background: #0f0f23; color: #6b7280; }

/* Theme: corp */
.theme-corp .hp-doc { background: #fafafa; color: #1a202c; font-family: 'Arial', sans-serif; }
.theme-corp .hpd-header { background: #003366; color: white; }
.theme-corp .hpd-table th { background: #003366; color: white; }
.theme-corp .hpd-table td { border-bottom: 1px solid #e5e5e5; }
.theme-corp .hpd-footer { background: #f0f0f0; color: #666; }

.hp-doc { border-radius: 6px; overflow: hidden; box-shadow: 0 10px 40px rgba(0,0,0,0.3); }
.hpd-header { padding: 14px 20px; }
.hpd-logo { font-size: 10px; text-transform: uppercase; letter-spacing: 1px; opacity: 0.7; }
.hpd-title { font-size: 15px; font-weight: 700; margin: 4px 0 2px; }
.hpd-meta { font-size: 10px; opacity: 0.7; }
.hpd-chart-area { padding: 12px; }
.hpd-table { width: 100%; border-collapse: collapse; font-size: 11px; }
.hpd-table th { padding: 6px 10px; text-align: left; }
.hpd-table td { padding: 6px 10px; }
td.score { font-weight: 700; }
td.high { color: #22c55e; }
td.mid { color: #f59e0b; }
td.low { color: #ef4444; }
.hpd-footer { padding: 8px 20px; font-size: 9px; }

/* IronXL */
.ironxl-demo { display: grid; grid-template-columns: 1fr 1.2fr; gap: 24px; }
.feature-cards { display: flex; flex-direction: column; gap: 10px; }
.xl-card { background: #0a0f2e; border-radius: 10px; padding: 16px; border: 1px solid rgba(59,130,246,0.1); display: grid; grid-template-columns: 28px 1fr 100px; gap: 10px; align-items: start; }
.xl-card.ok { border-color: rgba(59,130,246,0.25); }
.xl-card.warn { border-color: rgba(245,158,11,0.25); opacity: 0.8; }
.xl-icon { font-size: 18px; }
.xl-title { font-size: 13px; font-weight: 600; color: #e2e8f0; margin-bottom: 3px; }
.xl-desc { font-size: 11px; color: #6b7280; line-height: 1.4; }
.xl-badge { font-size: 10px; font-weight: 600; color: #93c5fd; text-align: right; }

/* Excel mockup */
.excel-mockup { background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 10px 40px rgba(0,0,0,0.3); }
.em-header { background: #217346; padding: 8px 14px; }
.em-title { color: white; font-size: 12px; font-weight: 600; }
.em-tabs { display: flex; background: #f3f3f3; border-bottom: 1px solid #ccc; }
.em-tab { font-size: 11px; padding: 5px 14px; color: #333; cursor: pointer; border-right: 1px solid #ddd; }
.em-tab.active { background: white; border-bottom: 2px solid #217346; font-weight: 600; }
.em-content { padding: 8px; font-size: 11px; color: #333; }
.em-row { display: grid; grid-template-columns: 2fr 80px 60px 40px; gap: 0; border-bottom: 1px solid #f0f0f0; }
.em-cell { padding: 4px 8px; }
.em-cell.header { background: #f3f3f3; font-weight: 700; color: #666; font-size: 10px; }
.em-cell.bold { font-weight: 700; }
.em-cell.blue-bg { background: #1e3a5f; color: white; }
.em-row.alt { background: #f9f9f9; }
.em-cell.text-sm { font-size: 10px; }
.em-cell.formula { font-weight: 700; }
.em-cell.high { color: #16a34a; }
.em-cell.mid { color: #d97706; }
.em-cell.low { color: #dc2626; }
.em-cell.muted { color: #999; }
.em-chart-area { padding: 8px; border-top: 1px solid #f0f0f0; }

/* S&W */
.sw-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
.sw-panel { background: #0a0f2e; border-radius: 12px; padding: 28px; }
.sw-panel.strengths { border-top: 3px solid #3b82f6; }
.sw-panel.weaknesses { border-top: 3px solid #f59e0b; }
.sw-panel h3 { font-size: 16px; font-weight: 700; margin: 0 0 20px; }
.strengths h3 { color: #3b82f6; }
.weaknesses h3 { color: #f59e0b; }
.sw-item { padding: 12px 0; border-bottom: 1px solid rgba(255,255,255,0.04); }
.sw-item:last-child { border-bottom: none; }
.sw-title { font-size: 13px; font-weight: 600; color: #e2e8f0; margin-bottom: 4px; }
.sw-desc { font-size: 12px; color: #64748b; line-height: 1.5; }

/* Workspace */
.workspace { display: grid; grid-template-columns: 340px 1fr; gap: 20px; }
.workspace-left, .workspace-right { display: flex; flex-direction: column; gap: 16px; }
</style>
