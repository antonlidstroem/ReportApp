// ┌─────────────────────────────────────────────────────────────────────────┐
// │  IronSuiteProvider — STUB                                               │
// │                                                                         │
// │  To activate:                                                           │
// │  1. Install NuGet packages:                                             │
// │       IronPdf  IronXL  IronPPT                                         │
// │  2. Set your license key in appsettings.json:                          │
// │       "IronSoftware": { "LicenseKey": "YOUR-KEY-HERE" }                │
// │  3. Uncomment the implementation below and remove the stub exceptions.  │
// │  4. Register in Program.cs:                                             │
// │       builder.Services.AddScoped<IReportProvider, IronSuiteProvider>() │
// └─────────────────────────────────────────────────────────────────────────┘

using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;

namespace ReportApp.WebAPI.Providers;

/// <summary>
/// Track 1: IronSuite enterprise provider.
/// Supports HTML-to-PDF via Chromium (ISupportHtmlTemplate).
/// </summary>
public class IronSuiteProvider : IReportProvider, ISupportHtmlTemplate
{
    public string Name => "IronSuite";

    // ── PDF ──────────────────────────────────────────────────────────────────

    public Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
    {
        // TODO: Implement with IronPDF
        // Example:
        //   var renderer = new ChromePdfRenderer();
        //   var html = BuildDefaultHtml(data);
        //   var pdf = await renderer.RenderHtmlAsPdfAsync(html);
        //   return pdf.BinaryData;

        throw new NotImplementedException(
            "IronSuiteProvider.GeneratePdfAsync: Install IronPDF and add license key.");
    }

    public Task<byte[]> GeneratePdfFromTemplateAsync(ReportDataViewModel data, string htmlTemplate)
    {
        // TODO: Resolve template vars then render via IronPDF
        // var resolved = TemplateResolver.Resolve(htmlTemplate, data);
        // var renderer = new ChromePdfRenderer();
        // var pdf = await renderer.RenderHtmlAsPdfAsync(resolved);
        // return pdf.BinaryData;

        throw new NotImplementedException(
            "IronSuiteProvider.GeneratePdfFromTemplateAsync: Install IronPDF and add license key.");
    }

    // ── Excel ─────────────────────────────────────────────────────────────────

    public Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
    {
        // TODO: Implement with IronXL
        // var workbook = WorkBook.Create(ExcelFileFormat.XLSX);
        // var sheet = workbook.CreateWorkSheet("Analys");
        // ...
        // return workbook.ToByteArray();

        throw new NotImplementedException(
            "IronSuiteProvider.GenerateExcelAsync: Install IronXL and add license key.");
    }

    // ── PowerPoint ────────────────────────────────────────────────────────────

    public Task<byte[]> GeneratePptAsync(ReportDataViewModel data)
    {
        // TODO: Implement with IronPPT
        // var pres = new IronPowerPoint.Presentation();
        // var slide = pres.Slides.AddSlide();
        // slide.Title.Text = data.SurveyTitle;
        // ...
        // using var ms = new MemoryStream();
        // pres.Save(ms);
        // return ms.ToArray();

        throw new NotImplementedException(
            "IronSuiteProvider.GeneratePptAsync: Install IronPPT and add license key.");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string BuildDefaultHtml(ReportDataViewModel data)
    {
        var rows = string.Join("", data.QuestionSummaries.Select(q =>
            $"<tr><td>{q.Text}</td><td>{q.Category}</td><td>{q.AverageValue:F2}</td><td>{q.TotalResponses}</td></tr>"));

        return $"""
            <!DOCTYPE html>
            <html>
            <head>
              <meta charset="UTF-8">
              <style>
                body {{ font-family: 'Segoe UI', sans-serif; padding: 32px; }}
                h1   {{ color: #1e3a5f; }}
                table {{ width: 100%; border-collapse: collapse; }}
                th {{ background: #1e3a5f; color: white; padding: 8px 12px; text-align: left; }}
                td {{ padding: 7px 12px; border-bottom: 1px solid #e8edf5; }}
              </style>
            </head>
            <body>
              <h1>{data.SurveyTitle}</h1>
              <p>{data.CompanyName} · {data.GeneratedAt:yyyy-MM-dd}</p>
              <table>
                <thead><tr><th>Fråga</th><th>Kategori</th><th>Snitt</th><th>Svar</th></tr></thead>
                <tbody>{rows}</tbody>
              </table>
            </body>
            </html>
            """;
    }
}
