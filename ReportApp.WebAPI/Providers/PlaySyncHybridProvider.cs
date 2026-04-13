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
        await GeneratePdfFromTemplateAsync(data, "<html><body><h1>Default Playwright Render</h1></body></html>");

    public async Task<byte[]> GeneratePdfFromTemplateAsync(ReportDataViewModel data, string htmlTemplate)
    {
        // Simple manual replacement since Playwright doesn't have a built-in Handlebars engine
        var html = htmlTemplate.Replace("{{SurveyTitle}}", data.SurveyTitle);
        return await _playwright.GeneratePdfAsync(html);
    }

    public Task<byte[]> GenerateExcelAsync(ReportDataViewModel data) => _excel.GenerateAsync(data);
    public Task<byte[]> GeneratePptAsync(ReportDataViewModel data) => _ppt.GenerateAsync(data);
}