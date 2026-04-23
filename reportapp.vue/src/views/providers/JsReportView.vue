<template>
  <div class="page">
    <!-- Hero -->
    <div class="hero" style="--hc: #a855f7">
      <div class="hero-content">
        <router-link to="/" class="back">← Dashboard</router-link>
        <div class="eyebrow">Track 1 · Web-Standard Engine</div>
        <h1>jsreport</h1>
        <div class="tagline">HTML + CSS + JavaScript → PDF via Chromium</div>
        <div class="chips">
          <span class="chip ok">★ Chart.js i PDF</span>
          <span class="chip ok">✓ Handlebars-mallar</span>
          <span class="chip ok">✓ Preview = PDF</span>
          <span class="chip warn">⚠ 2–5s cold start</span>
          <span class="chip bad">✗ Ingen native PPT</span>
        </div>
      </div>
      <div class="hero-meta">
        <div class="hm-stat" v-for="s in heroStats" :key="s.label">
          <span class="hm-val" :style="`color: var(--hc)`">{{ s.val }}</span>
          <span class="hm-lbl">{{ s.label }}</span>
        </div>
      </div>
    </div>

    <!-- Main workspace -->
    <section class="section">
      <ExportWorkspace
        provider="jsreport"
        provider-color="#a855f7"
        :supports-html-template="true"
        :supports-charts="true"
      >
        <template #options>
          <div class="tc-label">jsreport — PDF-alternativ</div>

          <div class="opt-group">
            <label
              v-for="opt in pdfOptions"
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
              <span v-if="opt.enabled" class="opt-active">✓ Aktiv</span>
            </label>
          </div>

          <div class="tc-label" style="margin-top: 16px">Handlebars-hjälpare</div>
          <div class="hb-note">
            Registreras server-side i jsreport. Möjliggör
            <code>{{'{{'}}#if (gt AverageValue 4){{'}}'}}</code> i mallen.
          </div>
          <div class="hb-helpers">
            <span v-for="h in helpers" :key="h" class="hb-chip">{{ h }}</span>
          </div>
        </template>
      </ExportWorkspace>
    </section>

    <section class="section">
      <StressTestPanel provider="jsreport" provider-color="#a855f7" />
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import ExportWorkspace from '../../components/ExportWorkspace.vue'
import StressTestPanel from '../../components/StressTestPanel.vue'

const heroStats = [
  { val: 'OSS/Pro', label: 'Licens'      },
  { val: '2–5s',    label: 'Cold start'  },
  { val: '3',       label: 'Format'      },
]

const pdfOptions = ref([
  { id: 'exec',      title: 'Executive-block',       desc: 'KPI-rad med OverallAverage och QuestionCount',  enabled: true  },
  { id: 'trend',     title: 'Chart.js-diagram',      desc: 'Renderas av Chromium, identisk med preview',    enabled: true  },
  { id: 'highlight', title: 'Markera låga poäng',    desc: 'Röd rad när AverageValue < 3.0',                enabled: false },
])

const helpers = ['gt', 'lt', 'gte', 'lte', 'eq', '#unless', '@index']
</script>

<style scoped>
.page { max-width: 1300px; margin: 0 auto; color: #e2e8f0; }

.hero {
  background: linear-gradient(135deg, #0d0018, #1a0033);
  border: 1px solid color-mix(in srgb, var(--hc) 20%, transparent);
  border-radius: 16px;
  padding: 40px;
  display: grid;
  grid-template-columns: 1fr auto;
  gap: 32px;
  align-items: center;
  margin-bottom: 32px;
}

.back { display: block; color: rgba(255,255,255,.3); text-decoration: none; font-size: 12px; margin-bottom: 16px; }
.back:hover { color: var(--hc); }
.eyebrow { font-size: 10px; text-transform: uppercase; letter-spacing: 2px; color: var(--hc); margin-bottom: 10px; }
h1 { font-size: 48px; font-weight: 800; color: #fff; letter-spacing: -2px; margin: 0 0 6px; }
.tagline { font-size: 13px; color: rgba(255,255,255,.5); margin-bottom: 16px; }

.chips { display: flex; flex-wrap: wrap; gap: 6px; }
.chip { font-size: 10px; padding: 3px 10px; border-radius: 100px; border: 1px solid; font-weight: 500; }
.chip.ok   { color: #a855f7; border-color: rgba(168,85,247,.3); background: rgba(168,85,247,.06); }
.chip.warn { color: #f59e0b; border-color: rgba(245,158,11,.3); background: rgba(245,158,11,.06); }
.chip.bad  { color: #ef4444; border-color: rgba(239,68,68,.3);  background: rgba(239,68,68,.06);  }

.hero-meta { display: flex; flex-direction: column; gap: 10px; }
.hm-stat { text-align: center; background: rgba(255,255,255,.04); border: 1px solid rgba(255,255,255,.07); border-radius: 10px; padding: 14px 20px; min-width: 100px; }
.hm-val  { display: block; font-size: 18px; font-weight: 800; font-variant-numeric: tabular-nums; }
.hm-lbl  { font-size: 10px; color: rgba(255,255,255,.35); text-transform: uppercase; letter-spacing: 1px; margin-top: 3px; display: block; }

.section { margin-bottom: 36px; }

/* Options slot styles */
.tc-label {
  font-size: 10px; font-weight: 700; text-transform: uppercase;
  letter-spacing: 1px; color: var(--text-muted, #64748b); margin-bottom: 10px;
}

.opt-group { display: flex; flex-direction: column; gap: 6px; }
.opt-row {
  display: flex; align-items: center; gap: 10px;
  padding: 9px 12px; border-radius: 8px;
  border: 1px solid var(--border, #334155);
  cursor: pointer; transition: all .12s;
}
.opt-row:hover   { background: rgba(255,255,255,.03); }
.opt-row.active  { border-color: rgba(168,85,247,.4); background: rgba(168,85,247,.05); }

.opt-toggle {
  width: 32px; height: 18px; border-radius: 100px;
  background: var(--border, #475569); position: relative;
  flex-shrink: 0; transition: background .2s;
}
.opt-toggle.on { background: #a855f7; }
.opt-thumb {
  position: absolute; left: 2px; top: 2px;
  width: 14px; height: 14px; border-radius: 50%;
  background: white; transition: transform .2s;
  box-shadow: 0 1px 3px rgba(0,0,0,.3);
}
.opt-toggle.on .opt-thumb { transform: translateX(14px); }

.opt-info   { flex: 1; }
.opt-title  { display: block; font-size: 12px; font-weight: 600; color: var(--text, #f1f5f9); margin-bottom: 1px; }
.opt-desc   { font-size: 10px; color: var(--text-muted, #64748b); }
.opt-active { font-size: 10px; color: #a855f7; font-weight: 700; }

.hb-note {
  font-size: 11px; color: var(--text-muted, #94a3b8);
  background: rgba(255,255,255,.02); border: 1px solid var(--border, #334155);
  border-radius: 6px; padding: 8px 10px; line-height: 1.5; margin-bottom: 8px;
}
.hb-note code { background: rgba(168,85,247,.12); color: #c4b5fd; padding: 1px 4px; border-radius: 3px; font-family: 'Courier New', monospace; font-size: 10px; }

.hb-helpers { display: flex; flex-wrap: wrap; gap: 5px; }
.hb-chip {
  font-size: 10px; padding: 2px 8px; border-radius: 4px;
  background: rgba(168,85,247,.1); border: 1px solid rgba(168,85,247,.25);
  color: #c4b5fd; font-family: 'Courier New', monospace;
}
</style>
