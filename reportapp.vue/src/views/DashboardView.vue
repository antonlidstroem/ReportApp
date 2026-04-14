<template>
  <div class="dashboard">
    <!-- ── HERO ── -->
    <header class="dash-hero">
      <div class="dh-bg">
        <div class="dh-grid"></div>
        <div class="dh-glow"></div>
      </div>
      <div class="dh-content">
        <div class="dh-tag">Proof of Concept · .NET 8 Reporting Engine</div>
        <h1>Modern Reporting Engine</h1>
        <p>Fyra tekniska spår. En databas. Obegränsade möjligheter att exportera.</p>
        <div class="dh-meta">
          <div class="meta-pill" v-for="m in heroMeta" :key="m.label">
            <span class="mp-val">{{ m.val }}</span>
            <span class="mp-lbl">{{ m.label }}</span>
          </div>
        </div>
      </div>
    </header>

    <!-- ── PROVIDER CARDS ── -->
    <section class="section">
      <h2 class="section-title">Tekniska spår</h2>
      <div class="provider-grid">
        <router-link
          v-for="p in providers"
          :key="p.route"
          :to="p.route"
          class="provider-card"
          :style="`--c:${p.color}`"
        >
          <div class="pc-top">
            <span class="pc-icon">{{ p.icon }}</span>
            <span class="pc-track">Spår {{ p.track }}</span>
          </div>
          <h3>{{ p.name }}</h3>
          <p>{{ p.description }}</p>
          <div class="pc-tags">
            <span v-for="t in p.tags" :key="t" class="tag">{{ t }}</span>
          </div>
          <div class="pc-scores">
            <div v-for="s in p.scores" :key="s.l" class="pcs-row">
              <span class="pcs-l">{{ s.l }}</span>
              <div class="pcs-bar">
                <div class="pcs-fill" :style="`width:${s.v * 10}%;background:${p.color}`"></div>
              </div>
            </div>
          </div>
          <div class="pc-footer">
            <span :class="['pc-cost', p.costClass]">{{ p.cost }}</span>
            <span class="pc-arrow">→</span>
          </div>
        </router-link>
      </div>
    </section>

    <!-- ── FULL COMPARISON MATRIX ── -->
    <section class="section">
      <h2 class="section-title">Fullständig jämförelse</h2>

      <!-- Filter tabs -->
      <div class="matrix-filters">
        <button
          v-for="f in matrixFilters"
          :key="f.id"
          class="mf-btn"
          :class="{ active: activeFilter === f.id }"
          @click="activeFilter = f.id"
        >
          {{ f.label }}
        </button>
      </div>

      <div class="matrix-wrap">
        <table class="matrix">
          <thead>
            <tr>
              <th class="feature-col">Funktion / Kategori</th>
              <th v-for="p in providers" :key="p.name" :style="`--c:${p.color}`">
                <div class="mh-inner">
                  <span class="mh-icon">{{ p.icon }}</span>
                  <span>{{ p.shortName }}</span>
                </div>
              </th>
            </tr>
          </thead>
          <tbody>
            <template v-for="(group, gi) in filteredMatrix" :key="group.category">
              <tr class="category-row">
                <td colspan="5" class="cat-header">{{ group.category }}</td>
              </tr>
              <tr v-for="(row, ri) in group.rows" :key="ri" class="data-row">
                <td class="feature-name">{{ row.feature }}</td>
                <td v-for="(val, vi) in row.values" :key="vi" class="feature-cell">
                  <span v-if="val === true" class="check">✓</span>
                  <span v-else-if="val === false" class="cross">✗</span>
                  <span v-else-if="typeof val === 'number'" class="score-chip" :style="`--s:${val}`"
                    >{{ val }}/10</span
                  >
                  <span v-else class="partial">{{ val }}</span>
                </td>
              </tr>
            </template>
          </tbody>
        </table>
      </div>
    </section>

    <!-- ── PROVIDER RADAR ── -->
    <section class="section">
      <h2 class="section-title">Styrke-radar per provider</h2>
      <div class="radar-section">
        <div class="radar-charts">
          <div v-for="p in providers" :key="p.name" class="radar-wrap">
            <div class="rw-title" :style="`color:${p.color}`">{{ p.icon }} {{ p.shortName }}</div>
            <canvas
              :ref="(el) => (radarRefs[p.shortName] = el as HTMLCanvasElement)"
              height="200"
            ></canvas>
          </div>
        </div>
      </div>
    </section>

    <section>
      <div class="radar-legend">
        <div v-for="dim in radarDims" :key="dim" class="rl-row">
          <span class="rl-dot"></span>
          {{ dim }}
        </div>
      </div>
    </section>

    <br />

    <!-- ── BENCHMARK HISTORY ── -->
    <section class="section">
      <div class="section-header">
        <h2 class="section-title">Benchmark-historik</h2>
        <button @click="benchmarks.clear()" class="clear-btn" v-if="benchmarks.results.length">
          🗑 Rensa
        </button>
      </div>

      <div v-if="!benchmarks.results.length" class="empty-bench">
        <div class="eb-icon">📊</div>
        <div class="eb-title">Inga mätningar ännu</div>
        <div class="eb-sub">
          Gå till en provider-sida och exportera en rapport för att se prestandadata här.
        </div>
      </div>

      <div v-else class="bench-grid">
        <!-- Speed chart -->
        <div class="bench-chart-card">
          <div class="bcc-header">Renderingstid per export</div>
          <canvas ref="benchBarRef" height="200"></canvas>
        </div>

        <!-- Stats cards -->
        <div class="bench-stats">
          <div v-for="stat in benchStats" :key="stat.label" class="bs-card">
            <div class="bs-val" :style="`color:${stat.color}`">{{ stat.val }}</div>
            <div class="bs-lbl">{{ stat.label }}</div>
          </div>
        </div>

        <!-- Log table -->
        <div class="bench-table-card">
          <div class="btc-header">Senaste 10 exporter</div>
          <div class="bench-table">
            <div class="bt-row header">
              <span>Provider</span><span>Format</span><span>Tid</span><span>Storlek</span
              ><span>Frågor</span>
            </div>
            <div v-for="r in benchmarks.results.slice(0, 10)" :key="r.timestamp" class="bt-row">
              <span class="bt-provider">{{ r.provider }}</span>
              <span class="bt-format">{{ r.format.toUpperCase() }}</span>
              <span class="bt-time" :class="speedClass(r.generationMs)"
                >{{ r.generationMs }}ms</span
              >
              <span class="bt-size">{{ formatSize(r.fileSizeBytes) }}</span>
              <span class="bt-q">{{ r.questionCount }}</span>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ── DECISION GUIDE ── -->
    <section class="section">
      <h2 class="section-title">Välj rätt provider för ditt projekt</h2>
      <div class="decision-grid">
        <div
          v-for="d in decisionGuide"
          :key="d.scenario"
          class="decision-card"
          :style="`--dc:${d.color}`"
        >
          <div class="dc-icon">{{ d.icon }}</div>
          <div class="dc-scenario">{{ d.scenario }}</div>
          <div class="dc-winner">→ {{ d.winner }}</div>
          <div class="dc-reason">{{ d.reason }}</div>
        </div>
      </div>
    </section>

    <!-- ── STRESS TEST CTA ── -->
    <section class="section stress-section">
      <div class="stress-inner">
        <div>
          <h2>⚡ Stresstesta alla providers</h2>
          <p>Kör export mot 500-fråge-enkäten (100 000 svar) och se prestandaskillnaderna.</p>
        </div>
        <div class="stress-btns">
          <router-link
            v-for="p in providers"
            :key="p.route"
            :to="p.route"
            class="stress-btn"
            :style="`--c:${p.color}`"
          >
            {{ p.icon }} {{ p.shortName }}
          </router-link>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from "vue";
