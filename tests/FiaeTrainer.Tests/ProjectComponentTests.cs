using System.Net;
using System.Reflection;
using System.Text.Json;
using FiaeTrainer.Core.ProjectManagement.Content;
using FiaeTrainer.Core.ProjectManagement.Evaluation;
using FiaeTrainer.Core.ProjectManagement.Models;
using FiaeTrainer.Core.ProjectManagement.Practice;
using FiaeTrainer.Web.Components.Pages;
using FiaeTrainer.Web.Components.Pages.Training;
using FiaeTrainer.Web.Components.ProjectManagement;
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

/// <summary> Eigene Pläne ohne vorzeitige Antworten, unveränderliche Vorlagen und ehrliche Lernrückmeldung. </summary>
public sealed class ProjectComponentTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-ProjectUi", Guid.NewGuid().ToString("N"));
    private readonly ProjectCatalog _catalog = new();
    private const BindingFlags FLAGS = BindingFlags.NonPublic | BindingFlags.Instance;

    [Theory]
    [InlineData(1)] [InlineData(2)] [InlineData(6)] [InlineData(13)] [InlineData(20)] [InlineData(27)] [InlineData(43)]
    public async Task EveryKindRendersWithoutWritingProgressOrShowingSolutions(int id)
    {
        using var services = Services(); var task = _catalog.Lessons[id - 1];
        string html = await RenderAsync<ProjectTraining>(services, new() { ["Id"] = id });
        Assert.Contains(task.Title, html); Assert.Contains("Ohne Hilfe lösen", html); Assert.Contains("Geführt lernen", html);
        Assert.DoesNotContain("Deine zusätzliche Begründung", html); Assert.DoesNotContain(task.Discovery, html); Assert.DoesNotContain(task.Explanation, html);
        if (id > 1) Assert.Contains("Vorheriger Planungsschritt noch offen", html);
        Assert.False(File.Exists(Path.Combine(_directory, "progress.json")));
    }

    [Fact]
    public async Task PreviewDoesNotCalculateEmptyNetworkCellsOrChangeAnswers()
    {
        using var services = Services(); var task = _catalog.Lessons[19]; var answer = task.InitialAnswer(); string before = JsonSerializer.Serialize(answer);
        string html = await RenderAsync<ProjectPlanView>(services, new() { ["Task"] = task, ["Answer"] = answer });
        Assert.Contains("FAZ: –", html); Assert.DoesNotContain("FAZ: 2", html); Assert.Contains("Plan als Text lesen", html);
        Assert.Equal(before, JsonSerializer.Serialize(answer));
        answer.Activities[0].Values["es"] = "99";
        html = await RenderAsync<ProjectPlanView>(services, new() { ["Task"] = task, ["Answer"] = answer }); Assert.Contains("FAZ: 99", html);
        answer.Dependencies.Add(new() { SourceId = answer.Activities[^1].Id, TargetId = answer.Activities[0].Id });
        Assert.Contains("Netzplan", await RenderAsync<ProjectPlanView>(services, new() { ["Task"] = task, ["Answer"] = answer }));
    }

    [Fact]
    public async Task LearningFeedbackIsOneOutcomeWithoutTrainingPointsOrPrematureDiscovery()
    {
        using var services = Services(); var task = _catalog.Lessons[30];
        var wrong = ProjectEvaluator.Evaluate(task, new() { Values = new() { [task.Fields[0].Key] = "1" } });
        string html = await RenderAsync<ProjectLearningFeedback>(services, new() { ["Task"] = task, ["Result"] = wrong });
        Assert.DoesNotContain(task.Discovery, html); Assert.DoesNotContain(task.Explanation, html);
        html = await RenderAsync<ProjectLearningFeedback>(services, new() { ["Task"] = task, ["Result"] = ProjectEvaluator.Evaluate(task, task.Solutions[0]) });
        Assert.Contains(task.Discovery, html); Assert.DoesNotContain("Trainingspunkte", html); Assert.DoesNotContain("IHK", html);
        Assert.Single(System.Text.RegularExpressions.Regex.Matches(html, "aria-label=\"Rückmeldung zu deiner Lösung\""));
    }

    [Fact]
    public async Task ReportShowsOnlyConfirmedPastAndCurrentStepsWithOpenGaps()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        foreach (int id in new[] { 1, 4, 9 })
        {
            var task = _catalog.Lessons[id - 1];
            await store.SaveProjectAsync(id, new() { Answer = task.Kind == "Choice" ? new() { SelectedIds = task.CorrectIds.ToList() } : task.Solutions[0].Copy() }, _catalog, true);
        }
        string html = await RenderAsync<ProjectTraining>(services, new() { ["Id"] = 4 });
        Assert.Contains("Aufgaben 2–3", html); Assert.Contains(_catalog.Lessons[3].Discovery, html); Assert.DoesNotContain(_catalog.Lessons[8].Discovery, html);
    }

    [Fact]
    public async Task ExamHomeOnlyReadsPauseHidesAndSubmissionLocksAllEditors()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        Assert.Contains("Prüfung starten", await RenderAsync<ProjectExamHome>(services)); Assert.False(File.Exists(Path.Combine(_directory, "progress.json")));
        var round = new ProjectExamRound { CaseId = "bibliothek" }; await store.StartProjectExamAsync(round, _catalog);
        string active = await RenderAsync<ProjectExam>(services); Assert.DoesNotContain("Musterlösung mit Erklärung", active);
        round.Pause(DateTimeOffset.UtcNow); round = await store.SaveProjectExamAsync(round, _catalog);
        string paused = await RenderAsync<ProjectExam>(services); Assert.Contains("Eine Pause für den Kopf", paused); Assert.DoesNotContain("Element hinzufügen", paused);
        round.RunningSinceUtc = DateTimeOffset.UtcNow;
        foreach (var task in _catalog.Cases[0].Tasks) round.Answers[task.Id] = task.Kind is "Choice" or "Review" ? new() { SelectedIds = task.CorrectIds.ToList() } : task.Solutions[0].Copy();
        await store.SaveProjectExamAsync(round, _catalog, true);
        string finished = await RenderAsync<ProjectExam>(services); Assert.Contains("50 / 50 Punkte", finished); Assert.Contains("Musterlösung mit Erklärung", finished); Assert.Contains("readonly", finished); Assert.DoesNotContain("Element hinzufügen", finished);
    }

    [Theory]
    [InlineData("bibliothek")] [InlineData("sportverein")] [InlineData("museum")]
    public async Task EachExamPartShowsItsOwnAssumptionsBeforeAnswering(string caseId)
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        var round = new ProjectExamRound { CaseId = caseId }; await store.StartProjectExamAsync(round, _catalog);
        for (int index = 0; index < 3; index++)
        {
            round.Index = index; round = await store.SaveProjectExamAsync(round, _catalog);
            string html = await RenderAsync<ProjectExam>(services);
            Assert.Contains(_catalog.Cases.Single(item => item.Id == caseId).Tasks[index].Story, html);
        }
    }

    [Fact]
    public async Task LockedPlanHandlersCannotMutateNodesOrDependencies()
    {
        var task = _catalog.Lessons[23]; var answer = task.Solutions[0].Copy(); string before = JsonSerializer.Serialize(answer);
        var editor = new ProjectPlanEditor();
        typeof(ProjectPlanEditor).GetProperty("Task")!.SetValue(editor, task); typeof(ProjectPlanEditor).GetProperty("Answer")!.SetValue(editor, answer); typeof(ProjectPlanEditor).GetProperty("ReadOnly")!.SetValue(editor, true);
        foreach (var invocation in new (string Name, object?[] Arguments)[] { ("AddAsync", []), ("AddLinkAsync", []), ("RemoveAsync", [answer.Activities[0]]), ("EditAsync", [answer.Activities[0], "duration", "99"]), ("EditAsync", [answer.Activities[0], "critical", false]), ("LinkAsync", [answer.Dependencies[0], true, answer.Activities[^1].Id]), ("RemoveLinkAsync", [answer.Dependencies[0]]) })
            await (Task)typeof(ProjectPlanEditor).GetMethod(invocation.Name, FLAGS)!.Invoke(editor, invocation.Arguments)!;
        Assert.Equal(before, JsonSerializer.Serialize(answer));
    }

    [Fact]
    public async Task ExamAutosaveKeepsNodeAndLinkIdentityBetweenInputEvents()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        var round = new ProjectExamRound { CaseId = "bibliothek" }; var answer = _catalog.Cases[0].Tasks[0].Solutions[0].Copy(); round.Answers[1] = answer; await store.StartProjectExamAsync(round, _catalog);
        using var page = new ProjectExam(); var type = typeof(ProjectExam);
        type.GetProperty("Store", FLAGS)!.SetValue(page, store); type.GetProperty("Catalog", FLAGS)!.SetValue(page, _catalog); type.GetField("_round", FLAGS)!.SetValue(page, round);
        var node = answer.Activities[0]; var link = answer.Dependencies[0];
        await (Task)type.GetMethod("SaveAsync", FLAGS)!.Invoke(page, [])!;
        Assert.Same(node, round.Answers[1].Activities[0]); Assert.Same(link, round.Answers[1].Dependencies[0]);
        node.Duration = "17"; link.TargetId = "";
        await (Task)type.GetMethod("SaveAsync", FLAGS)!.Invoke(page, [])!;
        var saved = (await store.LoadAsync()).ProjectExam!; Assert.Equal("17", saved.Answers[1].Activities[0].Duration); Assert.Equal("", saved.Answers[1].Dependencies[0].TargetId);
    }

    [Fact]
    public async Task ReferenceUsesValidatedReturnAndPausesExamWithAssistance()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        await store.StartProjectExamAsync(new() { CaseId = "bibliothek" }, _catalog);
        var page = new ProjectReference(); typeof(ProjectReference).GetProperty("Exam")!.SetValue(page, true);
        typeof(ProjectReference).GetProperty("Store", FLAGS)!.SetValue(page, store); typeof(ProjectReference).GetProperty("Catalog", FLAGS)!.SetValue(page, _catalog);
        await (Task)typeof(ProjectReference).GetMethod("OnInitializedAsync", FLAGS)!.Invoke(page, [])!;
        var round = (await store.LoadAsync()).ProjectExam!; Assert.Null(round.RunningSinceUtc); Assert.True(round.UsedReference);
        Assert.Equal("/projekte/pruefung/runde", typeof(ProjectReference).GetField("ReturnPath", FLAGS)!.GetValue(page));
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
    private sealed class NavigationStub : NavigationManager { public NavigationStub() => Initialize("http://localhost/", "http://localhost/projekte"); protected override void NavigateToCore(string uri, bool forceLoad) => throw new NotSupportedException(); }
    private sealed class EnvironmentStub : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests"; public string EnvironmentName { get; set; } = "Tests"; public string ContentRootPath { get; set; } = ""; public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider(); public string WebRootPath { get; set; } = ""; public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
