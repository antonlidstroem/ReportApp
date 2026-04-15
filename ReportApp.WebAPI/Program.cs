using jsreport.AspNetCore;
using jsreport.Binary;
using jsreport.Local;
using ReportApp.AnalysisEngine.Services;
using ReportApp.Backend.Providers;
using ReportApp.Core.Data;
using ReportApp.WebAPI.Interfaces;
using ReportApp.WebAPI.Providers;

var builder = WebApplication.CreateBuilder(args);

// ── Services ───────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ensure camelCase serialization for all response objects
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ReportApp PoC API", Version = "v1" });
});

// Database & analysis
builder.Services.AddDbContext<ReportDbContext>();
builder.Services.AddScoped<AnalysisService>();

// ── jsreport ──────────────────────────────────────────────────────────────
var jsreportTempDir = Path.Combine(Path.GetTempPath(), "reportapp_jsreport");
if (!Directory.Exists(jsreportTempDir)) Directory.CreateDirectory(jsreportTempDir);

builder.Services.AddJsReport(new LocalReporting()
    .UseBinary(JsReportBinary.GetBinary())
    .KillRunningJsReportProcesses()
    .TempDirectory(jsreportTempDir)
    .AsWebServer()
    .Configure(cfg =>
    {
        // Register Handlebars helpers used in templates: gt, lt, unless
        cfg.Handlebars = new jsreport.Types.HandlebarsConfiguration
        {
            Helpers = new Dictionary<string, object>()
        };
        return cfg;
    })
    .Create());

// ── Syncfusion license ────────────────────────────────────────────────────
var syncfusionKey = builder.Configuration["Syncfusion:LicenseKey"];
if (!string.IsNullOrWhiteSpace(syncfusionKey))
{
    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionKey);
}

// ── Report Providers ──────────────────────────────────────────────────────
// IMPORTANT: Registration order matters — the first matching provider is used
// when multiple providers share the same Name. Each provider has a unique Name.

// Syncfusion (pure .NET — PDF, Excel, PPT)
builder.Services.AddScoped<IReportProvider, SyncfusionProvider>();

// jsreport (HTML→PDF via Chromium + Handlebars; Excel via jsreport html-to-xlsx; PPT via Syncfusion)
builder.Services.AddScoped<IReportProvider, JsReportProvider>();

// Hybrid A: jsreport PDF + Syncfusion Excel/PPT
builder.Services.AddScoped<IReportProvider, JsSyncHybridProvider>();

// Playwright singleton (browser kept alive across requests)
builder.Services.AddSingleton<PlaywrightProvider>();

// Hybrid B: Playwright PDF + Syncfusion Excel/PPT
builder.Services.AddScoped<IReportProvider, PlaySyncHybridProvider>();

// ── CORS ──────────────────────────────────────────────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>()
    ?? ["http://localhost:5173", "http://localhost:61704"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("VuePolicy", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("X-Generation-Time-Ms", "X-File-Size-Bytes"));
});

// ── Build ──────────────────────────────────────────────────────────────────
var app = builder.Build();

// Seed database (guarded — only runs if empty)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ReportDbContext>();
    DbInitializer.Initialize(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Order matters: CORS before routing
app.UseCors("VuePolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
