namespace FiaeTrainer.Core.DatabaseDesign.Evaluation;

/// <summary> Nachvollziehbares Ergebnis eines konkreten Modellvertrags, kein allgemeiner Normalformbeweis. </summary>
public sealed class DesignEvaluation
{
    /// <summary> Getrennte Rückmeldungen zu Tabellen, Schlüsseln und Beziehungen. </summary>
    public List<DesignCheck> Checks { get; set; } = [];
    /// <summary> Erreichte der 50 möglichen Trainingspunkte. </summary>
    public int Earned => Checks.Sum(check => check.Earned);
    /// <summary> Vollständig erfüllter Auftragsvertrag. </summary>
    public bool Correct => Earned == 50;
}
