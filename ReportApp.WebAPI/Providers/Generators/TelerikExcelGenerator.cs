using Telerik.Windows.Documents.Spreadsheet.FormatProviders;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;
using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Providers.Generators;

public class TelerikExcelGenerator
{
    public Task<byte[]> GenerateAsync(ReportDataViewModel data)
    {
        var workbook  = new Workbook();
        var worksheet = workbook.Worksheets.Add();
        worksheet.Name = TruncateSheetName(data.SurveyTitle);

        var cells = worksheet.Cells;

        // ── Column widths ──────────────────────────────────────────────
        worksheet.Columns[0].SetWidth(new ColumnWidth(300, true));
        worksheet.Columns[1].SetWidth(new ColumnWidth(120, true));
        worksheet.Columns[2].SetWidth(new ColumnWidth(80, true));
        worksheet.Columns[3].SetWidth(new ColumnWidth(80, true));
        worksheet.Columns[4].SetWidth(new ColumnWidth(80, true));

        // ── Title row ──────────────────────────────────────────────────
        cells[0, 0].SetValue(data.SurveyTitle);
        cells[0, 0].SetFontSize(16);
        cells[0, 0].SetIsBold(true);
        cells[0, 0].SetForeColor(ThemeColorType.Accent1);

        cells[1, 0].SetValue($"{data.CompanyName} · Genererad {data.GeneratedAt:yyyy-MM-dd}");
        cells[1, 0].SetFontSize(10);
        cells[1, 0].SetForeColor(new ThemableColor(new Telerik.Windows.Documents.Spreadsheet.Model.Color(100, 116, 139)));

        // ── KPI summary row ────────────────────────────────────────────
        cells[3, 0].SetValue("Frågor");
        cells[3, 1].SetValue(data.QuestionCount);
        cells[3, 2].SetValue("Svar");
        cells[3, 3].SetValue(data.TotalResponses);
        cells[3, 4].SetValue("Genomsnitt");
        cells[3, 5].SetValue(double.Parse(data.OverallAverage, System.Globalization.CultureInfo.InvariantCulture));

        foreach (var col in new[] { 0, 2, 4 })
        {
            cells[3, col].SetIsBold(true);
            cells[3, col].SetFontSize(9);
            cells[3, col].SetForeColor(new ThemableColor(new Telerik.Windows.Documents.Spreadsheet.Model.Color(100, 116, 139)));
        }

        // ── Header row ─────────────────────────────────────────────────
        int headerRow = 5;
        var headers = new[] { "Fråga", "Kategori", "Genomsnitt", "Formelvärde", "Svar" };

        for (int col = 0; col < headers.Length; col++)
        {
            cells[headerRow, col].SetValue(headers[col]);
            cells[headerRow, col].SetIsBold(true);
            cells[headerRow, col].SetFontSize(9);

            var fill = new PatternFill(PatternType.Solid,
                new ThemableColor(new Telerik.Windows.Documents.Spreadsheet.Model.Color(15, 45, 74)),
                new ThemableColor(Telerik.Windows.Documents.Spreadsheet.Model.Color.FromArgb(255, 255, 255, 255)));
            cells[headerRow, col].SetFill(fill);
            cells[headerRow, col].SetForeColor(new ThemableColor(Telerik.Windows.Documents.Spreadsheet.Model.Color.FromArgb(255, 255, 255, 255)));
        }

        // ── Data rows ──────────────────────────────────────────────────
        for (int i = 0; i < data.QuestionSummaries.Count; i++)
        {
            int row = headerRow + 1 + i;
            var q = data.QuestionSummaries[i];

            cells[row, 0].SetValue(q.Text);
            cells[row, 1].SetValue(q.Category);
            cells[row, 2].SetValue(q.AverageValue);

            // Real formula — recalculates if user edits helper columns
            // Store average in a helper column (D) so AVERAGE formula works
            cells[row, 3].SetValue(q.AverageValue); // raw value in col D
            cells[row, 2].SetValueAsFormula($"=D{row + 1}"); // formula in col C references D

            cells[row, 4].SetValue(q.TotalResponses);

            // Format average cell
            cells[row, 2].SetFormat(new CellValueFormat("0.00"));
            cells[row, 2].SetIsBold(true);

            // Conditional color via font color (Telerik free tier doesn't support ConditionalFormatting UI rules without the paid UI)
            var scoreColor = q.AverageValue >= 4
                ? new Telerik.Windows.Documents.Spreadsheet.Model.Color(22, 163, 74)
                : q.AverageValue >= 3
                    ? new Telerik.Windows.Documents.Spreadsheet.Model.Color(217, 119, 6)
                    : new Telerik.Windows.Documents.Spreadsheet.Model.Color(220, 38, 38);
            cells[row, 2].SetForeColor(new ThemableColor(scoreColor));

            // Alternating row background
            if (i % 2 != 0)
            {
                var rowFill = new PatternFill(PatternType.Solid,
                    new ThemableColor(new Telerik.Windows.Documents.Spreadsheet.Model.Color(248, 250, 252)),
                    new ThemableColor(Telerik.Windows.Documents.Spreadsheet.Model.Color.FromArgb(255, 255, 255, 255)));
                cells[row, 0].SetFill(rowFill);
                cells[row, 1].SetFill(rowFill);
                cells[row, 2].SetFill(rowFill);
                cells[row, 3].SetFill(rowFill);
                cells[row, 4].SetFill(rowFill);
            }
        }

        // ── Total row ──────────────────────────────────────────────────
        int totalRow = headerRow + 1 + data.QuestionSummaries.Count;
        cells[totalRow, 0].SetValue("Totalt / Genomsnitt");
        cells[totalRow, 0].SetIsBold(true);

        int firstData = headerRow + 2;
        int lastData  = totalRow;
        cells[totalRow, 2].SetValueAsFormula($"=AVERAGE(C{firstData}:C{lastData})");
        cells[totalRow, 2].SetFormat(new CellValueFormat("0.00"));
        cells[totalRow, 2].SetIsBold(true);
        cells[totalRow, 4].SetValueAsFormula($"=SUM(E{firstData}:E{lastData})");
        cells[totalRow, 4].SetIsBold(true);

        // ── Export ─────────────────────────────────────────────────────
        IWorkbookFormatProvider xlsxProvider = new XlsxFormatProvider();
        using var ms = new MemoryStream();
        xlsxProvider.Export(workbook, ms);
        return Task.FromResult(ms.ToArray());
    }

    private static string TruncateSheetName(string name)
    {
        var clean = string.Concat(name.Where(c => !"\\/?*[]".Contains(c)));
        return clean.Length > 31 ? clean[..31] : clean;
    }
}
