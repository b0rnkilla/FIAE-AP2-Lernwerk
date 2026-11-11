using FiaeTrainer.Core.SoftwareDesign.Models;
using FiaeTrainer.Core.Uml.Content;

namespace FiaeTrainer.Core.SoftwareDesign.Content;

/// <summary> Eigene prüfungsnahe Softwareplanungsaufgabe mit ausdrücklich beschriebenen Varianten. </summary>
public sealed class SoftwareAssignment
{
    /// <summary> Stabile Lern- oder Prüfungsteilkennung. </summary>
    public int Id { get; set; }
    /// <summary> Kapitelüberschrift. </summary>
    public string Chapter { get; set; } = "";
    /// <summary> Kurzer Titel des konkreten Problems. </summary>
    public string Title { get; set; } = "";
    /// <summary> Choice, Worksheet, Diagram, Architecture, Api oder Form. </summary>
    public string Kind { get; set; } = "Worksheet";
    /// <summary> Build, Repair oder Analyze. </summary>
    public string Format { get; set; } = "Build";
    /// <summary> Eigenständiger Anlass bei Mira König und Nordhafen. </summary>
    public string Story { get; set; } = "";
    /// <summary> Nur bei bestätigtem Vorgänger sichtbarer Übergang. </summary>
    public string PreviousContext { get; set; } = "";
    /// <summary> Eindeutiger Arbeitsauftrag einschließlich Bewertungsgrenzen. </summary>
    public string Task { get; set; } = "";
    /// <summary> Bestätigte Erkenntnis nach Erfolg. </summary>
    public string Discovery { get; set; } = "";
    /// <summary> Optionale Unterlage oder sprachneutral erklärter Code. </summary>
    public string Material { get; set; } = "";
    /// <summary> Drei freiwillige Hinweise. </summary>
    public List<string> Hints { get; set; } = [];
    /// <summary> Fachliche Erklärung nach bewusster Aufdeckung. </summary>
    public string Explanation { get; set; } = "";
    /// <summary> Aussagen einer Auswahlaufgabe. </summary>
    public Dictionary<string, string> Options { get; set; } = [];
    /// <summary> Vollständige richtige Auswahl. </summary>
    public List<string> CorrectIds { get; set; } = [];
    /// <summary> Strukturierte Antwortfelder. </summary>
    public List<SoftwareField> Fields { get; set; } = [];
    /// <summary> Aufgabenwortschatz für eigene Bausteine und Formularfelder. </summary>
    public Dictionary<string, string> NodeVocabulary { get; set; } = [];
    /// <summary> Erlaubte Schichten beziehungsweise Gruppen. </summary>
    public Dictionary<string, string> Groups { get; set; } = [];
    /// <summary> Verwendbare Elementarten. </summary>
    public Dictionary<string, string> NodeKinds { get; set; } = [];
    /// <summary> Verwendbare Abhängigkeiten. </summary>
    public Dictionary<string, string> LinkKinds { get; set; } = [];
    /// <summary> Eigener UML-Auftrag verwendet dieselbe Darstellung, keine alten Lerninhalte. </summary>
    public UmlAssignment? Diagram { get; set; }
    /// <summary> None, Json oder Xml. </summary>
    public string PayloadFormat { get; set; } = "None";
    /// <summary> Unabhängige Startkopie für Reparaturen. </summary>
    public SoftwareAnswer Starter { get; set; } = new();
    /// <summary> Vollständige geprüfte Varianten; eigene Namen sind weiterhin erlaubt. </summary>
    public List<SoftwareAnswer> Solutions { get; set; } = [];
    /// <summary> Erstellt einen frischen Versuch ohne Veränderung der Vorlage. </summary>
    public SoftwareAnswer InitialAnswer() => Starter.Copy();
}
