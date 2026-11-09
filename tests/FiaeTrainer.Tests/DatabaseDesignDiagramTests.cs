using System.Reflection;
using System.Text.Json;
using FiaeTrainer.Core.DatabaseDesign.Content;
using FiaeTrainer.Core.DatabaseDesign.Evaluation;
using FiaeTrainer.Core.DatabaseDesign.Models;
using FiaeTrainer.Core.DatabaseDesign.Practice;
using FiaeTrainer.Web.Components.DatabaseDesign;
using FiaeTrainer.Web.Components.Pages;
using FiaeTrainer.Web.Components.Pages.Training;
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

/// <summary> Prüft Diagrammrichtungen, unabhängige Fachregeln, neue Lernwege und abwärtskompatible Speicherung. </summary>
public sealed class DatabaseDesignDiagramTests : IDisposable
{
    /// <summary> Eigenes synthetisches Profil, getrennt vom laufenden Nutzertrainer. </summary>
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-Diagrams", Guid.NewGuid().ToString("N"));
    private readonly DesignCatalog _catalog = new();

    [Theory]
    [InlineData(31)] [InlineData(32)] [InlineData(33)] [InlineData(34)]
    public void EveryDiagramSlotRejectsMissingWrongAndUnknownAnswers(int id)
    {
        var task = _catalog.Lessons[id - 1];
        var correct = task.DiagramSlots.ToDictionary(slot => slot.Id, slot => slot.CorrectId);
        Assert.True(DesignEvaluator.Evaluate(task, new() { DiagramValues = correct }).Correct);
        foreach (var slot in task.DiagramSlots)
        {
            var missing = new Dictionary<string, string>(correct); missing.Remove(slot.Id);
            Assert.False(DesignEvaluator.Evaluate(task, new() { DiagramValues = missing }).Correct);
            var wrong = new Dictionary<string, string>(correct) { [slot.Id] = slot.Options.Keys.First(key => key != slot.CorrectId) };
            var result = DesignEvaluator.Evaluate(task, new() { DiagramValues = wrong });
            Assert.InRange(result.Earned, 1, 49);
            Assert.DoesNotContain(task.Explanation, string.Join(" ", result.Checks.Select(check => check.Detail)));
        }
        var extra = new Dictionary<string, string>(correct) { ["fremd"] = "one" };
        Assert.False(DesignEvaluator.Evaluate(task, new() { DiagramValues = extra }).Correct);
        correct["A"] = "unbekannt";
        Assert.False(DesignEvaluator.Evaluate(task, new() { DiagramValues = correct }).Correct);
    }

    [Fact]
    public void CardinalityDirectionsAndRelationshipAttributeHaveIndependentExpectedValues()
    {
        Assert.True(DesignEvaluator.Evaluate(_catalog.Lessons[30], new() { DiagramValues = new() { ["A"] = "one", ["B"] = "many" } }).Correct);
        Assert.False(DesignEvaluator.Evaluate(_catalog.Lessons[30], new() { DiagramValues = new() { ["A"] = "many", ["B"] = "one" } }).Correct);
        Assert.True(DesignEvaluator.Evaluate(_catalog.Lessons[31], new() { DiagramValues = new() { ["A"] = "many", ["B"] = "many", ["C"] = "holds", ["D"] = "fee" } }).Correct);
        Assert.True(DesignEvaluator.Evaluate(_catalog.Lessons[32], new() { DiagramValues = new() { ["A"] = "room", ["B"] = "many", ["C"] = "one" } }).Correct);
    }

