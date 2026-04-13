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
        var page = document.Pages.Add();

        var font = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
        page.Graphics.DrawString(data.SurveyTitle, font, PdfBrushes.Black, new PointF(0, 0));

        var pdfGrid = new PdfGrid();
        pdfGrid.DataSource = data.QuestionSummaries.Select(q => new {
            Fråga = q.Text,
            Kategori = q.Category,
            Snitt = q.AverageValue
        }).ToList();

        pdfGrid.Draw(page, new PointF(0, 40));

        using var ms = new MemoryStream();
        document.Save(ms);
        return ms.ToArray();
    }
}