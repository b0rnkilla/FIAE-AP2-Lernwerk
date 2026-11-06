using FiaeTrainer.Core.Sql;
using FiaeTrainer.Core.Sql.Investigation;
using FiaeTrainer.Core.Sql.Results;
using FiaeTrainer.Core.Sql.Workshop;
using FiaeTrainer.Web.Components.Pages;
using FiaeTrainer.Web.Components.Sql;
using FiaeTrainer.Web.Components.Sql.Investigation;
using FiaeTrainer.Web.Learning;
using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft sichtbare Empfehlungen und Fallkontext ausschließlich mit eigenen Testprofilen. </summary>
public sealed class LearningExperienceTests : IDisposable
{
    /// <summary> Separates Profil für die gerenderten Lernplanfälle. </summary>
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-Experience", Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task SqlReferenceOffersTheSameReturnButtonForInvestigationAndWorkshop()
    {
        LearningNavigation navigation = new();
        using ServiceProvider services = new ServiceCollection().AddLogging().AddSingleton(navigation).BuildServiceProvider();
        navigation.Visit("sql/17");
        string investigation = await RenderAsync<Reference>([], services);
        Assert.Contains("href=\"/sql/17\"", investigation);
        Assert.Contains("Zurück zu deiner Aufgabe →", investigation);
        navigation.Visit("sql-werkstatt/4");
        string workshop = await RenderAsync<Reference>([], services);
        Assert.Contains("href=\"/sql-werkstatt/4\"", workshop);
        Assert.DoesNotContain("Pseudocode, UML und die übrigen Module sind in Vorbereitung", workshop);
    }

    [Fact]
    public async Task EmptyPlanOffersBothFirstTasksWithoutCalendarDeadlines()
    {
        string html = await RenderPlanAsync();
        Assert.Contains("href=\"/sql/1\"", html);
        Assert.Contains("href=\"/sql-werkstatt/1\"", html);
        Assert.Contains("href=\"/algorithmen/1\"", html);
        Assert.Contains("0 / 30 gelöst", html);
        Assert.DoesNotContain("WOCHE", html);
        Assert.DoesNotContain("drei Wochen", html);
        Assert.Contains("Keine gelöste Aufgabe wartet", html);
    }

    [Fact]
    public async Task AlgorithmWorkUsesTheSameOpenAndReviewSectionsWithoutContradictoryMessages()
    {
        ProgressStore store = CreateStore();
        await store.SaveAlgorithmAsync(1, new AlgorithmProgress { Completed = true, Independent = true });
        await store.SaveAlgorithmAsync(2, new AlgorithmProgress { Completed = true });
        await store.SaveAlgorithmAsync(3, new AlgorithmProgress { Draft = "Entwurf" });
        string html = await RenderPlanAsync();
        Assert.Contains("2 / 30 gelöst", html);
        string review = html.Split("Gelöst – jetzt ohne Hilfe wiederholen")[1].Split("DEINE LERNROUTINE")[0];
        Assert.Contains("href=\"/algorithmen/2\"", review);
        Assert.DoesNotContain("href=\"/algorithmen/1\"", review);
        Assert.DoesNotContain("Keine gelöste Aufgabe wartet", review);
        string open = html.Split("Begonnen, noch nicht gelöst")[1].Split("SELBSTSTÄNDIG FESTIGEN")[0];
        Assert.Contains("href=\"/algorithmen/3\"", open);
        Assert.DoesNotContain("Aktuell gibt es keine begonnenen", open);
        Assert.Contains("href=\"/algorithmen/4\"", html);
    }



