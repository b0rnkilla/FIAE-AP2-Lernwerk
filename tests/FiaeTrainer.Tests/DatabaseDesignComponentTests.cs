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

/// <summary> Neue Prüfungsoberflächen dürfen die Uhr nicht beim Einstieg starten oder offene Lösungen verraten. </summary>
public sealed class DatabaseDesignComponentTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-DesignUi", Guid.NewGuid().ToString("N"));
    private readonly DesignCatalog _catalog = new();
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
    private static async Task<string> RenderAsync<T>(ServiceProvider services) where T : IComponent
    {
        await using HtmlRenderer renderer = new(services, services.GetRequiredService<ILoggerFactory>());
        return await renderer.Dispatcher.InvokeAsync(async () => System.Net.WebUtility.HtmlDecode((await renderer.RenderComponentAsync<T>()).ToHtmlString()));
    }
    public void Dispose() { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
    private sealed class NavigationStub : NavigationManager { public NavigationStub() => Initialize("http://localhost/", "http://localhost/datenbankentwurf/pruefung"); protected override void NavigateToCore(string uri, bool forceLoad) => throw new NotSupportedException(); }
    private sealed class EnvironmentStub : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests"; public string EnvironmentName { get; set; } = "Tests"; public string ContentRootPath { get; set; } = ""; public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider(); public string WebRootPath { get; set; } = ""; public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
