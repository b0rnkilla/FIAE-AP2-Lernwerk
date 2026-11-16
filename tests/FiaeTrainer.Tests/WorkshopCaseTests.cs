using FiaeTrainer.Core.Sql.Database;
using FiaeTrainer.Core.Sql.Workshop;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Führt die neuen Fallaufgaben auf echten SQL-Server-Objekten mit unabhängigen Gegenbeispielen aus. </summary>
public sealed class WorkshopCaseTests : IAsyncLifetime
{
    private readonly TrainingDatabase _database = new(Environment.GetEnvironmentVariable("FIAE_SQL_CONNECTION") ?? "Server=.\\SQLEXPRESS;Database=FIAE_AP2_Training;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");
    private readonly WorkshopCatalog _catalog = new();
    public Task InitializeAsync() => Environment.GetEnvironmentVariable("FIAE_SQL_TESTS") == "1" ? _database.InitializeAsync() : Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;

    [SqlFact]
    public async Task AllTwentySolutionsPassAndLeaveFreshTablesAndPermissions()
    {
        WorkshopEvaluator evaluator = new(_database,new());
        foreach (var lesson in _catalog.Lessons.Where(l => l.Scenario.Length > 0))
        {
            var result = await evaluator.EvaluateAsync(lesson,lesson.ReferenceSql);
            Assert.True(result.Correct,$"{lesson.Id}: {result.Message} {result.Run.Error}");
            if (lesson.Id == 16) Assert.Equal(7,result.Run.Selection!.Rows.Count);
            if (lesson.Id == 17) Assert.Equal(4,result.Run.Selection!.Rows.Count);
            if (lesson.Id == 22) Assert.Equal(new[] {6,0,7,9}, result.Run.Tables["Artikel"].Rows.OrderBy(r => r[0]).Select(r => (int)r[1]!));
        }
        var start = await evaluator.GetInitialStateAsync(_catalog.Lessons.Single(l => l.Id == 28));
        Assert.Null(start.Error);
        Assert.Contains(start.ConstraintChecks.Rows, row => Equals(row[0],"Tagesplan lesen · Zeilen") && Equals(row[1],"kein Zugriff"));
    }

    [SqlFact]
    public async Task ExplicitForeignKeyCheckAndFreelyNamedIndexAreEquivalent()
    {
        WorkshopEvaluator evaluator = new(_database,new());
        var key = await evaluator.EvaluateAsync(_catalog.Lessons.Single(l => l.Id == 11),
            "ALTER TABLE Rueckgaben WITH CHECK ADD CONSTRAINT [Beleg-Verweis] FOREIGN KEY (BestellungId) REFERENCES Bestellungen(Id) ON DELETE NO ACTION;");
        Assert.True(key.Correct,key.Message);
        var index = await evaluator.EvaluateAsync(_catalog.Lessons.Single(l => l.Id == 30),
            "CREATE INDEX [Suche nach Titel] ON Termine (Bestaetigt ASC, Titel ASC);");
        Assert.True(index.Correct,index.Message);
    }

    [SqlFact]
    public async Task ConcurrentAttemptsStartWithIndependentPermissions()
    {
        WorkshopEvaluator evaluator = new(_database,new());
        var grant = _catalog.Lessons.Single(l => l.Id == 28);
        var revoke = _catalog.Lessons.Single(l => l.Id == 29);
        var runs = await Task.WhenAll(evaluator.PreviewAsync(grant,"GRANT SELECT ON OBJECT::Tagesplan TO Empfang;"),
            evaluator.PreviewAsync(revoke,"REVOKE UPDATE ON Tagesplan FROM Empfang;"));
        foreach (var run in runs)
        {
            Assert.Null(run.Error);
            Assert.Contains(run.ConstraintChecks.Rows,r => Equals(r[0],"Tagesplan lesen · Zeilen") && Equals(r[1],2));
            Assert.Contains(run.ConstraintChecks.Rows,r => Equals(r[0],"Titel im Tagesplan ändern") && Equals(r[1],"kein Zugriff"));
        }
    }

    [SqlFact]
    public async Task MissingRulesWrongQuantitiesAndExcessiveRightsFail()
    {
        WorkshopEvaluator evaluator = new(_database,new());
        (int Id, string Wrong)[] cases = [
            (11,"ALTER TABLE Rueckgaben ADD FOREIGN KEY (BestellungId) REFERENCES Bestellungen(Id) ON DELETE CASCADE;"),
            (12,"ALTER TABLE Rueckgaben ADD FOREIGN KEY (ArtikelId) REFERENCES Artikel(Id);"),
            (13,"ALTER TABLE Rueckgaben ADD FOREIGN KEY (BestellungId) REFERENCES Bestellungen(Id);"),
            (14,"CREATE TABLE Positionen (BestellungId int NOT NULL, ArtikelId int NOT NULL, PRIMARY KEY (BestellungId,ArtikelId));"),
            (15,"ALTER TABLE Positionen ADD UNIQUE (ArtikelId);"),
            (16,"SELECT ArtikelId,Menge FROM Nord UNION SELECT ArtikelId,Menge FROM Sued;"),
            (17,"SELECT ArtikelId FROM Nord UNION SELECT ArtikelId FROM Nord;"),
            (18,"SELECT ArtikelId FROM Sued EXCEPT SELECT ArtikelId FROM Nord;"),
            (19,"SELECT ArtikelId FROM Nord INTERSECT SELECT ArtikelId FROM Nord;"),
            (20,"INSERT Archiv SELECT DISTINCT ArtikelId,Menge FROM Nord WHERE Menge>0;"),
            (21,"INSERT Archiv SELECT ArtikelId,Menge FROM Nord WHERE Menge>0; DELETE Nord WHERE Menge>0;"),
            (22,"UPDATE a SET Bestand=m.Gezaehlt FROM Artikel a JOIN Meldungen m ON a.Id=m.ArtikelId WHERE m.Gezaehlt>0;"),
            (23,"SELECT Id,Bestand,'Bestand' FROM Artikel;"),
            (24,"SELECT MAX(Menge) FROM Nord;"),
            (25,"ALTER TABLE Termine ADD Freigabe int NULL; UPDATE Termine SET Freigabe=1;"),
            (26,"ALTER TABLE Termine ADD DEFAULT (2) FOR Stufe;"),
            (27,"CREATE VIEW Tagesplan AS SELECT Id,Titel FROM Termine;"),
            (28,"GRANT SELECT ON Termine TO Empfang;"),
            (29,"REVOKE SELECT ON Tagesplan FROM Empfang;"),
            (30,"CREATE INDEX IX_Test ON Termine (Titel,Bestaetigt);")
        ];
        foreach (var (id,sql) in cases)
        {
            var actual = await evaluator.EvaluateAsync(_catalog.Lessons.Single(l => l.Id == id),sql);
            Assert.False(actual.TechnicalFailure,$"{id}: {actual.Message}");
            Assert.False(actual.Correct,$"Werkstatt {id}");
        }
    }
}
