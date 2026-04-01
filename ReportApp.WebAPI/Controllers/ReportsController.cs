using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ReportApp.AnalysisEngine.Models;
using ReportApp.AnalysisEngine.Services;
using ReportApp.WebAPI.Interfaces;

namespace ReportApp.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly AnalysisService _analysisService;
    private readonly IEnumerable<IReportProvider> _providers;

    public ReportsController(AnalysisService analysisService, IEnumerable<IReportProvider> providers)
    {
        _analysisService = analysisService;
        _providers = providers;
    }

    /// <summary>
    /// Returns analysed data for a survey, with optional date and question filters.
    /// </summary>
    [HttpGet("data/{surveyId}")]
    public async Task<ActionResult<ReportDataViewModel>> GetReportData(
        int surveyId,
        [FromQuery] DateTime? start,
        [FromQuery] DateTime? end,
        [FromQuery] string? questionIds)
    {
        var ids = ParseIds(questionIds);
        var data = await _analysisService.GetSurveyAnalysisAsync(surveyId, start, end, ids);
        return Ok(data);
    }

    /// <summary>
    /// Lists all surveys with question count.
    /// </summary>
    [HttpGet("surveys")]
    public async Task<IActionResult> GetSurveys()
    {
        var surveys = await _analysisService.GetAllSurveysAsync();
        return Ok(surveys);
    }

    /// <summary>
    /// Returns questions for a specific survey (for the picker UI).
    /// </summary>
    [HttpGet("surveys/{surveyId}/questions")]
    public async Task<IActionResult> GetQuestions(int surveyId)
    {
        var questions = await _analysisService.GetSurveyQuestionsAsync(surveyId);
        return Ok(questions);
    }

    /// <summary>
    /// Lists all registered providers and their capabilities.
    /// </summary>
    [HttpGet("providers")]
    public IActionResult GetProviders()
    {
        var result = _providers.Select(p => new
        {
            name = p.Name,
            supportsPdf = true,
            supportsExcel = true,
            supportsPpt = true,
            supportsHtmlTemplate = p is ISupportHtmlTemplate
        });
        return Ok(result);
    }

    /// <summary>
    /// Generates and streams a report file (pdf / excel / ppt).
    /// Accepts optional questionIds filter and HTML template override.
    /// </summary>
    [HttpPost("export/{providerName}/{format}/{surveyId}")]
    public async Task<IActionResult> Export(
        string providerName,
        string format,
        int surveyId,
        [FromQuery] DateTime? start,
        [FromQuery] DateTime? end,
        [FromQuery] string? questionIds,
        [FromBody] ExportRequest? body)
    {
        var provider = _providers.FirstOrDefault(p =>
            p.Name.Equals(providerName, StringComparison.OrdinalIgnoreCase));
        if (provider == null)
            return NotFound($"Provider '{providerName}' not found.");

        var ids = ParseIds(questionIds);
        var data = await _analysisService.GetSurveyAnalysisAsync(surveyId, start, end, ids);

        byte[] fileBytes;
        var sw = Stopwatch.StartNew();

        try
        {
            fileBytes = format.ToLower() switch
            {
                "pdf" when body?.HtmlTemplate != null && provider is ISupportHtmlTemplate tp
                    => await tp.GeneratePdfFromTemplateAsync(data, body.HtmlTemplate),
                "pdf"    => await provider.GeneratePdfAsync(data),
                "excel"  => await provider.GenerateExcelAsync(data),
                "ppt"    => await provider.GeneratePptAsync(data),
                _ => throw new ArgumentException("Invalid format. Choose pdf, excel or ppt.")
            };
        }
        catch (NotImplementedException)
        {
            return BadRequest($"Provider '{providerName}' does not support format '{format}'.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Generation error: {ex.Message}");
        }

        sw.Stop();

        Response.Headers.Append("X-Generation-Time-Ms", sw.ElapsedMilliseconds.ToString());
        Response.Headers.Append("X-File-Size-Bytes", fileBytes.Length.ToString());
        Response.Headers.Append("Access-Control-Expose-Headers",
            "X-Generation-Time-Ms, X-File-Size-Bytes");

        var contentType = format.ToLower() switch
        {
            "pdf"   => "application/pdf",
            "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "ppt"   => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            _       => "application/octet-stream"
        };

        var ext = format.ToLower() == "excel" ? "xlsx" : format.ToLower();
        var fileName = $"Rapport_{providerName}_{DateTime.Now:yyyyMMdd}.{ext}";
        return File(fileBytes, contentType, fileName);
    }

    private static List<int>? ParseIds(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        return raw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
                  .Where(id => id > 0)
                  .ToList();
    }
}

public class ExportRequest
{
    public string? HtmlTemplate { get; set; }
    public Dictionary<string, object>? ModuleConfig { get; set; }
}
