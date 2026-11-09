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
    /// <summary> Übergang vom vorherigen Lernschritt; nur bei dessen bestätigtem Abschluss anzeigen. </summary>
    public string PreviousContext { get; set; } = "";
    /// <summary> Nachvollziehbare Ausgangssituation ohne vorweggenommene ungelöste Ergebnisse. </summary>
    public string Story { get; set; } = "";
    /// <summary> Ergebnis für den Planungsbericht, erst nach erfolgreicher Lösung sichtbar. </summary>
    public string Discovery { get; set; } = "";
    /// <summary> Vollständiger Bewertungsvertrag einschließlich fachlicher Abhängigkeiten. </summary>
    public string Task { get; set; } = "";
    /// <summary> Model, ErCompletion, Choice oder Multiple; Antworten besitzen stabile Fachkennungen. </summary>
    public string Kind { get; set; } = "Model";
    /// <summary> Vorgegebene fachliche ER-Abbildung zum Ergänzen oder als Übersetzungsvorlage. </summary>
    public DesignErDiagram? SourceDiagram { get; set; }
    /// <summary> Gezielte Lücken in der ER-Abbildung. </summary>
    public List<DesignDiagramSlot> DiagramSlots { get; set; } = [];
    /// <summary> Vorgegebener relationaler Entwurf bleibt als unveränderte Arbeitsunterlage sichtbar. </summary>
    public bool ShowStarter { get; set; }
    /// <summary> Beleg oder Beispieldaten zur selbstständigen Herleitung des Modells. </summary>
    public DesignEvidence? Evidence { get; set; }
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
