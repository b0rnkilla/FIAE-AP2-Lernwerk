namespace FiaeTrainer.Core.DatabaseDesign.Content;

/// <summary> Eigene fachliche ER-Abbildung; mit $ beginnende Beschriftungen verweisen auf Ergänzungsfelder. </summary>
public sealed class DesignErDiagram
{
    /// <summary> Aussagekräftiger Titel der Abbildung. </summary>
    public string Title { get; set; } = "ER-Modell";
    /// <summary> Entitätstypen mit Attributen, unabhängig von relationalen Tabellen und Fremdschlüsseln. </summary>
    public List<DesignErEntity> Entities { get; set; } = [];
    /// <summary> Binäre Beziehungen mit Kardinalitäten und eigenen Attributen. </summary>
    public List<DesignErRelationship> Relationships { get; set; } = [];
}
