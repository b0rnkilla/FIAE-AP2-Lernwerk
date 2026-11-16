using FiaeTrainer.Core.Learning;
using FiaeTrainer.Core.Uml.Content;
using FiaeTrainer.Core.Uml.Models;

namespace FiaeTrainer.Core.Uml.Practice;

/// <summary> Individueller Modellversuch; frühere Abschlüsse bleiben bei Wiederholung erhalten. </summary>
public sealed class UmlProgress : StructuredLessonProgress<UmlAnswer>
{
    /// <summary> Inhaltsvertrag für den gespeicherten Entwurf. </summary>
    public string ContentVersion { get; set; } = UmlCatalog.CONTENT_VERSION;
    /// <summary> Erkennt veraltete Bearbeitungsfenster. </summary>
    public int Revision { get; set; }
    protected override UmlAnswer Copy(UmlAnswer answer) => answer.Copy();
}
