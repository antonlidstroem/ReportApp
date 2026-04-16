namespace ReportApp.Core.Models;

public class Response
{
    public int Id { get; set; }
    public double Value { get; set; }
    public string? Comment { get; set; }
    public DateTime SubmittedAt { get; set; }
    public int QuestionId { get; set; }
    public Question? Question { get; set; }
}