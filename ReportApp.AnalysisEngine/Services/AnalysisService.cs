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
    public async Task<ReportDataViewModel> GetSurveyAnalysisAsync(int surveyId, DateTime? start = null, DateTime? end = null)
    {
        // Hämta enkäten men filtrera svaren direkt i "Include"
        var query = _context.Surveys
            .Include(s => s.Company)
            .Include(s => s.Questions)
            .ThenInclude(q => q.Responses.Where(r =>
                (!start.HasValue || r.SubmittedAt >= start.Value) &&
                (!end.HasValue || r.SubmittedAt <= end.Value)))
            .AsQueryable();

        var survey = await query.FirstOrDefaultAsync(s => s.Id == surveyId);

        if (survey == null) throw new Exception("Survey not found");

        var model = new ReportDataViewModel
        {
            CompanyName = survey.Company?.Name ?? "Okänt företag",
            SurveyTitle = survey.Title,
            GeneratedAt = DateTime.Now,
            // Vi sparar filter-datumen i modellen så att de kan skrivas ut i rapporten sen
            StartDate = start,
            EndDate = end
        };

        foreach (var q in survey.Questions)
        {
            // Vi räknar bara på de svar som passerat filtret
            if (!q.Responses.Any()) continue;

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

        // Uppdatera även trend-beräkningen så den följer filtret
        model.Trends = await _context.Responses
            .Where(r => r.Question.SurveyId == surveyId &&
                       (!start.HasValue || r.SubmittedAt >= start.Value) &&
                       (!end.HasValue || r.SubmittedAt <= end.Value))
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

    public async Task<object> GetAllSurveysAsync()
    {
        return await _context.Surveys
            .Select(s => new { s.Id, s.Title })
            .ToListAsync();
    }

    private static string GetMonthName(int month) =>
        new DateTime(2000, month, 1).ToString("MMM");
}