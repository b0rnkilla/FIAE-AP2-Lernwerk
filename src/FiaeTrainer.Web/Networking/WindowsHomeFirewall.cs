using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;

namespace FiaeTrainer.Web.Networking;

/// <summary> Eine freigegebene private Schnittstelle mit ihrer erreichbaren IPv4-Adresse. </summary>
public sealed record HomeNetworkAddress(string Address, string Name);

/// <summary> Diagnose der Windows-Regel, ohne Einstellungen zu verändern. </summary>
public sealed record HomeFirewallStatus(bool Supported, bool RuleReady, bool RuleExists, HomeNetworkAddress[] PrivateNetworks, string? Error = null);

/// <summary> Prüft die Firewall ohne erhöhte Rechte; ausschließlich der feste Helfer erhält bei Bedarf eine Windows-Bestätigung. </summary>
public class WindowsHomeFirewall
{
    /// <summary> Ausgeführte EXE; bei Entwicklungsstarts ist dies der .NET-Host. </summary>
    public string ProgramPath { get; } = Environment.ProcessPath ?? "";
    /// <summary> Mit dem Build/Release ausgelieferter, fest definierter Helfer. </summary>
    private string Script => Path.Combine(AppContext.BaseDirectory, "Networking", "Heimnetz-Firewall.ps1");
    /// <summary> Unterstützt ausschließlich die eingebaute Windows-Firewall. </summary>
    public virtual async Task<HomeFirewallStatus> StatusAsync(int port)
    {
        if (!OperatingSystem.IsWindows())
            return new(false, false, false, [], "Die automatische Firewall-Einrichtung ist unter Windows verfügbar.");
        try
        {
            using Process process = new() { StartInfo = StartInfo("Status", port, false) };
            process.Start();
            Task<string> output = process.StandardOutput.ReadToEndAsync();
            Task<string> errors = process.StandardError.ReadToEndAsync();
            using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(20));
            try { await process.WaitForExitAsync(timeout.Token); }
            catch (OperationCanceledException) { process.Kill(true); throw new IOException("Die Windows-Netzwerkprüfung hat zu lange gedauert."); }
            string json = await output; await errors;
            if (process.ExitCode != 0)
                return new(true, false, false, [], "Die Windows-Firewall konnte nicht geprüft werden. Prüfe, ob ihr Dienst läuft.");
            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;
            HomeNetworkAddress[] addresses = root.GetProperty("PrivateNetworks").EnumerateArray().Select(x => new HomeNetworkAddress(x.GetProperty("Address").GetString()!, x.GetProperty("Name").GetString()!)).ToArray();
            return new(true, root.GetProperty("RuleReady").GetBoolean(), root.GetProperty("RuleExists").GetBoolean(), addresses);
        }
        catch (Exception exception) when (exception is IOException or Win32Exception or JsonException or InvalidOperationException)
        { return new(true, false, false, [], "Die Heimnetz-Prüfung ist fehlgeschlagen. Starte die App erneut und prüfe deine Windows-Netzwerkeinstellungen."); }
    }

    /// <summary> Fordert UAC ausschließlich für die eigene Port-/Programmregel an. Abbruch ändert keine App-Einstellung. </summary>
    public virtual async Task<string?> ConfigureAsync(int port, bool remove = false)
    {
        if (!OperatingSystem.IsWindows()) return "Die automatische Freigabe benötigt Windows.";
        try
        {
            using Process process = new() { StartInfo = StartInfo(remove ? "Remove" : "Enable", port, true) };
            process.Start();
            await process.WaitForExitAsync();
            return process.ExitCode switch { 0 => null, 6 => "Es wurde kein privates Heimnetz gefunden. Prüfe das Netzwerkprofil in Windows.", _ => "Windows konnte die Firewall-Freigabe nicht einrichten. Die bisherige Betriebsart bleibt erhalten." };
        }
        catch (Win32Exception exception) when (exception.NativeErrorCode == 1223)
        { return "Die Windows-Bestätigung wurde abgebrochen. Die bisherige Betriebsart bleibt erhalten."; }
        catch (Exception exception) when (exception is Win32Exception or InvalidOperationException or IOException)
        { return "Der Firewall-Helfer konnte nicht gestartet werden. Die bisherige Betriebsart bleibt erhalten."; }
    }

    /// <summary> Keine frei eingegebenen Befehle; Argumente sind feste Werte, ein geprüfter Port und lokale Dateipfade. </summary>
    private ProcessStartInfo StartInfo(string action, int port, bool elevated)
    {
        if (port is < 1 or > 65535 || !File.Exists(Script)) throw new IOException("Der Firewall-Helfer fehlt.");
        ProcessStartInfo start = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "WindowsPowerShell", "v1.0", "powershell.exe"))
        {
            UseShellExecute = elevated, CreateNoWindow = !elevated, WindowStyle = ProcessWindowStyle.Hidden,
            RedirectStandardOutput = !elevated, RedirectStandardError = !elevated
        };
        if (elevated) start.Verb = "runas";
        else { start.StandardOutputEncoding = System.Text.Encoding.UTF8; start.StandardErrorEncoding = System.Text.Encoding.UTF8; }
        foreach (string argument in new[] { "-NoProfile", "-NonInteractive", "-ExecutionPolicy", "Bypass", "-File", Script, "-Action", action, "-Port", port.ToString(System.Globalization.CultureInfo.InvariantCulture), "-ProgramPath", ProgramPath }) start.ArgumentList.Add(argument);
        return start;
    }
}
