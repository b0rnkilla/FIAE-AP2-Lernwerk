using FiaeTrainer.Core.DatabaseDesign.Models;

namespace FiaeTrainer.Core.DatabaseDesign.Content;

/// <summary> Eigener Arbeitsauftrag mit explizitem Vokabular, Annahmen und geprüften Modellvarianten. </summary>
public sealed class DesignAssignment
{
    /// <summary> Unveränderliche Kennung innerhalb des Lern- oder Prüfungskatalogs. </summary>
    public int Id { get; set; }
    /// <summary> Kapitel des Lernwegs. </summary>
    public string Chapter { get; set; } = "";
    /// <summary> Kurzer Aufgabentitel. </summary>
    public string Title { get; set; } = "";
    /// <summary> Nachvollziehbare Ausgangssituation ohne vorweggenommene ungelöste Ergebnisse. </summary>
    public string Story { get; set; } = "";
    /// <summary> Vollständiger Bewertungsvertrag einschließlich fachlicher Abhängigkeiten. </summary>
    public string Task { get; set; } = "";
    /// <summary> Model, Choice oder Multiple; reine Auswahlaufgaben besitzen stabile Optionen. </summary>
    public string Kind { get; set; } = "Model";
    /// <summary> Bedeutung und Standardbeschriftung der verfügbaren Attribute. </summary>
    public Dictionary<string, string> Attributes { get; set; } = [];
    /// <summary> Unabhängige Ausgangskopie, bei Reparaturaufgaben absichtlich fehlerhaft. </summary>
    public DesignModel Starter { get; set; } = new();
    /// <summary> Explizit zugelassene vollständige Lösungsvarianten. </summary>
    public List<DesignModel> Solutions { get; set; } = [];
    /// <summary> Antwortmöglichkeiten bei Verständnisaufgaben. </summary>
    public Dictionary<string, string> Options { get; set; } = [];
    /// <summary> Vollständige richtige Auswahl; Mehrfachauswahl wird als Menge verglichen. </summary>
    public List<string> CorrectIds { get; set; } = [];
    /// <summary> Drei freiwillige, zunehmend konkrete Denkanstöße. </summary>
    public List<string> Hints { get; set; } = [];
    /// <summary> Fachliche Erklärung der Lösung und relevanter Varianten. </summary>
    public string Explanation { get; set; } = "";
    /// <summary> Erzeugt den Ausgangsentwurf ohne Anzeige einer Lösung. </summary>
    public DesignAnswer InitialAnswer() => new() { Model = Starter.Copy() };
}
