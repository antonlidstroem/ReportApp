<template>
  <div class="page">
    <div class="hero">
      <div class="hero-glow"></div>
      <div class="hero-content">
        <router-link to="/" class="back">← Dashboard</router-link>
        <div class="eyebrow">Track 4 · Hybrid Solution B</div>
        <h1>Playwright <span class="plus">+</span> Syncfusion</h1>
        <div class="tagline">For teams already running Playwright for tests.</div>
        <p>Same hybrid pattern as Track 3 — but Playwright replaces jsreport as the PDF engine. Faster cold start, no Node.js runtime layer, and if you already use Playwright for E2E testing, your CI already has Chromium.</p>
        <div class="verdicts">
          <div class="verdict ok">✓ Best for: Teams already using Playwright for E2E testing</div>
          <div class="verdict info">ℹ Unlike Hybrid A: PDF templates use token replacement, not full Handlebars</div>
        </div>
        <div class="badges">
          <span class="b green">★ ~200ms PDF cold start</span>
          <span class="b teal">★ Native Excel formulas</span>
          <span class="b teal">✓ Editable PPT charts</span>
          <span class="b yellow">⚠ No Chart.js in PDF</span>
        </div>
      </div>
      <div class="hero-timing">
        <div class="ht-card green-card">
          <div class="ht-val">~200ms</div>
          <div class="ht-lbl">Playwright cold start</div>
        </div>
        <div class="ht-vs">vs</div>
        <div class="ht-card yellow-card">
          <div class="ht-val">2–5s</div>
          <div class="ht-lbl">jsreport cold start</div>
        </div>
        <div class="ht-note">First PDF request — subsequent calls are faster for both</div>
      </div>
    </div>

    <section class="section">
      <div class="eyebrow-sm">KEY DIFFERENTIATOR vs HYBRID A</div>
      <h2>Cold Start Race — Playwright vs jsreport</h2>
      <p class="sub">The core reason to pick Hybrid B over Hybrid A. Playwright's Chromium launches faster because it skips the jsreport Node.js runtime layer. Click "Race" to see the startup sequence.</p>
      <div class="race-panel">
        <div class="race-lanes">
          <div class="race-lane">
            <div class="rl-hdr">
              <span class="rl-icon">🎭</span>
              <span class="rl-name">Playwright (Hybrid B)</span>
              <span class="rl-time" :class="{ done: raceDone }">{{ pwStatus }}</span>
            </div>
            <div class="rl-progress"><div class="rl-bar pw-bar" :style="`width:${pwPct}%`"></div></div>
            <div class="rl-steps">
              <div v-for="(s, i) in pwSteps" :key="i" class="rl-step" :class="{ done: s.done, active: s.active }">
                <span>{{ s.done ? '✓' : s.active ? '⟳' : '○' }}</span>
                <span>{{ s.label }}</span>
                <span v-if="s.ms" class="rl-ms">{{ s.ms }}ms</span>
              </div>
            </div>
          </div>
          <div class="race-lane race-lane-slower">
            <div class="rl-hdr">
              <span class="rl-icon">🌐</span>
              <span class="rl-name">jsreport (Hybrid A)</span>
              <span class="rl-time" :class="{ done: raceDone }">{{ jsStatus }}</span>
            </div>
            <div class="rl-progress"><div class="rl-bar js-bar" :style="`width:${jsPct}%`"></div></div>
            <div class="rl-steps">
              <div v-for="(s, i) in jsSteps" :key="i" class="rl-step" :class="{ done: s.done, active: s.active }">
                <span>{{ s.done ? '✓' : s.active ? '⟳' : '○' }}</span>
                <span>{{ s.label }}</span>
                <span v-if="s.ms" class="rl-ms">{{ s.ms }}ms</span>
              </div>
            </div>
          </div>
        </div>
        <div class="race-ctrls">
          <button class="race-btn" :disabled="isRacing" @click="startRace">
            {{ isRacing ? '🏁 Racing...' : raceDone ? '↺ Race Again' : '🏁 Start Race' }}
          </button>
          <div v-if="raceDone" class="race-winner">🏆 Playwright wins — ~{{ timeSaved }}ms faster on cold start</div>
        </div>
      </div>
    </section>

    <section class="section">
      <div class="eyebrow-sm">ARCHITECTURE — SAME PATTERN, DIFFERENT PDF ENGINE</div>
      <h2>Playwright routes PDF · Syncfusion routes Excel + PPT</h2>
      <RoutingVisualizer pdf-engine="playwright" />
    </section>

    <section class="section">
      <div class="eyebrow-sm">CRITICAL TRADEOFF vs HYBRID A</div>
      <h2>Template support — what you gain and lose vs jsreport</h2>
      <p class="sub">This is the key difference between Hybrid A and Hybrid B. Understand what you give up when you swap jsreport for Playwright.</p>
      <div class="tmpl-compare">
        <div class="tc-col tc-purple">
          <div class="tc-hdr"><span>🌐 jsreport (Hybrid A)</span><span class="tc-badge tc-full">Full Handlebars</span></div>
          <pre class="tc-code"><code>{{ jsTemplate }}</code></pre>
          <div class="tc-feats">
            <div v-for="f in jsTemplateFeats" :key="f" class="tcf ok">✓ {{ f }}</div>
          </div>
        </div>
        <div class="tc-arrow"><div>→</div><div class="tc-arrow-lbl">Switch to Hybrid B</div></div>
        <div class="tc-col tc-green">
          <div class="tc-hdr"><span>🎭 Playwright (Hybrid B)</span><span class="tc-badge tc-limited">Token replacement only</span></div>
          <pre class="tc-code"><code>{{ pwTemplate }}</code></pre>
          <div class="tc-feats">
            <div v-for="f in pwTemplateOk" :key="f" class="tcf ok">✓ {{ f }}</div>
            <div v-for="f in pwTemplateMissing" :key="f" class="tcf missing">✗ {{ f }}</div>
          </div>
        </div>
      </div>
    </section>

    <section class="section">
      <div class="eyebrow-sm">THE ZERO-INFRASTRUCTURE ARGUMENT</div>
      <h2>Already using Playwright? Zero new dependencies for PDF.</h2>
      <div class="ci-compare">
        <div class="ci-col ci-warn">
          <div class="ci-lbl">Without Playwright (jsreport)</div>
          <div class="ci-items">
            <div class="ci-item ci-exists">✓ .NET runtime</div>
            <div class="ci-item ci-exists">✓ Syncfusion DLLs</div>
            <div class="ci-item ci-new">+ Node.js runtime</div>
            <div class="ci-item ci-new">+ jsreport binary (~80MB)</div>
            <div class="ci-item ci-new">+ Chromium (via jsreport)</div>
            <div class="ci-item ci-new">+ jsreport Pro license</div>
          </div>
        </div>
        <div class="ci-vs">→</div>
        <div class="ci-col ci-ok">
          <div class="ci-lbl">With Playwright (Hybrid B)</div>
          <div class="ci-items">
            <div class="ci-item ci-exists">✓ .NET runtime</div>
            <div class="ci-item ci-exists">✓ Syncfusion DLLs</div>
            <div class="ci-item ci-exists">✓ Playwright (already in CI)</div>
            <div class="ci-item ci-exists">✓ Chromium (already in CI)</div>
            <div class="ci-item ci-saved">✗ No Node.js needed</div>
            <div class="ci-item ci-saved">✗ No jsreport license</div>
          </div>
        </div>
      </div>
    </section>

    <section class="section">
      <div class="eyebrow-sm">PDF PREVIEW — Playwright token replacement</div>
      <h2>What Playwright PDF output looks like</h2>
      <LiveReportPreview provider="play-sync" :show-sliders="false" />
    </section>

    <section class="section">
      <div class="eyebrow-sm">EXPORT WORKSPACE</div>
      <h2>Test Hybrid B</h2>
      <p class="sub">PDF uses Playwright (token replacement). Excel and PPT use Syncfusion (same as Hybrid A).</p>
      <div class="workspace">
        <div class="ws-left"><SurveyPicker /><ModuleList /></div>
        <div class="ws-right"><ExportButton provider="play-sync" :supports-html-template="true" /><TemplateDesigner provider-name="play-sync" :supports-html-template="true" /></div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import SurveyPicker from '../../components/SurveyPicker.vue'
