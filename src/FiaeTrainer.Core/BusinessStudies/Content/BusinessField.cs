namespace FiaeTrainer.Core.BusinessStudies.Content;

/// <summary> Ein einzeln bewertetes Zahlen-, Termin- oder Zuordnungsfeld. </summary>
public sealed class BusinessField
{
    /// <summary> Stabile Feldkennung innerhalb der Aufgabe. </summary>
    public string Id { get; init; } = "";
    /// <summary> Verständliche Beschriftung mit Teilauftrag. </summary>
    public string Label { get; init; } = "";
    /// <summary> Erwartete Zahl in invariantem Format, Datum, Uhrzeit oder Optionskennung. </summary>
    public string Expected { get; init; } = "";
    /// <summary> Einheit, die neben dem Eingabefeld angezeigt wird. </summary>
    public string Unit { get; init; } = "";
    /// <summary> Zahl der geforderten Nachkommastellen; nur Zahlenfelder nutzen diese Vorgabe. </summary>
    public int Decimals { get; init; }
    /// <summary> Unabhängig formulierter Rechenweg oder Zuordnungsgrund. </summary>
    public string Explanation { get; init; } = "";
}
