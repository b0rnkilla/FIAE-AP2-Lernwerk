namespace FiaeTrainer.Core.Uml.Content;

/// <summary> Eigener Prüfungsfall mit drei unabhängigen Diagrammaufträgen. </summary>
public sealed class UmlExamCase
{
    /// <summary> Unveränderliche Fallkennung. </summary>
    public string Id { get; set; } = "";
    /// <summary> Name des Prüfungsfalls. </summary>
    public string Title { get; set; } = "";
    /// <summary> Gemeinsamer Falltext ohne Abhängigkeit von Lernabschlüssen. </summary>
    public string Scenario { get; set; } = "";
    /// <summary> Drei eigene Teilaufgaben. </summary>
    public List<UmlAssignment> Tasks { get; set; } = [];
    /// <summary> Gewichtung auf fünfzig Trainingspunkte. </summary>
    public List<int> Points { get; set; } = [20, 20, 10];
}
