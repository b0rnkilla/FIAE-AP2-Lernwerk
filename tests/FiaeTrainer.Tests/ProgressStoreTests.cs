using FiaeTrainer.Web.Learning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft Fortschrittsspeicherung in ausschließlich für den Test erzeugten temporären Verzeichnissen. </summary>
public sealed class ProgressStoreTests : IDisposable
{
    /// <summary> Eindeutiges, ausschließlich vom Test verwaltetes Verzeichnis. </summary>
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-Tests", Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task DraftHintsAndAchievementsSurviveReload()
    {
        ProgressStore store = CreateStore();
        await store.SaveLessonAsync(3, new LessonProgress { Sql = "SELECT Id FROM fall.Sendungen", HintLevel = 2, SolutionViewed = true, Completed = true, Independent = false, Attempts = 4 });
        TrainingProgress loaded = await CreateStore().LoadAsync();
        Assert.Equal(3, loaded.LastLessonId);
        Assert.Equal("SELECT Id FROM fall.Sendungen", loaded.Lessons[3].Sql);
        Assert.Equal(2, loaded.Lessons[3].HintLevel);
        Assert.True(loaded.Lessons[3].SolutionViewed);
        Assert.True(loaded.Lessons[3].Completed);
        Assert.False(loaded.Lessons[3].Independent);
        Assert.Equal(4, loaded.Lessons[3].Attempts);
    }

    [Fact]
    public async Task ConcurrentLessonsAndAchievementsArePreserved()
    {
        ProgressStore store = CreateStore();
        await Task.WhenAll(store.SaveLessonAsync(1, new LessonProgress { Completed = true, Independent = true }), store.SaveLessonAsync(2, new LessonProgress { Sql = "Entwurf" }));
        await store.SaveLessonAsync(1, new LessonProgress());
        TrainingProgress loaded = await store.LoadAsync();
        Assert.Equal(2, loaded.Lessons.Count);
        Assert.True(loaded.Lessons[1].Completed);
        Assert.True(loaded.Lessons[1].Independent);
        Assert.Equal("Entwurf", loaded.Lessons[2].Sql);
    }

