using System.Text.Json.Serialization;
using FiaeTrainer.Core.BusinessStudies.Evaluation;

namespace FiaeTrainer.Core.BusinessStudies.Practice;

/// <summary> Fortsetzbare Runde mit stabilen Aufgaben, Antworten und optionaler Prüfungsuhr. </summary>
public sealed class BusinessRound
{
    /// <summary> Eindeutige Kennung; trennt neue Runden und veraltete Browserfenster. </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary> Erlaubt das Erkennen konkurrierender Änderungen im gemeinsamen Profil. </summary>
    public int Revision { get; set; }
    /// <summary> Gewählter Lernweg. </summary>
    public BusinessRoundKind Kind { get; set; }
    /// <summary> Bei Themenrunden die gewählte Themenkennung. </summary>
    public string TopicId { get; set; } = "";
    /// <summary> Fachlicher Inhaltsstand beim Erzeugen der Runde. </summary>
    public string ContentVersion { get; set; } = "2026-11-04";
    /// <summary> Bereits festgelegte Reihenfolge ohne Aufgabenwiederholung. </summary>
    public List<int> QuestionIds { get; set; } = [];
    /// <summary> Position beim letzten Speichern. </summary>
    public int Index { get; set; }
    /// <summary> Gespeicherte Eingaben je Aufgabe. </summary>
    public Dictionary<int, BusinessAnswer> Answers { get; set; } = [];
    /// <summary> Bereits geprüfte Antworten sind innerhalb derselben Lernrunde unveränderlich. </summary>
    public HashSet<int> CheckedIds { get; set; } = [];
    /// <summary> Für eine spätere Kontrolle vorgemerkte Aufgaben. </summary>
    public HashSet<int> FlaggedIds { get; set; } = [];
    /// <summary> Erzeugungszeit in UTC. </summary>
    public DateTimeOffset StartedUtc { get; set; } = DateTimeOffset.UtcNow;
    /// <summary> Bei einer laufenden Prüfung der Beginn des aktuellen Zeitabschnitts. </summary>
    public DateTimeOffset? RunningSinceUtc { get; set; }
    /// <summary> Restzeit vor dem aktuellen laufenden Zeitabschnitt; nur die Prüfung verwendet sie. </summary>
    public int RemainingSeconds { get; set; } = 3600;
    /// <summary> Abschlusszeit; danach können Antworten nur gelesen werden. </summary>
    public DateTimeOffset? FinishedUtc { get; set; }
    /// <summary> Abgabeergebnis, das bei späteren Inhaltsänderungen als historische Punktzahl erhalten bleibt. </summary>
    public int Earned { get; set; }
    /// <summary> Damals erzielbare Trainingspunkte. </summary>
    public int Maximum { get; set; }
    /// <summary> Kennzeichnet eine Runde, in der vor der Abgabe der Spickzettel geöffnet wurde. </summary>
    public bool UsedReference { get; set; }
    /// <summary> Beim Öffnen des Spickzettels wird eine Prüfungsuhr ausdrücklich pausiert. </summary>
    [JsonIgnore] public bool Paused => Kind == BusinessRoundKind.Exam && RunningSinceUtc == null && FinishedUtc == null;

    /// <summary> Ermittelt die Restzeit aus UTC statt aus unzuverlässigen Browser-Timer-Ticks. </summary>
    public int SecondsLeft(DateTimeOffset now) => Math.Max(0, RemainingSeconds - (RunningSinceUtc is { } start ? (int)Math.Max(0, (now - start).TotalSeconds) : 0));

    /// <summary> Hält die laufende Prüfungsuhr an und bewahrt die tatsächlich verbleibende Zeit. </summary>
    public void Pause(DateTimeOffset now)
    {
        RemainingSeconds = SecondsLeft(now);
        RunningSinceUtc = null;
    }
}
