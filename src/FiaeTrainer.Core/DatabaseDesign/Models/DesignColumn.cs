namespace FiaeTrainer.Core.DatabaseDesign.Models;

/// <summary> Attribut mit fachlicher Kennung und getrenntem Anzeigenamen. </summary>
public sealed class DesignColumn
{
    /// <summary> Technische Kennung für unverwechselbare Beziehungspaare. </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    /// <summary> Aus dem Auftragsvokabular gewählte Bedeutung. </summary>
    public string Attribute { get; set; } = "";
    /// <summary> Frei bearbeitbare Beschriftung, beispielsweise Kundennummer statt KundeId. </summary>
    public string Name { get; set; } = "";
    /// <summary> Teil des einfachen oder zusammengesetzten Primärschlüssels. </summary>
    public bool PrimaryKey { get; set; }
    /// <summary> Erlaubt einen fehlenden Attributwert; PK-Attribute dürfen dies nicht. </summary>
    public bool Nullable { get; set; }
    /// <summary> Gleiche nichtleere Gruppenkennung kennzeichnet zusammen einen alternativen eindeutigen Schlüssel. </summary>
    public string UniqueGroup { get; set; } = "";
}
