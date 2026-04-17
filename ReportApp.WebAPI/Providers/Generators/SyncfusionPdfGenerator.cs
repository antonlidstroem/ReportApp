using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Drawing;
using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Providers.Generators;

public class SyncfusionPdfGenerator : IReportGenerator
{
    public async Task<byte[]> GenerateAsync(ReportDataViewModel data)
    {
        using var document = new PdfDocument();
        document.DocumentInformation.Title   = data.SurveyTitle;
        document.DocumentInformation.Author  = data.CompanyName;
        document.DocumentInformation.Subject = "Arbetsmiljöanalys";

        // ── Cover page ────────────────────────────────────────────────────
        var coverPage = document.Pages.Add();
        var g = coverPage.Graphics;
        float w = coverPage.GetClientSize().Width;
        float h = coverPage.GetClientSize().Height;

        // Dark gradient background (simulated with a solid rect)
        g.DrawRectangle(new PdfSolidBrush(Color.FromArgb(255, 15, 45, 74)), new RectangleF(0, 0, w, h));

        // Accent bar
        g.DrawRectangle(new PdfSolidBrush(Color.FromArgb(255, 20, 184, 166)), new RectangleF(0, 0, 6, h));

        // Eyebrow
        var eyebrowFont = new PdfStandardFont(PdfFontFamily.Helvetica, 9);
        g.DrawString("ANALYSRAPPORT", eyebrowFont,
            new PdfSolidBrush(Color.FromArgb(180, 255, 255, 255)),
            new PointF(40, 160));

        // Survey title
        var titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 26, PdfFontStyle.Bold);
        var titleBrush = new PdfSolidBrush(Color.White);

        // Word-wrap the title manually
        var words = data.SurveyTitle.Split(' ');
        var line1 = string.Join(" ", words.Take(4));
        var line2 = words.Length > 4 ? string.Join(" ", words.Skip(4)) : null;
        g.DrawString(line1, titleFont, titleBrush, new PointF(40, 185));
        if (line2 != null)
            g.DrawString(line2, titleFont, titleBrush, new PointF(40, 220));

        // Company & date
        var metaFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
        var metaBrush = new PdfSolidBrush(Color.FromArgb(160, 255, 255, 255));
        g.DrawString(data.CompanyName, metaFont, metaBrush,
            new PointF(40, line2 != null ? 268 : 240));
        g.DrawString($"Genererad: {data.GeneratedAt:yyyy-MM-dd}", metaFont, metaBrush,
            new PointF(40, line2 != null ? 292 : 264));

        // KPI boxes at bottom of cover
        var kpis = new[]
        {
            (data.QuestionCount.ToString(),    "FRÅGOR"),
            (data.TotalResponses.ToString(),   "SVAR"),
            (data.OverallAverage,              "GENOMSNITT"),
            (data.CategoryCount.ToString(),    "KATEGORIER"),
        };

        float boxY  = h - 140;
        float boxW  = (w - 80) / 4f;
        var kpiValFont = new PdfStandardFont(PdfFontFamily.Helvetica, 22, PdfFontStyle.Bold);
        var kpiLblFont = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
        var kpiBg  = new PdfSolidBrush(Color.FromArgb(40, 255, 255, 255));
        var kpiBorder = new PdfPen(Color.FromArgb(60, 255, 255, 255), 1f);

        for (int i = 0; i < kpis.Length; i++)
        {
            float x = 40 + i * boxW;
            g.DrawRectangle(kpiBorder, kpiBg, new RectangleF(x + 4, boxY, boxW - 8, 70));
            g.DrawString(kpis[i].Item1, kpiValFont,
                new PdfSolidBrush(Color.FromArgb(255, 20, 184, 166)),
                new PointF(x + 14, boxY + 12));
            g.DrawString(kpis[i].Item2, kpiLblFont, metaBrush,
                new PointF(x + 14, boxY + 48));
        }

        // ── Data page ─────────────────────────────────────────────────────
        if (!data.QuestionSummaries.Any())
            return FinalizeDocument(document);

