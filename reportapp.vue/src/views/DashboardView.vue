<template>
  <div class="dashboard">
    <header class="dash-hero">
      <div class="hero-content">
        <div class="hero-tag">Proof of Concept</div>
        <h1>Modern Reporting Engine</h1>
        <p>Jämför tre olika tekniska spår för PDF-, Excel- och PowerPoint-generering i .NET</p>
      </div>
      <div class="hero-meta">
        <span>{{ surveys.length }} enkäter</span>
        <span>{{ totalResponses.toLocaleString('sv') }} svar</span>
        <span>3 providers</span>
      </div>
    </header>

    <!-- Provider cards -->
    <section class="section">
      <h2 class="section-title">Tekniska spår</h2>
      <div class="provider-grid">
        <router-link
          v-for="p in providers"
          :key="p.route"
          :to="p.route"
          class="provider-card"
          :style="{ '--accent-local': p.color }"
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
          <div class="pc-footer">
            <span :class="['pc-cost', p.costClass]">{{ p.cost }}</span>
            <span class="pc-arrow">→</span>
          </div>
        </router-link>
      </div>
    </section>

    <!-- Feature matrix -->
    <section class="section">
      <h2 class="section-title">Funktionsmatris</h2>
      <div class="matrix-wrap">
        <table class="matrix">
          <thead>
            <tr>
              <th>Funktion</th>
              <th v-for="p in providers" :key="p.name">
                <span class="mh-icon">{{ p.icon }}</span>
                {{ p.shortName }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="row in featureMatrix" :key="row.feature">
              <td class="feature-name">{{ row.feature }}</td>
              <td v-for="(val, i) in row.values" :key="i" class="feature-cell">
                <span v-if="val === true"  class="check">✓</span>
                <span v-else-if="val === false" class="cross">✗</span>
                <span v-else class="partial">{{ val }}</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </section>

    <!-- Benchmark history -->
    <section class="section">
      <div class="section-header">
        <h2 class="section-title">Benchmark-historik</h2>
        <button @click="benchmarks.clear()" class="clear-btn" v-if="benchmarks.results.length">
          🗑 Rensa
        </button>
      </div>

      <div v-if="!benchmarks.results.length" class="empty-bench">
        Inga mätningar ännu. Gå till en provider-sida och exportera en rapport!
      </div>

      <div v-else>
        <!-- Bar chart -->
        <div class="chart-wrap">
          <div
            v-for="r in benchmarks.results.slice(0, 10)"
            :key="r.timestamp"
            class="bar-row"
          >
            <div class="bar-label">
              <span class="bar-provider">{{ r.provider }}</span>
              <span class="bar-format">{{ r.format.toUpperCase() }}</span>
              <span class="bar-survey">{{ r.surveyTitle.slice(0, 30) }}{{ r.surveyTitle.length > 30 ? '…' : '' }}</span>
            </div>
            <div class="bar-track">
              <div
                class="bar-fill"
                :style="{
                  width: barWidth(r.generationMs) + '%',
                  background: barColor(r.generationMs)
                }"
              >
                <span class="bar-ms">{{ r.generationMs }} ms</span>
              </div>
            </div>
            <div class="bar-size">{{ formatSize(r.fileSizeBytes) }}</div>
          </div>
        </div>
      </div>
    </section>

    <!-- Stress test launcher -->
    <section class="section stress-section">
      <div class="stress-content">
        <div>
          <h2>⚡ Stresstesta alla providers</h2>
          <p>Kör export mot 500-fråge-enkäten (100 000 svar) på alla providers samtidigt och jämför prestanda.</p>
        </div>
        <router-link to="/providers/quest" class="stress-btn">
          Börja stresstesta →
        </router-link>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useBenchmarkStore } from '../stores/benchmarks'
import { fetchSurveys } from '../composables/useApi'
import { useReportBuilderStore } from '../stores/reportBuilder'

const benchmarks = useBenchmarkStore()
const store = useReportBuilderStore()
const surveys = ref<{ id: number; title: string; questionCount: number }[]>([])

onMounted(async () => {
  if (!store.surveys.length) {
    store.surveys = await fetchSurveys()
  }
  surveys.value = store.surveys
})

const totalResponses = computed(() =>
  surveys.value.reduce((a, s) => a + (s.questionCount * 30), 0) // estimate
)

