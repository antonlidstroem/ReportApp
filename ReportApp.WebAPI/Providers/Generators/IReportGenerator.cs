using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Providers.Generators;

public interface IReportGenerator
{
    Task<byte[]> GenerateAsync(ReportDataViewModel data);
}