using FiaeTrainer.Core.Exams;

namespace FiaeTrainer.Web.Learning;

public sealed partial class ProgressStore
{
    /// <summary> Startet erst nach Bestätigung eine neue Prüfung; andere Lernbereiche bleiben erhalten. </summary>
    public async Task StartExamAsync(ExamRound round, Guid? replaceId = null)
    {
        await _gate.WaitAsync();
        try
        {
            TrainingProgress state = await ReadFileAsync();
            if (state.ExamRounds.TryGetValue(round.Module, out ExamRound? old) && old.EvaluatedUtc == null && old.Id != replaceId)
                throw new InvalidOperationException("Eine Prüfung ist noch offen. Setze sie fort oder bestätige den neuen Versuch.");
            state.ExamRounds[round.Module] = round;
            await WriteFileAsync(state);
        }
        finally { _gate.Release(); }
    }

    /// <summary> Erkennt veraltete Fenster und erzwingt die gespeicherte Zeitgrenze auch nach Neustart oder verspätetem Pausieren. </summary>
    public async Task<ExamRound> SaveExamAsync(ExamRound round, bool submit = false)
    {
        await _gate.WaitAsync();
        try
        {
            TrainingProgress state = await ReadFileAsync();
            ExamRound old = CurrentExam(state, round.Module, round.Id, round.Revision);
            if (old.Submitted) throw new InvalidOperationException("Diese Prüfung ist bereits abgegeben. Lade sie neu, um die Auswertung zu sehen.");
            if (!old.TaskIds.SequenceEqual(round.TaskIds) || old.CaseId != round.CaseId || old.ContentVersion != round.ContentVersion || old.StartedUtc != round.StartedUtc)
                throw new InvalidOperationException("Der Aufgabenbestand wurde verändert. Lade die Prüfung neu.");
            DateTimeOffset now = DateTimeOffset.UtcNow;
            bool expired = old.SecondsLeft(now) == 0;
            if (expired)
            {
                round.Drafts = old.Drafts;
                round.FlaggedIds = old.FlaggedIds;
                round.RemainingSeconds = 0;
                round.Submit(now);
            }
            else
            {
                if (old.RunningSinceUtc != null && round.RunningSinceUtc != null) { round.RunningSinceUtc = old.RunningSinceUtc; round.RemainingSeconds = old.RemainingSeconds; }
                else if (round.RunningSinceUtc == null) round.RemainingSeconds = old.SecondsLeft(now);
                else { round.RunningSinceUtc = now; round.RemainingSeconds = old.RemainingSeconds; }
                if (submit) round.Submit(now);
            }
            round.UsedReference |= old.UsedReference;
            round.Results = [];
            round.Revision++;
            state.ExamRounds[round.Module] = round;
            await WriteFileAsync(state);
            return round;
        }
        finally { _gate.Release(); }
    }

    /// <summary> Hält den Spickzettelbesuch fest; ein technischer Fehler verlängert niemals die Prüfungsfrist. </summary>
    public async Task<ExamRound?> MarkExamReferenceAsync(ExamModule module)
    {
        ExamRound? round = (await LoadAsync()).ExamRounds.GetValueOrDefault(module);
        if (round is null || round.Submitted || round.Paused && round.UsedReference) return round;
        round.Pause(DateTimeOffset.UtcNow);
        round.UsedReference = true;
        return await SaveExamAsync(round);
    }

    /// <summary> Speichert nur eine vollständige Bewertung der bereits gesperrten Abgabe; erneute technische Versuche zählen nicht mehrfach. </summary>
    public async Task<ExamRound> CompleteExamAsync(ExamRound round, List<ExamTaskResult> results)
    {
        await _gate.WaitAsync();
        try
        {
            TrainingProgress state = await ReadFileAsync();
            ExamRound old = CurrentExam(state, round.Module, round.Id, round.Revision);
            if (!old.Submitted || old.EvaluatedUtc != null || results.Any(result => result == null || result.Checks == null || result.Checks.Any(check => check == null || check.Earned < 0 || check.Maximum < check.Earned)) || !results.Select(result => result.TaskId).SequenceEqual(old.TaskIds) || results.Sum(result => result.Maximum) != 50)
                throw new InvalidOperationException("Die Abgabe kann nicht erneut oder unvollständig bewertet werden.");
            old.Results = results;
            old.EvaluatedUtc = DateTimeOffset.UtcNow;
            old.Revision++;
            state.ExamRounds[old.Module] = old;
            state.ExamHistory.Insert(0, old);
            state.ExamHistory = state.ExamHistory.Where(item => item.Module == old.Module).Take(20).Concat(state.ExamHistory.Where(item => item.Module != old.Module).Take(20)).OrderByDescending(item => item.SubmittedUtc).ToList();
            await WriteFileAsync(state);
            return old;
        }
        finally { _gate.Release(); }
    }

    /// <summary> Liefert nur die unveränderte gespeicherte Revision des gewünschten Moduls. </summary>
    private static ExamRound CurrentExam(TrainingProgress state, ExamModule module, Guid id, int revision)
    {
        if (!state.ExamRounds.TryGetValue(module, out ExamRound? old) || old.Id != id || old.Revision != revision)
            throw new InvalidOperationException("Die Prüfung wurde in einem anderen Fenster geändert. Lade die Seite neu, bevor du weiterarbeitest.");
        return old;
    }

    /// <summary> Verhindert stilles Überschreiben beschädigter oder fremd zugeordneter Prüfungsdaten. </summary>
    private static void ValidateExamProgress(TrainingProgress state)
    {
        if (state.ExamRounds == null || state.ExamHistory == null || state.ExamRounds.Any(pair => pair.Value == null || pair.Key != pair.Value.Module)) throw new InvalidDataException("Prüfungsdaten sind beschädigt.");
        foreach (ExamRound round in state.ExamRounds.Values.Concat(state.ExamHistory))
        {
            if (round == null || !Enum.IsDefined(round.Module) || round.Id == Guid.Empty || round.Revision < 0 || round.ContentVersion == null || round.CaseId == null || round.TaskIds == null || round.TaskIds.Count != (round.Module == ExamModule.Sql ? 5 : 3) || round.TaskIds.Any(id => id < 1) || round.TaskIds.Distinct().Count() != round.TaskIds.Count || round.Index < 0 || round.Index >= round.TaskIds.Count || round.Drafts == null || round.FlaggedIds == null || round.Results == null || round.RemainingSeconds is < 0 or > 2700 || round.Drafts.Any(pair => !round.TaskIds.Contains(pair.Key) || pair.Value == null || pair.Value.Length > 12000) || round.FlaggedIds.Any(id => !round.TaskIds.Contains(id)) || round.Submitted != round.SubmittedUtc.HasValue || round.EvaluatedUtc != null && !round.Submitted)
                throw new InvalidDataException("Eine gespeicherte Prüfung ist beschädigt und wird nicht überschrieben.");
            if (round.Results.Any(result => result == null || result.Checks == null || result.Checks.Any(check => check == null || check.Earned < 0 || check.Maximum < check.Earned)) || round.EvaluatedUtc == null && round.Results.Count != 0 || round.EvaluatedUtc != null && (!round.Results.Select(result => result.TaskId).SequenceEqual(round.TaskIds) || round.Results.Sum(result => result.Maximum) != 50)) throw new InvalidDataException("Gespeicherte Prüfungsergebnisse sind beschädigt.");
        }
    }
}
