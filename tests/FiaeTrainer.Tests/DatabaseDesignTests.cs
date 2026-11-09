using FiaeTrainer.Core.DatabaseDesign.Content;
using FiaeTrainer.Core.DatabaseDesign.Models;
using FiaeTrainer.Core.DatabaseDesign.Evaluation;
using FiaeTrainer.Core.DatabaseDesign.Practice;
using FiaeTrainer.Web.Learning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Unabhängige Struktur-, Alternativ-, Fehler- und Zeitprüfungen für den Modellbaukasten. </summary>
public sealed class DatabaseDesignTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-Design", Guid.NewGuid().ToString("N"));
    private readonly DesignCatalog _catalog = new();
    public static IEnumerable<object[]> LearningIds => new DesignCatalog().Lessons.Select(task => new object[] { task.Id });
    [Theory, MemberData(nameof(LearningIds))]
    public void EveryReferenceAndExplicitVariantPassesButEmptyAnswerFails(int id)
    {
        var task = _catalog.Lessons.Single(task => task.Id == id);
        foreach (var answer in task.Kind == "Model" ? task.Solutions.Select(model => new DesignAnswer { Model = model.Copy() }) : [task.Kind == "ErCompletion" ? new DesignAnswer { DiagramValues = task.DiagramSlots.ToDictionary(slot => slot.Id, slot => slot.CorrectId) } : new DesignAnswer { SelectedIds = task.CorrectIds.ToList() }])
            Assert.Equal(50, DesignEvaluator.Evaluate(task, answer).Earned);
        Assert.False(DesignEvaluator.Evaluate(task, new()).Correct);
    }
    [Fact]
    public void OriginalSixChaptersAndThreeCasesRemainAlongsideNewDiagramChapters()
    {
        Assert.Equal(6, _catalog.Lessons.Where(task => task.Id <= 30).GroupBy(task => task.Chapter).Count());
        Assert.All(_catalog.Lessons.Where(task => task.Id <= 30).GroupBy(task => task.Chapter), group => Assert.Equal(5, group.Count()));
        Assert.Equal(42, _catalog.Lessons.Count);
        Assert.Equal(10, _catalog.Lessons.GroupBy(task => task.Chapter).Count());
        Assert.Equal(3, _catalog.Cases.Count);
        Assert.All(_catalog.Cases, item => { Assert.Equal(50, item.Points.Sum()); Assert.Equal(3, item.Tasks.Count); });
        var category = _catalog.Lessons.Single(task => task.Id == 18).Solutions[0];
        Assert.Equal(2, category.Tables.Count);
        Assert.Single(category.Tables, table => table.Columns.Any(column => column.Attribute == "Kategoriename"));
        Assert.Equal(2, category.Relations[0].ParentMinimum + category.Relations.Count);
    }
    [Fact]
    public void NamesIdsOrderAndUniqueGroupLabelsDoNotChangeMeaning()
    {
        var task = _catalog.Lessons[27]; var model = task.Solutions[1].Copy();
        Dictionary<string, string> ids = model.Tables.Select(table => table.Id).Concat(model.Tables.SelectMany(table => table.Columns.Select(column => column.Id))).ToDictionary(id => id, _ => Guid.NewGuid().ToString("N"));
        foreach (var table in model.Tables) { table.Id = ids[table.Id]; table.Name = "Eigene_" + table.Id[..6]; table.Columns.Reverse(); foreach (var column in table.Columns) { column.Id = ids[column.Id]; column.Name = "Feld_" + column.Id[..6]; if (column.UniqueGroup.Length > 0) column.UniqueGroup = "B"; } }
        foreach (var relation in model.Relations) { relation.SourceTable = ids[relation.SourceTable]; relation.TargetTable = ids[relation.TargetTable]; foreach (var pair in relation.Pairs) { pair.Source = ids[pair.Source]; pair.Target = ids[pair.Target]; } }
        model.Tables.Reverse(); model.Relations.Reverse();
        Assert.True(DesignEvaluator.Evaluate(task, new() { Model = model }).Correct);
    }
    [Fact]
    public void MissingCompositeKeyAndUnwantedUniquenessAreNotAccepted()
    {
        var task = _catalog.Lessons[9]; var model = task.Solutions[0].Copy();
        model.Tables.Single(table => table.Name == "Position").Columns.Single(column => column.Attribute == "ArtikelId").PrimaryKey = false;
        var result = DesignEvaluator.Evaluate(task, new() { Model = model }); Assert.InRange(result.Earned, 1, 49);
        var customer = _catalog.Lessons[4]; var duplicate = customer.Solutions[0].Copy(); duplicate.Tables[0].Columns.Single(column => column.Attribute == "Mail").UniqueGroup = "A";
        Assert.False(DesignEvaluator.Evaluate(customer, new() { Model = duplicate }).Correct);
    }
    [Fact]
    public void OptionalForeignKeyAndMinimumTwoHaveIndependentContracts()
    {
        var task = _catalog.Lessons[11]; var model = task.Solutions[0].Copy(); model.Tables[1].Columns.Single(column => column.Attribute == "KundeId").Nullable = false;
        Assert.False(DesignEvaluator.Evaluate(task, new() { Model = model }).Correct);
        var minimum = _catalog.Lessons[28]; var wrong = minimum.Solutions[0].Copy(); wrong.Relations[0].ChildMinimum = 0;
        Assert.InRange(DesignEvaluator.Evaluate(minimum, new() { Model = wrong }).Earned, 1, 49);
    }
    [Fact]
    public void DanglingPairsAndDuplicateSemanticFieldsProduceFeedbackWithoutExceptions()
    {
        var task = _catalog.Lessons[7]; var model = task.Solutions[0].Copy(); model.Relations[0].Pairs[0].Target = "unbekannt";
        Assert.False(DesignEvaluator.Evaluate(task, new() { Model = model }).Correct);
        model = task.Solutions[0].Copy(); var extra = new DesignColumn { Attribute = "KundeId", Name = "AndereNummer" }; model.Tables[0].Columns.Add(extra);
        Assert.False(DesignEvaluator.Evaluate(task, new() { Model = model }).Correct);
        Assert.False(DesignModelRules.Storable(new() { Model = new() { Tables = null! } }));
        model = task.Solutions[0].Copy(); model.Tables[0].Columns[1].Attribute = "Mail|Name";
        Assert.False(DesignEvaluator.Evaluate(task, new() { Model = model }).Correct);
    }
    [Fact]
    public async Task LegacyProfilesKeepOtherModulesAndRejectStaleLessonWindows()
    {
        Directory.CreateDirectory(_directory); await File.WriteAllTextAsync(Path.Combine(_directory, "progress.json"), "{\"LastLessonId\":17,\"Lessons\":{\"17\":{\"Sql\":\"SELECT 1;\",\"Completed\":true}}}");
        var store = Store(); var task = _catalog.Lessons[4]; var progress = new DesignProgress { Answer = new() { Model = task.Solutions[0].Copy() } };
        Assert.True((await store.SaveDesignAsync(5, progress, _catalog, true))!.Correct);
        var restored = await Store().LoadAsync(); Assert.Equal("SELECT 1;", restored.Lessons[17].Sql); Assert.True(restored.DesignLessons[5].Independent);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveDesignAsync(5, new() { Answer = task.InitialAnswer() }, _catalog));
        restored.DesignLessons[5].IndependentMode = false; restored.DesignLessons[5].Assisted = true; restored.DesignLessons[5].Answer = task.InitialAnswer(); await store.SaveDesignAsync(5, restored.DesignLessons[5], _catalog);
        Assert.True((await store.LoadAsync()).DesignLessons[5].Completed);
    }
    [Theory]
    [InlineData("werkstatt")] [InlineData("bibliothek")] [InlineData("fertigung")]
    public async Task EachExamScoresFiftyAndSubmissionIsImmutable(string id)
    {
        var item = _catalog.Cases.Single(item => item.Id == id); var round = new DesignExamRound { CaseId = id };
        foreach (var task in item.Tasks) round.Answers[task.Id] = task.Kind == "Model" ? new() { Model = task.Solutions[0].Copy() } : new() { SelectedIds = task.CorrectIds.ToList() };
        var store = Store(); await store.StartDesignExamAsync(round); round = await store.SaveDesignExamAsync(round, _catalog, true);
        Assert.Equal(50, round.Earned); Assert.Single((await store.LoadAsync()).DesignHistory);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveDesignExamAsync(round, _catalog, true));
        Assert.Single((await Store().LoadAsync()).DesignHistory);
    }
    [Fact]
    public async Task ExpiredExamRejectsLateAnswersAndLatePause()
    {
        var round = new DesignExamRound { CaseId = "werkstatt", RunningSinceUtc = DateTimeOffset.UtcNow.AddMinutes(-46) }; var store = Store(); await store.StartDesignExamAsync(round);
        round.Answers[1] = new() { Model = _catalog.Cases[0].Tasks[0].Solutions[0].Copy() }; round.Pause(DateTimeOffset.UtcNow);
        var saved = await store.SaveDesignExamAsync(round, _catalog); Assert.NotNull(saved.FinishedUtc); Assert.Empty(saved.Answers); Assert.Equal(0, saved.Earned);
    }
    private ProgressStore Store() => new(new EnvironmentStub(), new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Training:ProgressDirectory"] = _directory }).Build());
    public void Dispose() { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
    private sealed class EnvironmentStub : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests"; public string EnvironmentName { get; set; } = "Tests"; public string ContentRootPath { get; set; } = ""; public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider(); public string WebRootPath { get; set; } = ""; public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
