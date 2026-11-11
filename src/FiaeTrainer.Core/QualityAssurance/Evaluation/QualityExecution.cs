namespace FiaeTrainer.Core.QualityAssurance.Evaluation;

/// <summary> Ergebnis einer isolierten Ausführung des fest vorgegebenen Testmodells. </summary>
public sealed class QualityExecution
{
    /// <summary> Tatsächliche, kanonisch formatierte Rückgabe. </summary>
    public string Output { get; set; } = "";
    /// <summary> Wirklich ausgeführte gezählte Anweisungen. </summary>
    public HashSet<string> Statements { get; set; } = [];
    /// <summary> Wirklich ausgeführte bedingte Zweige. </summary>
    public HashSet<string> Branches { get; set; } = [];
    /// <summary> Zusammenhängende Folge der Bedingungsausgänge. </summary>
    public string Path { get; set; } = "";
}
