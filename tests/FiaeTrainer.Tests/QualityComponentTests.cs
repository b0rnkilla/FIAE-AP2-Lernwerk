using System.Net;
using System.Reflection;
using System.Text.Json;
using FiaeTrainer.Core.QualityAssurance.Content;
using FiaeTrainer.Core.QualityAssurance.Evaluation;
using FiaeTrainer.Core.QualityAssurance.Models;
using FiaeTrainer.Core.QualityAssurance.Practice;
using FiaeTrainer.Web.Components.Pages;
using FiaeTrainer.Web.Components.Pages.Training;
using FiaeTrainer.Web.Components.QualityAssurance;
using FiaeTrainer.Web.Learning;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft die Lernoberfläche, gesperrte Abgaben und das Lesen ohne Profilschreibzugriff. </summary>
public sealed class QualityComponentTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-QualityUi", Guid.NewGuid().ToString("N"));
    private readonly QualityCatalog _catalog = new();

    [Theory]
    [InlineData(1)] [InlineData(2)] [InlineData(3)] [InlineData(4)] [InlineData(13)] [InlineData(39)]
    public async Task EveryKindRendersWithoutCreatingProgressOrPrematureFeedback(int id)
    {
        using var services = Services(); var task = _catalog.Lessons[id - 1];
        string html = await RenderAsync<QualityTraining>(services, new() { ["Id"] = id });
        Assert.Contains(task.Title, html); Assert.Contains("Ohne Hilfe lösen", html); Assert.Contains("Geführt lernen", html);
        Assert.DoesNotContain("Begründung", html);
        Assert.DoesNotContain(task.Discovery, html); Assert.DoesNotContain(task.Explanation, html);
        Assert.DoesNotContain("Was dein Testsatz zeigt", html);
        if (id > 1) Assert.Contains("Vorheriger Prüfschritt noch offen", html);
        Assert.False(File.Exists(Path.Combine(_directory, "progress.json")));
    }

    [Fact]
    public async Task OneLearningOutcomeHasNoPointsNoPrematureDiscoveryAndOneHonestProbe()
    {
        using var services = Services(); var task = _catalog.Lessons[12];
        var answer = task.Solutions[0].Copy(); answer.TestCases[0].Expected = "true";
        string wrong = await RenderAsync<QualityLearningFeedback>(services, new() { ["Task"] = task, ["Result"] = QualityEvaluator.Evaluate(task, answer) });
        Assert.DoesNotContain(task.Explanation, wrong); Assert.DoesNotContain(task.Discovery, wrong);
        var result = QualityEvaluator.Evaluate(task, task.Solutions[0]);
        string right = await RenderAsync<QualityLearningFeedback>(services, new() { ["Task"] = task, ["Result"] = result });
        Assert.Contains(task.Discovery, right);
        foreach (string html in new[] { wrong, right })
        {
            Assert.Single(System.Text.RegularExpressions.Regex.Matches(html, "aria-label=\"Rückmeldung zu deiner Lösung\""));
            Assert.DoesNotContain("Trainingspunkte", html); Assert.DoesNotContain("50 / 50", html); Assert.DoesNotContain("IHK", html);
        }
        string probe = await RenderAsync<QualityTestResults>(services, new() { ["Task"] = task, ["Probe"] = result.Probe });
        Assert.Contains("Sollwert stimmt", probe); Assert.Contains("2 / 2", probe); Assert.Contains("quality-code-covered", probe);
    }

    [Fact]
    public async Task StoryReportStopsAtCurrentTaskAndKeepsOpenRanges()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        foreach (int id in new[] { 1, 4, 9 })
        {
            var task = _catalog.Lessons[id - 1];
            await store.SaveQualityAsync(id, new() { Answer = task.Kind == "Choice" ? new() { SelectedIds = task.CorrectIds.ToList() } : task.Solutions[0].Copy() }, _catalog, true);
        }
        string html = await RenderAsync<QualityTraining>(services, new() { ["Id"] = 4 });
        Assert.Contains("Aufgaben 2–3", html); Assert.Contains(_catalog.Lessons[3].Discovery, html); Assert.DoesNotContain(_catalog.Lessons[8].Discovery, html);
    }

    [Fact]
    public async Task ExamEntryOnlyReadsPauseHidesAndFinalSubmissionUnlocksResultsOnly()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        Assert.Contains("Prüfung starten", await RenderAsync<QualityExamHome>(services)); Assert.False(File.Exists(Path.Combine(_directory, "progress.json")));
        var round = new QualityExamRound { CaseId = "paketstation" }; await store.StartQualityExamAsync(round, _catalog);
        string active = await RenderAsync<QualityExam>(services); Assert.DoesNotContain("Was dein Testsatz zeigt", active); Assert.DoesNotContain("Musterlösung mit Erklärung", active);
        round.Pause(DateTimeOffset.UtcNow); round = await store.SaveQualityExamAsync(round, _catalog);
        string paused = await RenderAsync<QualityExam>(services); Assert.Contains("Eine Pause für den Kopf", paused); Assert.DoesNotContain("Testfall hinzufügen", paused);
        round.RunningSinceUtc = DateTimeOffset.UtcNow;
        foreach (var task in _catalog.Cases[0].Tasks) round.Answers[task.Id] = task.Solutions[0].Copy();
        await store.SaveQualityExamAsync(round, _catalog, true);
        string finished = await RenderAsync<QualityExam>(services); Assert.Contains("50 / 50 Punkte", finished); Assert.Contains("Was dein Testsatz zeigt", finished); Assert.Contains("Musterlösung mit Erklärung", finished); Assert.Contains("readonly", finished); Assert.DoesNotContain("Jetzt abgeben", finished);
    }

    [Fact]
    public async Task ReadOnlyHandlersCannotChangeAnyStructuredAnswer()
    {
        foreach (var task in _catalog.Lessons.GroupBy(task => task.Kind).Select(group => group.First()))
        {
            var answer = task.Solutions.FirstOrDefault()?.Copy() ?? new(); var row = answer.TestCases.FirstOrDefault() ?? new QualityTestCase();
#pragma warning disable BL0005
            var editor = new QualityEditor { Task = task, Answer = answer, ReadOnly = true };
#pragma warning restore BL0005
            string before = JsonSerializer.Serialize(answer);
            foreach (var (name, args) in new (string, object[])[] { ("ToggleAsync", ["1", true]), ("ValueAsync", ["x", "1"]), ("AddStepAsync", ["x"]), ("RemoveStepAsync", [0]), ("MoveAsync", [0, 1]), ("AddCaseAsync", []), ("RemoveCaseAsync", [row]), ("MoveCaseAsync", [0, 1]), ("CaseAsync", [row, "people", "7"]), ("ExpectedAsync", [row, "12"]) })
                await (Task)typeof(QualityEditor).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(editor, args)!;
            Assert.Equal(before, JsonSerializer.Serialize(answer));
        }
    }

    [Fact]
    public async Task AutosaveRetainsTestRowIdentityAcrossImmediatelyFollowingInputEvents()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        var round = new QualityExamRound { CaseId = "paketstation" }; var answer = _catalog.Cases[0].Tasks[0].Solutions[0].Copy(); round.Answers[1] = answer;
        await store.StartQualityExamAsync(round, _catalog);
        using var page = new QualityExam(); var type = typeof(QualityExam); const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
        type.GetProperty("Store", flags)!.SetValue(page, store); type.GetProperty("Catalog", flags)!.SetValue(page, _catalog); type.GetField("_round", flags)!.SetValue(page, round);
        var row = answer.TestCases[0]; await (Task)type.GetMethod("SaveAsync", flags)!.Invoke(page, [])!;
        Assert.Same(round, type.GetField("_round", flags)!.GetValue(page)); Assert.Same(row, round.Answers[1].TestCases[0]);
        row.Expected = "777"; await (Task)type.GetMethod("SaveAsync", flags)!.Invoke(page, [])!;
        Assert.Equal("777", (await store.LoadAsync()).QualityExam!.Answers[1].TestCases[0].Expected);
    }

    [Fact]
    public async Task RootResumesOwnLastLessonAndOnlyConfirmedStoryIsShown()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        await store.SaveQualityAsync(1, new() { Answer = new() { SelectedIds = _catalog.Lessons[0].CorrectIds.ToList() } }, _catalog, true);
        await store.SaveQualityAsync(2, new() { Answer = new() { Reasoning = "Eigener Ansatz" } }, _catalog);
        string before = await File.ReadAllTextAsync(Path.Combine(_directory, "progress.json"));
        string html = await RenderAsync<QualityTraining>(services);
        Assert.Contains(_catalog.Lessons[1].Title, html); Assert.Contains(_catalog.Lessons[1].PreviousContext, html);
        Assert.Contains(_catalog.Lessons[0].Discovery, html); Assert.DoesNotContain(_catalog.Lessons[1].Discovery, html);
        Assert.Equal(before, await File.ReadAllTextAsync(Path.Combine(_directory, "progress.json")));
        Assert.DoesNotContain("Eigener Ansatz", html);
        var legacy = (await store.LoadAsync()).QualityLessons[2];
        Assert.Equal("Eigener Ansatz", legacy.Answer.Reasoning);
        var page = new QualityTraining(); const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
        typeof(QualityTraining).GetField("_task", flags)!.SetValue(page, _catalog.Lessons[1]);
        typeof(QualityTraining).GetField("_progress", flags)!.SetValue(page, legacy);
        Assert.False((bool)typeof(QualityTraining).GetProperty("HasOwnDraft", flags)!.GetValue(page)!);
    }

    [Fact]
    public async Task ReferencePausesAndMarksExamAndKeepsAValidatedLocalReturn()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        await store.StartQualityExamAsync(new() { CaseId = "paketstation" }, _catalog);
        Assert.Equal("/qualitaet/pruefung/runde", await ReadReferenceAsync(store, true, null));
        var round = (await store.LoadAsync()).QualityExam!;
        Assert.Null(round.RunningSinceUtc); Assert.True(round.UsedReference);
        await store.SaveQualityAsync(35, new() { Answer = _catalog.Lessons[34].InitialAnswer() }, _catalog);
        Assert.Equal("/qualitaet/35", await ReadReferenceAsync(store, false, 999));
        Assert.True((await store.LoadAsync()).QualityLessons[35].Assisted);
    }

    /// <summary> Abfrageparameter werden im echten Browser vom Router gesetzt; hier wird dessen fertig gebundener Zustand geprüft. </summary>
    private async Task<string> ReadReferenceAsync(ProgressStore store, bool exam, int? lessonId)
    {
#pragma warning disable BL0005
        var reference = new QualityReference { Exam = exam, LessonId = lessonId };
#pragma warning restore BL0005
        var type = typeof(QualityReference); const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
        type.GetProperty("Store", flags)!.SetValue(reference, store); type.GetProperty("Catalog", flags)!.SetValue(reference, _catalog);
        await (Task)type.GetMethod("OnInitializedAsync", flags)!.Invoke(reference, [])!;
        return (string)type.GetField("ReturnPath", flags)!.GetValue(reference)!;
    }

    private ServiceProvider Services()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Training:ProgressDirectory"] = _directory }).Build();
        return new ServiceCollection().AddLogging().AddSingleton(_catalog).AddSingleton(new ProgressStore(new EnvironmentStub(), config)).AddSingleton<NavigationManager>(new NavigationStub()).BuildServiceProvider();
    }
    private static async Task<string> RenderAsync<T>(ServiceProvider services, Dictionary<string, object?>? parameters = null) where T : IComponent
    {
        await using HtmlRenderer renderer = new(services, services.GetRequiredService<ILoggerFactory>());
        return await renderer.Dispatcher.InvokeAsync(async () => WebUtility.HtmlDecode((await renderer.RenderComponentAsync<T>(ParameterView.FromDictionary(parameters ?? []))).ToHtmlString()));
    }
    public void Dispose() { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
    private sealed class NavigationStub : NavigationManager { public NavigationStub() => Initialize("http://localhost/", "http://localhost/qualitaet"); protected override void NavigateToCore(string uri, bool forceLoad) => throw new NotSupportedException(); }
    private sealed class EnvironmentStub : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests"; public string EnvironmentName { get; set; } = "Tests"; public string ContentRootPath { get; set; } = ""; public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider(); public string WebRootPath { get; set; } = ""; public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
