using FiaeTrainer.Core;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Kennzeichnet SQL-Tests, die ausdrücklich über die Umgebungsvariable eingeschaltet werden. </summary>
public sealed class SqlFactAttribute : FactAttribute
{
    /// <summary> Vermeidet eine unbeabsichtigte Datenbankinitialisierung bei reinen Unit-Tests. </summary>
    public SqlFactAttribute()
    {
        if (Environment.GetEnvironmentVariable("FIAE_SQL_TESTS") != "1")
            Skip = "SQL-Integrationstest: FIAE_SQL_TESTS=1 setzen.";
    }
}

/// <summary> Prüft Lerninhalte und Sicherheitsgrenzen gegen einen echten Microsoft SQL Server. </summary>
public sealed class SqlIntegrationTests : IAsyncLifetime
{
    /// <summary> Verwendet ausschließlich die eigene Trainingsdatenbank. </summary>
    private readonly TrainingDatabase _database = new(Environment.GetEnvironmentVariable("FIAE_SQL_CONNECTION") ?? "Server=.\\SQLEXPRESS;Database=FIAE_AP2_Training;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Connect Timeout=5");
    /// <summary> Syntaxprüfung und Umleitung auf Prüfdaten. </summary>
    private readonly SqlQueryValidator _validator = new();

    /// <summary> Macht jeden eingeschalteten SQL-Test unabhängig von Testreihenfolge und einem vorherigen App-Start. </summary>
    public async Task InitializeAsync()
    {
        if (Environment.GetEnvironmentVariable("FIAE_SQL_TESTS") == "1")
            await _database.InitializeAsync();
    }

    /// <summary> Datenbankverbindungen werden bereits pro Aufruf geschlossen. </summary>
    public Task DisposeAsync() => Task.CompletedTask;

    [SqlFact]
    public async Task AllThirtyReferenceSolutionsHaveIndependentlyExpectedDimensionsAndPassBothDatasets()
    {
        await _database.InitializeAsync();
        // Erwartete Zeilenzahlen wurden aus den synthetischen Falldaten fachlich nachgezählt.
        int[] expectedCounts = [12, 2, 1, 2, 12, 3, 6, 2, 7, 2, 4, 8, 18, 7, 1, 12, 1, 1, 1, 1, 4, 3, 1, 9, 1, 4, 1, 1, 5, 1];
        int[] expectedColumns = [2, 2, 3, 3, 2, 2, 2, 3, 2, 2, 1, 3, 4, 4, 3, 3, 2, 5, 1, 4, 2, 2, 2, 4, 4, 3, 2, 3, 3, 3];
        LessonCatalog catalog = new();
        QueryEvaluator evaluator = new(_database, _validator);
        foreach (Lesson lesson in catalog.Lessons)
        {
            Evaluation evaluation = await evaluator.EvaluateAsync(lesson, lesson.ReferenceSql);
            Assert.True(evaluation.Correct, $"Mission {lesson.Id}: {evaluation.Message}");
            Assert.Equal(expectedCounts[lesson.Id - 1], evaluation.Result.Rows.Count);
            Assert.Equal(expectedColumns[lesson.Id - 1], evaluation.Result.Columns.Count);
        }
    }

    [SqlFact]
    public async Task DecisiveEvidenceAndAggregateValuesAreCorrect()
    {
        LessonCatalog catalog = new();
        QueryResult final = await _database.ExecuteAsync(_validator.ForCaseData(catalog.Lessons[29].ReferenceSql));
        Assert.Null(final.Error);
        Assert.Single(final.Rows);
        Assert.Equal(new object?[] { 4, "Nora", "Seidel" }, final.Rows[0]);

        QueryResult count = await _database.ExecuteAsync(_validator.ForCaseData(catalog.Lessons[18].ReferenceSql));
        Assert.Equal(12, count.Rows[0][0]);
        QueryResult aggregate = await _database.ExecuteAsync(_validator.ForCaseData(catalog.Lessons[19].ReferenceSql));
        Assert.Equal(162000m, aggregate.Rows[0][0]);
        Assert.Equal(13500m, aggregate.Rows[0][1]);
        Assert.Equal(500m, aggregate.Rows[0][2]);
        Assert.Equal(48000m, aggregate.Rows[0][3]);

        QueryResult accessCounts = await _database.ExecuteAsync(_validator.ForCaseData(catalog.Lessons[23].ReferenceSql));
        Assert.Equal(0, accessCounts.Rows.Single(row => Equals(row[0], 8))[3]);
        Assert.Equal(8, accessCounts.Rows.Single(row => Equals(row[0], 4))[3]);
    }

