using FiaeTrainer.Core.Sql.Database;
using FiaeTrainer.Core.Sql.Exams;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Neue SQL-Fälle gegen echte Sitzungsarbeitskopien, unabhängige Zeilenzahlen und alternative Abfragen. </summary>
public sealed class SqlExamIntegrationTests : IAsyncLifetime
{
    private readonly TrainingDatabase _database = new(Environment.GetEnvironmentVariable("FIAE_SQL_CONNECTION") ?? "Server=.\\SQLEXPRESS;Database=FIAE_AP2_Training;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Connect Timeout=5");
    private readonly SqlExamCatalog _catalog = new();
    public async Task InitializeAsync() { if (Environment.GetEnvironmentVariable("FIAE_SQL_TESTS") == "1") await _database.InitializeAsync(); }
    public Task DisposeAsync() => Task.CompletedTask;

    [SqlFact]
    public async Task AllTwentyReferencesPassAndQueryDimensionsAreIndependentlyCounted()
    {
        Dictionary<int, (int Visible, int Probe, int Columns)> expected = new()
        {
            [1] = (3,3,3), [2] = (4,6,4), [3] = (1,2,2), [4] = (1,2,2),
            [6] = (2,3,2), [7] = (2,4,3), [8] = (2,3,2), [9] = (1,1,2),
            [11] = (2,3,3), [12] = (2,4,4), [13] = (3,5,2), [14] = (1,2,2), [16] = (5,8,2)
        };
        SqlExamEvaluator evaluator = new(_database);
        foreach (var item in _catalog.Cases)
        foreach (var task in item.Tasks)
        {
            var evaluation = await evaluator.EvaluateAsync(item, task, task.ReferenceSql);
            Assert.Equal(10, evaluation.Earned);
            Assert.Equal(10, evaluation.Maximum);
            if (task.Operation != "Query") continue;
            for (int probe = 0; probe < 2; probe++)
            {
                var run = await _database.ExecuteExamAsync(item, task, task.ReferenceSql, probe == 1);
                Assert.Null(run.Error);
                Assert.Equal(expected[task.Id].Columns, run.Query.Columns.Count);
                Assert.Equal(probe == 0 ? expected[task.Id].Visible : expected[task.Id].Probe, run.Query.Rows.Count);
                if (task.Id == 13 && probe == 1) Assert.Contains(run.Query.Rows, row => Convert.ToInt32(row[0]) == 5 && Convert.ToInt32(row[1]) == 0);
            }
        }
    }

    [SqlFact]
    public async Task AlternativeQueriesPassWhileVisibleDataConstantsAndWrongNullCountsFail()
    {
        SqlExamEvaluator evaluator = new(_database);
        var events = _catalog.Cases[0];
        var alternative = await evaluator.EvaluateAsync(events, events.Tasks[3], "SELECT v.Id, v.Titel FROM Veranstaltungen AS v LEFT JOIN Buchungen AS b ON b.VeranstaltungId=v.Id AND b.Status='bestaetigt' WHERE b.Id IS NULL;");
        Assert.Equal(10, alternative.Earned);
        Assert.Equal(2, (await evaluator.EvaluateAsync(events, events.Tasks[3], "SELECT Id, Titel FROM Veranstaltungen WHERE Id=4;")).Earned);
        var stock = _catalog.Cases[2];
        Assert.Equal(2, (await evaluator.EvaluateAsync(stock, stock.Tasks[2], "SELECT k.Id, COUNT(*) FROM Kategorien AS k LEFT JOIN Artikel AS a ON a.KategorieId=k.Id GROUP BY k.Id ORDER BY k.Id;")).Earned);
        Assert.Equal(8, (await evaluator.EvaluateAsync(stock, stock.Tasks[0], "SELECT Id, Name, Mindestbestand-Bestand FROM Artikel WHERE Bestand<Mindestbestand ORDER BY Id;")).Earned);
    }

    [SqlFact]
    public async Task UpdateFromJoinUsesTheTargetAliasAndChecksTheChangedRows()
    {
        var item = _catalog.Cases[0];
        var task = item.Tasks[4];
        var evaluator = new SqlExamEvaluator(_database);
        const string JOIN = "UPDATE b SET Status='bestaetigt' FROM Buchungen AS b JOIN Veranstaltungen AS v ON v.Id=b.VeranstaltungId WHERE b.Status='offen' AND v.Beginn>='20261101' AND v.Beginn<'20261201';";
        Assert.Equal(10, (await evaluator.EvaluateAsync(item, task, JOIN)).Earned);
        Assert.NotEqual(10, (await evaluator.EvaluateAsync(item, task, JOIN.Replace("20261201","20261101"))).Earned);
        Assert.NotNull(SqlExamValidator.Prepare(item, task, JOIN.Replace("UPDATE b","UPDATE v"), out _));
    }

