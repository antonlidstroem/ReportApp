<template>
  <div class="page">
    <div class="hero" style="--hc: #8b5cf6">
      <div class="hero-content">
        <router-link to="/" class="back">← Dashboard</router-link>
        <div class="eyebrow">Track 3 · Hybrid A</div>
        <h1>jsreport <span class="plus">+</span> Syncfusion</h1>
        <div class="tagline">jsreport hanterar PDF · Syncfusion hanterar Excel + PPT</div>
        <div class="chips">
          <span class="chip ok">★ Chart.js i PDF (jsreport)</span>
          <span class="chip ok">★ Live Excel-formler (Syncfusion)</span>
          <span class="chip ok">✓ Redigerbara PPT-diagram</span>
          <span class="chip warn">⚠ Två licenser + Node.js-process</span>
        </div>
      </div>
      <div class="hero-split">
        <div class="hs-half" style="--ec: #a855f7">
          <div class="hs-engine">🌐 jsreport</div>
          <div class="hs-formats"><span>📕 PDF</span></div>
          <div class="hs-note">Chromium · Chart.js · Handlebars</div>
        </div>
        <div class="hs-plus">+</div>
        <div class="hs-half" style="--ec: #14b8a6">
          <div class="hs-engine">💎 Syncfusion</div>
          <div class="hs-formats"><span>📗 Excel</span><span>📘 PPT</span></div>
          <div class="hs-note">Ren .NET · Live-formler · Redigerbara diagram</div>
        </div>
      </div>
    </div>

    <section class="section">
      <ExportWorkspace
        provider="js-sync"
        provider-color="#8b5cf6"
        :supports-html-template="true"
        :supports-charts="true"
      >
        <template #options>
          <!-- Two-column split: PDF side vs Excel side -->
          <div class="split-opts">
            <div class="so-col so-purple">
              <div class="so-engine">🌐 jsreport — PDF</div>
              <div class="opt-group">
                <label
                  v-for="opt in pdfOptions"
                  :key="opt.id"
                  :class="['opt-row', { active: opt.enabled }]"
                  @click="opt.enabled = !opt.enabled"
                >
                  <div class="opt-toggle" :class="{ on: opt.enabled }" style="--tc: #a855f7">
                    <div class="opt-thumb"></div>
                  </div>
                  <div class="opt-info">
                    <span class="opt-title">{{ opt.title }}</span>
                    <span class="opt-desc">{{ opt.desc }}</span>
                  </div>
                </label>
              </div>
            </div>

            <div class="so-divider">
              <div></div><span>Samma anrop</span><div></div>
            </div>

            <div class="so-col so-teal">
              <div class="so-engine">💎 Syncfusion — Excel/PPT</div>
              <div class="opt-group">
                <label
                  v-for="opt in excelOptions"
                  :key="opt.id"
                  :class="['opt-row', { active: opt.enabled }]"
                  @click="opt.enabled = !opt.enabled"
                >
                  <div class="opt-toggle" :class="{ on: opt.enabled }" style="--tc: #14b8a6">
                    <div class="opt-thumb"></div>
                  </div>
                  <div class="opt-info">
                    <span class="opt-title">{{ opt.title }}</span>
                    <span class="opt-desc">{{ opt.desc }}</span>
                  </div>
                </label>
              </div>
            </div>
          </div>
        </template>
      </ExportWorkspace>
    </section>

    <section class="section">
      <StressTestPanel provider="js-sync" provider-color="#8b5cf6" />
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import ExportWorkspace from '../../components/ExportWorkspace.vue'
import StressTestPanel from '../../components/StressTestPanel.vue'

const pdfOptions = ref([
  { id: 'chart',     title: 'Chart.js-diagram',    desc: 'Renderas av Chromium i PDF',      enabled: true  },
  { id: 'exec',      title: 'Executive-block',     desc: 'KPI-rad med OverallAverage',       enabled: true  },
  { id: 'highlight', title: 'Markera låga poäng',  desc: 'Röd rad när AverageValue < 3.0',  enabled: false },
])

const excelOptions = ref([
  { id: 'formulas',    title: 'Live =AVERAGE()-formler', desc: 'Räknar om i Excel', enabled: true  },
  { id: 'conditional', title: 'Villkorsstyrd formatering', desc: 'Röd/gul/grön baserat på poäng', enabled: true },
  { id: 'charts',      title: 'Redigerbara PPT-diagram', desc: 'OfficeChart-objekt, inte bilder', enabled: true },
])
</script>

