using System.Text.Json;

namespace FiaeTrainer.Core.DatabaseDesign.Models;

/// <summary> Modell oder begründete Auswahlantwort; beide bleiben je Aufgabe getrennt gespeichert. </summary>
public sealed class DesignAnswer
{
    /// <summary> Selbst bearbeiteter relationaler Entwurf. </summary>
    public DesignModel Model { get; set; } = new();
    /// <summary> Stabile Kennungen ausgewählter Aussagen. </summary>
    public List<string> SelectedIds { get; set; } = [];
    /// <summary> Unabhängige Kopie für Speicherung und Vorschau. </summary>
    public DesignAnswer Copy() => JsonSerializer.Deserialize<DesignAnswer>(JsonSerializer.Serialize(this))!;
}
