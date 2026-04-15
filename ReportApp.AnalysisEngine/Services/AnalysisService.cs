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
            .FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException($"Survey with id={surveyId} not found");

        var model = new ReportDataViewModel
        {
            CompanyName = surveyInfo.CompanyName,
            SurveyTitle = surveyInfo.Title,
            GeneratedAt = DateTime.Now,
            StartDate = start,
            EndDate = end
        };

        // Build the questions query
        var questionsQuery = _context.Questions
            .Where(q => q.SurveyId == surveyId);

        if (questionIds != null && questionIds.Any())
        {
            questionsQuery = questionsQuery.Where(q => questionIds.Contains(q.Id));
        }

        // Aggregate question data in the database
        model.QuestionSummaries = await questionsQuery
            .Select(q => new QuestionSummary
            {
                QuestionId = q.Id,
                Text = q.Text,
                Category = q.Category,
                TotalResponses = q.Responses.Count(r =>
                    (!start.HasValue || r.SubmittedAt >= start.Value) &&
                    (!end.HasValue || r.SubmittedAt <= end.Value)),
                // Use nullable average to avoid exception on empty set
                AverageValue = q.Responses
                    .Where(r => (!start.HasValue || r.SubmittedAt >= start.Value) &&
                                (!end.HasValue || r.SubmittedAt <= end.Value))
                    .Average(r => (double?)r.Value) ?? 0,
            })
            .ToListAsync();

        // Round after fetching (Math.Round not supported in SQLite EF translation)
        model.QuestionSummaries.ForEach(q =>
            q.AverageValue = Math.Round(q.AverageValue, 2));

        // Trend: fetch raw then group in memory — avoids SQLite LINQ translation issues
        // with DateTime.Year/Month extraction + Math.Round in the same query
        var trendResponses = await _context.Responses
            .Where(r => r.Question.SurveyId == surveyId &&
                        (!start.HasValue || r.SubmittedAt >= start.Value) &&
                        (!end.HasValue || r.SubmittedAt <= end.Value) &&
                        (questionIds == null || !questionIds.Any() || questionIds.Contains(r.QuestionId)))
            .Select(r => new { r.SubmittedAt.Year, r.SubmittedAt.Month, r.Value })
            .ToListAsync();

        model.Trends = trendResponses
            .GroupBy(r => new { r.Year, r.Month })
            .Select(g => new MonthlyTrend
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthName = GetMonthName(g.Key.Month),
                AverageValue = Math.Round(g.Average(r => r.Value), 2)
            })
            .OrderBy(t => t.Year)
            .ThenBy(t => t.Month)
            .ToList();

        return model;
    }

    /// <summary>
    /// Returns all surveys with id, title, companyName and questionCount.
    /// The companyName field is required by the Vue frontend Survey interface.
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
