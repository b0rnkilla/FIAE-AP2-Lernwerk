using FiaeTrainer.Core.QualityAssurance.Models;

namespace FiaeTrainer.Core.QualityAssurance.Content;

/// <summary> Eigener Auftrag für Testentwurf, Qualitätssicherung und defensive Sicherheitsentscheidungen. </summary>
public sealed class QualityAssignment
{
    /// <summary> Stabile Lernkennung beziehungsweise Teilkennung 1–3 innerhalb eines Prüfungsfalls. </summary>
    public int Id { get; set; }
    /// <summary> Zugehöriges Kapitel. </summary>
    public string Chapter { get; set; } = "";
    /// <summary> Konkreter Aufgabentitel. </summary>
    public string Title { get; set; } = "";
    /// <summary> Choice, Worksheet, Review, TestSuite, Matrix oder Order. </summary>
    public string Kind { get; set; } = "Worksheet";
    /// <summary> Build, Repair oder Analyze. </summary>
    public string Format { get; set; } = "Build";
    /// <summary> Eigenständige Situation im Nordhafen-Probebetrieb. </summary>
    public string Story { get; set; } = "";
    /// <summary> Übergang ausschließlich bei bestätigtem Vorgänger. </summary>
    public string PreviousContext { get; set; } = "";
    /// <summary> Verbindlicher Fachvertrag einschließlich der verlangten Abdeckung. </summary>
    public string Task { get; set; } = "";
    /// <summary> Bestätigte Erkenntnis nach richtigem Abschluss. </summary>
    public string Discovery { get; set; } = "";
    /// <summary> Eigene Unterlage, Beobachtung oder erklärter Codeausschnitt. </summary>
    public string Material { get; set; } = "";
    /// <summary> Drei freiwillige Denkanstöße. </summary>
    public List<string> Hints { get; set; } = [];
    /// <summary> Fachliche Mustererklärung ohne automatische Textbenotung. </summary>
    public string Explanation { get; set; } = "";
    /// <summary> Aussagen beziehungsweise konkrete Reviewbefunde. </summary>
    public Dictionary<string, string> Options { get; set; } = [];
    /// <summary> Richtige Auswahlmenge. </summary>
    public List<string> CorrectIds { get; set; } = [];
    /// <summary> Fachlich benannte Antwortfelder. </summary>
    public List<QualityField> Fields { get; set; } = [];
    /// <summary> Matrixzeilen, etwa ausdrücklich vereinbarte Rollen. </summary>
    public Dictionary<string, string> MatrixRows { get; set; } = [];
    /// <summary> Matrixspalten, etwa erlaubte Aktionen. </summary>
    public Dictionary<string, string> MatrixColumns { get; set; } = [];
    /// <summary> Verfügbare Karten für einen selbst aufgebauten Ablauf. </summary>
    public Dictionary<string, string> Cards { get; set; } = [];
    /// <summary> Kennung eines fest implementierten lokalen Beispielprogramms. </summary>
    public string ProgramKey { get; set; } = "";
    /// <summary> Öffentlich verlangte Äquivalenzklassen oder Grenzfälle. </summary>
    public List<string> RequiredPartitions { get; set; } = [];
    /// <summary> Verlangte gezählte Anweisungen. </summary>
    public List<string> RequiredStatements { get; set; } = [];
    /// <summary> Verlangte bedingte Zweige des beschrifteten Modells. </summary>
    public List<string> RequiredBranches { get; set; } = [];
    /// <summary> Verlangte erreichbare Pfade eines schleifenfreien Modells. </summary>
    public List<string> RequiredPaths { get; set; } = [];
    /// <summary> Gezielt eingebaute Fehlervarianten, die korrekte Tests erkennen sollen. </summary>
    public List<string> RequiredFaults { get; set; } = [];
    /// <summary> Die im Auftrag genannte Obergrenze der Testfallanzahl. </summary>
    public int MaximumCases { get; set; } = 12;
    /// <summary> Unabhängige Startvorlage; neue Testsätze sind zunächst leer. </summary>
    public QualityAnswer Starter { get; set; } = new();
    /// <summary> Geprüfte vollständige Referenzen; eigene äquivalente Testwerte bleiben erlaubt. </summary>
    public List<QualityAnswer> Solutions { get; set; } = [];
    /// <summary> Eigene Arbeitskopie statt Änderung der Inhaltsvorlage. </summary>
    public QualityAnswer InitialAnswer() => Starter.Copy();
}
