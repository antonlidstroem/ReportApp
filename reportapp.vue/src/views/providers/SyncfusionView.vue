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


  <section class="section sandbox">
    <h2>🛠️ Enterprise Feature Sandbox</h2>
    <p class="mb-3">Aktivera avancerade Syncfusion-funktioner för att se hur de skiljer sig från de andra spåren.</p>

    <div class="sandbox-grid">
      <div class="sf-feature-card" :class="{ active: sfConfig.useFormulas }">
        <div class="sf-feature-header">
          <input type="checkbox" v-model="sfConfig.useFormulas" id="f-formulas">
          <label for="f-formulas">Live Excel Formulas</label>
        </div>
        <p>Istället för att bara skriva '4.2', skriver vi <code>=AVERAGE(B2:B10)</code>. Ändra data i Excel och grafen följer med!</p>
      </div>

      <div class="sf-feature-card" :class="{ active: sfConfig.encrypt }">
        <div class="sf-feature-header">
          <input type="checkbox" v-model="sfConfig.encrypt" id="f-encrypt">
          <label for="f-encrypt">PDF Encryption (AES-256)</label>
        </div>
        <p>Lösenordsskydda rapporten direkt vid generering. (Lösenord: <code>1234</code>)</p>
      </div>

      <div class="sf-feature-card" :class="{ active: sfConfig.nativeCharts }">
        <div class="sf-feature-header">
          <input type="checkbox" v-model="sfConfig.nativeCharts" id="f-charts">
          <label for="f-charts">Native Editable Charts</label>
        </div>
        <p>Exportera grafer som PowerPoint-objekt istället för bilder. Går att redigera i Office-paketet.</p>
      </div>
    </div>
  </section>




</template>

<script setup lang="ts">
import SurveyPicker from "../../components/SurveyPicker.vue";
import ModuleList from "../../components/ModuleList.vue";
import TemplateDesigner from "../../components/TemplateDesigner.vue";
  import ExportButton from "../../components/ExportButton.vue";
  import { ref } from 'vue'

  const capabilities = [
    {
      icon: "🧮",
      title: "Excel Formula Engine",
      desc: "400+ built-in functions. The report calculates itself without backend logic.",
      badge: "✓ Unique",
      status: "ok",
    },
    {
      icon: "📊",
      title: "Native Office Charts",
      desc: "Genererar 'riktiga' grafer i Excel/PPT som användaren kan redigera efteråt.",
      badge: "✓ Best in Class",
      status: "ok",
    },
    {
      icon: "🔐",
      title: "Digital Signatures",
      desc: "Stöd för PAdES-standard och tidsstämplar för juridiskt bindande rapporter.",
      badge: "✓ Enterprise",
      status: "ok",
    },
    {
      icon: "♿",
      title: "Accessibility (PDF/UA)",
      desc: "Skapar automatiskt taggade PDF:er som fungerar med skärmläsare.",
      badge: "✓ Compliant",
      status: "ok",
    },
    {
      icon: "⚡",
      title: "Template Markers",
      desc: "Använd befintliga Excel-filer som mallar och fyll dem med data blixtsnabbt.",
      badge: "✓ Time Saver",
      status: "ok",
    },
    {
      icon: "🆓",
      title: "Community License",
      desc: "Gratis för småbolag (<$1M omsättning). En enorm fördel mot IronSuite.",
      badge: "✓ Generous",
      status: "ok",
    }
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


  const sfConfig = ref({
    useFormulas: true,
    encrypt: false,
    nativeCharts: true
  })
</script>

<style scoped src="../../assets/provider-styles.css"></style>
