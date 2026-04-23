<template>
  <div class="page">
    <div class="hero" style="--hc: #f05a28">
      <div class="hero-content">
        <router-link to="/" class="back">← Dashboard</router-link>
        <div class="eyebrow">Track 6 · Enterprise Document Processing</div>
        <h1>DevExpress</h1>
        <div class="tagline">Fullständig implementation kräver DevExpress Universal-licens</div>
        <div class="chips">
          <span class="chip info">ℹ Kräver betald licens</span>
          <span class="chip ok">✓ Hög PDF-kvalitet</span>
          <span class="chip ok">✓ Native Excel-formler</span>
          <span class="chip ok">✓ Redigerbara PPT-diagram</span>
        </div>
        <div class="stub-notice">
          <div class="sn-icon">🚧</div>
          <div>
            <strong>Stub-implementation</strong> — exportknappen visar vad som behövs för att aktivera.
            Arkitekturen och sidlayouten är klar. Lägg till NuGet-paketet och fyll i <code>DevExpressProvider.cs</code>.
          </div>
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
        provider="DevExpress"
        provider-color="#f05a28"
        :supports-html-template="false"
        :supports-charts="false"
        :is-stub="true"
      >
        <template #options>
          <div class="tc-label">Aktivering</div>
          <div class="activation-steps">
            <div class="step" v-for="(s, i) in activationSteps" :key="i">
              <span class="step-num">{{ i + 1 }}</span>
              <div class="step-content">
                <div class="step-title">{{ s.title }}</div>
                <code v-if="s.code" class="step-code">{{ s.code }}</code>
                <div v-if="s.note" class="step-note">{{ s.note }}</div>
              </div>
            </div>
          </div>
          <div class="tc-label" style="margin-top:16px">Vad DevExpress ger</div>
          <div class="dx-features">
            <div class="dx-feat" v-for="f in features" :key="f.t">
              <span class="df-icon">{{ f.icon }}</span>
              <div>
                <div class="df-title">{{ f.t }}</div>
                <div class="df-desc">{{ f.d }}</div>
              </div>
            </div>
          </div>
        </template>
      </ExportWorkspace>
    </section>
  </div>
</template>

<script setup lang="ts">
import ExportWorkspace from '../../components/ExportWorkspace.vue'

const heroStats = [
  { val: 'Betald',   label: 'Licens'   },
  { val: '< 80ms',   label: 'Cold start (estimerat)' },
  { val: '3',        label: 'Format'   },
]

const activationSteps = [
  {
    title: 'Skaffa DevExpress Universal-licens',
    note: 'https://www.devexpress.com/subscriptions/',
  },
  {
    title: 'Lägg till NuGet-källa',
    code: 'https://nuget.devexpress.com/api',
    note: 'Kräver DevExpress-inloggning',
  },
  {
    title: 'Installera paket',
    code: 'DevExpress.Document.Processor\nDevExpress.Pdf.Core\nDevExpress.Spreadsheet.Core',
  },
  {
    title: 'Registrera licens i Program.cs',
    code: 'DevExpress.Licensing.LicenseHelper.RegisterLicense("DIN-NYCKEL");',
  },
  {
    title: 'Implementera DevExpressProvider.cs',
    note: 'Ersätt NotImplementedException med din implementation. Stukturen är klar.',
  },
]

const features = [
  { icon: '📕', t: 'PDF med hög kvalitet', d: 'Native PDF-rendering utan Chromium, med fullt font- och layout-stöd.' },
  { icon: '📗', t: 'Excel med live-formler', d: '=AVERAGE()-formler, villkorsstyrd formatering, pivot-tabeller.' },
  { icon: '📘', t: 'Redigerbara PPT-diagram', d: 'OfficeChart-objekt som kan redigeras direkt i PowerPoint.' },
  { icon: '🔐', t: 'PDF-säkerhet', d: 'Lösenordsskydd, digital signatur, redaction.' },
]
</script>

<style scoped>
.page { max-width: 1300px; margin: 0 auto; color: #e2e8f0; }

.hero {
  background: linear-gradient(135deg, #1a0a06, #2d1005);
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
.chips { display: flex; flex-wrap: wrap; gap: 6px; margin-bottom: 14px; }
.chip { font-size: 10px; padding: 3px 10px; border-radius: 100px; border: 1px solid; font-weight: 500; }
.chip.ok   { color: #22c55e; border-color: rgba(34,197,94,.3); background: rgba(34,197,94,.06); }
.chip.info { color: #60a5fa; border-color: rgba(96,165,250,.3); background: rgba(96,165,250,.06); }

.stub-notice {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  background: rgba(245,158,11,.07);
  border: 1px solid rgba(245,158,11,.25);
  border-radius: 8px;
  padding: 12px 14px;
  font-size: 12px;
  color: rgba(255,255,255,.7);
  line-height: 1.5;
}
.sn-icon { font-size: 18px; flex-shrink: 0; }
.stub-notice strong { color: #f59e0b; }
.stub-notice code { background: rgba(245,158,11,.12); color: #fcd34d; padding: 1px 5px; border-radius: 3px; font-family: 'Courier New', monospace; font-size: 11px; }

.hero-meta { display: flex; flex-direction: column; gap: 12px; }
.hm-stat { text-align: center; background: rgba(255,255,255,.04); border: 1px solid rgba(255,255,255,.07); border-radius: 10px; padding: 14px 20px; }
.hm-val { display: block; font-size: 18px; font-weight: 800; color: var(--hc); font-variant-numeric: tabular-nums; }
.hm-lbl { font-size: 9px; color: rgba(255,255,255,.35); text-transform: uppercase; letter-spacing: 1px; margin-top: 3px; display: block; }

.section { margin-bottom: 36px; }

.tc-label {
  font-size: 10px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: var(--text-muted, #64748b);
  margin-bottom: 10px;
}

.activation-steps { display: flex; flex-direction: column; gap: 8px; }
.step {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  padding: 10px 12px;
  background: rgba(255,255,255,.02);
  border: 1px solid var(--border, #334155);
  border-radius: 8px;
}
.step-num {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background: rgba(240,90,40,.2);
  border: 1px solid rgba(240,90,40,.4);
  color: #f05a28;
  font-size: 10px;
  font-weight: 700;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  margin-top: 2px;
}
.step-title { font-size: 12px; font-weight: 600; color: var(--text, #f1f5f9); margin-bottom: 3px; }
.step-code {
  display: block;
  font-family: 'Courier New', monospace;
  font-size: 10px;
  color: #f05a28;
  background: rgba(240,90,40,.06);
  border: 1px solid rgba(240,90,40,.2);
  border-radius: 4px;
  padding: 5px 8px;
  margin-top: 4px;
  white-space: pre;
}
.step-note { font-size: 10px; color: var(--text-muted, #64748b); margin-top: 3px; }

.dx-features { display: flex; flex-direction: column; gap: 6px; }
.dx-feat {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  padding: 8px 10px;
  background: rgba(255,255,255,.02);
  border-radius: 6px;
}
.df-icon { font-size: 15px; flex-shrink: 0; margin-top: 1px; }
.df-title { font-size: 12px; font-weight: 600; color: var(--text, #f1f5f9); margin-bottom: 2px; }
.df-desc  { font-size: 11px; color: var(--text-muted, #64748b); }
</style>
