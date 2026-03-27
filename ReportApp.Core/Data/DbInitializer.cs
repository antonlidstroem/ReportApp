using ReportApp.Core.Models;

namespace ReportApp.Core.Data;

public static class DbInitializer
{
    public static void Initialize(ReportDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        if (context.Companies.Any()) return;

        var company = new Company { Name = "Storkommunen AB", OrganizationNumber = "556677-1234" };
        context.Companies.Add(company);

        var survey = new Survey
        {
            Title = "Årlig Arbetsmiljökartläggning 2026",
            CreatedAt = DateTime.Now.AddMonths(-12),
            Company = company
        };
        context.Surveys.Add(survey);

        var questions = new List<Question>
        {
            new() { Text = "Hur upplever du din arbetsbelastning?", Category = "Psykosocialt", Type = QuestionType.Scale, Survey = survey },
            new() { Text = "Har du tillgång till de verktyg du behöver?", Category = "Fysisk miljö", Type = QuestionType.Scale, Survey = survey },
            new() { Text = "Upplever du stöd från din närmaste chef?", Category = "Ledarskap", Type = QuestionType.Scale, Survey = survey },
            new() { Text = "Har du varit involverad i en tillbud under året?", Category = "Säkerhet", Type = QuestionType.YesNo, Survey = survey }
        };
        context.Questions.AddRange(questions);

        // Generera 500+ svar spritt över 12 månader för att simulera trender
        var rnd = new Random();
        for (int i = 0; i < 12; i++)
        {
            var date = DateTime.Now.AddMonths(-i);
            foreach (var q in questions)
            {
                // Skapa 5-10 svar per månad per fråga
                for (int j = 0; j < rnd.Next(5, 10); j++)
                {
                    double value;
                    if (q.Type == QuestionType.Scale)
                        // Simulera lite bättre mående på våren (månad 3-5)
                        value = (date.Month >= 3 && date.Month <= 5) ? rnd.Next(4, 6) : rnd.Next(1, 6);
                    else
                        value = rnd.Next(0, 2);

                    context.Responses.Add(new Response
                    {
                        Question = q,
                        Value = value,
                        SubmittedAt = date.AddDays(rnd.Next(-15, 15))
                    });
                }
            }
        }
        context.SaveChanges();
    }
}