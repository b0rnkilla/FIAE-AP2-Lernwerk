using FiaeTrainer.Core.Learning;
using FiaeTrainer.Core.ProjectManagement.Content;
using FiaeTrainer.Core.ProjectManagement.Models;

namespace FiaeTrainer.Core.ProjectManagement.Practice;

/// <summary> Individueller Projektversuch; frühere Abschlüsse bleiben bei Wiederholung erhalten. </summary>
public sealed class ProjectProgress : StructuredLessonProgress<ProjectAnswer>
{
    /// <summary> Inhaltsvertrag für den gespeicherten Entwurf. </summary>
    public string ContentVersion { get; set; } = ProjectCatalog.CONTENT_VERSION;
    /// <summary> Erkennt veraltete Bearbeitungsfenster. </summary>
    public int Revision { get; set; }
    protected override ProjectAnswer Copy(ProjectAnswer answer) => answer.Copy();
}
