using jsreport.Shared;
using jsreport.Types;
using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Providers.Generators;

public class JsReportPdfGenerator
{
    private readonly IRenderService _jsreport;

    // jsreport runs as a local webserver. Its headless Chrome can reach localhost.
    // We serve chart.umd.min.js as a static file from the .NET app so that
    // the headless Chrome in jsreport can load it without internet access.
    //
    // Run wwwroot/download-chartjs.sh once to get the real Chart.js.
    // The fallback shim renders simple bar charts so exports still work.
    private const string ChartJsUrl = "http://localhost:5207/chart.umd.min.js";

    public JsReportPdfGenerator(IRenderService jsreport)
    {
        _jsreport = jsreport;
    }

    public async Task<byte[]> GenerateAsync(ReportDataViewModel data, string htmlTemplate)
    {
        // Server-side Handlebars helper functions.
        // These are passed as a JS string and compiled by jsreport's Handlebars engine.
        // gt/lt/gte/lte/eq are used by the rich templates for conditional styling.
        var helpersJs = """
function gt(a, b) { return Number(a) > Number(b); }
function lt(a, b) { return Number(a) < Number(b); }
function gte(a, b) { return Number(a) >= Number(b); }
function lte(a, b) { return Number(a) <= Number(b); }
function eq(a, b) { return String(a) === String(b); }
""";

        // Replace any CDN Chart.js references with our localhost URL
        var processedTemplate = htmlTemplate
            .Replace("https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js", ChartJsUrl)
            .Replace("https://cdn.jsdelivr.net/npm/chart.js@4/dist/chart.umd.min.js", ChartJsUrl)
            .Replace("https://cdn.jsdelivr.net/npm/chart.js/dist/chart.umd.min.js", ChartJsUrl);

        var res = await _jsreport.RenderAsync(new RenderRequest
        {
            Template = new Template
            {
                Content  = processedTemplate,
                Engine   = Engine.Handlebars,
                Recipe   = Recipe.ChromePdf,
                Helpers  = helpersJs,
                Chrome   = new Chrome
                {
                    PrintBackground = true,
                    MarginTop    = "1cm",
                    MarginBottom = "1cm",
                    MarginLeft   = "1cm",
                    MarginRight  = "1cm",
                    // WaitForNetworkIdle does NOT exist on jsreport.Types.Chrome
                    // Chrome waits for the page to stabilise automatically.
                }
            },
            Data = data
        });

        using var ms = new MemoryStream();
        await res.Content.CopyToAsync(ms);
        return ms.ToArray();
    }
}
