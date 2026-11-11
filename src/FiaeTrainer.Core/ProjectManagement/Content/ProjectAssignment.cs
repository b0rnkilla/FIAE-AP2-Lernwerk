using FiaeTrainer.Core.ProjectManagement.Models;

namespace FiaeTrainer.Core.ProjectManagement.Content;

/// <summary> Eigener Auftrag für Projektentscheidungen, Rechentabellen und selbst aufgebaute Pläne. </summary>
public sealed class ProjectAssignment
{
    /// <summary> Stabile Aufgabenkennung beziehungsweise Teilkennung einer Prüfung. </summary>
    public int Id { get; set; }
    /// <summary> Zugehöriges Kapitel. </summary>
    public string Chapter { get; set; } = "";
    /// <summary> Konkreter Aufgabentitel. </summary>
    public string Title { get; set; } = "";
    /// <summary> Choice, Review, Worksheet, Structure, Network oder Schedule. </summary>
    public string Kind { get; set; } = "Worksheet";
    /// <summary> Build, Repair oder Analyze. </summary>
    public string Format { get; set; } = "Build";
    /// <summary> Eigenständig verständlicher Folgeauftrag bei Nordhafen. </summary>
    public string Story { get; set; } = "";
    /// <summary> Übergang ausschließlich bei tatsächlich abgeschlossenem Vorgänger. </summary>
    public string PreviousContext { get; set; } = "";
    /// <summary> Verbindlicher Arbeitsauftrag mit allen fachlichen Annahmen. </summary>
    public string Task { get; set; } = "";
    /// <summary> Erst nach richtigem Abschluss freigegebene Erkenntnis. </summary>
    public string Discovery { get; set; } = "";
    /// <summary> Eigene Unterlage mit Zeit-, Kosten- oder Angebotsdaten. </summary>
    public string Material { get; set; } = "";
    /// <summary> Drei freiwillige Denkanstöße. </summary>
    public List<string> Hints { get; set; } = [];
    /// <summary> Fachliche Mustererklärung. </summary>
    public string Explanation { get; set; } = "";
    /// <summary> Aussagen beziehungsweise konkret zu markierende Reviewbefunde. </summary>
    public Dictionary<string, string> Options { get; set; } = [];
    /// <summary> Geforderte vollständige Auswahlmenge. </summary>
    public List<string> CorrectIds { get; set; } = [];
    /// <summary> Benannte Antwortfelder und Rechenzellen. </summary>
    public List<ProjectField> Fields { get; set; } = [];
    /// <summary> Nur bei Planaufträgen vorhanden. </summary>
    public ProjectPlan? Plan { get; set; }
    /// <summary> Sichtbare unveränderte Vorlage für Ergänzungen oder Reparaturen. </summary>
    public ProjectAnswer Starter { get; set; } = new();
    /// <summary> Geprüfte Referenzen, keine Beschränkung gültiger Terminpläne auf deren Werte. </summary>
    public List<ProjectAnswer> Solutions { get; set; } = [];
    /// <summary> Unabhängige eigene Arbeitskopie. </summary>
    public ProjectAnswer InitialAnswer() => Starter.Copy();
}