<style scoped>
.page { max-width: 1300px; margin: 0 auto; color: #e2e8f0; }

.hero {
  background: linear-gradient(135deg, #080616, #100921);
  border: 1px solid color-mix(in srgb, var(--hc) 20%, transparent);
  border-radius: 16px; padding: 40px;
  display: grid; grid-template-columns: 1fr auto;
  gap: 32px; align-items: center; margin-bottom: 32px;
}
.back { display: block; color: rgba(255,255,255,.3); text-decoration: none; font-size: 12px; margin-bottom: 16px; }
.back:hover { color: var(--hc); }
.eyebrow { font-size: 10px; text-transform: uppercase; letter-spacing: 2px; color: var(--hc); margin-bottom: 10px; }
h1 { font-size: 36px; font-weight: 800; color: #fff; letter-spacing: -1.5px; margin: 0 0 6px; }
.plus { color: #8b5cf6; margin: 0 6px; }
.tagline { font-size: 13px; color: rgba(255,255,255,.5); margin-bottom: 16px; }
.chips { display: flex; flex-wrap: wrap; gap: 6px; }
.chip { font-size: 10px; padding: 3px 10px; border-radius: 100px; border: 1px solid; font-weight: 500; }
.chip.ok   { color: #8b5cf6; border-color: rgba(139,92,246,.3); background: rgba(139,92,246,.06); }
.chip.warn { color: #f59e0b; border-color: rgba(245,158,11,.3); background: rgba(245,158,11,.06); }

.hero-split { display: flex; flex-direction: column; align-items: stretch; gap: 0; min-width: 180px; border: 1px solid rgba(255,255,255,.06); border-radius: 10px; overflow: hidden; }
.hs-half { padding: 16px 14px; background: color-mix(in srgb, var(--ec) 5%, transparent); }
.hs-plus { text-align: center; padding: 6px; font-size: 16px; font-weight: 800; color: rgba(255,255,255,.15); background: rgba(255,255,255,.03); border-top: 1px solid rgba(255,255,255,.05); border-bottom: 1px solid rgba(255,255,255,.05); }
.hs-engine { font-size: 13px; font-weight: 700; color: white; margin-bottom: 6px; }
.hs-formats { display: flex; gap: 5px; margin-bottom: 5px; }
.hs-formats span { font-size: 10px; padding: 2px 8px; border-radius: 4px; background: rgba(255,255,255,.07); color: #94a3b8; border: 1px solid rgba(255,255,255,.08); }
.hs-note { font-size: 10px; color: rgba(255,255,255,.35); }

.section { margin-bottom: 36px; }

/* Split options */
.split-opts { display: grid; grid-template-columns: 1fr auto 1fr; gap: 12px; align-items: start; }
.so-col { display: flex; flex-direction: column; gap: 8px; }
.so-engine { font-size: 11px; font-weight: 700; color: var(--text-muted, #94a3b8); margin-bottom: 4px; }
.so-col.so-purple .so-engine { color: #a855f7; }
.so-col.so-teal   .so-engine { color: #14b8a6; }
.so-divider { display: flex; flex-direction: column; align-items: center; gap: 6px; padding: 16px 4px; }
.so-divider div { width: 1px; flex: 1; background: rgba(255,255,255,.07); }
.so-divider span { writing-mode: vertical-rl; font-size: 9px; color: var(--text-muted, #374151); text-transform: uppercase; letter-spacing: 1px; white-space: nowrap; }

.opt-group { display: flex; flex-direction: column; gap: 5px; }
.opt-row { display: flex; align-items: center; gap: 8px; padding: 7px 10px; border-radius: 7px; border: 1px solid var(--border, #334155); cursor: pointer; transition: all .12s; }
.opt-row:hover { background: rgba(255,255,255,.03); }
.opt-row.active { background: rgba(255,255,255,.03); }
.opt-toggle { width: 28px; height: 16px; border-radius: 100px; background: var(--border, #475569); position: relative; flex-shrink: 0; transition: background .2s; }
.opt-toggle.on { background: var(--tc, #8b5cf6); }
.opt-thumb { position: absolute; left: 2px; top: 1px; width: 14px; height: 14px; border-radius: 50%; background: white; transition: transform .2s; }
.opt-toggle.on .opt-thumb { transform: translateX(12px); }
.opt-info { flex: 1; }
.opt-title { display: block; font-size: 11px; font-weight: 600; color: var(--text, #f1f5f9); margin-bottom: 1px; }
.opt-desc  { font-size: 9px; color: var(--text-muted, #64748b); }
</style>
