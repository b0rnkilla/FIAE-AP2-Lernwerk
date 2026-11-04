namespace FiaeTrainer.Core.BusinessStudies.Content;

/// <summary> Eigenständige WiSo-Aufgabe mit unveränderlicher Lösung und Quellenbezug. </summary>
public sealed class BusinessQuestion
{
    /// <summary> Dauerhafte Nummer; veröffentlichte Aufgaben werden nicht umnummeriert. </summary>
    public int Id { get; init; }
    /// <summary> Zugehöriger Themenbereich. </summary>
    public string TopicId { get; init; } = "";
    /// <summary> Kurzer Titel für Ergebnislisten. </summary>
    public string Title { get; init; } = "";
    /// <summary> Eigener betrieblicher Sachverhalt mit allen benötigten Annahmen. </summary>
    public string Scenario { get; init; } = "";
    /// <summary> Eindeutiger Teilauftrag. </summary>
    public string Task { get; init; } = "";
    /// <summary> Bestimmt Eingabe und Bewertung. </summary>
    public BusinessQuestionKind Kind { get; init; }
    /// <summary> Auswahlmöglichkeiten oder zu sortierende Schritte. </summary>
    public BusinessOption[] Options { get; init; } = [];
    /// <summary> Richtige Optionskennungen bei Einfach- und Mehrfachauswahl. </summary>
    public string[] CorrectIds { get; init; } = [];
    /// <summary> Fachlich festgelegte Reihenfolge der Schritte. </summary>
    public string[] CorrectOrder { get; init; } = [];
    /// <summary> Einzelne Zuordnungen oder Ergebnisfelder. </summary>
    public BusinessField[] Fields { get; init; } = [];
    /// <summary> Gemeinsame fachliche Erklärung nach der Antwort. </summary>
    public string Explanation { get; init; } = "";
    /// <summary> Kurzer Merksatz zum Wiederholen. </summary>
    public string MemoryAid { get; init; } = "";
    /// <summary> Verweise auf den eigenen Quellenkatalog. </summary>
    public string[] SourceIds { get; init; } = [];
    /// <summary> Fachlicher Prüfstand; Rechenannahmen stehen zusätzlich im Sachverhalt. </summary>
    public string AsOf { get; init; } = "2026-11-04";
    /// <summary> Optionales selbst erstelltes Organigramm oder Sicherheitszeichen. </summary>
    public string Illustration { get; init; } = "";
    /// <summary> Vollständige Beschreibung einer fachlich relevanten Abbildung. </summary>
    public string IllustrationDescription { get; init; } = "";
    /// <summary> Spaltenüberschriften einer selbst erstellten Datentabelle. </summary>
    public string[] TableHeaders { get; init; } = [];
    /// <summary> Zugehörige Tabellenzeilen; keine HTML-Fragmente oder fremden Prüfungsscans. </summary>
    public string[][] TableRows { get; init; } = [];
    /// <summary> Anzahl der erzielbaren Trainingspunkte. </summary>
    public int Maximum => Fields.Length > 0 ? Fields.Length : 1;
}
