using Microsoft.EntityFrameworkCore;
using ReportApp.Core.Data;
using ReportApp.AnalysisEngine.Models;

namespace ReportApp.AnalysisEngine.Services;

public class AnalysisService
{
    private readonly ReportDbContext _context;

    public AnalysisService(ReportDbContext context)
    {
        _context = context;
    }

    public async Task<ReportDataViewModel> GetSurveyAnalysisAsync(int surveyId)
    {
        var survey = await _context.Surveys
            .Include(s => s.Company)
            .Include(s => s.Questions)
            .ThenInclude(q => q.Responses)
            .FirstOrDefaultAsync(s => s.Id == surveyId);

        if (survey == null) throw new Exception("Survey not found");

        var model = new ReportDataViewModel
        {
            CompanyName = survey.Company?.Name ?? "Okänt företag",
            SurveyTitle = survey.Title,
            GeneratedAt = DateTime.Now
        };

        foreach (var q in survey.Questions)
        {
            if (!q.Responses.Any()) continue;

            // Beräkna medelvärde
            var avg = q.Responses.Average(r => r.Value);

            model.QuestionSummaries.Add(new QuestionSummary
            {
                QuestionId = q.Id,
                Text = q.Text,
                Category = q.Category,
                AverageValue = Math.Round(avg, 2),
                TotalResponses = q.Responses.Count
            });
        }

        // Beräkna trender (aggregera alla svar per månad för hela enkäten)
        model.Trends = await _context.Responses
            .Where(r => r.Question.SurveyId == surveyId)
            .GroupBy(r => new { r.SubmittedAt.Year, r.SubmittedAt.Month })
            .Select(g => new MonthlyTrend
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthName = GetMonthName(g.Key.Month),
                AverageValue = Math.Round(g.Average(r => r.Value), 2)
            })
            .OrderBy(t => t.Year).ThenBy(t => t.Month)
            .ToListAsync();

        return model;
    }

    private static string GetMonthName(int month) =>
        new DateTime(2000, month, 1).ToString("MMM");
}