using FiaeTrainer.Core.Sql;
using FiaeTrainer.Core.Sql.Database;
using FiaeTrainer.Core.Sql.Investigation;
using FiaeTrainer.Core.Sql.Validation;
using FiaeTrainer.Web.Configuration;
using FiaeTrainer.Web.Networking;
using FiaeTrainer.Web.Sql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft persönliche Einrichtung in eigenen Verzeichnissen und serverseitige Schreibgrenzen. </summary>
public sealed class SetupTests : IDisposable
{
    /// <summary> Alle Testdateien liegen ausschließlich unter diesem neuen GUID-Pfad. </summary>
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-Setup", Guid.NewGuid().ToString("N"));

    [Theory]
    [InlineData(@".\SQLEXPRESS")]
    [InlineData(@"MEIN-PC\TRAINING")]
    [InlineData(@"(localdb)\MSSQLLocalDB")]
    [InlineData("localhost,1433")]
    public void ServerNamesAreNotHardcodedAndAlwaysUseTheOwnTrainingDatabase(string server)
    {
        SqlConnectionStringBuilder connection = new(LocalSettingsStore.BuildConnectionString(server));
        Assert.Equal(server, connection.DataSource);
        Assert.Equal(TrainingDatabase.DATABASE_NAME, connection.InitialCatalog);
        Assert.True(connection.IntegratedSecurity);
        Assert.Equal("", connection.Password);
        Assert.Equal(5, connection.ConnectTimeout);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("server;Database=master")]
    [InlineData("server\nother")]
    public void RawConnectionStringsAndControlCharactersAreRejected(string server)
        => Assert.Throws<ArgumentException>(() => LocalSettingsStore.BuildConnectionString(server));

    [Fact]
    public async Task ConcurrentSqlAndUpdatePreferencesPreserveOtherSettingsAndNeverBecomePublicDefaults()
    {
        Directory.CreateDirectory(_directory);
        string file = Path.Combine(_directory, "appsettings.Local.json");
        await File.WriteAllTextAsync(file, """{ "Other": { "Keep": "yes" } }""");
        var (store, config) = Create();
        string connection = LocalSettingsStore.BuildConnectionString(@".\MEINEINSTANZ");
        await Task.WhenAll(store.SaveConnectionAsync(connection), store.SaveUpdatePreferenceAsync(false));
        using ConfigurationManager reloaded = new();
        reloaded.AddJsonFile(file);
        Assert.Equal("yes", reloaded["Other:Keep"]);
        Assert.Equal(connection, reloaded.GetConnectionString("Training"));
        Assert.False(reloaded.GetValue<bool>("Updates:Enabled"));
        Assert.Equal(@".\MEINEINSTANZ", store.DataSource);
        Assert.Equal(connection, config.GetConnectionString("Training"));
        Assert.False(File.Exists(file + ".tmp"));
    }

    [Fact]
    public async Task CorruptPersonalSettingsAreNotSilentlyReplaced()
    {
        Directory.CreateDirectory(_directory);
        string file = Path.Combine(_directory, "appsettings.Local.json");
        await File.WriteAllTextAsync(file, "{broken");
        var (store, _) = Create();
        await Assert.ThrowsAnyAsync<System.Text.Json.JsonException>(() => store.SaveUpdatePreferenceAsync(true));
        Assert.Equal("{broken", await File.ReadAllTextAsync(file));
    }

