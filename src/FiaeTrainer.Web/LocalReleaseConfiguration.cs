namespace FiaeTrainer.Web;

/// <summary> Trennt Einstellungen und Lernstände der lokalen Windows-Veröffentlichung vom austauschbaren Programmordner. </summary>
public static class LocalReleaseConfiguration
{
    /// <summary> Kennzeichnet ausschließlich veröffentlichte Pakete, nicht normale Entwicklungsbuilds. </summary>
    public const string SETTINGS_FILE_NAME = "appsettings.Release.json";

    /// <summary> Verwendet pro Windows-Benutzer einen festen Profilpfad; Umgebungsvariablen und Startparameter haben Vorrang. </summary>
    public static string Apply(ConfigurationManager configuration, string userDirectory, string[] args)
    {
        string settingsPath = configuration["LocalRelease:SettingsFile"] ?? Path.Combine(userDirectory, "appsettings.Local.json");
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Training:ProgressDirectory"] = Path.Combine(userDirectory, "progress"),
            ["LocalRelease:SettingsFile"] = settingsPath
        });
        configuration.AddJsonFile(settingsPath, optional: true, reloadOnChange: false)
            .AddEnvironmentVariables(prefix: "ASPNETCORE_")
            .AddEnvironmentVariables()
            .AddCommandLine(args);
        return settingsPath;
    }

    /// <summary> Öffnet die tatsächlich gebundene Adresse; Platzhalterbindungen werden im Browser zu localhost. </summary>
    public static string BrowserAddress(IEnumerable<string> addresses)
    {
        string address = addresses.FirstOrDefault() ?? "http://localhost:5080";
        if (Uri.TryCreate(address, UriKind.Absolute, out Uri? uri))
        {
            UriBuilder browser = new(uri);
            if (uri.Host is "0.0.0.0" or "[::]" or "::" or "*" or "+")
                browser.Host = "localhost";
            return browser.Uri.AbsoluteUri;
        }
        return "http://localhost:5080";
    }
}
