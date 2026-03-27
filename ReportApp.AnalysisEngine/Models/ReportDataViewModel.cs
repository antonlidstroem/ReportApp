using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportApp.AnalysisEngine.Models
{
    public class ReportDataViewModel
    {
        public string CompanyName { get; set; } = string.Empty;
        public string SurveyTitle { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        // Sammanfattning per fråga
        public List<QuestionSummary> QuestionSummaries { get; set; } = new();

        // Data för trendlinjer (t.ex. medelvärde per månad)
        public List<MonthlyTrend> Trends { get; set; } = new();
    }
}
