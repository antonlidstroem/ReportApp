namespace ReportApp.AnalysisEngine.Models;

public class MonthlyTrend
{
    public string MonthName { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public double AverageValue { get; set; }
}