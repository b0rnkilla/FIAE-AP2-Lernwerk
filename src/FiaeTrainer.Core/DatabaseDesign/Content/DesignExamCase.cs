namespace FiaeTrainer.Core.DatabaseDesign.Content;

/// <summary> Eigenständiger unbekannter Prüfungsfall mit drei Teilaufträgen und insgesamt 50 Punkten. </summary>
public sealed class DesignExamCase
{
    /// <summary> Stabile Fallkennung. </summary>
    public string Id { get; set; } = "";
    /// <summary> Titel des neuen Fachfalls. </summary>
    public string Title { get; set; } = "";
    /// <summary> Gemeinsame Ausgangssituation. </summary>
    public string Scenario { get; set; } = "";
    /// <summary> Drei unabhängig mit ihren eigenen Startdaten bearbeitbare Teilaufträge. </summary>
    public List<DesignAssignment> Tasks { get; set; } = [];
    /// <summary> Gewichtung der Teilaufträge in derselben Reihenfolge. </summary>
    public List<int> Points { get; set; } = [20, 20, 10];
}
