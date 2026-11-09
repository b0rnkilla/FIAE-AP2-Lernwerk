namespace FiaeTrainer.Core.Uml.Models;

/// <summary> Bewusst begrenzte UML-Notation und ihre deutschsprachigen Bezeichnungen. </summary>
public static class UmlKinds
{
    /// <summary> Fünf Diagrammarten des freigegebenen Lernwegs. </summary>
    public static readonly Dictionary<string, string> Diagrams = new() { ["UseCase"] = "Anwendungsfalldiagramm", ["Class"] = "Klassendiagramm", ["Activity"] = "Aktivitätsdiagramm", ["Sequence"] = "Sequenzdiagramm", ["State"] = "Zustandsdiagramm" };
    /// <summary> Elementbezeichnungen für die Bearbeitung. </summary>
    public static readonly Dictionary<string, string> Nodes = new() { ["Actor"] = "Akteur", ["UseCase"] = "Anwendungsfall", ["Class"] = "Klasse", ["Interface"] = "Interface", ["Action"] = "Aktion", ["Initial"] = "Start", ["Final"] = "Ende", ["Decision"] = "Entscheidung", ["Merge"] = "Zusammenführung", ["Fork"] = "Parallelisierung", ["Join"] = "Synchronisation", ["Lifeline"] = "Lebenslinie", ["State"] = "Zustand" };
    /// <summary> Beziehungstypen einschließlich der ausdrücklich gerichteten Varianten. </summary>
    public static readonly Dictionary<string, string> Links = new() { ["Association"] = "Assoziation", ["Include"] = "include: Basis → eingebunden", ["Extend"] = "extend: Erweiterung → Basis", ["Generalization"] = "Vererbung: speziell → allgemein", ["Realization"] = "Realisierung: Klasse → Interface", ["Aggregation"] = "Aggregation: Teil → Ganzes", ["Composition"] = "Komposition: Teil → Ganzes", ["Flow"] = "Kontrollfluss", ["Message"] = "Synchroner Aufruf", ["Return"] = "Rückgabe", ["Transition"] = "Zustandsübergang" };
    /// <summary> Erlaubte Elementarten je Diagramm; ein Editor bietet keine fremden Notationen an. </summary>
    public static string[] NodeTypes(string diagram) => diagram switch { "UseCase" => ["Actor", "UseCase"], "Class" => ["Class", "Interface"], "Activity" => ["Action", "Initial", "Final", "Decision", "Merge", "Fork", "Join"], "Sequence" => ["Lifeline"], "State" => ["State", "Initial", "Final"], _ => [] };
    /// <summary> Erlaubte Verbindungen mit fachlich definierter Richtung. </summary>
    public static string[] LinkTypes(string diagram) => diagram switch { "UseCase" => ["Association", "Include", "Extend", "Generalization"], "Class" => ["Association", "Aggregation", "Composition", "Generalization", "Realization"], "Activity" => ["Flow"], "Sequence" => ["Message", "Return"], "State" => ["Transition"], _ => [] };
}
