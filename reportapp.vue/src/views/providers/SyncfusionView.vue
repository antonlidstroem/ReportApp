<template>
  <div class="page">
    <div class="hero" style="--hc: #14b8a6">
      <div class="hero-content">
        <router-link to="/" class="back">← Dashboard</router-link>
        <div class="eyebrow">Track 2 · Mature Enterprise Platform</div>
        <h1>Syncfusion</h1>
        <div class="tagline">Native Office engine — live formler, redigerbara diagram</div>
        <div class="chips">
          <span class="chip ok">★ Live =AVERAGE()-formler</span>
          <span class="chip ok">✓ Redigerbara PPT-diagram</span>
          <span class="chip ok">✓ PDF/UA</span>
          <span class="chip ok">✓ Community-licens gratis</span>
          <span class="chip warn">⚠ Ingen HTML → PDF</span>
        </div>
      </div>
      <div class="hero-meta">
        <div class="hm-stat" v-for="s in heroStats" :key="s.label">
          <span class="hm-val" :style="`color: var(--hc)`">{{ s.val }}</span>
          <span class="hm-lbl">{{ s.label }}</span>
        </div>
      </div>
    </div>

    <section class="section">
      <ExportWorkspace
        provider="Syncfusion"
        provider-color="#14b8a6"
        :supports-html-template="false"
        :supports-charts="false"
      >
        <template #options>
          <!-- Formula sandbox -->
          <div class="tc-label">Live Excel-formel-simulator</div>
          <div class="formula-sandbox">
            <div class="fs-hint">Redigera värden — =AVERAGE()-formeln räknar om precis som i den exporterade .xlsx-filen</div>
            <div class="fs-table">
              <div class="fs-head">
                <span>Kategori</span><span>Q1</span><span>Q2</span><span>Q3</span>
                <span class="fs-formula">=AVERAGE()</span>
              </div>
              <div
                v-for="(row, i) in fRows"
                :key="i"
                class="fs-row"
                :class="{ selected: selectedRow === i }"
                @click="selectedRow = i"
              >
                <span class="fs-cat">{{ row.label }}</span>
                <input v-for="k in ['q1','q2','q3']" :key="k"
                  v-model.number="(row as any)[k]"
                  type="number" min="1" max="5" step="0.1"
                  class="fs-input"
                />
                <span class="fs-avg" :style="`color:${avgColor(rowAvg(row))}`">
                  {{ rowAvg(row).toFixed(2) }}
                </span>
              </div>
            </div>
            <div class="fs-bar">
              <span>Formel för markerad rad:</span>
              <code>=AVERAGE(B{{ selectedRow + 2 }}:D{{ selectedRow + 2 }})</code>
              <span class="fs-total">Totalsnitt: <strong :style="`color:${avgColor(overallAvg)}`">{{ overallAvg.toFixed(2) }}</strong></span>
            </div>
          </div>

          <!-- Enterprise toggles -->
          <div class="tc-label" style="margin-top: 16px">Enterprise-funktioner</div>
          <div class="opt-group">
            <label
              v-for="opt in enterpriseOpts"
              :key="opt.id"
              :class="['opt-row', { active: opt.enabled }]"
              @click="opt.enabled = !opt.enabled"
            >
              <div class="opt-toggle" :class="{ on: opt.enabled }">
                <div class="opt-thumb"></div>
              </div>
              <span class="opt-icon">{{ opt.icon }}</span>
              <div class="opt-info">
                <span class="opt-title">{{ opt.title }}</span>
                <span class="opt-desc">{{ opt.desc }}</span>
              </div>
              <span v-if="opt.enabled" class="opt-active">✓</span>
            </label>
          </div>

          <!-- Native chart picker -->
          <div class="tc-label" style="margin-top: 16px">PPT-diagramtyp (redigerbar i PowerPoint)</div>
          <div class="chart-type-grid">
            <button
              v-for="ct in chartTypes"
              :key="ct.id"
              :class="['ct-btn', { active: selectedChartType === ct.id }]"
              @click="selectedChartType = ct.id"
            >
              <span>{{ ct.icon }}</span>
              <span>{{ ct.label }}</span>
              <code>{{ ct.enum }}</code>
            </button>
          </div>
        </template>
      </ExportWorkspace>
    </section>

    <section class="section">
      <StressTestPanel provider="Syncfusion" provider-color="#14b8a6" />
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import ExportWorkspace from '../../components/ExportWorkspace.vue'
import StressTestPanel from '../../components/StressTestPanel.vue'

