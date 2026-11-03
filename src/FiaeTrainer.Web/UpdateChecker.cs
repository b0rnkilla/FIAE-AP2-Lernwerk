using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FiaeTrainer.Web;

/// <summary> Liest nur veröffentlichte GitHub-Releases; Offlinefehler blockieren die Lernfunktionen nicht. </summary>
public sealed class UpdateChecker(IHttpClientFactory clients, IConfiguration configuration, AppVersionInfo currentVersion)
{
    /// <summary> Verhindert parallele Prüfungen und unnötige API-Aufrufe. </summary>
    private readonly SemaphoreSlim _gate = new(1, 1);
    /// <summary> Zeitpunkt der letzten Anfrage für eine kurze Wiederholungssperre. </summary>
    private DateTimeOffset _lastAttempt;
    /// <summary> Zuletzt tatsächlich geprüfte Quelle. </summary>
    private string? _lastRepository;
    /// <summary> Aktueller Prüfzustand. </summary>
    public UpdateCheckStatus Status { get; private set; } = UpdateCheckStatus.NotConfigured;
    /// <summary> Deutsche Rückmeldung ohne vertrauliche HTTP-Details. </summary>
    public string Message { get; private set; } = "Die Updateprüfung wird vorbereitet.";
    /// <summary> Gefundene neuere Version. </summary>
    public string? AvailableVersion { get; private set; }
    /// <summary> Ausschließlich ein Link zur Release-Seite, kein automatischer Download. </summary>
    public string? DownloadPage { get; private set; }
    /// <summary> Zeitpunkt der abgeschlossenen Prüfung. </summary>
    public DateTimeOffset? LastChecked { get; private set; }
    /// <summary> Persönliche Wahl zur automatischen Startprüfung. </summary>
    public bool Enabled => configuration.GetValue("Updates:Enabled", true);
    /// <summary> Geprüfte öffentliche Repositoryadresse. </summary>
    public GitHubRepository? Repository => GitHubRepository.Parse(configuration["Updates:Repository"]);
    /// <summary> Aktualisiert angeschlossene Blazor-Komponenten nach Zustandswechseln. </summary>
    public event Action? Changed;

