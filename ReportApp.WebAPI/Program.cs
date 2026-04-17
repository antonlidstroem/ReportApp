using jsreport.AspNetCore;
using jsreport.Binary;
using jsreport.Local;
using ReportApp.AnalysisEngine.Services;
using ReportApp.Backend.Providers;
using ReportApp.Core.Data;
using ReportApp.WebAPI.Interfaces;
using ReportApp.WebAPI.Providers;

var builder = WebApplication.CreateBuilder(args);

// ── JSON / Controllers ────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Force camelCase for all serialized objects including anonymous types
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "ReportApp PoC API", Version = "v1" }));

// ── Database & Analysis ───────────────────────────────────────────────────
builder.Services.AddDbContext<ReportDbContext>();
builder.Services.AddScoped<AnalysisService>();

// ── jsreport ──────────────────────────────────────────────────────────────
// jsreport needs a writable temp directory; do NOT use the app directory.
var jsreportTempDir = Path.Combine(Path.GetTempPath(), "reportapp_jsreport");
Directory.CreateDirectory(jsreportTempDir); // no-op if already exists

builder.Services.AddJsReport(new LocalReporting()
    .UseBinary(JsReportBinary.GetBinary())
    .KillRunningJsReportProcesses()
    .TempDirectory(jsreportTempDir)
    .AsWebServer()   // keeps the process alive — faster after first call
    .Create());

// ── Syncfusion license ────────────────────────────────────────────────────
// Add your key to appsettings.json: { "Syncfusion": { "LicenseKey": "YOUR_KEY" } }
// Community edition (free under $1M revenue) does not require a key but will
// show a pop-up warning in Office apps. Register key to suppress it.
var syncfusionKey = builder.Configuration["Syncfusion:LicenseKey"];
if (!string.IsNullOrWhiteSpace(syncfusionKey))
    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionKey);

// ── Report Providers ──────────────────────────────────────────────────────
// All providers implement IReportProvider; the controller finds them by Name.

builder.Services.AddScoped<IReportProvider, SyncfusionProvider>();   // "Syncfusion"
builder.Services.AddScoped<IReportProvider, JsReportProvider>();      // "jsreport"
builder.Services.AddScoped<IReportProvider, JsSyncHybridProvider>();  // "js-sync"

// PlaywrightProvider holds a long-lived IBrowser — must be Singleton
builder.Services.AddSingleton<PlaywrightProvider>();
builder.Services.AddScoped<IReportProvider, PlaySyncHybridProvider>(); // "play-sync"

// ── CORS ──────────────────────────────────────────────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>()
    ?? ["http://localhost:5173", "http://localhost:61704"];

builder.Services.AddCors(options =>
    options.AddPolicy("VuePolicy", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("X-Generation-Time-Ms", "X-File-Size-Bytes")));

// ── Build ──────────────────────────────────────────────────────────────────
var app = builder.Build();

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

// CORS must come before UseRouting / MapControllers
app.UseCors("VuePolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();