<template>
  <div class="provider-page" style="--hero-color: #a855f7">
    <div class="provider-hero" style="background: linear-gradient(135deg, #2e1065, #021314)">
      <div class="hero-content">
        <router-link to="/" class="back-link">← Dashboard</router-link>
        <div class="track-badge">Hybrid Solution A</div>
        <h1>jsreport + Syncfusion</h1>
        <p>
          <strong>Best of Both Worlds:</strong> Pixel-perfect HTML/CSS PDFs with live
          Chart.js visualizations via jsreport, combined with Syncfusion's
          enterprise-grade Excel and PowerPoint engines.
        </p>
        <div class="hero-chips">
          <span class="chip purple">PDF: jsreport (Chromium)</span>
          <span class="chip blue">Office: Syncfusion (Native)</span>
          <span class="chip green">★ Full Chart.js Support</span>
        </div>
      </div>
    </div>

    <section class="section">
      <h2>Strategisk arkitektur</h2>
      <div class="capabilities">
        <div class="cap-card ok">
          <div class="cap-icon">📄</div>
          <div class="cap-title">PDF via jsreport</div>
          <div class="cap-desc">Fullt stöd för Handlebars, CSS-grid och komplexa webb-grafer.</div>
        </div>
        <div class="cap-card ok">
          <div class="cap-icon">📊</div>
          <div class="cap-title">Office via Syncfusion</div>
          <div class="cap-desc">Excel-filer med riktiga formler och PPT med redigerbara grafer.</div>
        </div>
        <div class="cap-card blue">
          <div class="cap-icon">🔄</div>
          <div class="cap-title">Backend Orchestration</div>
          <div class="cap-desc">C#-controllern väljer motor baserat på det valda exportformatet.</div>
        </div>
      </div>
    </section>

    <div class="workspace">
      <div class="workspace-left">
        <SurveyPicker />
        <ModuleList />
      </div>
      <div class="workspace-right">
        <TemplateDesigner provider-name="jsreport-syncfusion" :supports-html-template="true" />
        <ExportButton provider="jsreport-syncfusion" :supports-html-template="true" />
      </div>
    </div>
  </div>
</template>
