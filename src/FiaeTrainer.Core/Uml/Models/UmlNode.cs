namespace FiaeTrainer.Core.Uml.Models;

/// <summary> Eigenständig angelegtes Diagrammelement mit stabiler Fachbedeutung. </summary>
public sealed class UmlNode
{
    /// <summary> Lokale Kennung für Verbindungen. </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    /// <summary> Fachrolle unabhängig vom Anzeigenamen. </summary>
    public string Key { get; set; } = "";
    /// <summary> Eigene sichtbare Beschriftung. </summary>
    public string Name { get; set; } = "";
    /// <summary> UML-Elementart aus dem erlaubten Diagrammumfang. </summary>
    public string Kind { get; set; } = "Class";
    /// <summary> Abstrakte Klasse. </summary>
    public bool Abstract { get; set; }
    /// <summary> Verantwortungsbereich einer Aktivität. </summary>
    public string Lane { get; set; } = "";
    /// <summary> Attribute, Methoden oder Zustandsaktionen. </summary>
    public List<UmlMember> Members { get; set; } = [];
}
