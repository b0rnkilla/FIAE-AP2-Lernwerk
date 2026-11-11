using FiaeTrainer.Core.QualityAssurance.Content;
using FiaeTrainer.Core.QualityAssurance.Evaluation;
using FiaeTrainer.Core.QualityAssurance.Models;
using FiaeTrainer.Core.QualityAssurance.Practice;
using FiaeTrainer.Web.Learning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Unabhängige Sollwerte, alternative Testsätze, tatsächliche Abdeckung und geschützte Prüfungsprofile. </summary>
public sealed class QualityAssuranceTests : IDisposable
{
    private readonly QualityCatalog _catalog = new();
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-QualityTests", Guid.NewGuid().ToString("N"));

    [Fact]
    public void CatalogHasNineChaptersAndEveryOwnReferencePasses()
    {
        Assert.Equal(54, _catalog.Lessons.Count); Assert.Equal(9, _catalog.Lessons.GroupBy(task => task.Chapter).Count());
        Assert.All(_catalog.Lessons.GroupBy(task => task.Chapter), chapter => Assert.Equal(6, chapter.Count()));
        Assert.Equal(6, _catalog.Lessons.Select(task => task.Kind).Distinct().Count()); Assert.Equal(3, _catalog.Cases.Count);
        Assert.Equal(new[] { 27, 33 }, _catalog.Lessons.Where(task => task.Format == "Repair").Select(task => task.Id));
        foreach (var task in _catalog.Lessons.Concat(_catalog.Cases.SelectMany(item => item.Tasks)))
        {
            Assert.False(QualityEvaluator.Evaluate(task, new()).Correct);
            foreach (var answer in task.Kind is "Choice" or "Review" ? [Reference(task)] : task.Solutions)
                Assert.True(QualityEvaluator.Evaluate(task, answer).Correct, task.Title);
            if (task.Format == "Repair") Assert.False(QualityEvaluator.Evaluate(task, task.InitialAnswer()).Correct);
        }
    }

    [Theory]
    [InlineData("capacity", "people=0", "false")]
    [InlineData("capacity", "people=1", "true")]
    [InlineData("capacity", "people=24", "true")]
    [InlineData("capacity", "people=25", "false")]
    [InlineData("fee", "people=9;member=true", "10800")]
    [InlineData("fee", "people=10;member=true", "9600")]
    [InlineData("fee", "people=11;member=true", "10560")]
    [InlineData("fee", "people=10;member=false", "12000")]
    [InlineData("fee", "people=-1;member=false", "-1")]
    [InlineData("rental", "days=1;damaged=false", "7")]
    [InlineData("rental", "days=14;damaged=true", "118")]
    [InlineData("rental", "days=15;damaged=true", "-1")]
    [InlineData("shipping", "weight=1000;express=false", "4")]
    [InlineData("shipping", "weight=1001;express=true", "11")]
    [InlineData("shipping", "weight=5000;express=false", "8")]
    [InlineData("shipping", "weight=5001;express=true", "-1")]
    [InlineData("access", "role=guest;own=true", "false")]
    [InlineData("access", "role=member;own=true", "true")]
    [InlineData("access", "role=member;own=false", "false")]
    [InlineData("access", "role=admin;own=false", "true")]
    [InlineData("attempt", "failed=2;correct=true", "allowed")]
    [InlineData("attempt", "failed=2;correct=false", "rejected")]
    [InlineData("attempt", "failed=3;correct=true", "locked")]
    [InlineData("cancel", "state=requested;action=confirm", "confirmed")]
    [InlineData("cancel", "state=confirmed;action=confirm", "rejected")]
    [InlineData("cancel", "state=cancelled;action=cancel", "rejected")]
    [InlineData("cancel", "state=confirmed;action=cancel", "cancelled")]
    [InlineData("availability", "capacity=9;booked=0", "9")]
    [InlineData("availability", "capacity=9;booked=9", "0")]
    [InlineData("availability", "capacity=9;booked=4", "5")]
    [InlineData("availability", "capacity=9;booked=11", "-1")]
    public void ModelsUseIndependentlyCalculatedOutputs(string key, string data, string expected)
    {
        var program = QualityPrograms.Find(key)!; var execution = QualitySimulator.Run(program, Inputs(data))!;
        Assert.Equal(expected, execution.Output);
        Assert.Subset(program.Statements.Keys.ToHashSet(), execution.Statements);
        Assert.Subset(program.Branches.Keys.ToHashSet(), execution.Branches);
        Assert.Contains(execution.Path, program.Paths.Keys);
    }

