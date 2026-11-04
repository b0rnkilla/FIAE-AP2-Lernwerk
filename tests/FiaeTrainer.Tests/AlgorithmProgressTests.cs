using FiaeTrainer.Web.Learning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft die Erweiterung älterer Profile, Modultrennung und den Aufgabenmodus. </summary>
public sealed class AlgorithmProgressTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-Algorithms", Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task ExistingSqlProfileKeepsItsDataWhenAlgorithmsAreAdded()
    {
        Directory.CreateDirectory(_directory);
        await File.WriteAllTextAsync(Path.Combine(_directory, "progress.json"), """
            {"LastLessonId": 17, "Lessons": {"17": {"Sql": "SELECT * FROM Sendungen;", "Completed": true, "Independent": true}}, "WorkshopLessons": {}}
            """);
        ProgressStore store = Store();
        await store.SaveAlgorithmAsync(1, new() { Draft = "AUSGABE 42", ExamMode = false, AttemptAssisted = true });
        await store.SaveAlgorithmAsync(2, new() { Draft = "650\n350\n400", Completed = true, Independent = true });
        TrainingProgress progress = await store.LoadAsync();
        Assert.Equal(17, progress.LastLessonId);
        Assert.Equal("SELECT * FROM Sendungen;", progress.Lessons[17].Sql);
        Assert.True(progress.Lessons[17].Independent);
        Assert.False(progress.AlgorithmLessons[1].ExamMode);
        Assert.True(progress.AlgorithmLessons[2].ExamMode);
        Assert.Equal(2, progress.LastAlgorithmLessonId);
        await store.SaveLessonAsync(2, new() { Sql = "SQL-Entwurf" }, LearningArea.Workshop);
        Assert.Equal("AUSGABE 42", (await store.LoadAsync()).AlgorithmLessons[1].Draft);
    }

    [Fact]
    public async Task SavingDifferentAreasRetainsAchievementsAndSettings()
    {
        ProgressStore store = Store();
        await store.SaveAlgorithmAsync(1, new() { Completed = true, Independent = true });
        await store.SetTaskListCollapsedAsync(true);
        await store.SetSelfTestHelpDismissedAsync(true);
        await store.SaveAlgorithmAsync(1, new() { Draft = "neuer Entwurf" });
        TrainingProgress progress = await store.LoadAsync();
        Assert.True(progress.AlgorithmLessons[1].Independent);
        Assert.True(progress.AlgorithmLessons[1].Completed);
        Assert.True(progress.TaskListCollapsed);
        Assert.True(progress.SelfTestHelpDismissed);
    }

    [Fact]
    public void IndependentRestartPreservesAchievementsAndRemovesAssistance()
    {
        AlgorithmProgress progress = new() { Completed = true, Independent = true, ExamMode = false, AttemptAssisted = true, SolutionViewed = true, HintLevel = 3, Attempts = 4 };
        progress.StartIndependentAttempt("vorgegebener Fehlercode");
        Assert.True(progress.Completed);
        Assert.True(progress.Independent);
        Assert.True(progress.ExamMode);
        Assert.False(progress.AttemptAssisted);
        Assert.False(progress.SolutionViewed);
        Assert.Equal(0, progress.HintLevel);
        Assert.Equal(4, progress.Attempts);
        Assert.Equal("vorgegebener Fehlercode", progress.Draft);
    }

    [Fact]
    public async Task InvalidAlgorithmProfileIsNotOverwritten()
    {
        Directory.CreateDirectory(_directory);
        string path = Path.Combine(_directory, "progress.json");
        const string damaged = "{\"AlgorithmLessons\":{\"1\":{\"Independent\":true,\"Completed\":false}}}";
        await File.WriteAllTextAsync(path, damaged);
        await Assert.ThrowsAsync<InvalidDataException>(() => Store().SaveAlgorithmAsync(2, new()));
        Assert.Equal(damaged, await File.ReadAllTextAsync(path));
    }

    [Fact]
    public void NavigationKeepsContextOnGeneralPagesAndChangesWithTheLearningArea()
    {
        LearningNavigation navigation = new();
        Assert.False(navigation.Algorithms);
        navigation.Visit("algorithmen/3"); Assert.True(navigation.Algorithms);
        navigation.Visit("lernplan"); Assert.True(navigation.Algorithms);
        navigation.Visit(""); Assert.True(navigation.Algorithms);
        navigation.Visit("sql-werkstatt/2"); Assert.False(navigation.Algorithms);
        navigation.Visit("algorithmen/spickzettel"); Assert.True(navigation.Algorithms);
        navigation.Visit("nachschlagen"); Assert.False(navigation.Algorithms);
        navigation.Visit("algorithmen-falsch"); Assert.False(navigation.Algorithms);
    }

    private ProgressStore Store() => new(new TestEnvironment(), new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Training:ProgressDirectory"] = _directory }).Build());
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
