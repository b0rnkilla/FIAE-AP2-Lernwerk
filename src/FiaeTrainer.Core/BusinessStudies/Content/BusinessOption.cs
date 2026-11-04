namespace FiaeTrainer.Core.BusinessStudies.Content;

/// <summary> Stabile Antwortkennung mit einer eigenen Begründung. </summary>
public sealed class BusinessOption
{
    /// <summary> Unabhängig von der sichtbaren Reihenfolge gespeicherte Kennung. </summary>
    public string Id { get; init; } = "";
    /// <summary> Angezeigte Antwort oder einzuordnender Schritt. </summary>
    public string Text { get; init; } = "";
    /// <summary> Erklärung der Aussage nach der Prüfung. </summary>
    public string Explanation { get; init; } = "";
}
