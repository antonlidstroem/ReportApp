using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.ColorSpaces;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;
using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Providers.Generators;

public class TelerikPdfGenerator
{
    public Task<byte[]> GenerateAsync(ReportDataViewModel data)
    {
        var document = new RadFixedDocument();

        // ── Cover page ──────────────────────────────────────────────────
        var coverPage = document.Pages.AddPage();
        coverPage.Size = new System.Windows.Size(595, 842); // A4
        var coverEditor = new FixedContentEditor(coverPage);

        // Background
        coverEditor.GraphicProperties.FillColor = new RgbColor(15, 45, 74);
        coverEditor.DrawRectangle(new System.Windows.Rect(0, 0, 595, 842));

        // Title
        coverEditor.GraphicProperties.FillColor = new RgbColor(255, 255, 255);
        coverEditor.TextProperties.FontSize = 28;
        coverEditor.TextProperties.HorizontalAlignment = Telerik.Windows.Documents.Fixed.Model.Editing.Flow.HorizontalAlignment.Left;

        var titleBlock = new Block();
        titleBlock.SetBold(true);
        titleBlock.HorizontalAlignment = Telerik.Windows.Documents.Fixed.Model.Editing.Flow.HorizontalAlignment.Left;
        titleBlock.TextProperties.FontSize = 9;
        titleBlock.TextProperties.FillColor = new RgbColor(148, 163, 184);
        titleBlock.InsertText("ANALYSRAPPORT · TELERIK DOCUMENT PROCESSING");

        coverEditor.DrawBlock(titleBlock, new System.Windows.Size(480, double.PositiveInfinity));
        coverEditor.Position.Translate(57, 200);
        coverEditor.DrawBlock(titleBlock, new System.Windows.Size(480, double.PositiveInfinity));

        // Company + Survey title
        var companyBlock = new Block();
        companyBlock.SetBold(true);
        companyBlock.TextProperties.FontSize = 28;
        companyBlock.TextProperties.FillColor = new RgbColor(255, 255, 255);
        companyBlock.InsertText(data.SurveyTitle);
        coverEditor.Position.Translate(57, 220);
        coverEditor.DrawBlock(companyBlock, new System.Windows.Size(480, double.PositiveInfinity));

        var subBlock = new Block();
        subBlock.TextProperties.FontSize = 14;
        subBlock.TextProperties.FillColor = new RgbColor(148, 163, 184);
        subBlock.InsertText(data.CompanyName);
        coverEditor.Position.Translate(57, 270);
        coverEditor.DrawBlock(subBlock, new System.Windows.Size(480, double.PositiveInfinity));

        // KPI strip
        DrawKpiStrip(coverEditor, data);

        // ── Data page ────────────────────────────────────────────────────
        var dataPage = document.Pages.AddPage();
        dataPage.Size = new System.Windows.Size(595, 842);
        var dataEditor = new FixedContentEditor(dataPage);

        // Page header
        dataEditor.GraphicProperties.FillColor = new RgbColor(15, 45, 74);
        dataEditor.DrawRectangle(new System.Windows.Rect(0, 0, 595, 48));

        var pageHeaderBlock = new Block();
        pageHeaderBlock.SetBold(true);
        pageHeaderBlock.TextProperties.FontSize = 11;
        pageHeaderBlock.TextProperties.FillColor = new RgbColor(255, 255, 255);
        pageHeaderBlock.InsertText($"{data.SurveyTitle}  ·  Genererad {data.GeneratedAt:yyyy-MM-dd}");
        dataEditor.Position.Translate(30, 16);
        dataEditor.DrawBlock(pageHeaderBlock, new System.Windows.Size(535, double.PositiveInfinity));

        // Table
        DrawQuestionTable(dataEditor, data);

        // Footer
        DrawFooter(dataEditor, data);

        // ── Serialize ─────────────────────────────────────────────────────
        var provider = new PdfFormatProvider();
        using var ms = new MemoryStream();
        provider.Export(document, ms);
        return Task.FromResult(ms.ToArray());
    }

    private static void DrawKpiStrip(FixedContentEditor editor, ReportDataViewModel data)
    {
        var kpis = new[]
        {
            (data.QuestionCount.ToString(), "FRÅGOR"),
            (data.TotalResponses.ToString(), "SVAR"),
            (data.OverallAverage, "GENOMSNITT"),
            (data.CategoryCount.ToString(), "KATEGORIER"),
        };

        double stripY = 360;
        double stripH = 80;
        double cellW  = 595.0 / kpis.Length;

        editor.GraphicProperties.FillColor = new RgbColor(30, 58, 95);
        editor.DrawRectangle(new System.Windows.Rect(0, stripY, 595, stripH));

        for (int i = 0; i < kpis.Length; i++)
        {
            double x = i * cellW + 20;

            var valBlock = new Block();
            valBlock.SetBold(true);
            valBlock.TextProperties.FontSize = 22;
            valBlock.TextProperties.FillColor = new RgbColor(96, 165, 250);
            valBlock.InsertText(kpis[i].Item1);
            editor.Position.Translate(x, stripY + 10);
            editor.DrawBlock(valBlock, new System.Windows.Size(cellW - 20, double.PositiveInfinity));

            var lblBlock = new Block();
            lblBlock.TextProperties.FontSize = 8;
            lblBlock.TextProperties.FillColor = new RgbColor(148, 163, 184);
            lblBlock.InsertText(kpis[i].Item2);
            editor.Position.Translate(x, stripY + 44);
            editor.DrawBlock(lblBlock, new System.Windows.Size(cellW - 20, double.PositiveInfinity));
        }
    }

