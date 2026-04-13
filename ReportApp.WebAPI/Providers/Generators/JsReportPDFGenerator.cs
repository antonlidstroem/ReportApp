using jsreport.Shared;
using jsreport.Types;
using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Providers.Generators;

public class JsReportPdfGenerator
{
    private readonly IRenderService _jsreport;

    public JsReportPdfGenerator(IRenderService jsreport)
    {
        _jsreport = jsreport;
    }

    public async Task<byte[]> GenerateAsync(ReportDataViewModel data, string htmlTemplate)
    {
        var res = await _jsreport.RenderAsync(new RenderRequest
        {
            Template = new Template
            {
                Content = htmlTemplate,
                Engine = Engine.Handlebars,
                Recipe = Recipe.ChromePdf,
                Chrome = new Chrome { PrintBackground = true }
            },
            Data = data
        });

        using var ms = new MemoryStream();
        await res.Content.CopyToAsync(ms);
        return ms.ToArray();
    }
}