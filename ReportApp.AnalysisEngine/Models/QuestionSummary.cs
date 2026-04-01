using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportApp.AnalysisEngine.Models
{
    public class QuestionSummary
    {
        public int QuestionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double AverageValue { get; set; }
        public int TotalResponses { get; set; }
        public Dictionary<string, int>? Distribution { get; set; } // t.ex. "Antal 5:or", "Antal 4:or"
    }
}
