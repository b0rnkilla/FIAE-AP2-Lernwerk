using FiaeTrainer.Core.Algorithms.Exams;
using FiaeTrainer.Core.BusinessStudies.Practice;
using FiaeTrainer.Core.Exams;
using FiaeTrainer.Core.Sql.Exams;
using FiaeTrainer.Web.Learning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüfungsfristen, Revisionen und alte Lernstände werden mit ausschließlich eigenen Testprofilen geprüft. </summary>
public sealed class ExamProgressTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-Exams", Guid.NewGuid().ToString("N"));
    private ProgressStore Store() => new(new TestEnvironment(), new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Training:ProgressDirectory"] = _directory }).Build());

    [Fact]
    public async Task OldLearningProfilesAndBothExamModulesSurviveIndependentWrites()
    {
        ProgressStore store = Store();
        await store.SaveLessonAsync(17, new() { Sql = "SELECT Id FROM Sendungen;", Completed = true, Independent = true });
        await store.SaveAlgorithmAsync(4, new() { Draft = "AUSGABE 1" });
        await store.StartBusinessRoundAsync(new() { Kind = BusinessRoundKind.Mixed, QuestionIds = [2] });
        var algorithms = new AlgorithmExamCatalog().CreateRound();
        var sql = new SqlExamCatalog().CreateRound("service");
        await store.StartExamAsync(algorithms);
        await store.StartExamAsync(sql);
        algorithms.Drafts[algorithms.TaskIds[0]] = "Eigener Prüfungsentwurf";
        await store.SaveExamAsync(algorithms);
        var restored = await Store().LoadAsync();
        Assert.Equal(2, restored.ExamRounds.Count);
        Assert.True(restored.Lessons[17].Independent);
        Assert.Equal("AUSGABE 1", restored.AlgorithmLessons[4].Draft);
        Assert.Equal(2, restored.BusinessRound!.QuestionIds[0]);
        Assert.Equal("Eigener Prüfungsentwurf", restored.ExamRounds[ExamModule.Algorithms].Drafts[algorithms.TaskIds[0]]);
    }

    [Fact]
    public async Task ExpiredRoundCannotBeRescuedByLatePauseOrLateDraft()
    {
        var round = new AlgorithmExamCatalog().CreateRound();
        round.RunningSinceUtc = DateTimeOffset.UtcNow.AddSeconds(-2710);
        round.Drafts[round.TaskIds[0]] = "Vor Frist gespeichert";
        await Store().StartExamAsync(round);
        round.Drafts[round.TaskIds[0]] = "Nach Frist verändert";
        round.Pause(DateTimeOffset.UtcNow);
        round = await Store().SaveExamAsync(round);
        Assert.True(round.Submitted);
        Assert.Equal(0, round.SecondsLeft(DateTimeOffset.UtcNow));
        Assert.Equal("Vor Frist gespeichert", round.Drafts[round.TaskIds[0]]);
        Assert.Null(round.EvaluatedUtc);
        Assert.Empty(round.Results);
    }

    [Fact]
    public async Task ExplicitPauseAndReferenceRemainAcrossReloadWithoutTimeGain()
    {
        var round = new AlgorithmExamCatalog().CreateRound();
        round.RunningSinceUtc = DateTimeOffset.UtcNow.AddSeconds(-120);
        await Store().StartExamAsync(round);
        var paused = (await Store().MarkExamReferenceAsync(ExamModule.Algorithms))!;
        Assert.True(paused.Paused);
        Assert.True(paused.UsedReference);
        Assert.InRange(paused.RemainingSeconds, 2578, 2580);
        var restored = (await Store().LoadAsync()).ExamRounds[ExamModule.Algorithms];
        Assert.Equal(paused.RemainingSeconds, restored.SecondsLeft(DateTimeOffset.UtcNow.AddHours(1)));
        restored.RunningSinceUtc = DateTimeOffset.UtcNow;
        restored = await Store().SaveExamAsync(restored);
        Assert.True(restored.UsedReference);
        Assert.False(restored.Paused);
        Assert.InRange(restored.SecondsLeft(DateTimeOffset.UtcNow), 2577, 2580);
    }

    [Fact]
    public async Task StaleWindowsAndUnconfirmedReplacementAreRejected()
    {
        var round = new AlgorithmExamCatalog().CreateRound();
        var store = Store();
        await store.StartExamAsync(round);
        var secondWindow = (await Store().LoadAsync()).ExamRounds[round.Module];
        await store.SaveExamAsync(round);
        await Assert.ThrowsAsync<InvalidOperationException>(() => Store().SaveExamAsync(secondWindow));
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.StartExamAsync(new AlgorithmExamCatalog().CreateRound()));
        await store.StartExamAsync(new AlgorithmExamCatalog().CreateRound(), round.Id);
        Assert.NotEqual(round.Id, (await store.LoadAsync()).ExamRounds[round.Module].Id);
    }

    [Fact]
    public async Task SubmissionStaysReadOnlyAndCannotBeCountedTwice()
    {
        var store = Store();
        var round = new AlgorithmExamCatalog().CreateRound();
        await store.StartExamAsync(round);
        round.Drafts[round.TaskIds[0]] = "Abgabe";
        round = await store.SaveExamAsync(round, true);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveExamAsync(round));
        List<ExamTaskResult> results = round.TaskIds.Select((id, index) => new ExamTaskResult { TaskId = id, Checks = [new("Prüffall", index == 0 ? 10 : 0, index < 2 ? 20 : 10, "Unabhängiger Sollwert")] }).ToList();
        var completed = await store.CompleteExamAsync(round, results);
        Assert.Equal(10, completed.Earned);
        Assert.Single((await store.LoadAsync()).ExamHistory);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.CompleteExamAsync(completed, results));
        Assert.Equal("Abgabe", (await store.LoadAsync()).ExamRounds[round.Module].Drafts[round.TaskIds[0]]);
    }

    /// <summary> Löscht nur den für diese Testinstanz erzeugten temporären Profilordner. </summary>
    public void Dispose() { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
    private sealed class TestEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests";
        public string EnvironmentName { get; set; } = "Tests";
        public string ContentRootPath { get; set; } = "";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = "";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