    [Fact]
    public async Task PlanDistinguishesOpenTasksAssistedAchievementsAndIndependentAchievements()
    {
        ProgressStore store = CreateStore();
        await store.SaveLessonAsync(1, new LessonProgress { Completed = true, Independent = true });
        await store.SaveLessonAsync(2, new LessonProgress { Completed = true });
        await store.SaveLessonAsync(3, new LessonProgress { Attempts = 2, Sql = "Entwurf" });
        await store.SaveLessonAsync(1, new LessonProgress { Completed = true }, LearningArea.Workshop);
        string html = await RenderPlanAsync();
        Assert.Contains("2 / 30 gelöst", html);
        Assert.Contains("1 davon auch ohne Hilfe", html);
        Assert.Contains("href=\"/sql/3\"", html);
        Assert.Contains("href=\"/sql/4\"", html);
        Assert.Contains("href=\"/sql-werkstatt/2\"", html);
        string review = html.Split("Gelöst – jetzt ohne Hilfe wiederholen")[1].Split("DEINE LERNROUTINE")[0];
        Assert.Contains("href=\"/sql/2\"", review);
        Assert.Contains("href=\"/sql-werkstatt/1\"", review);
        Assert.DoesNotContain("href=\"/sql/1\"", review);
        Assert.DoesNotContain("href=\"/sql/3\"", review);
    }

    [Fact]
    public async Task CompletedPlanInvitesTransferInsteadOfInventingAnotherOpenTask()
    {
        ProgressStore store = CreateStore();
        foreach (Lesson lesson in new LessonCatalog().Lessons)
            await store.SaveLessonAsync(lesson.Id, new LessonProgress { Completed = true, Independent = true });
        foreach (WorkshopLesson lesson in new WorkshopCatalog().Lessons)
            await store.SaveLessonAsync(lesson.Id, new LessonProgress { Completed = true, Independent = true }, LearningArea.Workshop);
        foreach (var lesson in new FiaeTrainer.Core.Algorithms.Content.AlgorithmCatalog().Lessons)
            await store.SaveAlgorithmAsync(lesson.Id, new AlgorithmProgress { Completed = true, Independent = true });
        string html = await RenderPlanAsync();
        Assert.Contains("Alle 70 Übungen aus SQL und Algorithmen sind gelöst", html);
        Assert.Contains("unbekannte Aufgaben auf Papier", html);
        Assert.DoesNotContain("Weiter ermitteln →", html);
    }

    [Fact]
    public async Task UnfinishedDraftsAreOfferedForContinuationWithoutClaimingCompletion()
    {
        ProgressStore store = CreateStore();
        foreach (Lesson lesson in new LessonCatalog().Lessons)
            await store.SaveLessonAsync(lesson.Id, new LessonProgress { Sql = "Entwurf" });
        foreach (WorkshopLesson lesson in new WorkshopCatalog().Lessons)
            await store.SaveLessonAsync(lesson.Id, new LessonProgress { Sql = "Entwurf" }, LearningArea.Workshop);
        foreach (var lesson in new FiaeTrainer.Core.Algorithms.Content.AlgorithmCatalog().Lessons)
            await store.SaveAlgorithmAsync(lesson.Id, new AlgorithmProgress { Draft = "Entwurf" });
        string html = await RenderPlanAsync();
        Assert.Contains("Du hast alle Aufgaben aus SQL und Algorithmen bereits begonnen", html);
        Assert.DoesNotContain("Alle 70 Übungen aus SQL und Algorithmen sind gelöst", html);
        Assert.Contains("href=\"/sql/1\"", html);
        Assert.Contains("href=\"/sql-werkstatt/10\"", html);
    }

