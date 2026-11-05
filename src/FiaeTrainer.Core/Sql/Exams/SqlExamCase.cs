namespace FiaeTrainer.Core.Sql.Exams;

/// <summary> Ein abgeschlossener Prüfungsfall mit neuen Tabellen und unabhängig lösbaren Aufgaben. </summary>
public sealed class SqlExamCase
{
    /// <summary> Dauerhafte Fallkennung. </summary>
    public string Id { get; init; } = "";
    /// <summary> Fachlicher Zusammenhang. </summary>
    public string Title { get; init; } = "";
    /// <summary> Ausgangssituation ohne Vorwegnahme einer Lösung. </summary>
    public string Scenario { get; init; } = "";
    /// <summary> Sichtbare Typen und Beziehungen. </summary>
    public string Schema { get; init; } = "";
    /// <summary> Erlaubte Tabellen, ausschließlich innerhalb der Arbeitskopie. </summary>
    public string[] Tables { get; init; } = [];
    /// <summary> Eigene Ausgangsdaten für eine neue Sitzung. </summary>
    public string SeedSql { get; init; } = "";
    /// <summary> Zusätzliche, von der Vorschau verschiedene Prüfdaten. </summary>
    public string VerificationSql { get; init; } = "";
    /// <summary> Fünf Aufgaben mit jeweils zehn Trainingspunkten. </summary>
    public SqlExamTask[] Tasks { get; init; } = [];
}
