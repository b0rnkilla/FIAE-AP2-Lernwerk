using FiaeTrainer.Core;
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
    [InlineData("SELECT Id FROM Sendungen WHERE Zielort = 'O''Brien';")]
    [InlineData("SELECT Id FROM Sendungen WHERE Zielort = 'Er sagt \"Hallo\"';")]
    [InlineData("SELECT \"I\"\"d\" FROM Sendungen;")]
    [InlineData("SELECT [I]]d] FROM Sendungen;")]
    [InlineData("SELECT Id FROM Sendungen /* ' \" ( /* verschachtelt */ ) */; -- ' \" (")]
    public void CorrectlyEscapedQuotesAndCommentsRemainAccepted(string sql) => Assert.Null(new SqlQueryValidator().Validate(sql));
}
