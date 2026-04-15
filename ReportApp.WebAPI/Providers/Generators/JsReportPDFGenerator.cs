using jsreport.Shared;
using jsreport.Types;
using ReportApp.AnalysisEngine.Models;

namespace ReportApp.WebAPI.Providers.Generators;

public class JsReportPdfGenerator
{
    private readonly IRenderService _jsreport;

    public JsReportPdfGenerator(IRenderService jsreport)
    {
        _jsreport = jsreport;
    }

    public async Task<byte[]> GenerateAsync(ReportDataViewModel data, string htmlTemplate)
    {
        // Inject Handlebars helper script at the bottom of the template body.
        // This registers gt/lt helpers required by the rich templates.
        // We inject before </body> if present, otherwise append.
        const string helpersScript = @"
<script>
  if (typeof Handlebars !== 'undefined') {
    Handlebars.registerHelper('gt', function(a, b) { return Number(a) > Number(b); });
    Handlebars.registerHelper('lt', function(a, b) { return Number(a) < Number(b); });
    Handlebars.registerHelper('gte', function(a, b) { return Number(a) >= Number(b); });
    Handlebars.registerHelper('lte', function(a, b) { return Number(a) <= Number(b); });
    Handlebars.registerHelper('eq', function(a, b) { return a === b; });
  }
</script>";

        // jsreport's Handlebars helpers are registered server-side, not via script tags.
        // Instead, provide a helpers string in the template configuration.
        var helpersJs = @"
function gt(a, b) { return Number(a) > Number(b); }
function lt(a, b) { return Number(a) < Number(b); }
function gte(a, b) { return Number(a) >= Number(b); }
function lte(a, b) { return Number(a) <= Number(b); }
function eq(a, b) { return a == b; }
";

        var res = await _jsreport.RenderAsync(new RenderRequest
        {
            Template = new Template
            {
                Content = htmlTemplate,
                Engine = Engine.Handlebars,
                Recipe = Recipe.ChromePdf,
                Helpers = helpersJs,
                Chrome = new Chrome
                {
                    PrintBackground = true,
                    MarginTop = "1cm",
                    MarginBottom = "1cm",
                    MarginLeft = "1cm",
                    MarginRight = "1cm",
                    WaitForNetworkIdle = true
                }
            },
            Data = data
        });

        using var ms = new MemoryStream();
        await res.Content.CopyToAsync(ms);
        return ms.ToArray();
    }
}