const heroStats = [
  { val: 'Community gratis', label: 'Licens'     },
  { val: '< 100ms',          label: 'Cold start' },
  { val: '3',                label: 'Format'     },
]

// Formula sandbox
const selectedRow = ref(0)
const fRows = ref([
  { label: 'Ledarskap',    q1: 4.2, q2: 3.8, q3: 4.5 },
  { label: 'Psykosocialt', q1: 3.1, q2: 3.4, q3: 2.9 },
  { label: 'Hälsa',        q1: 3.8, q2: 4.0, q3: 3.6 },
  { label: 'Säkerhet',     q1: 4.5, q2: 4.2, q3: 4.8 },
])
const rowAvg = (r: { q1: number; q2: number; q3: number }) => (r.q1 + r.q2 + r.q3) / 3
const overallAvg = computed(() =>
  fRows.value.reduce((s, r) => s + r.q1 + r.q2 + r.q3, 0) / (fRows.value.length * 3)
)
const avgColor = (v: number) => v >= 4 ? '#22c55e' : v >= 3 ? '#f59e0b' : '#ef4444'

// Enterprise options
const enterpriseOpts = ref([
  { id: 'formulas',      icon: '🧮', title: 'Live =AVERAGE()-formler',    desc: 'Räknar om i Excel när användaren öppnar filen',     enabled: true  },
  { id: 'conditional',   icon: '🎨', title: 'Villkorsstyrd formatering',  desc: 'Röd/gul/grön cell baserat på poäng',                enabled: true  },
  { id: 'encrypt',       icon: '🔐', title: 'AES-256-kryptering (PDF)',   desc: 'Lösenordsskyddad rapport — lösenord: 1234',         enabled: false },
  { id: 'accessibility', icon: '♿', title: 'PDF/UA tillgänglighet',      desc: 'Taggad PDF, WCAG 2.1 AA, skärmläsarkompatibel',    enabled: false },
  { id: 'watermark',     icon: '💧', title: 'Vattenstämpel',             desc: 'KONFIDENTIELLT diagonalt på alla sidor',            enabled: false },
])

// PPT chart type
const selectedChartType = ref('bar')
const chartTypes = [
  { id: 'bar',      icon: '📊', label: 'Stapel',   enum: 'Column_Clustered' },
  { id: 'line',     icon: '📈', label: 'Linje',    enum: 'Line'             },
  { id: 'radar',    icon: '🎯', label: 'Radar',    enum: 'Radar'            },
  { id: 'doughnut', icon: '🍩', label: 'Munk',     enum: 'Doughnut'         },
]
</script>

