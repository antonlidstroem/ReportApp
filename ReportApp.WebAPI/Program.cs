using jsreport.AspNetCore;
using jsreport.Binary;
using jsreport.Local;
using jsreport.Shared;
using ReportApp.AnalysisEngine.Services;
using ReportApp.Backend.Providers;
using ReportApp.Core.Data;
using ReportApp.WebAPI.Interfaces;
using ReportApp.WebAPI.Providers;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ReportApp PoC API", Version = "v1" });
});

// Database & analysis
builder.Services.AddDbContext<ReportDbContext>();
builder.Services.AddScoped<AnalysisService>();

// Skapa temp-mapp för jsreport
var jsreportTempDir = Path.Combine(Path.GetTempPath(), "reportapp_jsreport");
if (!Directory.Exists(jsreportTempDir)) Directory.CreateDirectory(jsreportTempDir);

// Program.cs - Runt rad 20-30
builder.Services.AddJsReport(new LocalReporting()
    .UseBinary(jsreport.Binary.JsReportBinary.GetBinary())
    .KillRunningJsReportProcesses()
    .TempDirectory(jsreportTempDir)
    .AsWebServer() // ÄNDRA FRÅN AsUtility() TILL AsWebServer()
    .Create());



// 1. Register the License (Get this from Syncfusion dashboard)
var syncfusionKey = builder.Configuration["Syncfusion:LicenseKey"];
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionKey);

// 2. Register the Provider
builder.Services.AddScoped<IReportProvider, SyncfusionProvider>();

// ── Report Providers ──────────────────────────────────────────────────────────
// Activate the providers you have installed.
// Each can be toggled independently.

builder.Services.AddSingleton<PlaywrightProvider>();
// builder.Services.AddScoped<IReportProvider, IronSuiteProvider>();
builder.Services.AddScoped<IReportProvider, JsReportProvider>();
builder.Services.AddScoped<IReportProvider, JsSyncHybridProvider>(); // Om du skapat denna
builder.Services.AddScoped<IReportProvider, PlaySyncHybridProvider>();

// ── CORS ──────────────────────────────────────────────────────────────────────
// Read allowed origins from config so you don't have to touch code for port changes.
// In appsettings.Development.json add: "AllowedOrigins": ["http://localhost:5173"]
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

// ── Build ─────────────────────────────────────────────────────────────────────
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
