namespace FiaeTrainer.Core.BusinessStudies.Evaluation;

/// <summary> Transparente Trainingsbewertung ohne Anspruch auf einen amtlichen IHK-Lösungsschlüssel. </summary>
public sealed record BusinessEvaluation(int Earned, int Maximum, bool Complete, IReadOnlyList<BusinessCheck> Checks)
{
    /// <summary> Nur eine vollständig richtige Antwort löst die Aufgabe. </summary>
    public bool Correct => Complete && Earned == Maximum;
    /// <summary> Anzeige des erreichten Anteils; ungerundet für Schwellenvergleiche verwenden. </summary>
    public decimal Percentage => Maximum == 0 ? 0 : 100m * Earned / Maximum;
}