<style scoped>
.page { max-width: 1300px; margin: 0 auto; color: #e2e8f0; }

.hero {
  background: linear-gradient(135deg, #021314, #041e1c);
  border: 1px solid color-mix(in srgb, var(--hc) 20%, transparent);
  border-radius: 16px; padding: 40px;
  display: grid; grid-template-columns: 1fr auto;
  gap: 32px; align-items: center; margin-bottom: 32px;
}
.back  { display: block; color: rgba(255,255,255,.3); text-decoration: none; font-size: 12px; margin-bottom: 16px; }
.back:hover { color: var(--hc); }
.eyebrow { font-size: 10px; text-transform: uppercase; letter-spacing: 2px; color: var(--hc); margin-bottom: 10px; }
h1 { font-size: 48px; font-weight: 800; color: #fff; letter-spacing: -2px; margin: 0 0 6px; }
.tagline { font-size: 13px; color: rgba(255,255,255,.5); margin-bottom: 16px; }
.chips { display: flex; flex-wrap: wrap; gap: 6px; }
.chip { font-size: 10px; padding: 3px 10px; border-radius: 100px; border: 1px solid; font-weight: 500; }
.chip.ok   { color: #14b8a6; border-color: rgba(20,184,166,.3); background: rgba(20,184,166,.06); }
.chip.warn { color: #f59e0b; border-color: rgba(245,158,11,.3); background: rgba(245,158,11,.06); }
.hero-meta { display: flex; flex-direction: column; gap: 10px; }
.hm-stat { text-align: center; background: rgba(255,255,255,.04); border: 1px solid rgba(255,255,255,.07); border-radius: 10px; padding: 12px 16px; min-width: 120px; }
.hm-val  { display: block; font-size: 16px; font-weight: 800; }
.hm-lbl  { font-size: 10px; color: rgba(255,255,255,.35); text-transform: uppercase; letter-spacing: 1px; margin-top: 3px; display: block; }

.section { margin-bottom: 36px; }

.tc-label { font-size: 10px; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; color: var(--text-muted, #64748b); margin-bottom: 10px; }

/* Formula sandbox */
.formula-sandbox { background: rgba(20,184,166,.04); border: 1px solid rgba(20,184,166,.2); border-radius: 10px; overflow: hidden; margin-bottom: 4px; }
.fs-hint { font-size: 10px; color: var(--text-muted, #64748b); padding: 7px 12px; background: rgba(20,184,166,.07); border-bottom: 1px solid rgba(20,184,166,.15); }
.fs-table { padding: 8px; }
.fs-head, .fs-row { display: grid; grid-template-columns: 90px 1fr 1fr 1fr 80px; gap: 4px; align-items: center; }
.fs-head { font-size: 9px; font-weight: 700; text-transform: uppercase; color: var(--text-muted, #64748b); padding: 4px 6px; }
.fs-formula { color: #14b8a6; }
.fs-row { padding: 4px 6px; border-radius: 5px; cursor: pointer; transition: background .1s; }
.fs-row:hover, .fs-row.selected { background: rgba(20,184,166,.08); }
.fs-cat { font-size: 11px; color: var(--text, #f1f5f9); font-weight: 500; }
.fs-input {
  width: 100%; background: rgba(0,0,0,.2); border: 1px solid rgba(20,184,166,.2);
  border-radius: 4px; color: var(--text, #f1f5f9); font-size: 11px; padding: 3px 6px;
  font-family: 'Courier New', monospace; text-align: center;
}
.fs-input:focus { outline: none; border-color: #14b8a6; }
.fs-avg { font-size: 13px; font-weight: 700; text-align: right; }
.fs-bar { display: flex; align-items: center; gap: 8px; padding: 8px 12px; background: rgba(0,0,0,.2); border-top: 1px solid rgba(20,184,166,.1); font-size: 10px; color: var(--text-muted, #64748b); }
.fs-bar code { background: rgba(20,184,166,.1); color: #14b8a6; padding: 1px 5px; border-radius: 3px; font-family: 'Courier New', monospace; font-size: 10px; }
.fs-total { margin-left: auto; font-size: 11px; }

/* Options shared */
.opt-group { display: flex; flex-direction: column; gap: 5px; }
.opt-row { display: flex; align-items: center; gap: 10px; padding: 8px 12px; border-radius: 8px; border: 1px solid var(--border, #334155); cursor: pointer; transition: all .12s; }
.opt-row:hover { background: rgba(255,255,255,.03); }
.opt-row.active { border-color: rgba(20,184,166,.4); background: rgba(20,184,166,.05); }
.opt-toggle { width: 30px; height: 17px; border-radius: 100px; background: var(--border, #475569); position: relative; flex-shrink: 0; transition: background .2s; }
.opt-toggle.on { background: #14b8a6; }
.opt-thumb { position: absolute; left: 2px; top: 1.5px; width: 14px; height: 14px; border-radius: 50%; background: white; transition: transform .2s; }
.opt-toggle.on .opt-thumb { transform: translateX(13px); }
.opt-icon { font-size: 14px; flex-shrink: 0; }
.opt-info { flex: 1; }
.opt-title { display: block; font-size: 11px; font-weight: 600; color: var(--text, #f1f5f9); margin-bottom: 1px; }
.opt-desc  { font-size: 10px; color: var(--text-muted, #64748b); }
.opt-active { font-size: 10px; color: #14b8a6; font-weight: 700; }

/* Chart type grid */
.chart-type-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 6px; }
.ct-btn {
  display: flex; flex-direction: column; align-items: center; gap: 3px;
  padding: 9px 4px; border-radius: 7px; border: 1px solid var(--border, #475569);
  background: transparent; cursor: pointer; color: var(--text-muted, #94a3b8);
  font-family: inherit; transition: all .12s; font-size: 11px;
}
.ct-btn:hover { border-color: #14b8a6; color: var(--text, #f1f5f9); }
.ct-btn.active { border-color: #14b8a6; background: rgba(20,184,166,.1); color: #14b8a6; }
.ct-btn code { font-family: 'Courier New', monospace; font-size: 8px; color: inherit; opacity: 0.7; }
</style>
