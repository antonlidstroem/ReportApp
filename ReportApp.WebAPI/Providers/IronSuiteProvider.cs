// IronSuiteProvider — STUB (excluded from compilation in .csproj)
// To activate: install IronPDF, IronXL, IronPPT NuGet packages,
// add license key to appsettings.json, then re-include this file.
using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;

namespace ReportApp.WebAPI.Providers;

public class IronSuiteProvider : IReportProvider, ISupportHtmlTemplate
{
    public string Name => "IronSuite";
    public Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
        => throw new NotImplementedException("Install IronPDF first.");
    public Task<byte[]> GeneratePdfFromTemplateAsync(ReportDataViewModel data, string htmlTemplate)
        => throw new NotImplementedException("Install IronPDF first.");
    public Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
        => throw new NotImplementedException("Install IronXL first.");
    public Task<byte[]> GeneratePptAsync(ReportDataViewModel data)
        => throw new NotImplementedException("Install IronPPT first.");
}
