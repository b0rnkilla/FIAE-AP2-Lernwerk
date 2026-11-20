namespace FiaeTrainer.Web.Components.Diagrams;

/// <summary> Eine lokale Geste auf der Zeichenfläche; der jeweilige Editor prüft ihre Zulässigkeit. </summary>
public sealed class DiagramAction
{
    /// <summary> Auswahl, Verschieben, Umbenennen, Verbinden oder Löschen. </summary>
    public string Kind { get; set; } = "";
    /// <summary> Kennung des betroffenen Elements. </summary>
    public string Id { get; set; } = "";
    /// <summary> Ziel einer neuen Verbindung. </summary>
    public string Target { get; set; } = "";
    /// <summary> Neue Beschriftung. </summary>
    public string Text { get; set; } = "";
    /// <summary> Verschiebung in Diagrammkoordinaten. </summary>
    public int X { get; set; }
    /// <summary> Vertikale Verschiebung in Diagrammkoordinaten. </summary>
    public int Y { get; set; }
}
