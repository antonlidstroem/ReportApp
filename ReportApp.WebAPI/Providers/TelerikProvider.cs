using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;
using ReportApp.WebAPI.Providers.Generators;

namespace ReportApp.WebAPI.Providers;

/// <summary>
/// Track 5: Telerik Document Processing (free — no license required for document processing).
/// Uses: Telerik.Documents.Fixed (PDF), Telerik.Documents.Spreadsheet (Excel),
///       Telerik.Documents.Presentation (PowerPoint).
/// NuGet packages required:
///   Telerik.Documents.Core
///   Telerik.Documents.Fixed
///   Telerik.Documents.Spreadsheet
///   Telerik.Documents.SpreadsheetStreaming
///   Telerik.Documents.Presentation
/// </summary>
public class TelerikProvider : IReportProvider
{
    private readonly TelerikPdfGenerator _pdf;
    private readonly TelerikExcelGenerator _excel;
    private readonly TelerikPptGenerator _ppt;

    public TelerikProvider()
    {
        _pdf   = new TelerikPdfGenerator();
        _excel = new TelerikExcelGenerator();
        _ppt   = new TelerikPptGenerator();
    }

    public string Name => "Telerik";

    public async Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
        => await _pdf.GenerateAsync(data);

    public async Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
        => await _excel.GenerateAsync(data);

    public async Task<byte[]> GeneratePptAsync(ReportDataViewModel data)
        => await _ppt.GenerateAsync(data);
}
