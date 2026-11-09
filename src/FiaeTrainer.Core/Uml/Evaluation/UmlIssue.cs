namespace FiaeTrainer.Core.Uml.Evaluation;

/// <summary> Markiert fachlich betroffene Teile des eigenen Diagramms. </summary>
public sealed class UmlIssue
{
    /// <summary> Betroffenes Element, sofern vorhanden. </summary>
    public string NodeId { get; set; } = "";
    /// <summary> Betroffene Verbindung, sofern vorhanden. </summary>
    public string LinkId { get; set; } = "";
    /// <summary> Konkreter fachlicher Hinweis. </summary>
    public string Text { get; set; } = "";
}
