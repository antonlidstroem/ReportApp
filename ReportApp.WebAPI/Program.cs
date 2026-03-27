using ReportApp.Core.Data;
using ReportApp.AnalysisEngine.Services;
using ReportApp.WebAPI.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Databas & Logik
builder.Services.AddDbContext<ReportDbContext>();
builder.Services.AddScoped<AnalysisService>();

// Registrera Providers (Här lägger vi till våra riktiga implementationer sen)
// builder.Services.AddScoped<IReportProvider, QuestOpenSourceProvider>();
// builder.Services.AddScoped<IReportProvider, IronSuiteProvider>();
// builder.Services.AddScoped<IReportProvider, JsReportProvider>();

// 2. CORS för Vue-frontend
builder.Services.AddCors(options => {
    options.AddPolicy("VuePolicy", policy => {
        policy.WithOrigins("http://localhost:5173") // Standard för Vite/Vue
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("X-Generation-Time-Ms"); // Viktigt för prestandamätning!
    });
});

var app = builder.Build();

// 3. Seed Database
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