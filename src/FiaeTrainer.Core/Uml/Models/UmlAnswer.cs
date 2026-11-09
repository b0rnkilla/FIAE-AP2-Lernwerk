using System.Text.Json;
namespace FiaeTrainer.Core.Uml.Models;

/// <summary> Diagrammentwurf oder fachliche Auswahlantwort. </summary>
public sealed class UmlAnswer
{
    /// <summary> Eigener Diagrammentwurf. </summary>
    public UmlModel Model { get; set; } = new();
    /// <summary> Stabile Kennungen der gewählten Aussagen. </summary>
    public List<string> SelectedIds { get; set; } = [];
    /// <summary> Trennt gespeicherten Entwurf und aktuelle Bearbeitung vollständig. </summary>
    public UmlAnswer Copy() => JsonSerializer.Deserialize<UmlAnswer>(JsonSerializer.Serialize(this))!;
}
