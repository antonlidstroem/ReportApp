<template>
  <div class="quest-page">
    <!-- ── HERO ── -->
    <div class="hero">
      <div class="hero-bg">
        <div class="grid-lines"></div>
        <div class="glow glow-1"></div>
        <div class="glow glow-2"></div>
      </div>
      <div class="hero-inner">
        <router-link to="/" class="back-link">← Dashboard</router-link>
        <div class="hero-eyebrow">
          <span class="track-dot"></span>
          Spår 2 — Open Source Stack
        </div>
        <h1>QuestPDF <span class="plus">+</span> ClosedXML <span class="plus">+</span> ShapeCrawler</h1>
        <p class="hero-sub">
          Tre battle-tested open source-bibliotek, handplockade för sina styrkor.
          Kod-first arkitektur. Noll licenskostnad. Full kontroll.
        </p>
        <div class="hero-badges">
          <span class="badge green">✓ MIT / Community gratis</span>
          <span class="badge green">✓ Ingen Chromium-dep</span>
          <span class="badge green">✓ Fluent C# API</span>
          <span class="badge yellow">⚠ HTML-mallar ej stödda</span>
          <span class="badge yellow">⚠ Grafer manuellt</span>
        </div>
      </div>
      <div class="hero-score-panel">
        <div class="score-ring" :style="`--score:${overallScore}`">
          <div class="ring-inner">
            <span class="ring-val">{{ overallScore }}</span>
            <span class="ring-lbl">/ 10</span>
          </div>
        </div>
        <div class="score-breakdown">
          <div v-for="s in scores" :key="s.label" class="score-row">
            <span class="sr-label">{{ s.label }}</span>
            <div class="sr-bar"><div class="sr-fill" :style="`width:${s.val*10}%; background:${s.color}`"></div></div>
            <span class="sr-val">{{ s.val }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- ── LIBRARY TRINITY ── -->
    <section class="section">
      <h2 class="section-title">De tre biblioteken</h2>
      <div class="trinity">
        <div class="trinity-card" v-for="lib in libraries" :key="lib.name" :style="`--lib-color:${lib.color}`">
          <div class="lib-header">
            <span class="lib-icon">{{ lib.icon }}</span>
            <div>
              <div class="lib-name">{{ lib.name }}</div>
              <div class="lib-role">{{ lib.role }}</div>
            </div>
            <span class="lib-badge" :style="`color:${lib.color}`">{{ lib.badge }}</span>
          </div>
          <p class="lib-desc">{{ lib.desc }}</p>
          <div class="lib-strengths">
            <div v-for="s in lib.strengths" :key="s" class="lib-strength">✓ {{ s }}</div>
          </div>
          <div class="lib-limits">
            <div v-for="l in lib.limits" :key="l" class="lib-limit">✗ {{ l }}</div>
          </div>
          <div class="lib-version">v{{ lib.version }} · {{ lib.license }}</div>
        </div>
      </div>
    </section>

    <!-- ── LIVE PDF PREVIEW ── -->
    <section class="section">
      <h2 class="section-title">Hur en QuestPDF-rapport ser ut</h2>
      <div class="demo-container">
        <div class="demo-left">
          <div class="code-block">
            <div class="cb-header">
              <span>QuestOpenSourceProvider.cs</span>
              <span class="cb-lang">C#</span>
            </div>
            <pre><code>{{ pdfCode }}</code></pre>
          </div>
          <div class="code-note green-note">
            <strong>Styrka:</strong> Fluent C# API ger pixel-perfekt kontroll utan
            en enda rad HTML. Tabeller, grafer (som SVG/bitmap), sidhuvuden och
            fotnoter byggs deklarativt som en LINQ-kedja.
          </div>
        </div>
        <div class="demo-right">
          <!-- Simulated PDF preview -->
          <div class="pdf-mockup">
            <div class="pdf-header">
              <div class="pdf-title">{{ demoSurveyTitle }}</div>
              <div class="pdf-meta">Demo AB · {{ new Date().toISOString().slice(0,10) }}</div>
            </div>
            <div class="pdf-stats-row">
              <div v-for="s in demoStats" :key="s.label" class="pdf-stat">
                <div class="ps-val" :style="`color:${s.color}`">{{ s.val }}</div>
                <div class="ps-lbl">{{ s.label }}</div>
              </div>
            </div>
            <div class="pdf-table">
              <div class="pt-header">
                <span>Fråga</span><span>Snitt</span><span>Svar</span>
              </div>
              <div v-for="(q,i) in demoQuestions" :key="i" class="pt-row" :class="{alt: i%2}">
                <span class="pt-text">{{ q.text }}</span>
                <span class="pt-avg" :style="`color:${scoreColor(q.avg)}`">{{ q.avg.toFixed(2) }}</span>
                <span class="pt-resp">{{ q.resp }}</span>
              </div>
            </div>
            <div class="pdf-trend">
              <div class="trend-label">Trendöversikt</div>
              <div class="trend-bars">
                <div v-for="t in demoTrend" :key="t.month" class="trend-bar-wrap">
                  <div class="trend-bar" :style="`height:${t.val*20}px`"></div>
                  <div class="trend-month">{{ t.month }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ── CHARTS APPROACH ── -->
    <section class="section">
      <h2 class="section-title">Grafer & visualisering</h2>
      <div class="charts-demo">
        <div class="chart-approach-card ok">
          <div class="cac-header">
            <span class="cac-icon">✅</span>
            <span class="cac-title">Pre-rendered SVG / Bitmap</span>
          </div>
          <p>Beräkna data i backend, generera SVG via ScottPlot eller Svg.NET,
          injicera som QuestPDF Image-komponent. Fungerar perfekt.</p>
          <div class="code-snippet">
            <code>page.Content().Image(chartSvgBytes, ImageScaling.FitArea);</code>
          </div>
          <div class="cac-rating">★★★★☆ Bra lösning</div>
        </div>
        <div class="chart-approach-card warn">
          <div class="cac-header">
            <span class="cac-icon">⚠️</span>
            <span class="cac-title">SkiaSharp Custom Drawing</span>
          </div>
          <p>Använd SkiaSharp (som QuestPDF själv använder internt) för att
          rita stapeldiagram och linjediagram direkt. Mer kod men full kontroll.</p>
          <div class="code-snippet">
            <code>col.Item().Canvas((canvas, size) =&gt; DrawBarChart(canvas, size, data));</code>
          </div>
          <div class="cac-rating">★★★★★ Kraftfullast</div>
        </div>
        <div class="chart-approach-card no">
          <div class="cac-header">
            <span class="cac-icon">❌</span>
            <span class="cac-title">Chart.js / D3 i rapport</span>
          </div>
          <p>Fungerar INTE — QuestPDF kräver inte Chromium och kan inte exekvera
          JavaScript. Välj jsreport om du behöver Chart.js direkt i rapporten.</p>
          <div class="cac-rating">✗ Ej möjligt</div>
        </div>
      </div>

      <!-- Live Chart.js demo of what the data looks like -->
      <div class="chart-showcase">
        <div class="cs-header">
          <span>Exempeldata som skulle inkluderas i rapporten</span>
          <span class="cs-note">Visualiserat med Chart.js för demonstration</span>
        </div>
        <div class="chart-grid">
          <canvas ref="barChart" height="200"></canvas>
          <canvas ref="radarChart" height="200"></canvas>
        </div>
      </div>
    </section>

    <!-- ── STRENGTHS & WEAKNESSES ── -->
    <section class="section">
      <div class="sw-grid">
        <div class="sw-panel strengths">
          <h3>💪 Styrkor</h3>
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
      <h2 class="section-title">Testa själv</h2>
      <div class="workspace">
        <div class="workspace-left">
          <SurveyPicker />
          <ModuleList />
        </div>
        <div class="workspace-right">
          <TemplateDesigner provider-name="QuestOpenSource" :supports-html-template="false" />
          <ExportButton provider="QuestOpenSource" :supports-html-template="false" />
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

const barChart = ref<HTMLCanvasElement>()
const radarChart = ref<HTMLCanvasElement>()

const overallScore = 7.8
const scores = [
  { label: 'PDF-kvalitet', val: 9, color: '#22c55e' },
  { label: 'Excel', val: 9, color: '#22c55e' },
  { label: 'PowerPoint', val: 6, color: '#f59e0b' },
  { label: 'Grafstöd', val: 6, color: '#f59e0b' },
  { label: 'Kostnad', val: 10, color: '#22c55e' },
  { label: 'Lärbarhet', val: 7, color: '#f59e0b' },
]

const libraries = [
  {
    name: 'QuestPDF', role: 'PDF-generering', icon: '📕', color: '#22c55e',
    badge: 'Community gratis',
    desc: 'Modernt C# Fluent API för PDF-generering. Ersätter iTextSharp och Telerik Reporting med ett rent, testbart API utan licensproblematik.',
    strengths: ['Deklarativt Fluent API', 'Inga tunga beroenden', 'Enkel testbarhet', 'A4/Letter/custom sidstorlekar', 'Headers, footers, sidnumrering'],
    limits: ['HTML-mallar stöds ej', 'Grafer = SkiaSharp-kod'],
    version: '2026.2', license: 'Community MIT'
  },
  {
    name: 'ClosedXML', role: 'Excel (.xlsx)', icon: '📗', color: '#3b82f6',
    badge: 'MIT-licens',
    desc: 'Mogen .NET-bibliotek för Excel-generering med ett läsbart API. Bygger på OpenXML SDK men exponerar ett mycket mer användarvänligt gränssnitt.',
    strengths: ['Formler & beräkningar', 'Cellformatering', 'Conditionell formatering', 'Namngivna intervall', 'Datavalidering'],
    limits: ['Inga native charts', 'Stora filer kan vara långsamma'],
    version: '0.105', license: 'MIT'
  },
  {
    name: 'ShapeCrawler', role: 'PowerPoint (.pptx)', icon: '📘', color: '#a855f7',
    badge: 'MIT-licens',
    desc: 'Modern .NET-bibliotek för PowerPoint-manipulation. Ersätter PresentationML-direktanrop med ett intuitivt shapes-baserat API.',
    strengths: ['Lägg till textboxar', 'Bakgrundsfärger per slide', 'Bildformat & storleksändring', 'Template-baserat arbetsflöde'],
    limits: ['Inga native editable charts', 'Begränsad animationskontroll', 'Yngre bibliotek'],
    version: '0.50', license: 'MIT'
  },
]

const pdfCode = `Document.Create(container => {
  container.Page(page => {
    page.Size(PageSizes.A4);
    page.Margin(40);

    page.Header()
      .BorderBottom(3).BorderColor("#22c55e")
      .Row(row => {
        row.RelativeItem()
           .Text(data.SurveyTitle)
           .FontSize(22).Bold();
      });

    page.Content().Column(col => {
      // Stats-rad
      col.Item().Row(row => {
        StatBox(row, "Frågor", count.ToString(), "#3b82f6");
        StatBox(row, "Svar",   total.ToString(), "#22c55e");
        StatBox(row, "Snitt",  avg.ToString("F2"), "#f59e0b");
      });

      // Frågetabell per kategori
      foreach (var group in grouped) {
        col.Item().Table(table => {
          // kolumndefinitioner + rader...
        });
      }
    });

    page.Footer().Row(row => {
      row.RelativeItem().Text("QuestPDF");
      row.ConstantItem(60).Text(x => {
        x.CurrentPageNumber(); x.Span(" / ");
        x.TotalPages();
      });
    });
  });
});`

const demoSurveyTitle = 'Arbetsmiljökartläggning 2026'
const demoStats = [
  { val: '8', label: 'Frågor', color: '#3b82f6' },
  { val: '245', label: 'Svar', color: '#22c55e' },
  { val: '3.84', label: 'Snitt', color: '#f59e0b' },
  { val: '4', label: 'Kategorier', color: '#a855f7' },
]
const demoQuestions = [
  { text: 'Hur upplever du din arbetsbelastning?', avg: 3.2, resp: 34 },
  { text: 'Upplever du stöd från din närmaste chef?', avg: 4.1, resp: 31 },
  { text: 'Är den fysiska arbetsmiljön tillfredsställande?', avg: 3.8, resp: 29 },
  { text: 'Har du tillgång till rätt verktyg?', avg: 4.3, resp: 33 },
  { text: 'Känner du dig inkluderad i beslut?', avg: 3.5, resp: 30 },
]
const demoTrend = [
  { month: 'Jan', val: 3.2 }, { month: 'Feb', val: 3.4 }, { month: 'Mar', val: 3.7 },
  { month: 'Apr', val: 3.5 }, { month: 'Maj', val: 3.9 }, { month: 'Jun', val: 4.1 },
]

function scoreColor(v: number) {
  return v >= 4 ? '#22c55e' : v >= 3 ? '#f59e0b' : '#ef4444'
}

const strengthsList = [
  { title: 'Noll licenskostnad', desc: 'Community-licens gratis upp till $1M omsättning. Open source — inga bindande leverantörsavtal.' },
  { title: 'Inga tunga beroenden', desc: 'Ingen Chromium-installation, ingen Office, inga externa processer. Körs i samma .NET-process.' },
  { title: 'Testbar kod', desc: 'Allt är C#-kod. Du kan enhetstesta din rapport-logik med xUnit/NUnit som vilken service som helst.' },
  { title: 'Pixel-perfekt PDF', desc: 'QuestPDF ger exakt same output varje gång. Ingen browser-varians, ingen CSS-beräkningsskillnad.' },
  { title: 'Bästa Excel-stödet', desc: 'ClosedXML stödjer conditionell formatering, formler, datavalidering och flera ark.' },
]

const weaknessList = [
  { title: 'Inget HTML-mallstöd', desc: 'Designern i appen kan inte göra HTML-mallar. Layoutförändringar kräver C#-kodändringar.' },
  { title: 'Grafer kräver extra arbete', desc: 'Inget inbyggt grafstöd. Du behöver SkiaSharp eller ScottPlot och skriva ritlogik manuellt.' },
  { title: 'PowerPoint är begränsat', desc: 'ShapeCrawler stödjer textboxar och bakgrunder men inga native editable charts som IronPPT ger.' },
  { title: 'Brant inlärningskurva för komplexa layouter', desc: 'Avancerade multi-kolumn layouter och floating elements kan vara knepiga med Fluent API.' },
  { title: 'Tre bibliotek = tre API:er', desc: 'Teamet måste lära sig tre olika API-stilar och göra tre separata uppgraderingar.' },
]

onMounted(() => {
  // Bar chart
  if (barChart.value) {
    new Chart(barChart.value, {
      type: 'bar',
      data: {
        labels: demoQuestions.map((_, i) => `Q${i+1}`),
        datasets: [{
          label: 'Genomsnitt per fråga',
          data: demoQuestions.map(q => q.avg),
          backgroundColor: demoQuestions.map(q => scoreColor(q.avg) + 'cc'),
          borderColor: demoQuestions.map(q => scoreColor(q.avg)),
          borderWidth: 2,
          borderRadius: 4,
        }]
      },
      options: {
        plugins: { legend: { labels: { color: '#94a3b8' } } },
        scales: {
          y: { min: 0, max: 5, ticks: { color: '#94a3b8' }, grid: { color: '#1e293b' } },
          x: { ticks: { color: '#94a3b8' }, grid: { color: '#1e293b' } },
        },
      }
    })
  }

  // Radar chart
  if (radarChart.value) {
    new Chart(radarChart.value, {
      type: 'radar',
      data: {
        labels: ['Ledarskap', 'Psykosocialt', 'Hälsa', 'Säkerhet', 'Resurser'],
        datasets: [{
          label: 'Genomsnitt per kategori',
          data: [4.1, 3.2, 3.8, 4.5, 3.5],
          backgroundColor: 'rgba(34,197,94,0.15)',
          borderColor: '#22c55e',
          pointBackgroundColor: '#22c55e',
        }]
      },
      options: {
        plugins: { legend: { labels: { color: '#94a3b8' } } },
        scales: {
          r: {
            min: 0, max: 5,
            ticks: { color: '#94a3b8', stepSize: 1 },
            grid: { color: '#1e293b' },
            pointLabels: { color: '#94a3b8' },
          }
        }
      }
    })
  }
})
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Space+Mono:wght@400;700&family=DM+Sans:wght@300;400;500;700&display=swap');

.quest-page { font-family: 'DM Sans', sans-serif; max-width: 1300px; margin: 0 auto; color: #e2e8f0; }

/* Hero */
.hero {
  position: relative;
  overflow: hidden;
  border-radius: 20px;
  background: #030b0f;
  margin-bottom: 48px;
  display: grid;
  grid-template-columns: 1fr 340px;
  gap: 0;
  min-height: 380px;
}
.hero-bg { position: absolute; inset: 0; pointer-events: none; }
.grid-lines {
  position: absolute; inset: 0;
  background-image:
    linear-gradient(rgba(34,197,94,0.04) 1px, transparent 1px),
    linear-gradient(90deg, rgba(34,197,94,0.04) 1px, transparent 1px);
  background-size: 40px 40px;
}
.glow {
  position: absolute;
  border-radius: 50%;
  filter: blur(80px);
  pointer-events: none;
}
.glow-1 { width: 400px; height: 400px; background: rgba(34,197,94,0.12); top: -100px; left: -80px; }
.glow-2 { width: 300px; height: 300px; background: rgba(59,130,246,0.08); bottom: -80px; right: 200px; }

.hero-inner { position: relative; padding: 48px; z-index: 1; }
.back-link { color: rgba(255,255,255,0.4); text-decoration: none; font-size: 13px; display: block; margin-bottom: 20px; }
.back-link:hover { color: #22c55e; }
.hero-eyebrow {
  display: flex; align-items: center; gap: 8px;
  font-size: 11px; text-transform: uppercase; letter-spacing: 2px;
  color: #22c55e; margin-bottom: 16px;
}
.track-dot { width: 8px; height: 8px; border-radius: 50%; background: #22c55e; box-shadow: 0 0 12px #22c55e; }
h1 { font-family: 'Space Mono', monospace; font-size: 28px; font-weight: 700; margin: 0 0 16px; line-height: 1.3; color: #fff; }
.plus { color: #22c55e; }
.hero-sub { font-size: 15px; color: rgba(255,255,255,0.6); margin: 0 0 24px; max-width: 520px; line-height: 1.6; }
.hero-badges { display: flex; flex-wrap: wrap; gap: 8px; }
.badge {
  font-size: 11px; padding: 4px 12px; border-radius: 100px; border: 1px solid;
  font-weight: 500;
}
.badge.green { color: #22c55e; border-color: rgba(34,197,94,0.3); background: rgba(34,197,94,0.08); }
.badge.yellow { color: #f59e0b; border-color: rgba(245,158,11,0.3); background: rgba(245,158,11,0.08); }

/* Score panel */
.hero-score-panel {
  position: relative; z-index: 1;
  background: rgba(255,255,255,0.03);
  border-left: 1px solid rgba(34,197,94,0.15);
  padding: 40px 32px;
  display: flex; flex-direction: column; align-items: center; gap: 28px;
}
.score-ring {
  width: 110px; height: 110px;
  border-radius: 50%;
  background: conic-gradient(#22c55e calc(var(--score)/10 * 360deg), #1e293b 0);
  display: flex; align-items: center; justify-content: center;
}
.ring-inner {
  width: 84px; height: 84px; border-radius: 50%;
  background: #030b0f;
  display: flex; flex-direction: column; align-items: center; justify-content: center;
}
.ring-val { font-family: 'Space Mono', monospace; font-size: 26px; font-weight: 700; color: #22c55e; line-height: 1; }
.ring-lbl { font-size: 11px; color: #6b7280; }
.score-breakdown { width: 100%; display: flex; flex-direction: column; gap: 8px; }
.score-row { display: flex; align-items: center; gap: 8px; }
.sr-label { font-size: 11px; color: #94a3b8; width: 90px; flex-shrink: 0; }
.sr-bar { flex: 1; height: 4px; background: #1e293b; border-radius: 2px; overflow: hidden; }
.sr-fill { height: 100%; border-radius: 2px; transition: width 1s ease; }
.sr-val { font-size: 11px; font-weight: 600; color: #e2e8f0; width: 20px; text-align: right; }

/* Sections */
.section { margin-bottom: 48px; }
.section-title { font-family: 'Space Mono', monospace; font-size: 20px; color: #fff; margin: 0 0 24px; }

/* Trinity */
.trinity { display: grid; grid-template-columns: repeat(3, 1fr); gap: 16px; }
.trinity-card {
  background: #0a1628;
  border: 1px solid rgba(255,255,255,0.07);
  border-top: 3px solid var(--lib-color);
  border-radius: 12px;
  padding: 24px;
}
.lib-header { display: flex; align-items: flex-start; gap: 12px; margin-bottom: 12px; }
.lib-icon { font-size: 24px; flex-shrink: 0; }
.lib-name { font-size: 16px; font-weight: 700; color: #fff; }
.lib-role { font-size: 11px; color: #6b7280; margin-top: 2px; }
.lib-badge { margin-left: auto; font-size: 10px; font-weight: 600; flex-shrink: 0; }
.lib-desc { font-size: 12px; color: #94a3b8; line-height: 1.6; margin-bottom: 12px; }
.lib-strengths { display: flex; flex-direction: column; gap: 4px; margin-bottom: 10px; }
.lib-strength { font-size: 11px; color: #22c55e; }
.lib-limits { display: flex; flex-direction: column; gap: 4px; margin-bottom: 10px; }
.lib-limit { font-size: 11px; color: #ef4444; }
.lib-version { font-size: 10px; color: #475569; padding-top: 8px; border-top: 1px solid #1e293b; }

/* Demo container */
.demo-container { display: grid; grid-template-columns: 1fr 1fr; gap: 24px; }
.code-block { background: #1a1a2e; border-radius: 10px; overflow: hidden; margin-bottom: 12px; }
.cb-header { display: flex; justify-content: space-between; padding: 10px 16px; background: #16213e; font-size: 12px; color: #aaa; border-bottom: 1px solid #0f3460; }
.cb-lang { color: #22c55e; font-weight: 700; }
.code-block pre { margin: 0; padding: 16px; overflow-x: auto; }
.code-block code { font-family: 'Space Mono', monospace; font-size: 11px; line-height: 1.7; color: #d4d4d4; white-space: pre; }
.code-note { font-size: 12px; padding: 12px 16px; border-radius: 8px; line-height: 1.6; }
.green-note { background: rgba(34,197,94,0.06); border: 1px solid rgba(34,197,94,0.2); color: #94a3b8; }

/* PDF Mockup */
.pdf-mockup {
  background: white;
  border-radius: 8px;
  padding: 24px;
  box-shadow: 0 20px 60px rgba(0,0,0,0.4);
  font-family: 'DM Sans', sans-serif;
  color: #1a202c;
  transform: perspective(1000px) rotateY(-3deg);
  height: 100%;
}
.pdf-header { border-bottom: 3px solid #22c55e; padding-bottom: 10px; margin-bottom: 14px; }
.pdf-title { font-size: 16px; font-weight: 700; color: #1e3a5f; }
.pdf-meta { font-size: 10px; color: #64748b; margin-top: 2px; }
.pdf-stats-row { display: flex; gap: 12px; margin-bottom: 14px; }
.pdf-stat { flex: 1; border: 1px solid #e2e8f0; border-radius: 6px; padding: 8px; text-align: center; }
.ps-val { font-size: 16px; font-weight: 700; }
.ps-lbl { font-size: 8px; color: #94a3b8; margin-top: 2px; }
.pdf-table { margin-bottom: 14px; }
.pt-header { display: grid; grid-template-columns: 1fr 50px 40px; background: #f1f5f9; border-bottom: 2px solid #94a3b8; padding: 5px 8px; font-size: 9px; font-weight: 700; color: #1e3a5f; gap: 8px; }
.pt-row { display: grid; grid-template-columns: 1fr 50px 40px; padding: 5px 8px; border-bottom: 1px solid #f1f5f9; font-size: 9px; gap: 8px; }
.pt-row.alt { background: #f8fafc; }
.pt-text { color: #334155; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.pt-avg { font-weight: 700; text-align: center; }
.pt-resp { color: #64748b; text-align: center; }
.pdf-trend { }
.trend-label { font-size: 9px; font-weight: 700; color: #1e3a5f; margin-bottom: 6px; }
.trend-bars { display: flex; align-items: flex-end; gap: 6px; height: 60px; }
.trend-bar-wrap { flex: 1; display: flex; flex-direction: column; align-items: center; gap: 2px; height: 100%; justify-content: flex-end; }
.trend-bar { width: 100%; background: #22c55e; border-radius: 2px 2px 0 0; min-height: 4px; }
.trend-month { font-size: 7px; color: #94a3b8; }

/* Charts demo */
.charts-demo { display: grid; grid-template-columns: repeat(3, 1fr); gap: 16px; margin-bottom: 24px; }
.chart-approach-card {
  border-radius: 10px; padding: 20px;
  border: 1px solid;
}
.chart-approach-card.ok { background: rgba(34,197,94,0.05); border-color: rgba(34,197,94,0.2); }
.chart-approach-card.warn { background: rgba(245,158,11,0.05); border-color: rgba(245,158,11,0.2); }
.chart-approach-card.no { background: rgba(239,68,68,0.05); border-color: rgba(239,68,68,0.2); opacity: 0.7; }
.cac-header { display: flex; align-items: center; gap: 8px; margin-bottom: 10px; }
.cac-icon { font-size: 18px; }
.cac-title { font-size: 13px; font-weight: 700; color: #e2e8f0; }
.chart-approach-card p { font-size: 12px; color: #94a3b8; line-height: 1.6; margin-bottom: 10px; }
.code-snippet { background: #1a1a2e; border-radius: 6px; padding: 8px 12px; font-size: 10px; font-family: 'Space Mono', monospace; color: #22c55e; overflow-x: auto; white-space: nowrap; margin-bottom: 10px; }
.cac-rating { font-size: 11px; color: #94a3b8; }

.chart-showcase { background: #0a1628; border-radius: 12px; overflow: hidden; }
.cs-header { display: flex; justify-content: space-between; align-items: center; padding: 14px 20px; background: rgba(255,255,255,0.03); border-bottom: 1px solid rgba(255,255,255,0.05); font-size: 13px; font-weight: 600; }
.cs-note { font-size: 11px; color: #6b7280; font-weight: 400; }
.chart-grid { display: grid; grid-template-columns: 1fr 1fr; padding: 20px; gap: 20px; }

/* S&W */
.sw-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
.sw-panel { background: #0a1628; border-radius: 12px; padding: 28px; }
.sw-panel.strengths { border-top: 3px solid #22c55e; }
.sw-panel.weaknesses { border-top: 3px solid #f59e0b; }
.sw-panel h3 { font-size: 16px; font-weight: 700; margin: 0 0 20px; }
.strengths h3 { color: #22c55e; }
.weaknesses h3 { color: #f59e0b; }
.sw-item { padding: 12px 0; border-bottom: 1px solid rgba(255,255,255,0.04); }
.sw-item:last-child { border-bottom: none; }
.sw-title { font-size: 13px; font-weight: 600; color: #e2e8f0; margin-bottom: 4px; }
.sw-desc { font-size: 12px; color: #64748b; line-height: 1.5; }

/* Workspace */
.workspace { display: grid; grid-template-columns: 340px 1fr; gap: 20px; }
.workspace-left, .workspace-right { display: flex; flex-direction: column; gap: 16px; }
</style>