    [Fact]
    public async Task CaseNotesAreChronologicalShowGapsAndDoNotRevealUnsolvedDiscoveries()
    {
        LessonCatalog catalog = new();
        Dictionary<int, LessonProgress> progress = new() { [3] = new() { Completed = true }, [1] = new() { Completed = true }, [2] = new() { Attempts = 1 } };
        string html = await RenderAsync<CaseNotes>(new() { [nameof(CaseNotes.Lessons)] = catalog.Lessons, [nameof(CaseNotes.Progress)] = progress });
        Assert.True(html.IndexOf("AUFGABE 01", StringComparison.Ordinal) < html.IndexOf("AUFGABE 03", StringComparison.Ordinal));
        Assert.Contains("Aufgabe 2 ist noch nicht gelöst", html);
        Assert.Contains(catalog.Lessons[2].Discovery, html);
        Assert.DoesNotContain(catalog.Lessons[1].Discovery, html);
        Assert.DoesNotContain(catalog.Lessons[29].Discovery, html);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task StoryMarksAnUnfinishedPreviousStepWithoutRevealingItsResult(bool previousCompleted)
    {
        LessonCatalog catalog = new();
        string html = await RenderAsync<InvestigationStory>(new()
        {
            [nameof(InvestigationStory.Lesson)] = catalog.Lessons[17],
            [nameof(InvestigationStory.PreviousLesson)] = catalog.Lessons[16],
            [nameof(InvestigationStory.PreviousCompleted)] = previousCompleted
        });
        Assert.Equal(!previousCompleted, html.Contains("Vorherige Spur noch offen", StringComparison.Ordinal));
        Assert.Contains(catalog.Lessons[17].Story, html);
        Assert.DoesNotContain(catalog.Lessons[16].Discovery, html);
        Assert.DoesNotContain(catalog.Lessons[17].Discovery, html);
    }

    [Fact]
    public async Task DateTimeResultsShowTheSameFormatAsSqlExamplesIncludingMidnight()
    {
        QueryResult result = new() { Columns = ["Zeitpunkt"], Rows = [[new DateTime(2026, 10, 12, 20, 0, 0)], [new DateTime(2026, 10, 13)]] };
        string html = await RenderAsync<ResultTable>(new() { [nameof(ResultTable.Result)] = result });
        Assert.Contains("2026-10-12 20:00:00", html);
        Assert.Contains("2026-10-13 00:00:00", html);
    }

    private ProgressStore CreateStore() => new(new TestEnvironment { ContentRootPath = _directory });

    private async Task<string> RenderPlanAsync()
    {
        using ServiceProvider services = new ServiceCollection().AddLogging().AddSingleton(CreateStore()).AddSingleton<LessonCatalog>().AddSingleton<WorkshopCatalog>().AddSingleton<FiaeTrainer.Core.Algorithms.Content.AlgorithmCatalog>().AddSingleton<FiaeTrainer.Core.BusinessStudies.Content.BusinessCatalog>().AddSingleton<FiaeTrainer.Core.DatabaseDesign.Content.DesignCatalog>().BuildServiceProvider();
        return await RenderAsync<LearningPlan>([], services);
    }

    /// <summary> Rendert die echte Razor-Komponente mit dem angegebenen Testzustand. </summary>
    private static async Task<string> RenderAsync<T>(Dictionary<string, object?> parameters, IServiceProvider? services = null) where T : IComponent
    {
        using ServiceProvider defaults = new ServiceCollection().AddLogging().BuildServiceProvider();
        services ??= defaults;
        await using HtmlRenderer renderer = new(services, services.GetRequiredService<ILoggerFactory>());
        return await renderer.Dispatcher.InvokeAsync(async () => WebUtility.HtmlDecode((await renderer.RenderComponentAsync<T>(ParameterView.FromDictionary(parameters))).ToHtmlString()));
    }

    /// <summary> Entfernt ausschließlich das pro Test neu angelegte Profilverzeichnis. </summary>
    public void Dispose()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, true);
    }

    /// <summary> Minimale Hostumgebung für das eigene temporäre Profil. </summary>
    private sealed class TestEnvironment : IWebHostEnvironment
    {
        /// <summary> Nicht benötigter Webpfad. </summary>
        public string WebRootPath { get; set; } = "";
        /// <summary> Keine Webdateien im Testprofil. </summary>
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        /// <summary> Ausschließlich das neu erzeugte Testverzeichnis. </summary>
        public string ContentRootPath { get; set; } = "";
        /// <summary> Keine eingebundenen Inhaltsdateien. </summary>
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        /// <summary> Kennzeichnung des Testhosts. </summary>
        public string EnvironmentName { get; set; } = "Testing";
        /// <summary> Name des Testprojekts. </summary>
        public string ApplicationName { get; set; } = "FiaeTrainer.Tests";
    }
}
