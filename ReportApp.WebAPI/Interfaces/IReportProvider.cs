using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Interfaces;

public interface IReportProvider
{
    string Name { get; }
    Task<byte[]> GeneratePdfAsync(ReportDataViewModel data);
    Task<byte[]> GenerateExcelAsync(ReportDataViewModel data);
    Task<byte[]> GeneratePptAsync(ReportDataViewModel data);
}