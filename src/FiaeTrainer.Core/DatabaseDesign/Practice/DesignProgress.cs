using FiaeTrainer.Core.DatabaseDesign.Content;
using FiaeTrainer.Core.DatabaseDesign.Models;

namespace FiaeTrainer.Core.DatabaseDesign.Practice;

/// <summary> Individueller Modellversuch; frühere Abschlüsse bleiben bei Wiederholung erhalten. </summary>
public sealed class DesignProgress
{
    /// <summary> Inhaltsvertrag für den gespeicherten Entwurf. </summary>
    public string ContentVersion { get; set; } = DesignCatalog.CONTENT_VERSION;
    /// <summary> Erkennt veraltete Bearbeitungsfenster. </summary>
    public int Revision { get; set; }
    /// <summary> Eigene Antwort unabhängig vom Ausgangsmodell. </summary>
    public DesignAnswer Answer { get; set; } = new();
    /// <summary> Neue Aufgaben starten ohne Hilfen. </summary>
    public bool IndependentMode { get; set; } = true;
    /// <summary> Unterstützung bleibt bis zum neuen Versuch vermerkt. </summary>
    public bool Assisted { get; set; }
    /// <summary> Anzahl geöffneter Hinweise. </summary>
    public int HintLevel { get; set; }
    /// <summary> Eine aufgedeckte Lösung bleibt als Unterstützung vermerkt. </summary>
    public bool SolutionViewed { get; set; }
    /// <summary> Anzahl geprüfter Antworten. </summary>
    public int Attempts { get; set; }
    /// <summary> Mindestens ein richtiger Versuch. </summary>
    public bool Completed { get; set; }
    /// <summary> Mindestens ein richtiger Versuch ohne Hilfen. </summary>
    public bool Independent { get; set; }
}
