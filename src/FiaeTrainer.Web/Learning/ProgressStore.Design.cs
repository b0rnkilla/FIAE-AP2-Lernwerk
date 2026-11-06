using FiaeTrainer.Core.DatabaseDesign.Content;
using FiaeTrainer.Core.DatabaseDesign.Evaluation;
using FiaeTrainer.Core.DatabaseDesign.Practice;

namespace FiaeTrainer.Web.Learning;

/// <summary> Speichert Datenbankentwürfe unter derselben atomaren Profilsperre wie die anderen Lernbereiche. </summary>
public sealed partial class ProgressStore
{
    /// <summary> Speichert einen individuellen Versuch und bewertet nur nach ausdrücklicher Abgabe. </summary>
    public async Task<DesignEvaluation?> SaveDesignAsync(int id, DesignProgress progress, DesignCatalog catalog, bool check = false)
    {
        await _gate.WaitAsync();
        try
        {
            TrainingProgress state = await ReadFileAsync();
            DesignAssignment task = catalog.Lessons.Single(item => item.Id == id);
            state.DesignLessons.TryGetValue(id, out var old);
            if ((old?.Revision ?? 0) != progress.Revision || progress.ContentVersion != DesignCatalog.CONTENT_VERSION || old != null && old.ContentVersion != progress.ContentVersion) throw new InvalidOperationException("Der Entwurf wurde geändert oder gehört zu einem anderen Inhaltsstand. Lade die Aufgabe neu.");
            progress.Completed |= old?.Completed ?? false;
            progress.Independent |= old?.Independent ?? false;
            DesignEvaluation? result = check ? DesignEvaluator.Evaluate(task, progress.Answer) : null;
            if (result != null) { progress.Attempts++; progress.Completed |= result.Correct; progress.Independent |= result.Correct && progress.IndependentMode && !progress.Assisted; }
            progress.Revision++;
            state.DesignLessons[id] = progress;
            state.LastDesignLessonId = id;
            await WriteFileAsync(state);
            return result;
        }
        finally { _gate.Release(); }
    }

    /// <summary> Erzeugt eine Prüfung erst nach einer Aktion, bei offenem Versuch nur nach explizitem Ersetzen. </summary>
    public async Task StartDesignExamAsync(DesignExamRound round, Guid? replaceId = null)
    {
        await _gate.WaitAsync();
        try
        {
            var state = await ReadFileAsync();
            if (state.DesignExam is { FinishedUtc: null } old && old.Id != replaceId) throw new InvalidOperationException("Eine Entwurfsprüfung ist noch offen. Bestätige den Neustart oder setze sie fort.");
            state.DesignExam = round;
            await WriteFileAsync(state);
        }
        finally { _gate.Release(); }
    }

    /// <summary> Fristablauf übernimmt nur rechtzeitig gespeicherte Entwürfe; Abgaben und Historie zählen genau einmal. </summary>
    public async Task<DesignExamRound> SaveDesignExamAsync(DesignExamRound round, DesignCatalog catalog, bool finish = false)
    {
        await _gate.WaitAsync();
        try
        {
            TrainingProgress state = await ReadFileAsync();
            DesignExamRound old = state.DesignExam ?? throw new InvalidOperationException("Die Entwurfsprüfung existiert nicht mehr.");
            if (old.Id != round.Id || old.Revision != round.Revision || old.FinishedUtc != null || round.CaseId != old.CaseId || round.StartedUtc != old.StartedUtc || round.ContentVersion != DesignCatalog.CONTENT_VERSION) throw new InvalidOperationException("Diese Prüfung wurde bereits geändert. Lade die Seite neu.");
            DesignExamCase item = catalog.Cases.Single(item => item.Id == old.CaseId);
            DateTimeOffset now = DateTimeOffset.UtcNow;
            bool expired = old.SecondsLeft(now) == 0;
            if (expired) { round.Answers = old.Answers; round.FlaggedIds = old.FlaggedIds; round.RemainingSeconds = 0; }
            else if (round.RunningSinceUtc == null) round.RemainingSeconds = old.SecondsLeft(now);
            else if (old.RunningSinceUtc != null) { round.RunningSinceUtc = old.RunningSinceUtc; round.RemainingSeconds = old.RemainingSeconds; }
            else { round.RunningSinceUtc = now; round.RemainingSeconds = old.RemainingSeconds; }
            round.UsedReference |= old.UsedReference;
            if (finish || expired)
            {
                round.Pause(now);
                round.FinishedUtc = now;
                round.Results = item.Tasks.ToDictionary(task => task.Id, task => DesignEvaluator.Evaluate(task, round.Answers.GetValueOrDefault(task.Id) ?? task.InitialAnswer()));
                round.Earned = item.Tasks.Select((task, index) => round.Results[task.Id].Earned * item.Points[index] / 50).Sum();
                state.DesignHistory.Insert(0, round);
                state.DesignHistory = state.DesignHistory.Take(20).ToList();
            }
            else { round.Results = []; round.Earned = 0; round.FinishedUtc = null; }
            round.Revision++;
            state.DesignExam = round;
            await WriteFileAsync(state);
            return round;
        }
        finally { _gate.Release(); }
    }

    /// <summary> Prüft das zusätzliche Profil, ohne alte Module oder Entwürfe zu verändern. </summary>
    private static void ValidateDesignProgress(TrainingProgress state)
    {
        if (state.DesignLessons == null || state.DesignHistory == null || state.LastDesignLessonId < 1 || state.DesignLessons.Any(pair => pair.Key < 1 || pair.Value == null || pair.Value.ContentVersion == null || pair.Value.Revision < 0 || pair.Value.HintLevel is < 0 or > 3 || pair.Value.Attempts < 0 || pair.Value.Independent && !pair.Value.Completed || !DesignModelRules.Storable(pair.Value.Answer))) throw new InvalidDataException("Ein gespeicherter Datenbankentwurf ist beschädigt.");
        foreach (var round in state.DesignHistory.Concat(state.DesignExam is { } current ? [current] : []))
        {
            if (round == null || round.Id == Guid.Empty || round.Revision < 0 || round.ContentVersion == null || round.CaseId == null || round.Index is < 0 or > 2 || round.RemainingSeconds is < 0 or > 2700 || round.Answers == null || round.Results == null || round.FlaggedIds == null || round.Answers.Any(pair => pair.Key is < 1 or > 3 || !DesignModelRules.Storable(pair.Value)) || round.FlaggedIds.Any(id => id is < 1 or > 3) || round.Earned is < 0 or > 50 || round.FinishedUtc == null && round.Results.Count != 0 || round.Results.Any(pair => pair.Key is < 1 or > 3 || pair.Value?.Checks == null || pair.Value.Checks.Any(check => check == null || check.Earned < 0 || check.Maximum < check.Earned))) throw new InvalidDataException("Eine gespeicherte Entwurfsprüfung ist beschädigt.");
            if (round.FinishedUtc != null && (!round.Results.Keys.Order().SequenceEqual(new[] { 1, 2, 3 }) || round.Results.Values.Any(result => result.Checks.Sum(check => check.Maximum) != 50) || round.Earned != round.Results.Sum(pair => pair.Value.Earned * (pair.Key == 3 ? 10 : 20) / 50))) throw new InvalidDataException("Die gespeicherte Entwurfsauswertung ist unvollständig.");
        }
    }
}
