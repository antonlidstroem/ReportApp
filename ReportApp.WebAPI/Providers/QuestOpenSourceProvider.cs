using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;
using ReportApp.WebAPI.Providers.Generators;

namespace ReportApp.WebAPI.Providers;

public class QuestOpenSourceProvider : IReportProvider
{
    public string Name => "QuestOpenSource";

    // Vi mappar format till rätt generator
    public async Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
        => await new QuestPdfGenerator().GenerateAsync(data);

    public async Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
        => await new ClosedXmlExcelGenerator().GenerateAsync(data);

    public async Task<byte[]> GeneratePptAsync(ReportDataViewModel data)
        => await new ShapeCrawlerPptGenerator().GenerateAsync(data);
}