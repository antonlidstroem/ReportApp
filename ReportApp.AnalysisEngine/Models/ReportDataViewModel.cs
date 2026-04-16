using System.Net;
using System.Text;

namespace ReportApp.AnalysisEngine.Models;

public class ReportDataViewModel
{
    public string CompanyName { get; set; } = string.Empty;
    public string SurveyTitle { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public List<QuestionSummary> QuestionSummaries { get; set; } = new();
    public List<MonthlyTrend> Trends { get; set; } = new();

    // ── Computed properties exposed to jsreport Handlebars templates ────────

    /// <summary>Total number of questions in this report.</summary>
    public int QuestionCount => QuestionSummaries.Count;

    /// <summary>Sum of all responses across all questions.</summary>
    public int TotalResponses => QuestionSummaries.Sum(q => q.TotalResponses);

    /// <summary>Overall average score, formatted to 2 decimal places.</summary>
    public string OverallAverage =>
        QuestionSummaries.Count > 0
            ? Math.Round(QuestionSummaries.Average(q => q.AverageValue), 2).ToString("F2")
            : "0.00";

    /// <summary>Number of distinct categories across all questions.</summary>
    public int CategoryCount =>
        QuestionSummaries.Select(q => q.Category).Distinct().Count();

    /// <summary>
    /// Pre-built HTML table for Playwright templates that only support
    /// token replacement (no Handlebars #each loops).
    /// Uses System.Net.WebUtility.HtmlEncode — no extra NuGet package needed.
    /// </summary>
    public string QuestionsTableHtml
    {
        get
        {
            if (!QuestionSummaries.Any())
                return "<p style=\"color:#64748b\">Inga frågor valda.</p>";

            var sb = new StringBuilder();
            sb.Append("<table style=\"width:100%;border-collapse:collapse;font-size:12px;margin-bottom:20px\">");
            sb.Append("<thead>");
            sb.Append("<tr style=\"background:#0f2d4a;color:white\">");
            sb.Append("<th style=\"padding:8px 12px;text-align:left\">Fråga</th>");
            sb.Append("<th style=\"padding:8px 12px;text-align:left\">Kategori</th>");
            sb.Append("<th style=\"padding:8px 12px;text-align:left\">Genomsnitt</th>");
            sb.Append("<th style=\"padding:8px 12px;text-align:left\">Svar</th>");
            sb.Append("</tr></thead><tbody>");

            for (int i = 0; i < QuestionSummaries.Count; i++)
            {
                var q = QuestionSummaries[i];
                var color = q.AverageValue >= 4 ? "#16a34a"
                          : q.AverageValue >= 3 ? "#d97706"
                          : "#dc2626";
                var bg = i % 2 == 0 ? "background:#ffffff" : "background:#f8fafc";
                sb.Append($"<tr style=\"{bg};border-bottom:1px solid #e2e8f0\">");
                sb.Append($"<td style=\"padding:7px 12px\">{WebUtility.HtmlEncode(q.Text)}</td>");
                sb.Append($"<td style=\"padding:7px 12px\">{WebUtility.HtmlEncode(q.Category)}</td>");
                sb.Append($"<td style=\"padding:7px 12px;font-weight:700;color:{color}\">{q.AverageValue:F2}</td>");
                sb.Append($"<td style=\"padding:7px 12px\">{q.TotalResponses}</td>");
                sb.Append("</tr>");
            }

            sb.Append("</tbody></table>");
            return sb.ToString();
        }
    }
}