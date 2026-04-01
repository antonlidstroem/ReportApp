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
        // 1. Hitta provider
        var provider = _providers.FirstOrDefault(p =>
            p.Name.Equals(providerName, StringComparison.OrdinalIgnoreCase));

        if (provider == null) return NotFound($"Provider '{providerName}' hittades inte.");

        // 2. Hämta data
        var ids = ParseIds(questionIds);
        var data = await _analysisService.GetSurveyAnalysisAsync(surveyId, start, end, ids);

        byte[] fileBytes;
        var sw = Stopwatch.StartNew();

        try
        {
            // 3. Generera fil
            // 3. Generera fil
            fileBytes = format.ToLower() switch
            {
                // Vi lägger till en kontroll för "string" och använder IsNullOrWhiteSpace för säkerhet
                "pdf" when !string.IsNullOrWhiteSpace(body?.HtmlTemplate)
                           && body.HtmlTemplate != "string"
                           && provider is ISupportHtmlTemplate tp
                    => await tp.GeneratePdfFromTemplateAsync(data, body.HtmlTemplate),

                "pdf" => await provider.GeneratePdfAsync(data),
                "excel" or "xlsx" => await provider.GenerateExcelAsync(data),
                "ppt" or "pptx" => await provider.GeneratePptAsync(data),
                _ => throw new ArgumentException("Ogiltigt format.")
            };
        }
        catch (Exception ex)
        {
            return BadRequest($"Fel vid generering ({providerName}): {ex.Message}");
        }

        sw.Stop();

        // 4. Bestäm exakt ContentType och Filändelse (HÄR VAR FELET INNAN)
        string contentType;
        string fileExtension;

        switch (format.ToLower())
        {
            case "pdf":
                contentType = "application/pdf";
                fileExtension = "pdf";
                break;
            case "excel":
            case "xlsx":
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                fileExtension = "xlsx";
                break;
            case "ppt":
            case "pptx":
                contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                fileExtension = "pptx";
                break;
            default:
                contentType = "application/octet-stream";
                fileExtension = "bin";
                break;
        }

        Response.Headers.Append("X-Generation-Time-Ms", sw.ElapsedMilliseconds.ToString());
        Response.Headers.Append("Access-Control-Expose-Headers", "X-Generation-Time-Ms");

        // 5. Returnera filen med garanterat rätt ändelse
        var fileName = $"Rapport_{providerName}_{DateTime.Now:yyyyMMdd}.{fileExtension}";
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

        return ids.Any() ? ids : null; // Om listan är tom, returnera null (hämta alla)
    }
}