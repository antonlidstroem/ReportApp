using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.XlsIO;
using Syncfusion.Presentation;
using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;
using Syncfusion.Drawing; // Essential for PointF and Color

namespace ReportApp.WebAPI.Providers;

public class SyncfusionProvider : IReportProvider
{
    public string Name => "Syncfusion";

    // ── PDF GENERATION ──────────────────────────────────────────────────
    public async Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
    {
        using var document = new PdfDocument();
        var page = document.Pages.Add();
        var font = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);

        page.Graphics.DrawString(data.SurveyTitle, font, PdfBrushes.Black, new PointF(0, 0));

        var pdfGrid = new PdfGrid();
        pdfGrid.DataSource = data.QuestionSummaries.Select(q => new {
            Fraga = q.Text,
            Kategori = q.Category,
            Snitt = q.AverageValue,
            Svar = q.TotalResponses
        }).ToList();

        pdfGrid.Style.CellPadding = new PdfPaddings(5, 5, 5, 5);
        pdfGrid.Draw(page, new PointF(0, 40));

        using var ms = new MemoryStream();
        document.Save(ms);
        return ms.ToArray();
    }

    // ── EXCEL GENERATION ────────────────────────────────────────────────
    public async Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
    {
        MemoryStream ms = new MemoryStream();

        try
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                // 1. This handles the format for the whole operation
                application.DefaultVersion = ExcelVersion.Xlsx;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];
                sheet.Name = "Analysis";

                if (data == null || data.QuestionSummaries == null)
                {
                    sheet.Range["A1"].Text = "Ingen data tillgänglig.";
                    workbook.SaveAs(ms);
                    return ms.ToArray();
                }

                // [Headers and Data Logic - Keep exactly as you have it]
                sheet.Range["A1"].Text = "Fråga";
                sheet.Range["B1"].Text = "Kategori";
                sheet.Range["C1"].Text = "Snittvärde";
                sheet.Range["D1"].Text = "Antal svar";

                var headerRange = sheet.Range["A1:D1"];
                headerRange.CellStyle.Font.Bold = true;
                headerRange.CellStyle.Font.Color = Syncfusion.XlsIO.ExcelKnownColors.White;
                headerRange.CellStyle.Color = Syncfusion.Drawing.Color.FromArgb(255, 30, 58, 95);
                headerRange.CellStyle.HorizontalAlignment = Syncfusion.XlsIO.ExcelHAlign.HAlignCenter;

                for (int i = 0; i < data.QuestionSummaries.Count; i++)
                {
                    var q = data.QuestionSummaries[i];
                    int r = i + 2;
                    sheet.Range[r, 1].Text = q.Text;
                    sheet.Range[r, 2].Text = q.Category;
                    sheet.Range[r, 3].Number = q.AverageValue;
                    sheet.Range[r, 4].Number = q.TotalResponses;

                    if (i % 2 == 0)
                    {
                        sheet.Range[r, 1, r, 4].CellStyle.Color = Syncfusion.Drawing.Color.FromArgb(255, 245, 247, 250);
                    }
                }

                sheet.UsedRange.AutofitColumns();

                // 2. THE FIX: Just use SaveAs(ms). 
                // It will respect the DefaultVersion (Xlsx) set above.
                workbook.SaveAs(ms);

                // 3. Keep the Close() call - it flushes the buffers!
                workbook.Close();
            }

            return ms.ToArray();
        }
        finally
        {
            ms.Dispose();
        }
    }

    // ── POWERPOINT GENERATION ──────────────────────────────────────────
    public async Task<byte[]> GeneratePptAsync(ReportDataViewModel data)
    {
        using var presentation = Presentation.Create();
        var slide = presentation.Slides.Add(SlideLayoutType.TitleOnly);

        var titleShape = slide.Shapes[0] as Syncfusion.Presentation.IShape;
        titleShape!.TextBody.AddParagraph(data.SurveyTitle);

        var contentSlide = presentation.Slides.Add(SlideLayoutType.Blank);
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