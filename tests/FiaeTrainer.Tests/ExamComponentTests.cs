using FiaeTrainer.Core.Algorithms.Exams;
using FiaeTrainer.Core.Algorithms.Execution;
using FiaeTrainer.Core.Exams;
using FiaeTrainer.Core.Sql.Database;
using FiaeTrainer.Core.Sql.Exams;
using FiaeTrainer.Web.Components.Pages.Training;
using FiaeTrainer.Web.Configuration;
using FiaeTrainer.Web.Exams;
using FiaeTrainer.Web.Learning;
using FiaeTrainer.Web.Networking;
using FiaeTrainer.Web.Sql;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Offene Prüfungen dürfen weder Lösungen noch Ausführungswerkzeuge oder vorweggenommene Noten rendern. </summary>
public sealed class ExamComponentTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-Exam-Components", Guid.NewGuid().ToString("N"));
    private readonly AlgorithmExamCatalog _algorithms = new();

    [Theory]
    [InlineData(false)] [InlineData(true)]
    public async Task ActiveRoundConcealsSolutionsAndPreviewUntilSubmission(bool sql)
    {
        using var services = Services(sql);
        var store = services.GetRequiredService<ProgressStore>();
        await store.StartExamAsync(sql ? new SqlExamCatalog().CreateRound("veranstaltungen") : _algorithms.CreateRound());
        string html = await RenderAsync(services);
        Assert.Contains("Verbleibende Prüfungszeit", html);
        Assert.DoesNotContain("Musterlösung", html);
        Assert.DoesNotContain("Ablauf untersuchen", html);
        Assert.DoesNotContain("SQL untersuchen", html);
        Assert.DoesNotContain("DEINE PRÜFUNGSAUSWERTUNG", html);
    }

    [Fact]
    public async Task PauseConcealsAssignmentAndEditor()
    {
        using var services = Services(false);
        var round = _algorithms.CreateRound(); round.RunningSinceUtc = null;
        await services.GetRequiredService<ProgressStore>().StartExamAsync(round);
        string html = await RenderAsync(services);
        Assert.Contains("Eine Pause für den Kopf", html);
        Assert.DoesNotContain("exam-editor", html);
        Assert.DoesNotContain("Funktionsvertrag", html);
    }

    [Fact]
    public async Task TechnicalSqlFailureKeepsSubmittedDraftReadOnlyWithoutInventingZeroScore()
    {
        using var services = Services(true);
        var store = services.GetRequiredService<ProgressStore>();
        var round = new SqlExamCatalog().CreateRound("service");
        round.Drafts[round.TaskIds[0]] = "SELECT Id FROM Auftraege;";
        await store.StartExamAsync(round);
        await store.SaveExamAsync(round, true);
        string html = await RenderAsync(services);
        Assert.Contains("Deine Abgabe ist gespeichert", html);
        Assert.Contains("Auswertung erneut versuchen", html);
        Assert.Contains("readonly", html);
        Assert.DoesNotContain("DEINE PRÜFUNGSAUSWERTUNG", html);
        Assert.DoesNotContain("Musterlösung", html);
        var restored = await store.LoadAsync();
        Assert.Empty(restored.ExamHistory);
        Assert.Null(restored.ExamRounds[ExamModule.Sql].EvaluatedUtc);
    }

    private ServiceProvider Services(bool sql)
    {
        var env = new TestEnvironment();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Training:ProgressDirectory"] = _directory, ["LocalRelease:SettingsFile"] = Path.Combine(_directory, "settings.json") }).Build();
        var state = new DatabaseState(new TrainingDatabase("Server=127.0.0.1,1;Database=FIAE_AP2_Training;Integrated Security=True;Connect Timeout=1"), NullLogger<DatabaseState>.Instance, new LocalSettingsStore(configuration, env, new NetworkAccessPolicy(false)));
        return new ServiceCollection().AddLogging().AddSingleton(new ProgressStore(env, configuration)).AddSingleton(_algorithms).AddSingleton<AlgorithmInterpreter>().AddSingleton<AlgorithmExamEvaluator>().AddSingleton<SqlExamCatalog>().AddSingleton(state).AddSingleton<ExamService>().AddSingleton<NavigationManager>(new TestNavigation(sql)).AddSingleton<IJSRuntime>(new StaticJs()).BuildServiceProvider();
    }
    private static async Task<string> RenderAsync(ServiceProvider services)
    {
        await using HtmlRenderer renderer = new(services, services.GetRequiredService<ILoggerFactory>());
        return await renderer.Dispatcher.InvokeAsync(async () => System.Net.WebUtility.HtmlDecode((await renderer.RenderComponentAsync<Exam>()).ToHtmlString()));
    }
    public void Dispose() { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
    private sealed class TestNavigation : NavigationManager
    {
        public TestNavigation(bool sql) => Initialize("http://localhost/", "http://localhost/" + (sql ? "sql" : "algorithmen") + "/pruefung/runde");
        protected override void NavigateToCore(string uri, bool forceLoad) => throw new NotSupportedException();
    }
    private sealed class StaticJs : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) => ValueTask.FromResult(default(TValue)!);
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) => ValueTask.FromResult(default(TValue)!);
    }
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
