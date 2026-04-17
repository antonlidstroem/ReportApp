<template>
  <div class="page">
    <div class="hero">
      <div class="hero-glow"></div>
      <div class="hero-content">
        <router-link to="/" class="back">← Dashboard</router-link>
        <div class="eyebrow">Track 3 · Hybrid Solution A</div>
        <h1>jsreport <span class="plus">+</span> Syncfusion</h1>
        <div class="tagline">Best of both. No compromise.</div>
        <p>PDF gets jsreport — designer HTML/CSS templates, Chart.js, Handlebars logic. Excel and PowerPoint get Syncfusion — live formulas, editable charts. One C# interface, two specialized engines.</p>
        <div class="verdicts">
          <div class="verdict ok">✓ Best for: Teams needing designer PDF templates AND Excel formulas</div>
          <div class="verdict warn">⚠ Trade-off: Two licenses + one Node.js process to maintain</div>
        </div>
        <div class="badges">
          <span class="b purple">★ Chart.js PDF via jsreport</span>
          <span class="b teal">★ Native Excel formulas</span>
          <span class="b teal">✓ Editable PPT charts</span>
          <span class="b yellow">⚠ Most complex setup</span>
        </div>
      </div>
      <div class="hero-split">
        <div class="hs-half hs-purple">
          <div class="hs-engine">🌐 jsreport</div>
          <div class="hs-fmts"><div class="hs-fmt">📕 PDF</div></div>
          <p>Chromium renders your HTML. Chart.js runs. PDF is captured.</p>
        </div>
        <div class="hs-plus">+</div>
        <div class="hs-half hs-teal">
          <div class="hs-engine">💎 Syncfusion</div>
          <div class="hs-fmts"><div class="hs-fmt">📗 Excel</div><div class="hs-fmt">📘 PPT</div></div>
          <p>Native .NET. No Chromium. Live formulas. Editable charts.</p>
        </div>
      </div>
    </div>

    <section class="section">
      <div class="eyebrow-sm">ARCHITECTURE</div>
      <h2>Each format routes to a different engine</h2>
      <p class="sub">Click a format below to watch the request flow. The C# controller switches engine at runtime — the frontend calls the same endpoint regardless.</p>
      <RoutingVisualizer pdf-engine="jsreport" />
    </section>

    <section class="section">
      <div class="eyebrow-sm">WHAT EACH ENGINE CONTRIBUTES</div>
      <h2>jsreport handles PDF · Syncfusion handles Office</h2>
      <div class="engine-compare">
        <div class="ec-col ec-purple">
          <div class="ec-hdr"><span>🌐</span><span>jsreport — PDF only</span></div>
          <div v-for="f in jsFeatures" :key="f" class="ec-feat"><span class="ec-check">✓</span>{{ f }}</div>
        </div>
        <div class="ec-vs">vs</div>
        <div class="ec-col ec-teal">
          <div class="ec-hdr"><span>💎</span><span>Syncfusion — Excel + PPT</span></div>
          <div v-for="f in sfFeatures" :key="f" class="ec-feat"><span class="ec-check teal">✓</span>{{ f }}</div>
        </div>
      </div>
    </section>

    <section class="section">
      <div class="eyebrow-sm">PDF SIDE — jsreport LIVE PREVIEW</div>
      <h2>The PDF template — Chart.js rendered by Chromium</h2>
      <p class="sub">This is what the PDF export generates. The "Full Report" template includes a Chart.js bar chart. The "Executive" template uses a radar chart — both rendered by Chromium.</p>
      <LiveReportPreview provider="jsreport" :show-sliders="true" />
    </section>

    <section class="section">
      <div class="eyebrow-sm">DUAL ENGINE CONFIGURATION</div>
      <h2>Configure PDF template + Excel features at the same time</h2>
      <p class="sub">This is the dual-engine nature in action. Left panel configures jsreport (PDF). Right panel configures Syncfusion (Excel/PPT). Both apply on export.</p>
      <div class="dual-config">
        <div class="dc-half dc-purple">
          <div class="dc-engine-lbl"><span class="dot purple"></span>jsreport — PDF Options</div>
          <div class="dc-opts">
            <label v-for="o in pdfOpts" :key="o.id" class="dc-opt">
              <input type="checkbox" v-model="o.enabled" />
              <div><span class="dot-title">{{ o.title }}</span><span class="dot-desc">{{ o.desc }}</span></div>
            </label>
          </div>
          <div class="dc-code-label">Template fragment:</div>
          <pre class="dc-code"><code>{{ pdfFragment }}</code></pre>
        </div>
        <div class="dc-divider"><div></div><span>Same export call</span><div></div></div>
        <div class="dc-half dc-teal">
          <div class="dc-engine-lbl"><span class="dot teal"></span>Syncfusion — Excel/PPT Options</div>
          <div class="dc-opts">
            <label v-for="o in excelOpts" :key="o.id" class="dc-opt">
              <input type="checkbox" v-model="o.enabled" />
              <div><span class="dot-title">{{ o.title }}</span><span class="dot-desc">{{ o.desc }}</span></div>
            </label>
          </div>
          <div class="dc-code-label">C# options:</div>
          <pre class="dc-code"><code>{{ excelFragment }}</code></pre>
        </div>
      </div>
    </section>

    <section class="section">
      <div class="eyebrow-sm">HONEST ASSESSMENT — INTEGRATION COST</div>
      <h2>What this adds to your project</h2>
      <div class="cx-grid">
        <div v-for="c in complexityItems" :key="c.t" class="cx-card" :class="c.cls">
          <div class="cx-sev">{{ c.sev }}</div>
          <div class="cx-t">{{ c.t }}</div>
          <div class="cx-d">{{ c.d }}</div>
        </div>
      </div>
    </section>

    <section class="section">
      <div class="eyebrow-sm">EXPORT WORKSPACE</div>
      <h2>Test Hybrid A — pdf uses jsreport, excel/ppt use Syncfusion</h2>
      <div class="workspace">
        <div class="ws-left"><SurveyPicker /><ModuleList /></div>
        <div class="ws-right"><ExportButton provider="js-sync" :supports-html-template="true" /><TemplateDesigner provider-name="js-sync" :supports-html-template="true" /></div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import SurveyPicker from '../../components/SurveyPicker.vue'
