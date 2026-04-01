using ReportApp.Core.Data;
using ReportApp.AnalysisEngine.Services;
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

// ── Report Providers ──────────────────────────────────────────────────────────
// Activate the providers you have installed.
// Each can be toggled independently.

builder.Services.AddScoped<IReportProvider, QuestOpenSourceProvider>();
// builder.Services.AddScoped<IReportProvider, IronSuiteProvider>();
// builder.Services.AddScoped<IReportProvider, JsReportProvider>();

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