const providers = [
  {
    route: '/providers/quest',
    track: 2,
    icon: '🏗️',
    name: 'QuestPDF + ClosedXML + ShapeCrawler',
    shortName: 'QuestOSS',
    description: 'Best-of-breed open source. Kod-centrerad PDF via Fluent API, Excel via ClosedXML, PPT via ShapeCrawler.',
    tags: ['Open Source', 'Gratis', 'Kod-first'],
    cost: 'Gratis (Community)',
    costClass: 'free',
    color: '#22c55e'
  },
  {
    route: '/providers/iron',
    track: 1,
    icon: '⚙️',
    name: 'IronSuite (PDF + XL + PPT)',
    shortName: 'IronSuite',
    description: 'Enterprise-paket från en leverantör. HTML-till-PDF, Excel och PPT med rikt API-stöd.',
    tags: ['Enterprise', 'HTML→PDF', 'Enhetlig'],
    cost: 'Betallicens',
    costClass: 'paid',
    color: '#3b82f6'
  },
  {
    route: '/providers/jsreport',
    track: 3,
    icon: '🌐',
    name: 'jsreport (Web-Standard Engine)',
    shortName: 'jsreport',
    description: 'Server-side rendering med Chromium. HTML/CSS/Chart.js-rapporter identiska med webbvyn.',
    tags: ['Chromium', 'HTML/CSS', 'Chart.js'],
    cost: 'Gratis + Enterprise',
    costClass: 'freemium',
    color: '#a855f7'
  },
  {
    route: '/providers/syncfusion',
    track: 4,
    icon: '💎',
    name: 'Syncfusion Essential Studio',
    shortName: 'Syncfusion',
    description: 'Mogen enterprise-lösning. Kraftfulla motorer för XlsIO och Presentation. Använder objektmodeller för att bygga dokument.',
    tags: ['Enterprise', 'Native Office', 'Hög prestanda'],
    cost: 'Betallicens',
    costClass: 'paid',
    color: '#007bff'
  }
]

const featureMatrix = [
  { feature: 'PDF-generering',       values: [true, true, true] },
  { feature: 'Excel-generering',     values: [true, true, '⚠ Begränsat'] },
  { feature: 'PPT-generering',       values: ['⚠ Basic', true, false] },
  { feature: 'HTML-mall → PDF',      values: [false, true, true] },
  { feature: 'Redigerbara grafer',   values: [false, true, false] },
  { feature: 'Chart.js i rapport',   values: [false, false, true] },
  { feature: 'Vattenstämpel',        values: [false, true, true] },
  { feature: 'Open source',          values: [true, false, 'Delvis'] },
  { feature: 'Ingen Chromium-dep.',  values: [true, true, false] },
  { feature: 'Mallsystem',           values: ['Presets', 'HTML', 'Handlebars'] },
]

function barWidth(ms: number) {
  const max = Math.max(...benchmarks.results.slice(0, 10).map(r => r.generationMs), 1)
  return Math.max(4, (ms / max) * 100)
}

function barColor(ms: number) {
  if (ms < 500)  return '#22c55e'
  if (ms < 2000) return '#f59e0b'
  return '#ef4444'
}

function formatSize(bytes: number) {
  if (!bytes) return '—'
  if (bytes > 1_000_000) return `${(bytes / 1_000_000).toFixed(1)} MB`
  return `${(bytes / 1024).toFixed(0)} KB`
}
</script>

<style scoped>
.dashboard { max-width: 1200px; margin: 0 auto; }

.dash-hero {
  background: linear-gradient(135deg, #0f172a 0%, #1e3a5f 100%);
  border-radius: 16px;
  padding: 48px;
  color: white;
  margin-bottom: 40px;
  position: relative;
  overflow: hidden;
}
.dash-hero::before {
  content: '';
  position: absolute;
  inset: 0;
  background: url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23ffffff' fill-opacity='0.03'%3E%3Cpath d='M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E");
}

.hero-tag {
  font-size: 11px;
  text-transform: uppercase;
  letter-spacing: 2px;
  color: rgba(255,255,255,0.5);
  margin-bottom: 12px;
}
.dash-hero h1 { font-size: 36px; font-weight: 700; margin: 0 0 12px; }
.dash-hero p  { font-size: 16px; color: rgba(255,255,255,0.7); margin: 0; max-width: 560px; }

.hero-meta {
  display: flex;
  gap: 24px;
  margin-top: 32px;
}
.hero-meta span {
  font-size: 13px;
  color: rgba(255,255,255,0.6);
  background: rgba(255,255,255,0.08);
  padding: 6px 16px;
  border-radius: 100px;
  border: 1px solid rgba(255,255,255,0.12);
}

.section { margin-bottom: 40px; }
.section-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.section-title { font-size: 20px; font-weight: 700; margin: 0 0 16px; color: var(--text); }

.provider-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 16px;
}

.provider-card {
  display: flex;
  flex-direction: column;
  background: var(--surface);
  border: 2px solid var(--border);
  border-radius: 12px;
  padding: 24px;
  text-decoration: none;
  color: var(--text);
  transition: all 0.2s;
}
.provider-card:hover {
  border-color: var(--accent-local, var(--accent));
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(0,0,0,0.08);
}

