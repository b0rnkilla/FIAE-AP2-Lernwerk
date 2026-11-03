using FiaeTrainer.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft, dass die Moduswahl ihren Zustand auch für assistive Technik eindeutig ausgibt. </summary>
public sealed class LearningModePickerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ModeButtonsExposeExplicitAccessibleToggleStates(bool examMode)
    {
        using ServiceProvider services = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using HtmlRenderer renderer = new(services, services.GetRequiredService<ILoggerFactory>());
        string html = await renderer.Dispatcher.InvokeAsync(async () =>
        {
            ParameterView parameters = ParameterView.FromDictionary(new Dictionary<string, object?> { [nameof(LearningModePicker.ExamMode)] = examMode });
            var output = await renderer.RenderComponentAsync<LearningModePicker>(parameters);
            return output.ToHtmlString();
        });
        string[] buttons = html.Split("<button", StringSplitOptions.None);
        Assert.Equal(3, buttons.Length);
        Assert.Contains($"aria-pressed=\"{(examMode ? "true" : "false")}\"", buttons[1]);
        Assert.Contains($"aria-pressed=\"{(examMode ? "false" : "true")}\"", buttons[2]);
    }
}
