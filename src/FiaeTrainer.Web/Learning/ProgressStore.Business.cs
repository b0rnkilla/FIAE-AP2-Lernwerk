using FiaeTrainer.Core.BusinessStudies.Content;
using FiaeTrainer.Core.BusinessStudies.Evaluation;
using FiaeTrainer.Core.BusinessStudies.Practice;

namespace FiaeTrainer.Web.Learning;

/// <summary> Speichert WiSo-Runden unter derselben Dateisperre wie SQL und Algorithmen. </summary>
public sealed partial class ProgressStore
{
    /// <summary> Beginnt eine neue Runde; vorhandene offene Runden werden nur nach ausdrücklicher Auswahl ersetzt. </summary>
    public async Task StartBusinessRoundAsync(BusinessRound round, Guid? replaceId = null)
    {
        await _gate.WaitAsync();
        try
        {
            TrainingProgress state = await ReadFileAsync();
            if (state.BusinessRound is { FinishedUtc: null } existing && existing.Id != replaceId)
                throw new InvalidOperationException("Es gibt bereits eine offene WiSo-Runde. Setze sie fort oder bestätige den Neustart.");
            state.BusinessRound = round;
            await WriteFileAsync(state);
        }
        finally { _gate.Release(); }
    }

    /// <summary> Bewahrt andere Module und erkennt veraltete Fenster. Eine Antwort zählt höchstens einmal pro Runde. </summary>
    public async Task SaveBusinessRoundAsync(BusinessRound round, BusinessCatalog catalog, int? checkId = null, bool finish = false)
    {
        await _gate.WaitAsync();
        try
        {
            TrainingProgress state = await ReadFileAsync();
            BusinessRound previous = state.BusinessRound ?? throw new InvalidOperationException("Die WiSo-Runde wurde inzwischen zurückgesetzt. Lade die Seite neu.");
            if (previous.Id != round.Id || previous.Revision != round.Revision || previous.FinishedUtc != null)
                throw new InvalidOperationException("Diese WiSo-Runde wurde in einem anderen Fenster geändert. Lade die Seite neu, bevor du weiterarbeitest.");
            if (!previous.QuestionIds.SequenceEqual(round.QuestionIds) || previous.Kind != round.Kind || previous.StartedUtc != round.StartedUtc || round.QuestionIds.Any(id => !catalog.Questions.Any(question => question.Id == id)))
                throw new InvalidOperationException("Der Aufgabenbestand dieser Runde passt nicht mehr. Starte eine neue WiSo-Runde.");
            // Bereits aufgelöste Antworten bleiben bis zur nächsten Runde gesperrt, auch in einem zweiten Fenster.
            foreach (int id in previous.CheckedIds)
                if (previous.Answers.TryGetValue(id, out BusinessAnswer? answer)) round.Answers[id] = answer;
            round.CheckedIds = previous.CheckedIds;
            DateTimeOffset now = DateTimeOffset.UtcNow;
            bool expired = previous.Kind == BusinessRoundKind.Exam && previous.SecondsLeft(now) == 0;
            finish |= expired;
            if (checkId is int checkedId && !finish)
            {
                if (round.Kind == BusinessRoundKind.Exam || !round.QuestionIds.Contains(checkedId)) throw new InvalidOperationException("Diese Antwort kann erst bei der Abgabe geprüft werden.");
                BusinessEvaluation evaluation = BusinessEvaluator.Evaluate(catalog.Questions.Single(question => question.Id == checkedId), round.Answers.GetValueOrDefault(checkedId) ?? new());
                if (!evaluation.Complete) throw new InvalidOperationException("Bitte fülle alle Antwortfelder im angegebenen Format aus.");
                if (round.CheckedIds.Add(checkedId)) Record(state, checkedId, evaluation, round.Answers[checkedId].Unsure || round.UsedReference, now);
            }
            if (finish)
            {
                round.Pause(now);
                round.FinishedUtc = now;
                round.Earned = 0;
                round.Maximum = 0;
                foreach (int id in round.QuestionIds)
                {
                    BusinessAnswer answer = round.Answers.GetValueOrDefault(id) ?? new();
                    BusinessEvaluation evaluation = BusinessEvaluator.Evaluate(catalog.Questions.Single(question => question.Id == id), answer);
                    round.Earned += evaluation.Earned;
                    round.Maximum += evaluation.Maximum;
                    if (round.CheckedIds.Add(id)) Record(state, id, evaluation, answer.Unsure || round.UsedReference, now);
                }
                state.BusinessHistory.Insert(0, round);
                state.BusinessHistory = state.BusinessHistory.Take(20).ToList();
            }
            round.Revision++;
            state.BusinessRound = round;
            await WriteFileAsync(state);
        }
        finally { _gate.Release(); }
    }

    /// <summary> Historische Erfolge bleiben bestehen, der letzte Fehlversuch wird dennoch für Wiederholungen vorgemerkt. </summary>
    private static void Record(TrainingProgress state, int id, BusinessEvaluation evaluation, bool unsure, DateTimeOffset now)
    {
        BusinessProgress progress = state.BusinessQuestions.GetValueOrDefault(id) ?? new();
        progress.Attempts++;
        progress.Completed |= evaluation.Correct;
        progress.Independent |= evaluation.Correct && !unsure;
        progress.LastCorrect = evaluation.Correct;
        progress.Unsure = unsure;
        progress.LastAnsweredUtc = now;
        state.BusinessQuestions[id] = progress;
    }

    /// <summary> Verhindert stilles Überschreiben beschädigter WiSo-Profile; kennt keine SQL-Abhängigkeit. </summary>
    private static void ValidateBusinessProgress(TrainingProgress state)
    {
        if (state.BusinessQuestions == null || state.BusinessHistory == null || state.BusinessQuestions.Any(entry => entry.Key < 1 || entry.Value == null || entry.Value.Attempts < 0 || entry.Value.Independent && !entry.Value.Completed))
            throw new InvalidDataException("Der WiSo-Lernstand ist beschädigt und wird nicht überschrieben.");
        foreach (BusinessRound round in state.BusinessHistory.Concat(state.BusinessRound is { } current ? [current] : []))
        {
            if (round == null || round.Id == Guid.Empty || !Enum.IsDefined(round.Kind) || round.QuestionIds == null || round.QuestionIds.Count == 0 || round.QuestionIds.Any(id => id < 1) || round.QuestionIds.Distinct().Count() != round.QuestionIds.Count || round.Index < 0 || round.Index >= round.QuestionIds.Count || round.Answers == null || round.CheckedIds == null || round.FlaggedIds == null || round.Revision < 0 || round.RemainingSeconds is < 0 or > 3600 || round.Maximum < round.Earned || round.Earned < 0)
                throw new InvalidDataException("Eine gespeicherte WiSo-Runde ist beschädigt und wird nicht überschrieben.");
            if (round.Answers.Any(entry => !round.QuestionIds.Contains(entry.Key) || entry.Value == null || entry.Value.SelectedIds == null || entry.Value.Values == null || entry.Value.Order == null || entry.Value.Values.Any(value => value.Value == null)) || round.CheckedIds.Any(id => !round.QuestionIds.Contains(id)) || round.FlaggedIds.Any(id => !round.QuestionIds.Contains(id)))
                throw new InvalidDataException("Gespeicherte WiSo-Antworten sind beschädigt und werden nicht überschrieben.");
        }
    }
}
