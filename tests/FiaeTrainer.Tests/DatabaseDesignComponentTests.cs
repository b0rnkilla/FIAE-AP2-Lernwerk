using FiaeTrainer.Core.DatabaseDesign.Content;
using FiaeTrainer.Core.DatabaseDesign.Practice;
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

/// <summary> Lernansichten zeigen nur bestätigte Planungsergebnisse; Prüfungsinfos starten keine Uhr und verraten keine Lösungen. </summary>
public sealed class DatabaseDesignComponentTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-DesignUi", Guid.NewGuid().ToString("N"));
    private readonly DesignCatalog _catalog = new();
    [Fact]
    public async Task UnsolvedPredecessorShowsReturnLinkWithoutInventingACompletedStep()
    {
        using var services = Services();
        string html = await RenderAsync<DatabaseDesign>(services, 2);
        Assert.Contains("Vorheriger Planungsschritt noch offen:", html);
        Assert.Contains("href=\"/datenbankentwurf/1\"", html);
        Assert.Contains("Du kannst diese Aufgabe trotzdem unabhängig bearbeiten.", html);
        Assert.DoesNotContain(_catalog.Lessons[1].PreviousContext, html);
        Assert.DoesNotContain(_catalog.Lessons[0].Discovery, html);
        Assert.DoesNotContain(_catalog.Lessons[1].Discovery, html);
        Assert.False(File.Exists(Path.Combine(_directory, "progress.json")));
    }

    [Fact]
    public async Task SolvedPredecessorConnectsTheStoryWithoutChangingTheStoredDraft()
    {
        using var services = Services();
        var store = services.GetRequiredService<ProgressStore>();
        var first = _catalog.Lessons[0];
        await store.SaveDesignAsync(1, new() { Answer = new() { SelectedIds = first.CorrectIds.ToList() } }, _catalog, check: true);
        await store.SaveDesignAsync(2, new() { Answer = new() { SelectedIds = [_catalog.Lessons[1].Options.Keys.First()] }, IndependentMode = false, Assisted = true }, _catalog);
        string path = Path.Combine(_directory, "progress.json");
        string before = await File.ReadAllTextAsync(path);
        string html = await RenderAsync<DatabaseDesign>(services, 2);
        Assert.Contains(_catalog.Lessons[1].PreviousContext, html);
        Assert.Contains(first.Discovery, html);
        Assert.DoesNotContain(_catalog.Lessons[1].Discovery, html);
        Assert.DoesNotContain("Vorheriger Planungsschritt noch offen:", html);
        Assert.Equal(before, await File.ReadAllTextAsync(path));
    }

    [Fact]
    public async Task PlanningReportKeepsGapsAndHidesFutureCompletedSteps()
    {
        using var services = Services();
        var store = services.GetRequiredService<ProgressStore>();
        foreach (int id in new[] { 3, 6 })
        {
            var task = _catalog.Lessons[id - 1];
            await store.SaveDesignAsync(id, new() { Answer = new() { SelectedIds = task.CorrectIds.ToList() } }, _catalog, check: true);
        }
        string html = await RenderAsync<DatabaseDesign>(services, 5);
        string report = html.Split("aria-label=\"Bestätigte Planungsschritte\"")[1];
        Assert.Contains(_catalog.Lessons[2].Discovery, report);
        Assert.Contains("Aufgabe 1</a>", report);
        Assert.Contains("Aufgabe 2</a>", report);
        Assert.DoesNotContain(_catalog.Lessons[0].Discovery, report);
        Assert.DoesNotContain(_catalog.Lessons[1].Discovery, report);
        Assert.DoesNotContain(_catalog.Lessons[5].Discovery, report);
    }

    [Fact]
    public async Task EntryOnlyReadsProfileAndShowsExplicitStart()
    {
        using var services = Services(); string html = await RenderAsync<DesignExamHome>(services);
        Assert.Contains("Prüfung starten", html); Assert.Contains("45 Minuten", html);
        Assert.Null((await services.GetRequiredService<ProgressStore>().LoadAsync()).DesignExam);
        Assert.False(File.Exists(Path.Combine(_directory, "progress.json")));
    }
    [Fact]
    public async Task OpenExamHidesSolutionsAndPauseHidesBuilder()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>(); var round = new DesignExamRound { CaseId = "werkstatt" };
        await store.StartDesignExamAsync(round); string open = await RenderAsync<DesignExam>(services);
        Assert.True(open.Contains("Tabelle hinzufügen"), open); Assert.DoesNotContain("Musterlösung", open); Assert.DoesNotContain("Fremdschlüssel</summary>", open);
        round = (await store.LoadAsync()).DesignExam!; round.Pause(DateTimeOffset.UtcNow); await store.SaveDesignExamAsync(round, _catalog);
        string paused = await RenderAsync<DesignExam>(services); Assert.Contains("Eine Pause für den Kopf", paused); Assert.DoesNotContain("Tabelle hinzufügen", paused);
    }
    [Fact]
    public async Task CompletedExamKeepsModelReadOnlyAndShowsExplanation()
    {
        using var services = Services(); var store = services.GetRequiredService<ProgressStore>(); var round = new DesignExamRound { CaseId = "werkstatt" };
        await store.StartDesignExamAsync(round); await store.SaveDesignExamAsync(round, _catalog, true);
        string html = await RenderAsync<DesignExam>(services); Assert.Contains("DEINE PRÜFUNGSAUSWERTUNG", html); Assert.Contains("Musterlösung", html); Assert.Contains("disabled", html);
    }
    private ServiceProvider Services()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Training:ProgressDirectory"] = _directory }).Build();
        return new ServiceCollection().AddLogging().AddSingleton(_catalog).AddSingleton(new ProgressStore(new EnvironmentStub(), config)).AddSingleton<NavigationManager>(new NavigationStub()).BuildServiceProvider();
    }
    private static async Task<string> RenderAsync<T>(ServiceProvider services, int? id = null) where T : IComponent
    {
        await using HtmlRenderer renderer = new(services, services.GetRequiredService<ILoggerFactory>());
        var parameters = ParameterView.FromDictionary(id.HasValue ? new Dictionary<string, object?> { ["Id"] = id.Value } : []);
        return await renderer.Dispatcher.InvokeAsync(async () => System.Net.WebUtility.HtmlDecode((await renderer.RenderComponentAsync<T>(parameters)).ToHtmlString()));
    }
    public void Dispose() { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
    private sealed class NavigationStub : NavigationManager { public NavigationStub() => Initialize("http://localhost/", "http://localhost/datenbankentwurf/pruefung"); protected override void NavigateToCore(string uri, bool forceLoad) => throw new NotSupportedException(); }
    private sealed class EnvironmentStub : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests"; public string EnvironmentName { get; set; } = "Tests"; public string ContentRootPath { get; set; } = ""; public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider(); public string WebRootPath { get; set; } = ""; public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