    [SqlFact]
    public async Task TypicalIncorrectSolutionsAreRejectedAndAlternativesAccepted()
    {
        QueryEvaluator evaluator = new(_database, _validator);
        LessonCatalog catalog = new();
        (int Id, string Sql)[] wrong = [
            (4, "SELECT Id, Zielort, Status FROM fall.Sendungen WHERE Zielort = 'Dortmund' OR Zielort = 'Essen' AND Status = 'Offen';"),
            (5, "SELECT Id, Wert FROM fall.Sendungen ORDER BY Wert ASC, Id ASC;"),
            (10, "SELECT Id, Bezeichnung FROM fall.Sendungen WHERE FahrzeugId = NULL;"),
            (16, "SELECT s.Id, s.Bezeichnung, f.Kennzeichen FROM fall.Sendungen s JOIN fall.Fahrzeuge f ON s.FahrzeugId = f.Id;"),
            (19, "SELECT COUNT(FahrzeugId) FROM fall.Sendungen;"),
            (24, "SELECT m.Id, m.Vorname, m.Nachname, COUNT(*) FROM fall.Mitarbeiter m LEFT JOIN fall.Zutritte z ON z.MitarbeiterId = m.Id GROUP BY m.Id, m.Vorname, m.Nachname;"),
            (28, "SELECT m.Id, m.Vorname, m.Nachname FROM fall.Mitarbeiter m WHERE EXISTS (SELECT 1 FROM fall.Fahrten r WHERE r.Zielort = 'Privatdepot');"),
            (30, "SELECT Id, Vorname, Nachname FROM fall.Mitarbeiter WHERE Id = 4;")];
        foreach (var attempt in wrong)
            Assert.False((await evaluator.EvaluateAsync(catalog.Lessons[attempt.Id - 1], attempt.Sql)).Correct, $"Falsche Lösung zu Mission {attempt.Id} wurde angenommen.");

        Evaluation alternative = await evaluator.EvaluateAsync(catalog.Lessons[16], "SELECT f.Id AS Nummer, f.Kennzeichen AS Schild FROM fall.Fahrzeuge f WHERE NOT EXISTS (SELECT 1 FROM fall.Sendungen s WHERE s.FahrzeugId = f.Id);");
        Assert.True(alternative.Correct, alternative.Message);
        Evaluation qualified = await evaluator.EvaluateAsync(catalog.Lessons[0], "SELECT fall.Sendungen.Id, fall.Sendungen.Bezeichnung FROM fall.Sendungen;");
        Assert.True(qualified.Correct, qualified.Message);
    }

    [SqlFact]
    public async Task AllThirtySolutionsAlsoWorkWithAnExplicitSchema()
    {
        QueryEvaluator evaluator = new(_database, _validator);
        foreach (Lesson lesson in new LessonCatalog().Lessons)
        {
            string sql = _validator.ForCaseData(lesson.ReferenceSql);
            Evaluation evaluation = await evaluator.EvaluateAsync(lesson, sql);
            Assert.True(evaluation.Correct, $"Mit Schema, Aufgabe {lesson.Id}: {evaluation.Message}");
        }
        Evaluation mixed = await evaluator.EvaluateAsync(new LessonCatalog().Lessons[16], "SELECT f.Id, f.Kennzeichen FROM Fahrzeuge f LEFT JOIN fall.Sendungen s ON s.FahrzeugId=f.Id WHERE s.Id IS NULL;");
        Assert.True(mixed.Correct, mixed.Message);
    }

    [SqlFact]
    public async Task DatabasePermissionsBlockChangesAndCrossDatabaseReadsEvenWithoutParser()
    {
        QueryResult before = await _database.ExecuteAsync("SELECT COUNT(*) FROM fall.Sendungen;");
        Assert.NotNull((await _database.ExecuteAsync("DELETE FROM fall.Sendungen;")).Error);
        // Öffentliche Systemmetadaten sind kein geeigneter Rechte-Test; die fremde Tabelle wird ohne Datenabruf geprüft.
        Assert.NotNull((await _database.ExecuteAsync("SELECT TOP (0) name FROM model.sys.tables;")).Error);
        Assert.NotNull((await _database.ExecuteAsync("SELECT Version FROM dbo.TrainerVersion;")).Error);
        Assert.NotNull((await _database.ExecuteAsync("CREATE TABLE fall.Unerlaubt (Id int);")).Error);
        Assert.NotNull((await _database.ExecuteAsync("REVERT; DELETE FROM fall.Sendungen;")).Error);
        QueryResult after = await _database.ExecuteAsync("SELECT COUNT(*) FROM fall.Sendungen;");
        Assert.Equal(before.Rows[0][0], after.Rows[0][0]);
    }

