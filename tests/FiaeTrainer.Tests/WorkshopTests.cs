using FiaeTrainer.Core;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft die erlaubte Sprache und sichere Umleitung schreibender Werkstattaufgaben. </summary>
public sealed class WorkshopValidatorTests
{
    [Theory]
    [InlineData("UPDATE fall.Sendungen SET Wert=0;")]
    [InlineData("DELETE FROM master.sys.objects;")]
    [InlineData("UPDATE #Artikel SET Preis=0;")]
    [InlineData("UPDATE ##Artikel SET Preis=0;")]
    [InlineData("EXEC sp_executesql N'DELETE FROM fall.Sendungen';")]
    [InlineData("SELECT * INTO Artikel FROM fall.Sendungen;")]
    [InlineData("DROP TABLE Artikel")]
    [InlineData("ALTER TABLE Artikel DROP COLUMN Preis;")]
    [InlineData("CREATE TABLE FremdeTabelle (Id int);")]
    [InlineData("BEGIN DISTRIBUTED TRANSACTION; COMMIT;")]
    [InlineData("REVERT")]
    [InlineData("WAITFOR DELAY '00:00:02';")]
    [InlineData("CREATE TABLE Rueckgaben(Id int, Grund nvarchar(max));")]
    [InlineData("CREATE TABLE Rueckgaben(Id int REFERENCES Konten(Id));")]
    [InlineData("INSERT Artikel (Id) SELECT Id FROM Konten;")]
    public void UnsafeOrUnsupportedScriptsAreRejected(string sql) => Assert.NotNull(new WorkshopSqlValidator().Validate(sql));

    [Fact]
    public void AllWorkshopLessonsUseSupportedSqlAndHaveThreeHints()
    {
        WorkshopCatalog catalog = new();
        Assert.Equal(Enumerable.Range(1, 10), catalog.Lessons.Select(x => x.Id));
        foreach (WorkshopLesson lesson in catalog.Lessons)
        {
            Assert.Null(new WorkshopSqlValidator().Validate(lesson.ReferenceSql, lesson.Operation));
            Assert.Equal(3, lesson.Hints.Length);
        }
    }

    [Fact]
    public void RewritingPreservesTextAndAliases()
    {
        WorkshopSqlValidator validator = new();
        Assert.Contains("#Artikel", validator.ToSessionTables("UPDATE Artikel SET Name='Artikel' WHERE Id=1;"));
        Assert.Contains("'Artikel'", validator.ToSessionTables("UPDATE Artikel SET Name='Artikel' WHERE Id=1;"));
        string select = validator.ToSessionTables("SELECT Artikel.Id FROM Artikel AS Artikel;");
        Assert.Contains("Artikel.Id", select);
        Assert.Contains("#Artikel AS Artikel", select);
    }

    [Fact]
    public void RollbackRequiresAnActualTransactionAndUpdate()
    {
        WorkshopSqlValidator validator = new();
        Assert.NotNull(validator.Validate("SELECT * FROM Konten;", WorkshopOperation.Rollback));
        Assert.NotNull(validator.Validate("BEGIN TRANSACTION; ROLLBACK; UPDATE Konten SET Guthaben=Guthaben-50 WHERE Id=1;", WorkshopOperation.Rollback));
    }
}

/// <summary> Prüft Datenänderungen, Struktur und Rücknahme auf echtem SQL Server ohne dauerhafte Schreibrechte. </summary>
public sealed class WorkshopIntegrationTests : IAsyncLifetime
{
    /// <summary> Ausschließlich die synthetische Trainingsdatenbank. </summary>
    private readonly TrainingDatabase _database = new(Environment.GetEnvironmentVariable("FIAE_SQL_CONNECTION") ?? "Server=.\\SQLEXPRESS;Database=FIAE_AP2_Training;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Connect Timeout=5");

