namespace FiaeTrainer.Core.ProjectManagement.Models;

/// <summary> Direkte Ende-Anfang-Beziehung ohne Zeitabstand zwischen zwei eigenen Vorgängen. </summary>
public sealed class ProjectDependency
{
    /// <summary> Stabile Identität der bearbeitbaren Verbindung. </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    /// <summary> Vorgängeridentität. </summary>
    public string SourceId { get; set; } = "";
    /// <summary> Nachfolgeridentität. </summary>
    public string TargetId { get; set; } = "";
}
