using System.Net;
using FiaeTrainer.Core.BusinessStudies.Content;
using FiaeTrainer.Core.BusinessStudies.Evaluation;
using FiaeTrainer.Core.BusinessStudies.Practice;
using FiaeTrainer.Web.Components.BusinessStudies;
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

/// <summary> Prüft echte WiSo-Komponenten, verborgene Prüfungslösungen und den Spickzettel-Rückweg. </summary>
public sealed class BusinessComponentTests : IDisposable
{
    private readonly BusinessCatalog _catalog = new();
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-Wiso-Components", Guid.NewGuid().ToString("N"));

    [Theory]
    [InlineData(2, "radio")]
    [InlineData(3, "checkbox")]
    [InlineData(24, "TT.MM.JJJJ")]
    [InlineData(35, "HH:mm")]
    [InlineData(69, "Dezimalkomma")]
    [InlineData(66, "Bitte zuordnen")]
    [InlineData(65, "Schritt 1 nach unten")]
    public async Task EveryAnswerFormatHasLabelledKeyboardAccessibleControls(int id, string expected)
    {
        BusinessQuestion question = _catalog.Questions.Single(question => question.Id == id);
        BusinessAnswer answer = new() { Order = question.Options.Select(option => option.Id).Reverse().ToList() };
        string html = await RenderAsync<BusinessAnswerInput>(new()
        {
            [nameof(BusinessAnswerInput.Question)] = question,
            [nameof(BusinessAnswerInput.Answer)] = answer,
            [nameof(BusinessAnswerInput.Options)] = question.Options
        });
        Assert.Contains("<fieldset", html);
        Assert.Contains("<legend", html);
        Assert.Contains(expected, html);
        Assert.DoesNotContain("target=\"_blank\"", html);
        Assert.DoesNotContain("Richtige Antwort", html);
        foreach (BusinessField field in question.Fields)
            Assert.Contains($"for=\"answer-{id}-{field.Id}\"", html);
    }

    [Fact]
    public async Task ReferencePausesExamMarksLookupAndReturnsToExactRound()
    {
        ProgressStore store = Store();
        BusinessRound round = new() { Kind = BusinessRoundKind.Exam, QuestionIds = [2, 24], Index = 1, RunningSinceUtc = DateTimeOffset.UtcNow.AddMinutes(-5) };
        round.Answers[24] = new() { Values = new() { ["f1"] = "30.04.2026" } };
        await store.StartBusinessRoundAsync(round);
        using ServiceProvider services = Services();
        string html = await RenderAsync<BusinessReference>([], services);
        Assert.Contains("href=\"/wiso/runde\"", html);
        Assert.Contains("Zurück zu deiner Aufgabe", html);
        BusinessRound saved = (await store.LoadAsync()).BusinessRound!;
        Assert.True(saved.Paused);
        Assert.True(saved.UsedReference);
        Assert.Equal(1, saved.Index);
        Assert.Equal("30.04.2026", saved.Answers[24].Values["f1"]);
        Assert.InRange(saved.RemainingSeconds, 3299, 3300);
    }

    [Fact]
    public async Task ActiveExamDoesNotRenderSolutionsSourcesOrTrainingCheckButton()
    {
        ProgressStore store = Store();
        BusinessRound round = new() { Kind = BusinessRoundKind.Exam, QuestionIds = [2], RunningSinceUtc = DateTimeOffset.UtcNow };
        await store.StartBusinessRoundAsync(round);
        using ServiceProvider services = Services();
        string html = await RenderAsync<Business>([], services);
        Assert.Contains("Runde auswerten", html);
        Assert.Contains("Verbleibende Prüfungszeit", html);
        Assert.DoesNotContain("Antwort prüfen", html);
        Assert.DoesNotContain("Richtige Antwort", html);
        Assert.DoesNotContain("Fachliche Quellen", html);
        Assert.DoesNotContain("Alle Antwortmöglichkeiten verstehen", html);
    }

    [Fact]
    public async Task PausedExamConcealsQuestionAndUnpausesOnlyOnExplicitAction()
    {
        await Store().StartBusinessRoundAsync(new() { Kind = BusinessRoundKind.Exam, QuestionIds = [2] });
        using ServiceProvider services = Services();
        string html = await RenderAsync<Business>([], services);
        Assert.Contains("Eine Pause für den Kopf", html);
        Assert.Contains("Prüfungsrunde fortsetzen", html);
        Assert.DoesNotContain("Ein zulässiger Vertrag", html);
        Assert.DoesNotContain("Wähle eine Antwort", html);
    }

    [Fact]
    public async Task SubmittedExamShowsReadOnlyAnswersAndTopicResults()
    {
        ProgressStore store = Store();
        BusinessRound round = new() { Kind = BusinessRoundKind.Exam, QuestionIds = [2, 24], RunningSinceUtc = DateTimeOffset.UtcNow };
        round.Answers[2] = new() { SelectedIds = ["a"] };
        await store.StartBusinessRoundAsync(round);
        await store.SaveBusinessRoundAsync(round, _catalog, finish: true);
        using ServiceProvider services = Services();
        string html = await RenderAsync<Business>([], services);
        Assert.Contains("DEINE AUSWERTUNG", html);
        Assert.Contains("Ergebnis nach Themen", html);
        Assert.Contains("Richtige Antwort", html);
        Assert.Contains("fieldset class=\"business-answer\" disabled", html);
        Assert.DoesNotContain("Antwort prüfen", html);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(101)]
    [InlineData(102)]
    [InlineData(139)]
    [InlineData(140)]
    public async Task OwnIllustrationsHaveCompleteAccessibleDescription(int id)
    {
        BusinessQuestion question = _catalog.Questions.Single(question => question.Id == id);
        string html = await RenderAsync<BusinessIllustration>(new() { [nameof(BusinessIllustration.Question)] = question });
        Assert.Contains("role=\"img\"", html);
        Assert.Contains(question.IllustrationDescription, html);
        Assert.Contains("figcaption", html);
    }

    private ProgressStore Store() => new(new TestEnvironment(), new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Training:ProgressDirectory"] = _directory }).Build());
    private ServiceProvider Services() => new ServiceCollection().AddLogging().AddSingleton(Store()).AddSingleton(_catalog).BuildServiceProvider();
    private static async Task<string> RenderAsync<T>(Dictionary<string, object?> parameters, IServiceProvider? services = null) where T : IComponent
    {
        using ServiceProvider defaults = new ServiceCollection().AddLogging().BuildServiceProvider();
        services ??= defaults;
        await using HtmlRenderer renderer = new(services, services.GetRequiredService<ILoggerFactory>());
        return await renderer.Dispatcher.InvokeAsync(async () => WebUtility.HtmlDecode((await renderer.RenderComponentAsync<T>(ParameterView.FromDictionary(parameters))).ToHtmlString()));
    }
    public void Dispose() { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
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
