using FiaeTrainer.Web.Components.Learning;
using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Verhindert widersprüchliche Erfolgshinweise bei übernommenen Versuchen im Modus ohne Hilfe. </summary>
public sealed class AttemptOutcomeNoteTests
{
    [Theory]
    [InlineData(true, false, "ohne Hilfe gelöst", "übernommene Versuch")]
    [InlineData(true, true, "übernommene Versuch", "Für einen frischen Selbsttest wähle")]
    [InlineData(false, true, "im geführten Lernen gelöst", "übernommene Versuch")]
    public async Task SuccessNoteDistinguishesModeAndAttemptHistory(bool examMode, bool assistedAttempt, string expected, string unexpected)
    {
        using ServiceProvider services = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using HtmlRenderer renderer = new(services, services.GetRequiredService<ILoggerFactory>());
        string html = await renderer.Dispatcher.InvokeAsync(async () =>
        {
            ParameterView parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(AttemptOutcomeNote.ExamMode)] = examMode,
                [nameof(AttemptOutcomeNote.AssistedAttempt)] = assistedAttempt
            });
            var output = await renderer.RenderComponentAsync<AttemptOutcomeNote>(parameters);
            return output.ToHtmlString();
        });
        string text = WebUtility.HtmlDecode(html);
        Assert.Contains(expected, text);
        Assert.DoesNotContain(unexpected, text);
        if (examMode && assistedAttempt)
        {
            Assert.Contains("im Modus „Ohne Hilfe lösen“", text);
            Assert.Contains("Neu ohne Hilfe starten", text);
            Assert.Contains("dein Abschluss bleibt erhalten", text);
        }
    }
}
