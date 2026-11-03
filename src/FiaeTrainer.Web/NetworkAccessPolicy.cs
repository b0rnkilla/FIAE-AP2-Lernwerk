using System.Net;
using System.Net.Sockets;

namespace FiaeTrainer.Web;

/// <summary> Erlaubt im Heimnetz-Modus zusätzlich private LAN-Adressen; öffentliche Quelladressen bleiben ausgeschlossen. </summary>
public sealed class NetworkAccessPolicy(bool allowLan)
{
    /// <summary> Der ausdrücklich aktivierte Heimnetz-Modus. </summary>
    public bool AllowLan { get; } = allowLan;

    /// <summary> Prüft die tatsächliche Verbindungsadresse, unabhängig von ungeprüften Forwarded-Headern. </summary>
    public bool Allows(IPAddress? address)
    {
        if (address == null)
            return false;
        if (address.IsIPv4MappedToIPv6)
            address = address.MapToIPv4();
        if (IPAddress.IsLoopback(address))
            return true;
        if (!AllowLan)
            return false;

        byte[] bytes = address.GetAddressBytes();
        if (address.AddressFamily == AddressFamily.InterNetwork)
            return bytes[0] == 10 || bytes[0] == 172 && bytes[1] is >= 16 and <= 31 || bytes[0] == 192 && bytes[1] == 168;
        return address.AddressFamily == AddressFamily.InterNetworkV6 && (bytes[0] & 0xfe) == 0xfc;
    }
}
