namespace ReportApp.Core.Models;

public class Response
{
    public int Id { get; set; }
    public double Value { get; set; } // 1-5 för skala, 0/1 för YesNo
    public string? Comment { get; set; }
    public DateTime SubmittedAt { get; set; }
    public int QuestionId { get; set; }
    public Question? Question { get; set; }
}