        var dataPage = document.Pages.Add();
        var dg = dataPage.Graphics;
        float dw = dataPage.GetClientSize().Width;

        // Page header
        var hdrFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
        var hdrBrush = new PdfSolidBrush(Color.FromArgb(255, 15, 45, 74));
        dg.DrawString("RESULTAT PER FRÅGA", hdrFont, hdrBrush, new PointF(0, 0));
        dg.DrawRectangle(new PdfSolidBrush(Color.FromArgb(255, 20, 184, 166)),
            new RectangleF(0, 16, dw, 2));

        // Build grid
        var grid = new PdfGrid();
        grid.Columns.Add(4);
        grid.Columns[0].Width = dw * 0.44f;
        grid.Columns[1].Width = dw * 0.18f;
        grid.Columns[2].Width = dw * 0.13f;
        grid.Columns[3].Width = dw * 0.25f;

        // Header row
        var headerRow = grid.Headers.Add(1)[0];
        headerRow.Cells[0].Value = "Fråga";
        headerRow.Cells[1].Value = "Kategori";
        headerRow.Cells[2].Value = "Snitt";
        headerRow.Cells[3].Value = "Fördelning";

        var headerStyle = new PdfGridCellStyle
        {
            BackgroundBrush = new PdfSolidBrush(Color.FromArgb(255, 15, 45, 74)),
            TextBrush       = PdfBrushes.White,
            Font            = new PdfStandardFont(PdfFontFamily.Helvetica, 9, PdfFontStyle.Bold),
        };
        //headerRow.ApplyStyle(new PdfGridRowStyle { Font = headerStyle.Font });
        for (int i = 0; i < 4; i++)
        {
            headerRow.Cells[i].Style.BackgroundBrush = headerStyle.BackgroundBrush;
            headerRow.Cells[i].Style.TextBrush       = headerStyle.TextBrush;
            headerRow.Cells[i].Style.Font            = headerStyle.Font;
        }

        // Data rows
        bool alternate = false;
        foreach (var q in data.QuestionSummaries)
        {
            var row = grid.Rows.Add();
            row.Cells[0].Value = q.Text.Length > 70 ? q.Text[..67] + "…" : q.Text;
            row.Cells[1].Value = q.Category;
            row.Cells[2].Value = q.AverageValue.ToString("F2");

            // Simple text bar using Unicode blocks
            int filled = (int)Math.Round(q.AverageValue / 5.0 * 20);
            row.Cells[3].Value = new string('█', filled) + new string('░', 20 - filled);

            var scoreColor = q.AverageValue >= 4
                ? Color.FromArgb(255, 22, 163, 74)
                : q.AverageValue >= 3
                    ? Color.FromArgb(255, 217, 119, 6)
                    : Color.FromArgb(255, 220, 38, 38);

            var bg = alternate
                ? Color.FromArgb(255, 248, 250, 252)
                : Color.White;

            var bodyFont = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
            for (int i = 0; i < 4; i++)
            {
                row.Cells[i].Style.BackgroundBrush = new PdfSolidBrush(bg);
                row.Cells[i].Style.Font = bodyFont;
                row.Cells[i].Style.TextBrush = i == 2
                    ? new PdfSolidBrush(scoreColor)
                    : PdfBrushes.Black;
            }
            row.Cells[3].Style.Font = new PdfStandardFont(PdfFontFamily.Courier, 7);
            row.Cells[3].Style.TextBrush = new PdfSolidBrush(scoreColor);

            alternate = !alternate;
        }

        grid.Draw(dataPage, new PointF(0, 26));

        // Footer on data page
        var footFont = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
        var footBrush = new PdfSolidBrush(Color.Gray);
        dg.DrawString(
            $"{data.CompanyName}  ·  {data.SurveyTitle}  ·  {data.GeneratedAt:yyyy-MM-dd}",
            footFont, footBrush,
            new PointF(0, dataPage.GetClientSize().Height - 18));

        return FinalizeDocument(document);
    }

    private static byte[] FinalizeDocument(PdfDocument document)
    {
        using var ms = new MemoryStream();
        document.Save(ms);
        return ms.ToArray();
    }
}
