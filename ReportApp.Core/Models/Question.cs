namespace ReportApp.Core.Models;

public enum QuestionType { Scale, YesNo }

public class Question
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Category { get; set; } = "Allmänt";
    public QuestionType Type { get; set; }
    public int SurveyId { get; set; }
    public Survey? Survey { get; set; }
    public List<Response> Responses { get; set; } = new();
}
