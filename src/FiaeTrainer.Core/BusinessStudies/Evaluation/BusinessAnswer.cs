namespace FiaeTrainer.Core.BusinessStudies.Evaluation;

/// <summary> Antwortdaten ohne hinterlegte Musterlösung. </summary>
public sealed class BusinessAnswer
{
    /// <summary> Gewählte stabile Optionskennungen. </summary>
    public List<string> SelectedIds { get; set; } = [];
    /// <summary> Eingegebene Ergebnisse nach Feldkennung. </summary>
    public Dictionary<string, string> Values { get; set; } = [];
    /// <summary> Vom Lernenden angeordnete Schritte. </summary>
    public List<string> Order { get; set; } = [];
    /// <summary> Kennzeichnet eine bewusst als unsicher markierte Antwort. </summary>
    public bool Unsure { get; set; }
}