    [Fact]
    public void OwnRepresentativesPassWithoutMatchingReferenceRows()
    {
        var task = _catalog.Lessons[12];
        var own = new QualityAnswer { TestCases = [Row("false", "people=-7"), Row("WAHR", "people=19"), Row("FALSCH", "people=37")] };
        Assert.True(QualityEvaluator.Evaluate(task, own).Correct);
        own.TestCases.RemoveAt(2); Assert.False(QualityEvaluator.Evaluate(task, own).Correct);
        own.TestCases.Add(Row("FALSCH", "people=37")); own.TestCases.Add(Row("true", "people=3"));
        Assert.True(QualityEvaluator.Evaluate(task, own).Correct);
    }

    [Fact]
    public void WrongOracleDoesNotDetectFaultButStillExecutesItsActualPath()
    {
        var task = _catalog.Lessons[13]; var answer = Reference(task);
        answer.TestCases.Single(row => row.Inputs["people"] == "0").Expected = "true";
        var result = QualityEvaluator.Evaluate(task, answer);
        Assert.False(result.Correct); Assert.Equal(2, result.Probe!.Statements.Count); Assert.Equal(2, result.Probe.Branches.Count);
        Assert.DoesNotContain("accept-zero", result.Probe.Rows.SelectMany(row => row.DetectedFaults));
        Assert.Contains(result.Checks, check => check.Name == "Gezielte Fehlervarianten" && check.Earned < check.Maximum);
    }

    [Fact]
    public void BranchAndPathCoverageHaveIndependentDenominators()
    {
        var program = QualityPrograms.Find("shipping")!;
        var answer = new QualityAnswer { TestCases = [Row("-1", "weight=0;express=false"), Row("4", "weight=400;express=false"), Row("11", "weight=1600;express=true")] };
        var task = _catalog.Lessons[19]; var result = QualityEvaluator.Evaluate(task, answer);
        Assert.True(result.Correct); Assert.Equal(5, result.Probe!.Statements.Count); Assert.Equal(6, result.Probe.Branches.Count); Assert.Equal(3, result.Probe.Paths.Count); Assert.Equal(5, program.Paths.Count);
        Assert.False(QualityEvaluator.Evaluate(_catalog.Lessons[20], answer).Correct);
        answer.TestCases.Add(Row("7", "weight=400;express=true")); answer.TestCases.Add(Row("8", "weight=1600;express=false"));
        Assert.True(QualityEvaluator.Evaluate(_catalog.Lessons[20], answer).Correct);
    }

    [Fact]
    public void InvalidInputsCannotExecuteOrInflateCoverageAndEachRowIsIsolated()
    {
        var program = QualityPrograms.Find("fee")!;
        foreach (string input in new[] { "people=1.5;member=true", "people=31;member=true", "people=12;member=maybe", "people=12", "people=12;member=true;extra=1" }) Assert.Null(QualitySimulator.Run(program, Inputs(input)));
        Assert.Null(QualitySimulator.Run(program, Inputs("people=12;member=true"), "unknown"));
        var first = QualitySimulator.Run(program, Inputs("people=10;member=true"))!; first.Statements.Clear();
        var second = QualitySimulator.Run(program, Inputs("people=4;member=false"))!;
        Assert.Equal("4800", second.Output); Assert.DoesNotContain("S3", second.Statements); Assert.Equal("D1.F|D2.F", second.Path);
        var damaged = Reference(_catalog.Lessons[14]); damaged.TestCases[0].Inputs.Clear();
        Assert.False(QualityEvaluator.Evaluate(_catalog.Lessons[14], damaged).Correct);
    }

    [Fact]
    public void CoincidentalFaultOutputNeedsAnotherRepresentative()
    {
        var program = QualityPrograms.Find("availability")!;
        Assert.Equal("-1", QualitySimulator.Run(program, Inputs("capacity=5;booked=6"), "overbooked")!.Output);
        Assert.Equal("-2", QualitySimulator.Run(program, Inputs("capacity=5;booked=7"), "overbooked")!.Output);
        Assert.Equal("-1", QualitySimulator.Run(program, Inputs("capacity=5;booked=7"))!.Output);
    }

