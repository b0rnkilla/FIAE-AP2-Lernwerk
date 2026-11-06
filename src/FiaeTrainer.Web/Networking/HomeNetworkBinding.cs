using System.Net;

namespace FiaeTrainer.Web.Networking;

/// <summary> Der Heimnetz-Schalter verwendet genau einen HTTP-Port und erhält abweichende Testports. </summary>
public static class HomeNetworkBinding
{
    /// <summary> Nur unmittelbar lokale Verbindungen dürfen die Freigabe verwalten; Header sind keine Berechtigung. </summary>
    public static bool IsHost(IPAddress? address) => address != null && IPAddress.IsLoopback(address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address);

    /// <summary> Loopback allein genügt nicht: fremde Hostnamen dürfen keine lokale Verwaltungsoberfläche erhalten. </summary>
    public static bool CanManage(HttpContext context) => IsHost(context.Connection.RemoteIpAddress) &&
        (context.Request.Host.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
         IPAddress.TryParse(context.Request.Host.Host.Trim('[', ']'), out IPAddress? host) && IsHost(host));

    /// <summary> Verhindert mehrdeutige Bindungen oder ein versehentliches Abschalten fremder HTTPS-Endpunkte. </summary>
    public static int Port(string? urls)
    {
        if (urls != null && !urls.Contains(';') && Uri.TryCreate(urls.Replace("*", "0.0.0.0").Replace("+", "0.0.0.0"), UriKind.Absolute, out Uri? uri) && uri.Scheme == "http" && uri.Port is > 0 and <= 65535)
            return uri.Port;
        throw new InvalidOperationException("Die Heimnetz-Einstellung benötigt eine einzelne HTTP-Adresse mit festem Port.");
    }

    /// <summary> Öffnet bei aktivierter Freigabe alle IPv4-Schnittstellen; die Firewall begrenzt auf private lokale Netze. </summary>
    public static string Url(int port, bool enabled) => enabled ? $"http://0.0.0.0:{port}" : $"http://localhost:{port}";
}