import ModuleList from '../../components/ModuleList.vue'
import TemplateDesigner from '../../components/TemplateDesigner.vue'
import ExportButton from '../../components/ExportButton.vue'
import LiveReportPreview from '../../components/LiveReportPreview.vue'
import RoutingVisualizer from '../../components/RoutingVisualizer.vue'

interface RaceStep { label: string; ms?: number; done: boolean; active: boolean }

const isRacing = ref(false)
const raceDone = ref(false)
const pwStatus = ref('Ready')
const jsStatus = ref('Ready')
const pwPct = ref(0)
const jsPct = ref(0)
const timeSaved = ref(2100)

const pwSteps = ref<RaceStep[]>([
  { label: 'Launch Chromium', done: false, active: false },
  { label: 'Create browser context', done: false, active: false },
  { label: 'Set HTML content', done: false, active: false },
  { label: 'Wait for networkidle', done: false, active: false },
  { label: 'PDF captured ✓', done: false, active: false },
])

const jsSteps = ref<RaceStep[]>([
  { label: 'Start Node.js process', done: false, active: false },
  { label: 'Initialize jsreport runtime', done: false, active: false },
  { label: 'Launch Chromium', done: false, active: false },
  { label: 'Compile Handlebars template', done: false, active: false },
  { label: 'Render & capture PDF ✓', done: false, active: false },
])

