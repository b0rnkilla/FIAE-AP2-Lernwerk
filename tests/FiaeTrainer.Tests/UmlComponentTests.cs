using System.Net;
using System.Reflection;
using FiaeTrainer.Core.Uml.Content;
using FiaeTrainer.Core.Uml.Evaluation;
using FiaeTrainer.Core.Uml.Models;
using FiaeTrainer.Core.Uml.Practice;
using FiaeTrainer.Web.Components.Pages;
using FiaeTrainer.Web.Components.Pages.Training;
using FiaeTrainer.Web.Components.Uml;
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

/// <summary> Prüft Lösungssperren, getrennte Lernrückmeldung und die Darstellung aller Diagrammarten. </summary>
public sealed class UmlComponentTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-UmlUi", Guid.NewGuid().ToString("N"));
    private readonly UmlCatalog _catalog = new();

    [Theory]
    [InlineData(5)] [InlineData(18)] [InlineData(24)] [InlineData(28)] [InlineData(34)]
    public async Task AllKindsRenderOwnReferenceWithTextAlternative(int id)
    {
        using var services = Services(); var task = _catalog.Lessons[id - 1];
        string html = await RenderAsync<UmlDiagram>(services, new() { ["Task"] = task, ["Model"] = task.Solutions[0] });
        Assert.Contains("<svg", html); Assert.Contains("role=\"img\"", html); Assert.Contains("Diagramm als Text lesen", html);
        foreach (var node in task.Solutions[0].Nodes) Assert.Contains(node.Name, html);
        Assert.DoesNotContain("<script", html);
    }

    [Fact]
    public async Task ClassMultiplicitiesRemainOutsideBoxesAndDoNotShareOnePosition()
    {
        using var services = Services(); var task = _catalog.Lessons[17];
        string html = await RenderAsync<UmlDiagram>(services, new() { ["Task"] = task, ["Model"] = task.Solutions[0] });
        var svg = System.Xml.Linq.XElement.Parse(html[html.IndexOf("<svg", StringComparison.Ordinal)..(html.IndexOf("</svg>", StringComparison.Ordinal) + 6)]);
        var labels = svg.Descendants().Where(element => element.Name.LocalName == "text" && element.Value is "1" or "0..*").ToArray();
        Assert.Equal(4, labels.Length);
        Assert.Equal(4, labels.Select(element => ((int)element.Attribute("x")!, (int)element.Attribute("y")!)).Distinct().Count());
        foreach (var label in labels)
        {
            int x = (int)label.Attribute("x")!, y = (int)label.Attribute("y")!;
            Assert.DoesNotContain(UmlLayout.Place(task, task.Solutions[0]).Values, box => x >= box.X && x <= box.X + box.Width && y >= box.Y && y <= box.Y + box.Height);
        }
    }

    [Fact]
    public async Task BuildStartsEmptyAndReadingThePageDoesNotCreateProgress()
    {
        using var services = Services(); string html = await RenderAsync<UmlTraining>(services, new() { ["Id"] = 5 });
        Assert.Contains("Deine Arbeitsfläche ist noch leer", html); Assert.Contains("Element hinzufügen", html);
        Assert.Contains("Ohne Hilfe lösen", html); Assert.Contains("Geführt lernen", html);
        Assert.Contains("Vorheriger Planungsschritt noch offen", html);
        Assert.DoesNotContain(_catalog.Lessons[4].Discovery, html);
        Assert.False(File.Exists(Path.Combine(_directory, "progress.json")));
    }

    [Fact]
    public async Task StoryUsesOnlyConfirmedPredecessorsAndRootResumesOwnLastTask()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        await store.SaveUmlAsync(4, new() { Answer = new() { SelectedIds = _catalog.Lessons[3].CorrectIds.ToList() } }, _catalog, true);
        await store.SaveUmlAsync(5, new() { Answer = new() { Model = _catalog.Lessons[4].Solutions[0].Copy() } }, _catalog);
        string path = Path.Combine(_directory, "progress.json"), before = await File.ReadAllTextAsync(path);
        string html = await RenderAsync<UmlTraining>(services);
        Assert.Contains(_catalog.Lessons[4].Title, html); Assert.Contains(_catalog.Lessons[4].PreviousContext, html);
        Assert.DoesNotContain(_catalog.Lessons[4].Discovery, html); Assert.Contains(_catalog.Lessons[3].Discovery, html);
        Assert.Equal(before, await File.ReadAllTextAsync(path));
    }

    [Fact]
    public async Task LearningFeedbackHasOneOutcomeAndNoPointsOrPrematureExplanation()
    {
        using var services = Services(); var task = _catalog.Lessons[6];
        string wrong = await RenderAsync<UmlLearningFeedback>(services, new() { ["Task"] = task, ["Result"] = UmlEvaluator.Evaluate(task, task.InitialAnswer()) });
        Assert.DoesNotContain(task.Explanation, wrong); Assert.DoesNotContain(task.Discovery, wrong);
        string right = await RenderAsync<UmlLearningFeedback>(services, new() { ["Task"] = task, ["Result"] = UmlEvaluator.Evaluate(task, new() { Model = task.Solutions[0].Copy() }) });
        Assert.Contains(task.Discovery, right);
        foreach (string html in new[] { wrong, right }) { Assert.DoesNotContain("Trainingspunkte", html); Assert.DoesNotContain("50 / 50", html); Assert.DoesNotContain("IHK", html); }
    }

    [Fact]
    public async Task ExamEntryOnlyReadsAndPauseHidesAnswers()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        string home = await RenderAsync<UmlExamHome>(services);
        Assert.Contains("Prüfung starten", home); Assert.Null((await store.LoadAsync()).UmlExam);
        Assert.False(File.Exists(Path.Combine(_directory, "progress.json")));
        await store.StartUmlExamAsync(new() { CaseId = "proberaum" });
        string open = await RenderAsync<UmlExam>(services);
        Assert.Contains("Element hinzufügen", open); Assert.DoesNotContain("Musterlösung", open);
        var round = (await store.LoadAsync()).UmlExam!; round.Pause(DateTimeOffset.UtcNow); await store.SaveUmlExamAsync(round, _catalog);
        string paused = await RenderAsync<UmlExam>(services);
        Assert.Contains("Eine Pause für den Kopf", paused); Assert.DoesNotContain("Element hinzufügen", paused); Assert.DoesNotContain("Merkmal-Fachrolle", paused);
    }

    [Fact]
    public async Task SubmittedExamHasReadOnlyBuilderAndSolutions()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        var round = new UmlExamRound { CaseId = "proberaum" };
        await store.StartUmlExamAsync(round);
        foreach (var task in _catalog.Cases[0].Tasks) round.Answers[task.Id] = new() { Model = task.Solutions[0].Copy() };
        await store.SaveUmlExamAsync(round, _catalog, true);
        string html = await RenderAsync<UmlExam>(services);
        Assert.Contains("50 / 50 Punkte", html); Assert.Contains("Musterlösung mit Erklärung", html);
        Assert.Contains("<fieldset class=\"uml-node-editor\" disabled", html); Assert.DoesNotContain("Jetzt abgeben", html);
    }

    [Fact]
    public async Task ReadOnlyHandlersCannotMutateGraphOrSelection()
    {
        var task = _catalog.Lessons[27]; var answer = new UmlAnswer { Model = task.Solutions[0].Copy() };
        // Gezielter Handler-Test ohne Renderer: die Parameter werden absichtlich direkt gesetzt.
#pragma warning disable BL0005
        var editor = new UmlEditor { Task = task, Answer = answer, ReadOnly = true };
#pragma warning restore BL0005
        var node = answer.Model.Nodes[0]; var link = answer.Model.Links[0]; var fragment = answer.Model.Fragments[0]; var member = new UmlMember(); node.Members.Add(member);
        string before = System.Text.Json.JsonSerializer.Serialize(answer);
        foreach (var (name, arguments) in new (string, object[] )[] {
            ("AddNodeAsync", []), ("RemoveNodeAsync", [node]), ("NodeMeaningAsync", [node, "archive"]),
            ("AddMemberAsync", [node]), ("RemoveMemberAsync", [node, member]), ("MemberMeaningAsync", [member, "x"]),
            ("AddLinkAsync", []), ("RemoveLinkAsync", [link]), ("LinkMeaningAsync", [link, "x"]),
            ("AddFragmentAsync", []), ("RemoveFragmentAsync", [fragment]), ("MoveAsync", [link, 1]), ("SelectAsync", ["x"]) })
            await (Task)typeof(UmlEditor).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(editor, arguments)!;
        Assert.Equal(before, System.Text.Json.JsonSerializer.Serialize(answer));
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
    private sealed class NavigationStub : NavigationManager { public NavigationStub() => Initialize("http://localhost/", "http://localhost/uml"); protected override void NavigateToCore(string uri, bool forceLoad) => throw new NotSupportedException(); }
    private sealed class EnvironmentStub : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests"; public string EnvironmentName { get; set; } = "Tests"; public string ContentRootPath { get; set; } = ""; public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider(); public string WebRootPath { get; set; } = ""; public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