    [SqlFact]
    public async Task DelimitedAliasWorksAndUnknownColumnPointsIntoLearnerText()
    {
        var item = _catalog.Cases[0];
        var task = item.Tasks[1];
        var query = task.ReferenceSql.Replace("SELECT b.Id,", "SELECT b.Id AS [Buchungs-Id],");
        Assert.Equal(10,(await new SqlExamEvaluator(_database).EvaluateAsync(item,task,query)).Earned);
        var invalid = await _database.ExecuteExamAsync(item,task,"SELECT b.Id,\n       b.FalscheSpalte FROM Buchungen AS b;");
        Assert.Contains("FalscheSpalte",invalid.Error);
        Assert.Contains("Zeile 2",invalid.Error);
    }

    [SqlFact]
    public async Task FestivalAcceptsEquivalentSolutionsForAllFiveTasks()
    {
        var item = _catalog.Cases.Single(c => c.Id == "stadtfest");
        var evaluator = new SqlExamEvaluator(_database);
        string[] alternatives =
        [
            "SELECT MaterialId, Menge FROM DepotSued UNION ALL SELECT MaterialId, Menge FROM DepotNord;",
            "ALTER TABLE Rueckgaben WITH CHECK ADD FOREIGN KEY (AusgabeId) REFERENCES Ausgaben (Id) ON DELETE NO ACTION;",
            "UPDATE Material SET Bestand=(SELECT z.Menge FROM Zaehlliste z WHERE z.MaterialId=Material.Id) WHERE EXISTS (SELECT 1 FROM Zaehlliste z WHERE z.MaterialId=Material.Id AND z.Menge IS NOT NULL);",
            "DELETE a OUTPUT DELETED.Id, DELETED.Stand, DELETED.Erledigt INTO AusgabeArchiv (Id,Stand,Erledigt) FROM Ausgaben a WHERE a.Erledigt=1;",
            "REVOKE UPDATE, INSERT ON OBJECT::[Ausgabeliste] FROM [Empfang];"
        ];
        foreach (var (task, answer) in item.Tasks.Zip(alternatives))
        {
            var result = await evaluator.EvaluateAsync(item, task, answer);
            Assert.True(result.Earned == 10, task.Title + ": " + string.Join(" / ",result.Checks.Where(c => c.Earned < c.Maximum).Select(c => c.Detail)));
        }
        Assert.Equal(10, (await evaluator.EvaluateAsync(item, item.Tasks[4], "DENY UPDATE ON Ausgabeliste TO Empfang;")).Earned);
    }

    [SqlFact]
    public async Task FestivalDistinguishesIncorrectDataRulesMissingCountsAndIncompleteRights()
    {
        var item = _catalog.Cases.Single(c => c.Id == "stadtfest");
        var evaluator = new SqlExamEvaluator(_database);
        var cases = new (int Index, string Answer, int Points, string Failed)[]
        {
            (0, item.Tasks[0].ReferenceSql.Replace("UNION ALL", "UNION"), 2, "Ergebniswerte"),
            (1, item.Tasks[1].ReferenceSql.TrimEnd(';') + " ON DELETE CASCADE;", 9, "Ausgabe mit Rückgabe"),
            (1, item.Tasks[1].ReferenceSql.Replace("ADD CONSTRAINT", "WITH NOCHECK ADD CONSTRAINT"), 7, "Verknüpfte Spalten"),
            (2, item.Tasks[2].ReferenceSql.Replace("z.Menge IS NOT NULL", "z.Menge > 0"), 8, "Gemeldete null"),
            (2, item.Tasks[2].ReferenceSql.Replace("SET Bestand = z.Menge","SET Bestand = ISNULL(z.Menge, 0)").Replace("WHERE z.Menge IS NOT NULL",""), 7, "Fehlender Messwert"),
            (3, item.Tasks[3].ReferenceSql.Replace("BEGIN TRANSACTION;", "").Replace("COMMIT;", ""), 8, "Gemeinsame Transaktion"),
            (3, "INSERT AusgabeArchiv SELECT * FROM Ausgaben WHERE Erledigt=1;", 5, "Aktive Ausgaben"),
            (4, "GRANT SELECT ON Ausgabeliste TO Empfang;", 7, "Sicht vor Änderungen"),
            (4, item.Tasks[4].ReferenceSql + " GRANT SELECT ON Ausgaben TO Empfang;", 8, "Weitere Rechte"),
            (4, "REVOKE SELECT, UPDATE ON Ausgabeliste FROM Empfang;", 7, "Offene Ausgaben")
        };
        foreach (var example in cases)
        {
            var result = await evaluator.EvaluateAsync(item, item.Tasks[example.Index], example.Answer);
            Assert.Equal(example.Points, result.Earned);
            Assert.Contains(result.Checks, c => c.Earned == 0 && c.Name.StartsWith(example.Failed, StringComparison.Ordinal));
        }
    }

