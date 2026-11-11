namespace FiaeTrainer.Core.QualityAssurance.Content;

/// <summary> Öffentlicher Vertrag eines begrenzten Beispielprogramms, ohne fremden Code auszuführen. </summary>
public sealed class QualityProgram
{
    /// <summary> Stabile Kennung der fest implementierten Rechenregel. </summary>
    public string Key { get; set; } = "";
    /// <summary> Sichtbarer Name. </summary>
    public string Title { get; set; } = "";
    /// <summary> Unabhängige Spezifikation für die Sollwertermittlung. </summary>
    public string Contract { get; set; } = "";
    /// <summary> Erklärte Codeunterlage; S zählt Anweisungen, D benennt Entscheidungen. </summary>
    public string Code { get; set; } = "";
    /// <summary> Gebundene Parameter mit eigenen zulässigen Testwerten. </summary>
    public List<QualityInput> Inputs { get; set; } = [];
    /// <summary> Number, Boolean oder Text. </summary>
    public string OutputKind { get; set; } = "Number";
    /// <summary> Benannte Ausgaben bei Textprogrammen. </summary>
    public Dictionary<string, string> OutputOptions { get; set; } = [];
    /// <summary> Anweisungskennungen und deren Bedeutung. </summary>
    public Dictionary<string, string> Statements { get; set; } = [];
    /// <summary> Gezählt werden die ausdrücklich beschrifteten wahren/falschen Bedingungsausgänge. </summary>
    public Dictionary<string, string> Branches { get; set; } = [];
    /// <summary> Erreichbare Pfade; kein unbegrenzter Schleifenbeweis. </summary>
    public Dictionary<string, string> Paths { get; set; } = [];
    /// <summary> Unabhängig aus dem Eingabevertrag abgeleitete Klassen/Grenzen. </summary>
    public Dictionary<string, string> Partitions { get; set; } = [];
    /// <summary> Eigene, gezielt eingebaute Fehler; keine vollständige Korrektheitsgarantie. </summary>
    public Dictionary<string, string> Faults { get; set; } = [];
}
