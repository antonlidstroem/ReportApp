<template>
  <div class="provider-page">
    <!-- Hero -->
    <div class="provider-hero" style="--hero-color: #22c55e">
      <div class="hero-left">
        <router-link to="/" class="back-link">← Dashboard</router-link>
        <div class="track-badge">Spår 2 — Best-of-Breed Open Source</div>
        <h1>🏗️ QuestPDF + ClosedXML + ShapeCrawler</h1>
        <p>Kod-centrerad approach med tre dedikerade open source-bibliotek. PDF byggs i C# via ett Fluent API, Excel via ClosedXML och PPT via ShapeCrawler.</p>
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
        QuestPDF använder ett Fluent C# API — allt definieras i kod. Det ger full kontroll men innebär
        att HTML-mallar från designern inte kan användas direkt. Istället fungerar "mallar" som förkonfigurerade
        layoutval (vilka sektioner som inkluderas, färgschema etc.).
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
        <TemplateDesigner
          provider-name="QuestOpenSource"
          :supports-html-template="false"
        />
        <ExportButton provider="QuestOpenSource" :supports-html-template="false" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import SurveyPicker    from '../../components/SurveyPicker.vue'
import ModuleList      from '../../components/ModuleList.vue'
import TemplateDesigner from '../../components/TemplateDesigner.vue'
import ExportButton    from '../../components/ExportButton.vue'

const capabilities = [
  { icon: '📕', title: 'PDF (QuestPDF)',     desc: 'Fullständigt stöd via Fluent C# API. Tabeller, header/footer, sidnumrering, bilder.', badge: '✓ Full', status: 'ok' },
  { icon: '📗', title: 'Excel (ClosedXML)',   desc: 'Workbooks med flera ark, formatering, formler, cell-styles. Inga native charts ännu.', badge: '✓ Full', status: 'ok' },
  { icon: '📘', title: 'PPT (ShapeCrawler)', desc: 'Kan skapa och modifiera presentationer. Begränsat grafstöd i nuläget.', badge: '⚠ Basic', status: 'warn' },
  { icon: '🎨', title: 'HTML-mallar',         desc: 'Stöds INTE — QuestPDF är kod-first. Mallsystemet erbjuder i stället konfigurerbara layoutpresets.', badge: '✗ Nej', status: 'no' },
  { icon: '📈', title: 'Grafer i PDF',        desc: 'Kan renderas som SVG eller bitmap om data förbereds i backend innan generering.', badge: '⚠ Manuell', status: 'warn' },
  { icon: '💰', title: 'Kostnad',             desc: 'Gratis (Community-licens) för projekt med <1M USD intäkt. Professional kostar.', badge: '✓ Gratis', status: 'ok' },
]

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
return document.GeneratePdf();`
</script>

<style scoped>
.provider-page { max-width: 1200px; margin: 0 auto; }

.provider-hero {
  background: linear-gradient(135deg, #0a1628, #0f2d1a);
  border: 1px solid color-mix(in srgb, var(--hero-color) 30%, transparent);
  border-radius: 16px;
  padding: 40px;
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 32px;
  margin-bottom: 40px;
}

.back-link { font-size: 12px; color: rgba(255,255,255,0.5); text-decoration: none; display: block; margin-bottom: 12px; }
.back-link:hover { color: white; }

.track-badge {
  font-size: 10px;
  text-transform: uppercase;
  letter-spacing: 2px;
  color: var(--hero-color);
  margin-bottom: 10px;
}

.provider-hero h1 { font-size: 26px; font-weight: 700; margin: 0 0 12px; }
.provider-hero p  { font-size: 14px; color: rgba(255,255,255,0.7); margin: 0 0 16px; max-width: 560px; line-height: 1.6; }

.hero-chips { display: flex; flex-wrap: wrap; gap: 8px; }
.chip {
  font-size: 11px;
  padding: 4px 12px;
  border-radius: 100px;
  border: 1px solid;
}
.chip.green  { color: #22c55e; border-color: rgba(34,197,94,0.3); background: rgba(34,197,94,0.08); }
.chip.yellow { color: #f59e0b; border-color: rgba(245,158,11,0.3); background: rgba(245,158,11,0.08); }
.chip.red    { color: #ef4444; border-color: rgba(239,68,68,0.3);  background: rgba(239,68,68,0.08); }

.hero-stats { display: flex; flex-direction: column; gap: 16px; min-width: 140px; }
.hs { display: flex; flex-direction: column; align-items: center; background: rgba(255,255,255,0.06); border-radius: 10px; padding: 16px; border: 1px solid rgba(255,255,255,0.08); }
.hs-v { font-size: 24px; font-weight: 700; color: var(--hero-color); }
.hs-l { font-size: 10px; color: rgba(255,255,255,0.5); margin-top: 4px; text-transform: uppercase; letter-spacing: 0.5px; }

.section { margin-bottom: 36px; }
.section h2 { font-size: 18px; font-weight: 700; margin: 0 0 16px; color: var(--text); }

.capabilities { display: grid; grid-template-columns: repeat(3, 1fr); gap: 12px; }
.cap-card {
  display: flex;
  flex-direction: column;
  gap: 8px;
  padding: 16px;
  border-radius: 10px;
  border: 1px solid var(--border);
  background: var(--surface);
}
.cap-card.ok   { border-color: rgba(34,197,94,0.3); }
.cap-card.warn { border-color: rgba(245,158,11,0.3); }
.cap-card.no   { border-color: rgba(239,68,68,0.25); opacity: 0.7; }

.cap-icon { font-size: 22px; }
.cap-title { font-size: 13px; font-weight: 600; }
.cap-desc  { font-size: 11px; color: var(--text-muted); line-height: 1.5; margin-top: 2px; }
.cap-badge { font-size: 11px; font-weight: 600; }
.ok .cap-badge   { color: #22c55e; }
.warn .cap-badge { color: #f59e0b; }
.no .cap-badge   { color: #ef4444; }

.code-block {
  background: #1e1e1e;
  border-radius: 10px;
  overflow: hidden;
  margin-bottom: 12px;
}
.cb-header {
  display: flex;
  justify-content: space-between;
  padding: 10px 16px;
  background: #2d2d2d;
  font-size: 12px;
  color: #aaa;
  border-bottom: 1px solid #333;
}
.cb-lang { color: #4ec9b0; font-weight: 600; }
.code-block pre { margin: 0; padding: 16px; overflow-x: auto; }
.code-block code {
  font-family: 'Courier New', monospace;
  font-size: 12px;
  line-height: 1.7;
  color: #d4d4d4;
  white-space: pre;
}

.code-note {
  font-size: 13px;
  color: var(--text-muted);
  padding: 12px 16px;
  background: var(--surface-2);
  border-radius: 8px;
  border-left: 3px solid #f59e0b;
  line-height: 1.6;
}

.workspace {
  display: grid;
  grid-template-columns: 340px 1fr;
  gap: 20px;
  margin-bottom: 40px;
}
.workspace-left  { display: flex; flex-direction: column; gap: 16px; }
.workspace-right { display: flex; flex-direction: column; gap: 16px; }
</style>
