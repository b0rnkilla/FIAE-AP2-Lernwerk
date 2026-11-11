namespace FiaeTrainer.Core.SoftwareDesign.Content;

/// <summary> Eigener unbekannter Fall mit drei unabhängig lösbaren Planungsaufträgen. </summary>
public sealed class SoftwareExamCase
{
    /// <summary> Stabile Fallkennung. </summary>
    public string Id { get; set; } = "";
    /// <summary> Name des Falls. </summary>
    public string Title { get; set; } = "";
    /// <summary> Gemeinsamer Sachverhalt ohne Ergebnisse anderer Teile. </summary>
    public string Scenario { get; set; } = "";
    /// <summary> Drei Aufträge mit IDs 1–3. </summary>
    public List<SoftwareAssignment> Tasks { get; set; } = [];
    /// <summary> Gewichtung 20/20/10. </summary>
    public int[] Points { get; set; } = [20, 20, 10];
}
