using FiaeTrainer.Core.Sql.Validation;
using FiaeTrainer.Core.Sql.Workshop;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft konkrete Hilfen zu Syntaxfehlern und Gegenfälle mit korrekt maskierten Zeichen. </summary>
public sealed class SqlSyntaxFeedbackTests
{
    [Theory]
    [InlineData("SELECT Id, Zielort, Status FROM Sendungen Where Zielort = 'Dortmund' or Zielort = 'Essen\"", "'Essen\"", "einfachen Anführungszeichen")]
    [InlineData("SELECT Id FROM Sendungen WHERE Zielort = 'Essen", "'Essen", "einfachen Anführungszeichen")]
    [InlineData("SELECT Id FROM Sendungen WHERE Zielort = \"Essen'", "\"Essen'", "schließende doppelte Anführungszeichen")]
    [InlineData("SELECT Id FROM Sendungen WHERE Zielort = 'O''Brien", "'O''Brien", "einfachen Anführungszeichen")]
    [InlineData("SELECT [Id FROM Sendungen", "[Id", "schließende eckige Klammer")]
    [InlineData("SELECT Id FROM Sendungen WHERE (Wert > 100", "(Wert", "schließende Klammer")]
    [InlineData("SELECT Id FROM Sendungen WHERE Wert > 100)", "Wert > 100)", "ohne passende öffnende Klammer")]
    [InlineData("SELECT Id FROM Sendungen /* Kommentar nicht geschlossen", "/* Kommentar", "*/ am Ende")]
    [InlineData("SELECT Id, FROM Sendungen", "FROM Sendungen", "fehlende Spaltennamen")]
    [InlineData("SELECT Id FROM Sendungen WHERE Wert >", "Wert >", "endet unerwartet")]
    public void BothValidatorsExplainTheAffectedSqlInsteadOfAColumnNumber(string sql, string excerpt, string explanation)
    {
        string? investigation = new SqlQueryValidator().Validate(sql);
        string? workshop = new WorkshopSqlValidator().Validate(sql);
        Assert.NotNull(investigation);
        Assert.Equal(investigation, workshop);
        Assert.Contains(excerpt, investigation);
        Assert.Contains(explanation, investigation);
        Assert.DoesNotMatch(@"Zeile \d+|Spalte \d+", investigation);
    }

    [Theory]
    [InlineData("UPDATE Artikel SET Preis = Preis * 1,1 WHERE Kategorie = 'Sensoren';", "1.1")]
    [InlineData("UPDATE Artikel SET Preis =", "Mit SET")]
    [InlineData("INSERT INTO Artikel (Id, Preis) VALUES (1,);", "Werteliste")]
    [InlineData("DELETE FROM Artikel WHERE Id =;", "DELETE FROM")]
    [InlineData("UPDTE Artikel SET Preis = 2;", "SQL-Befehl")]
    [InlineData("CREATE TABLE Rueckgaben (Id int, Anzahl);", "Spaltendefinitionen")]
    [InlineData("ALTER TABLE Artikel ADD;", "Ergänzung nach ADD")]
    [InlineData("BEGIN TRANSACTION; /* SELECT; */ UPDATE Artikel SET Kategorie = 'Text; SELECT', Preis =;", "Mit SET")]
    [InlineData("BEGIN TRANSACTION; UPDATE Artikel SET Preis = ROUND(Preis * 1.1, 2), Bestand =;", "Mit SET")]
    [InlineData("BEGIN TRANSACTION; ROLLBACK TRANSACTION 42;", "COMMIT oder ROLLBACK")]
    public void WorkshopExplainsTheFailingStatement(string sql, string hint)
    {
        WorkshopSqlValidator validator = new();
        string? feedback = validator.Validate(sql);
        Assert.NotNull(feedback);
        Assert.Contains(hint, feedback);
        Assert.DoesNotContain("GROUP BY", feedback);
        Assert.Equal(feedback, validator.Validate(sql, WorkshopOperation.Update));
    }

    [Theory]
    [InlineData("UPDATE Artikel SET Preis = Preis * 1.1 WHERE Kategorie = 'Sensoren';")]
    [InlineData("UPDATE Artikel SET Preis = ROUND(Preis * 1.1,2), Kategorie = 'Text 1,1';")]
    public void WorkshopAcceptsDecimalPointsAndListSeparators(string sql) => Assert.Null(new WorkshopSqlValidator().Validate(sql));

    [Theory]
    [InlineData("SELECT Id FROM Sendungen WHERE Zielort = 'O''Brien';")]
    [InlineData("SELECT Id FROM Sendungen WHERE Zielort = 'Er sagt \"Hallo\"';")]
    [InlineData("SELECT \"I\"\"d\" FROM Sendungen;")]
    [InlineData("SELECT [I]]d] FROM Sendungen;")]
    [InlineData("SELECT Id FROM Sendungen /* ' \" ( /* verschachtelt */ ) */; -- ' \" (")]
    public void CorrectlyEscapedQuotesAndCommentsRemainAccepted(string sql) => Assert.Null(new SqlQueryValidator().Validate(sql));
}
