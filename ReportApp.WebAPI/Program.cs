using ReportApp.Core.Data;
using ReportApp.AnalysisEngine.Services;
using ReportApp.WebAPI.Interfaces;
using ReportApp.WebAPI.Providers; // <--- 1. SE TILL ATT DENNA FINNS

var builder = WebApplication.CreateBuilder(args);

// 1. Add Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Databas & Logik
builder.Services.AddDbContext<ReportDbContext>();
builder.Services.AddScoped<AnalysisService>();

// --- 2. AKTIVERA PROVIDERS HÄR ---
builder.Services.AddScoped<IReportProvider, QuestOpenSourceProvider>();
// builder.Services.AddScoped<IReportProvider, IronSuiteProvider>();
// builder.Services.AddScoped<IReportProvider, JsReportProvider>();

// --- 3. FIXA CORS PORTEN (Ändrad till 61704) ---
builder.Services.AddCors(options => {
    options.AddPolicy("VuePolicy", policy => {
        policy.WithOrigins("http://localhost:61704") // <--- VIKTIGT: Matcha din Vite-port!
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("X-Generation-Time-Ms");
    });
});

var app = builder.Build();

// 4. Seed Database
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

// --- 5. ORDNINGEN ÄR VIKTIG ---
app.UseCors("VuePolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();