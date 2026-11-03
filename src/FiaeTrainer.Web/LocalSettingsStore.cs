using System.Text.Json;
using System.Text.Json.Nodes;
using FiaeTrainer.Core;
using Microsoft.Data.SqlClient;

namespace FiaeTrainer.Web;

/// <summary> Speichert persönliche Einrichtung atomar, ohne andere Einstellungen oder Zugangsdaten anzuzeigen. </summary>
public sealed class LocalSettingsStore(IConfiguration configuration, IWebHostEnvironment environment, NetworkAccessPolicy networkAccess)
{
    /// <summary> Verhindert konkurrierendes Überschreiben persönlicher Einstellungen. </summary>
    private readonly SemaphoreSlim _gate = new(1, 1);
    /// <summary> Pfad der ausschließlich lokalen Einstellungsdatei. </summary>
    public string FilePath { get; } = configuration["LocalRelease:SettingsFile"] ?? Path.Combine(environment.ContentRootPath, "appsettings.Local.json");
    /// <summary> Die gemeinsame Heimnetz-Vorführung darf keine lokale Einrichtung verändern. </summary>
    public bool CanEdit => !networkAccess.AllowLan;
    /// <summary> Zeigt ausschließlich den Instanznamen, keine Anmeldedaten. </summary>
    public string DataSource
    {
        get
        {
            try { return new SqlConnectionStringBuilder(configuration.GetConnectionString("Training") ?? "").DataSource; }
            catch (ArgumentException) { return ""; }
        }
    }

    /// <summary> Erstellt eine begrenzte Windows-Anmeldung; Datenbankname und Verbindungsoptionen sind nicht frei eingebbar. </summary>
    public static string BuildConnectionString(string server)
    {
        if (string.IsNullOrWhiteSpace(server) || server.Length > 200 || server.Any(char.IsControl) || server.Contains(';'))
            throw new ArgumentException("Gib einen Server- oder Instanznamen ein, keinen vollständigen Connectionstring.");
        return new SqlConnectionStringBuilder
        {
            DataSource = server.Trim(), InitialCatalog = TrainingDatabase.DATABASE_NAME,
            IntegratedSecurity = true, Encrypt = true, TrustServerCertificate = true, ConnectTimeout = 5
        }.ConnectionString;
    }

    /// <summary> Speichert erst nach erfolgreicher Verbindungsprüfung den ausgewählten Server. </summary>
    public Task SaveConnectionAsync(string connectionString) => SaveValueAsync("ConnectionStrings", "Training", JsonValue.Create(connectionString));

    /// <summary> Merkt die persönliche Wahl zur automatischen Updateprüfung. </summary>
    public Task SaveUpdatePreferenceAsync(bool enabled) => SaveValueAsync("Updates", "Enabled", JsonValue.Create(enabled));

    private async Task SaveValueAsync(string section, string key, JsonNode? value)
    {
        if (!CanEdit)
            throw new InvalidOperationException("Starte den Trainer im lokalen Modus, um die Einrichtung zu ändern. Im Heimnetz-Modus ist sie gesperrt.");
        await _gate.WaitAsync();
        try
        {
            JsonObject root = File.Exists(FilePath) ? JsonNode.Parse(await File.ReadAllTextAsync(FilePath)) as JsonObject ?? throw new InvalidDataException("Die persönliche Einstellungsdatei enthält kein JSON-Objekt und wird nicht überschrieben.") : new JsonObject();
            if (root[section] != null && root[section] is not JsonObject)
                throw new InvalidDataException("Ein Einstellungsbereich ist beschädigt und wird nicht überschrieben.");
            JsonObject settings = root[section] as JsonObject ?? new JsonObject();
            if (root[section] == null)
                root[section] = settings;
            settings[key] = value;
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(FilePath)) ?? throw new InvalidOperationException("Der Einstellungspfad fehlt."));
            string temporary = FilePath + ".tmp";
            await File.WriteAllTextAsync(temporary, root.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            File.Move(temporary, FilePath, true);
            configuration[$"{section}:{key}"] = value?.ToString();
        }
        finally { _gate.Release(); }
    }
}
