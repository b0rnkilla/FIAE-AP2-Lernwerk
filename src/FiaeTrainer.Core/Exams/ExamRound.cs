using System.Text.Json.Serialization;

namespace FiaeTrainer.Core.Exams;

/// <summary> Fortsetzbare 45-Minuten-Prüfung mit eigenen Entwürfen und unveränderlicher Abgabe. </summary>
public sealed class ExamRound
{
    /// <summary> Unterscheidet neue und ersetzte Prüfungsrunden. </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary> Schutz vor veralteten Browserfenstern. </summary>
    public int Revision { get; set; }
    /// <summary> Fachlicher Prüfungsbereich. </summary>
    public ExamModule Module { get; set; }
    /// <summary> Version des festen Aufgabenvertrags. </summary>
    public string ContentVersion { get; set; } = "2026-11-05";
    /// <summary> SQL-Fallkennung; bei Algorithmen leer. </summary>
    public string CaseId { get; set; } = "";
    /// <summary> Beim Start festgelegte Aufgabenreihenfolge. </summary>
    public List<int> TaskIds { get; set; } = [];
    /// <summary> Zuletzt angezeigte Position. </summary>
    public int Index { get; set; }
    /// <summary> Ausschließlich eigene Prüfungsentwürfe, getrennt von Lernaufgaben. </summary>
    public Dictionary<int, string> Drafts { get; set; } = [];
    /// <summary> Persönliche Kontrollmarkierungen ohne Einfluss auf die Punkte. </summary>
    public HashSet<int> FlaggedIds { get; set; } = [];
    /// <summary> Erzeugungszeitpunkt. </summary>
    public DateTimeOffset StartedUtc { get; set; } = DateTimeOffset.UtcNow;
    /// <summary> Beginn des aktuellen Zeitabschnitts; null bedeutet ausdrückliche Pause. </summary>
    public DateTimeOffset? RunningSinceUtc { get; set; } = DateTimeOffset.UtcNow;
    /// <summary> Restzeit vor dem aktuellen Zeitabschnitt. </summary>
    public int RemainingSeconds { get; set; } = 2700;
    /// <summary> Frist ist abgelaufen; Entwürfe bleiben bis zur möglichen technischen Auswertung gesperrt. </summary>
    public bool Submitted { get; set; }
    /// <summary> Zeitpunkt der endgültigen Abgabe. </summary>
    public DateTimeOffset? SubmittedUtc { get; set; }
    /// <summary> Ergebnisse liegen vollständig vor. </summary>
    public DateTimeOffset? EvaluatedUtc { get; set; }
    /// <summary> Bewahrt die Kennzeichnung nach einem Spickzettelbesuch. </summary>
    public bool UsedReference { get; set; }
    /// <summary> Historische Ergebnisse; niemals während einer offenen Runde auswerten. </summary>
    public List<ExamTaskResult> Results { get; set; } = [];
    /// <summary> Eine Pause blendet den Aufgabeninhalt aus. </summary>
    [JsonIgnore] public bool Paused => RunningSinceUtc == null && !Submitted;
    /// <summary> Punktesumme der abgeschlossenen Bewertung. </summary>
    [JsonIgnore] public int Earned => Results.Sum(result => result.Earned);
    /// <summary> Uhr läuft auch bei geschlossenem Browser weiter. </summary>
    public int SecondsLeft(DateTimeOffset now) => Math.Max(0, RemainingSeconds - (RunningSinceUtc is { } since ? (int)Math.Max(0, (now - since).TotalSeconds) : 0));
    /// <summary> Friert die bereits abgelaufene Zeit ein, ohne die Frist zu verlängern. </summary>
    public void Pause(DateTimeOffset now) { RemainingSeconds = SecondsLeft(now); RunningSinceUtc = null; }
    /// <summary> Sperrt die Entwürfe vor einer gegebenenfalls später wiederholten technischen Bewertung. </summary>
    public void Submit(DateTimeOffset now) { Pause(now); Submitted = true; SubmittedUtc ??= now; }
}
