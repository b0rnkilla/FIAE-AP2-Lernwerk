namespace FiaeTrainer.Core.SoftwareDesign.Models;

/// <summary> Eigenes Architekturbauteil oder Formularfeld mit freiem Anzeigenamen und fester Fachrolle. </summary>
public sealed class SoftwareNode
{
    /// <summary> Lokale Kennung für Verbindungen und Bearbeitung. </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    /// <summary> Bedeutung aus dem Aufgabenwortschatz. </summary>
    public string Key { get; set; } = "";
    /// <summary> Frei wählbare sichtbare Beschriftung. </summary>
    public string Name { get; set; } = "Neues Element";
    /// <summary> Architekturart oder Eingabeelement. </summary>
    public string Kind { get; set; } = "Component";
    /// <summary> Zugeordnete Schicht oder Formulargruppe. </summary>
    public string Group { get; set; } = "";
    /// <summary> Verpflichtende Eingabe in einer Formularvorschau. </summary>
    public bool Required { get; set; }
    /// <summary> Eigener Hilfetext; seine Verständlichkeit bleibt Teil der Selbstkontrolle. </summary>
    public string Hint { get; set; } = "";
}
