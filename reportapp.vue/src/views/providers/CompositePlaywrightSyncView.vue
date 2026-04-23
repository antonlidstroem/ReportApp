<template>
  <div class="page">
    <div class="hero" style="--hc: #10b981">
      <div class="hero-content">
        <router-link to="/" class="back">← Dashboard</router-link>
        <div class="eyebrow">Track 4 · Hybrid B</div>
        <h1>Playwright <span class="plus">+</span> Syncfusion</h1>
        <div class="tagline">Playwright PDF · Syncfusion Excel + PPT · ~200ms cold start</div>
        <div class="chips">
          <span class="chip ok">★ ~200ms cold start (vs 2–5s jsreport)</span>
          <span class="chip ok">✓ Live Excel-formler</span>
          <span class="chip ok">✓ Redigerbara PPT-diagram</span>
          <span class="chip warn">⚠ Token-ersättning — ingen Chart.js</span>
        </div>
      </div>
      <div class="hero-timing">
        <div class="ht-card green">
          <div class="ht-val">~200ms</div>
          <div class="ht-lbl">Playwright cold start</div>
        </div>
        <div class="ht-vs">vs</div>
        <div class="ht-card yellow">
          <div class="ht-val">2–5s</div>
          <div class="ht-lbl">jsreport cold start</div>
        </div>
      </div>
    </div>

    <section class="section">
      <ExportWorkspace
        provider="play-sync"
        provider-color="#10b981"
        :supports-html-template="true"
        :supports-charts="false"
      >
        <template #options>
          <div class="tc-label">Tillgängliga tokens i PDF-mallen</div>
          <div class="token-note">
            Playwright stöder enbart <strong>token-ersättning</strong> — inga Handlebars-loopar eller Chart.js.
            Klicka ett token för att kopiera till urklipp.
          </div>
          <div class="token-grid">
            <button
              v-for="t in tokens"
              :key="t.token"
              :class="['tok-btn', { copied: copiedToken === t.token }]"
              @click="copyToken(t.token)"
              :title="t.desc"
            >
              <code>{{ t.token }}</code>
              <span class="tok-desc">{{ t.desc }}</span>
              <span v-if="copiedToken === t.token" class="tok-copied">✓ kopierat</span>
            </button>
          </div>

          <div class="tc-label" style="margin-top: 16px">Syncfusion — Excel/PPT</div>
          <div class="opt-group">
            <label
              v-for="opt in excelOptions"
              :key="opt.id"
              :class="['opt-row', { active: opt.enabled }]"
              @click="opt.enabled = !opt.enabled"
            >
              <div class="opt-toggle" :class="{ on: opt.enabled }">
                <div class="opt-thumb"></div>
              </div>
              <div class="opt-info">
                <span class="opt-title">{{ opt.title }}</span>
                <span class="opt-desc">{{ opt.desc }}</span>
              </div>
            </label>
          </div>
        </template>
      </ExportWorkspace>
    </section>

    <section class="section">
      <StressTestPanel provider="play-sync" provider-color="#10b981" />
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import ExportWorkspace from '../../components/ExportWorkspace.vue'
import StressTestPanel from '../../components/StressTestPanel.vue'

const copiedToken = ref<string | null>(null)

const tokens = [
  { token: '{{SurveyTitle}}',      desc: 'Enkätens titel'         },
  { token: '{{CompanyName}}',      desc: 'Företagsnamn'           },
  { token: '{{GeneratedAt}}',      desc: 'Datum genererad'        },
  { token: '{{StartDate}}',        desc: 'Startdatum (filter)'    },
  { token: '{{EndDate}}',          desc: 'Slutdatum (filter)'     },
  { token: '{{QuestionCount}}',    desc: 'Antal frågor'           },
  { token: '{{TotalResponses}}',   desc: 'Totalt antal svar'      },
  { token: '{{OverallAverage}}',   desc: 'Totalsnitt (2 decimaler)'},
  { token: '{{CategoryCount}}',    desc: 'Antal kategorier'       },
  { token: '{{QuestionsTableHtml}}', desc: 'Förbyggd HTML-tabell' },
]

async function copyToken(token: string) {
  await navigator.clipboard.writeText(token)
  copiedToken.value = token
  setTimeout(() => { copiedToken.value = null }, 1500)
}

const excelOptions = ref([
  { id: 'formulas',    title: 'Live =AVERAGE()-formler',   desc: 'Räknar om i Excel', enabled: true  },
  { id: 'conditional', title: 'Villkorsstyrd formatering', desc: 'Röd/gul/grön baserat på poäng', enabled: true },
])
</script>

