using jsreport.Shared;
using jsreport.Types;
using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;

namespace ReportApp.WebAPI.Providers;

public class JsReportProvider : IReportProvider, ISupportHtmlTemplate
{
    private readonly IRenderService _jsreport;
    public JsReportProvider(IRenderService jsreport) => _jsreport = jsreport;

    public string Name => "jsreport";

    public async Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
        => await GeneratePdfFromTemplateAsync(data, "<html><body><h1>Standardmall</h1></body></html>");

    public async Task<byte[]> GeneratePdfFromTemplateAsync(ReportDataViewModel data, string htmlTemplate)
    {
        var report = await _jsreport.RenderAsync(new RenderRequest
        {
            Template = new Template
            {
                Content = htmlTemplate,
                Engine = Engine.Handlebars,
                Recipe = Recipe.ChromePdf
            },
            Data = data
        });

        using var ms = new MemoryStream();
        await report.Content.CopyToAsync(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
    {
        var report = await _jsreport.RenderAsync(new RenderRequest
        {
            Template = new Template
            {
                Content = "<table>{{#each QuestionSummaries}}<tr><td>{{Text}}</td><td>{{AverageValue}}</td></tr>{{/each}}</table>",
                Engine = Engine.Handlebars,
                Recipe = Recipe.HtmlToXlsx
            },
            Data = data
        });

        using var ms = new MemoryStream();
        await report.Content.CopyToAsync(ms);
        return ms.ToArray();
    }

    public Task<byte[]> GeneratePptAsync(ReportDataViewModel data)
        => throw new NotImplementedException("jsreport stödjer inte PPT.");
}