import { useBenchmarkStore } from "../stores/benchmarks";
import { fetchSurveys } from "../composables/useApi";
import { useReportBuilderStore } from "../stores/reportBuilder";
import Chart from "chart.js/auto";

const benchmarks = useBenchmarkStore();
const store = useReportBuilderStore();
const radarRefs = ref<Record<string, HTMLCanvasElement>>({});
const benchBarRef = ref<HTMLCanvasElement>();
let benchBarChart: Chart | null = null;
const activeFilter = ref("all");

const heroMeta = [
  { val: "4", label: "Providers" },
  { val: "4", label: "Enkäter" },
  { val: "100k+", label: "Svar" },
  { val: "3", label: "Format" },
];

const providers = [
  {
    route: "/providers/jsreport",
    track: 1,
    icon: "🌐",
    name: "jsreport (Pure Web)",
    shortName: "jsreport",
    description: "Ren webb-motor. Bäst för PDF, men begränsad i Excel/PPT.",
    tags: ["PDF Focus", "Chart.js"],
    cost: "OSS/Pro",
    costClass: "freemium",
    color: "#a855f7",
    scores: [
      { l: "PDF", v: 10 },
      { l: "Excel", v: 4 },
    ],
  },
  {
    route: "/providers/syncfusion",
    track: 2,
    icon: "💎",
    name: "Syncfusion (Pure Native)",
    shortName: "Syncfusion",
    description: "Ren enterprise-motor. Bäst för Office, men saknar HTML-rendering.",
    tags: ["Office Focus", "Formulas"],
    cost: "Community",
    costClass: "free",
    color: "#14b8a6",
    scores: [
      { l: "Excel", v: 10 },
      { l: "PDF", v: 7 },
    ],
  },
  {
    route: "/providers/js-sync",
    track: 3,
    icon: "⚒️",
    name: "jsreport + Syncfusion",
    shortName: "Hybrid A",
    description: "Kombinerar jsreports design med Syncfusions datakraft.",
    tags: ["Best of Both", "Designer Friendly"],
    cost: "Enterprise",
    costClass: "paid",
    color: "#8b5cf6",
    scores: [
      { l: "PDF", v: 10 },
      { l: "Excel", v: 10 },
    ],
  },
  {
    route: "/providers/play-sync",
    track: 4,
    icon: "🎭",
    name: "Playwright + Syncfusion",
    shortName: "Hybrid B",
    description: "Modern, snabb stack för både webb-PDF och native Office.",
    tags: ["Modern", "Lightweight PDF"],
    cost: "Enterprise",
    costClass: "paid",
    color: "#10b981",
    scores: [
      { l: "PDF", v: 9 },
      { l: "Excel", v: 10 },
    ],
  },
];

