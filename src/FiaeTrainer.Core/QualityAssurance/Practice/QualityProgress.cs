using FiaeTrainer.Core.Learning;
using FiaeTrainer.Core.QualityAssurance.Content;
using FiaeTrainer.Core.QualityAssurance.Models;

namespace FiaeTrainer.Core.QualityAssurance.Practice;

/// <summary> Individueller Qualitätsversuch; frühere Abschlüsse bleiben bei Wiederholung erhalten. </summary>
public sealed class QualityProgress : StructuredLessonProgress<QualityAnswer>
{
    /// <summary> Inhaltsvertrag für den gespeicherten Entwurf. </summary>
    public string ContentVersion { get; set; } = QualityCatalog.CONTENT_VERSION;
    /// <summary> Erkennt veraltete Bearbeitungsfenster. </summary>
    public int Revision { get; set; }
    protected override QualityAnswer Copy(QualityAnswer answer) => answer.Copy();
}
