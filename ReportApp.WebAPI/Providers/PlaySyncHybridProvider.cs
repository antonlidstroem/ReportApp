using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;
using ReportApp.WebAPI.Providers.Generators;
using ReportApp.Backend.Providers;

namespace ReportApp.WebAPI.Providers;

public class PlaySyncHybridProvider : IReportProvider, ISupportHtmlTemplate
{
    private readonly PlaywrightProvider _playwright;
    private readonly SyncfusionExcelGenerator _excel;
    private readonly SyncfusionPptGenerator _ppt;

    public PlaySyncHybridProvider(PlaywrightProvider playwright)
    {
        _playwright = playwright;
        _excel = new SyncfusionExcelGenerator();
        _ppt = new SyncfusionPptGenerator();
    }

    public string Name => "play-sync";

    public async Task<byte[]> GeneratePdfAsync(ReportDataViewModel data) =>
        await GeneratePdfFromTemplateAsync(data, BuildDefaultTemplate(data));

    public async Task<byte[]> GeneratePdfFromTemplateAsync(ReportDataViewModel data, string htmlTemplate)
    {
        var html = ReplaceTokens(htmlTemplate, data);
        return await _playwright.GeneratePdfAsync(html);
    }

    public Task<byte[]> GenerateExcelAsync(ReportDataViewModel data) => _excel.GenerateAsync(data);
    public Task<byte[]> GeneratePptAsync(ReportDataViewModel data) => _ppt.GenerateAsync(data);

    /// <summary>
    /// Replaces all supported {{Token}} placeholders.
    /// Playwright does NOT support Handlebars loops/conditionals — only flat tokens.
    /// </summary>
    private static string ReplaceTokens(string template, ReportDataViewModel data)
    {
        return template
            .Replace("{{SurveyTitle}}", data.SurveyTitle)
            .Replace("{{CompanyName}}", data.CompanyName)
            .Replace("{{GeneratedAt}}", data.GeneratedAt.ToString("yyyy-MM-dd"))
            .Replace("{{StartDate}}", data.StartDate?.ToString("yyyy-MM-dd") ?? "")
            .Replace("{{EndDate}}", data.EndDate?.ToString("yyyy-MM-dd") ?? "")
            .Replace("{{QuestionCount}}", data.QuestionCount.ToString())
            .Replace("{{TotalResponses}}", data.TotalResponses.ToString())
            .Replace("{{OverallAverage}}", data.OverallAverage)
            .Replace("{{CategoryCount}}", data.CategoryCount.ToString())
            // Pre-built HTML table injected as a single token
            .Replace("{{QuestionsTableHtml}}", data.QuestionsTableHtml);
    }

    private static string BuildDefaultTemplate(ReportDataViewModel data)
    {
        return $@"<!DOCTYPE html>
<html><head><meta charset=""UTF-8"">
<style>
  *{{margin:0;padding:0;box-sizing:border-box}}
  body{{font-family:'Segoe UI',sans-serif;padding:40px;color:#1e293b}}
  h1{{font-size:26px;font-weight:800;color:#0f2d4a;margin-bottom:4px}}
  .sub{{color:#64748b;font-size:14px;margin-bottom:6px}}
  .meta{{font-size:11px;color:#94a3b8;margin-bottom:30px}}
  .notice{{padding:13px;background:#fffbeb;border:1px solid #fcd34d;border-radius:7px;font-size:11px;color:#92400e;margin-top:20px}}
</style></head>
<body>
  <h1>{{{{SurveyTitle}}}}</h1>
  <div class=""sub"">{{{{CompanyName}}}}</div>
  <div class=""meta"">Genererad: {{{{GeneratedAt}}}}</div>
  {{{{QuestionsTableHtml}}}}
  <div class=""notice"">ℹ Genererad av Playwright (token-ersättning). Inga Handlebars-loopar eller Chart.js — använd jsreport (Hybrid A) för det.</div>
</body></html>"
        .Replace("{{{{SurveyTitle}}}}", data.SurveyTitle)
        .Replace("{{{{CompanyName}}}}", data.CompanyName)
        .Replace("{{{{GeneratedAt}}}}", data.GeneratedAt.ToString("yyyy-MM-dd"))
        .Replace("{{{{QuestionsTableHtml}}}}", data.QuestionsTableHtml);
    }
}
