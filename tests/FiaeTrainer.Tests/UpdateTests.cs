using FiaeTrainer.Web.Components.Updates;
using FiaeTrainer.Web.Updates;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FiaeTrainer.Tests;

/// <summary> Prüft Versionsreihenfolge, öffentliche Updatequellen und GitHub-Fehler ohne echte Netzwerkaufrufe. </summary>
public sealed class UpdateTests
{
    [Theory]
    [InlineData("1.10.0", "1.9.9", 1)]
    [InlineData("1.0.0", "1.0.0-alpha.1", 1)]
    [InlineData("1.0.0-alpha.10", "1.0.0-alpha.2", 1)]
    [InlineData("1.0.0-alpha", "1.0.0-alpha.1", -1)]
    [InlineData("1.0.0-beta", "1.0.0-alpha.99", 1)]
    [InlineData("1.0.0-1", "1.0.0-alpha", -1)]
    [InlineData("v1.2.3+abc", "1.2.3+def", 0)]
    [InlineData("0.1.1-alpha.1", "0.1.0", 1)]
    public void VersionsFollowSemverInsteadOfAlphabeticalOrPublicationOrder(string left, string right, int expected)
        => Assert.Equal(expected, Math.Sign(Assert.IsType<SemanticVersion>(SemanticVersion.Parse(left)).CompareTo(SemanticVersion.Parse(right))));

    [Theory]
    [InlineData("1.0")]
    [InlineData("01.0.0")]
    [InlineData("1.0.0-alpha.01")]
    [InlineData("1.0.0-alpha..1")]
    [InlineData("release-1.0.0")]
    [InlineData("1.0.0+")]
    public void InvalidVersionsAreNotGuessed(string value) => Assert.Null(SemanticVersion.Parse(value));

    [Theory]
    [InlineData("http://github.com/owner/repo")]
    [InlineData("https://github.com.evil.example/owner/repo")]
    [InlineData("https://token@github.com/owner/repo")]
    [InlineData("https://github.com/owner/repo?token=secret")]
    [InlineData("https://github.com/owner/repo/releases")]
    [InlineData("owner/..")]
    [InlineData("https://localhost/owner/repo")]
    public void UpdateSourcesCannotContainCredentialsOrAnotherHost(string value) => Assert.Null(GitHubRepository.Parse(value));

    [Fact]
    public void RealRepositoryAndGitCloneAddressResolveToTheSamePublicApi()
    {
        GitHubRepository repository = Assert.IsType<GitHubRepository>(GitHubRepository.Parse("https://github.com/b0rnkilla/FIAE-AP2-Lernwerk.git"));
        Assert.Equal("https://api.github.com/repos/b0rnkilla/FIAE-AP2-Lernwerk/releases?per_page=100", repository.ApiAddress.AbsoluteUri);
        Assert.StartsWith("https://github.com/b0rnkilla/FIAE-AP2-Lernwerk/releases/tag/", repository.ReleaseAddress("https://evil.example"));
    }

    [Fact]
    public async Task NewestAlphaIsSelectedNumericallyAndOnlyTheReleasePageIsExposed()
    {
        var (checker, factory, _) = Create("0.1.1-alpha.1", [Release("v0.1.1-alpha.2", prerelease: true), Release("v0.1.1-alpha.10", prerelease: true)]);
        await checker.CheckAsync();
        Assert.Equal(UpdateCheckStatus.Available, checker.Status);
        Assert.Equal("0.1.1-alpha.10", checker.AvailableVersion);
        Assert.Equal("https://github.com/b0rnkilla/FIAE-AP2-Lernwerk/releases/tag/v0.1.1-alpha.10", checker.DownloadPage);
        Assert.Equal(1, factory.Handler.Requests);
        Assert.Equal("api.github.com", factory.Handler.Address?.Host);
        Assert.Null(factory.Handler.Authorization);
    }

