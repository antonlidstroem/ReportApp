<template>
  <div class="page">
    <div class="hero" style="--hc: #e4572e">
      <div class="hero-content">
        <router-link to="/" class="back">← Dashboard</router-link>
        <div class="eyebrow">Track 5 · Free Document Processing</div>
        <h1>Telerik</h1>
        <div class="tagline">Telerik Document Processing — free, no license key required</div>
        <div class="chips">
          <span class="chip ok">✓ Gratis NuGet</span>
          <span class="chip ok">✓ Ren .NET — ingen Chromium</span>
          <span class="chip ok">✓ PDF + Excel + PPT</span>
          <span class="chip warn">⚠ Ingen Chart.js i PDF</span>
          <span class="chip warn">⚠ Ingen HTML-mall</span>
        </div>
      </div>
      <div class="hero-meta">
        <div class="hm-stat" v-for="s in heroStats" :key="s.label">
          <span class="hm-val">{{ s.val }}</span>
          <span class="hm-lbl">{{ s.label }}</span>
        </div>
      </div>
    </div>

    <section class="section">
      <ExportWorkspace
        provider="Telerik"
        provider-color="#e4572e"
        :supports-html-template="false"
        :supports-charts="false"
      >
        <template #options>
          <div class="tc-label">Telerik — ingen extra konfiguration krävs</div>
          <div class="opt-note">
            Telerik Document Processing bygger dokument via .NET-objekt direkt.
            Ingen Chromium-process, ingen Node.js, ingen mall-syntax.
            Vad du ser i tabellen ovan är exakt vad som genereras.
          </div>
          <div class="pkg-list">
            <div class="pkg-label">NuGet-paket som används</div>
            <div class="pkg-item" v-for="p in packages" :key="p">
              <span class="pkg-icon">📦</span>
              <code>{{ p }}</code>
            </div>
          </div>
        </template>
      </ExportWorkspace>
    </section>

    <section class="section">
      <StressTestPanel provider="Telerik" provider-color="#e4572e" />
    </section>
  </div>
</template>

<script setup lang="ts">
import ExportWorkspace from '../../components/ExportWorkspace.vue'
import StressTestPanel from '../../components/StressTestPanel.vue'

const heroStats = [
  { val: 'Gratis',   label: 'Licens'     },
  { val: '< 50ms',   label: 'Cold start' },
  { val: '3',        label: 'Format'     },
]

const packages = [
  'Telerik.Documents.Core',
  'Telerik.Documents.Fixed',
  'Telerik.Documents.Spreadsheet',
  'Telerik.Documents.Presentation',
]
</script>

<style scoped>
.page { max-width: 1300px; margin: 0 auto; color: #e2e8f0; }

.hero {
  background: linear-gradient(135deg, #1a0a06, #2d1208);
  border: 1px solid color-mix(in srgb, var(--hc) 20%, transparent);
  border-radius: 16px;
  padding: 40px;
  display: grid;
  grid-template-columns: 1fr auto;
  gap: 32px;
  align-items: center;
  margin-bottom: 32px;
}

.back { display: block; color: rgba(255,255,255,.3); text-decoration: none; font-size: 12px; margin-bottom: 16px; transition: color .15s; }
.back:hover { color: var(--hc); }
.eyebrow { font-size: 10px; text-transform: uppercase; letter-spacing: 2px; color: var(--hc); margin-bottom: 10px; }
h1 { font-size: 42px; font-weight: 800; color: #fff; letter-spacing: -2px; margin: 0 0 6px; }
.tagline { font-size: 13px; color: rgba(255,255,255,.5); margin-bottom: 16px; }
.chips { display: flex; flex-wrap: wrap; gap: 6px; }
.chip { font-size: 10px; padding: 3px 10px; border-radius: 100px; border: 1px solid; font-weight: 500; }
.chip.ok   { color: #22c55e; border-color: rgba(34,197,94,.3); background: rgba(34,197,94,.06); }
.chip.warn { color: #f59e0b; border-color: rgba(245,158,11,.3); background: rgba(245,158,11,.06); }

.hero-meta { display: flex; flex-direction: column; gap: 12px; }
.hm-stat { text-align: center; background: rgba(255,255,255,.04); border: 1px solid rgba(255,255,255,.07); border-radius: 10px; padding: 14px 20px; }
.hm-val { display: block; font-size: 20px; font-weight: 800; color: var(--hc); font-variant-numeric: tabular-nums; }
.hm-lbl { font-size: 10px; color: rgba(255,255,255,.35); text-transform: uppercase; letter-spacing: 1px; margin-top: 3px; display: block; }

.section { margin-bottom: 36px; }

.tc-label {
  font-size: 10px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: var(--text-muted, #64748b);
  margin-bottom: 10px;
}

.opt-note {
  font-size: 12px;
  color: var(--text-muted, #94a3b8);
  background: rgba(255,255,255,.03);
  border: 1px solid var(--border, #334155);
  border-radius: 8px;
  padding: 12px 14px;
  line-height: 1.6;
  margin-bottom: 14px;
}

.pkg-list { display: flex; flex-direction: column; gap: 4px; }
.pkg-label { font-size: 10px; font-weight: 600; color: var(--text-muted, #64748b); text-transform: uppercase; letter-spacing: .5px; margin-bottom: 6px; }
.pkg-item { display: flex; align-items: center; gap: 8px; padding: 6px 10px; background: rgba(228,87,46,.06); border: 1px solid rgba(228,87,46,.2); border-radius: 6px; }
.pkg-icon { font-size: 13px; flex-shrink: 0; }
.pkg-item code { font-family: 'Courier New', monospace; font-size: 11px; color: #e4572e; }
</style>