    [SqlFact]
    public async Task FestivalGradesHiddenChangesAndDiscardsFailedTransfers()
    {
        var item = _catalog.Cases.Single(c => c.Id == "stadtfest");
        var stock = item.Tasks[2];
        string visibleOnly = stock.ReferenceSql.Replace("IS NOT NULL;", "IS NOT NULL AND m.Id <= 4;");
        var evaluator = new SqlExamEvaluator(_database);
        Assert.Equal(8, (await evaluator.EvaluateAsync(item, stock, visibleOnly)).Earned);
        var archive = item.Tasks[3];
        string duplicate = archive.ReferenceSql.Replace("DELETE FROM", "INSERT AusgabeArchiv SELECT * FROM Ausgaben WHERE Erledigt=1; DELETE FROM");
        var failed = await _database.ExecuteExamAsync(item, archive, duplicate);
        Assert.NotNull(failed.Error);
        Assert.False(failed.TechnicalFailure);
        Assert.Equal(10, (await evaluator.EvaluateAsync(item, archive, archive.ReferenceSql)).Earned);
    }

    [Theory]
    [InlineData(1, "ALTER TABLE Ausgaben ADD FOREIGN KEY (Id) REFERENCES Material (Id);")]
    [InlineData(3, "COMMIT;")]
    [InlineData(3, "BEGIN TRANSACTION; DELETE FROM Ausgaben;")]
    [InlineData(3, "DELETE FROM AusgabeArchiv;")]
    [InlineData(3, "DELETE FROM Ausgaben OUTPUT DELETED.Id INTO Material;")]
    [InlineData(4, "GRANT SELECT ON Material TO Empfang;")]
    [InlineData(4, "GRANT UPDATE ON Ausgabeliste TO Andere;")]
    public void FestivalRejectsAnswersOutsideTheTask(int index, string answer)
    {
        var item = _catalog.Cases.Single(c => c.Id == "stadtfest");
        Assert.NotNull(SqlExamValidator.Prepare(item, item.Tasks[index], answer, out _));
    }

    [Fact]
    public void EachExamTaskExplainsItsOwnSolution()
    {
        var tasks = _catalog.Cases.SelectMany(c => c.Tasks).ToList();
        Assert.Equal(20,tasks.Select(t => t.Explanation).Distinct().Count());
        Assert.All(tasks,t => Assert.Contains('\n',t.ReferenceSql));
    }

    [Theory]
    [InlineData("SELECT b.Id AS Buchungs-Id FROM Buchungen b;", "Bindestrich", "Spalte 24")]
    [InlineData("SELECT b.Id FROM Buchungen b\nWHERE b.Status IS NOT 'offen';", "IS NULL", "Zeile 2")]
    [InlineData("SELECT b.Id FROM Buchungen b WHERE b.Id=1\nJOIN Veranstaltungen v ON v.Id=b.VeranstaltungId;", "JOIN", "Zeile 2")]
    [InlineData("UPDATE Buchungen b SET Status='offen';", "Zielalias", "Zeile 1")]
    public void DiagnosticsLocateTheFirstProblemInTheOriginalAnswer(string sql, string reason, string location)
    {
        var item = _catalog.Cases[0];
        string? message = SqlExamValidator.Prepare(item,item.Tasks[0],sql,out _);
        Assert.Contains(reason,message);
        Assert.Contains(location,message);
    }

    [SqlFact]
    public async Task WritingTasksAreIsolatedAndDoNotLeakIntoTheNextAttempt()
    {
        var item = _catalog.Cases[0];
        var change = item.Tasks[4];
        Assert.Null((await _database.ExecuteExamAsync(item, change, change.ReferenceSql)).Error);
        var read = await _database.ExecuteExamAsync(item, item.Tasks[1], item.Tasks[1].ReferenceSql);
        Assert.Equal(4, read.Query.Rows.Count);
        var stock = _catalog.Cases[2];
        var first = await _database.ExecuteExamAsync(stock, stock.Tasks[4], stock.Tasks[4].ReferenceSql);
        var second = await _database.ExecuteExamAsync(stock, stock.Tasks[4], stock.Tasks[4].ReferenceSql);
        Assert.Null(first.Error);
        Assert.Null(second.Error);
        Assert.Equal(7, second.State["Artikel:Spalten"].Rows.Count);
    }
}