const matrixFilters = [
  { id: "all", label: "Alla" },
  { id: "output", label: "📄 Filformat" },
  { id: "charts", label: "📊 Grafer" },
  { id: "advanced", label: "🔐 Avancerat" },
  { id: "cost", label: "💰 Kostnad & DX" },
];

const fullMatrix = [
  {
    category: "📄 Filformat & Kvalitet",
    filter: "output",
    rows: [
      { feature: "PDF-generering", values: [true, true, true, true] },
      { feature: "Excel (.xlsx)", values: [true, true, "⚠ HTML only", true] },
      { feature: "PowerPoint (.pptx)", values: ["Basic", true, "Fallback", true] },
      { feature: "HTML → PDF", values: [false, true, true, false] },
      { feature: "PDF sidnumrering", values: [true, true, true, true] },
      { feature: "Multi-sheet Excel", values: [true, true, false, true] },
    ],
  },
  {
    category: "📊 Grafer & Visualisering",
    filter: "charts",
    rows: [
      { feature: "Chart.js i PDF", values: [false, false, true, false] },
      { feature: "Pre-rendered SVG/bitmap", values: [true, true, true, true] },
      { feature: "Native Excel-grafer (redigerbara)", values: [false, true, false, true] },
      { feature: "Native PPT-grafer (redigerbara)", values: [false, true, false, true] },
      { feature: "SkiaSharp custom drawing", values: [true, false, false, false] },
      {
        feature: "Radar/donut/linje-chart i PDF",
        values: ["Manuellt", "Via HTML", "Chart.js", "Manuellt"],
      },
    ],
  },
  {
    category: "🔐 Säkerhet & Enterprise",
    filter: "advanced",
    rows: [
      { feature: "AES-256 PDF-kryptering", values: [false, true, false, true] },
      { feature: "Lösenordsskydd", values: [false, true, false, true] },
      { feature: "Vattenstämpel", values: [false, true, true, true] },
      { feature: "PDF/UA Accessibility", values: [false, false, false, true] },
      { feature: "Digitala signaturer", values: [false, true, false, true] },
      { feature: "Excel conditionell formatering", values: [true, true, false, true] },
      { feature: "Excel formler (=AVERAGE etc.)", values: [false, "Begränsat", false, true] },
    ],
  },
  {
    category: "💰 Kostnad & Developer Experience",
    filter: "cost",
    rows: [
      { feature: "Licenskostnad", values: ["Gratis", "$$$", "OSS/Pro", "Community/Pro"] },
      { feature: "Kräver Chromium-process", values: [false, true, true, false] },
      { feature: "HTML-mallstöd", values: [false, true, true, false] },
      { feature: "Enhetstestbar", values: [true, "Partiell", false, "Partiell"] },
      { feature: "Antal bibliotek", values: ["3 OSS", "1 Suite", "1 + Node.js", "3 SF"] },
      { feature: "Cold start tid", values: ["< 100ms", "2-5s", "2-5s", "< 100ms"] },
      { feature: "Dokumentation", values: ["Utmärkt", "Utmärkt", "Bra", "Utmärkt"] },
    ],
  },
];