.pc-top { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px; }
.pc-icon { font-size: 28px; }
.pc-track { font-size: 10px; text-transform: uppercase; letter-spacing: 1px; color: var(--text-muted); }

.provider-card h3 { font-size: 15px; font-weight: 700; margin: 0 0 8px; }
.provider-card p  { font-size: 13px; color: var(--text-muted); line-height: 1.5; flex: 1; margin: 0 0 12px; }

.pc-tags { display: flex; flex-wrap: wrap; gap: 6px; margin-bottom: 16px; }
.tag {
  font-size: 11px;
  padding: 3px 8px;
  background: color-mix(in srgb, var(--accent-local, var(--accent)) 10%, transparent);
  color: var(--accent-local, var(--accent));
  border-radius: 100px;
  border: 1px solid color-mix(in srgb, var(--accent-local, var(--accent)) 30%, transparent);
}

.pc-footer { display: flex; justify-content: space-between; align-items: center; }
.pc-cost { font-size: 12px; font-weight: 600; }
.pc-cost.free { color: #22c55e; }
.pc-cost.paid { color: #3b82f6; }
.pc-cost.freemium { color: #a855f7; }
.pc-arrow { color: var(--text-muted); transition: transform 0.15s; }
.provider-card:hover .pc-arrow { transform: translateX(4px); color: var(--accent-local, var(--accent)); }

/* Matrix */
.matrix-wrap { overflow-x: auto; }
.matrix { width: 100%; border-collapse: collapse; font-size: 13px; }
.matrix th {
  background: var(--surface-2);
  padding: 12px 16px;
  text-align: left;
  font-size: 12px;
  font-weight: 600;
  border-bottom: 2px solid var(--border);
  white-space: nowrap;
}
.mh-icon { margin-right: 4px; }
.matrix td { padding: 10px 16px; border-bottom: 1px solid var(--border); }
.feature-name { font-weight: 500; color: var(--text); }
.feature-cell { text-align: center; }
.check  { color: #22c55e; font-size: 16px; font-weight: 700; }
.cross  { color: #ef4444; font-size: 16px; }
.partial { font-size: 11px; color: var(--text-muted); }

/* Benchmark chart */
.empty-bench {
  padding: 32px;
  text-align: center;
  color: var(--text-muted);
  background: var(--surface);
  border: 1px dashed var(--border);
  border-radius: 12px;
  font-size: 14px;
}

.chart-wrap { display: flex; flex-direction: column; gap: 10px; }
.bar-row { display: flex; align-items: center; gap: 12px; }

.bar-label {
  min-width: 220px;
  display: flex;
  flex-direction: column;
  gap: 2px;
}
.bar-provider { font-size: 12px; font-weight: 600; }
.bar-format   { font-size: 10px; text-transform: uppercase; color: var(--accent); letter-spacing: 0.5px; }
.bar-survey   { font-size: 10px; color: var(--text-muted); }

.bar-track {
  flex: 1;
  height: 28px;
  background: var(--surface-2);
  border-radius: 6px;
  overflow: hidden;
}
.bar-fill {
  height: 100%;
  border-radius: 6px;
  display: flex;
  align-items: center;
  padding: 0 8px;
  min-width: 40px;
  transition: width 0.4s ease;
}
.bar-ms { font-size: 11px; font-weight: 700; color: white; white-space: nowrap; }
.bar-size { min-width: 70px; text-align: right; font-size: 11px; color: var(--text-muted); }

.clear-btn {
  font-size: 12px;
  padding: 6px 12px;
  border-radius: 6px;
  border: 1px solid var(--border);
  background: transparent;
  color: var(--text-muted);
  cursor: pointer;
}
.clear-btn:hover { background: #ef4444; color: white; border-color: #ef4444; }

/* Stress */
.stress-section {
  background: linear-gradient(135deg, #1a0533, #2d1b69);
  border-radius: 16px;
  padding: 32px;
  color: white;
}
.stress-content { display: flex; justify-content: space-between; align-items: center; gap: 24px; }
.stress-section h2 { font-size: 22px; margin: 0 0 8px; }
.stress-section p  { font-size: 14px; color: rgba(255,255,255,0.7); margin: 0; }
.stress-btn {
  white-space: nowrap;
  padding: 14px 28px;
  background: #a855f7;
  color: white;
  border-radius: 8px;
  text-decoration: none;
  font-weight: 600;
  font-size: 14px;
  transition: all 0.15s;
}
.stress-btn:hover { background: #9333ea; transform: translateY(-1px); }
</style>
