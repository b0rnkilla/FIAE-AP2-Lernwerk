using System.Text.Json;
using FiaeTrainer.Core.Uml.Models;

namespace FiaeTrainer.Core.SoftwareDesign.Models;

/// <summary> Antwort eines Softwareentwurfs; optionale Begründungen verändern die automatische Bewertung nicht. </summary>
public sealed class SoftwareAnswer
{
    /// <summary> Strukturierte Spezifikations-, OOP-, Werkzeug- oder Vertragsfelder. </summary>
    public Dictionary<string, string> Values { get; set; } = [];
    /// <summary> Stabile Kennungen einer Einfach- oder Mehrfachauswahl. </summary>
    public List<string> SelectedIds { get; set; } = [];
    /// <summary> Eigenes Klassen-/Musterdiagramm, getrennt von Abschnitt 03. </summary>
    public UmlAnswer Diagram { get; set; } = new();
    /// <summary> Eigene Architektur oder eigener Formularentwurf. </summary>
    public SoftwareBoard Board { get; set; } = new();
    /// <summary> Eigene JSON-/XML-Nachricht ohne externe Ausführung. </summary>
    public string Payload { get; set; } = "";
    /// <summary> Freie Begründung zur ausdrücklich unbewerteten Selbstkontrolle. </summary>
    public string Reasoning { get; set; } = "";
    /// <summary> Trennt Referenzen, Entwürfe und unveränderliche Speicherübergaben. </summary>
    public SoftwareAnswer Copy() => JsonSerializer.Deserialize<SoftwareAnswer>(JsonSerializer.Serialize(this))!;
}
