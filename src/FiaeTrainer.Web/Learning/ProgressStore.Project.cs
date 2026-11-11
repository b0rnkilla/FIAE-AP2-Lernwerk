using FiaeTrainer.Core.ProjectManagement.Content;
using FiaeTrainer.Core.ProjectManagement.Evaluation;
using FiaeTrainer.Core.ProjectManagement.Practice;

namespace FiaeTrainer.Web.Learning;

/// <summary> Speichert Projektentwürfe unter derselben atomaren Profilsperre wie die anderen Lernbereiche. </summary>
public sealed partial class ProgressStore
{
    /// <summary> Speichert einen individuellen Versuch und bewertet nur nach ausdrücklicher Abgabe. </summary>
    public async Task<ProjectEvaluation?> SaveProjectAsync(int id, ProjectProgress progress, ProjectCatalog catalog, bool check = false)
    {
        await _gate.WaitAsync();
        try
        {
            TrainingProgress state = await ReadFileAsync();
            ProjectAssignment task = catalog.Lessons.Single(item => item.Id == id);
            state.ProjectLessons.TryGetValue(id, out var old);
            if ((old?.Revision ?? 0) != progress.Revision || progress.ContentVersion != ProjectCatalog.CONTENT_VERSION || old != null && old.ContentVersion != progress.ContentVersion) throw new InvalidOperationException("Der Entwurf wurde geändert oder gehört zu einem anderen Inhaltsstand. Lade die Aufgabe neu.");
            progress.Completed = old?.Completed ?? false;
            progress.Independent = old?.Independent ?? false;
            progress.Attempts = old?.Attempts ?? 0;
            ProjectEvaluation? result = check ? ProjectEvaluator.Evaluate(task, progress.Answer) : null;
            if (result != null) { progress.Attempts++; progress.Completed |= result.Correct; progress.Independent |= result.Correct && progress.IndependentMode && !progress.Assisted; }
            progress.Revision++;
            state.ProjectLessons[id] = progress;
            state.LastProjectLessonId = id;
            await WriteFileAsync(state);
            return result;
        }
        finally { _gate.Release(); }
    }

    /// <summary> Erzeugt eine Prüfung erst nach einer Aktion, bei offenem Versuch nur nach explizitem Ersetzen. </summary>
    public async Task StartProjectExamAsync(ProjectExamRound round, ProjectCatalog catalog, Guid? replaceId = null)
    {
        await _gate.WaitAsync();
        try
        {
            var state = await ReadFileAsync();
            if (state.ProjectExam is { FinishedUtc: null } old && old.Id != replaceId) throw new InvalidOperationException("Eine Projekt-Prüfung ist noch offen. Bestätige den Neustart oder setze sie fort.");
            if (!catalog.Cases.Any(item => item.Id == round.CaseId)) throw new InvalidOperationException("Dieser Prüfungsfall existiert nicht.");
            state.ProjectExam = round.Copy();
            await WriteFileAsync(state);
        }
        finally { _gate.Release(); }
    }

    /// <summary> Fristablauf übernimmt nur rechtzeitig gespeicherte Entwürfe; Abgaben und Historie zählen genau einmal. </summary>
    public async Task<ProjectExamRound> SaveProjectExamAsync(ProjectExamRound round, ProjectCatalog catalog, bool finish = false)
    {
        round = round.Copy();
        await _gate.WaitAsync();
        try
        {
            TrainingProgress state = await ReadFileAsync();
            ProjectExamRound old = state.ProjectExam ?? throw new InvalidOperationException("Die Projekt-Prüfung existiert nicht mehr.");
            if (old.Id != round.Id || old.Revision != round.Revision || old.FinishedUtc != null || round.CaseId != old.CaseId || round.StartedUtc != old.StartedUtc || old.ContentVersion != ProjectCatalog.CONTENT_VERSION || round.ContentVersion != ProjectCatalog.CONTENT_VERSION) throw new InvalidOperationException("Diese Prüfung wurde bereits geändert. Lade die Seite neu.");
            ProjectExamCase item = catalog.Cases.Single(item => item.Id == old.CaseId);
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
                round.Results = item.Tasks.ToDictionary(task => task.Id, task => ProjectEvaluator.Evaluate(task, round.Answers.GetValueOrDefault(task.Id) ?? task.InitialAnswer()));
                round.Earned = item.Tasks.Select((task, index) => round.Results[task.Id].Earned * item.Points[index] / 50).Sum();
                state.ProjectHistory.Insert(0, round);
                state.ProjectHistory = state.ProjectHistory.Take(20).ToList();
            }
            else { round.Results = []; round.Earned = 0; round.FinishedUtc = null; }
            round.Revision++;
            if (!catalog.Cases.Any(item => item.Id == round.CaseId)) throw new InvalidOperationException("Dieser Prüfungsfall existiert nicht.");
            state.ProjectExam = round.Copy();
            await WriteFileAsync(state);
            return round;
        }
        finally { _gate.Release(); }
    }

    /// <summary> Prüft das zusätzliche Profil, ohne alte Module oder Entwürfe zu verändern. </summary>
    private static void ValidateProjectProgress(TrainingProgress state)
    {
        if (state.ProjectLessons == null || state.ProjectHistory == null || state.LastProjectLessonId < 1 || state.ProjectLessons.Any(pair => pair.Key < 1 || pair.Value == null || pair.Value.ContentVersion == null || pair.Value.Revision < 0 || pair.Value.HintLevel is < 0 or > 3 || pair.Value.Attempts < 0 || pair.Value.Independent && !pair.Value.Completed || !ProjectRules.Storable(pair.Value.Answer))) throw new InvalidDataException("Ein gespeicherter Projektentwurf ist beschädigt.");
        foreach (var round in state.ProjectHistory.Concat(state.ProjectExam is { } current ? [current] : []))
        {
            if (round == null || round.Id == Guid.Empty || round.Revision < 0 || round.ContentVersion == null || round.CaseId == null || round.Index is < 0 or > 2 || round.RemainingSeconds is < 0 or > 2700 || round.Answers == null || round.Results == null || round.FlaggedIds == null || round.Answers.Any(pair => pair.Key is < 1 or > 3 || !ProjectRules.Storable(pair.Value)) || round.FlaggedIds.Any(id => id is < 1 or > 3) || round.Earned is < 0 or > 50 || round.FinishedUtc == null && round.Results.Count != 0 || round.Results.Any(pair => pair.Key is < 1 or > 3 || pair.Value?.Checks == null || pair.Value.Checks.Any(check => check == null || check.Earned < 0 || check.Maximum < check.Earned))) throw new InvalidDataException("Eine gespeicherte Projekt-Prüfung ist beschädigt.");
            if (round.FinishedUtc != null && (!round.Results.Keys.Order().SequenceEqual(new[] { 1, 2, 3 }) || round.Results.Values.Any(result => result.Checks.Sum(check => check.Maximum) != 50) || round.Earned != round.Results.Sum(pair => pair.Value.Earned * (pair.Key == 3 ? 10 : 20) / 50))) throw new InvalidDataException("Die gespeicherte Projektauswertung ist unvollständig.");
        }
    }
}
