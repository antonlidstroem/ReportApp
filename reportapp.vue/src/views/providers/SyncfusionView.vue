<template>
  <div class="provider-page">
    <div class="provider-hero" style="--hero-color: #007bff">
      <div class="hero-left">
        <router-link to="/" class="back-link">← Dashboard</router-link>
        <div class="track-badge">Spår 4 — Enterprise Components</div>
        <h1>💎 Syncfusion Essential Studio</h1>
        <p>
          En av de mest mogna aktörerna på marknaden. Syncfusion erbjuder separata motorer
          (Essential PDF, XlsIO, Presentation) som jobbar direkt mot filformaten utan behov
          av externa beroenden eller installerad mjukvara.
        </p>
        <div class="hero-chips">
          <span class="chip blue">✓ Native Excel/PPT</span>
          <span class="chip blue">✓ Ingen Office-interop</span>
          <span class="chip yellow">⚠ HTML-mallar kräver plugin</span>
          <span class="chip red">✗ Betallicens</span>
        </div>
      </div>
      <div class="hero-stats">
        <div class="hs"><span class="hs-v">$$</span><span class="hs-l">Licenskostnad</span></div>
        <div class="hs"><span class="hs-v">Native</span><span class="hs-l">Engine</span></div>
        <div class="hs"><span class="hs-v">High</span><span class="hs-l">Performance</span></div>
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
      <h2>Kodexempel: Excel-generering (XlsIO)</h2>
      <div class="code-block">
        <div class="cb-header">
          <span>SyncfusionProvider.cs — ExportToExcel</span>
          <span class="cb-lang">C#</span>
        </div>
        <pre><code>{{ codeExample }}</code></pre>
      </div>
      <div class="code-note">
        Syncfusion är känt för sin <strong>XlsIO</strong>-motor som är extremt snabb på att hantera
        stora datamängder. Till skillnad från HTML-baserade lösningar bygger man här upp
        dokumentet via en strukturerad objektmodell (rader, celler, stilar).
      </div>
    </section>

    <div class="workspace">
      <div class="workspace-left">
        <SurveyPicker />
        <ModuleList />
      </div>
      <div class="workspace-right">
        <TemplateDesigner provider-name="Syncfusion" :supports-html-template="false" />
        <ExportButton provider="Syncfusion" :supports-html-template="false" />
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
    icon: "📗",
    title: "Essential XlsIO",
    desc: "Branschledande prestanda för Excel. Stödjer avancerade pivot-tabeller och villkorsstyrd formatering.",
    badge: "✓ Utmärkt",
    status: "ok",
  },
  {
    icon: "📕",
    title: "Essential PDF",
    desc: "Skapar PDF via objektmodell. Har stöd för HTML-konvertering via QtBinaries/Blink-insticksmodul.",
    badge: "✓ Full",
    status: "ok",
  },
  {
    icon: "📘",
    title: "Presentation",
    desc: "Skapa PowerPoint-slides med full kontroll över former, textrutor och animationer.",
    badge: "✓ Mycket bra",
    status: "ok",
  },
  {
    icon: "🏗️",
    title: "Arkitektur",
    desc: "Helt skriven i C#. Inga beroenden av COM-objekt eller installerad Microsoft Office.",
    badge: "✓ Native",
    status: "ok",
  },
  {
    icon: "📊",
    title: "Grafer",
    desc: "Stödjer skapande av riktiga, redigerbara grafer inuti Excel och PowerPoint.",
    badge: "✓ Ja",
    status: "ok",
  },
  {
    icon: "📜",
    title: "Licens",
    desc: "Kommersiell. Syncfusion erbjuder dock en 'Community License' för små bolag/individer.",
    badge: "Villkorad",
    status: "warn",
  },
];

const codeExample = `using (ExcelEngine excelEngine = new ExcelEngine())
{
    IApplication application = excelEngine.Excel;
    IWorkbook workbook = application.Workbooks.Create(1);
    IWorksheet worksheet = workbook.Worksheets[0];

    // Lägg till data från vår survey
    worksheet.Range["A1"].Text = data.SurveyTitle;
    worksheet.Range["A1"].CellStyle.Font.Bold = true;

    int rowIndex = 3;
    foreach (var q in data.QuestionSummaries)
    {
        worksheet.Range[rowIndex, 1].Text = q.Text;
        worksheet.Range[rowIndex, 2].Number = q.AverageValue;
        rowIndex++;
    }

    MemoryStream stream = new MemoryStream();
    workbook.SaveAs(stream);
    return stream;
}`;
</script>

<style scoped src="../../assets/provider-styles.css"></style>
