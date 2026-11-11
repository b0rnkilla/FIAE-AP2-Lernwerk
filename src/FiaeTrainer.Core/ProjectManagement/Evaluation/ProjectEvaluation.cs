namespace FiaeTrainer.Core.ProjectManagement.Evaluation;

/// <summary> Nach ausdrücklicher Prüfung berechnete Fachkriterien; Lernansichten zeigen keine Punkte. </summary>
public sealed class ProjectEvaluation
{
    /// <summary> Zusammen exakt 50 interne Anteile für die Prüfungsgewichtung. </summary>
    public List<ProjectCheck> Checks { get; set; } = [];
    /// <summary> Erreichte interne Anteile. </summary>
    public int Earned => Checks.Sum(check => check.Earned);
    /// <summary> Vollständiger fachlicher Erfolg. </summary>
    public bool Correct => Checks.Count > 0 && Earned == 50;
}
