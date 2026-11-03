using Microsoft.Win32;

namespace FiaeTrainer.Web;

/// <summary> Liest registrierte lokale SQL-Instanzen, ohne Dienste zu ändern oder fremde Server auszuprobieren. </summary>
public sealed class SqlInstanceDiscovery
{
    /// <summary> Offizielle deutschsprachige Downloadseite; die App installiert nichts selbst. </summary>
    public const string DOWNLOAD_URL = "https://www.microsoft.com/de-de/sql-server/sql-server-downloads";

    /// <summary> Installationshinweise sind keine Garantie für Erreichbarkeit oder Berechtigungen. </summary>
    public IReadOnlyList<string> FindLocalInstances()
    {
        HashSet<string> instances = new(StringComparer.OrdinalIgnoreCase);
        if (!OperatingSystem.IsWindows())
            return [];
        foreach (RegistryView view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
        {
            try
            {
                using RegistryKey machine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);
                using RegistryKey? sql = machine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL");
                foreach (string name in sql?.GetValueNames() ?? [])
                    instances.Add(name.Equals("MSSQLSERVER", StringComparison.OrdinalIgnoreCase) ? "." : @".\" + name);
                using RegistryKey? localDb = machine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions");
                if (localDb?.SubKeyCount > 0)
                    instances.Add(@"(localdb)\MSSQLLocalDB");
            }
            catch (Exception exception) when (exception is UnauthorizedAccessException or System.Security.SecurityException or IOException)
            {
                // Ohne Leserecht bleibt die manuelle Eingabe nutzbar; fehlende Funde sind kein Installationsbeweis.
            }
        }
        return instances.Order(StringComparer.OrdinalIgnoreCase).ToArray();
    }
}
