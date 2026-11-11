namespace FiaeTrainer.Core.ProjectManagement.Evaluation;

/// <summary> Fachliches Einzelkriterium; Punkte werden nur in der Prüfungsansicht gezeigt. </summary>
public sealed class ProjectCheck
{
    /// <summary> Verständlicher Name. </summary>
    public string Name { get; set; } = "";
    /// <summary> Erreichte interne Kriterienanteile. </summary>
    public int Earned { get; set; }
    /// <summary> Maximale Kriterienanteile. </summary>
    public int Maximum { get; set; }
    /// <summary> Hilfreiche Rückmeldung ohne unaufgeforderte Musterlösung. </summary>
    public string Detail { get; set; } = "";
}
