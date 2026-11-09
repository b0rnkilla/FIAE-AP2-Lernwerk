namespace FiaeTrainer.Core.Uml.Evaluation;

/// <summary> Ergebnis mit hervorgehobenen Elementen und getrennten Kriterien. </summary>
public sealed class UmlEvaluation
{
    /// <summary> Fachliche Kriterien mit zusammen fünfzig internen Punkten. </summary>
    public List<UmlCheck> Checks { get; set; } = [];
    /// <summary> Betroffene Diagrammteile ohne Koordinatenvergleich. </summary>
    public List<UmlIssue> Issues { get; set; } = [];
    /// <summary> Summe der erfüllten Kriterien. </summary>
    public int Earned => Checks.Sum(item => item.Earned);
    /// <summary> Nur die vollständig erfüllte Bewertung gilt als gelöst. </summary>
    public bool Correct => Checks.Count > 0 && Earned == 50;
}
