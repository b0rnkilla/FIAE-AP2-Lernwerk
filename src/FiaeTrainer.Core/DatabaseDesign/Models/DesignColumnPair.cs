namespace FiaeTrainer.Core.DatabaseDesign.Models;

/// <summary> Ein Attributpaar innerhalb eines Fremdschlüssels. </summary>
public sealed class DesignColumnPair
{
    /// <summary> Technische Kennung des FK-Attributs. </summary>
    public string Source { get; set; } = "";
    /// <summary> Technische Kennung des referenzierten Attributs. </summary>
    public string Target { get; set; } = "";
}