    [Fact]
    public void TranslationAndEvidenceTasksRejectPlausibleStructuralMistakes()
    {
        var material = _catalog.Lessons[34]; var model = material.Solutions[0].Copy();
        model.Tables.Single(table => table.Name == "Materialbedarf").Columns.Single(column => column.Attribute == "MaterialId").PrimaryKey = false;
        Assert.False(DesignEvaluator.Evaluate(material, new() { Model = model }).Correct);
        var briefing = _catalog.Lessons[35]; model = briefing.Solutions[0].Copy();
        model.Tables.Single(table => table.Name == "Briefing").Columns.Single(column => column.Attribute == "AuftragId").UniqueGroup = "";
        Assert.False(DesignEvaluator.Evaluate(briefing, new() { Model = model }).Correct);
        var composite = _catalog.Lessons[36];
        Assert.False(DesignEvaluator.Evaluate(composite, composite.InitialAnswer()).Correct);
        Assert.Equal(2, composite.Solutions[0].Relations.Single().Pairs.Count);
        var questions = _catalog.Lessons[38]; model = questions.Solutions[0].Copy();
        model.Tables.Single(table => table.Name == "Rueckfrage").Columns.Single(column => column.Attribute == "BeantwortetAm").Nullable = false;
        Assert.False(DesignEvaluator.Evaluate(questions, new() { Model = model }).Correct);
        var stands = _catalog.Lessons[39]; model = stands.Solutions[0].Copy();
        model.Tables.Single(table => table.Name == "Partner").Columns.Single(column => column.Attribute == "Kontaktmail").UniqueGroup = "A";
        Assert.False(DesignEvaluator.Evaluate(stands, new() { Model = model }).Correct);
        Assert.Equal(4, _catalog.Lessons[40].Solutions[0].Tables.Count);
        Assert.True(DesignEvaluator.Evaluate(_catalog.Lessons[41], new() { SelectedIds = ["change", "insert"] }).Correct);
        Assert.All(_catalog.Lessons.Where(task => task.ShowStarter), task => Assert.False(DesignEvaluator.Evaluate(task, task.InitialAnswer()).Correct));
    }