    /// <summary> Die Tests benötigen keinen vorherigen App-Start. </summary>
    public async Task InitializeAsync()
    {
        if (Environment.GetEnvironmentVariable("FIAE_SQL_TESTS") == "1")
            await _database.InitializeAsync();
    }
    /// <summary> Jede Ausführung schließt ihre eigene Verbindung. </summary>
    public Task DisposeAsync() => Task.CompletedTask;

    [SqlFact]
    public async Task AllTenSolutionsPassBothDatasetsAndHaveExpectedEffects()
    {
        WorkshopEvaluator evaluator = new(_database, new WorkshopSqlValidator());
        foreach (WorkshopLesson lesson in new WorkshopCatalog().Lessons)
        {
            WorkshopEvaluation result = await evaluator.EvaluateAsync(lesson, lesson.ReferenceSql);
            Assert.True(result.Correct, $"Werkstatt {lesson.Id}: {result.Message}");
            Assert.Equal(lesson.Id == 3 ? 6 : lesson.Id == 4 ? 4 : 5, result.Run.Tables["Artikel"].Rows.Count);
            if (lesson.Id == 1)
                Assert.Equal(44.90m, result.Run.Tables["Artikel"].Rows.Single(x => Equals(x[0], 2))[3]);
            if (lesson.Id == 6)
            {
                Assert.Empty(result.Run.Tables["Rueckgaben"].Rows);
                Assert.Equal(new object?[] { "Rückgabe: Menge 0", false }, result.Run.ConstraintChecks.Rows.Single(x => Equals(x[0], "Rückgabe: Menge 0")));
            }
            if (lesson.Id == 9)
            {
                Assert.Equal(550m, result.Run.Tables["Konten"].Rows[0][2]);
                Assert.Equal(200m, result.Run.Tables["Konten"].Rows[1][2]);
                Assert.Equal(100m, result.Run.Tables["Konten"].Rows[2][2]);
            }
            if (lesson.Id == 10)
                Assert.Equal(600m, result.Run.Tables["Konten"].Rows[0][2]);
        }
    }

    [SqlFact]
    public async Task WrongFiltersHardcodedBalancesAndMissingConstraintsAreRejected()
    {
        WorkshopEvaluator evaluator = new(_database, new WorkshopSqlValidator());
        WorkshopCatalog catalog = new();
        (int Id, string Sql)[] wrong = [
            (1,"UPDATE Artikel SET Preis=44.90;"),
            (2,"UPDATE Artikel SET Preis=ROUND(Preis*1.10,2) WHERE Id IN(1,2);"),
            (4,"DELETE Artikel WHERE Id=4;"),
            (5,"UPDATE Artikel SET Bestand=10 WHERE Aktiv=1 AND Bestand<5;"),
            (6,"CREATE TABLE Rueckgaben(Id int PRIMARY KEY, Grund nvarchar(120) NOT NULL, Menge int NOT NULL);"),
            (6,"CREATE TABLE Rueckgaben(Id int PRIMARY KEY, Grund nvarchar(120) NOT NULL, Menge int NOT NULL CHECK(Menge>=0));"),
            (8,"ALTER TABLE Artikel ADD CHECK(Bestand>0);"),
            (9,"BEGIN TRANSACTION; UPDATE Konten SET Guthaben=550 WHERE Id=1; UPDATE Konten SET Guthaben=200 WHERE Id=2; COMMIT;"),
            (10,"SELECT * FROM Konten;")];
        wrong = [.. wrong, (10,"BEGIN TRANSACTION; UPDATE Konten SET Guthaben=Guthaben WHERE Id=1; ROLLBACK;"), (10,"BEGIN TRANSACTION; UPDATE Konten SET Guthaben=Guthaben-10 WHERE Id=1; ROLLBACK;")];
        foreach (var example in wrong)
            Assert.False((await evaluator.EvaluateAsync(catalog.Lessons[example.Id-1],example.Sql)).Correct, $"Falsche Werkstattlösung {example.Id} angenommen.");
    }

