namespace FiaeTrainer.Core.Uml.Models;

/// <summary> Ein einfaches, nicht verschachteltes alt-, opt- oder loop-Fragment. </summary>
public sealed class UmlFragment
{
    /// <summary> Lokale Gruppenkennung. </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    /// <summary> Fachrolle der Gruppe. </summary>
    public string Key { get; set; } = "";
    /// <summary> alt, opt oder loop. </summary>
    public string Kind { get; set; } = "alt";
    /// <summary> Bedingung für opt oder loop; alt verwendet Bedingungen je Nachricht. </summary>
    public string Guard { get; set; } = "";
}
