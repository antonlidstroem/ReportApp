using jsreport.Shared;
using jsreport.Types;
using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;
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

    public async Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
    {
        var template = @"
        <html>
        <head>
        <style>
            body { font-family: Arial; margin: 40px; }
            h1 { color: #a855f7; }
            table { width: 100%; border-collapse: collapse; }
            th, td { border-bottom: 1px solid #ddd; padding: 8px; }
        </style>
        </head>
        <body>
            <h1>{{SurveyTitle}}</h1>
            <p>{{CompanyName}} - {{GeneratedAt}}</p>

            <table>
                <thead>
                    <tr>
                        <th>Fråga</th>
                        <th>Kategori</th>
                        <th>Snitt</th>
                        <th>Svar</th>
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
            </table>
        </body>
        </html>";

        var res = await _jsreport.RenderAsync(new RenderRequest
        {
            Template = new Template
            {
                Content = template,
                Engine = Engine.Handlebars,
                Recipe = Recipe.ChromePdf,
                Chrome = new Chrome
                {
                    PrintBackground = true
                }
            },
            Data = data
        });

        using var ms = new MemoryStream();
        await res.Content.CopyToAsync(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
    {
        var res = await _jsreport.RenderAsync(new RenderRequest
        {
            Template = new Template
            {
                Content = @"<table>
                    {{#each QuestionSummaries}}
                    <tr>
                        <td>{{Text}}</td>
                        <td>{{Category}}</td>
                        <td>{{AverageValue}}</td>
                        <td>{{TotalResponses}}</td>
                    </tr>
                    {{/each}}
                </table>",
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
        var prs = new Presentation();

        var slide = prs.Slides[0];
        slide.Background.Fill.SolidColor.Color = ColorTranslator.FromHtml("#1e1b4b");

        var title = slide.Shapes.AddShape(ShapeType.Rectangle, 50, 120, 600, 80);
        title.TextFrame.Text = data.SurveyTitle;
        title.Fill.FillType = FillType.NoFill;

        var file = Path.GetTempFileName() + ".pptx";
        prs.SaveAs(file);

        return Task.FromResult(File.ReadAllBytes(file));
    }
}