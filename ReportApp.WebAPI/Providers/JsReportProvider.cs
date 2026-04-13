using jsreport.Shared;
using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;
using ReportApp.WebAPI.Providers.Generators;

namespace ReportApp.WebAPI.Providers;

public class JsReportProvider : IReportProvider, ISupportHtmlTemplate
{
    private readonly IRenderService _jsreport;
    private readonly JsReportPdfGenerator _pdfGenerator;
    private readonly JsReportExcelGenerator _excelGenerator;
    private readonly SyncfusionPptGenerator _pptGenerator; // Tillagd

    public JsReportProvider(IRenderService jsreport)
    {
        _jsreport = jsreport;
        _pdfGenerator = new JsReportPdfGenerator(_jsreport);
        _excelGenerator = new JsReportExcelGenerator(_jsreport);
        _pptGenerator = new SyncfusionPptGenerator(); 
    }

    public string Name => "jsreport";

    public async Task<byte[]> GeneratePdfFromTemplateAsync(ReportDataViewModel data, string htmlTemplate)
        => await _pdfGenerator.GenerateAsync(data, htmlTemplate);

    public async Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
    {
        var defaultHtml = "<html><body><h1>{{SurveyTitle}}</h1></body></html>";
        return await _pdfGenerator.GenerateAsync(data, defaultHtml);
    }

    public async Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
        => await _excelGenerator.GenerateAsync(data);

    public async Task<byte[]> GeneratePptAsync(ReportDataViewModel data)
        => await _pptGenerator.GenerateAsync(data);
}