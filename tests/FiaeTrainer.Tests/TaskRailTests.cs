using System.Net;
using FiaeTrainer.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft ausgeblendete Navigation und zugängliche Steuerung der Aufgabenliste. </summary>
public sealed class TaskRailTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CollapsedTaskListKeepsAToggleButHidesNavigation(bool collapsed)
    {
        using ServiceProvider services = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using HtmlRenderer renderer = new(services, services.GetRequiredService<ILoggerFactory>());
        string html = await renderer.Dispatcher.InvokeAsync(async () =>
        {
            RenderFragment content = builder => builder.AddContent(0, "Aufgabenauswahl");
            ParameterView parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(TaskRail.Collapsed)] = collapsed,
                [nameof(TaskRail.Id)] = "test-tasks",
                [nameof(TaskRail.ChildContent)] = content
            });
            var output = await renderer.RenderComponentAsync<TaskRail>(parameters);
            return output.ToHtmlString();
        });
        string text = WebUtility.HtmlDecode(html);
        Assert.Contains("aria-controls=\"test-tasks\"", text);
        Assert.Contains(collapsed ? "aria-expanded=\"false\"" : "aria-expanded=\"true\"", text);
        Assert.Contains(collapsed ? "Aufgaben anzeigen" : "Aufgaben ausblenden", text);
        Assert.Equal(collapsed, text.Contains(" hidden", StringComparison.Ordinal));
        Assert.DoesNotContain(">Aufgaben anzeigen<", text);
        Assert.DoesNotContain(">Aufgaben ausblenden<", text);
    }
}
