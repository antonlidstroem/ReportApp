using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf.Security;
using Syncfusion.XlsIO;
using Syncfusion.Presentation;
using Syncfusion.Drawing;
using Syncfusion.OfficeChart; // Required for OfficeChartType and IOfficeChartSerie
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ReportApp.AnalysisEngine.Models;
using ReportApp.WebAPI.Interfaces;

namespace ReportApp.WebAPI.Providers
{
    public class SyncfusionProvider : IReportProvider
    {
        public string Name => "Syncfusion";

        // ── IReportProvider Implementation ───────────────────────────────
        // These fulfill the interface contract (Fixes CS0535 & CS0311)

        public Task<byte[]> GeneratePdfAsync(ReportDataViewModel data)
            => GeneratePdfAsync(data, false);

        public Task<byte[]> GenerateExcelAsync(ReportDataViewModel data)
            => GenerateExcelAsync(data, true);

        public Task<byte[]> GeneratePptAsync(ReportDataViewModel data)
            => GeneratePptAsync(data, true);

        // ── Sandbox Overloads (With Boolean Toggles) ─────────────────────

        public async Task<byte[]> GeneratePdfAsync(ReportDataViewModel data, bool encrypt)
        {
            using var document = new PdfDocument();

            if (encrypt)
            {
                // Syncfusion .NET Core Security API
                document.Security.KeySize = PdfEncryptionKeySize.Key256Bit;
                document.Security.Algorithm = PdfEncryptionAlgorithm.AES;
                document.Security.UserPassword = "1234";
            }

            var page = document.Pages.Add();
            var font = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
            page.Graphics.DrawString(data.SurveyTitle, font, PdfBrushes.Black, new PointF(0, 0));

            var pdfGrid = new PdfGrid();
            pdfGrid.DataSource = data.QuestionSummaries.Select(q => new {
                Fraga = q.Text,
                Kategori = q.Category,
                Snitt = q.AverageValue
            }).ToList();

            pdfGrid.Draw(page, new PointF(0, 40));

            using var ms = new MemoryStream();
            document.Save(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> GenerateExcelAsync(ReportDataViewModel data, bool useFormulas)
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

            sheet.Range["A1"].Text = "Fråga";
            sheet.Range["B1"].Text = "Kategori";
            sheet.Range["C1"].Text = "Resultat";
            sheet.Range["A1:C1"].CellStyle = headerStyle;

            for (int i = 0; i < data.QuestionSummaries.Count; i++)
            {
                var q = data.QuestionSummaries[i];
                int r = i + 2;
                sheet.Range[r, 1].Text = q.Text;
                sheet.Range[r, 2].Text = q.Category;

                if (useFormulas)
                {
                    // Demonstrating live calculation: value stored in J, referenced in C
                    sheet.Range[r, 10].Number = q.AverageValue;
                    sheet.Range[r, 3].Formula = $"=J{r}";
                    sheet.Range[r, 3].CellStyle.Color = Color.LightYellow;
                }
                else
                {
                    sheet.Range[r, 3].Number = q.AverageValue;
                }
            }

            sheet.UsedRange.AutofitColumns();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> GeneratePptAsync(ReportDataViewModel data, bool nativeCharts)
        {
            // Fully qualified to avoid ambiguity with XlsIO (Fixes CS0104)
            using var presentation = Syncfusion.Presentation.Presentation.Create();
            var slide = presentation.Slides.Add(SlideLayoutType.TitleOnly);

            // Cast to Presentation-specific IShape (Fixes CS0104)
            var titleShape = slide.Shapes[0] as Syncfusion.Presentation.IShape;
            titleShape!.TextBody.AddParagraph(data.SurveyTitle);

            if (nativeCharts && data.QuestionSummaries.Any())
            {
                // Create actual Office Chart object (Fixes CS0103 & CS0246)
                IPresentationChart chart = slide.Shapes.AddChart(100, 100, 600, 400);
                chart.ChartType = OfficeChartType.Column_Clustered;

                for (int i = 0; i < data.QuestionSummaries.Count; i++)
                {
                    chart.ChartData.SetValue(i + 2, 1, "Q" + (i + 1));
                    chart.ChartData.SetValue(i + 2, 2, data.QuestionSummaries[i].AverageValue);
                }

                var series = chart.Series.Add("Snitt");
                series.Values = chart.ChartData[2, 2, data.QuestionSummaries.Count + 1, 2];
            }

            using var ms = new MemoryStream();
            presentation.Save(ms);
            return ms.ToArray();
        }
    }
}