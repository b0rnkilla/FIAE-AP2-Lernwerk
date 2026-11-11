using System.Text.Json;

namespace FiaeTrainer.Core.ProjectManagement.Models;

/// <summary> Strukturierte Fachangaben und eigene Pläne ohne zusätzliche Freitextbegründung. </summary>
public sealed class ProjectAnswer
{
    /// <summary> Benannte Antwortfelder und Rechenzellen. </summary>
    public Dictionary<string, string> Values { get; set; } = [];
    /// <summary> Stabile Kennungen gewählter Aussagen beziehungsweise Reviewbefunde. </summary>
    public List<string> SelectedIds { get; set; } = [];
    /// <summary> Eigene Arbeitspakete, Gruppen und Meilensteine. </summary>
    public List<ProjectActivity> Activities { get; set; } = [];
    /// <summary> Direkt eingegebene gerichtete Vorgängerbeziehungen. </summary>
    public List<ProjectDependency> Dependencies { get; set; } = [];
    /// <summary> Unabhängige Kopie für Vorlagen und atomare Speicherübergaben. </summary>
    public ProjectAnswer Copy() => JsonSerializer.Deserialize<ProjectAnswer>(JsonSerializer.Serialize(this))!;
}