const filteredMatrix = computed(() => {
  if (activeFilter.value === "all") return fullMatrix;
  return fullMatrix.filter((g) => g.filter === activeFilter.value || activeFilter.value === "all");
});

const radarDims = [
  "PDF-kvalitet",
  "Excel",
  "PPT",
  "Grafer",
  "Kostnad/licens",
  "HTML-mallar",
  "Enterprise",
  "Prestanda",
];

const radarData: Record<string, number[]> = {
  QuestOSS: [9, 9, 6, 5, 10, 0, 4, 9],
  IronSuite: [9, 8, 8, 8, 3, 9, 9, 7],
  jsreport: [9, 5, 3, 10, 7, 10, 5, 6],
  Syncfusion: [8, 10, 9, 7, 8, 0, 10, 9],
};

const radarColors: Record<string, string> = {
  QuestOSS: "#22c55e",
  IronSuite: "#3b82f6",
  jsreport: "#a855f7",
  Syncfusion: "#14b8a6",
};

const decisionGuide = [
  {
    icon: "💰",
    scenario: "Budget är viktigast",
    winner: "QuestPDF + ClosedXML",
    reason: "Noll licenskostnad för Community. Alla tre bibliotek är MIT.",
    color: "#22c55e",
  },
  {
    icon: "🎨",
    scenario: "Frontend-designern ska bygga mallen",
    winner: "IronSuite eller jsreport",
    reason: "Båda stödjer HTML/CSS-mallar. Designern arbetar i sitt rätta medium.",
    color: "#3b82f6",
  },
  {
    icon: "📊",
    scenario: "Chart.js-grafer direkt i PDF",
    winner: "jsreport",
    reason: "Enda providern som kör JavaScript (Chart.js) inuti Chromium vid rendering.",
    color: "#a855f7",
  },
  {
    icon: "🧮",
    scenario: "Excel-formulär som räknar om sig",
    winner: "Syncfusion XlsIO",
    reason: "400+ native Excel-formler. Mottagaren öppnar Excel — data räknas om automatiskt.",
    color: "#14b8a6",
  },
  {
    icon: "🔐",
    scenario: "Kryptering, signaturer, PDF/UA",
    winner: "Syncfusion eller IronSuite",
    reason: "Båda erbjuder enterprise-grade PDF-säkerhet. Syncfusion leder på accessibility.",
    color: "#f59e0b",
  },
  {
    icon: "⚡",
    scenario: "Snabbast möjliga rendering",
    winner: "QuestPDF / Syncfusion",
    reason: "Inga externa processer. Ren .NET — renderingstid under 100ms för normala rapporter.",
    color: "#ef4444",
  },
];