    [Fact]
    public void MatricesOrdersReviewsAndIndependentAssertionsRejectConcreteMistakes()
    {
        foreach (var task in _catalog.Lessons.Where(task => task.Kind is "Choice" or "Review"))
        {
            var answer = Reference(task); answer.SelectedIds.Add("unknown"); Assert.False(QualityEvaluator.Evaluate(task, answer).Correct);
            answer = Reference(task); answer.SelectedIds.Add(answer.SelectedIds[0]); Assert.False(QualityEvaluator.Evaluate(task, answer).Correct);
        }
        var matrix = _catalog.Lessons[38]; var own = Reference(matrix); own.Values["guest.read"] = "allow"; Assert.False(QualityEvaluator.Evaluate(matrix, own).Correct);
        var order = _catalog.Lessons[52]; own = Reference(order); own.Order.Reverse(); Assert.False(QualityEvaluator.Evaluate(order, own).Correct);
        var assertion = _catalog.Lessons[26]; own = Reference(assertion); own.Values[assertion.Fields[0].Key] = "ist"; Assert.False(QualityEvaluator.Evaluate(assertion, own).Correct);
        Assert.True(QualityEvaluator.Evaluate(_catalog.Lessons[1], new() { Values = Reference(_catalog.Lessons[1]).Values, Reasoning = "Völlig eigene Formulierung" }).Correct);
        Assert.False(QualityRules.HasAnswer(new() { Reasoning = "Nur ein Text" }));
    }

    [Fact]
    public void IncompleteDraftsAreSavedButBrokenOrOversizeDataIsRejected()
    {
        var draft = new QualityAnswer { TestCases = [new()] }; Assert.True(QualityRules.Storable(draft)); Assert.False(QualityRules.HasAnswer(draft));
        draft.TestCases.Add(draft.TestCases[0]); Assert.False(QualityRules.Storable(draft));
        Assert.False(QualityRules.Storable(new QualityAnswer { TestCases = null! }));
        Assert.False(QualityRules.Storable(new QualityAnswer { Reasoning = new string('x', 4001) }));
        var task = _catalog.Lessons[13]; draft = Reference(task); while (draft.TestCases.Count <= task.MaximumCases) draft.TestCases.Add(Row("true", "people=4"));
        Assert.False(QualityEvaluator.Evaluate(task, draft).Correct);
        draft = Reference(task); var copy = draft.Copy(); copy.TestCases[0].Expected = "other"; Assert.NotEqual(copy.TestCases[0].Expected, draft.TestCases[0].Expected);
    }