<style scoped>
.page { max-width: 1300px; margin: 0 auto; color: #e2e8f0; }

.hero {
  background: linear-gradient(135deg, #020f0a, #041e14);
  border: 1px solid color-mix(in srgb, var(--hc) 20%, transparent);
  border-radius: 16px; padding: 40px;
  display: grid; grid-template-columns: 1fr auto;
  gap: 32px; align-items: center; margin-bottom: 32px;
}
.back { display: block; color: rgba(255,255,255,.3); text-decoration: none; font-size: 12px; margin-bottom: 16px; }
.back:hover { color: var(--hc); }
.eyebrow { font-size: 10px; text-transform: uppercase; letter-spacing: 2px; color: var(--hc); margin-bottom: 10px; }
h1 { font-size: 36px; font-weight: 800; color: #fff; letter-spacing: -1.5px; margin: 0 0 6px; }
.plus { color: #10b981; margin: 0 6px; }
.tagline { font-size: 13px; color: rgba(255,255,255,.5); margin-bottom: 16px; }
.chips { display: flex; flex-wrap: wrap; gap: 6px; }
.chip { font-size: 10px; padding: 3px 10px; border-radius: 100px; border: 1px solid; font-weight: 500; }
.chip.ok   { color: #10b981; border-color: rgba(16,185,129,.3); background: rgba(16,185,129,.06); }
.chip.warn { color: #f59e0b; border-color: rgba(245,158,11,.3); background: rgba(245,158,11,.06); }

.hero-timing { display: flex; flex-direction: column; align-items: center; gap: 8px; }
.ht-card { text-align: center; border-radius: 10px; padding: 14px 20px; min-width: 120px; border: 1px solid; }
.ht-card.green { background: rgba(16,185,129,.06); border-color: rgba(16,185,129,.2); }
.ht-card.yellow { background: rgba(245,158,11,.04); border-color: rgba(245,158,11,.15); }
.ht-val { font-size: 22px; font-weight: 800; font-variant-numeric: tabular-nums; }
.ht-card.green .ht-val  { color: #10b981; }
.ht-card.yellow .ht-val { color: #f59e0b; }
.ht-lbl { font-size: 10px; color: rgba(255,255,255,.35); margin-top: 3px; }
.ht-vs { font-size: 14px; font-weight: 800; color: rgba(255,255,255,.12); }

.section { margin-bottom: 36px; }

.tc-label { font-size: 10px; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; color: var(--text-muted, #64748b); margin-bottom: 10px; }

.token-note { font-size: 11px; color: var(--text-muted, #94a3b8); background: rgba(255,255,255,.02); border: 1px solid var(--border, #334155); border-radius: 6px; padding: 8px 10px; line-height: 1.5; margin-bottom: 10px; }
.token-note strong { color: var(--text, #f1f5f9); }

.token-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 5px; margin-bottom: 4px; }
.tok-btn {
  display: flex; flex-direction: column; gap: 2px;
  padding: 7px 10px; border-radius: 6px;
  border: 1px solid rgba(16,185,129,.2); background: rgba(16,185,129,.04);
  cursor: pointer; text-align: left; transition: all .12s; font-family: inherit;
}
.tok-btn:hover { border-color: rgba(16,185,129,.4); background: rgba(16,185,129,.08); }
.tok-btn.copied { border-color: #10b981; background: rgba(16,185,129,.12); }
.tok-btn code { font-family: 'Courier New', monospace; font-size: 10px; color: #10b981; }
.tok-desc { font-size: 9px; color: var(--text-muted, #64748b); }
.tok-copied { font-size: 9px; color: #10b981; font-weight: 700; }

.opt-group { display: flex; flex-direction: column; gap: 5px; }
.opt-row { display: flex; align-items: center; gap: 8px; padding: 7px 10px; border-radius: 7px; border: 1px solid var(--border, #334155); cursor: pointer; transition: all .12s; }
.opt-row:hover { background: rgba(255,255,255,.03); }
.opt-row.active { border-color: rgba(16,185,129,.4); background: rgba(16,185,129,.05); }
.opt-toggle { width: 28px; height: 16px; border-radius: 100px; background: var(--border, #475569); position: relative; flex-shrink: 0; transition: background .2s; }
.opt-toggle.on { background: #10b981; }
.opt-thumb { position: absolute; left: 2px; top: 1px; width: 14px; height: 14px; border-radius: 50%; background: white; transition: transform .2s; }
.opt-toggle.on .opt-thumb { transform: translateX(12px); }
.opt-info { flex: 1; }
.opt-title { display: block; font-size: 11px; font-weight: 600; color: var(--text, #f1f5f9); margin-bottom: 1px; }
.opt-desc  { font-size: 9px; color: var(--text-muted, #64748b); }
</style>
