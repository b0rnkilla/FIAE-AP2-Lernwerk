using FiaeTrainer.Web;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft stabile Benutzerprofile und Konfigurationsvorrang ohne den echten Benutzerordner zu verändern. </summary>
public sealed class LocalReleaseConfigurationTests : IDisposable
{
    /// <summary> Ausschließlich für diesen Test neu angelegter Profilpfad. </summary>
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-Release", Guid.NewGuid().ToString("N"));

    [Fact]
    public void MissingPersonalSettingsUseAStableUserProfile()
    {
        ConfigurationManager firstVersion = new();
        ConfigurationManager nextVersion = new();
        string path = LocalReleaseConfiguration.Apply(firstVersion, _directory, []);
        LocalReleaseConfiguration.Apply(nextVersion, _directory, []);
        Assert.Equal(Path.Combine(_directory, "appsettings.Local.json"), path);
        Assert.Equal(Path.Combine(_directory, "progress"), firstVersion["Training:ProgressDirectory"]);
        Assert.Equal(firstVersion["Training:ProgressDirectory"], nextVersion["Training:ProgressDirectory"]);
        Assert.False(Directory.Exists(_directory));
    }

    [Fact]
    public void ExplicitSettingsFileIsLoadedInsteadOfTheRealUserProfile()
    {
        Directory.CreateDirectory(_directory);
        string customFile = Path.Combine(_directory, "custom.Local.json");
        File.WriteAllText(customFile, """{ "Updates": { "Enabled": false } }""");
        using ConfigurationManager configuration = new();
        configuration["LocalRelease:SettingsFile"] = customFile;
        string selected = LocalReleaseConfiguration.Apply(configuration, Path.Combine(_directory, "unused"), []);
        Assert.Equal(customFile, selected);
        Assert.False(configuration.GetValue<bool>("Updates:Enabled"));
        Assert.False(Directory.Exists(Path.Combine(_directory, "unused")));
    }

    [Fact]
    public void PersonalConnectionAndCommandLineOverridesAreLoadedInOrder()
    {
        Directory.CreateDirectory(_directory);
        File.WriteAllText(Path.Combine(_directory, "appsettings.Local.json"), """
            { "ConnectionStrings": { "Training": "Server=TEST;Database=FIAE_AP2_Training" }, "LocalRelease": { "OpenBrowser": true } }
            """);
        ConfigurationManager configuration = new();
        string qaDirectory = Path.Combine(_directory, "isolated-qa");
        LocalReleaseConfiguration.Apply(configuration, _directory, ["--LocalRelease:OpenBrowser", "false", "--Training:ProgressDirectory", qaDirectory, "--urls", "http://localhost:5082"]);
        Assert.Equal("Server=TEST;Database=FIAE_AP2_Training", configuration.GetConnectionString("Training"));
        Assert.False(configuration.GetValue<bool>("LocalRelease:OpenBrowser"));
        Assert.Equal(qaDirectory, configuration["Training:ProgressDirectory"]);
        Assert.Equal("http://localhost:5082", configuration["urls"]);
    }

    [Theory]
    [InlineData("http://localhost:5091", false)]
    [InlineData("http://localhost:5092", true)]
    public void StandardHostingEnvironmentOverridesDefaultsButNotCommandLine(string address, bool useCommandLine)
    {
        string? previous = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
        try
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://localhost:5091");
            ConfigurationManager configuration = new();
            configuration["urls"] = "http://localhost:5080";
            LocalReleaseConfiguration.Apply(configuration, _directory, useCommandLine ? ["--urls", address] : []);
            Assert.Equal(address, configuration["urls"]);
        }
        finally { Environment.SetEnvironmentVariable("ASPNETCORE_URLS", previous); }
    }

    [Theory]
    [InlineData("http://localhost:5082", "http://localhost:5082/")]
    [InlineData("http://0.0.0.0:5080", "http://localhost:5080/")]
    [InlineData("http://[::]:5083", "http://localhost:5083/")]
    [InlineData("http://127.0.0.1:5090", "http://127.0.0.1:5090/")]
    public void BrowserUsesTheActualPortAndAUsableHost(string binding, string expected)
        => Assert.Equal(expected, LocalReleaseConfiguration.BrowserAddress([binding]));

    /// <summary> Entfernt nur das vom jeweiligen Test erzeugte Verzeichnis. </summary>
    public void Dispose()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, true);
    }
}
