using System.Reflection;
using System.Text.Json;
using FiaeTrainer.Core.DatabaseDesign.Content;
using FiaeTrainer.Core.DatabaseDesign.Evaluation;
using FiaeTrainer.Core.DatabaseDesign.Models;
using FiaeTrainer.Core.DatabaseDesign.Practice;
using FiaeTrainer.Web.Components.DatabaseDesign;
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

/// <summary> Prüft die Trennung von Lernen/Prüfung, falsche Entwürfe und konkurrierende Bedienaktionen. </summary>
public sealed class DatabaseDesignAuditTests : IDisposable
{
    private readonly DesignCatalog _catalog = new();
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-DesignAudit", Guid.NewGuid().ToString("N"));
    public static IEnumerable<object[]> ModelIds => new DesignCatalog().Lessons.Where(task => task.Kind == "Model").Select(task => new object[] { task.Id });

    [Theory, MemberData(nameof(ModelIds))]
    public void EveryModelAcceptsReorderingAndRenamingButRejectsNullablePrimaryKeys(int id)
    {
        var task = _catalog.Lessons[id - 1];
        foreach (var solution in task.Solutions)
        {
            var model = solution.Copy();
            model.Tables.Reverse();
            for (int i = 0; i < model.Tables.Count; i++)
            {
                model.Tables[i].Name = "Eigene Tabelle " + i;
                model.Tables[i].Columns.Reverse();
                for (int c = 0; c < model.Tables[i].Columns.Count; c++) model.Tables[i].Columns[c].Name = "Feld " + c;
            }
            model.Relations.Reverse();
            foreach (var relation in model.Relations) relation.Pairs.Reverse();
            Assert.True(DesignEvaluator.Evaluate(task, new() { Model = model }).Correct);
            model.Tables.SelectMany(table => table.Columns).First(column => column.PrimaryKey).Nullable = true;
            Assert.False(DesignEvaluator.Evaluate(task, new() { Model = model }).Correct);
        }
    }

    [Fact]
    public void EverySelectionRejectsMissingExtraAndDuplicateOptionsWithoutRevealingTheSolution()
    {
        foreach (var task in _catalog.Lessons.Where(task => task.Kind is "Choice" or "Multiple"))
        {
            foreach (var ids in new[] { new List<string>(), task.CorrectIds.Skip(1).ToList(), task.CorrectIds.Concat(["unbekannt"]).ToList(), task.CorrectIds.Concat([task.CorrectIds[0]]).ToList() })
            {
                var result = DesignEvaluator.Evaluate(task, new() { SelectedIds = ids });
                Assert.False(result.Correct);
                Assert.DoesNotContain(task.Explanation, string.Join(" ", result.Checks.Select(check => check.Detail)));
            }
        }
    }

    [Theory]
    [InlineData(1, true)] [InlineData(1, false)] [InlineData(5, true)] [InlineData(5, false)]
    public async Task LearningFeedbackHasOneOutcomeCardWithoutExamPoints(int id, bool correct)
    {
        var task = _catalog.Lessons[id - 1];
        var answer = correct ? task.Kind == "Model" ? new DesignAnswer { Model = task.Solutions[0].Copy() } : new() { SelectedIds = task.CorrectIds.ToList() } : new();
        string html = await RenderAsync<DesignLearningFeedback>(new()
        {
            ["Task"] = task, ["Result"] = DesignEvaluator.Evaluate(task, answer), ["IndependentMode"] = true
        });
        Assert.Equal(1, html.Split("role=\"status\"").Length - 1);
        Assert.DoesNotContain("Trainingspunkte", html);
        Assert.DoesNotContain("50 / 50", html);
        Assert.DoesNotContain("IHK", html);
        Assert.Contains(correct ? "Dieser Schritt stimmt." : "Schau noch einmal genau hin.", html);
        if (correct) Assert.Contains(task.Discovery, html);
        else Assert.DoesNotContain(task.Discovery, html);
    }

    [Theory]
    [InlineData(20)] [InlineData(10)]
    public async Task ExamFeedbackKeepsItsWeightedPoints(int maximum)
    {
        var task = _catalog.Cases[0].Tasks[maximum == 10 ? 2 : 0];
        var answer = task.Kind == "Model" ? new DesignAnswer { Model = task.Solutions[0].Copy() } : new() { SelectedIds = task.CorrectIds.ToList() };
        string html = await RenderAsync<DesignFeedback>(new() { ["Result"] = DesignEvaluator.Evaluate(task, answer), ["Maximum"] = maximum });
        Assert.Contains($"{maximum} / {maximum} Trainingspunkte", html);
        Assert.Contains("keine amtliche IHK-Note", html);
    }

    [Fact]
    public async Task TableNamesRemainUniqueAfterRemovingAMiddleTable()
    {
        var editor = Configure(new DesignAnswerEditor(), new() { ["Task"] = _catalog.Lessons[4], ["Answer"] = new DesignAnswer() });
        for (int i = 0; i < 3; i++) await InvokeAsync(editor, "AddTableAsync");
        await InvokeAsync(editor, "RemoveTableAsync", editor.Answer.Model.Tables[1]);
        await InvokeAsync(editor, "AddTableAsync");
        Assert.Equal(3, editor.Answer.Model.Tables.Select(table => table.Name).Distinct().Count());
        Assert.Contains(editor.Answer.Model.Tables, table => table.Name == "Tabelle2");
    }

