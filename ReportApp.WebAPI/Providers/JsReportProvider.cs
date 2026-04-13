using System.Drawing; // Krävs för ColorTranslator
using jsreport.Shared;
using jsreport.Types;
using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;
using ShapeCrawler; // Krävs för Presentation
using ShapeCrawler.Shapes;

namespace ReportApp.WebAPI.Providers;

public class JsReportProvider : IReportProvider, ISupportHtmlTemplate
{
    private readonly IRenderService _jsreport;

    public JsReportProvider(IRenderService jsreport)
    {
        _jsreport = jsreport;
    }

    public string Name => "jsreport";

    // FIX: Implementera saknad metod från ISupportHtmlTemplate
    public async Task<byte[]> GeneratePdfFromTemplateAsync(ReportDataViewModel data, string htmlTemplate)
    {
        var res = await _jsreport.RenderAsync(new RenderRequest
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
        await res.Content.CopyToAsync(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
    {
        // ... (Din befintliga kod för html-strängen)
        var template = "<html>...</html>";

        return await GeneratePdfFromTemplateAsync(data, template);
    }

    public async Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
    {
        var res = await _jsreport.RenderAsync(new RenderRequest
        {
            Template = new Template
            {
                Content = @"<table>...</table>",
                Engine = Engine.Handlebars,
                Recipe = Recipe.HtmlToXlsx
            },
            Data = data
        });

        using var ms = new MemoryStream();
        await res.Content.CopyToAsync(ms);
        return ms.ToArray();
    }

    public Task<byte[]> GeneratePptAsync(ReportDataViewModel data)
    {
        var prs = new ShapeCrawler.Presentation();
        var slide = prs.Slides[0];

        slide.Shapes.AddRectangle(50, 120, 600, 80);
        // FIX: Fullständig namnrymd här med
        var title = slide.Shapes.Last() as ShapeCrawler.IAutoShape;

        if (title != null)
        {
            title.TextFrame.Text = data.SurveyTitle;
            title.Fill.ColorHex = null;
        }

        var file = Path.GetTempFileName() + ".pptx";
        prs.SaveAs(file);
        var bytes = File.ReadAllBytes(file);
        if (File.Exists(file)) File.Delete(file);

        return Task.FromResult(bytes);
    }
}