    private static void DrawQuestionTable(FixedContentEditor editor, ReportDataViewModel data)
    {
        double tableY = 68;
        double tableX = 30;
        double tableW = 535;

        // Header row
        var headers = new[] { "Fråga", "Kategori", "Snitt", "Svar" };
        var colWidths = new[] { 270.0, 120.0, 75.0, 70.0 };

        editor.GraphicProperties.FillColor = new RgbColor(15, 45, 74);
        editor.DrawRectangle(new System.Windows.Rect(tableX, tableY, tableW, 24));

        double cx = tableX + 6;
        foreach (var (header, width) in headers.Zip(colWidths))
        {
            var hBlock = new Block();
            hBlock.SetBold(true);
            hBlock.TextProperties.FontSize = 8;
            hBlock.TextProperties.FillColor = new RgbColor(255, 255, 255);
            hBlock.InsertText(header);
            editor.Position.Translate(cx, tableY + 6);
            editor.DrawBlock(hBlock, new System.Windows.Size(width - 8, double.PositiveInfinity));
            cx += width;
        }

        // Data rows
        double rowH = 22;
        double currentY = tableY + 24;
        int maxRows = Math.Min(data.QuestionSummaries.Count, 28); // avoid overflow

        for (int i = 0; i < maxRows; i++)
        {
            var q = data.QuestionSummaries[i];
            bool even = i % 2 == 0;

            editor.GraphicProperties.FillColor = even
                ? new RgbColor(255, 255, 255)
                : new RgbColor(248, 250, 252);
            editor.DrawRectangle(new System.Windows.Rect(tableX, currentY, tableW, rowH));

            var rowData = new[]
            {
                (q.Text.Length > 48 ? q.Text[..48] + "…" : q.Text, colWidths[0]),
                (q.Category, colWidths[1]),
                (q.AverageValue.ToString("F2"), colWidths[2]),
                (q.TotalResponses.ToString(), colWidths[3]),
            };

            cx = tableX + 6;
            foreach (var (text, width) in rowData)
            {
                var color = text == q.AverageValue.ToString("F2")
                    ? GetScoreColor(q.AverageValue)
                    : new RgbColor(30, 41, 59);

                var cell = new Block();
                cell.TextProperties.FontSize = 8;
                cell.TextProperties.FillColor = color;
                if (text == q.AverageValue.ToString("F2")) cell.SetBold(true);
                cell.InsertText(text);
                editor.Position.Translate(cx, currentY + 5);
                editor.DrawBlock(cell, new System.Windows.Size(width - 8, double.PositiveInfinity));
                cx += width;
            }

            // Row separator
            editor.GraphicProperties.StrokeColor = new RgbColor(226, 232, 240);
            editor.GraphicProperties.StrokeThickness = 0.5;
            editor.DrawLine(
                new System.Windows.Point(tableX, currentY + rowH),
                new System.Windows.Point(tableX + tableW, currentY + rowH));

            currentY += rowH;
        }
    }

    private static void DrawFooter(FixedContentEditor editor, ReportDataViewModel data)
    {
        double footerY = 810;
        editor.GraphicProperties.StrokeColor = new RgbColor(226, 232, 240);
        editor.GraphicProperties.StrokeThickness = 0.5;
        editor.DrawLine(
            new System.Windows.Point(30, footerY),
            new System.Windows.Point(565, footerY));

        var footerBlock = new Block();
        footerBlock.TextProperties.FontSize = 7;
        footerBlock.TextProperties.FillColor = new RgbColor(148, 163, 184);
        footerBlock.InsertText($"{data.CompanyName}  ·  {data.SurveyTitle}  ·  Genererad {data.GeneratedAt:yyyy-MM-dd}  ·  Telerik Document Processing");
        editor.Position.Translate(30, footerY + 6);
        editor.DrawBlock(footerBlock, new System.Windows.Size(535, double.PositiveInfinity));
    }

    private static RgbColor GetScoreColor(double value) => value >= 4
        ? new RgbColor(22, 163, 74)
        : value >= 3
            ? new RgbColor(217, 119, 6)
            : new RgbColor(220, 38, 38);
}
