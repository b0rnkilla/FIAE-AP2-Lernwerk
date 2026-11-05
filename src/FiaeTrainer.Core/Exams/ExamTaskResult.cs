namespace FiaeTrainer.Core.Exams;

/// <summary> Gespeicherte Trainingspunkte einer Aufgabe, unabhängig von späteren Bewertungen. </summary>
public sealed class ExamTaskResult
{
    /// <summary> Dauerhafte Kennung innerhalb des Prüfungsmoduls. </summary>
    public int TaskId { get; set; }
    /// <summary> Damalige Teilprüfungen und ihre Rückmeldungen. </summary>
    public List<ExamCheck> Checks { get; set; } = [];
    /// <summary> Erreichte Trainingspunkte. </summary>
    public int Earned => Checks.Sum(check => check.Earned);
    /// <summary> Erreichbare Trainingspunkte. </summary>
    public int Maximum => Checks.Sum(check => check.Maximum);
}
