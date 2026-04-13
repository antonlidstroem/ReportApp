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

    public async Task<ReportDataViewModel> GetSurveyAnalysisAsync(
    int surveyId,
    DateTime? start = null,
    DateTime? end = null,
    List<int>? questionIds = null)
    {
        var surveyInfo = await _context.Surveys
            .Where(s => s.Id == surveyId)
            .Select(s => new { s.Title, CompanyName = s.Company!.Name })
            .FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Survey not found");

        var model = new ReportDataViewModel
        {
            CompanyName = surveyInfo.CompanyName,
            SurveyTitle = surveyInfo.Title,
            GeneratedAt = DateTime.Now,
            StartDate = start,
            EndDate = end
        };

        // Aggregera frågedata direkt i databasen
        var questionsQuery = _context.Questions
            .Where(q => q.SurveyId == surveyId);

        if (questionIds != null && questionIds.Any())
        {
            questionsQuery = questionsQuery.Where(q => questionIds.Contains(q.Id));
        }

        model.QuestionSummaries = await questionsQuery
            .Select(q => new QuestionSummary
            {
                QuestionId = q.Id,
                Text = q.Text,
                Category = q.Category,
                TotalResponses = q.Responses.Count(r =>
                    (!start.HasValue || r.SubmittedAt >= start.Value) &&
                    (!end.HasValue || r.SubmittedAt <= end.Value)),
                AverageValue = q.Responses
                    .Where(r => (!start.HasValue || r.SubmittedAt >= start.Value) &&
                                (!end.HasValue || r.SubmittedAt <= end.Value))
                    .Average(r => (double?)r.Value) ?? 0,
                // Distribution beräknas bäst i minnet efteråt pga SQLite begränsningar i komplexa grupper
            })
            .ToListAsync();

        // Fix: Runda av värden efter hämtning
        model.QuestionSummaries.ForEach(q => q.AverageValue = Math.Round(q.AverageValue, 2));

        // Trend-logik (redan effektiv, men lagt till null-check)
        model.Trends = await _context.Responses
            .Where(r => r.Question.SurveyId == surveyId &&
                        (!start.HasValue || r.SubmittedAt >= start.Value) &&
                        (!end.HasValue || r.SubmittedAt <= end.Value) &&
                        (questionIds == null || !questionIds.Any() || questionIds.Contains(r.QuestionId)))
            .GroupBy(r => new { r.SubmittedAt.Year, r.SubmittedAt.Month })
            .Select(g => new MonthlyTrend
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthName = "", // Sätts i minnet nedan
                AverageValue = Math.Round(g.Average(r => r.Value), 2)
            })
            .OrderBy(t => t.Year).ThenBy(t => t.Month)
            .ToListAsync();

        model.Trends.ForEach(t => t.MonthName = GetMonthName(t.Month));

        return model;
    }

    public async Task<object> GetAllSurveysAsync()
    {
        return await _context.Surveys
            .Select(s => new { s.Id, s.Title, QuestionCount = s.Questions.Count })
            .ToListAsync();
    }

    public async Task<object> GetSurveyQuestionsAsync(int surveyId)
    {
        return await _context.Questions
            .Where(q => q.SurveyId == surveyId)
            .Select(q => new
            {
                q.Id,
                q.Text,
                q.Category,
                Type = q.Type.ToString(),
                ResponseCount = q.Responses.Count
            })
            .ToListAsync();
    }

    private static string GetMonthName(int month) =>
        new DateTime(2000, month, 1).ToString("MMM");
}
