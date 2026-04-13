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
        var res = await _jsreport.RenderAsync(new RenderRequest
        {
            Template = new Template
            {
                Content = "<table><tr><td>{{SurveyTitle}}</td></tr></table>", // Din Excel-mall
                Engine = Engine.Handlebars,
                Recipe = Recipe.HtmlToXlsx
            },
            Data = data
        });

        using var ms = new MemoryStream();
        await res.Content.CopyToAsync(ms);
        return ms.ToArray();
    }
}