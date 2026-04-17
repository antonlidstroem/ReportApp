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
        var template = @"
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
      <td>{{AverageValue}}</td>
      <td>{{TotalResponses}}</td>
    </tr>
    {{/each}}
  </tbody>
</table>";

        var res = await _jsreport.RenderAsync(new RenderRequest
        {
            Template = new Template
            {
                Content = template,
                Engine = Engine.Handlebars,
                Recipe = Recipe.HtmlToXlsx,
                //HtmlToXlsx = new HtmlToXlsx
                //{
                //    SheetName = data.SurveyTitle.Length > 31
                //        ? data.SurveyTitle[..31]
                //        : data.SurveyTitle
                //}
            },
            Data = data
        });

        using var ms = new MemoryStream();
        await res.Content.CopyToAsync(ms);
        return ms.ToArray();
    }
}
