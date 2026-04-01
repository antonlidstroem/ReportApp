<template>
  <div class="provider-page">
    <!-- Hero -->
    <div class="provider-hero" style="--hero-color: #22c55e">
      <div class="hero-left">
        <router-link to="/" class="back-link">← Dashboard</router-link>
        <div class="track-badge">Spår 2 — Best-of-Breed Open Source</div>
        <h1>🏗️ QuestPDF + ClosedXML + ShapeCrawler</h1>
        <p>
          Kod-centrerad approach med tre dedikerade open source-bibliotek. PDF byggs i C# via ett
          Fluent API, Excel via ClosedXML och PPT via ShapeCrawler.
        </p>
        <div class="hero-chips">
          <span class="chip green">✓ Gratis (Community)</span>
          <span class="chip green">✓ Ingen Chromium</span>
          <span class="chip yellow">⚠ HTML-mallar ej stödda</span>
          <span class="chip yellow">⚠ PPT begränsat</span>
        </div>
      </div>
      <div class="hero-stats">
        <div class="hs"><span class="hs-v">0 kr</span><span class="hs-l">Licenskostnad</span></div>
        <div class="hs"><span class="hs-v">C#</span><span class="hs-l">API-stil</span></div>
        <div class="hs"><span class="hs-v">3</span><span class="hs-l">Bibliotek</span></div>
      </div>
    </div>

    <!-- Capabilities -->
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

    <!-- Code example -->
    <section class="section">
      <h2>Hur genereras en PDF?</h2>
      <div class="code-block">
        <div class="cb-header">
          <span>QuestOpenSourceProvider.cs — GeneratePdfAsync</span>
          <span class="cb-lang">C#</span>
        </div>
        <pre><code>{{ pdfCodeExample }}</code></pre>
      </div>
      <div class="code-note">
        QuestPDF använder ett Fluent C# API — allt definieras i kod. Det ger full kontroll men
        innebär att HTML-mallar från designern inte kan användas direkt. Istället fungerar "mallar"
        som förkonfigurerade layoutval (vilka sektioner som inkluderas, färgschema etc.).
      </div>
    </section>

    <!-- Main workspace -->
    <div class="workspace">
      <!-- Left column -->
      <div class="workspace-left">
        <SurveyPicker />
        <ModuleList />
      </div>

      <!-- Right column -->
      <div class="workspace-right">
        <TemplateDesigner provider-name="QuestOpenSource" :supports-html-template="false" />
        <ExportButton provider="QuestOpenSource" :supports-html-template="false" />
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
    title: "PDF (QuestPDF)",
    desc: "Fullständigt stöd via Fluent C# API. Tabeller, header/footer, sidnumrering, bilder.",
    badge: "✓ Full",
    status: "ok",
  },
  {
    icon: "📗",
    title: "Excel (ClosedXML)",
    desc: "Workbooks med flera ark, formatering, formler, cell-styles. Inga native charts ännu.",
    badge: "✓ Full",
    status: "ok",
  },
  {
    icon: "📘",
    title: "PPT (ShapeCrawler)",
    desc: "Kan skapa och modifiera presentationer. Begränsat grafstöd i nuläget.",
    badge: "⚠ Basic",
    status: "warn",
  },
  {
    icon: "🎨",
    title: "HTML-mallar",
    desc: "Stöds INTE — QuestPDF är kod-first. Mallsystemet erbjuder i stället konfigurerbara layoutpresets.",
    badge: "✗ Nej",
    status: "no",
  },
  {
    icon: "📈",
    title: "Grafer i PDF",
    desc: "Kan renderas som SVG eller bitmap om data förbereds i backend innan generering.",
    badge: "⚠ Manuell",
    status: "warn",
  },
  {
    icon: "💰",
    title: "Kostnad",
    desc: "Gratis (Community-licens) för projekt med <1M USD intäkt. Professional kostar.",
    badge: "✓ Gratis",
    status: "ok",
  },
];

const pdfCodeExample = `var document = Document.Create(container => {
    container.Page(page => {
        page.Size(PageSizes.A4);
        page.Margin(1.5f, Unit.Centimetre);

        page.Header()
            .BorderBottom(2).BorderColor(Colors.Blue.Darken2)
            .Row(row => {
                row.RelativeItem()
                   .Text(data.SurveyTitle)
                   .FontSize(20).Bold();
                row.ConstantItem(120).AlignRight()
                   .Text(data.GeneratedAt.ToString("yyyy-MM-dd"));
            });

        page.Content().Column(col => {
            col.Item().Table(table => {
                table.ColumnsDefinition(c => {
                    c.RelativeColumn(5);
                    c.RelativeColumn(1);
                });
                table.Header(h => { /* ... */ });
                foreach (var q in data.QuestionSummaries) {
                    table.Cell().Text(q.Text);
                    table.Cell().Text(q.AverageValue.ToString("F2"));
                }
            });
        });
    });
});
return document.GeneratePdf();`;
</script>

<style scoped src="../../assets/provider-styles.css"></style>
