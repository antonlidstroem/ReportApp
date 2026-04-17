using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;
using ReportApp.WebAPI.Providers.Generators;

namespace ReportApp.WebAPI.Providers;

public class SyncfusionProvider : IReportProvider
{
    private readonly SyncfusionPdfGenerator _pdf;
    private readonly SyncfusionExcelGenerator _excel;
    private readonly SyncfusionPptGenerator _ppt;

    public SyncfusionProvider()
    {
        _pdf   = new SyncfusionPdfGenerator();
        _excel = new SyncfusionExcelGenerator();
        _ppt   = new SyncfusionPptGenerator();
    }

    public string Name => "Syncfusion";

    public Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)   => _pdf.GenerateAsync(data);
    public Task<byte[]> GenerateExcelAsync(ReportDataViewModel data) => _excel.GenerateAsync(data);
    public Task<byte[]> GeneratePptAsync(ReportDataViewModel data)   => _ppt.GenerateAsync(data);
}
