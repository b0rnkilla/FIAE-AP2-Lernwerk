using System.Net;
using FiaeTrainer.Core.Algorithms.Content;
using FiaeTrainer.Web.Components.Learning;
using FiaeTrainer.Web.Components.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Sichtbare Verträge unterscheiden Text-Rückgabe und Tabellenaktionen; der Spickzettel erklärt beide. </summary>
public sealed class AlgorithmTextComponentTests
{
    [Theory]
    [InlineData(31)]
    [InlineData(32)]
    public async Task TextContractShowsOptionalTypesAndTheLargerBudget(int id)
    {
        string html = await RenderAsync<AlgorithmFunctionContract>(new() { ["Lesson"] = new AlgorithmCatalog().Lessons[id - 1] });
        Assert.Contains("Typangaben freiwillig", html);
        Assert.Contains("100.000", html);
        Assert.Contains("500", html);
        Assert.Contains("Gib das Ergebnis mit", html);
        Assert.Contains("text: String): String", html);
    }

    [Fact]
    public async Task TableContractExplainsActionsInsteadOfDemandingAResult()
    {
        string html = await RenderAsync<AlgorithmFunctionContract>(new() { ["Lesson"] = new AlgorithmCatalog().Lessons[32] });
        Assert.Contains("FUNKTION output(): Void", html);
        Assert.Contains("ohne Argumente", html);
        Assert.Contains("kein Rückgabewert", html);
        Assert.DoesNotContain("Gib das Ergebnis mit", html);
        Assert.DoesNotContain("100.000", html);
    }

    [Fact]
    public async Task ReferenceExplainsNewSyntaxWithoutPublishingTheTaskSolutions()
    {
        string html = await RenderAsync<AlgorithmReference>([]);
        foreach (string fragment in new[] { "ALS_TEXT", "ALS_GANZZAHL", "VERLASSE SCHLEIFE", "KeinRückgabewert", "hole_daten()", "16.000", "100.000" })
            Assert.Contains(fragment, html);
        Assert.DoesNotContain("FUNKTION komprimiere", html);
        Assert.DoesNotContain("FUNKTION dekomprimiere", html);
        Assert.DoesNotContain("FUNKTION output", html);
        Assert.DoesNotContain("geben einen Wert zurück", html);
    }

    private static async Task<string> RenderAsync<T>(Dictionary<string, object?> parameters) where T : IComponent
    {
        using ServiceProvider services = new ServiceCollection().AddLogging().AddSingleton<NavigationManager>(new TestNavigation()).BuildServiceProvider();
        await using HtmlRenderer renderer = new(services, services.GetRequiredService<ILoggerFactory>());
        return await renderer.Dispatcher.InvokeAsync(async () => WebUtility.HtmlDecode((await renderer.RenderComponentAsync<T>(ParameterView.FromDictionary(parameters))).ToHtmlString()));
    }

    private sealed class TestNavigation : NavigationManager
    {
        public TestNavigation() => Initialize("http://localhost/", "http://localhost/algorithmen/spickzettel");
        protected override void NavigateToCore(string uri, bool forceLoad) => throw new NotSupportedException();
    }
}
