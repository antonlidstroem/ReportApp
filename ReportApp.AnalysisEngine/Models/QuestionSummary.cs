namespace ReportApp.AnalysisEngine.Models;

public class QuestionSummary
{
    public int QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public double AverageValue { get; set; }
    public int TotalResponses { get; set; }
    public Dictionary<string, int>? Distribution { get; set; }

    /// <summary>
    /// AverageValue as a percentage of 5 (max scale).
    /// e.g. 4.2 → 84. Used for HTML progress bars in jsreport templates.
    /// </summary>
    public int AverageValue20Pct => (int)Math.Round(AverageValue * 20.0);
}