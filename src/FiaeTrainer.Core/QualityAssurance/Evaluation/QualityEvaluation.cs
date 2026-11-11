namespace FiaeTrainer.Core.QualityAssurance.Evaluation;

/// <summary> Strukturierte Bewertung ohne Interpretation freier Begründungen. </summary>
public sealed class QualityEvaluation
{
    /// <summary> Einzelkriterien mit insgesamt 50 internen Anteilen. </summary>
    public List<QualityCheck> Checks { get; set; } = [];
    /// <summary> Tatsächliche lokale Ausführung der eigenen Testfälle; erst nach Prüfung sichtbar. </summary>
    public QualityProbe? Probe { get; set; }
    /// <summary> Summe der Kriterienanteile. </summary>
    public int Earned => Checks.Sum(check => check.Earned);
    /// <summary> Vollständig passende Fachlösung. </summary>
    public bool Correct => Checks.Count > 0 && Earned == 50;
}
