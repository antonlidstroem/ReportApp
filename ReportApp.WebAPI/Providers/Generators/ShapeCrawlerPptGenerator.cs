using System.Diagnostics;
using ShapeCrawler;
using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Providers.Generators;

public class ShapeCrawlerPptGenerator : IReportGenerator
{
    public Task<byte[]> GenerateAsync(ReportDataViewModel data)
    {
        try
        {
            var prs = new ShapeCrawler.Presentation();

            if (prs.Slides.Count == 0)
            {
                throw new Exception("ShapeCrawler: Inga slides hittades. Biblioteket kräver en mall (.pptx) för att fungera stabilt.");
            }

            var slide = prs.Slides[0];

            // Här lägger du din ShapeCrawler-logik (Rektanglar, text etc.)
            // slide.Shapes.AddRectangle(...)

            using var ms = new MemoryStream();
            prs.SaveAs(ms);
            return Task.FromResult(ms.ToArray());
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"CRITICAL PPT ERROR: {ex.Message}");
            throw;
        }
    }
}