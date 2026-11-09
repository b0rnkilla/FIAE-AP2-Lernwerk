using FiaeTrainer.Core.Uml.Models;
namespace FiaeTrainer.Core.Uml.Content;

/// <summary> Eigene Aufgabe mit ausdrücklichem Vokabular und geprüften Modellvarianten. </summary>
public sealed class UmlAssignment
{
    /// <summary> Stabile Lern- oder Teilaufgabenkennung. </summary>
    public int Id { get; set; }
    /// <summary> Kapitel des Lernwegs. </summary>
    public string Chapter { get; set; } = "";
    /// <summary> Kurzer Aufgabentitel. </summary>
    public string Title { get; set; } = "";
    /// <summary> UseCase, Class, Activity, Sequence oder State. </summary>
    public string DiagramType { get; set; } = "Class";
    /// <summary> Build, Repair oder Read. </summary>
    public string Format { get; set; } = "Build";
    /// <summary> Übergang nur bei bestätigtem Vorgänger. </summary>
    public string PreviousContext { get; set; } = "";
    /// <summary> Unabhängig verständlicher Anlass im Kulturzentrum Nordhafen. </summary>
    public string Story { get; set; } = "";
    /// <summary> Vollständige fachliche Anforderungen. </summary>
    public string Task { get; set; } = "";
    /// <summary> Erkenntnis erst nach erfolgreicher Lösung. </summary>
    public string Discovery { get; set; } = "";
    /// <summary> Drei freiwillige Denkanstöße. </summary>
    public List<string> Hints { get; set; } = [];
    /// <summary> Fachliche Erklärung der Musterlösung. </summary>
    public string Explanation { get; set; } = "";
    /// <summary> Fachrollen der möglichen Elemente. </summary>
    public Dictionary<string, string> NodeVocabulary { get; set; } = [];
    /// <summary> Fachrollen von Attributen, Methoden und Aktionen. </summary>
    public Dictionary<string, string> MemberVocabulary { get; set; } = [];
    /// <summary> Benannte Bedingungen einschließlich sonst. </summary>
    public Dictionary<string, string> Conditions { get; set; } = [];
    /// <summary> Ereignisse für Zustandsübergänge. </summary>
    public Dictionary<string, string> Events { get; set; } = [];
    /// <summary> Fachliche Nachrichten oder Rückgaben. </summary>
    public Dictionary<string, string> Messages { get; set; } = [];
    /// <summary> Verantwortungsbereiche des Aktivitätsdiagramms. </summary>
    public Dictionary<string, string> Lanes { get; set; } = [];
    /// <summary> Fachrollen der Sequenzgruppen. </summary>
    public Dictionary<string, string> FragmentVocabulary { get; set; } = [];
    /// <summary> Vorlage für Lesen und Reparieren; Build beginnt leer. </summary>
    public UmlModel Starter { get; set; } = new();
    /// <summary> Ausdrücklich zugelassene vollständige Varianten. </summary>
    public List<UmlModel> Solutions { get; set; } = [];
    /// <summary> Aussagen bei Leseaufgaben. </summary>
    public Dictionary<string, string> Options { get; set; } = [];
    /// <summary> Vollständige richtige Auswahl. </summary>
    public List<string> CorrectIds { get; set; } = [];
    /// <summary> Ein neuer Versuch übernimmt ausschließlich die vorgesehene Vorlage. </summary>
    public FiaeTrainer.Core.Uml.Models.UmlAnswer InitialAnswer() => new() { Model = Starter.Copy() };
}
