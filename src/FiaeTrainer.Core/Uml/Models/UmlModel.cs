using System.Text.Json;
namespace FiaeTrainer.Core.Uml.Models;

/// <summary> Begrenzter, selbst aufgebauter UML-Entwurf ohne bewertete Zeichenkoordinaten. </summary>
public sealed class UmlModel
{
    /// <summary> Beschriftete Systemgrenze im Anwendungsfalldiagramm. </summary>
    public string SystemName { get; set; } = "";
    /// <summary> Eigene Elemente in frei gewählter Erstellreihenfolge. </summary>
    public List<UmlNode> Nodes { get; set; } = [];
    /// <summary> Eigene Verbindungen; bei Sequenzen fachlich geordnete Nachrichten. </summary>
    public List<UmlLink> Links { get; set; } = [];
    /// <summary> Explizite Gruppen für Sequenznachrichten. </summary>
    public List<UmlFragment> Fragments { get; set; } = [];
    /// <summary> Erstellt eine unabhängige Arbeitskopie. </summary>
    public UmlModel Copy() => JsonSerializer.Deserialize<UmlModel>(JsonSerializer.Serialize(this))!;
}
