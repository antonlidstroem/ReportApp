namespace ReportApp.Core.Models;

public class Survey
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int CompanyId { get; set; }
    public Company? Company { get; set; }
    public List<Question> Questions { get; set; } = new();
}
