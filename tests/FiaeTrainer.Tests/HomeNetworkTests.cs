using System.Net;
using System.Text.Json.Nodes;
using FiaeTrainer.Web.Configuration;
using FiaeTrainer.Web.Networking;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Heimnetz-Wechsel erhalten private Einstellungen und benötigen eine vollständige bestätigte Freigabe. </summary>
public sealed class HomeNetworkTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-Heimnetz", Guid.NewGuid().ToString("N"));

    [Theory]
    [InlineData("127.0.0.1", "localhost:5213", true)]
    [InlineData("::ffff:127.0.0.1", "127.0.0.1:5213", true)]
    [InlineData("::1", "[::1]:5213", true)]
    [InlineData("127.0.0.1", "fremde-seite.example:5213", false)]
    [InlineData("192.168.1.14", "localhost:5213", false)]
    [InlineData("192.168.1.14", "192.168.1.21:5213", false)]
    public void OnlyDirectLoopbackWithLocalHostMayManage(string ip, string host, bool allowed)
    {
        DefaultHttpContext context = new();
        context.Connection.RemoteIpAddress = IPAddress.Parse(ip);
        context.Request.Host = new HostString(host);
        context.Request.Headers["X-Forwarded-For"] = "127.0.0.1";
        Assert.Equal(allowed, HomeNetworkBinding.CanManage(context));
    }

    [Theory]
    [InlineData("http://localhost:5213", 5213)]
    [InlineData("http://0.0.0.0:5080", 5080)]
    [InlineData("http://[::]:5180", 5180)]
    public void PortRemainsStableAcrossModes(string address, int port)
    {
        Assert.Equal(port, HomeNetworkBinding.Port(address));
        Assert.Equal($"http://localhost:{port}", HomeNetworkBinding.Url(port, false));
        Assert.Equal($"http://0.0.0.0:{port}", HomeNetworkBinding.Url(port, true));
    }

    [Theory]
    [InlineData("https://localhost:5213")]
    [InlineData("http://localhost:5213;http://localhost:5214")]
    [InlineData("http://localhost:0")]
    [InlineData("kein-endpunkt")]
    public void AmbiguousOrUnsupportedBindingsAreNotRewritten(string address)
        => Assert.Throws<InvalidOperationException>(() => HomeNetworkBinding.Port(address));

    [Fact]
    public async Task ExistingFirewallAvoidsElevationAndPersistsModeWithoutChangingOtherSettings()
    {
        var (service, firewall, settings, config) = Create();
        Directory.CreateDirectory(_directory);
        await File.WriteAllTextAsync(settings.FilePath, """{"ConnectionStrings":{"Training":"private"},"Updates":{"Enabled":false},"Other":{"Value":42}}""");
        Assert.Null(await service.PrepareAsync(true));
        Assert.Equal(0, firewall.Calls);
        JsonObject stored = JsonNode.Parse(await File.ReadAllTextAsync(settings.FilePath))!.AsObject();
        Assert.True(stored["Network"]!["AllowLan"]!.GetValue<bool>());
        Assert.Equal("http://0.0.0.0:5213", stored["Urls"]!.GetValue<string>());
        Assert.Equal("private", stored["ConnectionStrings"]!["Training"]!.GetValue<string>());
        Assert.False(stored["Updates"]!["Enabled"]!.GetValue<bool>());
        Assert.Equal(42, stored["Other"]!["Value"]!.GetValue<int>());
        Assert.False(service.Enabled); // Die laufende Instanz ändert sich erst beim kontrollierten Neustart.
        Assert.Equal("http://localhost:5213", config["Urls"]);
    }

    [Fact]
    public async Task MissingRuleIsConfirmedAgainAfterTheWindowsAction()
    {
        var (service, firewall, settings, _) = Create();
        firewall.Ready = false;
        Assert.Null(await service.PrepareAsync(true));
        Assert.Equal(1, firewall.Calls);
        Assert.True(File.Exists(settings.FilePath));
    }

    [Theory]
    [InlineData("cancel")]
    [InlineData("no-private-network")]
    [InlineData("diagnostic-error")]
    [InlineData("rule-still-missing")]
    public async Task FailedSetupNeverSavesTheNewMode(string failure)
    {
        var (service, firewall, settings, _) = Create();
        firewall.Ready = false;
        firewall.Error = failure == "cancel" ? "Windows-Bestätigung abgebrochen" : null;
        firewall.HasPrivateNetwork = failure != "no-private-network";
        firewall.StatusError = failure == "diagnostic-error" ? "Diagnose fehlgeschlagen" : null;
        firewall.RemainUnready = failure == "rule-still-missing";
        Assert.NotNull(await service.PrepareAsync(true));
        Assert.False(File.Exists(settings.FilePath));
        Assert.Equal(failure is "cancel" or "rule-still-missing" ? 1 : 0, firewall.Calls);
    }

    [Fact]
    public async Task DisableNeedsNoElevationEvenWhenNetworkIsGone()
    {
        var (service, firewall, settings, _) = Create(true);
        firewall.HasPrivateNetwork = false;
        Assert.Null(await service.PrepareAsync(false));
        Assert.Equal(0, firewall.Calls);
        JsonNode stored = JsonNode.Parse(await File.ReadAllTextAsync(settings.FilePath))!;
        Assert.False(stored["Network"]!["AllowLan"]!.GetValue<bool>());
        Assert.Equal("http://localhost:5213", stored["Urls"]!.GetValue<string>());
    }

    [Fact]
    public async Task BrokenSettingsStayByteForByteAndNoProgressFileIsTouched()
    {
        var (service, _, settings, _) = Create();
        Directory.CreateDirectory(_directory);
        await File.WriteAllTextAsync(settings.FilePath, "{broken");
        await File.WriteAllTextAsync(Path.Combine(_directory, "progress.json"), "private-learning-progress");
        await Assert.ThrowsAnyAsync<System.Text.Json.JsonException>(() => service.PrepareAsync(false));
        Assert.Equal("{broken", await File.ReadAllTextAsync(settings.FilePath));
        Assert.Equal("private-learning-progress", await File.ReadAllTextAsync(Path.Combine(_directory, "progress.json")));
    }

    [Fact]
    public void QrCodeIsGeneratedLocallyAsPng()
    {
        string image = HomeNetworkService.QrImage("http://192.168.1.21:5213/");
        Assert.StartsWith("data:image/png;base64,", image);
        byte[] png = Convert.FromBase64String(image.Split(',')[1]);
        Assert.Equal(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }, png[..8]);
    }

    [Fact]
    public async Task HttpEndpointRejectsRemoteForgedHostMissingTokenAndBadModeBeforeChangingAnything()
    {
        var (service, firewall, settings, _) = Create();
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Configuration["AllowedHosts"] = "*";
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddAntiforgery();
        builder.Services.AddSingleton(service);
        NetworkRestart restart = new();
        builder.Services.AddSingleton(restart);
        builder.Services.AddSingleton(new NetworkAccessPolicy(false));
        await using WebApplication app = builder.Build();
        // Ausschließlich dieser Testhost simuliert die tatsächliche TCP-Gegenstelle.
        app.Use(async (context, next) => { if (context.Request.Headers["X-Test-Remote"] == "true") context.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.14"); await next(); });
        app.MapHomeNetwork();
        app.MapGet("/token", (HttpContext context, IAntiforgery anti) => anti.GetAndStoreTokens(context).RequestToken!);
        await app.StartAsync();
        string address = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
        using HttpClient client = new(new HttpClientHandler { UseCookies = true }) { BaseAddress = new Uri(address) };
        string token = await client.GetStringAsync("/token");
        using var missing = await client.PostAsync("/heimnetz/umschalten", new FormUrlEncodedContent(new Dictionary<string, string> { ["mode"] = "enable" }));
        Assert.Equal(HttpStatusCode.BadRequest, missing.StatusCode);
        foreach (bool remote in new[] { true, false })
        {
            using HttpRequestMessage forged = new(HttpMethod.Post, "/heimnetz/umschalten") { Content = Form(token, "enable") };
            if (remote) forged.Headers.Add("X-Test-Remote", "true");
            else forged.Headers.Host = "fremde-seite.example";
            using HttpResponseMessage response = await client.SendAsync(forged);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        using var invalid = await client.PostAsync("/heimnetz/umschalten", Form(token, "fremder-befehl"));
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        Assert.False(File.Exists(settings.FilePath)); Assert.Equal(0, firewall.Calls);
        using var accepted = await client.PostAsync("/heimnetz/umschalten", Form(token, "enable"));
        Assert.Equal(HttpStatusCode.OK, accepted.StatusCode);
        Assert.Contains("Das Lernwerk startet neu", await accepted.Content.ReadAsStringAsync());
        Assert.True(restart.Requested); Assert.True(restart.NextMode);
        Assert.True(File.Exists(settings.FilePath));
        using var notReady = await client.GetAsync("/heimnetz/bereit");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, notReady.StatusCode);
        using var repeated = await client.PostAsync("/heimnetz/umschalten", Form(token, "disable"));
        Assert.Equal(HttpStatusCode.Conflict, repeated.StatusCode);
        await app.WaitForShutdownAsync(new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
        restart.BeginHost(); Assert.False(restart.Requested); Assert.Equal(1, restart.Gate.CurrentCount);
    }

    /// <summary> Erstellt ein echtes Browserformular einschließlich Antiforgery-Token. </summary>
    private static FormUrlEncodedContent Form(string token, string mode) => new(new Dictionary<string, string> { ["__RequestVerificationToken"] = token, ["mode"] = mode });

    [Fact]
    public async Task NewReleaseLoadsSavedModeFromTheSamePersonalSettingsFile()
    {
        var (service, _, settings, _) = Create();
        Assert.Null(await service.PrepareAsync(true));
        using ConfigurationManager next = new();
        next["LocalRelease:SettingsFile"] = settings.FilePath;
        next["Urls"] = "http://localhost:5080";
        next["Network:AllowLan"] = "false";
        LocalReleaseConfiguration.Apply(next, Path.Combine(_directory, "unused"), []);
        Assert.True(next.GetValue<bool>("Network:AllowLan"));
        Assert.Equal("http://0.0.0.0:5213", next["Urls"]);
        Assert.Equal("*", next["AllowedHosts"]);
    }

    private (HomeNetworkService, FakeFirewall, LocalSettingsStore, ConfigurationManager) Create(bool enabled = false)
    {
        ConfigurationManager config = new();
        config["Urls"] = "http://localhost:5213";
        config["LocalRelease:SettingsFile"] = Path.Combine(_directory, "settings.json");
        NetworkAccessPolicy access = new(enabled);
        LocalSettingsStore settings = new(config, new EnvironmentStub(), access);
        FakeFirewall firewall = new();
        return (new HomeNetworkService(config, access, firewall, settings), firewall, settings, config);
    }

    public void Dispose() { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }

    private sealed class FakeFirewall : WindowsHomeFirewall
    {
        public bool Ready = true, HasPrivateNetwork = true, RemainUnready;
        public int Calls;
        public string? Error, StatusError;
        public override Task<HomeFirewallStatus> StatusAsync(int port) => Task.FromResult(new HomeFirewallStatus(true, Ready, Ready, HasPrivateNetwork ? [new("192.168.1.21", "Test-Heimnetz")] : [], StatusError));
        public override Task<string?> ConfigureAsync(int port, bool remove = false) { Calls++; Ready = Error == null && !RemainUnready; return Task.FromResult(Error); }
    }

    private sealed class EnvironmentStub : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests"; public string EnvironmentName { get; set; } = "Tests"; public string ContentRootPath { get; set; } = ""; public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider(); public string WebRootPath { get; set; } = ""; public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
