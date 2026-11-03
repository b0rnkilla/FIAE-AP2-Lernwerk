using FiaeTrainer.Core;

namespace FiaeTrainer.Web;

/// <summary> Hält den Datenbankstatus bereit und erlaubt einen kontrollierten erneuten Verbindungsversuch. </summary>
public sealed class DatabaseState(TrainingDatabase database, ILogger<DatabaseState> logger, LocalSettingsStore settings)
{
    /// <summary> Neue Lernseiten erhalten diese geprüfte Verbindung; laufende Auswertungen behalten ihre eigene Instanz. </summary>
    public TrainingDatabase Database { get; private set; } = database;
    /// <summary> Verhindert gleichzeitig laufende Initialisierungen. </summary>
    private readonly SemaphoreSlim _gate = new(1, 1);
    /// <summary> Ist nach erfolgreicher Initialisierung wahr. </summary>
    public bool Ready { get; private set; }
    /// <summary> Deutsche Information bei fehlender Verbindung. </summary>
    public string? Error { get; private set; }

    /// <summary> Prüft die eigene Datenbank; technische Einzelheiten werden nur protokolliert. </summary>
    public async Task ConnectAsync()
    {
        await _gate.WaitAsync();
        try
        {
            await Database.InitializeAsync();
            Ready = true;
            Error = null;
        }
        catch (Exception exception) when (exception is Microsoft.Data.SqlClient.SqlException or InvalidOperationException or IOException or ArgumentException)
        {
            Ready = false;
            Error = "Die Trainingsdatenbank ist noch nicht erreichbar oder konnte nicht vorbereitet werden. Prüfe die SQL-Server-Instanz und die Verbindungseinstellung in appsettings.Local.json. Einzelheiten stehen im Anwendungsprotokoll.";
            logger.LogError(exception, "Die Trainingsdatenbank konnte nicht initialisiert werden.");
        }
        finally { _gate.Release(); }
    }

    /// <summary> Prüft und speichert eine gewählte Instanz, ohne eine bisher funktionierende Verbindung bei Fehlern zu verlieren. </summary>
    public async Task<string?> ConfigureAsync(string server)
    {
        if (!settings.CanEdit)
            return "Die Einrichtung ist während der Heimnetz-Vorführung gesperrt. Starte die App im lokalen Modus.";
        await _gate.WaitAsync();
        try
        {
            string connectionString = LocalSettingsStore.BuildConnectionString(server);
            TrainingDatabase candidate = new(connectionString);
            await candidate.InitializeAsync();
            await settings.SaveConnectionAsync(connectionString);
            Database = candidate;
            Ready = true;
            Error = null;
            return null;
        }
        catch (Microsoft.Data.SqlClient.SqlException exception)
        {
            logger.LogWarning(exception, "Die ausgewählte SQL-Verbindung konnte nicht eingerichtet werden.");
            return exception.Number is 18456 or 262 or 15247
                ? "Die Windows-Anmeldung hat keine ausreichenden SQL-Rechte. Sie muss die eigene Trainingsdatenbank anlegen und vorbereiten dürfen."
                : "Die Verbindung konnte nicht eingerichtet werden. Prüfe Instanzname, laufenden SQL-Dienst und Rechte deiner Windows-Anmeldung. Das bedeutet nicht automatisch, dass SQL Server fehlt.";
        }
        catch (ArgumentException exception) { return exception.Message; }
        catch (InvalidOperationException exception) { return exception.Message; }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
        {
            logger.LogWarning(exception, "Die persönliche SQL-Einstellung konnte nicht gespeichert werden.");
            return "Die persönliche Einstellungsdatei konnte nicht gespeichert werden. Eine beschädigte Datei wird nicht überschrieben; die bisherige Verbindung bleibt erhalten.";
        }
        finally { _gate.Release(); }
    }
}
