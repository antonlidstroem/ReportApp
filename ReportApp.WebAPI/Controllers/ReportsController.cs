using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ReportApp.AnalysisEngine.Models;
using ReportApp.AnalysisEngine.Services;
using ReportApp.WebAPI.Interfaces;
using ReportApp.WebAPI.Models;

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

    [HttpGet("surveys")]
    public async Task<IActionResult> GetSurveys()
    {
        var surveys = await _analysisService.GetAllSurveysAsync();
        return Ok(surveys);
    }

    [HttpGet("surveys/{surveyId}/questions")]
    public async Task<IActionResult> GetQuestions(int surveyId)
    {
        var questions = await _analysisService.GetSurveyQuestionsAsync(surveyId);
        return Ok(questions);
    }

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
        // 1. Find provider (case-insensitive)
        var provider = _providers.FirstOrDefault(p =>
            string.Equals(p.Name, providerName, StringComparison.OrdinalIgnoreCase));

        if (provider == null)
            return NotFound($"Provider '{providerName}' not found. Available: {string.Join(", ", _providers.Select(p => p.Name))}");

        // 2. Fetch data
        var ids = ParseIds(questionIds);
        ReportDataViewModel data;
        try
        {
            data = await _analysisService.GetSurveyAnalysisAsync(surveyId, start, end, ids);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }

        byte[] fileBytes;
        var sw = Stopwatch.StartNew();

        try
        {
            // 3. Generate file
            fileBytes = format.ToLowerInvariant() switch
            {
                "pdf" when body?.HtmlTemplate is { Length: > 0 } tmpl
                           && tmpl != "string"
                           && provider is ISupportHtmlTemplate tp
                    => await tp.GeneratePdfFromTemplateAsync(data, tmpl),

                "pdf" => await provider.GeneratePdfAsync(data),
                "excel" or "xlsx" => await provider.GenerateExcelAsync(data),
                "ppt" or "pptx" => await provider.GeneratePptAsync(data),
                _ => throw new ArgumentException($"Invalid format: '{format}'. Supported: pdf, excel, xlsx, ppt, pptx")
            };
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error generating {format.ToUpper()} with provider '{providerName}': {ex.Message}");
        }

        sw.Stop();

        // 4. Determine content type and extension
        var (contentType, fileExtension) = format.ToLowerInvariant() switch
        {
            "pdf" => ("application/pdf", "pdf"),
            "excel" or "xlsx" => ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx"),
            "ppt" or "pptx" => ("application/vnd.openxmlformats-officedocument.presentationml.presentation", "pptx"),
            _ => ("application/octet-stream", "bin")
        };

        // 5. Add timing headers (these must be set before File() is called)
        Response.Headers.Append("X-Generation-Time-Ms", sw.ElapsedMilliseconds.ToString());
        Response.Headers.Append("X-File-Size-Bytes", fileBytes.Length.ToString());
        // Access-Control-Expose-Headers is handled by the CORS policy (WithExposedHeaders)

        var fileName = $"Rapport_{providerName}_{DateTime.Now:yyyyMMdd_HHmmss}.{fileExtension}";
        return File(fileBytes, contentType, fileName);
    }

    private static List<int>? ParseIds(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw) || raw == "undefined" || raw == "null")
            return null;

        var ids = raw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                     .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
                     .Where(id => id > 0)
                     .ToList();

        return ids.Count > 0 ? ids : null;
    }
}
