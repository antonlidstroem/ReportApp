using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    /// Hämtar analyserad data för dashboarden med valfritt datumfilter.
    /// </summary>
    [HttpGet("data/{surveyId}")]
    public async Task<ActionResult<ReportDataViewModel>> GetReportData(
        int surveyId,
        [FromQuery] DateTime? start,
        [FromQuery] DateTime? end)
    {
        var data = await _analysisService.GetSurveyAnalysisAsync(surveyId, start, end);
        return Ok(data);
    }

    /// <summary>
    /// Genererar och exporterar en rapportfil baserat på vald provider, format och datumfilter.
    /// </summary>
    [HttpPost("export/{providerName}/{format}/{surveyId}")]
    public async Task<IActionResult> Export(
        string providerName,
        string format,
        int surveyId,
        [FromQuery] DateTime? start,
        [FromQuery] DateTime? end)
    {
        // 1. Hitta rätt provider (QuestPDF, IronSuite, jsreport etc.)
        var provider = _providers.FirstOrDefault(p => p.Name.Equals(providerName, StringComparison.OrdinalIgnoreCase));
        if (provider == null) return NotFound($"Provider '{providerName}' hittades inte.");

        // 2. Hämta den filtrerade datan som ska ligga till grund för rapporten
        var data = await _analysisService.GetSurveyAnalysisAsync(surveyId, start, end);

        byte[] fileBytes;
        var sw = Stopwatch.StartNew();

        try
        {
            // 3. Generera filen baserat på önskat format
            fileBytes = format.ToLower() switch
            {
                "pdf" => await provider.GeneratePdfAsync(data),
                "excel" => await provider.GenerateExcelAsync(data),
                "ppt" => await provider.GeneratePptAsync(data),
                _ => throw new ArgumentException("Ogiltigt format. Välj pdf, excel eller ppt.")
            };
        }
        catch (Exception ex)
        {
            return BadRequest($"Fel vid generering: {ex.Message}");
        }

        sw.Stop();

        // 4. Skicka prestandamätning i headern så att Vue-frontend kan visa den
        Response.Headers.Append("X-Generation-Time-Ms", sw.ElapsedMilliseconds.ToString());

        var contentType = format.ToLower() switch
        {
            "pdf" => "application/pdf",
            "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "ppt" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            _ => "application/octet-stream"
        };

        var fileName = $"Rapport_{providerName}_{DateTime.Now:yyyyMMdd}.{format.ToLower()}";
        return File(fileBytes, contentType, fileName);
    }

    /// <summary>
    /// Listar alla tillgängliga enkäter.
    /// </summary>
    [HttpGet("surveys")]
    public async Task<IActionResult> GetSurveys()
    {
        var surveys = await _analysisService.GetAllSurveysAsync();
        return Ok(surveys);
    }
}