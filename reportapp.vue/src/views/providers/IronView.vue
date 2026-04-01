<template>
  <div class="provider-page">
    <div class="provider-hero" style="--hero-color: #3b82f6">
      <div class="hero-left">
        <router-link to="/" class="back-link">← Dashboard</router-link>
        <div class="track-badge">Spår 1 — Enterprise Suite</div>
        <h1>⚙️ IronSuite (PDF, Excel, PPT)</h1>
        <p>
          En kommersiell helhetslösning för .NET. IronPDF använder en modern Chrome-rendering för
          att konvertera HTML till PDF, medan IronXL och IronPPT hanterar Office-dokument utan
          beroende av Office Interop.
        </p>
        <div class="hero-chips">
          <span class="chip blue">✓ HTML-till-PDF</span>
          <span class="chip blue">✓ Redigerbara Grafer</span>
          <span class="chip blue">✓ Enhetlig Support</span>
          <span class="chip red">✗ Betallicens</span>
        </div>
      </div>
      <div class="hero-stats">
        <div class="hs"><span class="hs-v">$$$</span><span class="hs-l">Licenskostnad</span></div>
        <div class="hs"><span class="hs-v">Chrome</span><span class="hs-l">Rendering</span></div>
        <div class="hs"><span class="hs-v">SLA</span><span class="hs-l">Support</span></div>
      </div>
    </div>

    <section class="section">
      <h2>Vad kan denna provider?</h2>
      <div class="capabilities">
        <div class="cap-card" v-for="cap in capabilities" :key="cap.title" :class="cap.status">
          <div class="cap-icon">{{ cap.icon }}</div>
          <div>
            <div class="cap-title">{{ cap.title }}</div>
            <div class="cap-desc">{{ cap.desc }}</div>
          </div>
          <div class="cap-badge">{{ cap.badge }}</div>
        </div>
      </div>
    </section>

    <section class="section">
      <h2>Kodexempel: HTML till PDF</h2>
      <div class="code-block">
        <div class="cb-header">
          <span>IronSuiteProvider.cs — RenderHtmlToPdf</span>
          <span class="cb-lang">C#</span>
        </div>
        <pre><code>{{ codeExample }}</code></pre>
      </div>
      <div class="code-note">
        IronPDF är extremt kraftfullt när det gäller att tolka modern CSS3 och Flexbox. Detta gör
        att designern i Vue-appen kan producera exakt samma layout i PDF:en som du ser i
        förhandsgranskningen.
      </div>
    </section>

    <div class="workspace">
      <div class="workspace-left">
        <SurveyPicker />
        <ModuleList />
      </div>
      <div class="workspace-right">
        <TemplateDesigner provider-name="IronSuite" :supports-html-template="true" />
        <ExportButton provider="IronSuite" :supports-html-template="true" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import SurveyPicker from "../../components/SurveyPicker.vue";
import ModuleList from "../../components/ModuleList.vue";
import TemplateDesigner from "../../components/TemplateDesigner.vue";
import ExportButton from "../../components/ExportButton.vue";

const capabilities = [
  {
    icon: "📕",
    title: "IronPDF",
    desc: "Pixel-perfekt rendering av HTML/CSS. Stödjer JavaScript-exekvering under rendering.",
    badge: "✓ Utmärkt",
    status: "ok",
  },
  {
    icon: "📗",
    title: "IronXL",
    desc: "Skapa och läs Excel-filer snabbt. Stödjer formler, cellformat och kryptering.",
    badge: "✓ Full",
    status: "ok",
  },
  {
    icon: "📘",
    title: "IronPPT",
    desc: "Konvertera HTML-slides till PowerPoint eller skapa presentationer från scratch.",
    badge: "✓ Bra",
    status: "ok",
  },
  {
    icon: "🎨",
    title: "Mallsystem",
    desc: "Fullt stöd för HTML-mallar. Användaren kan redigera layouten direkt i webbläsaren.",
    badge: "✓ Ja",
    status: "ok",
  },
  {
    icon: "🛡️",
    title: "Säkerhet",
    desc: "Enterprise-grade kryptering, lösenordsskydd och digitala signaturer direkt i API:et.",
    badge: "✓ High",
    status: "ok",
  },
  {
    icon: "💳",
    title: "Licens",
    desc: "Kräver betald licens för produktion. Gratis testversion finns tillgänglig.",
    badge: "Betald",
    status: "warn",
  },
];

const codeExample = `// Konfigurera renderaren
var renderer = new ChromePdfRenderer();

// Inställningar för layout
renderer.RenderingOptions.MarginTop = 20;
renderer.RenderingOptions.EnableJavaScript = true;
renderer.RenderingOptions.RenderDelay = 500; // Vänta på grafer

// Rendera HTML-strängen från Vue-appen
var pdf = renderer.RenderHtmlAsPdf(htmlTemplate);

// Spara eller returnera som stream
return pdf.Stream;`;
</script>

<style scoped src="../../assets/provider-styles.css"></style>
