namespace FiaeTrainer.Core.DatabaseDesign.Content;

/// <summary> Entitätstyp der fachlichen ER-Vorlage. </summary>
public sealed class DesignErEntity
{
    /// <summary> Stabile Kennung für die Endpunkte der Beziehungen. </summary>
    public string Id { get; set; } = "";
    /// <summary> Beschriftung oder Verweis auf ein Ergänzungsfeld. </summary>
    public string Name { get; set; } = "";
    /// <summary> Fachliche Eigenschaften, noch ohne technische PK-/FK-Markierungen. </summary>
    public List<string> Attributes { get; set; } = [];
}
