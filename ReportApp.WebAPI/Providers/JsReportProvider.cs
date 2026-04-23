using jsreport.Shared;
using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;
using ReportApp.WebAPI.Providers.Generators;

namespace ReportApp.WebAPI.Providers;

public class JsReportProvider : IReportProvider, ISupportHtmlTemplate
{
    private readonly JsReportPdfGenerator _pdfGenerator;
    private readonly JsReportExcelGenerator _excelGenerator;
    private readonly SyncfusionPptGenerator _pptGenerator;

    public JsReportProvider(IRenderService jsreport)
    {
        _pdfGenerator   = new JsReportPdfGenerator(jsreport);
        _excelGenerator = new JsReportExcelGenerator(jsreport);
        _pptGenerator   = new SyncfusionPptGenerator();
    }

    public string Name => "jsreport";

    public Task<byte[]> GeneratePdfFromTemplateAsync(ReportDataViewModel data, string htmlTemplate)
        => _pdfGenerator.GenerateAsync(data, htmlTemplate);

    public Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
    {
        var defaultHtml = @"<!DOCTYPE html>
<html><head><meta charset=""UTF-8"">
<style>
  *{margin:0;padding:0;box-sizing:border-box}
  body{font-family:'Segoe UI',sans-serif;padding:32px;color:#1e293b}
  h1{font-size:24px;font-weight:800;color:#0f2d4a;margin-bottom:4px}
  .sub{color:#64748b;font-size:13px;margin-bottom:24px}
  table{width:100%;border-collapse:collapse;font-size:12px}
  thead{background:#1e3a5f;color:white}
  th{padding:9px 12px;text-align:left}
  td{padding:7px 12px;border-bottom:1px solid #e2e8f0}
  tr:nth-child(even){background:#f8fafc}
</style></head>
<body>
  <h1>{{SurveyTitle}}</h1>
  <div class=""sub"">{{CompanyName}} &middot; Genererad {{GeneratedAt}}</div>
  <table>
    <thead><tr><th>Fråga</th><th>Kategori</th><th>Genomsnitt</th><th>Svar</th></tr></thead>
    <tbody>
      {{#each QuestionSummaries}}
      <tr>
        <td>{{Text}}</td><td>{{Category}}</td>
        <td><strong>{{AverageValue}}</strong></td><td>{{TotalResponses}}</td>
      </tr>
      {{/each}}
    </tbody>
  </table>
</body></html>";
        return _pdfGenerator.GenerateAsync(data, defaultHtml);
    }

    public Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
        => _excelGenerator.GenerateAsync(data);

    public Task<byte[]> GeneratePptAsync(ReportDataViewModel data)
        => _pptGenerator.GenerateAsync(data);
}
