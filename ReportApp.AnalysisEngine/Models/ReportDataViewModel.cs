using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportApp.AnalysisEngine.Models
{
    public class ReportDataViewModel
    {
        public string CompanyName { get; set; } = string.Empty;
        public string SurveyTitle { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Summary per question
        public List<QuestionSummary> QuestionSummaries { get; set; } = new();

        // Data for trend lines (e.g. average per month)
        public List<MonthlyTrend> Trends { get; set; } = new();

        // ── Computed properties for jsreport Handlebars templates ──────────────

        public int QuestionCount => QuestionSummaries.Count;

        public int TotalResponses => QuestionSummaries.Sum(q => q.TotalResponses);

        public string OverallAverage =>
            QuestionSummaries.Count > 0
                ? Math.Round(QuestionSummaries.Average(q => q.AverageValue), 2).ToString("F2")
                : "0.00";

        public int CategoryCount =>
            QuestionSummaries.Select(q => q.Category).Distinct().Count();

        /// <summary>
        /// Pre-built HTML table for Playwright templates (no Handlebars loops).
        /// </summary>
        public string QuestionsTableHtml
        {
            get
            {
                if (!QuestionSummaries.Any()) return "<p>Inga frågor valda.</p>";

                var sb = new StringBuilder();
                sb.Append("<table style=\"width:100%;border-collapse:collapse;font-size:12px;margin-bottom:20px\">");
                sb.Append("<thead><tr style=\"background:#0f2d4a;color:white\">");
                sb.Append("<th style=\"padding:8px 12px;text-align:left\">Fråga</th>");
                sb.Append("<th style=\"padding:8px 12px;text-align:left\">Kategori</th>");
                sb.Append("<th style=\"padding:8px 12px;text-align:left\">Genomsnitt</th>");
                sb.Append("<th style=\"padding:8px 12px;text-align:left\">Svar</th>");
                sb.Append("</tr></thead><tbody>");

                int i = 0;
                foreach (var q in QuestionSummaries)
                {
                    var color = q.AverageValue >= 4 ? "#16a34a" : q.AverageValue >= 3 ? "#d97706" : "#dc2626";
                    var bg = i % 2 == 0 ? "" : "background:#f8fafc";
                    sb.Append($"<tr style=\"{bg};border-bottom:1px solid #e2e8f0\">");
                    // System.Web.HttpUtility is not available in .NET 8 without System.Web.
                    // System.Net.WebUtility.HtmlEncode is the correct .NET Core / .NET 8 API.
                    sb.Append($"<td style=\"padding:7px 12px\">{System.Net.WebUtility.HtmlEncode(q.Text)}</td>");
                    sb.Append($"<td style=\"padding:7px 12px\">{System.Net.WebUtility.HtmlEncode(q.Category)}</td>");
                    sb.Append($"<td style=\"padding:7px 12px;font-weight:700;color:{color}\">{q.AverageValue:F2}</td>");
                    sb.Append($"<td style=\"padding:7px 12px\">{q.TotalResponses}</td>");
                    sb.Append("</tr>");
                    i++;
                }

                sb.Append("</tbody></table>");
                return sb.ToString();
            }
        }
    }
}