// Benchmark computed
const benchStats = computed(() => {
  const rs = benchmarks.results;
  if (!rs.length) return [];
  const fastest = rs.reduce((a, b) => (a.generationMs < b.generationMs ? a : b));
  const avgMs = Math.round(rs.reduce((s, r) => s + r.generationMs, 0) / rs.length);
  const totalSize = rs.reduce((s, r) => s + r.fileSizeBytes, 0);
  return [
    { val: `${fastest.generationMs}ms`, label: `Snabbast (${fastest.provider})`, color: "#22c55e" },
    { val: `${avgMs}ms`, label: "Genomsnitt", color: "#f59e0b" },
    { val: `${rs.length}`, label: "Exporter", color: "#3b82f6" },
    { val: formatSize(totalSize), label: "Total storlek", color: "#a855f7" },
  ];
});

function speedClass(ms: number) {
  return ms < 500 ? "fast" : ms < 2000 ? "ok" : "slow";
}
function formatSize(bytes: number) {
  if (!bytes) return "—";
  if (bytes > 1_000_000) return `${(bytes / 1_000_000).toFixed(1)} MB`;
  return `${(bytes / 1024).toFixed(0)} KB`;
}

function drawRadarCharts() {
  providers.forEach((p) => {
    const canvas = radarRefs.value[p.shortName];
    if (!canvas) return;
    const existing = (canvas as HTMLCanvasElement & { _chartInstance?: Chart })._chartInstance;
    if (existing) existing.destroy();

    const inst = new Chart(canvas, {
      type: "radar",
      data: {
        labels: radarDims,
        datasets: [
          {
            // FIX: Add fallback empty array and cast to number[]
            data: (radarData[p.shortName] ?? []) as number[],
            backgroundColor: `${p.color}22`,
            borderColor: p.color,
            pointBackgroundColor: p.color,
            borderWidth: 2,
            pointRadius: 3,
          },
        ],
      },
      options: {
        plugins: { legend: { display: false } },
        scales: {
          r: {
            min: 0,
            max: 10,
            ticks: { color: "#475569", stepSize: 2, font: { size: 8 } },
            grid: { color: "#1e293b" },
            pointLabels: { color: "#94a3b8", font: { size: 9 } },
          },
        },
      },
    });
    (canvas as HTMLCanvasElement & { _chartInstance?: Chart })._chartInstance = inst;
  });
}

function drawBenchChart() {
  // 1. Skapa en lokal referens för att "låsa" typen så TS inte tvivlar
  const canvasElement = benchBarRef.value;
  if (!canvasElement || !benchmarks.results.length) return;

  if (benchBarChart) benchBarChart.destroy();

  const recent = benchmarks.results.slice(0, 8).reverse();

  benchBarChart = new Chart(canvasElement, {
    // Använd den lokala variabeln här
    type: "bar",
    data: {
      labels: recent.map((r) => `${r.provider ?? "Okänd"} ${(r.format ?? "PDF").toUpperCase()}`),
      datasets: [
        {
          data: recent.map((r) => r.generationMs),
          backgroundColor: recent.map((r) => {
            const providerName = r.provider ?? "";
            const p = providers.find(
              (p) =>
                p.shortName === providerName ||
                // FIX: Lägg till ?? '' för att garantera en sträng till .includes()
                providerName.includes(p.shortName.split(" ")[0] ?? ""),
            );

            const color: string = p?.color ?? "#6b7280";
            return color + "cc";
          }),
          borderRadius: 4,
        },
      ],
    },
    options: {
      plugins: { legend: { display: false } },
      scales: {
        y: { ticks: { color: "#94a3b8", font: { size: 10 } }, grid: { color: "#1e293b" } },
        x: {
          ticks: { color: "#94a3b8", font: { size: 9 }, maxRotation: 30 },
          grid: { display: false },
        },
      },
    },
  });
}

onMounted(async () => {
  if (!store.surveys.length) {
    store.surveys = await fetchSurveys();
  }
  setTimeout(drawRadarCharts, 100);
});

watch(
  () => benchmarks.results.length,
  () => {
    setTimeout(drawBenchChart, 100);
  },
);
</script>

