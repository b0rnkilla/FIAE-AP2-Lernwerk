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
    /// <summary> Arbeitsform, die die zulässigen Anweisungen für diesen Auftrag festlegt. </summary>
    public string Operation { get; init; } = "Query";
    /// <summary> Feste zusätzliche Tabelle für eine Schreibaufgabe. </summary>
    public string Target { get; init; } = "";
    /// <summary> Nach Abgabe sichtbare geprüfte Referenz. </summary>
    public string ReferenceSql { get; init; } = "";
    /// <summary> Erläuterung der Musterlösung. </summary>
    public string Explanation { get; init; } = "";
    /// <summary> Fallinterne Einrichtung vor einem einzelnen Auftrag. </summary>
    public string[] Preparation { get; init; } = [];
    /// <summary> Unabhängige fachliche Prüfungen zusätzlich zu den zwei Formpunkten. </summary>
    public SqlExamCriterion[] Criteria { get; init; } = [];
    /// <summary> Nur bei ausdrücklich verlangter Sortierung relevant. </summary>
    public bool Ordered { get; init; }
}
