using FiaeTrainer.Core.Learning;
using FiaeTrainer.Core.DatabaseDesign.Content;
using FiaeTrainer.Core.DatabaseDesign.Models;

namespace FiaeTrainer.Core.DatabaseDesign.Practice;

/// <summary> Individueller Modellversuch; frühere Abschlüsse bleiben bei Wiederholung erhalten. </summary>
public sealed class DesignProgress : StructuredLessonProgress<DesignAnswer>
{
    /// <summary> Inhaltsvertrag für den gespeicherten Entwurf. </summary>
    public string ContentVersion { get; set; } = DesignCatalog.CONTENT_VERSION;
    /// <summary> Erkennt veraltete Bearbeitungsfenster. </summary>
    public int Revision { get; set; }
    protected override DesignAnswer Copy(DesignAnswer answer) => answer.Copy();
}