    [Theory]
    [InlineData(31)] [InlineData(34)] [InlineData(37)] [InlineData(41)] [InlineData(42)]
    public async Task NewAnswersRoundTripWithoutChangingOldProfileFields(int id)
    {
        Directory.CreateDirectory(_directory);
        await File.WriteAllTextAsync(Path.Combine(_directory, "progress.json"), "{\"LastLessonId\":17,\"Lessons\":{\"17\":{\"Sql\":\"SELECT 1;\",\"Completed\":true}},\"LastDesignLessonId\":21,\"DesignLessons\":{\"21\":{\"ContentVersion\":\"2026-11-06\",\"Answer\":{\"Model\":{\"Tables\":[],\"Relations\":[]},\"SelectedIds\":[]},\"Completed\":true}}}");
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        var task = _catalog.Lessons[id - 1]; var answer = CorrectAnswer(task);
        var progress = new DesignProgress { Answer = answer };
        Assert.True((await store.SaveDesignAsync(id, progress, _catalog, check: true))!.Correct);
        var restored = await store.LoadAsync();
        Assert.Equal("SELECT 1;", restored.Lessons[17].Sql); Assert.True(restored.DesignLessons[21].Completed);
        Assert.Equal(id, restored.LastDesignLessonId);
        Assert.Equal(JsonSerializer.Serialize(answer), JsonSerializer.Serialize(restored.DesignLessons[id].Answer));
        Assert.True(restored.DesignLessons[id].Independent);
        var copy = answer.Copy(); copy.DiagramValues["separat"] = "Wert";
        Assert.False(answer.DiagramValues.ContainsKey("separat"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveDesignAsync(id, new() { Answer = answer }, _catalog));
    }

    [Theory]
    [InlineData(31)] [InlineData(35)] [InlineData(37)] [InlineData(40)] [InlineData(41)] [InlineData(42)]
    public async Task LearningPageShowsActualMaterialsWithoutSolutionsOrPoints(int id)
    {
        using var services = Services();
        string html = await RenderAsync<DatabaseDesign>(services, new() { ["Id"] = id });
        Assert.DoesNotContain("Trainingspunkte", html);
        Assert.DoesNotContain("50 / 50", html);
        Assert.DoesNotContain(_catalog.Lessons[id - 1].Explanation, html);
        Assert.Contains("AUFGABE " + id + " / 42", html);
        if (id == 31) { Assert.Contains("[A]", html); Assert.Contains("<ellipse", html); Assert.Contains("<polygon", html); Assert.Contains("disabled>Lösung prüfen", html); }
        if (id == 35) { Assert.Contains("Materialbedarf im fachlichen ER-Modell", html); Assert.Contains("Tabelle hinzufügen", html); }
        if (id == 37) Assert.Contains("Vorgegebenes relationales Modell", html);
        if (id >= 40) { Assert.Contains("<caption>", html); Assert.Contains("<th scope=\"col\">", html); }
        Assert.False(File.Exists(Path.Combine(_directory, "progress.json")));
    }

    [Theory]
    [InlineData(31)] [InlineData(42)]
    public async Task ReferenceReturnsToNewLessonAndPreservesItsDraft(int id)
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>();
        var answer = new DesignAnswer { DiagramValues = new() { ["A"] = "one" } };
        await store.SaveDesignAsync(id, new() { Answer = answer }, _catalog);
        var reference = new DesignReference();
        typeof(DesignReference).GetProperty("LessonId")!.SetValue(reference, (int?)id);
        typeof(DesignReference).GetProperty("Store", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(reference, store);
        typeof(DesignReference).GetProperty("Catalog", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(reference, _catalog);
        await (Task)typeof(DesignReference).GetMethod("OnInitializedAsync", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(reference, [])!;
        Assert.Equal("/datenbankentwurf/" + id, typeof(DesignReference).GetField("ReturnPath", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(reference));
        var saved = (await store.LoadAsync()).DesignLessons[id];
        Assert.Equal("one", saved.Answer.DiagramValues["A"]); Assert.True(saved.Assisted);
    }

    [Fact]
    public async Task ReadOnlyDiagramIgnoresEvenDirectChangeCalls()
    {
        var answer = new DesignAnswer { DiagramValues = new() { ["A"] = "one" } };
        var editor = new DesignAnswerEditor();
        typeof(DesignAnswerEditor).GetProperty("Task")!.SetValue(editor, _catalog.Lessons[30]);
        typeof(DesignAnswerEditor).GetProperty("Answer")!.SetValue(editor, answer);
        typeof(DesignAnswerEditor).GetProperty("ReadOnly")!.SetValue(editor, true);
        await (Task)typeof(DesignAnswerEditor).GetMethod("DiagramValueAsync", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(editor, ["A", "many"])!;
        Assert.Equal("one", answer.DiagramValues["A"]);
        Assert.False(DesignModelRules.Storable(new() { DiagramValues = null! }));
        Assert.False(DesignModelRules.Storable(new() { DiagramValues = new() { ["A"] = new string('x', 81) } }));
    }

    /// <summary> Referenzen werden nur für Speicherung verwendet; fachliche Gegenbeispiele stehen separat in den Tests. </summary>
    private static DesignAnswer CorrectAnswer(DesignAssignment task) => task.Kind switch
    {
        "ErCompletion" => new() { DiagramValues = task.DiagramSlots.ToDictionary(slot => slot.Id, slot => slot.CorrectId) },
        "Model" => new() { Model = task.Solutions[0].Copy() },
        _ => new() { SelectedIds = task.CorrectIds.ToList() }
    };
    private ServiceProvider Services() => new ServiceCollection().AddLogging().AddSingleton(_catalog).AddSingleton(new ProgressStore(new EnvironmentStub(), new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Training:ProgressDirectory"] = _directory }).Build())).AddSingleton<NavigationManager>(new NavigationStub()).BuildServiceProvider();
    private static async Task<string> RenderAsync<T>(ServiceProvider services, Dictionary<string, object?> parameters) where T : IComponent
    {
        await using HtmlRenderer renderer = new(services, services.GetRequiredService<ILoggerFactory>());
        return await renderer.Dispatcher.InvokeAsync(async () => System.Net.WebUtility.HtmlDecode((await renderer.RenderComponentAsync<T>(ParameterView.FromDictionary(parameters))).ToHtmlString()));
    }
    public void Dispose() { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
    private sealed class NavigationStub : NavigationManager { public NavigationStub() => Initialize("http://localhost/", "http://localhost/datenbankentwurf/31"); protected override void NavigateToCore(string uri, bool forceLoad) => throw new NotSupportedException(); }
    private sealed class EnvironmentStub : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests"; public string EnvironmentName { get; set; } = "Tests"; public string ContentRootPath { get; set; } = ""; public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider(); public string WebRootPath { get; set; } = ""; public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
