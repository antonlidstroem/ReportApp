using ReportApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ReportApp.Core.Data;

public static class DbInitializer
{
    public static void Initialize(ReportDbContext context)
    {
        // Rensa och skapa om DB för att få en fräsch start
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var company = new Company { Name = "Storkommunen AB", OrganizationNumber = "556677-1234" };
        context.Companies.Add(company);

        // --- ENKÄT 1: Årlig Arbetsmiljökartläggning (Bred) ---
        var survey1 = new Survey
        {
            Title = "Årlig Arbetsmiljökartläggning 2026",
            CreatedAt = DateTime.Now.AddMonths(-12),
            Company = company
        };

        // --- ENKÄT 2: Pulsmätning: Stress & Belastning (Fokuserad) ---
        var survey2 = new Survey
        {
            Title = "Månadskoll: Stress & Belastning",
            CreatedAt = DateTime.Now.AddMonths(-3),
            Company = company
        };

        // --- ENKÄT 3: Skyddsrond: Verkstad Nord (Teknisk/Säkerhet) ---
        var survey3 = new Survey
        {
            Title = "Skyddsrond: Verkstad Nord",
            CreatedAt = DateTime.Now.AddMonths(-1),
            Company = company
        };

        context.Surveys.AddRange(survey1, survey2, survey3);

        // Definiera frågor för de olika enkäterna
        var questions = new List<Question>
        {
            // Frågor till Survey 1
            new() { Text = "Hur upplever du din arbetsbelastning?", Category = "Psykosocialt", Type = QuestionType.Scale, Survey = survey1 },
            new() { Text = "Upplever du stöd från din närmaste chef?", Category = "Ledarskap", Type = QuestionType.Scale, Survey = survey1 },
            new() { Text = "Är den fysiska arbetsmiljön tillfredsställande?", Category = "Fysisk miljö", Type = QuestionType.Scale, Survey = survey1 },
            
            // Frågor till Survey 2
            new() { Text = "Känner du dig utvilad när du börjar arbetsdagen?", Category = "Hälsa", Type = QuestionType.Scale, Survey = survey2 },
            new() { Text = "Har du haft tillräckligt med tid för återhämtning?", Category = "Hälsa", Type = QuestionType.Scale, Survey = survey2 },
            
            // Frågor till Survey 3
            new() { Text = "Fungerar nödutgångar och brandsläckare utan anmärkning?", Category = "Säkerhet", Type = QuestionType.YesNo, Survey = survey3 },
            new() { Text = "Används föreskriven skyddsutrustning?", Category = "Säkerhet", Type = QuestionType.YesNo, Survey = survey3 }
        };

        context.Questions.AddRange(questions);

        // Generera slumpmässig data (Svar) för alla frågor
        var rnd = new Random();
        foreach (var q in questions)
        {
            // Generera 20-40 svar per fråga
            int responseCount = rnd.Next(20, 41);
            for (int i = 0; i < responseCount; i++)
            {
                double val;
                if (q.Type == QuestionType.Scale)
                {
                    // Skapa lite variation i "måendet"
                    val = rnd.Next(1, 6);
                    if (q.Category == "Hälsa") val = rnd.Next(2, 5); // Mer stabilt i mitten
                }
                else
                {
                    val = rnd.Next(0, 2); // 0 eller 1
                }

                context.Responses.Add(new Response
                {
                    Question = q,
                    Value = val,
                    // Sprid ut svaren över tid baserat på när enkäten skapades
                    SubmittedAt = q.Survey!.CreatedAt.AddDays(rnd.Next(0, 30))
                });
            }
        }

        context.SaveChanges();
    }
}