using jsreport.Shared;
using jsreport.Types;
using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Providers.Generators;

public class JsReportExcelGenerator
{
    private readonly IRenderService _jsreport;

    public JsReportExcelGenerator(IRenderService jsreport)
    {
        _jsreport = jsreport;
    }

    public async Task<byte[]> GenerateAsync(ReportDataViewModel data)
    {
        // html-to-xlsx recipe renders the HTML table as an Excel sheet.
        // HtmlToXlsx in jsreport.Types v3 only has: Enabled (bool?)
        // SheetName is NOT a property and causes a compile error — omit it.
        var template = @"
<!DOCTYPE html>
<html><head>
<meta charset=""UTF-8"">
<style>
  body { font-family: Arial, sans-serif; }
  table { border-collapse: collapse; width: 100%; }
  th { background: #1e3a5f; color: white; padding: 8px 12px; text-align: left; font-weight: bold; }
  td { padding: 7px 12px; border-bottom: 1px solid #e2e8f0; }
  tr:nth-child(even) td { background: #f8fafc; }
  .num { text-align: right; font-weight: bold; }
  .high { color: #16a34a; }
  .mid  { color: #d97706; }
  .low  { color: #dc2626; }
</style>
</head>
<body>
<h2>{{SurveyTitle}}</h2>
<p>{{CompanyName}} &middot; Genererad {{GeneratedAt}}</p>
<table>
  <thead>
    <tr>
      <th>Fråga</th>
      <th>Kategori</th>
      <th>Genomsnitt</th>
      <th>Antal svar</th>
    </tr>
  </thead>
  <tbody>
    {{#each QuestionSummaries}}
    <tr>
      <td>{{Text}}</td>
      <td>{{Category}}</td>
      <td class=""num {{#if (gte AverageValue 4)}}high{{else}}{{#if (gte AverageValue 3)}}mid{{else}}low{{/if}}{{/if}}"">{{AverageValue}}</td>
      <td class=""num"">{{TotalResponses}}</td>
    </tr>
    {{/each}}
  </tbody>
</table>
</body></html>";

        var helpersJs = """
function gt(a, b)  { return Number(a) > Number(b);  }
function lt(a, b)  { return Number(a) < Number(b);  }
function gte(a, b) { return Number(a) >= Number(b); }
function lte(a, b) { return Number(a) <= Number(b); }
function eq(a, b)  { return String(a) === String(b); }
""";

        var res = await _jsreport.RenderAsync(new RenderRequest
        {
            Template = new Template
            {
                Content  = template,
                Engine   = Engine.Handlebars,
                Recipe   = Recipe.HtmlToXlsx,
                Helpers  = helpersJs,
                // NOTE: HtmlToXlsx only has Enabled (bool?) in jsreport.Types v3.8.1
                // SheetName is NOT a property — do NOT add it.
                HtmlToXlsx = new HtmlToXlsx { }
            },
            Data = data
        });

        using var ms = new MemoryStream();
        await res.Content.CopyToAsync(ms);
        return ms.ToArray();
    }
}
