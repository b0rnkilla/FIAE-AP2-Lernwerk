namespace FiaeTrainer.Web.Learning;

/// <summary> Getrennter Entwurf und Lernversuch einer Pseudocode-Aufgabe ohne SQL-Eigenschaften. </summary>
public sealed class AlgorithmProgress
{
    /// <summary> Eigener Code, vorhergesagte Ausgaben oder Testfälle; null kennzeichnet einen noch nicht angelegten Reparaturentwurf. </summary>
    public string? Draft { get; set; }
    /// <summary> Neue Aufgaben starten ohne Hilfen. </summary>
    public bool ExamMode { get; set; } = true;
    /// <summary> Merkt jede Unterstützung bis zum ausdrücklichen Neustart dieses Versuchs. </summary>
    public bool AttemptAssisted { get; set; }
    /// <summary> Mindestens eine fachlich korrekte Abgabe. </summary>
    public bool Completed { get; set; }
    /// <summary> Mindestens ein korrekter Versuch ohne Hilfen. </summary>
    public bool Independent { get; set; }
    /// <summary> Anzahl abgegebener Antworten. </summary>
    public int Attempts { get; set; }
    /// <summary> Höchster geöffneter Hinweis dieses Versuchs. </summary>
    public int HintLevel { get; set; }
    /// <summary> Eine einmal aufgedeckte Musterlösung bleibt als Unterstützung vermerkt. </summary>
    public bool SolutionViewed { get; set; }

    /// <summary> Leert nur den aktuellen Versuch; bisherige Erfolge bleiben erhalten. </summary>
    public void StartIndependentAttempt(string initialDraft)
    {
        Draft = initialDraft;
        ExamMode = true;
        AttemptAssisted = false;
        HintLevel = 0;
        SolutionViewed = false;
    }
}
