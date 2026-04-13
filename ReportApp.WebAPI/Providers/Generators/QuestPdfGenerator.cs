using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Providers.Generators;

public class QuestPdfGenerator : IReportGenerator
{
    public Task<byte[]> GenerateAsync(ReportDataViewModel data)
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

                page.Header().BorderBottom(3).BorderColor(accentGreen).PaddingBottom(12).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(data.SurveyTitle).FontSize(22).Bold().FontColor(headerBg);
                        col.Item().Text($"{data.CompanyName} · {data.GeneratedAt:yyyy-MM-dd}").FontSize(10).FontColor("#64748b");
                    });
                });

                page.Content().Column(col =>
                {
                    var grouped = data.QuestionSummaries.GroupBy(q => q.Category);
                    foreach (var group in grouped)
                    {
                        col.Item().PaddingVertical(8).Text(group.Key).Bold().FontSize(12).FontColor(headerBg);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c => { c.RelativeColumn(5); c.ConstantColumn(60); c.ConstantColumn(60); });
                            table.Header(header => {
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

                page.Footer().AlignCenter().Text($"Generated {data.GeneratedAt:yyyy-MM-dd HH:mm}").FontSize(8);
            });
        });

        return Task.FromResult(document.GeneratePdf());
    }
}