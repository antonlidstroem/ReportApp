using ReportApp.Core.Models;

namespace ReportApp.Core.Data;

public static class DbInitializer
{
    public static void Initialize(ReportDbContext context)
    {
        // Under utveckling kan du lägga till denna rad för att rensa gamla data:
        context.Database.EnsureDeleted();

        context.Database.EnsureCreated();

        if (context.Surveys.Any()) return;

        var company = new Company { Name = "Storkommunen AB", OrganizationNumber = "556677-1234" };
        context.Companies.Add(company);

        var survey1 = new Survey
        {
            Title = "Årlig Arbetsmiljökartläggning 2026",
            CreatedAt = DateTime.Now.AddMonths(-12),
            Company = company
        };
        var survey2 = new Survey
        {
            Title = "Månadskoll: Stress & Belastning",
            CreatedAt = DateTime.Now.AddMonths(-3),
            Company = company
        };
        var survey3 = new Survey
        {
            Title = "Skyddsrond: Verkstad Nord",
            CreatedAt = DateTime.Now.AddMonths(-1),
            Company = company
        };
        var survey4 = new Survey
        {
            Title = "⚡ Stresstest: Fullständig Organisationsanalys 2026",
            CreatedAt = DateTime.Now.AddMonths(-6),
            Company = company
        };

        context.Surveys.AddRange(survey1, survey2, survey3, survey4);

        var rnd = new Random(42);

        var questions = new List<Question>
        {
            new() { Text = "Hur upplever du din arbetsbelastning?",         Category = "Psykosocialt", Type = QuestionType.Scale, Survey = survey1 },
            new() { Text = "Upplever du stöd från din närmaste chef?",       Category = "Ledarskap",    Type = QuestionType.Scale, Survey = survey1 },
            new() { Text = "Är den fysiska arbetsmiljön tillfredsställande?",Category = "Fysisk miljö", Type = QuestionType.Scale, Survey = survey1 },
            new() { Text = "Har du tillgång till rätt verktyg för ditt arbete?", Category = "Resurser", Type = QuestionType.Scale, Survey = survey1 },
            new() { Text = "Känner du dig inkluderad i teamets beslut?",     Category = "Psykosocialt", Type = QuestionType.Scale, Survey = survey1 },
            new() { Text = "Känner du dig utvilad när du börjar arbetsdagen?", Category = "Hälsa",      Type = QuestionType.Scale, Survey = survey2 },
            new() { Text = "Har du haft tillräckligt med tid för återhämtning?", Category = "Hälsa",   Type = QuestionType.Scale, Survey = survey2 },
            new() { Text = "Upplever du stress relaterad till deadlines?",   Category = "Psykosocialt", Type = QuestionType.Scale, Survey = survey2 },
            new() { Text = "Fungerar nödutgångar och brandsläckare utan anmärkning?", Category = "Säkerhet", Type = QuestionType.YesNo, Survey = survey3 },
            new() { Text = "Används föreskriven skyddsutrustning?",          Category = "Säkerhet",    Type = QuestionType.YesNo, Survey = survey3 },
            new() { Text = "Är alla maskiner försedda med korrekt skydd?",   Category = "Säkerhet",    Type = QuestionType.YesNo, Survey = survey3 },
        };

        context.Questions.AddRange(questions);

        foreach (var q in questions)
        {
            int responseCount = rnd.Next(20, 41);
            for (int i = 0; i < responseCount; i++)
            {
                double val = q.Type == QuestionType.Scale
                    ? (q.Category == "Hälsa" ? rnd.Next(2, 5) : rnd.Next(1, 6))
                    : rnd.Next(0, 2);

                context.Responses.Add(new Response
                {
                    Question = q,
                    Value = val,
                    SubmittedAt = q.Survey!.CreatedAt.AddDays(rnd.Next(0, 30))
                });
            }
        }

        var categories = new[] { "Ledarskap", "Psykosocialt", "Fysisk miljö", "Hälsa", "Säkerhet", "Kompetens", "Kommunikation", "Värderingar", "Innovation", "Mångfald" };
        var questionTexts = new[]
        {
            "Hur värderar du kvaliteten på {0} i din dagliga verksamhet?",
            "I vilken utsträckning uppfylls dina förväntningar gällande {0}?",
            "Hur nöjd är du med nuvarande {0}-processer?",
            "Anser du att {0} fungerar tillfredsställande i organisationen?",
            "Hur bedömer du organisationens förmåga att hantera {0}?"
        };

        var heavyQuestions = new List<Question>();
        for (int i = 1; i <= 500; i++)
        {
            var cat = categories[i % categories.Length];
            var textTemplate = questionTexts[i % questionTexts.Length];
            heavyQuestions.Add(new Question
            {
                Text = string.Format(textTemplate, $"{cat.ToLower()} (fråga {i:000})"),
                Category = cat,
                Type = i % 7 == 0 ? QuestionType.YesNo : QuestionType.Scale,
                Survey = survey4
            });
        }
        context.Questions.AddRange(heavyQuestions);

        var responses = new List<Response>();
        foreach (var q in heavyQuestions)
        {
            for (int i = 0; i < 200; i++)
            {
                responses.Add(new Response
                {
                    Question = q,
                    Value = q.Type == QuestionType.Scale ? rnd.Next(1, 6) : rnd.Next(0, 2),
                    SubmittedAt = survey4.CreatedAt.AddDays(rnd.Next(0, 180))
                });
            }
        }
        context.Responses.AddRange(responses);

        context.SaveChanges();
    }
}