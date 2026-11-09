namespace FiaeTrainer.Core.DatabaseDesign.Content;

/// <summary> Eigene Arbeitsunterlage als lesbarer Beleg oder Datentabelle. </summary>
public sealed class DesignEvidence
{
    /// <summary> Überschrift der Unterlage. </summary>
    public string Title { get; set; } = "";
    /// <summary> Kontext und Bedeutung der Beispielzeilen. </summary>
    public string Description { get; set; } = "";
    /// <summary> Beschriftete Spalten. </summary>
    public List<string> Columns { get; set; } = [];
    /// <summary> Eigene synthetische Datensätze in Spaltenreihenfolge. </summary>
    public List<List<string>> Rows { get; set; } = [];
}