const pwTimings = [50, 180, 240, 290, 340]
const jTimings  = [50, 660, 1470, 1850, 1960]
const pwDurs    = [120, 50, 40, 40, 10]
const jDurs     = [600, 800, 350, 80, 120]

function reset() {
  pwSteps.value.forEach(s => { s.done = false; s.active = false; s.ms = undefined })
  jsSteps.value.forEach(s => { s.done = false; s.active = false; s.ms = undefined })
  pwPct.value = 0; jsPct.value = 0
  pwStatus.value = 'Ready'; jsStatus.value = 'Ready'
  raceDone.value = false
}

async function startRace() {
  if (isRacing.value) return
  reset()
  isRacing.value = true

  pwTimings.forEach((delay, i) => {
    setTimeout(() => { pwSteps.value[i].active = true }, delay)
    setTimeout(() => {
      pwSteps.value[i].active = false
      pwSteps.value[i].done = true
      pwSteps.value[i].ms = pwDurs[i] + Math.floor(Math.random() * 20)
      pwPct.value = Math.round(((i + 1) / pwSteps.value.length) * 100)
    }, delay + pwDurs[i])
  })

  jTimings.forEach((delay, i) => {
    setTimeout(() => { jsSteps.value[i].active = true }, delay)
    setTimeout(() => {
      jsSteps.value[i].active = false
      jsSteps.value[i].done = true
      jsSteps.value[i].ms = jDurs[i] + Math.floor(Math.random() * 30)
      jsPct.value = Math.round(((i + 1) / jsSteps.value.length) * 100)
    }, delay + jDurs[i])
  })

  setTimeout(() => { pwStatus.value = '✓ ~340ms' }, 350)
  setTimeout(() => {
    jsStatus.value = '✓ ~2,440ms'
    raceDone.value = true
    isRacing.value = false
    timeSaved.value = 2100
  }, 2100)
}

const jsTemplate = `{{#each QuestionSummaries}}
<div class="row
  {{#if (gt AverageValue 4)}}high
  {{else}}normal{{/if}}">
  <span>Q{{@index}}: {{Text}}</span>
  <b>{{AverageValue}}</b>
</div>
{{/each}}

{{#if Trends}}
<canvas id="chart"></canvas>
<script>new Chart("chart",{...})<\/script>
{{/if}}`

const pwTemplate = `<h1>{{SurveyTitle}}</h1>
<p>{{CompanyName}} · {{GeneratedAt}}</p>

{{QuestionsTableHtml}}

<img src="data:image/png;base64,{{ChartPng}}" />`

const jsTemplateFeats = ['#each loops directly in template', '#if/#unless conditionals', 'Chart.js runs in Chromium', 'Dynamic CSS classes from data', '@index counters']
const pwTemplateOk = ['{{Token}} replacement', 'Full CSS/font support', 'Any static HTML layout']
const pwTemplateMissing = ['No #each loops', 'No #if conditionals', 'No Chart.js execution', 'No dynamic classes from data']
</script>

