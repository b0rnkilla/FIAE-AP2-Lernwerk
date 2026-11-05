using FiaeTrainer.Core.Algorithms.Exams;
using FiaeTrainer.Core.Exams;
using FiaeTrainer.Core.Sql.Exams;
using FiaeTrainer.Web.Sql;

namespace FiaeTrainer.Web.Exams;

/// <summary> Verbindet die getrennten fachlichen Prüfungen mit der gemeinsamen Rundenseite. </summary>
public sealed class ExamService(AlgorithmExamCatalog algorithms, AlgorithmExamEvaluator algorithmEvaluator, SqlExamCatalog sql, DatabaseState database)
{
    /// <summary> Eigene SQL-Fälle für die Auswahl vor dem Start. </summary>
    public IReadOnlyList<SqlExamCase> SqlCases => sql.Cases;
    /// <summary> Erzeugt einen fachlich passenden, bis zur Abgabe festen Aufgabenmix. </summary>
    public ExamRound Create(ExamModule module, string? caseId = null) => module == ExamModule.Sql ? sql.CreateRound(caseId) : algorithms.CreateRound();
    /// <summary> Liefert ausschließlich die zum SQL-Fall gehörenden Tabellen. </summary>
    public SqlExamCase SqlCase(ExamRound round) => sql.Cases.Single(item => item.Id == round.CaseId);
    /// <summary> Liefert eine Aufgabe aus dem separaten Algorithmus-Prüfungspool. </summary>
    public AlgorithmExamTask AlgorithmTask(int id) => algorithms.Tasks.Single(task => task.Lesson.Id == id);
    /// <summary> Eindeutiger Titel, ohne Bewertungsdaten an die Oberfläche einer offenen Prüfung zu geben. </summary>
    public string Title(ExamRound round, int id) => round.Module == ExamModule.Sql ? SqlCase(round).Tasks.Single(task => task.Id == id).Title : AlgorithmTask(id).Lesson.Title;
    /// <summary> Verhindert die Fortsetzung mit unpassendem oder verändertem Aufgabenbestand. </summary>
    public bool IsCompatible(ExamRound round) => round.ContentVersion == "2026-11-05" && (round.Module == ExamModule.Sql
        ? sql.Cases.Any(item => item.Id == round.CaseId && item.Tasks.Select(task => task.Id).SequenceEqual(round.TaskIds))
        : round.CaseId.Length == 0 && round.TaskIds.Count == 3 && round.TaskIds[0] is >= 1 and <= 3 && round.TaskIds[1] is >= 4 and <= 8 && round.TaskIds[2] is 9 or 10);
    /// <summary> Bewertet nur eine gesperrte Abgabe; technische SQL-Störungen liefern keine Nullpunkte. </summary>
    public async Task<List<ExamTaskResult>> EvaluateAsync(ExamRound round)
    {
        if (!round.Submitted || !IsCompatible(round)) throw new InvalidOperationException("Die Prüfung muss zuerst vollständig abgegeben werden.");
        if (round.Module == ExamModule.Sql && !database.Ready) throw new IOException("Die SQL-Verbindung fehlt. Deine Abgabe ist gespeichert. Richte SQL ein und starte die Auswertung erneut.");
        List<ExamTaskResult> results = [];
        foreach (int id in round.TaskIds)
        {
            string answer = round.Drafts.GetValueOrDefault(id, "");
            results.Add(round.Module == ExamModule.Sql
                ? await new SqlExamEvaluator(database.Database).EvaluateAsync(SqlCase(round), SqlCase(round).Tasks.Single(task => task.Id == id), answer)
                : algorithmEvaluator.Evaluate(AlgorithmTask(id), answer));
        }
        return results;
    }
}
