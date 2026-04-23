using Telerik.Windows.Documents.Presentation.FormatProviders.Pptx;
using Telerik.Windows.Documents.Presentation.Model;
using Telerik.Windows.Documents.Presentation.Model.Shapes;
using Telerik.Windows.Documents.Presentation.Model.Tables;
using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Providers.Generators;

public class TelerikPptGenerator
{
    public Task<byte[]> GenerateAsync(ReportDataViewModel data)
    {
        var presentation = new RadFlowPresentation();
        presentation.SlideSize = SlideSizes.GetSlideSize(SlideSizeType.Widescreen);

        // ── Title slide ───────────────────────────────────────────────
        var titleSlide = presentation.Slides.AddSlide();
        titleSlide.Layout = SlideLayoutType.Blank;

        // Background
        titleSlide.Background = new SlideBackground
        {
            Fill = new SolidFill { Color = new ThemableColor(ColorExtensions.FromArgb(255, 15, 45, 74)) }
        };

        // Track badge text box
        var badge = titleSlide.ShapeCollection.AddTextBox(
            new System.Windows.Rect(80, 180, 600, 30));
        badge.TextBody.AddParagraph().AddRun("ANALYSRAPPORT · TELERIK DOCUMENT PROCESSING").FontSize = 9;
        badge.TextBody.Paragraphs[0].Runs[0].ForeColor = new ThemableColor(ColorExtensions.FromArgb(255, 96, 165, 250));

        // Title
        var titleBox = titleSlide.ShapeCollection.AddTextBox(
            new System.Windows.Rect(80, 220, 840, 120));
        var titlePara = titleBox.TextBody.AddParagraph();
        var titleRun = titlePara.AddRun(data.SurveyTitle);
        titleRun.FontSize = 36;
        titleRun.IsBold = true;
        titleRun.ForeColor = new ThemableColor(ColorExtensions.FromArgb(255, 255, 255, 255));

        // Company
        var compBox = titleSlide.ShapeCollection.AddTextBox(
            new System.Windows.Rect(80, 350, 840, 40));
        var compRun = compBox.TextBody.AddParagraph().AddRun(data.CompanyName);
        compRun.FontSize = 16;
        compRun.ForeColor = new ThemableColor(ColorExtensions.FromArgb(255, 148, 163, 184));

        // KPI boxes
        AddKpiBoxes(titleSlide, data);

        // ── Data slide ────────────────────────────────────────────────
        var dataSlide = presentation.Slides.AddSlide();
        dataSlide.Layout = SlideLayoutType.Blank;

        // Slide title
        var slideTitle = dataSlide.ShapeCollection.AddTextBox(
            new System.Windows.Rect(40, 20, 880, 50));
        var stRun = slideTitle.TextBody.AddParagraph().AddRun(
            $"{data.SurveyTitle} — Frågeöversikt");
        stRun.FontSize = 18;
        stRun.IsBold = true;
        stRun.ForeColor = new ThemableColor(ColorExtensions.FromArgb(255, 15, 45, 74));

        // Table
        AddDataTable(dataSlide, data);

        // ── Export ────────────────────────────────────────────────────
        var provider = new PptxFormatProvider();
        using var ms = new MemoryStream();
        provider.Export(presentation, ms);
        return Task.FromResult(ms.ToArray());
    }

    private static void AddKpiBoxes(RadFlowSlide slide, ReportDataViewModel data)
    {
        var kpis = new[]
        {
            (data.QuestionCount.ToString(), "Frågor"),
            (data.TotalResponses.ToString(), "Svar"),
            (data.OverallAverage, "Genomsnitt"),
            (data.CategoryCount.ToString(), "Kategorier"),
        };

        double boxW = 170, boxH = 90, startX = 80, y = 430, gap = 20;

        foreach (var (val, lbl) in kpis)
        {
            var box = slide.ShapeCollection.AddTextBox(
                new System.Windows.Rect(startX, y, boxW, boxH));
            box.ShapeProperties.Fill = new SolidFill
            {
                Color = new ThemableColor(ColorExtensions.FromArgb(255, 30, 58, 95))
            };

            var valPara = box.TextBody.AddParagraph();
            valPara.SpaceAfter = 0;
            var valRun = valPara.AddRun(val);
            valRun.FontSize = 28;
            valRun.IsBold = true;
            valRun.ForeColor = new ThemableColor(ColorExtensions.FromArgb(255, 96, 165, 250));

            var lblPara = box.TextBody.AddParagraph();
            var lblRun = lblPara.AddRun(lbl.ToUpperInvariant());
            lblRun.FontSize = 9;
            lblRun.ForeColor = new ThemableColor(ColorExtensions.FromArgb(255, 148, 163, 184));

            startX += boxW + gap;
        }
    }

    private static void AddDataTable(RadFlowSlide slide, ReportDataViewModel data)
    {
        int maxRows = Math.Min(data.QuestionSummaries.Count, 20);
        var table = new Table();
        table.Rows.Add(); // header

        // Header
        var headerRow = table.Rows[0];
        var hHeaders = new[] { "Fråga", "Kategori", "Snitt", "Svar" };
        foreach (var h in hHeaders)
        {
            var cell = headerRow.Cells.Add();
            cell.Fill = new SolidFill
            {
                Color = new ThemableColor(ColorExtensions.FromArgb(255, 15, 45, 74))
            };
            var para = cell.TextBody.AddParagraph();
            var run = para.AddRun(h);
            run.FontSize = 9;
            run.IsBold = true;
            run.ForeColor = new ThemableColor(ColorExtensions.FromArgb(255, 255, 255, 255));
        }

        // Data
        for (int i = 0; i < maxRows; i++)
        {
            var q = data.QuestionSummaries[i];
            table.Rows.Add();
            var row = table.Rows[i + 1];

            var cellData = new[]
            {
                (q.Text.Length > 60 ? q.Text[..60] + "…" : q.Text, false),
                (q.Category, false),
                (q.AverageValue.ToString("F2"), true),
                (q.TotalResponses.ToString(), false),
            };

            foreach (var (text, isBold) in cellData)
            {
                var cell = row.Cells.Add();
                if (i % 2 != 0)
                {
                    cell.Fill = new SolidFill
                    {
                        Color = new ThemableColor(ColorExtensions.FromArgb(255, 248, 250, 252))
                    };
                }
                var para = cell.TextBody.AddParagraph();
                var run = para.AddRun(text);
                run.FontSize = 8;
                run.IsBold = isBold;

                if (isBold)
                {
                    var color = q.AverageValue >= 4
                        ? ColorExtensions.FromArgb(255, 22, 163, 74)
                        : q.AverageValue >= 3
                            ? ColorExtensions.FromArgb(255, 217, 119, 6)
                            : ColorExtensions.FromArgb(255, 220, 38, 38);
                    run.ForeColor = new ThemableColor(color);
                }
            }
        }

        slide.ShapeCollection.AddTable(table, new System.Windows.Rect(40, 80, 880, 420));
    }
}
