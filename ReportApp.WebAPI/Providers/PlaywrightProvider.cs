using Microsoft.Playwright;

namespace ReportApp.Backend.Providers;

public class PlaywrightProvider : IAsyncDisposable
{
    private IPlaywright? _playwright;
    private IBrowser?    _browser;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private async Task<IBrowser> GetBrowserAsync()
    {
        if (_browser != null) return _browser;

        await _lock.WaitAsync();
        try
        {
            if (_browser == null)
            {
                _playwright = await Playwright.CreateAsync();
                _browser    = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true
                });
            }
            return _browser;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<byte[]> GeneratePdfAsync(string htmlContent)
    {
        var browser = await GetBrowserAsync();

        await using var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.SetContentAsync(htmlContent, new PageSetContentOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });

        return await page.PdfAsync(new PagePdfOptions
        {
            Format          = "A4",
            PrintBackground = true,
            Margin          = new Margin { Top = "1cm", Right = "1cm", Bottom = "1cm", Left = "1cm" }
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser != null) await _browser.CloseAsync();
        _playwright?.Dispose();
        GC.SuppressFinalize(this);
    }
}
