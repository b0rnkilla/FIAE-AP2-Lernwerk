namespace FiaeTrainer.Core;

/// <summary> Ergänzt die gemeinsame Lernaufgabe um die fachlich benötigte Schreib- oder Strukturänderung. </summary>
public sealed class WorkshopLesson : Lesson
{
    /// <summary> Verlangter SQL-Baustein; Ergebnisse allein reichen bei Transaktionen nicht aus. </summary>
    public WorkshopOperation Operation { get; init; }
}

/// <summary> Unterstützte Übungsformen der SQL-Werkstatt. </summary>
public enum WorkshopOperation { Update, Insert, Delete, CreateTable, AlterTable, Commit, Rollback }

/// <summary> Zustand sämtlicher Arbeitskopien nach einem Skript einschließlich überprüfter Strukturregeln. </summary>
public sealed class WorkshopRun
{
    /// <summary> Vollständige Datenstände der vorhandenen Übungstabellen. </summary>
    public Dictionary<string, QueryResult> Tables { get; init; } = [];
    /// <summary> Spalten, Datentypen, NULL-Zulässigkeit und Schlüsselmerkmale. </summary>
    public Dictionary<string, QueryResult> Structures { get; init; } = [];
    /// <summary> Sichtbare Zwischenstände unmittelbar vor einem ROLLBACK, damit auch der Probeabzug überprüft wird. </summary>
    public Dictionary<string, QueryResult> BeforeRollback { get; init; } = [];
    /// <summary> Ergebnisse gezielter Einfügeversuche zur Prüfung von Constraints. </summary>
    public QueryResult ConstraintChecks { get; init; } = new();
    /// <summary> Verständlicher Fehler statt eines bewertbaren Endzustands. </summary>
    public string? Error { get; init; }
    /// <summary> Infrastrukturfehler ohne fachliche Wertung. </summary>
    public bool TechnicalFailure { get; init; }
}

/// <summary> Bewertung einer Werkstattaufgabe mit sichtbarem Endzustand. </summary>
public sealed record WorkshopEvaluation(bool Correct, string Message, WorkshopRun Run, bool TechnicalFailure = false);
