using FiaeTrainer.Web.Networking;
using System.Net;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft die explizite LAN-Freigabe, private Adressgrenzen und das weiterhin geschlossene lokale Profil. </summary>
public sealed class NetworkAccessPolicyTests
{
    [Theory]
    [InlineData("127.0.0.1")]
    [InlineData("127.23.4.5")]
    [InlineData("::1")]
    [InlineData("::ffff:127.0.0.1")]
    public void LoopbackWorksInBothModes(string address)
    {
        Assert.True(new NetworkAccessPolicy(false).Allows(IPAddress.Parse(address)));
        Assert.True(new NetworkAccessPolicy(true).Allows(IPAddress.Parse(address)));
    }

    [Theory]
    [InlineData("10.0.0.1")]
    [InlineData("10.255.255.254")]
    [InlineData("172.16.0.1")]
    [InlineData("172.31.255.254")]
    [InlineData("192.168.1.21")]
    [InlineData("192.168.255.254")]
    [InlineData("::ffff:192.168.1.20")]
    [InlineData("fc00::1")]
    [InlineData("fdff:1234::1")]
    public void PrivateClientsRequireLanMode(string address)
    {
        Assert.False(new NetworkAccessPolicy(false).Allows(IPAddress.Parse(address)));
        Assert.True(new NetworkAccessPolicy(true).Allows(IPAddress.Parse(address)));
    }

    [Theory]
    [InlineData("8.8.8.8")]
    [InlineData("192.0.2.10")]
    [InlineData("172.15.255.254")]
    [InlineData("172.32.0.1")]
    [InlineData("192.169.1.1")]
    [InlineData("169.254.1.1")]
    [InlineData("100.64.0.1")]
    [InlineData("0.0.0.0")]
    [InlineData("255.255.255.255")]
    [InlineData("::ffff:8.8.8.8")]
    [InlineData("2001:db8::1")]
    [InlineData("fe80::1")]
    [InlineData("::")]
    public void PublicAndUnspecifiedClientsRemainBlocked(string address)
    {
        Assert.False(new NetworkAccessPolicy(false).Allows(IPAddress.Parse(address)));
        Assert.False(new NetworkAccessPolicy(true).Allows(IPAddress.Parse(address)));
    }

    [Fact]
    public void MissingClientAddressIsDenied()
    {
        Assert.False(new NetworkAccessPolicy(false).Allows(null));
        Assert.False(new NetworkAccessPolicy(true).Allows(null));
    }
}
