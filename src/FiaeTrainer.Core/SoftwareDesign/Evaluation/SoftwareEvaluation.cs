namespace FiaeTrainer.Core.SoftwareDesign.Evaluation;

/// <summary> Strukturierte Bewertung ohne Interpretation freier Begründungen. </summary>
public sealed class SoftwareEvaluation
{
    /// <summary> Einzelkriterien mit insgesamt 50 internen Anteilen. </summary>
    public List<SoftwareCheck> Checks { get; set; } = [];
    /// <summary> Summe der Kriterienanteile. </summary>
    public int Earned => Checks.Sum(check => check.Earned);
    /// <summary> Vollständig passende Fachlösung. </summary>
    public bool Correct => Checks.Count > 0 && Earned == 50;
}
