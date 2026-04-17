<template>
  <div class="page">
    <div class="hero">
      <div class="hero-content">
        <router-link to="/" class="back">← Dashboard</router-link>
        <div class="eyebrow">Track 3 · Hybrid Solution A</div>
        <h1>Syncfusion</h1>
        <div class="tagline">Native Office engine. 20 years of Excel expertise.</div>
        <p>Not a PDF engine that also does Excel — a native Office engine. Live formulas, editable charts, AES-256 encryption, PDF/UA.</p>
        <div class="verdicts">
          <div class="verdict ok">✓ Best for: Native Office docs with live formulas</div>
          <div class="verdict warn">⚠ Avoid if: A designer needs to own the layout in HTML/CSS</div>
        </div>
        <div class="badges">
          <span class="b teal">★ Live =AVERAGE() formulas</span>
          <span class="b teal">✓ Editable PPT charts</span>
          <span class="b teal">✓ PDF/UA accessibility</span>
          <span class="b yellow">⚠ No HTML → PDF</span>
        </div>
      </div>
    </div>
    <section class="section">
      <LiveReportPreview provider="js-sync" :show-sliders="false" />
    </section>
    <StressTestPanel provider="js-sync" provider-color="#8b5cf6" />
    <section class="section">
      <div class="eyebrow-sm">EXPORT WORKSPACE</div>
      <h2>Test Syncfusion exports</h2>
      <p class="sub">Excel exports contain live =AVERAGE() formulas. PPT exports contain editable OfficeChart objects.</p>
      <div class="workspace">
        <div class="ws-left"><SurveyPicker /><ModuleList /></div>
        <div class="ws-right"><ExportButton provider="js-sync" :supports-html-template="true" /><TemplateDesigner provider-name="js-sync" :supports-html-template="true" /></div>
      </div>
    </section>
  </div>
</template>
<script setup lang="ts">
import SurveyPicker from '../../components/SurveyPicker.vue'
import ModuleList from '../../components/ModuleList.vue'
import TemplateDesigner from '../../components/TemplateDesigner.vue'
import ExportButton from '../../components/ExportButton.vue'
import StressTestPanel from '../../components/StressTestPanel.vue'
import LiveReportPreview from '../../components/LiveReportPreview.vue'
</script>
<style scoped>
.page{max-width:1300px;margin:0 auto;color:#e2e8f0}
.hero{background:linear-gradient(135deg,#021314,#0a2228);border-radius:20px;padding:48px;margin-bottom:40px}
.back{display:inline-block;color:rgba(255,255,255,.3);text-decoration:none;font-size:13px;margin-bottom:20px}
.back:hover{color:#14b8a6}
.eyebrow{font-size:11px;text-transform:uppercase;letter-spacing:2px;color:#14b8a6;margin-bottom:12px}
.eyebrow-sm{font-size:10px;text-transform:uppercase;letter-spacing:3px;color:#14b8a6;margin-bottom:6px}
h1{font-size:50px;font-weight:800;margin:0 0 4px;color:#fff;letter-spacing:-2px}
.tagline{font-size:14px;color:#14b8a6;margin-bottom:16px}
.hero-content>p{font-size:14px;color:rgba(255,255,255,.55);max-width:500px;line-height:1.6;margin:0 0 18px}
.verdicts{display:flex;flex-direction:column;gap:5px;margin-bottom:18px}
.verdict{font-size:11px;padding:5px 10px;border-radius:5px;font-weight:500}
.verdict.ok{background:rgba(34,197,94,.07);border:1px solid rgba(34,197,94,.2);color:#22c55e}
.verdict.warn{background:rgba(245,158,11,.07);border:1px solid rgba(245,158,11,.2);color:#f59e0b}
.badges{display:flex;flex-wrap:wrap;gap:6px}
.b{font-size:10px;padding:3px 10px;border-radius:100px;border:1px solid;font-weight:500}
.b.teal{color:#14b8a6;border-color:rgba(20,184,166,.3);background:rgba(20,184,166,.06)}
.b.yellow{color:#f59e0b;border-color:rgba(245,158,11,.3);background:rgba(245,158,11,.06)}
.section{margin-bottom:40px}
.section>h2{font-size:22px;font-weight:700;color:#fff;margin:0 0 8px}
.sub{font-size:13px;color:#64748b;line-height:1.6;margin:0 0 20px}
.workspace{display:grid;grid-template-columns:340px 1fr;gap:20px}
.ws-left,.ws-right{display:flex;flex-direction:column;gap:16px}
</style>
