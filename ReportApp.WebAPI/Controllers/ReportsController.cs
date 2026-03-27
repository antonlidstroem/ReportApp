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

    // Hämtar data till Vue Dashboard
    [HttpGet("data/{surveyId}")]
    public async Task<ActionResult<ReportDataViewModel>> GetReportData(int surveyId)
    {
        var data = await _analysisService.GetSurveyAnalysisAsync(surveyId);
        return Ok(data);
    }

    // Trigger för export
    [HttpPost("export/{providerName}/{format}/{surveyId}")]
    public async Task<IActionResult> Export(string providerName, string format, int surveyId)
    {
        var provider = _providers.FirstOrDefault(p => p.Name.Equals(providerName, StringComparison.OrdinalIgnoreCase));
        if (provider == null) return NotFound("Provider hittades inte");

        var data = await _analysisService.GetSurveyAnalysisAsync(surveyId);

        byte[] fileBytes;
        var sw = Stopwatch.StartNew();

        try
        {
            fileBytes = format.ToLower() switch
            {
                "pdf" => await provider.GeneratePdfAsync(data),
                "excel" => await provider.GenerateExcelAsync(data),
                "ppt" => await provider.GeneratePptAsync(data),
                _ => throw new ArgumentException("Ogiltigt format")
            };
        }
        catch (Exception ex)
        {
            return BadRequest($"Fel vid generering: {ex.Message}");
        }

        sw.Stop();

        // Vi skickar med prestanda-headern för att Vue ska kunna läsa den
        Response.Headers.Add("X-Generation-Time-Ms", sw.ElapsedMilliseconds.ToString());

        var contentType = format.ToLower() switch
        {
            "pdf" => "application/pdf",
            "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "ppt" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            _ => "application/octet-stream"
        };

        return File(fileBytes, contentType, $"Rapport_{providerName}.{format}");
    }
}