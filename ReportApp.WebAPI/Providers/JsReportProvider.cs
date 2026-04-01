using jsreport.Shared;
using jsreport.Types;
using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;
using System.Diagnostics;

namespace ReportApp.WebAPI.Providers;

public class JsReportProvider : IReportProvider, ISupportHtmlTemplate
{
    private readonly IRenderService _jsreport;
    public JsReportProvider(IRenderService jsreport) => _jsreport = jsreport;

    public string Name => "jsreport";

    public async Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
    {
        // Nu lägger vi till en riktig Handlebars-loop i standardmallen
        var defaultTemplate = @"
        <html>
            <head>
                <style>
                    body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 40px; color: #333; }
                    h1 { color: #a855f7; border-bottom: 2px solid #f3e8ff; padding-bottom: 10px; }
                    table { width: 100%; border-collapse: collapse; margin-top: 20px; }
                    th { text-align: left; background-color: #faf5ff; border-bottom: 2px solid #a855f7; padding: 10px; }
                    td { border-bottom: 1px solid #eee; padding: 10px; }
                    .footer { margin-top: 30px; font-size: 0.8em; color: #999; }
                </style>
            </head>
            <body>
                <h1>{{SurveyTitle}}</h1>
                <p>Här är sammanställningen av enkäten.</p>
                
                <table>
                    <thead>
                        <tr>
                            <th>Fråga</th>
                            <th style='width: 80px;'>Snitt</th>
                        </tr>
                    </thead>
                    <tbody>
                        {{#each QuestionSummaries}}
                        <tr>
                            <td>{{Text}}</td>
                            <td><strong>{{AverageValue}}</strong></td>
                        </tr>
                        {{/each}}
                    </tbody>
                </table>

                <div class='footer'>Genererad via jsreport (Chromium)</div>
            </body>
        </html>";

        return await GeneratePdfFromTemplateAsync(data, defaultTemplate);
    }

    public async Task<byte[]> GeneratePdfFromTemplateAsync(ReportDataViewModel data, string htmlTemplate)
    {
        Console.WriteLine(">>> [JSREPORT] Anrop startat. Förbereder rendering...");

        try
        {
            // Vi sätter en stenhård timeout på 60 sekunder
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

            var report = await _jsreport.RenderAsync(new RenderRequest
            {
                Template = new Template
                {
                    Content = htmlTemplate,
                    Engine = Engine.Handlebars,
                    Recipe = Recipe.ChromePdf,
                    Chrome = new Chrome { PrintBackground = true }
                },
                Data = data
            }, cts.Token);

            Console.WriteLine(">>> [JSREPORT] Rendering klar. Läser stream...");

            using var ms = new MemoryStream();
            await report.Content.CopyToAsync(ms);

            byte[] result = ms.ToArray();
            Console.WriteLine($">>> [JSREPORT] Klar! Genererade {result.Length} bytes.");

            return result;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine(">>> [JSREPORT] FEL: Timeout efter 60 sekunder. Chromium svarade inte.");
            throw new Exception("jsreport timeout. Chromium-processen hängde sig.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($">>> [JSREPORT] KRASCH: {ex.Message}");
            if (ex.InnerException != null) Console.WriteLine($">>> INNER: {ex.InnerException.Message}");
            throw;
        }
    }

    public async Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
    {
        // Samma logik här om du vill testa Excel
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
        => throw new NotSupportedException("jsreport stödjer inte PPT.");
}