import ModuleList from '../../components/ModuleList.vue'
import TemplateDesigner from '../../components/TemplateDesigner.vue'
import ExportButton from '../../components/ExportButton.vue'
// StressTestPanel import borttagen
import LiveReportPreview from '../../components/LiveReportPreview.vue'
import RoutingVisualizer from '../../components/RoutingVisualizer.vue'

const jsFeatures = ['Designer-owned HTML/CSS layout', 'Chart.js visualizations (runs in Chromium)', 'Handlebars {{#if}}/{{#each}} logic', 'Identical to web browser preview', 'Custom fonts, gradients, CSS grid']
const sfFeatures = ['Native .xlsx with =AVERAGE() formulas', 'Editable OfficeChart objects in PPT', 'Conditional formatting (red/yellow/green)', 'Multi-sheet workbooks with auto-fit', 'AES-256 PDF encryption option']

const pdfOpts = ref([
  { id: 'exec', title: 'Executive summary', desc: 'KPIs with OverallAverage from Handlebars', enabled: true },
  { id: 'trend', title: 'Monthly trend chart (Chart.js)', desc: 'Line chart via Chromium JS execution', enabled: true },
  { id: 'highlight', title: 'Highlight low scores', desc: 'Red row when AverageValue < 3.0', enabled: false },
])
const excelOpts = ref([
  { id: 'formulas', title: 'Live =AVERAGE() formulas', desc: 'Recalculate when opened in Excel', enabled: true },
  { id: 'conditional', title: 'Conditional formatting', desc: 'Red/yellow/green cells by score', enabled: true },
  { id: 'charts', title: 'Editable PPT charts', desc: 'OfficeChart objects (not images)', enabled: false },
])

const pdfFragment = computed(() => {
  const L: string[] = []
  if (pdfOpts.value.find(o => o.id === 'exec')?.enabled) L.push('<div class="kpis">\n  <b>{{OverallAverage}}</b> avg\n</div>')
  if (pdfOpts.value.find(o => o.id === 'highlight')?.enabled) L.push('{{#each QuestionSummaries}}\n<div class="{{#if (lt AverageValue 3)}}low{{/if}}">\n  {{Text}} — {{AverageValue}}\n</div>\n{{/each}}')
  else L.push('{{#each QuestionSummaries}}\n<div>{{Text}} — {{AverageValue}}</div>\n{{/each}}')
  if (pdfOpts.value.find(o => o.id === 'trend')?.enabled) L.push('<canvas id="trend"></canvas>\n<script>new Chart("trend",{...})<\\/script>')
  return L.join('\n') || ''
})

