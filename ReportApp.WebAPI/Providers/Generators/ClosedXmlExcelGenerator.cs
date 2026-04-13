using ClosedXML.Excel;
using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Providers.Generators;

public class ClosedXmlExcelGenerator : IReportGenerator
{
    public Task<byte[]> GenerateAsync(ReportDataViewModel data)
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
}