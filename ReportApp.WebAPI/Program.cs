using jsreport.AspNetCore;
using jsreport.Binary;
using jsreport.Local;
using ReportApp.AnalysisEngine.Services;
using ReportApp.Backend.Providers;
using ReportApp.Core.Data;
using ReportApp.WebAPI.Interfaces;
using ReportApp.WebAPI.Providers;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddDbContext<ReportDbContext>();
builder.Services.AddScoped<AnalysisService>();

var jsreportTempDir = Path.Combine(Path.GetTempPath(), "reportapp_jsreport");
Directory.CreateDirectory(jsreportTempDir);

builder.Services.AddJsReport(new LocalReporting()
    .UseBinary(JsReportBinary.GetBinary())
    .KillRunningJsReportProcesses()
    .TempDirectory(jsreportTempDir)
    .AsWebServer()
    .Create());

var syncfusionKey = builder.Configuration["Syncfusion:LicenseKey"];
if (!string.IsNullOrWhiteSpace(syncfusionKey))
    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionKey);

builder.Services.AddScoped<IReportProvider, SyncfusionProvider>();
builder.Services.AddScoped<IReportProvider, JsReportProvider>();
builder.Services.AddScoped<IReportProvider, JsSyncHybridProvider>();
builder.Services.AddSingleton<PlaywrightProvider>();
builder.Services.AddScoped<IReportProvider, PlaySyncHybridProvider>();

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

// Serve wwwroot/chart.umd.min.js so jsreport's headless Chrome can load it
// at http://localhost:5207/chart.umd.min.js (no external CDN needed)
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

app.Run();
