namespace ReportApp.WebAPI.Models;

public class ExportRequest
{
    public string? HtmlTemplate { get; set; }
    public Dictionary<string, object>? ModuleConfig { get; set; }

    // Add these to fix the CS1061 errors
    public int SurveyId { get; set; }
    public string? Format { get; set; }
}