using System.Text.Json;
using FiaeTrainer.Core.SoftwareDesign.Content;
using FiaeTrainer.Core.SoftwareDesign.Evaluation;
using FiaeTrainer.Core.SoftwareDesign.Models;

namespace FiaeTrainer.Core.SoftwareDesign.Practice;

/// <summary> Unabhängige 45-Minuten-Modellprüfung mit unveränderlicher Abgabe und UTC-Zeitbudget. </summary>
public sealed class SoftwareExamRound
{
    /// <summary> Stabile Versuchserkennung. </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary> Revisionsschutz gegen konkurrierende Fenster. </summary>
    public int Revision { get; set; }
    /// <summary> Vertragsstand beim Start. </summary>
    public string ContentVersion { get; set; } = SoftwareCatalog.CONTENT_VERSION;
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
    public Dictionary<int, SoftwareAnswer> Answers { get; set; } = [];
    /// <summary> Kontrollmarkierungen. </summary>
    public HashSet<int> FlaggedIds { get; set; } = [];
    /// <summary> Abgabezeit; danach sind Antworten gesperrt. </summary>
    public DateTimeOffset? FinishedUtc { get; set; }
    /// <summary> Nachschlagen kennzeichnet den gesamten Versuch. </summary>
    public bool UsedReference { get; set; }
    /// <summary> Historische Punktzahl, unabhängig von späteren Inhaltsänderungen. </summary>
    public int Earned { get; set; }
    /// <summary> Fachliche Detailprüfungen der gesperrten Abgabe. </summary>
    public Dictionary<int, SoftwareEvaluation> Results { get; set; } = [];
    /// <summary> Erzeugt eine unveränderliche Übergabekopie für einen Speichervorgang. </summary>
    public SoftwareExamRound Copy() => JsonSerializer.Deserialize<SoftwareExamRound>(JsonSerializer.Serialize(this))!;
    /// <summary> Ermittelt die Restzeit auch nach App-Neustart. </summary>
    public int SecondsLeft(DateTimeOffset now) => Math.Max(0, RemainingSeconds - (RunningSinceUtc is { } start ? (int)Math.Max(0, (now - start).TotalSeconds) : 0));
    /// <summary> Hält das gespeicherte UTC-Budget an. </summary>
    public void Pause(DateTimeOffset now) { RemainingSeconds = SecondsLeft(now); RunningSinceUtc = null; }
}
