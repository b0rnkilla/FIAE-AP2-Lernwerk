namespace FiaeTrainer.Core.QualityAssurance.Content;

/// <summary> Klar benanntes Antwortfeld; zulässige Lösungen stehen ausschließlich in den Referenzvarianten. </summary>
public sealed class QualityField
{
    /// <summary> Stabile Fachkennung. </summary>
    public string Key { get; set; } = "";
    /// <summary> Sichtbare Beschriftung einschließlich Einheit oder Bedingung. </summary>
    public string Label { get; set; } = "";
    /// <summary> Select, Text oder Number. </summary>
    public string Kind { get; set; } = "Select";
    /// <summary> Fachlich sinnvolle Optionen einschließlich Ablenkantworten. </summary>
    public Dictionary<string, string> Options { get; set; } = [];
    /// <summary> Erklärt die Eingabe, ohne die Lösung vorwegzunehmen. </summary>
    public string Help { get; set; } = "";
    /// <summary> Exact für Vertragskennungen, Text für benannte Schreibvarianten oder Number. </summary>
    public string Comparison { get; set; } = "Exact";
}
