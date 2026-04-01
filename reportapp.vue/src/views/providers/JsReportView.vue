<template>
  <div class="provider-page">
    <div class="provider-hero" style="--hero-color: #a855f7">
      <div class="hero-left">
        <router-link to="/" class="back-link">← Dashboard</router-link>
        <div class="track-badge">Spår 3 — Web-Standard Engine</div>
        <h1>🌐 jsreport (Chromium & Handlebars)</h1>
        <p>
          En dedikerad rapportmotor som körs som en sidovagn eller tjänst. Den använder
          webbteknologier fullt ut: HTML, CSS och JavaScript (t.ex. Chart.js) för att skapa
          dynamiska dokument.
        </p>
        <div class="hero-chips">
          <span class="chip purple">✓ Chart.js Support</span>
          <span class="chip purple">✓ Handlebars Logic</span>
          <span class="chip green">✓ Open Source / Pro</span>
          <span class="chip red">✗ PPT Saknas</span>
        </div>
      </div>
      <div class="hero-stats">
        <div class="hs"><span class="hs-v">Sidecar</span><span class="hs-l">Arkitektur</span></div>
        <div class="hs"><span class="hs-v">JS</span><span class="hs-l">Logik-motor</span></div>
        <div class="hs"><span class="hs-v">Any</span><span class="hs-l">Datakälla</span></div>
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
      <h2>Kodexempel: Handlebars & Chrome</h2>
      <div class="code-block">
        <div class="cb-header">
          <span>jsreportProvider.cs — RenderReportAsync</span>
          <span class="cb-lang">C#</span>
        </div>
        <pre><code>{{ codeExample }}</code></pre>
      </div>
      <div class="code-note">
        jsreport utmärker sig genom att låta dig använda <strong>Handlebars</strong>-loopar direkt i
        HTML-mallen. Du kan till och med inkludera <code>&lt;script&gt;</code>-taggar för att ladda
        Chart.js och rita grafer innan PDF:en "fotograferas".
      </div>
    </section>

    <div class="workspace">
      <div class="workspace-left">
        <SurveyPicker />
        <ModuleList />
      </div>
      <div class="workspace-right">
        <TemplateDesigner provider-name="jsreport" :supports-html-template="true" />
        <ExportButton provider="jsreport" :supports-html-template="true" />
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
    title: "PDF (Chrome)",
    desc: "Använder Chromium för rendering. Bästa stödet för grafer och komplex CSS.",
    badge: "✓ Bäst",
    status: "ok",
  },
  {
    icon: "📗",
    title: "Excel (Recipe)",
    desc: "Använder Excel-recept. Kan generera XLSX genom att mappa HTML-tabeller eller använda templates.",
    badge: "⚠ Begränsat",
    status: "warn",
  },
  {
    icon: "📘",
    title: "PPT",
    desc: "Inget native stöd i standardversionen. Kräver ofta anpassade plugins.",
    badge: "✗ Nej",
    status: "no",
  },
  {
    icon: "📊",
    title: "JS-Grafer",
    desc: "Du kan köra Chart.js eller D3.js inuti rapporten. Graferna blir statiska bilder i PDF:en.",
    badge: "✓ Unikt",
    status: "ok",
  },
  {
    icon: "🧩",
    title: "Handlebars",
    desc: "Kraftfull logik direkt i mallen: if-satser, loopar och helpers.",
    badge: "✓ Ja",
    status: "ok",
  },
  {
    icon: "☁️",
    title: "Deployment",
    desc: "Körs ofta som en Docker-container. Kräver mer infrastruktur än ett bibliotek.",
    badge: "⚠ Sidecar",
    status: "warn",
  },
];

const codeExample = `// Anropa jsreport-servern (lokal eller moln)
var report = await jsreport.RenderAsync(new RenderRequest {
    Template = new Template {
        Content = htmlTemplate, // Din HTML/Handlebars kod
        Recipe = Recipe.ChromePdf,
        Engine = Engine.Handlebars,
        Chrome = new Chrome {
            WaitForNetworkIddle = true // Vänta på JS-grafer
        }
    },
    Data = surveyData // Skickas in som JSON till mallen
});

return report.Content;`;
</script>

<style scoped src="../../assets/provider-styles.css"></style>