<style scoped>
  @import url('https://fonts.googleapis.com/css2?family=DM+Sans:wght@300;400;500;600;700;800&family=DM+Mono:wght@400;500&display=swap');

  :root {
    --accent: #10b981;
    --surface: #020f0a;
    --surface-2: #0a1f14;
    --border: rgba(16,185,129,0.15);
    --text: #e2e8f0;
    --text-muted: #64748b
  }

  .page {
    font-family: 'DM Sans',sans-serif;
    max-width: 1300px;
    margin: 0 auto;
    color: #e2e8f0
  }

  .hero {
    position: relative;
    overflow: hidden;
    border-radius: 20px;
    background: #020f0a;
    margin-bottom: 48px;
    display: grid;
    grid-template-columns: 1fr 280px;
    min-height: 420px
  }

  .hero-glow {
    position: absolute;
    inset: 0;
    background: radial-gradient(ellipse at 80% 50%,rgba(16,185,129,.12) 0%,transparent 70%);
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
      color: #10b981
    }

  .eyebrow {
    font-size: 11px;
    text-transform: uppercase;
    letter-spacing: 2px;
    color: #10b981;
    margin-bottom: 12px
  }

  .eyebrow-sm {
    font-size: 10px;
    text-transform: uppercase;
    letter-spacing: 3px;
    color: #10b981;
    margin-bottom: 6px;
    font-family: 'DM Mono',monospace
  }

  h1 {
    font-size: 46px;
    font-weight: 800;
    margin: 0 0 4px;
    color: #fff;
    letter-spacing: -2px
  }

  .plus {
    color: #10b981;
    margin: 0 8px
  }

  .tagline {
    font-family: 'DM Mono',monospace;
    font-size: 13px;
    color: #10b981;
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

    .verdict.info {
      background: rgba(59,130,246,.07);
      border: 1px solid rgba(59,130,246,.2);
      color: #60a5fa
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

    .b.green {
      color: #10b981;
      border-color: rgba(16,185,129,.3);
      background: rgba(16,185,129,.06)
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

  .hero-timing {
    position: relative;
    z-index: 1;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 10px;
    padding: 32px 24px;
    background: rgba(16,185,129,.03);
    border-left: 1px solid rgba(16,185,129,.1)
  }

  .ht-card {
    text-align: center;
    border-radius: 12px;
    padding: 16px 20px;
    width: 100%;
    border: 1px solid
  }

    .ht-card.green-card {
      background: rgba(16,185,129,.06);
      border-color: rgba(16,185,129,.2)
    }

    .ht-card.yellow-card {
      background: rgba(245,158,11,.04);
      border-color: rgba(245,158,11,.15)
    }

  .ht-val {
    font-size: 26px;
    font-weight: 800;
    font-family: 'DM Mono',monospace
  }

  .ht-card.green-card .ht-val {
    color: #10b981
  }

  .ht-card.yellow-card .ht-val {
    color: #f59e0b
  }

  .ht-lbl {
    font-size: 10px;
    color: #475569;
    margin-top: 4px
  }

  .ht-vs {
    font-size: 18px;
    font-weight: 800;
    color: rgba(255,255,255,.12)
  }

  .ht-note {
    font-size: 10px;
    color: #374151;
    text-align: center;
    line-height: 1.4
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

  .race-panel {
    background: #020f0a;
    border: 1px solid rgba(16,185,129,.15);
    border-radius: 14px;
    padding: 28px
  }

  .race-lanes {
    display: flex;
    flex-direction: column;
    gap: 24px;
    margin-bottom: 24px
  }

  .rl-hdr {
    display: flex;
    align-items: center;
    gap: 10px;
    margin-bottom: 10px
  }

  .rl-icon {
    font-size: 18px
  }

  .rl-name {
    font-size: 13px;
    font-weight: 700;
    color: #fff;
    flex: 1
  }

  .rl-time {
    font-size: 12px;
    font-weight: 700;
    color: #475569;
    font-family: 'DM Mono',monospace
  }

    .rl-time.done {
      color: #22c55e
    }

  .rl-progress {
    height: 6px;
    background: rgba(255,255,255,.04);
    border-radius: 3px;
    margin-bottom: 10px;
    overflow: hidden
  }

  .rl-bar {
    height: 100%;
    border-radius: 3px;
    transition: width .3s ease
  }

  .pw-bar {
    background: #10b981
  }

  .js-bar {
    background: #f59e0b
  }

  .rl-steps {
    display: flex;
    gap: 0;
    flex-wrap: nowrap;
    overflow-x: auto
  }

  .rl-step {
    display: flex;
    align-items: center;
    gap: 5px;
    padding: 3px 12px;
    font-size: 10px;
    color: #374151;
    white-space: nowrap;
    border-right: 1px solid rgba(255,255,255,.03);
    transition: color .2s
  }

    .rl-step.done {
      color: #22c55e
    }

    .rl-step.active {
      color: #e2e8f0
    }

      .rl-step.active span:first-child {
        display: inline-block;
        animation: spin .6s linear infinite
      }

  @keyframes spin {
    to {
      transform: rotate(360deg)
    }
  }

  .rl-ms {
    font-family: 'DM Mono',monospace;
    font-size: 9px;
    color: #10b981;
    background: rgba(16,185,129,.1);
    padding: 1px 4px;
    border-radius: 3px;
    margin-left: 2px
  }

  .race-ctrls {
    display: flex;
    align-items: center;
    gap: 20px
  }

  .race-btn {
    padding: 10px 28px;
    border-radius: 8px;
    border: none;
    background: #10b981;
    color: #000;
    font-size: 14px;
    font-weight: 700;
    cursor: pointer;
    font-family: 'DM Sans',sans-serif;
    transition: all .15s
  }

    .race-btn:disabled {
      opacity: .5;
      cursor: wait;
      background: #374151;
      color: #6b7280
    }

    .race-btn:hover:not(:disabled) {
      background: #34d399
    }

  .race-winner {
    font-size: 14px;
    font-weight: 700;
    color: #10b981
  }

  .tmpl-compare {
    display: grid;
    grid-template-columns: 1fr auto 1fr;
    gap: 20px;
    align-items: start
  }

  .tc-col {
    background: #020f0a;
    border-radius: 12px;
    overflow: hidden;
    border: 1px solid rgba(255,255,255,.05)
  }

    .tc-col.tc-purple {
      border-top: 3px solid #a855f7
    }

    .tc-col.tc-green {
      border-top: 3px solid #10b981
    }

  .tc-hdr {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 12px 16px;
    background: rgba(255,255,255,.02);
    font-size: 12px;
    font-weight: 600;
    color: #94a3b8;
    border-bottom: 1px solid rgba(255,255,255,.04)
  }

  .tc-badge {
    font-size: 10px;
    padding: 2px 8px;
    border-radius: 4px;
    font-weight: 600
  }

    .tc-badge.tc-full {
      background: rgba(168,85,247,.12);
      color: #a855f7
    }

    .tc-badge.tc-limited {
      background: rgba(245,158,11,.12);
      color: #f59e0b
    }

  .tc-code {
    margin: 0;
    padding: 14px;
    background: #020a14;
    overflow-x: auto
  }

    .tc-code code {
      font-family: 'DM Mono',monospace;
      font-size: 10px;
      color: #94a3b8;
      white-space: pre;
      line-height: 1.7
    }

  .tc-feats {
    padding: 12px 16px;
    display: flex;
    flex-direction: column;
    gap: 4px
  }

  .tcf {
    font-size: 11px
  }

    .tcf.ok {
      color: #22c55e
    }

    .tcf.missing {
      color: #ef4444
    }

  .tc-arrow {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 6px;
    padding: 40px 6px 0
  }

    .tc-arrow div:first-child {
      font-size: 22px;
      color: rgba(255,255,255,.12)
    }

  .tc-arrow-lbl {
    writing-mode: vertical-rl;
    font-size: 9px;
    color: #374151;
    text-transform: uppercase;
    letter-spacing: 1px
  }

  .ci-compare {
    display: grid;
    grid-template-columns: 1fr auto 1fr;
    gap: 24px;
    align-items: start
  }

  .ci-col {
    background: #020f0a;
    border-radius: 12px;
    padding: 24px
  }

    .ci-col.ci-warn {
      border-top: 3px solid #f59e0b
    }

    .ci-col.ci-ok {
      border-top: 3px solid #10b981
    }

  .ci-lbl {
    font-size: 11px;
    font-weight: 700;
    text-transform: uppercase;
    letter-spacing: .5px;
    margin-bottom: 14px
  }

  .ci-col.ci-warn .ci-lbl {
    color: #f59e0b
  }

  .ci-col.ci-ok .ci-lbl {
    color: #10b981
  }

  .ci-items {
    display: flex;
    flex-direction: column;
    gap: 8px
  }

  .ci-item {
    font-size: 12px;
    padding: 7px 12px;
    border-radius: 6px
  }

    .ci-item.ci-exists {
      color: #94a3b8;
      background: rgba(255,255,255,.02)
    }

    .ci-item.ci-new {
      color: #f59e0b;
      background: rgba(245,158,11,.06);
      border: 1px solid rgba(245,158,11,.15)
    }

    .ci-item.ci-saved {
      color: #64748b;
      text-decoration: line-through;
      background: rgba(16,185,129,.04);
      border: 1px dashed rgba(16,185,129,.1)
    }

  .ci-vs {
    display: flex;
    align-items: center;
    font-size: 22px;
    color: rgba(255,255,255,.1);
    margin-top: 46px
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
