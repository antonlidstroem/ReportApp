using Microsoft.EntityFrameworkCore;
using ReportApp.Core.Models;

namespace ReportApp.Core.Data;

public class ReportDbContext : DbContext
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Survey> Surveys => Set<Survey>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Response> Responses => Set<Response>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Vi lägger databasen i projektmappen för enkelhetens skull i denna PoC
        options.UseSqlite("Data Source=reports_poc.db");
    }
}