using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;
using System.IO;

namespace ReportApp.WebAPI.Providers;

public class QuestOpenSourceProvider : IReportProvider
{
    public string Name => "QuestOpenSource";

    public async Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
    {
        // Viktigt: QuestPDF 2026 kräver licenssättning i koden
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Header().Text(data.SurveyTitle).FontSize(25).SemiBold().FontColor(Colors.Blue.Medium);

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn();
                        c.ConstantColumn(100);
                    });

                    // Header
                    table.Cell().Element(Block).Text("Fråga").Bold();
                    table.Cell().Element(Block).Text("Snittvärde").Bold();

                    // Rader
                    foreach (var q in data.QuestionSummaries)
                    {
                        table.Cell().Element(Block).Text(q.Text);
                        table.Cell().Element(Block).Text(q.AverageValue.ToString("F1"));
                    }
                });
            });
        });

        return document.GeneratePdf();

        // Helper för tabell-styling
        static IContainer Block(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
    }

    public async Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        ws.Cell(1, 1).Value = "Fråga";
        ws.Cell(1, 2).Value = "Snitt";

        for (int i = 0; i < data.QuestionSummaries.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = data.QuestionSummaries[i].Text;
            // Tvinga värdet till ett nummer så Excel inte tror det är text
            ws.Cell(i + 2, 2).Value = (double)data.QuestionSummaries[i].AverageValue;
        }

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> GeneratePptAsync(ReportDataViewModel data)
    {
        // Vi håller detta interface-metoden nöjd men inaktiv tills vidare
        throw new NotImplementedException("PowerPoint-export är tillfälligt inaktiverad under uppgradering.");
    }
}