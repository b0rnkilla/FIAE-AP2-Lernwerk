namespace FiaeTrainer.Core.Uml.Models;

/// <summary> Gerichtete Verbindung; in Sequenzdiagrammen bestimmt die Listenposition die Nachrichtenfolge. </summary>
public sealed class UmlLink
{
    /// <summary> Lokale Kennung für Bearbeitung und Fehlermarkierung. </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    /// <summary> Kennung des Ausgangselements. </summary>
    public string Source { get; set; } = "";
    /// <summary> Kennung des Zielelements. </summary>
    public string Target { get; set; } = "";
    /// <summary> Beziehung, Kontrollfluss, Nachricht oder Zustandsübergang. </summary>
    public string Kind { get; set; } = "Association";
    /// <summary> Nachrichten- oder Ereigniskennung aus dem Aufgabenwortschatz. </summary>
    public string Meaning { get; set; } = "";
    /// <summary> Eigene lesbare Nachrichten- oder Ereignisbeschriftung. </summary>
    public string Name { get; set; } = "";
    /// <summary> Anzahl am Ausgangsende einer Klassenbeziehung. </summary>
    public string SourceMultiplicity { get; set; } = "";
    /// <summary> Anzahl am Zielende einer Klassenbeziehung. </summary>
    public string TargetMultiplicity { get; set; } = "";
    /// <summary> Fachliche Bedingung aus dem Aufgabenwortschatz. </summary>
    public string Guard { get; set; } = "";
    /// <summary> Kennung des umgebenden Sequenzfragments. </summary>
    public string Fragment { get; set; } = "";
}
