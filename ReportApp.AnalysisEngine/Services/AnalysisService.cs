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
            .Select(s => new { s.Title, CompanyName = s.Company != null ? s.Company.Name : string.Empty })
            .FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException($"Survey with id={surveyId} not found.");

        var model = new ReportDataViewModel
        {
            CompanyName = surveyInfo.CompanyName,
            SurveyTitle = surveyInfo.Title,
            GeneratedAt = DateTime.Now,
            StartDate = start,
            EndDate = end
        };

        var questionsQuery = _context.Questions
            .Where(q => q.SurveyId == surveyId);

        if (questionIds != null && questionIds.Count > 0)
            questionsQuery = questionsQuery.Where(q => questionIds.Contains(q.Id));

        // Aggregate in DB; round in memory (Math.Round not translatable by SQLite EF provider)
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
                    .Average(r => (double?)r.Value) ?? 0.0,
            })
            .ToListAsync();

        // Round after fetch — SQLite EF cannot translate Math.Round inside a projection
        model.QuestionSummaries.ForEach(q => q.AverageValue = Math.Round(q.AverageValue, 2));

        // Fetch trend rows as primitives then group in memory to avoid SQLite DateTime.Year/Month issues
        var trendRaw = await _context.Responses
            .Where(r => r.Question.SurveyId == surveyId &&
                        (!start.HasValue || r.SubmittedAt >= start.Value) &&
                        (!end.HasValue || r.SubmittedAt <= end.Value) &&
                        (questionIds == null || questionIds.Count == 0 || questionIds.Contains(r.QuestionId)))
            .Select(r => new { r.SubmittedAt.Year, r.SubmittedAt.Month, r.Value })
            .ToListAsync();

        model.Trends = trendRaw
            .GroupBy(r => new { r.Year, r.Month })
            .Select(g => new MonthlyTrend
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthName = GetMonthName(g.Key.Month),
                AverageValue = Math.Round(g.Average(r => r.Value), 2)
            })
            .OrderBy(t => t.Year).ThenBy(t => t.Month)
            .ToList();

        return model;
    }

    /// <summary>
    /// Returns surveys with id, title, companyName and questionCount.
    /// Uses explicit lowercase property names to guarantee camelCase JSON output
    /// regardless of the JsonNamingPolicy configuration.
    /// </summary>
    public async Task<object> GetAllSurveysAsync()
    {
        return await _context.Surveys
            .Select(s => new
            {
                id = s.Id,
                title = s.Title,
                companyName = s.Company != null ? s.Company.Name : string.Empty,
                questionCount = s.Questions.Count
            })
            .ToListAsync();
    }

    public async Task<object> GetSurveyQuestionsAsync(int surveyId)
    {
        return await _context.Questions
            .Where(q => q.SurveyId == surveyId)
            .Select(q => new
            {
                id = q.Id,
                text = q.Text,
                category = q.Category,
                type = q.Type.ToString(),
                responseCount = q.Responses.Count
            })
            .ToListAsync();
    }

    private static string GetMonthName(int month) =>
        new DateTime(2000, month, 1).ToString("MMM");
}