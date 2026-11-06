namespace FiaeTrainer.Core.DatabaseDesign.Models;

/// <summary> Eine Referenz von der abhängigen zur referenzierten Tabelle; Minima beschreiben zusätzliche Geschäftsregeln. </summary>
public sealed class DesignRelation
{
    /// <summary> Stabile Kennung für Bearbeiten und Entfernen. </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    /// <summary> Tabelle, in der die FK-Attribute liegen. </summary>
    public string SourceTable { get; set; } = "";
    /// <summary> Tabelle mit dem referenzierten Schlüssel. </summary>
    public string TargetTable { get; set; } = "";
    /// <summary> Vollständige Zuordnung auch zusammengesetzter Fremdschlüssel. </summary>
    public List<DesignColumnPair> Pairs { get; set; } = [new()];
    /// <summary> Null oder genau ein Elternsatz je abhängigem Satz; das Maximum ist bei einem FK immer eins. </summary>
    public int ParentMinimum { get; set; } = 1;
    /// <summary> Mindestzahl abhängiger Sätze je Elternsatz, gegebenenfalls als zusätzliche Geschäftsregel. </summary>
    public int ChildMinimum { get; set; }
    /// <summary> Eins oder viele abhängige Sätze je Elternsatz. </summary>
    public string ChildMaximum { get; set; } = "n";
}