    [SqlFact]
    public async Task EachRunStartsFreshAndParallelRunsDoNotShareTables()
    {
        WorkshopEvaluator evaluator = new(_database, new WorkshopSqlValidator());
        WorkshopRun changed = await evaluator.PreviewAsync("UPDATE Artikel SET Preis=1 WHERE Id=1;");
        Assert.Equal(1m, changed.Tables["Artikel"].Rows[0][3]);
        WorkshopRun fresh = await evaluator.GetInitialStateAsync();
        Assert.Equal(25m, fresh.Tables["Artikel"].Rows[0][3]);
        WorkshopLesson create = new WorkshopCatalog().Lessons[5];
        WorkshopEvaluation[] parallel = await Task.WhenAll(evaluator.EvaluateAsync(create, create.ReferenceSql), evaluator.EvaluateAsync(create, create.ReferenceSql));
        Assert.All(parallel, x => Assert.True(x.Correct, x.Message));
        Assert.Equal(12, (await _database.ExecuteAsync("SELECT COUNT(*) FROM fall.Sendungen;")).Rows[0][0]);
    }

    [SqlFact]
    public async Task OpenTransactionsAndConstraintViolationsReceiveFeedback()
    {
        WorkshopEvaluator evaluator = new(_database, new WorkshopSqlValidator());
        Assert.Contains("noch offen", (await evaluator.PreviewAsync("BEGIN TRANSACTION; UPDATE Konten SET Guthaben=0 WHERE Id=1;")).Error ?? "");
        Assert.Contains("Datenregel", (await evaluator.PreviewAsync("UPDATE Konten SET Guthaben=-1 WHERE Id=1;")).Error ?? "");
        Assert.Equal(600m, (await evaluator.GetInitialStateAsync()).Tables["Konten"].Rows[0][2]);
    }

    [SqlFact]
    public async Task EquivalentNamedConstraintsAndLowercaseIdentifiersAreAccepted()
    {
        WorkshopEvaluator evaluator = new(_database, new WorkshopSqlValidator());
        string sql = "CREATE TABLE rueckgaben(id int NOT NULL, grund nvarchar(120) NOT NULL, menge int NOT NULL, CONSTRAINT PK_R PRIMARY KEY(id), CONSTRAINT CK_M CHECK(menge>0));";
        WorkshopEvaluation result = await evaluator.EvaluateAsync(new WorkshopCatalog().Lessons[5], sql);
        Assert.True(result.Correct, result.Message);
    }

    [SqlFact]
    public async Task SessionExecutorStillCannotWritePermanentTables()
    {
        Assert.NotNull((await _database.ExecuteWorkshopAsync("UPDATE fall.Sendungen SET Wert=0;")).Error);
        Assert.NotNull((await _database.ExecuteWorkshopAsync("CREATE TABLE fall.Unerlaubt(Id int);")).Error);
        Assert.Equal(162000m, (await _database.ExecuteAsync("SELECT SUM(Wert) FROM fall.Sendungen;")).Rows[0][0]);
    }

    [SqlFact]
    public async Task WorkshopDistinguishesDoubleQuotedColumnsFromTextValues()
    {
        WorkshopEvaluator evaluator = new(_database, new WorkshopSqlValidator());
        WorkshopRun wrong = await evaluator.PreviewAsync("UPDATE Artikel SET Kategorie = \"Sensoren\" WHERE Id = 1;");
        Assert.Contains("einfache Anführungszeichen", wrong.Error ?? "");
        Assert.False(wrong.TechnicalFailure);
        WorkshopRun correct = await evaluator.PreviewAsync("UPDATE Artikel SET \"Kategorie\" = 'Sensoren' WHERE \"Id\" = 1;");
        Assert.Null(correct.Error);
        Assert.Equal("Sensoren", correct.Tables["Artikel"].Rows[0][2]);
        WorkshopRun ordinaryError = await evaluator.PreviewAsync("UPDATE Artikel SET Fantasiespalte = 'Text mit \"Anführungszeichen\"' WHERE Id = 1;");
        Assert.Contains("Spaltenname", ordinaryError.Error ?? "");
        Assert.DoesNotContain("doppelte Anführungszeichen", ordinaryError.Error ?? "");
    }
}
