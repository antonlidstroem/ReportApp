using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;
using ReportApp.WebAPI.Providers.Generators;
using ReportApp.Backend.Providers; // Se till att din PlaywrightProvider ligger här

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

    public string Name => "play-sync"; // Matchar Vue-sidans namn

    public async Task<byte[]> GeneratePdfAsync(ReportDataViewModel data) =>
        await GeneratePdfFromTemplateAsync(data, "<html><body><h1>Default Playwright</h1></body></html>");

    public async Task<byte[]> GeneratePdfFromTemplateAsync(ReportDataViewModel data, string htmlTemplate)
    {
        // Enkel token-ersättning (eller använd Handlebars.Net om du vill vara avancerad)
        var html = htmlTemplate.Replace("{{SurveyTitle}}", data.SurveyTitle);
        return await _playwright.GeneratePdfAsync(html);
    }

    public Task<byte[]> GenerateExcelAsync(ReportDataViewModel data) => _excel.GenerateAsync(data);
    public Task<byte[]> GeneratePptAsync(ReportDataViewModel data) => _ppt.GenerateAsync(data);
}