const excelFragment = computed(() => {
  const L: string[] = []
  if (excelOpts.value.find(o => o.id === 'formulas')?.enabled) L.push('sheet.Range[r,3].Formula = $"=AVERAGE(B{r}:D{r})";')
  if (excelOpts.value.find(o => o.id === 'conditional')?.enabled) L.push('var cf = sheet.Range[r,3].ConditionalFormats.AddCondition();\ncf.FirstFormula = "3"; cf.BackColorRGB = Red;')
  if (excelOpts.value.find(o => o.id === 'charts')?.enabled) L.push('chart.ChartType = OfficeChartType.Column_Clustered;')
  return L.join('\n') || '// No features enabled'
})

const complexityItems = [
  { cls: 'cx-warn', sev: '⚠ Added complexity', t: 'Two runtime dependencies', d: 'jsreport spawns a Node.js + Chromium process. Syncfusion is pure .NET. Both must be healthy.' },
  { cls: 'cx-warn', sev: '⚠ Added complexity', t: 'Two licenses to maintain', d: 'jsreport Pro + Syncfusion license. Renewal cycles, support contracts, audit requirements doubled.' },
  { cls: 'cx-ok', sev: '✓ Mitigated', t: 'Single API endpoint', d: 'Frontend calls /export/{provider}/{format}. Routing complexity is invisible to the caller.' },
  { cls: 'cx-ok', sev: '✓ Mitigated', t: 'Same IReportProvider interface', d: 'Both engines implement IReportProvider. Swapping later requires one line in Program.cs.' },
  { cls: 'cx-bad', sev: '✗ Not solved', t: 'jsreport cold start still present', d: "PDF requests still hit jsreport's 2–5s cold start. Hybridization doesn't fix this." },
  { cls: 'cx-ok', sev: '✓ Best outcome', t: 'Maximum output quality', d: 'PDF looks like your web app. Excel has real formulas. PPT has editable charts. No single-engine does all three.' },
]
</script>