    [SqlFact]
    public async Task InvalidSqlAndExcessiveResultsReceiveGermanFeedback()
    {
        QueryEvaluator evaluator = new(_database, _validator);
        Assert.Contains("Spaltenname", (await evaluator.PreviewAsync("SELECT Fantasiespalte FROM fall.Sendungen;")).Error ?? "");
        Assert.Contains("GROUP BY", (await evaluator.PreviewAsync("SELECT Status, Id, COUNT(*) FROM fall.Sendungen GROUP BY Status;")).Error ?? "");
        QueryResult large = await evaluator.PreviewAsync("SELECT a.Id FROM fall.Zutritte a CROSS JOIN fall.Zutritte b;");
        Assert.True(large.Truncated);
        Assert.Equal(200, large.Rows.Count);
    }

    [SqlFact]
    public async Task SpacedDateTimeLiteralsWorkWithAGermanConnectionLanguage()
    {
        Microsoft.Data.SqlClient.SqlConnectionStringBuilder settings = new(Environment.GetEnvironmentVariable("FIAE_SQL_CONNECTION") ?? "Server=.\\SQLEXPRESS;Database=FIAE_AP2_Training;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Connect Timeout=5") { CurrentLanguage = "German" };
        TrainingDatabase germanDatabase = new(settings.ConnectionString);
        QueryEvaluator evaluator = new(germanDatabase, new SqlQueryValidator());
        Lesson lesson = new LessonCatalog().Lessons[11];
        Evaluation result = await evaluator.EvaluateAsync(lesson, lesson.ReferenceSql);
        Assert.True(result.Correct, result.Message);
        Assert.Equal(8, result.Result.Rows.Count);
        Assert.All(result.Result.Rows, row => Assert.Equal(new DateTime(2026, 10, 12), Assert.IsType<DateTime>(row[2]).Date));
    }

    [SqlFact]
    public async Task DoubleQuotedTextReceivesAHintWhileQuotedColumnsRemainValid()
    {
        QueryEvaluator evaluator = new(_database, _validator);
        const string WRONG_SQL = "SELECT Id, Bezeichnung, Wert FROM Sendungen WHERE Status = \"verschwunden\" AND Wert > 10000;";
        QueryResult wrong = await evaluator.PreviewAsync(WRONG_SQL);
        Assert.Contains("einfache Anführungszeichen", wrong.Error ?? "");
        Assert.Contains("WHERE Status = 'verschwunden'", wrong.Error ?? "");
        Assert.False(wrong.TechnicalFailure);
        Evaluation graded = await evaluator.EvaluateAsync(new LessonCatalog().Lessons[2], WRONG_SQL);
        Assert.False(graded.Correct);
        Assert.Contains("einfache Anführungszeichen", graded.Message);

        QueryResult correct = await evaluator.PreviewAsync("SELECT Id, Bezeichnung, Wert FROM Sendungen WHERE Status = 'verschwunden' AND Wert > 10000;");
        QueryResult quoted = await evaluator.PreviewAsync("SELECT \"Id\", \"Bezeichnung\", \"Wert\" FROM \"Sendungen\" WHERE \"Status\" = 'verschwunden' AND \"Wert\" > 10000;");
        Assert.Null(correct.Error);
        Assert.NotEmpty(correct.Rows);
        Assert.Null(quoted.Error);
        Assert.True(ResultComparer.AreEqual(correct, quoted, false));

        foreach (string sql in new[]
        {
            "SELECT Fantasiespalte FROM Sendungen; -- \"verschwunden\"",
            "SELECT Fantasiespalte FROM Sendungen /* \"verschwunden\" */;",
            "SELECT Fantasiespalte, 'Er sagt \"Hallo\"' FROM Sendungen;",
            "SELECT Fantasiespalte AS \"Meine Ausgabe\" FROM Sendungen;",
            "SELECT [Fantasiespalte] FROM Sendungen;"
        })
        {
            QueryResult result = await evaluator.PreviewAsync(sql);
            Assert.Contains("Spaltenname", result.Error ?? "");
            Assert.DoesNotContain("doppelte Anführungszeichen", result.Error ?? "");
        }
    }
}
