using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportApp.AnalysisEngine.Models
{
    public class MonthlyTrend
    {
        public string MonthName { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Month { get; set; }
        public double AverageValue { get; set; }
    }
}
