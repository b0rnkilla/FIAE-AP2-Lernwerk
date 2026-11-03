using FiaeTrainer.Core.Sql;
using FiaeTrainer.Core.Sql.Investigation;
using FiaeTrainer.Core.Sql.Validation;
using FiaeTrainer.Core.Sql.Workshop;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft Trainingsregeln einschließlich Kommentaren, Unterabfragen und mehrteiligen Transaktionen. </summary>
public sealed class SqlWritingRulesTests
{
    [Theory]
    [InlineData("select Id FROM Sendungen;", "SELECT")]
    [InlineData("SELECT Id from Sendungen;", "FROM")]
    [InlineData("SELECT Id FROM Sendungen WHERE FahrzeugId is null;", "IS")]
    [InlineData("SELECT count(*) AS anzahl FROM Sendungen;", "COUNT")]
    [InlineData("SELECT cast(Wert AS int) FROM Sendungen;", "CAST")]
    [InlineData("SELECT try_cast(Wert AS int) FROM Sendungen;", "TRY_CAST")]
    [InlineData("SELECT iif(Wert > 0, 1, 0) FROM Sendungen;", "IIF")]
    [InlineData("SELECT Id FROM Sendungen", "Semikolon")]
    [InlineData("SELECT Id FROM Sendungen -- ;", "Semikolon")]
    [InlineData("SELECT Id FROM Sendungen /* ; */", "Semikolon")]
    public void InvestigationExplainsTheConcreteWritingIssue(string sql, string expected)
    {
        string? error = new SqlQueryValidator().Validate(sql);
        Assert.NotNull(error);
        Assert.Contains(expected, error);
    }

    [Theory]
    [InlineData("SELECT id AS klein FROM sendungen WHERE Status = 'select from'; -- where")]
    [InlineData("SELECT [select] AS [from], \"where\" FROM Sendungen;")]
    [InlineData("SELECT Id FROM Sendungen WHERE Bezeichnung = 'O''Brien; select'; /* where ; */")]
    [InlineData("SELECT Id FROM Sendungen WHERE EXISTS (SELECT 1 FROM Fahrzeuge); -- Ende")]
    [InlineData("SELECT COUNT(*) AS count FROM Sendungen;")]
    public void NamesLiteralsCommentsAndSubqueriesKeepTheirOwnSpelling(string sql)
        => Assert.Null(new SqlQueryValidator().Validate(sql));

    [Theory]
    [InlineData("BEGIN TRANSACTION UPDATE Konten SET Guthaben=Guthaben-50 WHERE Id=1; COMMIT;", "Anweisung 1")]
    [InlineData("BEGIN TRANSACTION; UPDATE Konten SET Guthaben=Guthaben-50 WHERE Id=1 COMMIT;", "Anweisung 2")]
    [InlineData("BEGIN TRANSACTION; UPDATE Konten SET Guthaben=Guthaben-50 WHERE Id=1; COMMIT -- ;", "Anweisung 3")]
    [InlineData("UPDATE Artikel set Preis=1;", "SET")]
    [InlineData("UPDATE Artikel SET Preis=round(Preis,2);", "ROUND")]
    public void WorkshopChecksEveryStatementIncludingTransactionCommands(string sql, string expected)
    {
        string? error = new WorkshopSqlValidator().Validate(sql);
        Assert.NotNull(error);
        Assert.Contains(expected, error);
    }

    [Fact]
    public void CompletedTransactionWithTrailingCommentsIsAccepted()
        => Assert.Null(new WorkshopSqlValidator().Validate("BEGIN TRANSACTION; -- Start\nUPDATE Konten SET Guthaben=Guthaben-50 WHERE Id=1; ROLLBACK; /* Ende */", WorkshopOperation.Rollback));

    [Fact]
    public void VisibleCatalogsContainNoSchemaPrefixesOrCompactDateLiterals()
    {
        foreach (Lesson lesson in new LessonCatalog().Lessons)
        {
            string content = string.Join("\n", new[] { lesson.Story, lesson.Briefing, lesson.Task, lesson.ReferenceSql, lesson.Explanation, lesson.Discovery }.Concat(lesson.Hints));
            Assert.DoesNotContain("fall.", content);
            Assert.DoesNotMatch(@"'\d{8}", content);
        }
    }
}
