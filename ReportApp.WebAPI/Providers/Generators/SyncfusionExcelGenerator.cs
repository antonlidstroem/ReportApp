using Syncfusion.XlsIO;
using Syncfusion.Drawing;
using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Providers.Generators;

public class SyncfusionExcelGenerator : IReportGenerator
{
    public async Task<byte[]> GenerateAsync(ReportDataViewModel data)
    {
        using var excelEngine = new ExcelEngine();
        var application = excelEngine.Excel;
        application.DefaultVersion = ExcelVersion.Xlsx;

        var workbook = application.Workbooks.Create(2);

        // ── Sheet 1: Summary ──────────────────────────────────────────────
        var sheet = workbook.Worksheets[0];
        sheet.Name = "Analys";

        // Company / title header block
        sheet.Range["A1"].Text = data.SurveyTitle;
        sheet.Range["A1"].CellStyle.Font.Size  = 16;
        sheet.Range["A1"].CellStyle.Font.Bold  = true;
        sheet.Range["A1"].CellStyle.Font.Color = ExcelKnownColors.Dark_blue;

        sheet.Range["A2"].Text = $"{data.CompanyName}  ·  Genererad {data.GeneratedAt:yyyy-MM-dd}";
        sheet.Range["A2"].CellStyle.Font.Color = ExcelKnownColors.Grey_50_percent;

        // KPI row
        var kpiLabels  = new[] { "Frågor", "Svar", "Genomsnitt", "Kategorier" };
        var kpiValues  = new object[] { data.QuestionCount, data.TotalResponses, data.OverallAverage, data.CategoryCount };
        for (int i = 0; i < 4; i++)
        {
            var cell = sheet.Range[4, i + 1];
            cell.Text = kpiLabels[i];
            cell.CellStyle.Font.Bold  = true;
            cell.CellStyle.Font.Size  = 9;
            cell.CellStyle.Color      = Color.FromArgb(255, 15, 45, 74);
            cell.CellStyle.Font.Color = ExcelKnownColors.White;

            var valCell = sheet.Range[5, i + 1];
            if (i == 2)
                valCell.Text   = data.OverallAverage;
            else
                valCell.Number = Convert.ToDouble(kpiValues[i]);
            valCell.CellStyle.Font.Bold = true;
            valCell.CellStyle.Font.Size = 14;
        }

        // Column headers for data table (row 7)
        var headers = new[] { "Fråga", "Kategori", "Genomsnitt", "Svar" };
        for (int c = 0; c < headers.Length; c++)
        {
            var hdr = sheet.Range[7, c + 1];
            hdr.Text = headers[c];
            hdr.CellStyle.Font.Bold  = true;
            hdr.CellStyle.Color      = Color.FromArgb(255, 15, 45, 74);
            hdr.CellStyle.Font.Color = ExcelKnownColors.White;
            hdr.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
        }

        // Data rows starting at row 8
        for (int i = 0; i < data.QuestionSummaries.Count; i++)
        {
            var q   = data.QuestionSummaries[i];
            int row = i + 8;

            sheet.Range[row, 1].Text    = q.Text;
            sheet.Range[row, 2].Text    = q.Category;
            sheet.Range[row, 3].Number  = q.AverageValue;
            sheet.Range[row, 3].NumberFormat = "0.00";
            sheet.Range[row, 4].Number  = q.TotalResponses;

            // Alternating row background
            var bg = i % 2 == 0 ? Color.White : Color.FromArgb(255, 248, 250, 252);
            for (int c = 1; c <= 4; c++)
                sheet.Range[row, c].CellStyle.Color = bg;

            // Conditional colour for average score
            var scoreColor = q.AverageValue >= 4 ? ExcelKnownColors.Green
                           : q.AverageValue >= 3 ? ExcelKnownColors.Orange
                           : ExcelKnownColors.Red;
            sheet.Range[row, 3].CellStyle.Font.Bold  = true;
            sheet.Range[row, 3].CellStyle.Font.Color = scoreColor;

            // Thin bottom border
            for (int c = 1; c <= 4; c++)
                sheet.Range[row, c].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
        }

        // Autofit
        sheet.Range[$"A1:D{7 + data.QuestionSummaries.Count}"].AutofitColumns();
        sheet.Range[$"A1:D{7 + data.QuestionSummaries.Count}"].AutofitRows();

        // ── Sheet 2: Category Summary ─────────────────────────────────────
        var catSheet = workbook.Worksheets[1];
        catSheet.Name = "Per kategori";

        catSheet.Range["A1"].Text = "Kategori";
        catSheet.Range["B1"].Text = "Antal frågor";
        catSheet.Range["C1"].Text = "Genomsnitt";

        for (int c = 1; c <= 3; c++)
        {
            catSheet.Range[1, c].CellStyle.Font.Bold  = true;
            catSheet.Range[1, c].CellStyle.Color      = Color.FromArgb(255, 15, 45, 74);
            catSheet.Range[1, c].CellStyle.Font.Color = ExcelKnownColors.White;
        }

        var categories = data.QuestionSummaries
            .GroupBy(q => q.Category)
            .OrderByDescending(g => g.Average(q => q.AverageValue))
            .ToList();

        for (int i = 0; i < categories.Count; i++)
        {
            int r = i + 2;
            var grp = categories[i];
            double avg = Math.Round(grp.Average(q => q.AverageValue), 2);

            catSheet.Range[r, 1].Text   = grp.Key;
            catSheet.Range[r, 2].Number = grp.Count();
            catSheet.Range[r, 3].Number = avg;
            catSheet.Range[r, 3].NumberFormat = "0.00";

            var scoreColor = avg >= 4 ? ExcelKnownColors.Green
                           : avg >= 3 ? ExcelKnownColors.Orange
                           : ExcelKnownColors.Red;
            catSheet.Range[r, 3].CellStyle.Font.Color = scoreColor;
            catSheet.Range[r, 3].CellStyle.Font.Bold  = true;
        }

        catSheet.Range[$"A1:C{1 + categories.Count}"].AutofitColumns();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }
}