<style scoped>
  /* Behåller all befintlig CSS */
  @import url('https://fonts.googleapis.com/css2?family=Sora:wght@300;400;600;700;800&family=Fira+Code:wght@400;500&display=swap');

  :root {
    --accent: #8b5cf6;
    --surface: #080616;
    --surface-2: #0f0926;
    --border: rgba(139,92,246,0.15);
    --text: #e2e8f0;
    --text-muted: #64748b
  }

  .page {
    font-family: 'Sora',sans-serif;
    max-width: 1300px;
    margin: 0 auto;
    color: #e2e8f0
  }

  .hero {
    position: relative;
    overflow: hidden;
    border-radius: 20px;
    background: #080616;
    margin-bottom: 48px;
    display: grid;
    grid-template-columns: 1fr 340px;
    min-height: 420px
  }

  .hero-glow {
    position: absolute;
    inset: 0;
    background: radial-gradient(ellipse at 20% 50%,rgba(139,92,246,.1) 0%,transparent 60%),radial-gradient(ellipse at 80% 50%,rgba(20,184,166,.08) 0%,transparent 60%);
    pointer-events: none
  }

  .hero-content {
    position: relative;
    z-index: 1;
    padding: 48px
  }

  .back {
    display: inline-block;
    color: rgba(255,255,255,.3);
    text-decoration: none;
    font-size: 13px;
    margin-bottom: 20px;
    transition: color .15s
  }

    .back:hover {
      color: #8b5cf6
    }

  .eyebrow {
    font-size: 11px;
    text-transform: uppercase;
    letter-spacing: 2px;
    color: #8b5cf6;
    margin-bottom: 12px
  }

  .eyebrow-sm {
    font-size: 10px;
    text-transform: uppercase;
    letter-spacing: 3px;
    color: #8b5cf6;
    margin-bottom: 6px;
    font-family: 'Fira Code',monospace
  }

  h1 {
    font-size: 44px;
    font-weight: 800;
    margin: 0 0 4px;
    color: #fff;
    letter-spacing: -2px
  }

  .plus {
    color: #8b5cf6;
    margin: 0 8px
  }

  .tagline {
    font-family: 'Fira Code',monospace;
    font-size: 13px;
    color: #7c3aed;
    margin-bottom: 16px
  }

  .hero-content > p {
    font-size: 14px;
    color: rgba(255,255,255,.55);
    max-width: 480px;
    line-height: 1.6;
    margin: 0 0 18px
  }

  .verdicts {
    display: flex;
    flex-direction: column;
    gap: 5px;
    margin-bottom: 18px
  }

  .verdict {
    font-size: 11px;
    padding: 5px 10px;
    border-radius: 5px;
    font-weight: 500
  }

    .verdict.ok {
      background: rgba(34,197,94,.07);
      border: 1px solid rgba(34,197,94,.2);
      color: #22c55e
    }

    .verdict.warn {
      background: rgba(245,158,11,.07);
      border: 1px solid rgba(245,158,11,.2);
      color: #f59e0b
    }

  .badges {
    display: flex;
    flex-wrap: wrap;
    gap: 6px
  }

  .b {
    font-size: 10px;
    padding: 3px 10px;
    border-radius: 100px;
    border: 1px solid;
    font-weight: 500
  }

    .b.purple {
      color: #a855f7;
      border-color: rgba(168,85,247,.3);
      background: rgba(168,85,247,.06)
    }

    .b.teal {
      color: #14b8a6;
      border-color: rgba(20,184,166,.3);
      background: rgba(20,184,166,.06)
    }

    .b.yellow {
      color: #f59e0b;
      border-color: rgba(245,158,11,.3);
      background: rgba(245,158,11,.06)
    }

  .hero-split {
    position: relative;
    z-index: 1;
    display: flex;
    flex-direction: column;
    border-left: 1px solid rgba(255,255,255,.05)
  }

  .hs-half {
    flex: 1;
    padding: 28px 20px;
    display: flex;
    flex-direction: column;
    gap: 8px
  }

    .hs-half.hs-purple {
      background: rgba(139,92,246,.04);
      border-bottom: 1px solid rgba(255,255,255,.04)
    }

    .hs-half.hs-teal {
      background: rgba(20,184,166,.04)
    }

    .hs-half p {
      font-size: 11px;
      color: #475569;
      line-height: 1.5
    }

  .hs-engine {
    font-size: 15px;
    font-weight: 700;
    color: #fff
  }

  .hs-fmts {
    display: flex;
    gap: 6px
  }

  .hs-fmt {
    font-size: 11px;
    padding: 3px 10px;
    border-radius: 5px;
    background: rgba(255,255,255,.05);
    color: #94a3b8;
    border: 1px solid rgba(255,255,255,.08)
  }

  .hs-half.hs-purple .hs-fmt {
    color: #a855f7;
    border-color: rgba(168,85,247,.3);
    background: rgba(168,85,247,.06)
  }

  .hs-half.hs-teal .hs-fmt {
    color: #14b8a6;
    border-color: rgba(20,184,166,.3);
    background: rgba(20,184,166,.06)
  }

  .hs-plus {
    text-align: center;
    padding: 10px;
    font-size: 18px;
    font-weight: 800;
    color: rgba(255,255,255,.15);
    background: rgba(255,255,255,.02);
    border-top: 1px solid rgba(255,255,255,.03);
    border-bottom: 1px solid rgba(255,255,255,.03)
  }

  .section {
    margin-bottom: 52px
  }

    .section > h2 {
      font-size: 22px;
      font-weight: 700;
      color: #fff;
      margin: 0 0 8px
    }

  .sub {
    font-size: 13px;
    color: #64748b;
    line-height: 1.6;
    margin: 0 0 22px;
    max-width: 700px
  }

  .engine-compare {
    display: grid;
    grid-template-columns: 1fr auto 1fr;
    gap: 20px;
    align-items: start
  }

  .ec-col {
    background: #080616;
    border-radius: 12px;
    padding: 24px
  }

    .ec-col.ec-purple {
      border-top: 3px solid #a855f7
    }

    .ec-col.ec-teal {
      border-top: 3px solid #14b8a6
    }

  .ec-hdr {
    display: flex;
    align-items: center;
    gap: 10px;
    margin-bottom: 16px;
    font-size: 13px;
    font-weight: 700;
    color: #fff
  }

  .ec-feat {
    display: flex;
    align-items: flex-start;
    gap: 8px;
    padding: 7px 0;
    border-bottom: 1px solid rgba(255,255,255,.04);
    font-size: 12px;
    color: #94a3b8
  }

    .ec-feat:last-child {
      border-bottom: none
    }

  .ec-check {
    color: #a855f7;
    font-weight: 700;
    flex-shrink: 0
  }

    .ec-check.teal {
      color: #14b8a6
    }

  .ec-vs {
    display: flex;
    align-items: center;
    font-size: 18px;
    font-weight: 800;
    color: rgba(255,255,255,.15);
    padding: 0 8px;
    margin-top: 50px
  }

  .dual-config {
    display: grid;
    grid-template-columns: 1fr auto 1fr;
    gap: 20px;
    align-items: start
  }

  .dc-half {
    background: #080616;
    border-radius: 12px;
    padding: 24px
  }

    .dc-half.dc-purple {
      border-top: 3px solid #a855f7
    }

    .dc-half.dc-teal {
      border-top: 3px solid #14b8a6
    }

  .dc-engine-lbl {
    display: flex;
    align-items: center;
    gap: 8px;
    font-size: 11px;
    font-weight: 700;
    color: #94a3b8;
    text-transform: uppercase;
    letter-spacing: .5px;
    margin-bottom: 14px
  }

  .dot {
    width: 8px;
    height: 8px;
    border-radius: 50%;
    flex-shrink: 0
  }

    .dot.purple {
      background: #a855f7;
      box-shadow: 0 0 8px #a855f7
    }

    .dot.teal {
      background: #14b8a6;
      box-shadow: 0 0 8px #14b8a6
    }

  .dc-opts {
    display: flex;
    flex-direction: column;
    gap: 8px;
    margin-bottom: 14px
  }

  .dc-opt {
    display: flex;
    align-items: flex-start;
    gap: 10px;
    cursor: pointer;
    padding: 8px;
    border-radius: 8px;
    border: 1px solid rgba(255,255,255,.04);
    transition: background .1s
  }

    .dc-opt:hover {
      background: rgba(255,255,255,.02)
    }

    .dc-opt input {
      margin-top: 2px;
      accent-color: #8b5cf6;
      flex-shrink: 0
    }

  .dot-title {
    display: block;
    font-size: 12px;
    font-weight: 600;
    color: #e2e8f0;
    margin-bottom: 1px
  }

  .dot-desc {
    font-size: 11px;
    color: #475569
  }

  .dc-code-label {
    font-size: 10px;
    color: #374151;
    text-transform: uppercase;
    letter-spacing: 1px;
    margin-bottom: 6px
  }

  .dc-code {
    margin: 0;
    padding: 12px;
    background: #020a14;
    border-radius: 8px;
    overflow-x: auto;
    border: 1px solid rgba(255,255,255,.04)
  }

    .dc-code code {
      font-family: 'Fira Code',monospace;
      font-size: 10px;
      color: #94a3b8;
      white-space: pre;
      line-height: 1.6
    }

  .dc-divider {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 8px;
    padding: 20px 0;
    margin-top: 40px
  }

    .dc-divider div {
      width: 2px;
      flex: 1;
      background: rgba(255,255,255,.05)
    }

    .dc-divider span {
      writing-mode: vertical-rl;
      font-size: 9px;
      color: #374151;
      text-transform: uppercase;
      letter-spacing: 1px;
      white-space: nowrap
    }

  .cx-grid {
    display: grid;
    grid-template-columns: repeat(3,1fr);
    gap: 12px
  }

  .cx-card {
    background: #080616;
    border-radius: 10px;
    padding: 18px;
    border: 1px solid rgba(255,255,255,.05)
  }

    .cx-card.cx-warn {
      border-top: 3px solid #f59e0b
    }

    .cx-card.cx-ok {
      border-top: 3px solid #22c55e
    }

    .cx-card.cx-bad {
      border-top: 3px solid #ef4444
    }

  .cx-sev {
    font-size: 10px;
    font-weight: 700;
    margin-bottom: 6px
  }

  .cx-card.cx-warn .cx-sev {
    color: #f59e0b
  }

  .cx-card.cx-ok .cx-sev {
    color: #22c55e
  }

  .cx-card.cx-bad .cx-sev {
    color: #ef4444
  }

  .cx-t {
    font-size: 13px;
    font-weight: 700;
    color: #e2e8f0;
    margin-bottom: 6px
  }

  .cx-d {
    font-size: 11px;
    color: #64748b;
    line-height: 1.5
  }

  .workspace {
    display: grid;
    grid-template-columns: 340px 1fr;
    gap: 20px
  }

  .ws-left, .ws-right {
    display: flex;
    flex-direction: column;
    gap: 16px
  }
</style>
