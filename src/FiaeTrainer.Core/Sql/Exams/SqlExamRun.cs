using FiaeTrainer.Core.Sql.Results;

namespace FiaeTrainer.Core.Sql.Exams;

/// <summary> Begrenzte Ergebnisse oder Zustände einer verworfenen SQL-Prüfungsarbeitskopie. </summary>
public sealed class SqlExamRun
{
    /// <summary> Ausgabetabelle bei einer SELECT-Aufgabe. </summary>
    public QueryResult Query { get; set; } = new();
    /// <summary> Tabellenstände und Strukturen nach einer Schreibaufgabe. </summary>
    public Dictionary<string, QueryResult> State { get; set; } = [];
    /// <summary> Verständlicher Fehler; technische Fehler werden nicht als falsche Antwort benotet. </summary>
    public string? Error { get; set; }
    /// <summary> Infrastrukturfehler erlauben eine erneute Bewertung der gesperrten Abgabe. </summary>
    public bool TechnicalFailure { get; set; }
}
