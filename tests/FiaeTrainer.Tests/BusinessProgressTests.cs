using System.Text.Json;
using FiaeTrainer.Core.BusinessStudies.Content;
using FiaeTrainer.Core.BusinessStudies.Evaluation;
using FiaeTrainer.Core.BusinessStudies.Practice;
using FiaeTrainer.Web.Learning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft Rundenablauf, Zeitablauf, Profilkompatibilität und konkurrierende Fenster mit eigenen temporären Profilen. </summary>
public sealed class BusinessProgressTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-Wiso", Guid.NewGuid().ToString("N"));
    private readonly BusinessCatalog _catalog = new();

    [Fact]
    public async Task OldProfileAndOtherModuleDraftsSurviveWisoRoundAndFurtherSqlWrites()
    {
        Directory.CreateDirectory(_directory);
        await File.WriteAllTextAsync(Path.Combine(_directory, "progress.json"), """
            {"LastLessonId":17,"Lessons":{"17":{"Sql":"SQL-Entwurf","Completed":true,"Independent":true}},"AlgorithmLessons":{"4":{"Draft":"WENN erledigt DANN","Completed":true}},"TaskListCollapsed":true}
            """);
        ProgressStore store = Store();
        BusinessRound round = Round(2, 24);
        await store.StartBusinessRoundAsync(round);
        round.Answers[2] = new() { SelectedIds = ["a"] };
        await store.SaveBusinessRoundAsync(round, _catalog, 2);
        await store.SaveLessonAsync(3, new() { Sql = "Werkstatt-Entwurf" }, LearningArea.Workshop);
        await store.SaveAlgorithmAsync(8, new() { Draft = "AUSGABE 7" });
        TrainingProgress restored = await Store().LoadAsync();
        Assert.Equal("SQL-Entwurf", restored.Lessons[17].Sql);
        Assert.True(restored.Lessons[17].Independent);
        Assert.Equal("WENN erledigt DANN", restored.AlgorithmLessons[4].Draft);
        Assert.True(restored.TaskListCollapsed);
        Assert.True(restored.BusinessQuestions[2].Completed);
        Assert.Equal("a", Assert.Single(restored.BusinessRound!.Answers[2].SelectedIds));
    }

    [Fact]
    public async Task RecheckingAndFinishingDoesNotCountTheSameAnswerTwice()
    {
        ProgressStore store = Store();
        BusinessRound round = Round(2, 24);
        await store.StartBusinessRoundAsync(round);
        round.Answers[2] = new() { SelectedIds = ["a"] };
        await store.SaveBusinessRoundAsync(round, _catalog, 2);
        round.Answers[2] = new() { SelectedIds = ["b"] };
        await store.SaveBusinessRoundAsync(round, _catalog, 2);
        Assert.Equal("a", Assert.Single(round.Answers[2].SelectedIds));
        await store.SaveBusinessRoundAsync(round, _catalog, finish: true);
        TrainingProgress result = await store.LoadAsync();
        Assert.Equal(1, result.BusinessQuestions[2].Attempts);
        Assert.Equal(1, result.BusinessQuestions[24].Attempts);
        Assert.True(result.BusinessQuestions[24].NeedsReview);
        Assert.Equal(1, result.BusinessRound!.Earned);
        Assert.Single(result.BusinessHistory);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveBusinessRoundAsync(round, _catalog, finish: true));
        Assert.Single((await store.LoadAsync()).BusinessHistory);
    }

    [Fact]
    public async Task ExamDoesNotExposeChecksBeforeSubmissionAndPartialAnswersKeepTheirPoints()
    {
        ProgressStore store = Store();
        BusinessRound round = Round(1, 3);
        round.Kind = BusinessRoundKind.Exam;
        round.RunningSinceUtc = DateTimeOffset.UtcNow;
        await store.StartBusinessRoundAsync(round);
        round.Answers[1] = new() { Values = new() { ["f1"] = "1" } };
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveBusinessRoundAsync(round, _catalog, 1));
        Assert.Empty((await store.LoadAsync()).BusinessQuestions);
        await store.SaveBusinessRoundAsync(round, _catalog, finish: true);
        TrainingProgress state = await store.LoadAsync();
        Assert.Equal(1, state.BusinessRound!.Earned);
        Assert.Equal(3, state.BusinessRound.Maximum);
        Assert.False(state.BusinessQuestions[1].Completed);
        Assert.True(state.BusinessQuestions[1].NeedsReview);
    }

    [Fact]
    public async Task ReloadPreservesPositionFlagsAnswersAndClockPause()
    {
        ProgressStore store = Store();
        BusinessRound round = Round(2, 24);
        round.Kind = BusinessRoundKind.Exam;
        round.RunningSinceUtc = DateTimeOffset.UtcNow.AddMinutes(-10);
        round.Pause(DateTimeOffset.UtcNow);
        await store.StartBusinessRoundAsync(round);
        round.Index = 1;
        round.Answers[24] = new() { Values = new() { ["f1"] = "30.04.2026" }, Unsure = true };
        round.FlaggedIds.Add(24);
        await store.SaveBusinessRoundAsync(round, _catalog);
        BusinessRound restored = (await Store().LoadAsync()).BusinessRound!;
        Assert.Equal(1, restored.Index);
        Assert.Contains(24, restored.FlaggedIds);
        Assert.Equal("30.04.2026", restored.Answers[24].Values["f1"]);
        Assert.True(restored.Answers[24].Unsure);
        Assert.True(restored.Paused);
        Assert.InRange(restored.RemainingSeconds, 2999, 3000);
    }

    [Fact]
    public async Task ExpiredExamIsSubmittedEvenIfClientAttemptsToPauseIt()
    {
        ProgressStore store = Store();
        BusinessRound round = Round(2);
        round.Kind = BusinessRoundKind.Exam;
        round.RunningSinceUtc = DateTimeOffset.UtcNow.AddSeconds(-3601);
        await store.StartBusinessRoundAsync(round);
        round.Pause(DateTimeOffset.UtcNow);
        await store.SaveBusinessRoundAsync(round, _catalog);
        TrainingProgress state = await store.LoadAsync();
        Assert.NotNull(state.BusinessRound!.FinishedUtc);
        Assert.Equal(0, state.BusinessRound.RemainingSeconds);
        Assert.Equal(0, state.BusinessRound.Earned);
        Assert.True(state.BusinessQuestions[2].NeedsReview);
    }

    [Fact]
    public async Task StaleBrowserAndUnconfirmedReplacementCannotOverwriteNewerAnswers()
    {
        ProgressStore store = Store();
        BusinessRound round = Round(2);
        await store.StartBusinessRoundAsync(round);
        BusinessRound stale = (await store.LoadAsync()).BusinessRound!;
        round.Answers[2] = new() { SelectedIds = ["a"] };
        await store.SaveBusinessRoundAsync(round, _catalog);
        stale.Answers[2] = new() { SelectedIds = ["b"] };
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveBusinessRoundAsync(stale, _catalog));
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.StartBusinessRoundAsync(Round(3)));
        Assert.Equal("a", Assert.Single((await store.LoadAsync()).BusinessRound!.Answers[2].SelectedIds));
        await store.StartBusinessRoundAsync(Round(3), round.Id);
        Assert.Equal(3, Assert.Single((await store.LoadAsync()).BusinessRound!.QuestionIds));
    }

    [Fact]
    public async Task ANewErrorKeepsOldAchievementButSchedulesReviewAndUnsureCorrectAlsoNeedsReview()
    {
        ProgressStore store = Store();
        BusinessRound first = Round(2);
        first.Answers[2] = new() { SelectedIds = ["a"] };
        await store.StartBusinessRoundAsync(first);
        await store.SaveBusinessRoundAsync(first, _catalog, finish: true);
        BusinessRound second = Round(2, 4);
        second.Answers[2] = new() { SelectedIds = ["b"] };
        second.Answers[4] = new() { SelectedIds = ["a"], Unsure = true };
        await store.StartBusinessRoundAsync(second);
        await store.SaveBusinessRoundAsync(second, _catalog, finish: true);
        TrainingProgress state = await store.LoadAsync();
        Assert.True(state.BusinessQuestions[2].Completed);
        Assert.True(state.BusinessQuestions[2].Independent);
        Assert.True(state.BusinessQuestions[2].NeedsReview);
        Assert.Equal(2, state.BusinessQuestions[2].Attempts);
        Assert.True(state.BusinessQuestions[4].Completed);
        Assert.False(state.BusinessQuestions[4].Independent);
        Assert.True(state.BusinessQuestions[4].NeedsReview);
    }

    [Fact]
    public async Task LookingUpReferenceDoesNotBecomeIndependentSuccess()
    {
        ProgressStore store = Store();
        BusinessRound round = Round(2);
        round.UsedReference = true;
        round.Answers[2] = new() { SelectedIds = ["a"] };
        await store.StartBusinessRoundAsync(round);
        await store.SaveBusinessRoundAsync(round, _catalog, finish: true);
        BusinessProgress progress = (await store.LoadAsync()).BusinessQuestions[2];
        Assert.True(progress.Completed);
        Assert.False(progress.Independent);
        Assert.True(progress.NeedsReview);
    }

    [Fact]
    public async Task HistoryRetainsOnlyTwentyResultsWithoutRemovingAchievements()
    {
        ProgressStore store = Store();
        for (int i = 0; i < 23; i++)
        {
            BusinessRound round = Round(2);
            round.Answers[2] = new() { SelectedIds = ["a"] };
            await store.StartBusinessRoundAsync(round);
            await store.SaveBusinessRoundAsync(round, _catalog, finish: true);
        }
        TrainingProgress state = await store.LoadAsync();
        Assert.Equal(20, state.BusinessHistory.Count);
        Assert.Equal(23, state.BusinessQuestions[2].Attempts);
    }

    [Theory]
    [InlineData("{\"BusinessQuestions\":null}")]
    [InlineData("{\"BusinessQuestions\":{\"2\":{\"Independent\":true}}}")]
    [InlineData("{\"BusinessRound\":{\"QuestionIds\":[2,2]}}")]
    [InlineData("{\"BusinessRound\":{\"QuestionIds\":[2],\"Answers\":{\"2\":{\"Values\":null}}}}")]
    public async Task BrokenProfileIsNeverOverwrittenByAnotherModule(string damaged)
    {
        Directory.CreateDirectory(_directory);
        string path = Path.Combine(_directory, "progress.json");
        await File.WriteAllTextAsync(path, damaged);
        await Assert.ThrowsAsync<InvalidDataException>(() => Store().SaveAlgorithmAsync(1, new()));
        Assert.Equal(damaged, await File.ReadAllTextAsync(path));
    }

    [Fact]
    public void NavigationRetainsWisoOnOverviewAndLearningPlanAndKeepsSqlReturnPath()
    {
        LearningNavigation navigation = new();
        navigation.Visit("sql-werkstatt/7");
        navigation.Visit("wiso/runde");
        Assert.Equal(LearningModule.BusinessStudies, navigation.Module);
        navigation.Visit("lernplan");
        navigation.Visit("");
        Assert.Equal(LearningModule.BusinessStudies, navigation.Module);
        Assert.Equal("/sql-werkstatt/7", navigation.SqlTaskPath);
        navigation.Visit("algorithmen/2");
        Assert.Equal(LearningModule.Algorithms, navigation.Module);
        navigation.Visit("wiso-falsch");
        Assert.Equal(LearningModule.Algorithms, navigation.Module);
    }

    private static BusinessRound Round(params int[] ids) => new() { Kind = BusinessRoundKind.Mixed, QuestionIds = ids.ToList() };
    private ProgressStore Store() => new(new TestEnvironment(), new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Training:ProgressDirectory"] = _directory }).Build());
    /// <summary> Entfernt ausschließlich das für diesen Test erzeugte Profil. </summary>
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
