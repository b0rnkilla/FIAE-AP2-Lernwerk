using FiaeTrainer.Core.Sql;
using FiaeTrainer.Core.Sql.Investigation;
using FiaeTrainer.Core.Sql.Validation;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft die Sicherheitsgrenzen der Lernabfragen unabhängig von einem SQL Server. </summary>
public sealed class SqlQueryValidatorTests
{
    [Theory]
    [InlineData("DELETE FROM fall.Sendungen;")]
    [InlineData("SELECT Id FROM fall.Sendungen; DROP TABLE fall.Sendungen;")]
    [InlineData("SELECT * INTO fall.Kopie FROM fall.Sendungen;")]
    [InlineData("SELECT name FROM master.sys.databases;")]
    [InlineData("SELECT * FROM probe.Sendungen;")]
    [InlineData("SELECT * FROM sys.tables;")]
    [InlineData("SELECT * FROM TrainerVersion;")]
    [InlineData("SELECT * FROM dbo.Sendungen;")]
    [InlineData("SELECT * FROM fall.Sendungen WITH (TABLOCKX);")]
    [InlineData("EXEC sp_configure;")]
    [InlineData("REVERT")]
    [InlineData("SELECT * FROM OPENQUERY(andererServer, 'SELECT 1');")]
    [InlineData("SELECT * FROM OPENROWSET(BULK 'C:\\fremd.txt', SINGLE_CLOB) AS x;")]
    [InlineData("SELECT @@SERVERNAME;")]
    [InlineData("SELECT @x = Id FROM fall.Sendungen;")]
    [InlineData("SELECT dbo.UnbekannteFunktion();")]
    [InlineData("WITH x AS (SELECT * FROM fall.Sendungen) SELECT * FROM x;")]
    [InlineData("SELECT * FROM fall.Sendungen FOR JSON AUTO;")]
    [InlineData("SELECT 1; GO SELECT 2;")]
    [InlineData("")]
    public void UnsafeOrUnsupportedStatementsAreRejected(string sql) => Assert.NotNull(new SqlQueryValidator().Validate(sql));

    [Theory]
    [InlineData("SELECT Id FROM fall.Sendungen WHERE Status = 'DELETE';")]
    [InlineData("-- DROP TABLE ist nur ein Kommentar\nSELECT [Id] FROM [fall].[Sendungen];")]
    [InlineData("SELECT m.Id FROM fall.Mitarbeiter m WHERE NOT EXISTS (SELECT 1 FROM fall.Fahrten r WHERE r.MitarbeiterId = m.Id);")]
    [InlineData("SELECT * FROM Sendungen;")]
    [InlineData("SELECT s.Id FROM sendungen s JOIN Fahrzeuge f ON s.FahrzeugId=f.Id;")]
    [InlineData("SELECT m.Id FROM Mitarbeiter m WHERE EXISTS (SELECT 1 FROM fall.Fahrten f WHERE f.MitarbeiterId=m.Id);")]
    public void KeywordsInsideLiteralsOrCommentsAndValidSubqueriesAreAllowed(string sql) => Assert.Null(new SqlQueryValidator().Validate(sql));

    [Fact]
    public void VerificationRewritesOnlyTableReferences()
    {
        string rewritten = new SqlQueryValidator().ForVerificationData("SELECT Id, 'fall.Sendungen' AS Hinweis FROM fall.Sendungen;");
        Assert.Contains("probe.Sendungen", rewritten);
        Assert.Contains("'fall.Sendungen'", rewritten);
    }

    [Fact]
    public void FullyQualifiedColumnsAndStarsAreRewrittenWithTheirTables()
    {
        SqlQueryValidator validator = new();
        string sql = "SELECT fall.Sendungen.Id, fall.Sendungen.Bezeichnung FROM fall.Sendungen;";
        Assert.Null(validator.Validate(sql));
        string rewritten = validator.ForVerificationData(sql);
        Assert.Contains("probe.Sendungen.Id", rewritten);
        Assert.DoesNotContain("fall.Sendungen", rewritten);
        Assert.Contains("probe.Sendungen.*", validator.ForVerificationData("SELECT fall.Sendungen.* FROM fall.Sendungen;"));
    }

    [Fact]
    public void OptionalSchemaNamesAreResolvedInBothDatasetsWithoutChangingLiterals()
    {
        SqlQueryValidator validator = new();
        string sql = "SELECT s.Id, 'Sendungen' AS Hinweis FROM Sendungen s WHERE s.FahrzeugId IN(SELECT f.Id FROM Fahrzeuge f);";
        Assert.Null(validator.Validate(sql));
        string primary = validator.ForCaseData(sql);
        string secondary = validator.ForVerificationData(sql);
        Assert.Contains("fall.Sendungen", primary);
        Assert.Contains("fall.Fahrzeuge", primary);
        Assert.Contains("probe.Sendungen", secondary);
        Assert.Contains("probe.Fahrzeuge", secondary);
        Assert.Contains("'Sendungen'", secondary);
    }

    [Fact]
    public void CatalogContainsThirtyCompleteLessons()
    {
        LessonCatalog catalog = new();
        Assert.Equal(Enumerable.Range(1, 30), catalog.Lessons.Select(x => x.Id));
        foreach (Lesson lesson in catalog.Lessons)
        {
            Assert.Null(new SqlQueryValidator().Validate(lesson.ReferenceSql));
            Assert.Equal(3, lesson.Hints.Length);
            Assert.False(string.IsNullOrWhiteSpace(lesson.Story));
            Assert.False(string.IsNullOrWhiteSpace(lesson.Briefing));
            Assert.False(string.IsNullOrWhiteSpace(lesson.Task));
            Assert.False(string.IsNullOrWhiteSpace(lesson.Explanation));
        }
    }
}
