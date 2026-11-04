namespace FiaeTrainer.Web.Learning;

/// <summary> Eigener WiSo-Lernstand; ein früherer Erfolg bleibt bei späteren Fehlern sichtbar. </summary>
public sealed class BusinessProgress
{
    /// <summary> Zahl tatsächlich geprüfter Antworten, nicht bloßer Seitenaufrufe. </summary>
    public int Attempts { get; set; }
    /// <summary> Mindestens einmal vollständig richtig gelöst. </summary>
    public bool Completed { get; set; }
    /// <summary> Erste Antwort einer Runde richtig und nicht als unsicher markiert. </summary>
    public bool Independent { get; set; }
    /// <summary> Die zuletzt geprüfte Antwort war vollständig richtig. </summary>
    public bool LastCorrect { get; set; }
    /// <summary> Die zuletzt geprüfte Antwort war als unsicher markiert. </summary>
    public bool Unsure { get; set; }
    /// <summary> UTC-Zeit der letzten Prüfung. </summary>
    public DateTimeOffset LastAnsweredUtc { get; set; }
    /// <summary> Fehler und Unsicherheit sind Vorrang für die gezielte Wiederholung. </summary>
    public bool NeedsReview => Attempts > 0 && (!LastCorrect || Unsure);
}
