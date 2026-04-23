using Syncfusion.Presentation;
using Syncfusion.OfficeChart;
using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Providers.Generators;

public class SyncfusionPptGenerator : IReportGenerator
{
    public async Task<byte[]> GenerateAsync(ReportDataViewModel data)
    {
        using var presentation = Syncfusion.Presentation.Presentation.Create();

        // ── Slide 1: Cover ────────────────────────────────────────────────
        var coverSlide = presentation.Slides.Add(SlideLayoutType.Blank);

        // Dark background rectangle
        var bgRect = coverSlide.Shapes.AddTextBox(0, 0, 960, 540);
        bgRect.Fill.FillType = FillType.Solid;
        bgRect.Fill.SolidFill.Color = ColorObject.FromArgb(15, 45, 74);

        // Accent bar
        var accentBar = coverSlide.Shapes.AddTextBox(0, 0, 6, 540);
        accentBar.Fill.FillType = FillType.Solid;
        accentBar.Fill.SolidFill.Color = ColorObject.FromArgb(20, 184, 166);

        // Eyebrow text
        var eyebrow = coverSlide.Shapes.AddTextBox(40, 160, 800, 30);
        var eyebrowPara = eyebrow.TextBody.AddParagraph("ANALYSRAPPORT");
        eyebrowPara.Font.FontSize = 10;
        eyebrowPara.Font.Color    = ColorObject.FromArgb(150, 200, 225, 220);

        // Survey title
        var titleBox = coverSlide.Shapes.AddTextBox(40, 195, 800, 100);
        var titlePara = titleBox.TextBody.AddParagraph(data.SurveyTitle);
        titlePara.Font.Bold      = true;
        titlePara.Font.FontSize  = 28;
        titlePara.Font.Color     = ColorObject.White;

        // Company + date
        var metaBox = coverSlide.Shapes.AddTextBox(40, 310, 800, 50);
        var metaPara = metaBox.TextBody.AddParagraph(
            $"{data.CompanyName}   ·   Genererad {data.GeneratedAt:yyyy-MM-dd}");
        metaPara.Font.FontSize = 13;
        metaPara.Font.Color    = ColorObject.FromArgb(160, 200, 200, 200);

        // KPI boxes
        var kpis = new[]
        {
            (data.QuestionCount.ToString(),  "FRÅGOR"),
            (data.TotalResponses.ToString(), "SVAR"),
            (data.OverallAverage,            "GENOMSNITT"),
            (data.CategoryCount.ToString(),  "KATEGORIER"),
        };

        for (int i = 0; i < kpis.Length; i++)
        {
            int bx = 40 + i * 225;
            var box = coverSlide.Shapes.AddTextBox(bx, 410, 210, 80);
            box.Fill.FillType = FillType.Solid;
            box.Fill.SolidFill.Color = ColorObject.FromArgb(40, 255, 255, 255);
            // NOTE: IShape does not expose .Line in this version of Syncfusion.Presentation.
            // Border styling on TextBox shapes is not supported via the public IShape API.
            // Omit border — the semi-transparent fill provides sufficient visual grouping.

            var valPara = box.TextBody.AddParagraph(kpis[i].Item1);
            valPara.HorizontalAlignment = HorizontalAlignmentType.Center;
            valPara.Font.Bold           = true;
            valPara.Font.FontSize       = 22;
            valPara.Font.Color          = ColorObject.FromArgb(20, 184, 166);

            var lblPara = box.TextBody.AddParagraph(kpis[i].Item2);
            lblPara.HorizontalAlignment = HorizontalAlignmentType.Center;
            lblPara.Font.FontSize       = 9;
            lblPara.Font.Color          = ColorObject.FromArgb(150, 200, 200, 200);
        }

        // ── Slide 2: Bar Chart ────────────────────────────────────────────
        if (data.QuestionSummaries.Any())
        {
            var chartSlide = presentation.Slides.Add(SlideLayoutType.Blank);

            var chartBg = chartSlide.Shapes.AddTextBox(0, 0, 960, 540);
            chartBg.Fill.FillType = FillType.Solid;
            chartBg.Fill.SolidFill.Color = ColorObject.FromArgb(15, 23, 42);

            // Slide title
            var chartTitle = chartSlide.Shapes.AddTextBox(30, 20, 900, 50);
            var ctPara = chartTitle.TextBody.AddParagraph("Genomsnittlig poäng per fråga");
            ctPara.Font.Bold      = true;
            ctPara.Font.FontSize  = 18;
            ctPara.Font.Color     = ColorObject.White;

            // Build chart with up to 20 questions (PPT has column width limits)
            var qs = data.QuestionSummaries.Take(20).ToList();

            IPresentationChart chart = chartSlide.Shapes.AddChart(30, 75, 900, 420);
            // NOTE: IPresentationChart does not expose HasTitle in this version.
            // Setting ChartTitle to empty string effectively hides the title area.
            chart.ChartTitle = "";
            chart.ChartType  = OfficeChartType.Column_Clustered;

            // Set header row
            chart.ChartData.SetValue(1, 1, "Fråga");
            chart.ChartData.SetValue(1, 2, "Genomsnitt");

            for (int i = 0; i < qs.Count; i++)
            {
                chart.ChartData.SetValue(i + 2, 1, $"Q{i + 1}");
                chart.ChartData.SetValue(i + 2, 2, qs[i].AverageValue);
            }

            var series = chart.Series.Add("Genomsnitt");
            series.Values = chart.ChartData[2, 2, qs.Count + 1, 2];

            // Style the chart
            chart.PrimaryValueAxis.MinimumValue = 0;
            chart.PrimaryValueAxis.MaximumValue = 5;
            // NOTE: IOfficeChartLegend does not expose IsVisible in this version.
            // Use HasLegend on IPresentationChart to hide the legend entirely.
            chart.HasLegend = false;
        }

        // ── Slide 3: Category Summary ─────────────────────────────────────
        var catSlide = presentation.Slides.Add(SlideLayoutType.Blank);

        var catBg = catSlide.Shapes.AddTextBox(0, 0, 960, 540);
        catBg.Fill.FillType = FillType.Solid;
        catBg.Fill.SolidFill.Color = ColorObject.FromArgb(15, 23, 42);

        var catTitleBox = catSlide.Shapes.AddTextBox(30, 20, 900, 50);
        var catTitlePara = catTitleBox.TextBody.AddParagraph("Analys per kategori");
        catTitlePara.Font.Bold     = true;
        catTitlePara.Font.FontSize = 18;
        catTitlePara.Font.Color    = ColorObject.White;

        var categories = data.QuestionSummaries
            .GroupBy(q => q.Category)
            .OrderByDescending(g => g.Average(q => q.AverageValue))
            .ToList();

        if (categories.Any())
        {
            // Category scores as horizontal bars
            float barY = 85;
            float barH = Math.Min(40f, (450f / categories.Count) - 6);

            foreach (var cat in categories.Take(10))
            {
                double avg  = Math.Round(cat.Average(q => q.AverageValue), 2);
                float  pct  = (float)(avg / 5.0) * 600;

                // Category label
                var lbl = catSlide.Shapes.AddTextBox(30, (int)barY, 240, (int)barH);
                lbl.Fill.FillType = FillType.None;
                var lblPara = lbl.TextBody.AddParagraph(cat.Key);
                lblPara.Font.FontSize = (int)Math.Max(8, barH * 0.45f);
                lblPara.Font.Color    = ColorObject.FromArgb(200, 200, 200);

                // Bar background
                var barBg = catSlide.Shapes.AddTextBox(280, (int)barY + 4, 600, (int)(barH - 8));
                barBg.Fill.FillType = FillType.Solid;
                barBg.Fill.SolidFill.Color = ColorObject.FromArgb(40, 100, 116, 139);

                // Filled bar
                var colorArgb = avg >= 4
                    ? ColorObject.FromArgb(22, 163, 74)
                    : avg >= 3
                        ? ColorObject.FromArgb(217, 119, 6)
                        : ColorObject.FromArgb(220, 38, 38);

                var barFill = catSlide.Shapes.AddTextBox(280, (int)barY + 4, (int)pct, (int)(barH - 8));
                barFill.Fill.FillType = FillType.Solid;
                barFill.Fill.SolidFill.Color = colorArgb;

                // Score label at end of bar
                var scoreLbl = catSlide.Shapes.AddTextBox(890, (int)barY, 60, (int)barH);
                scoreLbl.Fill.FillType = FillType.None;
                var scorePara = scoreLbl.TextBody.AddParagraph(avg.ToString("F2"));
                scorePara.Font.Bold     = true;
                scorePara.Font.FontSize = (int)Math.Max(8, barH * 0.4f);
                scorePara.Font.Color    = colorArgb;

                barY += barH + 6;
            }
        }

        // ── Slide 4: Question table ───────────────────────────────────────
        var tableSlide = presentation.Slides.Add(SlideLayoutType.Blank);

        var tblBg = tableSlide.Shapes.AddTextBox(0, 0, 960, 540);
        tblBg.Fill.FillType = FillType.Solid;
        tblBg.Fill.SolidFill.Color = ColorObject.FromArgb(15, 23, 42);

        var tblTitle = tableSlide.Shapes.AddTextBox(30, 15, 900, 40);
        var tblTitlePara = tblTitle.TextBody.AddParagraph("Frågedetaljer");
        tblTitlePara.Font.Bold     = true;
        tblTitlePara.Font.FontSize = 16;
        tblTitlePara.Font.Color    = ColorObject.White;

        // Table
        int rowCount = Math.Min(data.QuestionSummaries.Count, 18) + 1; // +1 header
        var tbl = tableSlide.Shapes.AddTable(rowCount, 4, 30, 60, 900, 460);

        // Header
        string[] colHeaders = { "Fråga", "Kategori", "Snitt", "Svar" };
        for (int c = 0; c < 4; c++)
        {
            var cell = tbl.Rows[0].Cells[c];
            cell.TextBody.AddParagraph(colHeaders[c]).Font.Bold     = true;
            cell.TextBody.Paragraphs[0].Font.Color                  = ColorObject.White;
            cell.TextBody.Paragraphs[0].Font.FontSize               = 10;
            cell.Fill.FillType                                       = FillType.Solid;
            cell.Fill.SolidFill.Color                               = ColorObject.FromArgb(15, 45, 74);
        }

        // Data rows
        for (int i = 0; i < Math.Min(data.QuestionSummaries.Count, 18); i++)
        {
            var q    = data.QuestionSummaries[i];
            var row  = tbl.Rows[i + 1];
            var bg   = i % 2 == 0
                ? ColorObject.FromArgb(22, 30, 44)
                : ColorObject.FromArgb(30, 40, 55);

            row.Cells[0].TextBody.AddParagraph(
                q.Text.Length > 60 ? q.Text[..57] + "…" : q.Text);
            row.Cells[1].TextBody.AddParagraph(q.Category);
            row.Cells[2].TextBody.AddParagraph(q.AverageValue.ToString("F2"));
            row.Cells[3].TextBody.AddParagraph(q.TotalResponses.ToString());

            var scoreColor = q.AverageValue >= 4
                ? ColorObject.FromArgb(22, 163, 74)
                : q.AverageValue >= 3
                    ? ColorObject.FromArgb(217, 119, 6)
                    : ColorObject.FromArgb(220, 38, 38);

            for (int c = 0; c < 4; c++)
            {
                var cell = row.Cells[c];
                cell.Fill.FillType = FillType.Solid;
                cell.Fill.SolidFill.Color = bg;
                if (cell.TextBody.Paragraphs.Count > 0)
                {
                    cell.TextBody.Paragraphs[0].Font.FontSize = 9;
                    cell.TextBody.Paragraphs[0].Font.Color    =
                        c == 2 ? scoreColor : ColorObject.FromArgb(200, 200, 200);
                    if (c == 2) cell.TextBody.Paragraphs[0].Font.Bold = true;
                }
            }
        }

        using var ms = new MemoryStream();
        presentation.Save(ms);
        return ms.ToArray();
    }
}