    [Fact]
    public async Task LanDemonstrationCannotCreateDatabasesOrChangeLocalPreferences()
    {
        var (store, _) = Create(allowLan: true);
        Assert.False(store.CanEdit);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveUpdatePreferenceAsync(false));
        TrainingDatabase original = new(LocalSettingsStore.BuildConnectionString("unreachable"));
        DatabaseState state = new(original, NullLogger<DatabaseState>.Instance, store);
        string? error = await state.ConfigureAsync(@".\SHOULD_NOT_BE_CONTACTED");
        Assert.Contains("Heimnetz", error);
        Assert.Same(original, state.Database);
        Assert.False(Directory.Exists(_directory));
    }

    [SqlFact]
    public async Task VerifiedServerCanBeSelectedWithoutAnAppRestartAndExistingProgressIsUntouched()
    {
        var (store, config) = Create();
        string originalSettings = config.GetConnectionString("Training") ?? "";
        TrainingDatabase original = new(originalSettings);
        DatabaseState state = new(original, NullLogger<DatabaseState>.Instance, store);
        string connection = Environment.GetEnvironmentVariable("FIAE_SQL_CONNECTION") ?? "Server=.\\SQLEXPRESS;Database=FIAE_AP2_Training;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Connect Timeout=5";
        SqlConnectionStringBuilder expected = new(connection);
        Assert.True(expected.IntegratedSecurity, "Dieser Einrichtungstest benötigt die Windows-Anmeldung der lokalen SQL-Testinstanz.");
        string? error = await state.ConfigureAsync(expected.DataSource);
        Assert.Null(error);
        Assert.True(state.Ready);
        Assert.NotSame(original, state.Database);
        Assert.Equal(expected.DataSource, store.DataSource);
        QueryEvaluator evaluator = new(state.Database, new SqlQueryValidator());
        Lesson lesson = new LessonCatalog().Lessons[0];
        Assert.True((await evaluator.EvaluateAsync(lesson, lesson.ReferenceSql)).Correct);
        Assert.False(Directory.Exists(Path.Combine(_directory, "progress")));
    }

    [Fact]
    public void MalformedOldConnectionCanBeReplacedThroughTheSetupForm()
    {
        var (store, config) = Create();
        config["ConnectionStrings:Training"] = "not a connection string";
        Assert.Equal("", store.DataSource);
    }

    [SqlFact]
    public async Task FailedSettingsWriteKeepsThePreviouslyWorkingConnectionAndCorruptFile()
    {
        var (store, _) = Create();
        string connection = Environment.GetEnvironmentVariable("FIAE_SQL_CONNECTION") ?? "Server=.\\SQLEXPRESS;Database=FIAE_AP2_Training;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Connect Timeout=5";
        TrainingDatabase original = new(connection);
        DatabaseState state = new(original, NullLogger<DatabaseState>.Instance, store);
        await state.ConnectAsync();
        Assert.True(state.Ready);
        Directory.CreateDirectory(_directory);
        await File.WriteAllTextAsync(store.FilePath, "{broken");
        string? error = await state.ConfigureAsync(new SqlConnectionStringBuilder(connection).DataSource);
        Assert.NotNull(error);
        Assert.True(state.Ready);
        Assert.Same(original, state.Database);
        Assert.Equal("{broken", await File.ReadAllTextAsync(store.FilePath));
    }

    [Fact]
    public async Task InvalidSelectionKeepsTheExistingDatabaseReferenceAndWritesNoFile()
    {
        var (store, _) = Create();
        TrainingDatabase original = new(LocalSettingsStore.BuildConnectionString("unreachable"));
        DatabaseState state = new(original, NullLogger<DatabaseState>.Instance, store);
        Assert.NotNull(await state.ConfigureAsync("server;Database=master"));
        Assert.Same(original, state.Database);
        Assert.False(Directory.Exists(_directory));
    }

    private (LocalSettingsStore, ConfigurationManager) Create(bool allowLan = false)
    {
        ConfigurationManager configuration = new();
        configuration["ConnectionStrings:Training"] = LocalSettingsStore.BuildConnectionString(@".\SQLEXPRESS");
        return (new LocalSettingsStore(configuration, new TestEnvironment { ContentRootPath = _directory }, new NetworkAccessPolicy(allowLan)), configuration);
    }

    /// <summary> Entfernt ausschließlich die selbst erstellten Einrichtungsdateien. </summary>
    public void Dispose()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, true);
    }

    /// <summary> Minimale Testumgebung ohne Zugriff auf das echte Benutzerprofil. </summary>
    private sealed class TestEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get; set; } = "";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = "";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string EnvironmentName { get; set; } = "Testing";
        public string ApplicationName { get; set; } = "FiaeTrainer.Tests";
    }
}
