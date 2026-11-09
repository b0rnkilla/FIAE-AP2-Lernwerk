namespace FiaeTrainer.Core.DatabaseDesign.Content;

/// <summary> Gezielte Ergänzung einer ER-Abbildung mit stabilen Antwortkennungen. </summary>
public sealed class DesignDiagramSlot
{
    /// <summary> Kennung des Platzhalters in der Abbildung. </summary>
    public string Id { get; set; } = "";
    /// <summary> Vollständige Frage einschließlich der Kardinalitätsrichtung. </summary>
    public string Prompt { get; set; } = "";
    /// <summary> Kennungen und sichtbare Beschriftungen der möglichen Ergänzungen. </summary>
    public Dictionary<string, string> Options { get; set; } = [];
    /// <summary> Fachlich richtige Ergänzung. </summary>
    public string CorrectId { get; set; } = "";
}
