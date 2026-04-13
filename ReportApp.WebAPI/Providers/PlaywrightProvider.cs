using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Playwright;

namespace ReportApp.Backend.Providers
{
    public class PlaywrightProvider
    {
        public async Task<byte[]> GeneratePdfAsync(string htmlContent)
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            var page = await browser.NewPageAsync();

            // Set content and wait for network to be idle (important for charts/images)
            await page.SetContentAsync(htmlContent, new PageSetContentOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            // Generate PDF
            return await page.PdfAsync(new PagePdfOptions
            {
                Format = "A4",
                PrintBackground = true,
                Margin = new Margin { Top = "1cm", Right = "1cm", Bottom = "1cm", Left = "1cm" }
            });
        }
    }
}