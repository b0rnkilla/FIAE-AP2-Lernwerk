namespace FiaeTrainer.Core.SoftwareDesign.Models;

/// <summary> Fachliche Abhängigkeit zwischen selbst angelegten Architekturbausteinen. </summary>
public sealed class SoftwareLink
{
    /// <summary> Lokale Kennung für die Bearbeitung. </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    /// <summary> Kennung des aufrufenden oder abhängigen Elements. </summary>
    public string Source { get; set; } = "";
    /// <summary> Kennung des angesprochenen Elements. </summary>
    public string Target { get; set; } = "";
    /// <summary> Call, Dependency, Notify oder Create. </summary>
    public string Kind { get; set; } = "Call";
}
