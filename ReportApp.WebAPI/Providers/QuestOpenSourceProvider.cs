using System.Drawing;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;
using ShapeCrawler;
using ShapeCrawler.Shapes;

namespace ReportApp.WebAPI.Providers;

public class QuestOpenSourceProvider : IReportProvider
{
    public string Name => "QuestOpenSource";

    public Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var headerBg = "#1e3a5f";
        var accentGreen = "#22c55e";

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);

                page.Header()
                    .BorderBottom(3).BorderColor(accentGreen)
                    .PaddingBottom(12)
                    .Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(data.SurveyTitle)
                                .FontSize(22).Bold()
                                .FontColor(headerBg);

                            col.Item().Text($"{data.CompanyName} · {data.GeneratedAt:yyyy-MM-dd}")
                                .FontSize(10)
                                .FontColor("#64748b");
                        });

                        row.ConstantItem(80)
                            .AlignRight()
                            .Text("Report")
                            .FontSize(10)
                            .FontColor(accentGreen);
                    });

                page.Content().Column(col =>
                {
                    var grouped = data.QuestionSummaries
                        .GroupBy(q => q.Category);

                    foreach (var group in grouped)
                    {
                        col.Item().PaddingVertical(8).Text(group.Key)
                            .Bold().FontSize(12).FontColor(headerBg);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(5);
                                c.ConstantColumn(60);
                                c.ConstantColumn(60);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Fråga").Bold();
                                header.Cell().AlignRight().Text("Snitt").Bold();
                                header.Cell().AlignRight().Text("Svar").Bold();
                            });

                            foreach (var q in group)
                            {
                                table.Cell().Text(q.Text).FontSize(9);

                                table.Cell().AlignRight().Text(q.AverageValue.ToString("F2"));

                                table.Cell().AlignRight().Text(q.TotalResponses.ToString());
                            }
                        });
                    }
                });

                page.Footer()
                    .AlignCenter()
                    .Text($"Generated {data.GeneratedAt:yyyy-MM-dd HH:mm}")
                    .FontSize(8);
            });
        });

        return Task.FromResult(document.GeneratePdf());
    }

    public Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Report");

        ws.Cell(1, 1).Value = data.SurveyTitle;
        ws.Cell(2, 1).Value = data.CompanyName;

        int row = 4;

        ws.Cell(row, 1).Value = "Fråga";
        ws.Cell(row, 2).Value = "Kategori";
        ws.Cell(row, 3).Value = "Snitt";
        ws.Cell(row, 4).Value = "Svar";

        foreach (var q in data.QuestionSummaries)
        {
            row++;

            ws.Cell(row, 1).Value = q.Text;
            ws.Cell(row, 2).Value = q.Category;
            ws.Cell(row, 3).Value = q.AverageValue;
            ws.Cell(row, 4).Value = q.TotalResponses;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return Task.FromResult(ms.ToArray());
    }

    public Task<byte[]> GeneratePptAsync(ReportDataViewModel data)
    {
        var prs = new Presentation();

        var slide = prs.Slides[0];
        slide.Background.Fill.SolidColor.Color = ColorTranslator.FromHtml("#0f2d1a");

        var title = slide.Shapes.AddShape(ShapeType.Rectangle, 50, 100, 600, 80);
        title.TextFrame.Text = data.SurveyTitle;
        title.Fill.FillType = FillType.NoFill;

        var sub = slide.Shapes.AddShape(ShapeType.Rectangle, 50, 200, 600, 40);
        sub.TextFrame.Text = $"{data.CompanyName} · {data.GeneratedAt:yyyy-MM-dd}";
        sub.Fill.FillType = FillType.NoFill;

        var file = Path.GetTempFileName() + ".pptx";
        prs.SaveAs(file);

        return Task.FromResult(File.ReadAllBytes(file));
    }
}