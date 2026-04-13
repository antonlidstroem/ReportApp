<template>
  <div class="provider-page" style="--hero-color: #22c55e">
    <div class="provider-hero" style="background: linear-gradient(135deg, #064e3b, #021314)">
      <div class="hero-content">
        <router-link to="/" class="back-link">← Dashboard</router-link>
        <div class="track-badge">Hybrid Solution B</div>
        <h1>Playwright + Syncfusion</h1>
        <p>
          <strong>The Modern Pipeline:</strong> Utnyttja Playwrights extremt snabba
          head-less motor för PDF-rendering. Perfekt för molnmiljöer där Playwright
          redan används för tester.
        </p>
        <div class="hero-chips">
          <span class="chip green">PDF: Playwright (Chromium)</span>
          <span class="chip blue">Office: Syncfusion (Native)</span>
          <span class="chip yellow">★ Snabbare PDF-initiering</span>
        </div>
      </div>
    </div>

    <section class="section">
      <h2>Varför Playwright?</h2>
      <div class="capabilities">
        <div class="cap-card ok">
          <div class="cap-icon">⚡</div>
          <div class="cap-title">Lättviktig PDF</div>
          <div class="cap-desc">Snabbare "cold start" än jsreport. Hanterar modern CSS3 och Flexbox perfekt.</div>
        </div>
        <div class="cap-card ok">
          <div class="cap-icon">📈</div>
          <div class="cap-title">Native Office</div>
          <div class="cap-desc">Använder Syncfusion XlsIO för att skapa Excel-filer som inte är begränsade till HTML.</div>
        </div>
      </div>
    </section>

    <div class="workspace">
      <div class="workspace-left">
        <SurveyPicker />
        <ModuleList />
      </div>
      <div class="workspace-right">
        <TemplateDesigner provider-name="playwright-syncfusion" :supports-html-template="true" />
        <ExportButton provider="playwright-syncfusion" :supports-html-template="true" />
      </div>
    </div>
  </div>
</template>
