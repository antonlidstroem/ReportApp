using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;
using ShapeCrawler;

// VIKTIGT: Detta namespace måste matcha mappen och namnet i Program.cs
namespace ReportApp.WebAPI.Providers;

public class QuestOpenSourceProvider : IReportProvider
{
    public string Name => "QuestOpenSource";

    public async Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.Header().Row(row => {
                    row.RelativeItem().Text(data.SurveyTitle).FontSize(24).Bold().FontColor(Colors.Blue.Medium);
                    row.RelativeItem().AlignRight().Text(DateTime.Now.ToShortDateString());
                });

                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text($"Företag: {data.CompanyName}").FontSize(14);
                    col.Item().PaddingTop(10).Table(table => {
                        table.ColumnsDefinition(c => {
                            c.RelativeColumn(3);
                            c.RelativeColumn(1);
                        });
                        table.Header(h => {
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Fråga").Bold();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Snitt").Bold();
                        });
                        foreach (var q in data.QuestionSummaries)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(q.Text);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten4).Padding(5).Text(q.AverageValue.ToString());
                        }
                    });
                });
            });
        });
        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Analys");
        ws.Cell(1, 1).Value = "Fråga"; ws.Cell(1, 2).Value = "Medelvärde";
        for (int i = 0; i < data.QuestionSummaries.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = data.QuestionSummaries[i].Text;
            ws.Cell(i + 2, 2).Value = data.QuestionSummaries[i].AverageValue;
        }
        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> GeneratePptAsync(ReportDataViewModel data)
    {
        // För PoC: skapar en tom presentation
        var pres = new Presentation();
        using var ms = new MemoryStream();
        pres.Save(ms);
        return ms.ToArray();
    }
}