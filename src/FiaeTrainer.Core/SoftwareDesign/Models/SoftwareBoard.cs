namespace FiaeTrainer.Core.SoftwareDesign.Models;

/// <summary> Architektur oder Formular; nur bei Formularen hat die Elementreihenfolge fachliche Bedeutung. </summary>
public sealed class SoftwareBoard
{
    /// <summary> Eigene Bausteine in Erstell- beziehungsweise Formularreihenfolge. </summary>
    public List<SoftwareNode> Nodes { get; set; } = [];
    /// <summary> Gerichtete Architekturverbindungen. </summary>
    public List<SoftwareLink> Links { get; set; } = [];
}
