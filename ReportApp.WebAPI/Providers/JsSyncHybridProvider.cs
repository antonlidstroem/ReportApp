using jsreport.Shared;
using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;
using ReportApp.WebAPI.Providers.Generators;

namespace ReportApp.WebAPI.Providers;

public class JsSyncHybridProvider : IReportProvider, ISupportHtmlTemplate
{
    private readonly JsReportPdfGenerator _pdf;
    private readonly SyncfusionExcelGenerator _excel;
    private readonly SyncfusionPptGenerator _ppt;

    public JsSyncHybridProvider(IRenderService jsreport)
    {
        _pdf   = new JsReportPdfGenerator(jsreport);
        _excel = new SyncfusionExcelGenerator();
        _ppt   = new SyncfusionPptGenerator();
    }

    public string Name => "js-sync";

    public Task<byte[]> GeneratePdfAsync(ReportDataViewModel data) =>
        _pdf.GenerateAsync(data, "<html><body><h1>{{SurveyTitle}}</h1></body></html>");

    public Task<byte[]> GeneratePdfFromTemplateAsync(ReportDataViewModel data, string htmlTemplate) =>
        _pdf.GenerateAsync(data, htmlTemplate);

    public Task<byte[]> GenerateExcelAsync(ReportDataViewModel data) => _excel.GenerateAsync(data);
    public Task<byte[]> GeneratePptAsync(ReportDataViewModel data)   => _ppt.GenerateAsync(data);
}
