using System.Diagnostics;
using System.Globalization;
using System.Net;
using FiaeTrainer.Core.Sql.Database;
using FiaeTrainer.Core.Sql.Investigation;
using FiaeTrainer.Core.Sql.Validation;
using FiaeTrainer.Core.Sql.Workshop;
using FiaeTrainer.Web.Components;
using FiaeTrainer.Web.Configuration;
using FiaeTrainer.Web.Learning;
using FiaeTrainer.Web.Networking;
using FiaeTrainer.Web.Sql;
using FiaeTrainer.Web.Updates;

string releaseSettings = Path.Combine(AppContext.BaseDirectory, LocalReleaseConfiguration.SETTINGS_FILE_NAME);
bool localRelease = File.Exists(releaseSettings);
WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = localRelease ? AppContext.BaseDirectory : null
});
if (localRelease)
{
    builder.Configuration.AddJsonFile(releaseSettings, optional: false, reloadOnChange: false);
    string profileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AP2-Lernwerk");
    LocalReleaseConfiguration.Apply(builder.Configuration, profileDirectory, args);
}
else
    builder.Configuration.AddJsonFile(builder.Configuration["LocalRelease:SettingsFile"] ?? "appsettings.Local.json", optional: true, reloadOnChange: false).AddEnvironmentVariables(prefix: "ASPNETCORE_").AddEnvironmentVariables().AddCommandLine(args);
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("de-DE");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("de-DE");
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddSingleton<LessonCatalog>();
builder.Services.AddSingleton<WorkshopCatalog>();
builder.Services.AddSingleton<WorkshopSqlValidator>();
builder.Services.AddSingleton<SqlQueryValidator>();
builder.Services.AddSingleton(new TrainingDatabase(builder.Configuration.GetConnectionString("Training") ?? throw new InvalidOperationException("Die Trainingsverbindung fehlt.")));
builder.Services.AddTransient<QueryEvaluator>(services => new(services.GetRequiredService<DatabaseState>().Database, services.GetRequiredService<SqlQueryValidator>()));
builder.Services.AddTransient<WorkshopEvaluator>(services => new(services.GetRequiredService<DatabaseState>().Database, services.GetRequiredService<WorkshopSqlValidator>()));
builder.Services.AddSingleton<DatabaseState>();
builder.Services.AddSingleton<ProgressStore>();
builder.Services.AddSingleton<LocalSettingsStore>();
builder.Services.AddSingleton<SqlInstanceDiscovery>();
builder.Services.AddSingleton(new AppVersionInfo());
builder.Services.AddSingleton<UpdateChecker>();
builder.Services.AddHttpClient("GitHubUpdates", client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
    client.MaxResponseContentBufferSize = 4 * 1024 * 1024;
    client.DefaultRequestHeaders.UserAgent.ParseAdd("AP2-Lernwerk/" + new AppVersionInfo().Display);
    client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2026-03-10");
});
builder.Services.AddHostedService<UpdateCheckWorker>();
NetworkAccessPolicy networkAccess = new(builder.Configuration.GetValue<bool>("Network:AllowLan"));
builder.Services.AddSingleton(networkAccess);
// Nur die Heimnetz-Vorführung aus dem Quellprojekt lädt das Buildmanifest; Release-Pakete nutzen veröffentlichte Dateien.
if (networkAccess.AllowLan && builder.Environment.IsEnvironment("LanDemo"))
    builder.WebHost.UseStaticWebAssets();

WebApplication app = builder.Build();
// Standard ist der lokale Rechner; die bestehende Heimnetz-Vorführung bleibt ausdrücklich zuschaltbar.
app.Use(async (context, next) =>
{
    if (!networkAccess.Allows(context.Connection.RemoteIpAddress))
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync(networkAccess.AllowLan ? "Dieser Trainer ist für den lokalen Computer und private Heimnetz-Adressen freigegeben." : "Dieser Trainer läuft im lokalen Modus. Starte für andere Rechner im Heimnetz Trainer-im-Netzwerk-starten.cmd.");
        return;
    }
    await next();
});
app.UseExceptionHandler("/fehler", createScopeForErrors: true);
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
await app.Services.GetRequiredService<DatabaseState>().ConnectAsync();
app.Lifetime.ApplicationStarted.Register(() =>
{
    if (localRelease)
    {
        app.Logger.LogInformation("AP2 Lernwerk läuft lokal: {Address}", LocalReleaseConfiguration.BrowserAddress(app.Urls));
        app.Logger.LogInformation("Lernstand: {ProgressDirectory} · Verbindungseinstellung: {SettingsFile}", builder.Configuration["Training:ProgressDirectory"], builder.Configuration["LocalRelease:SettingsFile"]);
        app.Logger.LogInformation("Zum Beenden Strg+C drücken oder dieses Fenster schließen.");
    }
    if (networkAccess.AllowLan)
    {
        try
        {
            IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName()).Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !IPAddress.IsLoopback(x) && networkAccess.Allows(x)).ToArray();
            foreach (string url in app.Urls)
                if (Uri.TryCreate(url, UriKind.Absolute, out Uri? endpoint))
                    foreach (IPAddress address in addresses)
                        app.Logger.LogInformation("Im Heimnetz erreichbar: {Scheme}://{Address}:{Port} · Gemeinsamer Lernstand ohne Anmeldung.", endpoint.Scheme, address, endpoint.Port);
        }
        catch (System.Net.Sockets.SocketException exception) { app.Logger.LogWarning(exception, "Die Heimnetz-Adresse konnte nicht ermittelt werden. Nutze die IPv4-Adresse dieses Rechners mit Port 5080."); }
    }
    if (builder.Configuration.GetValue<bool>("LocalRelease:OpenBrowser") || Environment.GetEnvironmentVariable("FIAE_OPEN_BROWSER") == "1")
    {
        string browserAddress = LocalReleaseConfiguration.BrowserAddress(app.Urls);
        try { Process.Start(new ProcessStartInfo(browserAddress) { UseShellExecute = true }); }
        catch (System.ComponentModel.Win32Exception exception) { app.Logger.LogWarning(exception, "Der Browser konnte nicht geöffnet werden. Öffne {Address} manuell.", browserAddress); }
    }
});
await app.RunAsync();
