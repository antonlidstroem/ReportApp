using Syncfusion.Presentation;
using Syncfusion.OfficeChart;
using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Providers.Generators;

public class SyncfusionPptGenerator : IReportGenerator
{
    public async Task<byte[]> GenerateAsync(ReportDataViewModel data)
    {
        using var presentation = Syncfusion.Presentation.Presentation.Create();
        var slide = presentation.Slides.Add(SlideLayoutType.TitleOnly);

        // Här vet kompilatorn exakt vilken IShape det gäller
        var titleShape = slide.Shapes[0] as IShape;
        titleShape!.TextBody.AddParagraph(data.SurveyTitle);

        if (data.QuestionSummaries.Any())
        {
            IPresentationChart chart = slide.Shapes.AddChart(100, 100, 600, 400);
            chart.ChartType = OfficeChartType.Column_Clustered;

            for (int i = 0; i < data.QuestionSummaries.Count; i++)
            {
                chart.ChartData.SetValue(i + 2, 1, "Q" + (i + 1));
                chart.ChartData.SetValue(i + 2, 2, data.QuestionSummaries[i].AverageValue);
            }

            var series = chart.Series.Add("Snitt");
            series.Values = chart.ChartData[2, 2, data.QuestionSummaries.Count + 1, 2];
        }

        using var ms = new MemoryStream();
        presentation.Save(ms);
        return ms.ToArray();
    }
}