<style scoped>
@import url("https://fonts.googleapis.com/css2?family=Cabinet+Grotesk:wght@400;500;700;800;900&family=Roboto+Mono:wght@400;500&display=swap");

.dashboard {
  font-family: "Cabinet Grotesk", sans-serif;
  max-width: 1300px;
  margin: 0 auto;
  color: #e2e8f0;
}

/* Hero */
.dash-hero {
  position: relative;
  overflow: hidden;
  border-radius: 20px;
  background: #050d1f;
  margin-bottom: 48px;
  padding: 60px;
}
.dh-bg {
  position: absolute;
  inset: 0;
}
.dh-grid {
  position: absolute;
  inset: 0;
  background-image:
    linear-gradient(rgba(255, 255, 255, 0.02) 1px, transparent 1px),
    linear-gradient(90deg, rgba(255, 255, 255, 0.02) 1px, transparent 1px);
  background-size: 50px 50px;
}
.dh-glow {
  position: absolute;
  width: 600px;
  height: 600px;
  background: radial-gradient(circle, rgba(99, 102, 241, 0.15) 0%, transparent 70%);
  top: -200px;
  right: -100px;
}
.dh-content {
  position: relative;
  z-index: 1;
}
.dh-tag {
  font-family: "Roboto Mono", monospace;
  font-size: 11px;
  text-transform: uppercase;
  letter-spacing: 2px;
  color: rgba(255, 255, 255, 0.4);
  margin-bottom: 16px;
}
.dash-hero h1 {
  font-size: 52px;
  font-weight: 900;
  letter-spacing: -2px;
  margin: 0 0 12px;
  color: #fff;
}
.dash-hero p {
  font-size: 18px;
  color: rgba(255, 255, 255, 0.5);
  margin: 0 0 36px;
}
.dh-meta {
  display: flex;
  gap: 16px;
  flex-wrap: wrap;
}
.meta-pill {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 12px;
  padding: 12px 20px;
  text-align: center;
}
.mp-val {
  display: block;
  font-size: 24px;
  font-weight: 800;
  color: #fff;
  font-family: "Roboto Mono", monospace;
}
.mp-lbl {
  font-size: 11px;
  color: rgba(255, 255, 255, 0.4);
  text-transform: uppercase;
  letter-spacing: 1px;
}

/* Sections */
.section {
  margin-bottom: 48px;
}
.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}
.section-title {
  font-size: 22px;
  font-weight: 800;
  margin: 0 0 16px;
}

/* Provider cards */
.provider-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
}
.provider-card {
  display: flex;
  flex-direction: column;
  background: #050d1f;
  border: 1px solid rgba(255, 255, 255, 0.06);
  border-top: 3px solid var(--c);
  border-radius: 14px;
  padding: 24px;
  text-decoration: none;
  color: #e2e8f0;
  transition: all 0.2s;
}
.provider-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 16px 40px rgba(0, 0, 0, 0.3);
  border-color: var(--c);
}
.pc-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}
.pc-icon {
  font-size: 26px;
}
.pc-track {
  font-size: 10px;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: var(--c);
  font-family: "Roboto Mono", monospace;
}
.provider-card h3 {
  font-size: 14px;
  font-weight: 700;
  margin: 0 0 8px;
  color: #fff;
}
.provider-card p {
  font-size: 12px;
  color: #64748b;
  line-height: 1.5;
  flex: 1;
  margin: 0 0 12px;
}
.pc-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 5px;
  margin-bottom: 14px;
}
.tag {
  font-size: 10px;
  padding: 2px 8px;
  background: color-mix(in srgb, var(--c) 10%, transparent);
  color: var(--c);
  border-radius: 100px;
  border: 1px solid color-mix(in srgb, var(--c) 25%, transparent);
}
.pc-scores {
  display: flex;
  flex-direction: column;
  gap: 6px;
  margin-bottom: 14px;
}
.pcs-row {
  display: flex;
  align-items: center;
  gap: 8px;
}
.pcs-l {
  font-size: 10px;
  color: #64748b;
  width: 60px;
  flex-shrink: 0;
}
.pcs-bar {
  flex: 1;
  height: 4px;
  background: #1e293b;
  border-radius: 2px;
  overflow: hidden;
}
.pcs-fill {
  height: 100%;
  border-radius: 2px;
}
.pc-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 12px;
  border-top: 1px solid rgba(255, 255, 255, 0.05);
}
.pc-cost {
  font-size: 12px;
  font-weight: 700;
}
.pc-cost.free {
  color: #22c55e;
}
.pc-cost.paid {
  color: #f59e0b;
}
.pc-cost.freemium {
  color: #a855f7;
}
.pc-arrow {
  color: #475569;
  transition: transform 0.15s;
}
.provider-card:hover .pc-arrow {
  transform: translateX(4px);
  color: var(--c);
}