    [Fact]
    public async Task StableChannelIgnoresDraftsUnfinishedAssetsAndPrereleases()
    {
        var (checker, _, _) = Create("1.0.0", [Release("v9.0.0", draft: true), Release("v3.0.0", hasAsset: false), Release("v2.0.0-alpha.1", prerelease: true), Release("v1.0.0")]);
        await checker.CheckAsync();
        Assert.Equal(UpdateCheckStatus.UpToDate, checker.Status);
        Assert.Null(checker.DownloadPage);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    [InlineData(HttpStatusCode.BadGateway)]
    public async Task HttpFailuresAreNotReportedAsAnUpToDateVersion(HttpStatusCode status)
    {
        var (checker, factory, _) = Create("1.0.0", []);
        factory.Handler.Status = status;
        await checker.CheckAsync();
        Assert.Equal(UpdateCheckStatus.Unavailable, checker.Status);
        Assert.Null(checker.DownloadPage);
    }

    [Fact]
    public async Task EmptyPublishedReleaseListIsDifferentFromAnOfflineError()
    {
        var (checker, _, _) = Create("1.0.0", []);
        await checker.CheckAsync();
        Assert.Equal(UpdateCheckStatus.NoReleases, checker.Status);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task BrokenJsonAndOfflineConnectionsRemainNonfatal(bool offline)
    {
        var (checker, factory, _) = Create("1.0.0", []);
        factory.Handler.Offline = offline;
        factory.Handler.Json = "{broken";
        await checker.CheckAsync();
        Assert.Equal(UpdateCheckStatus.Unavailable, checker.Status);
        Assert.Null(checker.DownloadPage);
    }

    [Fact]
    public async Task ShortRepeatedChecksUseTheExistingResultAndKeepTheActualTimestamp()
    {
        var (checker, factory, _) = Create("1.0.0", [Release("v1.1.0")]);
        await checker.CheckAsync();
        DateTimeOffset? firstCheck = checker.LastChecked;
        await checker.CheckAsync();
        Assert.Equal(1, factory.Handler.Requests);
        Assert.Equal(firstCheck, checker.LastChecked);
    }

    [Fact]
    public async Task DisabledOrMissingSourcesNeverContactGithubAndClearAnOldNotice()
    {
        var (checker, factory, config) = Create("1.0.0", [Release("v1.1.0")]);
        config["Updates:Repository"] = "";
        await checker.CheckAsync();
        Assert.Equal(UpdateCheckStatus.NotConfigured, checker.Status);
        Assert.Equal(0, factory.Handler.Requests);
        config["Updates:Repository"] = "b0rnkilla/FIAE-AP2-Lernwerk";
        await checker.CheckAsync();
        Assert.Equal(UpdateCheckStatus.Available, checker.Status);
        config["Updates:Enabled"] = "false";
        await checker.CheckAsync();
        Assert.Equal(UpdateCheckStatus.Disabled, checker.Status);
        Assert.Null(checker.AvailableVersion);
        Assert.Equal(1, factory.Handler.Requests);
    }

    [Fact]
    public async Task TimeoutDoesNotBlockLearningOrExposeADownloadLink()
    {
        var (checker, factory, _) = Create("1.0.0", []);
        factory.Handler.Timeout = true;
        await checker.CheckAsync();
        Assert.Equal(UpdateCheckStatus.Unavailable, checker.Status);
        Assert.Null(checker.DownloadPage);
    }

    [Fact]
    public async Task AvailableUpdateIsRenderedAsAnExplicitUserDownloadNotice()
    {
        var (checker, _, _) = Create("1.0.0", [Release("v1.1.0")]);
        await checker.CheckAsync();
        using ServiceProvider services = new ServiceCollection().AddLogging().AddSingleton(checker).BuildServiceProvider();
        await using HtmlRenderer renderer = new(services, services.GetRequiredService<ILoggerFactory>());
        string html = await renderer.Dispatcher.InvokeAsync(async () => (await renderer.RenderComponentAsync<UpdateNotice>(ParameterView.Empty)).ToHtmlString());
        string text = WebUtility.HtmlDecode(html);
        Assert.Contains("Neue Version 1.1.0 verfügbar.", text);
        Assert.Contains("https://github.com/b0rnkilla/FIAE-AP2-Lernwerk/releases/tag/v1.1.0", text);
        Assert.Contains("Release öffnen", text);
    }

    private static object Release(string tag, bool prerelease = false, bool draft = false, bool hasAsset = true) => new
    {
        tag_name = tag, prerelease, draft, published_at = "2026-11-03T10:00:00Z", html_url = "https://untrusted.example/ignored",
        assets = hasAsset ? new[] { new { name = "AP2-Lernwerk-win-x64.zip", state = "uploaded" } } : []
    };

    private static (UpdateChecker, FakeClientFactory, Microsoft.Extensions.Configuration.ConfigurationManager) Create(string version, object[] releases)
    {
        Microsoft.Extensions.Configuration.ConfigurationManager config = new();
        config["Updates:Repository"] = "b0rnkilla/FIAE-AP2-Lernwerk";
        config["Updates:Enabled"] = "true";
        FakeClientFactory factory = new() { Handler = new FakeHandler { Json = JsonSerializer.Serialize(releases) } };
        return (new UpdateChecker(factory, config, new AppVersionInfo(version)), factory, config);
    }

    /// <summary> Liefert ausschließlich künstliche Antworten, nie einen echten Internetclient. </summary>
    private sealed class FakeClientFactory : IHttpClientFactory
    {
        public required FakeHandler Handler { get; init; }
        public HttpClient CreateClient(string name) => new(Handler, disposeHandler: false);
    }

    /// <summary> Zeichnet nur API-Metadaten auf, ohne Netzwerk oder Zugangsschlüssel. </summary>
    private sealed class FakeHandler : HttpMessageHandler
    {
        public string Json { get; set; } = "[]";
        public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;
        public bool Offline { get; set; }
        public bool Timeout { get; set; }
        public int Requests { get; private set; }
        public Uri? Address { get; private set; }
        public string? Authorization { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests++;
            Address = request.RequestUri;
            Authorization = request.Headers.Authorization?.ToString();
            if (Offline)
                throw new HttpRequestException("Test: offline");
            if (Timeout)
                throw new TaskCanceledException("Test: timeout");
            return Task.FromResult(new HttpResponseMessage(Status) { Content = new StringContent(Json, Encoding.UTF8, "application/json") });
        }
    }
}
