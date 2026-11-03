namespace FiaeTrainer.Core.Sql.Results;

/// <summary> Enthält eine begrenzte Ergebnistabelle oder eine verständliche Fehlermeldung. </summary>
public sealed class QueryResult
{
    /// <summary> Spaltennamen in Ausgabereihenfolge. </summary>
    public List<string> Columns { get; init; } = [];
    /// <summary> SQL-Werte; NULL bleibt als null erhalten. </summary>
    public List<object?[]> Rows { get; init; } = [];
    /// <summary> Fachliche oder technische Fehlermeldung. </summary>
    public string? Error { get; init; }
    /// <summary> Kennzeichnet ein über dem Anzeigelimit liegendes Ergebnis. </summary>
    public bool Truncated { get; init; }
    /// <summary> Verbindungs- oder Infrastrukturfehler ohne fachliche Wertung. </summary>
    public bool TechnicalFailure { get; init; }
}
