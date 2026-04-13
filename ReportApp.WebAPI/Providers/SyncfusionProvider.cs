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
        _pdf = new SyncfusionPdfGenerator();
        _excel = new SyncfusionExcelGenerator();
        _ppt = new SyncfusionPptGenerator();
    }

    public string Name => "Syncfusion";

    public async Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
        => await _pdf.GenerateAsync(data);

    public async Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
        => await _excel.GenerateAsync(data);

    public async Task<byte[]> GeneratePptAsync(ReportDataViewModel data)
        => await _ppt.GenerateAsync(data);
}