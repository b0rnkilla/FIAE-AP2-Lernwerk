namespace FiaeTrainer.Core.QualityAssurance.Evaluation;

/// <summary> Trennt Eingabefehler, fachlich richtigen Sollwert und tatsächliche Fehlererkennung. </summary>
public sealed class QualityRowResult
{
    /// <summary> Zugehöriger eigener Testfall. </summary>
    public string Id { get; set; } = "";
    /// <summary> Beschriftete Eingaben zur späteren Auswertung. </summary>
    public string Inputs { get; set; } = "";
    /// <summary> Vom Lernenden erwartetes Ergebnis. </summary>
    public string Expected { get; set; } = "";
    /// <summary> Ausführung nur bei gültigem Testdatenformat. </summary>
    public QualityExecution? Execution { get; set; }
    /// <summary> Der Sollwert passt zur unabhängigen fachlichen Referenz. </summary>
    public bool ExpectedCorrect { get; set; }
    /// <summary> Erklärter Status des Falls. </summary>
    public string Detail { get; set; } = "";
    /// <summary> Tatsächlich erkannte Fehlervarianten; ein falscher Sollwert zählt nie als Fehlernachweis. </summary>
    public HashSet<string> DetectedFaults { get; set; } = [];
}
