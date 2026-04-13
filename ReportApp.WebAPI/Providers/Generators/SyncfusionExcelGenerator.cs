using Syncfusion.XlsIO;
using Syncfusion.Drawing;
using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Providers.Generators;

public class SyncfusionExcelGenerator : IReportGenerator
{
    public async Task<byte[]> GenerateAsync(ReportDataViewModel data)
    {
        using var excelEngine = new ExcelEngine();
        IApplication application = excelEngine.Excel;
        application.DefaultVersion = ExcelVersion.Xlsx;

        var workbook = application.Workbooks.Create(1);
        var sheet = workbook.Worksheets[0];

        // Header Style
        var headerStyle = workbook.Styles.Add("HeaderStyle");
        headerStyle.Font.Bold = true;
        headerStyle.Color = Color.FromArgb(255, 30, 58, 95);
        headerStyle.Font.Color = ExcelKnownColors.White;

        sheet.Range["A1:C1"].CellStyle = headerStyle;
        sheet.Range["A1"].Text = "Fråga";
        sheet.Range["B1"].Text = "Kategori";
        sheet.Range["C1"].Text = "Resultat";

        for (int i = 0; i < data.QuestionSummaries.Count; i++)
        {
            var q = data.QuestionSummaries[i];
            int r = i + 2;
            sheet.Range[r, 1].Text = q.Text;
            sheet.Range[r, 2].Text = q.Category;
            sheet.Range[r, 3].Number = q.AverageValue;
        }

        sheet.UsedRange.AutofitColumns();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }
}