/* Matrix */
.matrix-filters {
  display: flex;
  gap: 8px;
  margin-bottom: 16px;
  flex-wrap: wrap;
}
.mf-btn {
  font-size: 12px;
  padding: 6px 16px;
  border-radius: 100px;
  border: 1px solid #1e293b;
  background: transparent;
  color: #64748b;
  cursor: pointer;
  transition: all 0.15s;
  font-family: "Cabinet Grotesk", sans-serif;
}
.mf-btn:hover {
  border-color: #475569;
  color: #94a3b8;
}
.mf-btn.active {
  background: #0f172a;
  border-color: #475569;
  color: #e2e8f0;
}
.matrix-wrap {
  overflow-x: auto;
  border-radius: 12px;
  border: 1px solid #1e293b;
}
.matrix {
  width: 100%;
  border-collapse: collapse;
}
.matrix th {
  background: #0a1628;
  padding: 14px 16px;
  text-align: left;
  font-size: 13px;
  font-weight: 700;
  border-bottom: 2px solid #1e293b;
  white-space: nowrap;
}
.matrix th:first-child {
  color: #94a3b8;
}
.mh-inner {
  display: flex;
  align-items: center;
  gap: 6px;
}
.mh-icon {
}
.category-row .cat-header {
  background: #0a1628;
  padding: 8px 16px;
  font-size: 11px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: #94a3b8;
}
.data-row:hover td {
  background: rgba(255, 255, 255, 0.02);
}
.matrix td {
  padding: 10px 16px;
  border-bottom: 1px solid #0f172a;
  font-size: 12px;
}
.feature-name {
  color: #94a3b8;
  font-weight: 500;
}
.feature-col {
  width: 240px;
}
.feature-cell {
  text-align: center;
}
.check {
  color: #22c55e;
  font-size: 14px;
  font-weight: 700;
}
.cross {
  color: #ef4444;
  font-size: 14px;
}
.partial {
  font-size: 11px;
  color: #94a3b8;
}
.score-chip {
  font-size: 11px;
  font-weight: 700;
  padding: 2px 6px;
  border-radius: 4px;
  background: rgba(255, 255, 255, 0.05);
}

/* Radar section */
.radar-section {
  display: flex;
  gap: 24px;
  align-items: flex-start;
}
.radar-charts {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
  flex: 1;
}
.radar-wrap {
  background: #050d1f;
  border-radius: 10px;
  padding: 16px;
  border: 1px solid #1e293b;
}
.rw-title {
  font-size: 13px;
  font-weight: 700;
  margin-bottom: 8px;
}
.radar-legend {
  width: 160px;
  flex-shrink: 0;
  background: #050d1f;
  border: 1px solid #1e293b;
  border-radius: 10px;
  padding: 16px;
}
.rl-row {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 11px;
  color: #64748b;
  padding: 4px 0;
}
.rl-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: #475569;
  flex-shrink: 0;
}