    /// <summary> Begrenzt Anfragen auf einmal pro Minute und Quelle. Vorabversionen aktualisieren auch auf neuere Vorabversionen. </summary>
    public async Task CheckAsync(CancellationToken cancellationToken = default)
    {
        if (!await _gate.WaitAsync(0, cancellationToken))
            return;
        bool requested = false;
        try
        {
            if (!Enabled)
            {
                _lastAttempt = DateTimeOffset.MinValue;
                SetStatus(UpdateCheckStatus.Disabled, "Die automatische Updateprüfung ist ausgeschaltet.");
                return;
            }
            if (string.IsNullOrWhiteSpace(configuration["Updates:Repository"]))
            {
                SetStatus(UpdateCheckStatus.NotConfigured, "Es ist noch kein öffentliches GitHub-Repository hinterlegt.");
                return;
            }
            GitHubRepository? repository = Repository;
            if (repository == null || currentVersion.Parsed == null)
            {
                SetStatus(UpdateCheckStatus.Unavailable, "Updatequelle oder aktuelle Versionsnummer kann nicht ausgewertet werden.");
                return;
            }
            if (_lastRepository == repository.WebAddress && DateTimeOffset.UtcNow - _lastAttempt < TimeSpan.FromMinutes(1))
                return;
            _lastRepository = repository.WebAddress;
            _lastAttempt = DateTimeOffset.UtcNow;
            requested = true;
            SetStatus(UpdateCheckStatus.Checking, "GitHub-Releases werden geprüft …");
            using HttpClient client = clients.CreateClient("GitHubUpdates");
            using HttpResponseMessage response = await client.GetAsync(repository.ApiAddress, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                SetStatus(UpdateCheckStatus.Unavailable, "Das Repository ist noch nicht öffentlich erreichbar. Du kannst uneingeschränkt weiterlernen.");
                return;
            }
            if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.TooManyRequests)
            {
                SetStatus(UpdateCheckStatus.Unavailable, "GitHub begrenzt gerade die Abfragen. Versuche es später erneut; Lernen bleibt möglich.");
                return;
            }
            response.EnsureSuccessStatusCode();
            GitHubRelease[] releases = await response.Content.ReadFromJsonAsync<GitHubRelease[]>(cancellationToken) ?? throw new JsonException("Release-Liste fehlt.");
            var candidates = releases.Where(x => !x.Draft && x.PublishedAt != null && x.Tag != null)
                .Select(x => new { Release = x, Version = SemanticVersion.Parse(x.Tag) })
                .Where(x => x.Version != null && (currentVersion.IsPrerelease || !x.Release.Prerelease && !x.Version.IsPrerelease)
                    && x.Release.Assets?.Any(a => a.State == "uploaded" && a.Name?.EndsWith("-win-x64.zip", StringComparison.OrdinalIgnoreCase) == true) == true)
                .OrderByDescending(x => x.Version).ToList();
            if (candidates.Count == 0)
            {
                SetStatus(UpdateCheckStatus.NoReleases, "Noch kein auswertbares Windows-Release mit ZIP-Download veröffentlicht.");
                return;
            }
            var latest = candidates[0];
            if (latest.Version is SemanticVersion version && version.CompareTo(currentVersion.Parsed) > 0)
            {
                AvailableVersion = version.Display;
                DownloadPage = repository.ReleaseAddress(latest.Release.Tag ?? "");
                Status = UpdateCheckStatus.Available;
                Message = $"Version {AvailableVersion} ist verfügbar.";
                Changed?.Invoke();
            }
            else
                SetStatus(UpdateCheckStatus.UpToDate, "Für deinen Versionskanal wurde keine neuere veröffentlichte Windows-Version gefunden.");
        }
        catch (Exception exception) when (exception is HttpRequestException or OperationCanceledException or JsonException or NotSupportedException)
        {
            SetStatus(UpdateCheckStatus.Unavailable, "Updateprüfung gerade nicht möglich. Prüfe später erneut; deine Lernfunktionen bleiben verfügbar.");
        }
        finally
        {
            if (requested)
                LastChecked = DateTimeOffset.UtcNow;
            _gate.Release();
            Changed?.Invoke();
        }
    }

    private void SetStatus(UpdateCheckStatus status, string message)
    {
        Status = status;
        Message = message;
        AvailableVersion = null;
        DownloadPage = null;
        Changed?.Invoke();
    }

    /// <summary> Benötigte öffentliche Release-Metadaten; Bodytexte und fremde Downloadlinks werden nicht verwendet. </summary>
    private sealed class GitHubRelease
    {
        /// <summary> Auswertbarer Versions-Tag. </summary>
        [JsonPropertyName("tag_name")] public string? Tag { get; set; }
        /// <summary> Noch nicht freigegebener Entwurf. </summary>
        [JsonPropertyName("draft")] public bool Draft { get; set; }
        /// <summary> GitHubs zusätzliche Vorabkennzeichnung. </summary>
        [JsonPropertyName("prerelease")] public bool Prerelease { get; set; }
        /// <summary> Nur tatsächlich veröffentlichte Releases werden berücksichtigt. </summary>
        [JsonPropertyName("published_at")] public DateTimeOffset? PublishedAt { get; set; }
        /// <summary> Angefügte Downloadpakete, keine automatisch erzeugten Quellcodearchive. </summary>
        [JsonPropertyName("assets")] public GitHubAsset[]? Assets { get; set; }
    }

    /// <summary> Prüft nur, ob ein fertiges Windows-ZIP vorhanden ist. </summary>
    private sealed class GitHubAsset
    {
        /// <summary> Erwarteter Name des Windows-Pakets. </summary>
        [JsonPropertyName("name")] public string? Name { get; set; }
        /// <summary> Ein noch laufender Upload darf keinen Updatehinweis auslösen. </summary>
        [JsonPropertyName("state")] public string? State { get; set; }
    }
}

/// <summary> Unterscheidet einen fehlenden Release von einem fehlgeschlagenen Abruf. </summary>
public enum UpdateCheckStatus { NotConfigured, Disabled, Checking, NoReleases, UpToDate, Available, Unavailable }

/// <summary> Prüft im Hintergrund einmal pro Programmstart, ohne den Webstart auf das Internet warten zu lassen. </summary>
public sealed class UpdateCheckWorker(UpdateChecker checker) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => checker.CheckAsync(stoppingToken);
}
