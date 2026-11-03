namespace FiaeTrainer.Web.Learning;

/// <summary> Unterscheidet Abschluss, eigenständigen Erfolg und Unterstützung im aktuellen Versuch. </summary>
public sealed class LessonProgress
{
    /// <summary> Individueller Lernmodus dieser Aufgabe; neue und ältere Einträge beginnen ohne Hilfe. </summary>
    public bool ExamMode { get; set; } = true;
    /// <summary> Letzter SQL-Entwurf. </summary>
    public string Sql { get; set; } = "";
    /// <summary> Unterstützung im aktuellen Versuch; null kennzeichnet ältere Fortschrittsdateien. </summary>
    public bool? AttemptAssisted { get; set; }
    /// <summary> Ältere Entwürfe gelten vorsichtig als unterstützt, damit ein Neuladen keinen selbstständigen Erfolg erzeugt. </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public bool UsedAssistance => AttemptAssisted ?? (!string.IsNullOrWhiteSpace(Sql) || HintLevel > 0 || SolutionViewed);
    /// <summary> Mindestens eine korrekte Abgabe. </summary>
    public bool Completed { get; set; }
    /// <summary> Mindestens eine korrekte Abgabe im Modus ohne Hilfen. </summary>
    public bool Independent { get; set; }
    /// <summary> Höchste im laufenden Lernversuch angezeigte Hilfestufe. </summary>
    public int HintLevel { get; set; }
    /// <summary> Gibt an, ob die Musterlösung im laufenden Versuch aufgedeckt wurde. </summary>
    public bool SolutionViewed { get; set; }
    /// <summary> Anzahl der abgegebenen Antworten. </summary>
    public int Attempts { get; set; }

    /// <summary> Beginnt einen leeren Selbsttest; bisherige Erfolge und die Anzahl der Abgaben bleiben erhalten. </summary>
    public void StartIndependentAttempt()
    {
        ExamMode = true;
        Sql = "";
        HintLevel = 0;
        SolutionViewed = false;
        AttemptAssisted = false;
    }
}
