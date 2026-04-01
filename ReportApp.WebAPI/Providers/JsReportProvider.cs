// ┌─────────────────────────────────────────────────────────────────────────┐
// │  JsReportProvider — STUB                                                │
// │                                                                         │
// │  To activate:                                                           │
// │  1. Install NuGet: jsreport.Local  jsreport.Binary.Linux (or .Win)     │
// │  2. Register jsreport in Program.cs (see comment below)                 │
// │  3. Uncomment the implementation and remove stub exceptions.            │
// └─────────────────────────────────────────────────────────────────────────┘

// Program.cs additions needed:
//   using jsreport.Local;
//   using jsreport.Types;
//   builder.Services.AddSingleton<IRenderService>(
//       new LocalReporting().UseBinary(JsReportBinary.GetInstance()).AsUtility().Create());
//   builder.Services.AddScoped<IReportProvider, JsReportProvider>();

using System.Text.Json;
using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;

namespace ReportApp.WebAPI.Providers;

/// <summary>
/// Track 3: jsreport web-standard engine.
/// Uses Handlebars templates rendered by Chromium via jsreport.Local.
/// Supports HTML-to-PDF (ISupportHtmlTemplate).
/// NOTE: No native PPT support — throws NotImplementedException.
/// </summary>
public class JsReportProvider : IReportProvider, ISupportHtmlTemplate
{
    // private readonly IRenderService _jsreport;
    // public JsReportProvider(IRenderService jsreport) { _jsreport = jsreport; }

    public string Name => "JsReport";

    // ── PDF ──────────────────────────────────────────────────────────────────

    public Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
    {
        // var html  = await File.ReadAllTextAsync("Templates/default-report.html");
        // return await RenderHtmlToPdfAsync(html, data);
        throw new NotImplementedException("Install jsreport.Local NuGet package.");
    }

    public Task<byte[]> GeneratePdfFromTemplateAsync(ReportDataViewModel data, string htmlTemplate)
    {
        // return await RenderHtmlToPdfAsync(htmlTemplate, data);
        throw new NotImplementedException("Install jsreport.Local NuGet package.");
    }

    // ── Excel ─────────────────────────────────────────────────────────────────

    public Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
    {
        // jsreport xlsx recipe — basic table export
        throw new NotImplementedException("Install jsreport.Local NuGet package.");
    }

    // ── PPT ───────────────────────────────────────────────────────────────────

    public Task<byte[]> GeneratePptAsync(ReportDataViewModel data)
    {
        // jsreport has no native PPT recipe.
        // Possible workaround: export a multi-page PDF with slide-like layouts.
        throw new NotImplementedException(
            "JsReport does not support PowerPoint natively. " +
            "Consider exporting slide-layout PDFs as an alternative.");
    }

    // ── Core rendering helper ─────────────────────────────────────────────────

    private async Task<byte[]> RenderHtmlToPdfAsync(string htmlTemplate, ReportDataViewModel data)
    {
        // Step 1: Build data object for Handlebars
        var templateData = new
        {
            surveyTitle   = data.SurveyTitle,
            companyName   = data.CompanyName,
            generatedAt   = data.GeneratedAt.ToString("yyyy-MM-dd"),
            startDate     = data.StartDate?.ToString("yyyy-MM-dd"),
            endDate       = data.EndDate?.ToString("yyyy-MM-dd"),
            questionCount = data.QuestionSummaries.Count,
            totalResponses = data.QuestionSummaries.Sum(q => q.TotalResponses),
            overallAverage = data.QuestionSummaries.Any()
                ? data.QuestionSummaries.Average(q => q.AverageValue).ToString("F2") : "–",
            questions     = data.QuestionSummaries.Select(q => new {
                text          = q.Text,
                category      = q.Category,
                averageValue  = q.AverageValue.ToString("F2"),
                totalResponses = q.TotalResponses
            }),
            trendLabels   = data.Trends.Select(t => $"{t.MonthName} {t.Year}").ToList(),
            trendValues   = data.Trends.Select(t => t.AverageValue).ToList(),
        };

        // Step 2: Render via jsreport
        // using var result = await _jsreport.RenderAsync(new RenderRequest {
        //     Template = new Template {
        //         Content = htmlTemplate,
        //         Engine  = Engine.Handlebars,
        //         Recipe  = Recipe.ChromePdf,
        //         Chrome  = new Chrome {
        //             MarginTop    = "15mm",
        //             MarginBottom = "15mm",
        //             MarginLeft   = "10mm",
        //             MarginRight  = "10mm",
        //         }
        //     },
        //     Data = templateData
        // });
        // using var ms = new MemoryStream();
        // await result.Content.CopyToAsync(ms);
        // return ms.ToArray();

        throw new NotImplementedException("Install jsreport.Local NuGet package.");
    }
}
