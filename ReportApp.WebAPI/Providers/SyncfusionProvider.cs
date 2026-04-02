using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.XlsIO;
using Syncfusion.Presentation;
using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;
// 1. REMOVE System.Drawing and REPLACE with Syncfusion.Drawing
using Syncfusion.Drawing;

namespace ReportApp.WebAPI.Providers;

public class SyncfusionProvider : IReportProvider
{
    public string Name => "Syncfusion";

    public async Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
    {
        using var document = new PdfDocument();
        var page = document.Pages.Add();
        var graphics = page.Graphics;

        // Use Syncfusion's standard font
        var font = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);

        // 2. Fixed PointF: Now uses Syncfusion.Drawing.PointF
        graphics.DrawString(data.SurveyTitle, font, PdfBrushes.Black, new PointF(0, 0));

        var pdfGrid = new PdfGrid();
        var gridData = data.QuestionSummaries.Select(q => new {
            Fraga = q.Text,
            Kategori = q.Category,
            Snitt = q.AverageValue,
            Svar = q.TotalResponses
        }).ToList();

        pdfGrid.DataSource = gridData;
        pdfGrid.Style.CellPadding = new PdfPaddings(5, 5, 5, 5);

        // 3. Fixed Draw: Use the overload that takes the page and a PointF/RectangleF
        pdfGrid.Draw(page, new PointF(0, 40));

        using var ms = new MemoryStream();
        document.Save(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
    {
        using var excelEngine = new ExcelEngine();
        var application = excelEngine.Excel;
        var workbook = application.Workbooks.Create(1);
        var sheet = workbook.Worksheets[0];

        sheet.Range["A1"].Text = "Fråga";
        sheet.Range["B1"].Text = "Kategori";
        sheet.Range["C1"].Text = "Snittvärde";
        sheet.Range["A1:C1"].CellStyle.Font.Bold = true;

        for (int i = 0; i < data.QuestionSummaries.Count; i++)
        {
            var q = data.QuestionSummaries[i];
            sheet.Range[i + 2, 1].Text = q.Text;
            sheet.Range[i + 2, 2].Text = q.Category;
            sheet.Range[i + 2, 3].Number = q.AverageValue;
        }

        sheet.UsedRange.AutofitColumns();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> GeneratePptAsync(ReportDataViewModel data)
    {
        using var presentation = Presentation.Create();
        var slide = presentation.Slides.Add(SlideLayoutType.TitleOnly);

        // 4. Fixed Ambiguity: Explicitly tell the compiler we mean the Presentation IShape
        Syncfusion.Presentation.IShape titleShape = slide.Shapes[0] as Syncfusion.Presentation.IShape;
        titleShape.TextBody.AddParagraph(data.SurveyTitle);

        var contentSlide = presentation.Slides.Add(SlideLayoutType.Blank);

        // Fixed Table: Ensure the size/position uses Syncfusion.Drawing logic
        var table = contentSlide.Shapes.AddTable(data.QuestionSummaries.Count + 1, 2, 50, 50, 600, 300);

        table.Rows[0].Cells[0].TextBody.AddParagraph("Fråga");
        table.Rows[0].Cells[1].TextBody.AddParagraph("Resultat");

        for (int i = 0; i < data.QuestionSummaries.Count; i++)
        {
            table.Rows[i + 1].Cells[0].TextBody.AddParagraph(data.QuestionSummaries[i].Text);
            table.Rows[i + 1].Cells[1].TextBody.AddParagraph(data.QuestionSummaries[i].AverageValue.ToString());
        }

        using var ms = new MemoryStream();
        presentation.Save(ms);
        return ms.ToArray();
    }
}