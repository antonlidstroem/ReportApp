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
var jsreportTempDir = Path.Combine(Path.GetTempPath(), "reportapp_jsreport");
Directory.CreateDirectory(jsreportTempDir);

builder.Services.AddJsReport(new LocalReporting()
    .UseBinary(JsReportBinary.GetBinary())
    .KillRunningJsReportProcesses()
    .TempDirectory(jsreportTempDir)
    .AsWebServer()
    .Create());

// ── Syncfusion license ────────────────────────────────────────────────────
var syncfusionKey = builder.Configuration["Syncfusion:LicenseKey"];
if (!string.IsNullOrWhiteSpace(syncfusionKey))
    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionKey);

// ── Report Providers ──────────────────────────────────────────────────────
builder.Services.AddScoped<IReportProvider, SyncfusionProvider>();   // "Syncfusion"
builder.Services.AddScoped<IReportProvider, JsReportProvider>();      // "jsreport"
builder.Services.AddScoped<IReportProvider, JsSyncHybridProvider>();  // "js-sync"

// PlaywrightProvider holds a long-lived IBrowser — must be Singleton
builder.Services.AddSingleton<PlaywrightProvider>();
builder.Services.AddScoped<IReportProvider, PlaySyncHybridProvider>(); // "play-sync"

// Telerik Document Processing (free — no license required for document processing)
builder.Services.AddScoped<IReportProvider, TelerikProvider>();       // "Telerik"

// DevExpress — stub until a Universal license is obtained
// See DevExpressProvider.cs for setup instructions.
builder.Services.AddScoped<IReportProvider, DevExpressProvider>();    // "DevExpress"

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

app.UseCors("VuePolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
