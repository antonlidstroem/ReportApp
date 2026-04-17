using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Interfaces;

public interface ISupportHtmlTemplate
{
    Task<byte[]> GeneratePdfFromTemplateAsync(ReportDataViewModel data, string htmlTemplate);
}
