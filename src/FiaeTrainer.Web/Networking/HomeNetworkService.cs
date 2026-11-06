using FiaeTrainer.Web.Configuration;
using QRCoder;

namespace FiaeTrainer.Web.Networking;

/// <summary> Führt Diagnose, eng begrenzte Firewall-Freigabe und atomare Betriebsartwahl zusammen. </summary>
public sealed class HomeNetworkService(IConfiguration configuration, NetworkAccessPolicy access, WindowsHomeFirewall firewall, LocalSettingsStore settings)
{
    /// <summary> Tatsächlich konfigurierte Portnummer; QA und abweichende Nutzerports bleiben erhalten. </summary>
    public int Port => HomeNetworkBinding.Port(configuration["Urls"] ?? "http://localhost:5080");
    /// <summary> Aktuell laufende Betriebsart, nicht eine noch ausstehende Einstellung. </summary>
    public bool Enabled => access.AllowLan;
    /// <summary> Fragt ausschließlich die Windows-Diagnose ab. </summary>
    public Task<HomeFirewallStatus> StatusAsync() => firewall.StatusAsync(Port);
    /// <summary> Richtet nur fehlende Regeln ein; vor Erfolg wird die Betriebsart nicht gespeichert. </summary>
    public async Task<string?> PrepareAsync(bool enabled)
    {
        if (enabled)
        {
            HomeFirewallStatus status = await StatusAsync();
            if (status.Error != null) return status.Error;
            if (!status.Supported) return "Die automatische Heimnetz-Freigabe benötigt Windows.";
            if (status.PrivateNetworks.Length == 0) return "Windows erkennt kein privates Heimnetz. Öffne die Windows-Netzwerkeinstellungen und kennzeichne dein vertrautes Heimnetz als privat.";
            if (!status.RuleReady)
            {
                string? error = await firewall.ConfigureAsync(Port);
                if (error != null) return error;
                status = await StatusAsync();
                if (!status.RuleReady || status.PrivateNetworks.Length == 0) return "Die Freigabe konnte nicht bestätigt werden. Eine Sicherheitssoftware oder Windows-Richtlinie verhindert möglicherweise die Einrichtung.";
            }
        }
        await settings.SaveNetworkPreferenceAsync(enabled, Port);
        return null;
    }
    /// <summary> QR-Code entsteht ausschließlich lokal und enthält nur die angezeigte Geräteadresse. </summary>
    public static string QrImage(string address)
    {
        using QRCodeGenerator generator = new();
        using QRCodeData data = generator.CreateQrCode(address, QRCodeGenerator.ECCLevel.M);
        using PngByteQRCode renderer = new(data);
        return "data:image/png;base64," + Convert.ToBase64String(renderer.GetGraphic(6));
    }
}
