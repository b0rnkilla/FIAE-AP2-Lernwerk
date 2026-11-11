namespace FiaeTrainer.Core.QualityAssurance.Evaluation;

/// <summary> Beobachtbare lokale Testausführung, sichtbar erst nach bewusstem Prüfen oder endgültiger Prüfungsabgabe. </summary>
public sealed class QualityProbe
{
    /// <summary> Ergebnisse in der Reihenfolge der eigenen Testfälle. </summary>
    public List<QualityRowResult> Rows { get; set; } = [];
    /// <summary> Vereinigung ausgeführter Anweisungen, unabhängig vom Erfolg einer Assertion. </summary>
    public HashSet<string> Statements => Rows.Where(row => row.Execution != null).SelectMany(row => row.Execution!.Statements).ToHashSet();
    /// <summary> Vereinigung ausgeführter bedingter Zweige. </summary>
    public HashSet<string> Branches => Rows.Where(row => row.Execution != null).SelectMany(row => row.Execution!.Branches).ToHashSet();
    /// <summary> Tatsächlich ausgeführte Pfade. </summary>
    public HashSet<string> Paths => Rows.Where(row => row.Execution != null).Select(row => row.Execution!.Path).ToHashSet();
}
