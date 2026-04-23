using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;

namespace ReportApp.WebAPI.Providers;

/// <summary>
/// Track 6: DevExpress Document Processing.
///
/// STUB — DevExpress.Document.Processor requires a paid DevExpress Universal license.
/// There is no free NuGet tier for document generation.
///
/// To activate:
///   1. Obtain a DevExpress Universal subscription (https://www.devexpress.com/subscriptions/)
///   2. Add NuGet sources: https://nuget.devexpress.com/api (requires DevExpress login)
///   3. Add packages: DevExpress.Document.Processor, DevExpress.Pdf.Core, DevExpress.Spreadsheet.Core
///   4. Add your license key:
///        DevExpress.Licensing.LicenseHelper.RegisterLicense("YOUR-KEY-HERE");
///   5. Replace the NotImplementedException bodies below with your implementation.
/// </summary>
public class DevExpressProvider : IReportProvider
{
    public string Name => "DevExpress";

    private const string StubMessage =
        "DevExpress.Document.Processor requires a paid DevExpress Universal license. " +
        "Add the NuGet package from nuget.devexpress.com and your license key to activate. " +
        "See DevExpressProvider.cs for setup instructions.";

    public Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
        => throw new NotImplementedException(StubMessage);

    public Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
        => throw new NotImplementedException(StubMessage);

    public Task<byte[]> GeneratePptAsync(ReportDataViewModel data)
        => throw new NotImplementedException(StubMessage);
}
