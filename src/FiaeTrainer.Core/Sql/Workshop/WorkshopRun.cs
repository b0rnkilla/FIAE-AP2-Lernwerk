using FiaeTrainer.Core.Sql.Results;

namespace FiaeTrainer.Core.Sql.Workshop;

/// <summary> Zustand sämtlicher Arbeitskopien nach einem Skript einschließlich überprüfter Strukturregeln. </summary>
public sealed class WorkshopRun
{
    /// <summary> Datenstände der vorhandenen Übungstabellen mit höchstens 200 Zeilen je Tabelle. </summary>
    public Dictionary<string, QueryResult> Tables { get; init; } = [];
    /// <summary> Spalten, Datentypen, NULL-Zulässigkeit und Schlüsselmerkmale. </summary>
    public Dictionary<string, QueryResult> Structures { get; init; } = [];
    /// <summary> Sichtbare Zwischenstände unmittelbar vor einem ROLLBACK, damit auch der Probeabzug überprüft wird. </summary>
    public Dictionary<string, QueryResult> BeforeRollback { get; init; } = [];
    /// <summary> Ergebnisse gezielter Einfügeversuche zur Prüfung von Constraints. </summary>
    public QueryResult ConstraintChecks { get; init; } = new();
    /// <summary> Ausgabe der eigenen Mengenabfrage. </summary>
    public QueryResult? Selection => Statements.LastOrDefault(x => x.Result != null)?.Result;
    /// <summary> Ausgaben in Skriptreihenfolge, unabhängig vom abschließenden Tabellenstand. </summary>
    public List<WorkshopStatementResult> Statements { get; } = [];
    /// <summary> Verständlicher Fehler statt eines bewertbaren Endzustands. </summary>
    public string? Error { get; init; }
    /// <summary> Infrastrukturfehler ohne fachliche Wertung. </summary>
    public bool TechnicalFailure { get; init; }
}

/// <summary> Tatsächliche Ausgabe einer Anweisung in der Arbeitskopie. </summary>
public sealed record WorkshopStatementResult(string Command, QueryResult? Result = null, int Affected = -1);