/* Benchmark */
.bench-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}
.bench-chart-card {
  background: #050d1f;
  border: 1px solid #1e293b;
  border-radius: 12px;
  padding: 20px;
  grid-column: 1;
}
.bcc-header {
  font-size: 13px;
  font-weight: 700;
  margin-bottom: 12px;
  color: #94a3b8;
}
.bench-stats {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 10px;
  grid-column: 2;
  grid-row: 1 / 3;
  align-content: start;
}
.bench-table-card {
  background: #050d1f;
  border: 1px solid #1e293b;
  border-radius: 12px;
  overflow: hidden;
  grid-column: 1;
}
.btc-header {
  font-size: 13px;
  font-weight: 700;
  padding: 14px 16px;
  background: #0a1628;
  border-bottom: 1px solid #1e293b;
  color: #94a3b8;
}
.bench-table {
}
.bt-row {
  display: grid;
  grid-template-columns: 110px 60px 70px 70px 50px;
  padding: 8px 16px;
  border-bottom: 1px solid #0f172a;
  font-size: 12px;
}
.bt-row.header {
  font-size: 11px;
  color: #475569;
  background: #050d1f;
  font-weight: 700;
  text-transform: uppercase;
}
.bt-provider {
  color: #94a3b8;
  font-weight: 600;
}
.bt-format {
  color: #64748b;
  font-family: "Roboto Mono", monospace;
  font-size: 10px;
}
.bt-time.fast {
  color: #22c55e;
  font-weight: 700;
}
.bt-time.ok {
  color: #f59e0b;
  font-weight: 700;
}
.bt-time.slow {
  color: #ef4444;
  font-weight: 700;
}
.bt-size {
  color: #64748b;
  font-size: 11px;
}
.bt-q {
  color: #475569;
  font-size: 11px;
}
.bs-card {
  background: #050d1f;
  border: 1px solid #1e293b;
  border-radius: 10px;
  padding: 16px;
}
.bs-val {
  font-size: 22px;
  font-weight: 800;
  font-family: "Roboto Mono", monospace;
}
.bs-lbl {
  font-size: 11px;
  color: #64748b;
  margin-top: 4px;
}
.empty-bench {
  padding: 48px;
  text-align: center;
  background: #050d1f;
  border: 1px dashed #1e293b;
  border-radius: 12px;
}
.eb-icon {
  font-size: 40px;
  margin-bottom: 12px;
}
.eb-title {
  font-size: 16px;
  font-weight: 700;
  color: #e2e8f0;
  margin-bottom: 6px;
}
.eb-sub {
  font-size: 13px;
  color: #475569;
}
.clear-btn {
  font-size: 12px;
  padding: 6px 14px;
  border-radius: 6px;
  border: 1px solid #1e293b;
  background: transparent;
  color: #64748b;
  cursor: pointer;
}
.clear-btn:hover {
  background: #ef4444;
  color: white;
  border-color: #ef4444;
}

/* Decision guide */
.decision-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 16px;
}
.decision-card {
  background: #050d1f;
  border: 1px solid rgba(255, 255, 255, 0.05);
  border-left: 4px solid var(--dc);
  border-radius: 12px;
  padding: 20px;
}
.dc-icon {
  font-size: 24px;
  margin-bottom: 8px;
}
.dc-scenario {
  font-size: 13px;
  font-weight: 700;
  color: #e2e8f0;
  margin-bottom: 6px;
}
.dc-winner {
  font-size: 13px;
  font-weight: 700;
  color: var(--dc);
  margin-bottom: 6px;
}
.dc-reason {
  font-size: 12px;
  color: #64748b;
  line-height: 1.5;
}

/* Stress */
.stress-section {
  background: #050d1f;
  border: 1px solid #1e293b;
  border-radius: 16px;
  padding: 32px;
}
.stress-inner {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 24px;
  flex-wrap: wrap;
}
.stress-section h2 {
  font-size: 20px;
  font-weight: 800;
  margin: 0 0 8px;
}
.stress-section p {
  font-size: 13px;
  color: #64748b;
  margin: 0;
}
.stress-btns {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
}
.stress-btn {
  padding: 10px 20px;
  background: color-mix(in srgb, var(--c) 15%, transparent);
  color: var(--c);
  border: 1px solid color-mix(in srgb, var(--c) 30%, transparent);
  border-radius: 8px;
  text-decoration: none;
  font-weight: 600;
  font-size: 13px;
  transition: all 0.15s;
}
.stress-btn:hover {
  background: var(--c);
  color: white;
}
</style>