    [Fact]
    public async Task ReadOnlyEditorRejectsAllMutationCommands()
    {
        var task = _catalog.Lessons[9];
        var answer = new DesignAnswer { Model = task.Solutions[0].Copy() };
        var editor = Configure(new DesignAnswerEditor(), new() { ["Task"] = task, ["Answer"] = answer, ["ReadOnly"] = true });
        var table = answer.Model.Tables[0]; var column = table.Columns[0]; var relation = answer.Model.Relations[0];
        string before = JsonSerializer.Serialize(answer);
        await InvokeAsync(editor, "AddTableAsync");
        await InvokeAsync(editor, "RemoveTableAsync", table);
        await InvokeAsync(editor, "AddColumnAsync", table);
        await InvokeAsync(editor, "RemoveColumnAsync", table, column);
        await InvokeAsync(editor, "AttributeAsync", column, "Anders");
        await InvokeAsync(editor, "AddRelationAsync");
        await InvokeAsync(editor, "RemoveRelationAsync", relation);
        await InvokeAsync(editor, "TableAsync", relation, true, "Anders");
        await InvokeAsync(editor, "AddPairAsync", relation);
        await InvokeAsync(editor, "RemovePairAsync", relation, relation.Pairs[0]);
        await InvokeAsync(editor, "SelectAsync", "a");
        Assert.Equal(before, JsonSerializer.Serialize(answer));
    }

    [Fact]
    public async Task RapidNavigationDoesNotCombineTheOldTaskWithTheNewDraft()
    {
        var store = Store();
        foreach (int id in new[] { 5, 8 })
        {
            var model = _catalog.Lessons[id - 1].Solutions[0].Copy(); model.Tables[0].Name = "Entwurf " + id;
            await store.SaveDesignAsync(id, new() { Answer = new() { Model = model } }, _catalog);
        }
        var page = Configure(new TestPage { Catalog = _catalog, Store = store }, new() { ["Id"] = 5 });
        var storeGate = Field<SemaphoreSlim>(store, "_gate");
        await storeGate.WaitAsync();
        Task first = page.LoadAsync();
        Configure(page, new() { ["Id"] = 8 });
        Task second = page.LoadAsync();
        storeGate.Release();
        await Task.WhenAll(first, second);
        Assert.Equal(8, Field<DesignAssignment>(page, "_task").Id);
        Assert.Equal("Entwurf 8", Field<DesignProgress>(page, "_progress").Answer.Model.Tables[0].Name);
        Assert.Equal(8, Field<int>(page, "_loaded"));
        var resume = new TestPage { Catalog = _catalog, Store = store };
        await resume.LoadAsync();
        Assert.Equal(8, Field<DesignAssignment>(resume, "_task").Id);
    }

    [Fact]
    public async Task DoubleCheckCountsOnceAndModeChangeClearsThePreviousOutcome()
    {
        var store = Store();
        await store.SaveDesignAsync(1, new() { Answer = new() { SelectedIds = _catalog.Lessons[0].CorrectIds.ToList() } }, _catalog);
        var page = Configure(new TestPage { Catalog = _catalog, Store = store }, new() { ["Id"] = 1 });
        await page.LoadAsync();
        var storeGate = Field<SemaphoreSlim>(store, "_gate");
        await storeGate.WaitAsync();
        Task first = InvokeAsync(page, "CheckAsync");
        Task second = InvokeAsync(page, "CheckAsync");
        storeGate.Release();
        await Task.WhenAll(first, second);
        Assert.Equal(1, (await store.LoadAsync()).DesignLessons[1].Attempts);
        Assert.True(Field<DesignEvaluation>(page, "_result").Correct);
        await InvokeAsync(page, "SetModeAsync", false);
        Assert.Null(PrivateField(page, "_result").GetValue(page));
        var saved = (await store.LoadAsync()).DesignLessons[1];
        Assert.True(saved.Completed && saved.Independent && saved.Assisted);
        Assert.False(saved.IndependentMode);
    }

    private static T Configure<T>(T component, Dictionary<string, object?> parameters) where T : IComponent { ParameterView.FromDictionary(parameters).SetParameterProperties(component); return component; }
    private ProgressStore Store() => new(new EnvironmentStub(), new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Training:ProgressDirectory"] = _directory }).Build());
    private static FieldInfo PrivateField(object target, string name) => (target is DatabaseDesign ? typeof(DatabaseDesign) : target.GetType()).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!;
    private static T Field<T>(object target, string name) => (T)PrivateField(target, name).GetValue(target)!;
    private static Task InvokeAsync(object target, string name, params object[] args) => (Task)(target is DatabaseDesign ? typeof(DatabaseDesign) : target.GetType()).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(target, args)!;
    private static async Task<string> RenderAsync<T>(Dictionary<string, object?> parameters) where T : IComponent
    {
        using var services = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using HtmlRenderer renderer = new(services, services.GetRequiredService<ILoggerFactory>());
        return await renderer.Dispatcher.InvokeAsync(async () => System.Net.WebUtility.HtmlDecode((await renderer.RenderComponentAsync<T>(ParameterView.FromDictionary(parameters))).ToHtmlString()));
    }
    public void Dispose() { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
    private sealed class TestPage : DatabaseDesign { public Task LoadAsync() => OnParametersSetAsync(); }
    private sealed class EnvironmentStub : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests"; public string EnvironmentName { get; set; } = "Tests"; public string ContentRootPath { get; set; } = ""; public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider(); public string WebRootPath { get; set; } = ""; public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
