namespace ReportApp.WebAPI.Models;

public class ExportRequest
{
    public string? HtmlTemplate { get; set; }
    public Dictionary<string, object>? ModuleConfig { get; set; }
}