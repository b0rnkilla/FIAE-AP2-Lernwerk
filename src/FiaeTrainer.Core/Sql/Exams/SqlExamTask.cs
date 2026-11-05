namespace FiaeTrainer.Core.Sql.Exams;

/// <summary> Eigenständiger SQL-Prüfungsauftrag mit zehn möglichen Trainingspunkten. </summary>
public sealed class SqlExamTask
{
    /// <summary> Dauerhafte Kennung im SQL-Prüfungspool. </summary>
    public int Id { get; init; }
    /// <summary> Kurzer Titel der Aufgabe. </summary>
    public string Title { get; init; } = "";
    /// <summary> Expliziter Antwortvertrag und Spaltenreihenfolge. </summary>
    public string Task { get; init; } = "";
    /// <summary> Query, Update, Insert oder Alter; nur die angegebene Anweisungsart ist erlaubt. </summary>
    public string Operation { get; init; } = "Query";
    /// <summary> Feste zusätzliche Tabelle für eine Schreibaufgabe. </summary>
    public string Target { get; init; } = "";
    /// <summary> Nach Abgabe sichtbare geprüfte Referenz. </summary>
    public string ReferenceSql { get; init; } = "";
    /// <summary> Erläuterung der Musterlösung. </summary>
    public string Explanation { get; init; } = "";
    /// <summary> Nur bei ausdrücklich verlangter Sortierung relevant. </summary>
    public bool Ordered { get; init; }
}