    [Fact]
    public async Task LegacyModulesAndRevisionProtectedIndependentAchievementsSurvive()
    {
        Directory.CreateDirectory(_directory);
        await File.WriteAllTextAsync(Path.Combine(_directory, "progress.json"), """
            {"LastLessonId":17,"Lessons":{"17":{"Sql":"SELECT 1;","Completed":true}},
             "WorkshopLessons":{"4":{"Sql":"SELECT 2;","Attempts":1}},
             "LastAlgorithmLessonId":8,"AlgorithmLessons":{"8":{"Draft":"AUSGABE 123","Completed":true}},
             "BusinessQuestions":{"7":{"Attempts":2,"Completed":true,"Independent":true}},
             "DesignLessons":{"37":{"ContentVersion":"2026-11-06","Completed":true}},
             "UmlLessons":{"12":{"ContentVersion":"2026-11-08","Completed":true}},
             "SoftwareLessons":{"32":{"ContentVersion":"2026-11-09","Completed":true}}}
            """);
        var store = Store(); var task = _catalog.Lessons[12];
        var progress = new QualityProgress { Answer = Reference(task), Completed = true, Independent = true, Attempts = 99 };
        await store.SaveQualityAsync(task.Id, progress, _catalog); Assert.False(progress.Completed); Assert.False(progress.Independent); Assert.Equal(0, progress.Attempts);
        Assert.True((await store.SaveQualityAsync(task.Id, progress, _catalog, true))!.Correct);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveQualityAsync(task.Id, new(), _catalog));
        progress.Answer = new() { Reasoning = "Noch unfertig" }; await store.SaveQualityAsync(task.Id, progress, _catalog);
        var state = await Store().LoadAsync(); Assert.True(state.QualityLessons[task.Id].Independent); Assert.Equal(1, state.QualityLessons[task.Id].Attempts);
        Assert.Equal("Noch unfertig", state.QualityLessons[task.Id].Answer.Reasoning); Assert.Equal("SELECT 1;", state.Lessons[17].Sql); Assert.Equal("SELECT 2;", state.WorkshopLessons[4].Sql);
        Assert.Equal("AUSGABE 123", state.AlgorithmLessons[8].Draft); Assert.True(state.BusinessQuestions[7].Independent); Assert.True(state.DesignLessons[37].Completed); Assert.True(state.UmlLessons[12].Completed); Assert.True(state.SoftwareLessons[32].Completed);
        progress.ContentVersion = "alt"; await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveQualityAsync(task.Id, progress, _catalog));
    }

    [Theory]
    [InlineData("paketstation")] [InlineData("werkzeugverleih")] [InlineData("terminportal")]
    public async Task ExamsScoreFiftyAndFinalAnswersAndHistoryStayImmutable(string caseId)
    {
        var store = Store(); var round = new QualityExamRound { CaseId = caseId }; await store.StartQualityExamAsync(round, _catalog);
        foreach (var task in _catalog.Cases.Single(item => item.Id == caseId).Tasks) round.Answers[task.Id] = Reference(task);
        round = await store.SaveQualityExamAsync(round, _catalog, true); Assert.Equal(50, round.Earned); Assert.Single((await Store().LoadAsync()).QualityHistory);
        round.Answers.Clear(); await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveQualityExamAsync(round, _catalog, true));
        Assert.Equal(3, (await Store().LoadAsync()).QualityExam!.Answers.Count); Assert.Single((await Store().LoadAsync()).QualityHistory);
    }

    [Fact]
    public async Task PausedRestartStaleWindowAndLateAnswersRespectSavedUtcClock()
    {
        var store = Store(); var round = new QualityExamRound { CaseId = "paketstation", RunningSinceUtc = DateTimeOffset.UtcNow.AddSeconds(-80) }; await store.StartQualityExamAsync(round, _catalog);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.StartQualityExamAsync(new() { CaseId = "terminportal" }, _catalog));
        round.Pause(DateTimeOffset.UtcNow); round.FlaggedIds.Add(2); round.UsedReference = true; round = await store.SaveQualityExamAsync(round, _catalog);
        Assert.InRange(round.RemainingSeconds, 2618, 2620); Assert.Null((await Store().LoadAsync()).QualityExam!.RunningSinceUtc);
        var stale = round.Copy(); round.RemainingSeconds = 2700; round.RunningSinceUtc = DateTimeOffset.UtcNow; round.UsedReference = false; round = await store.SaveQualityExamAsync(round, _catalog);
        Assert.InRange(round.SecondsLeft(DateTimeOffset.UtcNow), 2617, 2620); Assert.True(round.UsedReference); Assert.Contains(2, round.FlaggedIds);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveQualityExamAsync(stale, _catalog));
        var expired = new QualityExamRound { CaseId = "terminportal", RunningSinceUtc = DateTimeOffset.UtcNow.AddMinutes(-46) }; await store.StartQualityExamAsync(expired, _catalog, round.Id);
        expired.Answers[1] = Reference(_catalog.Cases[2].Tasks[0]); expired.Pause(DateTimeOffset.UtcNow); var submitted = await store.SaveQualityExamAsync(expired, _catalog);
        Assert.NotNull(submitted.FinishedUtc); Assert.Empty(submitted.Answers); Assert.Equal(0, submitted.Earned);
    }

    private static Dictionary<string, string> Inputs(string data) => data.Split(';').Select(value => value.Split('=')).ToDictionary(pair => pair[0], pair => pair[1]);
    private static QualityTestCase Row(string expected, string data) => new() { Expected = expected, Inputs = Inputs(data) };
    private static QualityAnswer Reference(QualityAssignment task) => task.Kind is "Choice" or "Review" ? new() { SelectedIds = task.CorrectIds.ToList() } : task.Solutions[0].Copy();
    private ProgressStore Store() => new(new EnvironmentStub(), new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Training:ProgressDirectory"] = _directory }).Build());
    public void Dispose() { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
    private sealed class EnvironmentStub : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests"; public string EnvironmentName { get; set; } = "Tests"; public string ContentRootPath { get; set; } = ""; public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider(); public string WebRootPath { get; set; } = ""; public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
