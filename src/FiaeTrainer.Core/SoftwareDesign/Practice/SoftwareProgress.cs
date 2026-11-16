using FiaeTrainer.Core.Learning;
using FiaeTrainer.Core.SoftwareDesign.Content;
using FiaeTrainer.Core.SoftwareDesign.Models;

namespace FiaeTrainer.Core.SoftwareDesign.Practice;

/// <summary> Individueller Modellversuch; frühere Abschlüsse bleiben bei Wiederholung erhalten. </summary>
public sealed class SoftwareProgress : StructuredLessonProgress<SoftwareAnswer>
{
    /// <summary> Inhaltsvertrag für den gespeicherten Entwurf. </summary>
    public string ContentVersion { get; set; } = SoftwareCatalog.CONTENT_VERSION;
    /// <summary> Erkennt veraltete Bearbeitungsfenster. </summary>
    public int Revision { get; set; }
    protected override SoftwareAnswer Copy(SoftwareAnswer answer) => answer.Copy();
}
