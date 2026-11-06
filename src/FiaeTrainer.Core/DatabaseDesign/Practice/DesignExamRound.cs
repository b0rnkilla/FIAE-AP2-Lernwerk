using FiaeTrainer.Core.DatabaseDesign.Content;
using FiaeTrainer.Core.DatabaseDesign.Evaluation;
using FiaeTrainer.Core.DatabaseDesign.Models;

namespace FiaeTrainer.Core.DatabaseDesign.Practice;

/// <summary> Unabhängige 45-Minuten-Modellprüfung mit unveränderlicher Abgabe und UTC-Zeitbudget. </summary>
public sealed class DesignExamRound
{
    /// <summary> Stabile Versuchserkennung. </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary> Revisionsschutz gegen konkurrierende Fenster. </summary>
    public int Revision { get; set; }
    /// <summary> Vertragsstand beim Start. </summary>
    public string ContentVersion { get; set; } = DesignCatalog.CONTENT_VERSION;
    /// <summary> Gewählter eigener Prüfungsfall. </summary>
    public string CaseId { get; set; } = "";
    /// <summary> Aktuelle Teilaufgabe. </summary>
    public int Index { get; set; }
    /// <summary> Startzeit ohne spätere Änderung. </summary>
    public DateTimeOffset StartedUtc { get; set; } = DateTimeOffset.UtcNow;
    /// <summary> Laufender Zeitabschnitt; null bedeutet ausdrücklich pausiert. </summary>
    public DateTimeOffset? RunningSinceUtc { get; set; } = DateTimeOffset.UtcNow;
    /// <summary> Restzeit vor dem aktuellen Zeitabschnitt. </summary>
    public int RemainingSeconds { get; set; } = 2700;
    /// <summary> Getrennte Entwürfe der drei Teilaufgaben. </summary>
    public Dictionary<int, DesignAnswer> Answers { get; set; } = [];
    /// <summary> Kontrollmarkierungen. </summary>
    public HashSet<int> FlaggedIds { get; set; } = [];
    /// <summary> Abgabezeit; danach sind Antworten gesperrt. </summary>
    public DateTimeOffset? FinishedUtc { get; set; }
    /// <summary> Nachschlagen kennzeichnet den gesamten Versuch. </summary>
    public bool UsedReference { get; set; }
    /// <summary> Historische Punktzahl, unabhängig von späteren Inhaltsänderungen. </summary>
    public int Earned { get; set; }
    /// <summary> Fachliche Detailprüfungen der gesperrten Abgabe. </summary>
    public Dictionary<int, DesignEvaluation> Results { get; set; } = [];
    /// <summary> Ermittelt die Restzeit auch nach App-Neustart. </summary>
    public int SecondsLeft(DateTimeOffset now) => Math.Max(0, RemainingSeconds - (RunningSinceUtc is { } start ? (int)Math.Max(0, (now - start).TotalSeconds) : 0));
    /// <summary> Hält das gespeicherte UTC-Budget an. </summary>
    public void Pause(DateTimeOffset now) { RemainingSeconds = SecondsLeft(now); RunningSinceUtc = null; }
}