    [Fact]
    public async Task CorruptProgressIsNotSilentlyOverwritten()
    {
        string path = Path.Combine(_directory, "Data", "progress", "progress.json");
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException());
        await File.WriteAllTextAsync(path, "Defekte Testdatei");
        ProgressStore store = CreateStore();
        await Assert.ThrowsAsync<System.Text.Json.JsonException>(() => store.LoadAsync());
        await Assert.ThrowsAsync<System.Text.Json.JsonException>(() => store.SaveLessonAsync(1, new LessonProgress()));
        Assert.Equal("Defekte Testdatei", await File.ReadAllTextAsync(path));
    }

    [Fact]
    public async Task ResetCreatesAnEmptyProfile()
    {
        ProgressStore store = CreateStore();
        await store.SaveLessonAsync(1, new LessonProgress { Completed = true });
        await store.ResetAsync();
        TrainingProgress loaded = await store.LoadAsync();
        Assert.Empty(loaded.Lessons);
        Assert.Equal(1, loaded.LastLessonId);
    }

    [Theory]
    [InlineData("null")]
    [InlineData("{\"Lessons\":null}")]
    [InlineData("{\"Lessons\":{\"1\":null}}")]
    [InlineData("{\"Lessons\":{\"1\":{\"HintLevel\":8}}}")]
    public async Task StructurallyInvalidJsonIsPreservedAndReported(string json)
    {
        string path = Path.Combine(_directory, "Data", "progress", "progress.json");
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException());
        await File.WriteAllTextAsync(path, json);
        await Assert.ThrowsAsync<InvalidDataException>(() => CreateStore().LoadAsync());
        await Assert.ThrowsAsync<InvalidDataException>(() => CreateStore().SaveLessonAsync(1, new LessonProgress()));
        Assert.Equal(json, await File.ReadAllTextAsync(path));
    }

    [Fact]
    public async Task WorkshopDoesNotOverwriteInvestigationOrOldProfileFiles()
    {
        string path = Path.Combine(_directory, "Data", "progress", "progress.json");
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException());
        await File.WriteAllTextAsync(path, "{\"LastLessonId\":2,\"Lessons\":{\"2\":{\"Sql\":\"Alter Entwurf\",\"Completed\":true}}}");
        ProgressStore store = CreateStore();
        await store.SaveLessonAsync(2, new LessonProgress { Sql = "UPDATE Artikel", Completed = true }, LearningArea.Workshop);
        TrainingProgress loaded = await store.LoadAsync();
        Assert.Equal("Alter Entwurf", loaded.Lessons[2].Sql);
        Assert.True(loaded.Lessons[2].Completed);
        Assert.Equal("UPDATE Artikel", loaded.WorkshopLessons[2].Sql);
    }

    private ProgressStore CreateStore() => new(new TestEnvironment { ContentRootPath = _directory });

    [Theory]
    [InlineData(LearningArea.Investigation)]
    [InlineData(LearningArea.Workshop)]
    public async Task EachTaskKeepsItsOwnModeAndAchievementsAfterReload(LearningArea area)
    {
        ProgressStore store = CreateStore();
        await store.SaveLessonAsync(1, new LessonProgress { ExamMode = true, Completed = true, Independent = true, AttemptAssisted = false, Sql = "Eigener Entwurf" }, area);
        await store.SaveLessonAsync(2, new LessonProgress { ExamMode = false, AttemptAssisted = true, Sql = "Geführter Entwurf" }, area);
        TrainingProgress loaded = await CreateStore().LoadAsync();
        Assert.True(loaded.GetLessons(area)[1].ExamMode);
        Assert.True(loaded.GetLessons(area)[1].Independent);
        Assert.Equal("Eigener Entwurf", loaded.GetLessons(area)[1].Sql);
        Assert.False(loaded.GetLessons(area)[2].ExamMode);
        Assert.True(loaded.GetLessons(area)[2].UsedAssistance);
        Assert.True(new LessonProgress().ExamMode);
    }

    [Fact]
    public async Task OldProfileDefaultsToIndependentModeWithoutChangingItsHistory()
    {
        string path = Path.Combine(_directory, "Data", "progress", "progress.json");
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException());
        await File.WriteAllTextAsync(path, "{\"Lessons\":{\"1\":{\"Sql\":\"Alter Entwurf\",\"Completed\":true,\"HintLevel\":2}}}");
        TrainingProgress loaded = await CreateStore().LoadAsync();
        Assert.True(loaded.Lessons[1].ExamMode);
        Assert.True(loaded.Lessons[1].Completed);
        Assert.Equal(2, loaded.Lessons[1].HintLevel);
        Assert.True(loaded.Lessons[1].UsedAssistance);
        Assert.False(loaded.SelfTestHelpDismissed);
    }

    [Fact]
    public async Task HelpPreferenceAndConcurrentTaskUpdatesPreserveEachOther()
    {
        ProgressStore store = CreateStore();
        await Task.WhenAll(
            store.SetSelfTestHelpDismissedAsync(true),
            store.SaveLessonAsync(1, new LessonProgress { ExamMode = false, Sql = "Ermittlungsentwurf" }),
            store.SaveLessonAsync(1, new LessonProgress { ExamMode = true, Completed = true }, LearningArea.Workshop));
        TrainingProgress loaded = await CreateStore().LoadAsync();
        Assert.True(loaded.SelfTestHelpDismissed);
        Assert.False(loaded.Lessons[1].ExamMode);
        Assert.Equal("Ermittlungsentwurf", loaded.Lessons[1].Sql);
        Assert.True(loaded.WorkshopLessons[1].Completed);
    }

    [Fact]
    public async Task RestoringHelpPreservesDraftAndSolvedTask()
    {
        ProgressStore store = CreateStore();
        await store.SaveLessonAsync(1, new LessonProgress { Sql = "Entwurf", ExamMode = false, Completed = true });
        await store.SetSelfTestHelpDismissedAsync(true);
        await store.SetSelfTestHelpDismissedAsync(false);
        TrainingProgress loaded = await CreateStore().LoadAsync();
        Assert.False(loaded.SelfTestHelpDismissed);
        Assert.Equal("Entwurf", loaded.Lessons[1].Sql);
        Assert.False(loaded.Lessons[1].ExamMode);
        Assert.True(loaded.Lessons[1].Completed);
    }

    [Fact]
    public async Task HelpPreferenceDoesNotOverwriteACorruptProfile()
    {
        string path = Path.Combine(_directory, "Data", "progress", "progress.json");
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException());
        await File.WriteAllTextAsync(path, "Defekte Testdatei");
        await Assert.ThrowsAsync<System.Text.Json.JsonException>(() => CreateStore().SetSelfTestHelpDismissedAsync(true));
        Assert.Equal("Defekte Testdatei", await File.ReadAllTextAsync(path));
    }

    [Theory]
    [InlineData(LearningArea.Investigation)]
    [InlineData(LearningArea.Workshop)]
    public async Task IndependentDraftRemainsIndependentAfterReload(LearningArea area)
    {
        ProgressStore store = CreateStore();
        LessonProgress attempt = new();
        attempt.StartIndependentAttempt();
        attempt.Sql = "SELECT Id FROM Sendungen";
        await store.SaveLessonAsync(1, attempt, area);
        LessonProgress loaded = (await CreateStore().LoadAsync()).GetLessons(area)[1];
        Assert.Equal(attempt.Sql, loaded.Sql);
        Assert.False(loaded.UsedAssistance);
    }

    [Fact]
    public async Task AssistedDraftCannotBecomeIndependentByReloading()
    {
        ProgressStore store = CreateStore();
        await store.SaveLessonAsync(1, new LessonProgress { Sql = "SELECT * FROM Sendungen", AttemptAssisted = true });
        LessonProgress loaded = (await CreateStore().LoadAsync()).Lessons[1];
        Assert.True(loaded.UsedAssistance);
        Assert.Equal("SELECT * FROM Sendungen", loaded.Sql);
    }

    [Fact]
    public void FreshIndependentAttemptPreservesAchievementsButClearsHelpAndDraft()
    {
        LessonProgress progress = new() { Sql = "Musterlösung", Completed = true, Independent = true, HintLevel = 3, SolutionViewed = true, Attempts = 4, AttemptAssisted = true };
        progress.StartIndependentAttempt();
        Assert.Equal("", progress.Sql);
        Assert.Equal(0, progress.HintLevel);
        Assert.False(progress.SolutionViewed);
        Assert.False(progress.UsedAssistance);
        Assert.True(progress.Completed);
        Assert.True(progress.Independent);
        Assert.Equal(4, progress.Attempts);
        Assert.True(progress.ExamMode);
    }

    [Fact]
    public void LegacyDraftIsConservativelyAssistedWhileAnEmptyNewAttemptIsNot()
    {
        LessonProgress? legacy = System.Text.Json.JsonSerializer.Deserialize<LessonProgress>("{\"Sql\":\"Alter Entwurf\"}");
        Assert.NotNull(legacy);
        Assert.True(legacy.UsedAssistance);
        Assert.False(new LessonProgress().UsedAssistance);
        Assert.True(new LessonProgress { HintLevel = 1 }.UsedAssistance);
        Assert.True(new LessonProgress { SolutionViewed = true }.UsedAssistance);
    }

    [Fact]
    public async Task CollapsedTaskListSurvivesReloadAndConcurrentDraftAndHelpUpdates()
    {
        ProgressStore store = CreateStore();
        await Task.WhenAll(
            store.SetTaskListCollapsedAsync(true),
            store.SetSelfTestHelpDismissedAsync(true),
            store.SaveLessonAsync(4, new LessonProgress { Sql = "SELECT Id FROM Sendungen;", ExamMode = false, Completed = true }));
        TrainingProgress loaded = await CreateStore().LoadAsync();
        Assert.True(loaded.TaskListCollapsed);
        Assert.True(loaded.SelfTestHelpDismissed);
        Assert.Equal("SELECT Id FROM Sendungen;", loaded.Lessons[4].Sql);
        Assert.False(loaded.Lessons[4].ExamMode);
        Assert.True(loaded.Lessons[4].Completed);
        await store.SetTaskListCollapsedAsync(false);
        Assert.False((await CreateStore().LoadAsync()).TaskListCollapsed);
    }

    /// <summary> Entfernt ausschließlich das bei diesem Test neu angelegte GUID-Verzeichnis. </summary>
    public void Dispose()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, true);
    }

    /// <summary> Minimale Hostumgebung für Dateispeicherung ohne gestarteten Webserver. </summary>
    private sealed class TestEnvironment : IWebHostEnvironment
    {
        /// <summary> Nicht verwendetes Webverzeichnis. </summary>
        public string WebRootPath { get; set; } = "";
        /// <summary> Nicht verwendete Webressourcen. </summary>
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        /// <summary> Isoliertes Testverzeichnis. </summary>
        public string ContentRootPath { get; set; } = "";
        /// <summary> Nicht verwendete Dateiresourcen. </summary>
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        /// <summary> Name der Testumgebung. </summary>
        public string EnvironmentName { get; set; } = "Testing";
        /// <summary> Name der Testanwendung. </summary>
        public string ApplicationName { get; set; } = "FiaeTrainer.Tests";
    }
}
