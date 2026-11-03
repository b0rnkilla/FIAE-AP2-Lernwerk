using System.Net;
using FiaeTrainer.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft die zugänglichen Bedienelemente der ausblendbaren allgemeinen Hilfe. </summary>
public sealed class SelfTestHelpTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DismissedHelpLeavesNoInlineRestoreControl(bool hidden)
    {
        using ServiceProvider services = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using HtmlRenderer renderer = new(services, services.GetRequiredService<ILoggerFactory>());
        string html = await renderer.Dispatcher.InvokeAsync(async () =>
        {
            RenderFragment content = builder => builder.AddContent(0, "So funktioniert dein Selbsttest");
            ParameterView parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(SelfTestHelp.Hidden)] = hidden,
                [nameof(SelfTestHelp.ChildContent)] = content
            });
            var output = await renderer.RenderComponentAsync<SelfTestHelp>(parameters);
            return output.ToHtmlString();
        });
        string text = WebUtility.HtmlDecode(html);
        if (hidden)
        {
            Assert.DoesNotContain("Selbsttest-Hinweis anzeigen", text);
            Assert.DoesNotContain("<button", text);
            Assert.DoesNotContain("So funktioniert dein Selbsttest", text);
        }
        else
        {
            Assert.Contains("So funktioniert dein Selbsttest", text);
            Assert.Contains("aria-label=\"Selbsttest-Hinweis ausblenden\"", text);
        }
    }
}
