namespace ReportApp.Core.Models;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OrganizationNumber { get; set; } = string.Empty;
    public List<Survey> Surveys { get; set; } = new();
}
