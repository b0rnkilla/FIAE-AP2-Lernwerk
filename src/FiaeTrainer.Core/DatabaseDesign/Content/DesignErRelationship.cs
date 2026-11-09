namespace FiaeTrainer.Core.DatabaseDesign.Content;

/// <summary> Beziehungsraute zwischen zwei Entitätstypen mit eindeutig erklärter Leserichtung. </summary>
public sealed class DesignErRelationship
{
    /// <summary> Kennung des links dargestellten Entitätstyps. </summary>
    public string Left { get; set; } = "";
    /// <summary> Kennung des rechts dargestellten Entitätstyps. </summary>
    public string Right { get; set; } = "";
    /// <summary> Tätigkeits- oder Beziehungsname. </summary>
    public string Name { get; set; } = "";
    /// <summary> Anzahl linker Objekte je rechtem Objekt. </summary>
    public string LeftCardinality { get; set; } = "";
    /// <summary> Anzahl rechter Objekte je linkem Objekt. </summary>
    public string RightCardinality { get; set; } = "";
    /// <summary> Eigenschaften der Zuordnung, beispielsweise ein vereinbartes Honorar. </summary>
    public List<string> Attributes { get; set; } = [];
}
