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
    public async Task AllFifteenReferencesPassAndQueryDimensionsAreIndependentlyCounted()
    {
        Dictionary<int, (int Visible, int Probe, int Columns)> expected = new()
        {
            [1] = (3,3,3), [2] = (4,6,4), [3] = (1,2,2), [4] = (1,2,2),
            [6] = (2,3,2), [7] = (2,4,3), [8] = (2,3,2), [9] = (1,1,2),
            [11] = (2,3,3), [12] = (2,4,4), [13] = (3,5,2), [14] = (1,2,2)
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
