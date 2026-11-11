using System.Net;
using System.Reflection;
using System.Text.Json;
using FiaeTrainer.Core.SoftwareDesign.Content;
using FiaeTrainer.Core.SoftwareDesign.Evaluation;
using FiaeTrainer.Core.SoftwareDesign.Models;
using FiaeTrainer.Core.SoftwareDesign.Practice;
using FiaeTrainer.Web.Components.Pages;
using FiaeTrainer.Web.Components.Pages.Training;
using FiaeTrainer.Web.Components.SoftwareDesign;
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
public sealed class SoftwareComponentTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-SoftwareUi", Guid.NewGuid().ToString("N"));
    private readonly SoftwareCatalog _catalog = new();

    [Theory]
    [InlineData(1)] [InlineData(2)] [InlineData(10)] [InlineData(26)] [InlineData(32)] [InlineData(38)]
    public async Task EveryAnswerKindRendersWithoutCreatingProgressOrShowingSolution(int id)
    {
        using var services = Services(); var task = _catalog.Lessons[id - 1];
        string html = await RenderAsync<SoftwareTraining>(services, new() { ["Id"] = id });
        Assert.Contains(task.Title, html); Assert.Contains("Ohne Hilfe lösen", html); Assert.Contains("Geführt lernen", html);
        Assert.DoesNotContain("Begründung", html);
        Assert.DoesNotContain(task.Discovery, html); Assert.DoesNotContain(task.Explanation, html);
        if (id > 1) Assert.Contains("Vorheriger Planungsschritt noch offen", html);
        if (task.Kind is "Architecture" or "Form") Assert.Contains("noch leer", html);
        if (task.Kind == "Diagram") Assert.Contains("Deine Arbeitsfläche ist noch leer", html);
        Assert.False(File.Exists(Path.Combine(_directory, "progress.json")));
    }

    [Fact]
    public async Task FeedbackHasOneOutcomeNoPracticePointsAndNoPrematureExplanation()
    {
        using var services = Services(); var task = _catalog.Lessons[31];
        string wrong = await RenderAsync<SoftwareLearningFeedback>(services, new() { ["Task"] = task, ["Result"] = SoftwareEvaluator.Evaluate(task, task.InitialAnswer()) });
        Assert.DoesNotContain(task.Explanation, wrong); Assert.DoesNotContain(task.Discovery, wrong);
        string right = await RenderAsync<SoftwareLearningFeedback>(services, new() { ["Task"] = task, ["Result"] = SoftwareEvaluator.Evaluate(task, task.Solutions[0]) });
        Assert.Contains(task.Discovery, right); Assert.DoesNotContain("Begründung", right);
        foreach (var html in new[] { wrong, right })
        {
            Assert.Single(System.Text.RegularExpressions.Regex.Matches(html, "aria-label=\"Rückmeldung zu deiner Lösung\""));
            Assert.DoesNotContain("Trainingspunkte", html); Assert.DoesNotContain("50 / 50", html); Assert.DoesNotContain("IHK", html);
        }
    }

    [Fact]
    public async Task RootResumesOwnLastLessonAndOnlyConfirmedStoryIsShown()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        await store.SaveSoftwareAsync(1, new() { Answer = new() { SelectedIds = _catalog.Lessons[0].CorrectIds.ToList() } }, _catalog, true);
        await store.SaveSoftwareAsync(2, new() { Answer = new() { Reasoning = "Eigener Ansatz" } }, _catalog);
        string before = await File.ReadAllTextAsync(Path.Combine(_directory, "progress.json"));
        string html = await RenderAsync<SoftwareTraining>(services);
        Assert.Contains(_catalog.Lessons[1].Title, html); Assert.Contains(_catalog.Lessons[1].PreviousContext, html);
        Assert.Contains(_catalog.Lessons[0].Discovery, html); Assert.DoesNotContain(_catalog.Lessons[1].Discovery, html);
        Assert.Equal(before, await File.ReadAllTextAsync(Path.Combine(_directory, "progress.json")));
        Assert.DoesNotContain("Eigener Ansatz", html);
        var legacy = (await store.LoadAsync()).SoftwareLessons[2];
        Assert.Equal("Eigener Ansatz", legacy.Answer.Reasoning);
        var page = new SoftwareTraining(); const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
        typeof(SoftwareTraining).GetField("_task", flags)!.SetValue(page, _catalog.Lessons[1]);
        typeof(SoftwareTraining).GetField("_progress", flags)!.SetValue(page, legacy);
        Assert.False((bool)typeof(SoftwareTraining).GetProperty("HasOwnDraft", flags)!.GetValue(page)!);
    }

    [Fact]
    public async Task ArchitectureHasTextAlternativeAndFormsHaveRealLabelledPreviewInputs()
    {
        using var services = Services(); var task = _catalog.Lessons[25];
        string diagram = await RenderAsync<SoftwareBoardView>(services, new() { ["Task"] = task, ["Board"] = task.Solutions[0].Board });
        Assert.Contains("<svg", diagram); Assert.Contains("role=\"img\"", diagram); Assert.Contains("Architektur als Text lesen", diagram);
        foreach (var node in task.Solutions[0].Board.Nodes) Assert.Contains(node.Name, diagram);
        task = _catalog.Lessons[37];
        string form = await RenderAsync<SoftwareBoardView>(services, new() { ["Task"] = task, ["Board"] = task.Solutions[0].Board });
        Assert.Contains("type=\"date\"", form); Assert.Contains("type=\"number\"", form); Assert.Contains("<select", form);
        Assert.Contains("Vorschau deines Formulars", form); Assert.DoesNotContain("<script", form);
    }

    [Fact]
    public async Task ArchitectureEdgeLabelsStayOutsideNodesAndMvcReturnAvoidsMiddleBox()
    {
        using var services = Services(); var task = _catalog.Lessons[25];
        string html = await RenderAsync<SoftwareBoardView>(services, new() { ["Task"] = task, ["Board"] = task.Solutions[0].Board });
        var svg = System.Xml.Linq.XElement.Parse(html[html.IndexOf("<svg", StringComparison.Ordinal)..(html.IndexOf("</svg>", StringComparison.Ordinal) + 6)]);
        var nodes = svg.Descendants("rect").Where(element => (string?)element.Attribute("class") == "design-node").ToArray();
        foreach (var label in svg.Descendants("text").Where(element => element.Value == "Aufruf"))
            Assert.DoesNotContain(nodes, node => (int)label.Attribute("y")! >= (int)node.Attribute("y")! && (int)label.Attribute("y")! <= (int)node.Attribute("y")! + 80);
        task = _catalog.Lessons[28];
        html = await RenderAsync<SoftwareBoardView>(services, new() { ["Task"] = task, ["Board"] = task.Solutions[0].Board });
        Assert.Contains(" V", html); Assert.Contains("Benachrichtigung", html);
    }

    [Fact]
    public async Task ReferencePausesAndMarksExamAndKeepsAValidatedLocalReturn()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        await store.StartSoftwareExamAsync(new() { CaseId = "radverleih" }, _catalog);
        Assert.Equal("/softwareplanung/pruefung/runde", await ReadReferenceAsync(store, true, null));
        var round = (await store.LoadAsync()).SoftwareExam!;
        Assert.Null(round.RunningSinceUtc); Assert.True(round.UsedReference);
        await store.SaveSoftwareAsync(35, new() { Answer = _catalog.Lessons[34].InitialAnswer() }, _catalog);
        Assert.Equal("/softwareplanung/35", await ReadReferenceAsync(store, false, 999));
        Assert.True((await store.LoadAsync()).SoftwareLessons[35].Assisted);
    }

    /// <summary> Abfrageparameter werden im echten Browser vom Router gesetzt; hier wird dessen fertig gebundener Zustand geprüft. </summary>
    private async Task<string> ReadReferenceAsync(ProgressStore store, bool exam, int? lessonId)
    {
#pragma warning disable BL0005
        var reference = new SoftwareReference { Exam = exam, LessonId = lessonId };
#pragma warning restore BL0005
        var type = typeof(SoftwareReference); const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
        type.GetProperty("Store", flags)!.SetValue(reference, store); type.GetProperty("Catalog", flags)!.SetValue(reference, _catalog);
        await (Task)type.GetMethod("OnInitializedAsync", flags)!.Invoke(reference, [])!;
        return (string)type.GetField("ReturnPath", flags)!.GetValue(reference)!;
    }

    [Theory]
    [InlineData("radverleih")]
    [InlineData("reparatur")]
    [InlineData("kurse")]
    public async Task EachExamPartShowsItsOwnCaseContext(string caseId)
    {
        using var services = Services();
        var store = services.GetRequiredService<ProgressStore>();
        var exam = _catalog.Cases.Single(item => item.Id == caseId);
        var round = new SoftwareExamRound { CaseId = caseId };
        await store.StartSoftwareExamAsync(round, _catalog);
        for (int index = 0; index < exam.Tasks.Count; index++)
        {
            round.Index = index;
            round = await store.SaveSoftwareExamAsync(round, _catalog);
            string html = await RenderAsync<SoftwareExam>(services);
            Assert.Contains($"<p>{exam.Tasks[index].Story}</p>", html);
            Assert.Contains(exam.Tasks[index].Task, html);
            Assert.DoesNotContain("Musterlösung mit Erklärung", html);
        }
    }

    [Fact]
    public async Task ExamEntryOnlyReadsPauseHidesAndSubmittedAnswersAreDisabled()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        string home = await RenderAsync<SoftwareExamHome>(services);
        Assert.Contains("Prüfung starten", home); Assert.Null((await store.LoadAsync()).SoftwareExam);
        Assert.False(File.Exists(Path.Combine(_directory, "progress.json")));
        var round = new SoftwareExamRound { CaseId = "radverleih" };
        await store.StartSoftwareExamAsync(round, _catalog);
        Assert.DoesNotContain("Musterlösung mit Erklärung", await RenderAsync<SoftwareExam>(services));
        round.Pause(DateTimeOffset.UtcNow); round = await store.SaveSoftwareExamAsync(round, _catalog);
        string paused = await RenderAsync<SoftwareExam>(services);
        Assert.Contains("Eine Pause für den Kopf", paused); Assert.DoesNotContain("Deine Ausarbeitung", paused);
        round.RunningSinceUtc = DateTimeOffset.UtcNow;
        foreach (var task in _catalog.Cases[0].Tasks) round.Answers[task.Id] = task.Solutions[0].Copy();
        await store.SaveSoftwareExamAsync(round, _catalog, true);
        string finished = await RenderAsync<SoftwareExam>(services);
        Assert.Contains("50 / 50 Punkte", finished); Assert.Contains("Musterlösung mit Erklärung", finished);
        Assert.Contains("<fieldset class=\"software-worksheet\" disabled", finished);
        Assert.DoesNotContain("Jetzt abgeben", finished);
    }

    [Fact]
    public async Task ReadOnlyHandlersCannotChangeAnyAnswerKind()
    {
        foreach (var task in _catalog.Lessons.Where(task => task.Kind != "Diagram"))
        {
            var answer = task.Solutions.FirstOrDefault()?.Copy() ?? new();
            var node = answer.Board.Nodes.FirstOrDefault() ?? new SoftwareNode();
            var link = answer.Board.Links.FirstOrDefault() ?? new SoftwareLink();
#pragma warning disable BL0005
            var editor = new SoftwareEditor { Task = task, Answer = answer, ReadOnly = true };
#pragma warning restore BL0005
            string before = JsonSerializer.Serialize(answer);
            foreach (var (name, arguments) in new (string, object[])[] {
                ("SelectAsync", ["x"]), ("ValueAsync", [new SoftwareField(), "x"]), ("PayloadAsync", ["x"]),
                ("AddNodeAsync", []), ("RemoveNodeAsync", [node]), ("NodeAsync", [node, "Name", "x"]), ("RequiredAsync", [node, true]),
                ("MoveAsync", [node, 1]), ("AddLinkAsync", []), ("RemoveLinkAsync", [link]), ("LinkAsync", [link, "Kind", "x"]) })
                await (Task)typeof(SoftwareEditor).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(editor, arguments)!;
            Assert.Equal(before, JsonSerializer.Serialize(answer));
        }
    }

    [Fact]
    public async Task ExamAutosaveRetainsEditorReferencesForImmediatelyFollowingInputEvents()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        var round = new SoftwareExamRound { CaseId = "radverleih" };
        var answer = _catalog.Cases[0].Tasks[1].Solutions[0].Copy(); round.Answers[2] = answer;
        await store.StartSoftwareExamAsync(round, _catalog);
        using var page = new SoftwareExam(); var type = typeof(SoftwareExam); const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
        type.GetProperty("Store", flags)!.SetValue(page, store); type.GetProperty("Catalog", flags)!.SetValue(page, _catalog);
        type.GetField("_round", flags)!.SetValue(page, round);
        var link = answer.Board.Links[0];
        await (Task)type.GetMethod("SaveAsync", flags)!.Invoke(page, [])!;
        Assert.Same(round, type.GetField("_round", flags)!.GetValue(page));
        Assert.Same(answer, round.Answers[2]); Assert.Same(link, round.Answers[2].Board.Links[0]);
        link.Target = "";
        await (Task)type.GetMethod("SaveAsync", flags)!.Invoke(page, [])!;
        Assert.Equal("", (await store.LoadAsync()).SoftwareExam!.Answers[2].Board.Links[0].Target);
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
    private sealed class NavigationStub : NavigationManager { public NavigationStub() => Initialize("http://localhost/", "http://localhost/softwareplanung"); protected override void NavigateToCore(string uri, bool forceLoad) => throw new NotSupportedException(); }
    private sealed class EnvironmentStub : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests"; public string EnvironmentName { get; set; } = "Tests"; public string ContentRootPath { get; set; } = ""; public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider(); public string WebRootPath { get; set; } = ""; public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
