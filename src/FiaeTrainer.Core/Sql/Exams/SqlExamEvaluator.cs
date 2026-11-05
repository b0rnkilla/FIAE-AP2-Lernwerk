using FiaeTrainer.Core.Exams;
using FiaeTrainer.Core.Sql.Database;
using FiaeTrainer.Core.Sql.Results;

namespace FiaeTrainer.Core.Sql.Exams;

/// <summary> Bewertet jede Antwort auf sichtbaren und zusätzlichen Daten, unabhängig von vorherigen Aufgaben. </summary>
public sealed class SqlExamEvaluator(TrainingDatabase database)
{
    /// <summary> Formale Punkte und funktionale Punkte werden getrennt ausgewiesen; technische Störungen bleiben unbenotet. </summary>
    public async Task<ExamTaskResult> EvaluateAsync(SqlExamCase item, SqlExamTask task, string answer)
    {
        ExamTaskResult result = new() { TaskId = task.Id };
        string? error = SqlExamValidator.Prepare(item, task, answer, out _);
        if (error != null) { result.Checks.Add(new("Ausführbare Anweisung", 0, 10, error)); return result; }
        bool valuesCorrect = true;
        bool orderCorrect = true;
        for (int index = 0; index < 2; index++)
        {
            SqlExamRun actual = await database.ExecuteExamAsync(item, task, answer, index == 1);
            SqlExamRun expected = await database.ExecuteExamAsync(item, task, task.ReferenceSql, index == 1);
            if (actual.TechnicalFailure || expected.TechnicalFailure) throw new IOException("Die SQL-Verbindung ist für die Auswertung nicht verfügbar. Deine Abgabe bleibt gespeichert und gesperrt. Richte SQL ein und versuche die Auswertung erneut.");
            if (expected.Error != null) throw new InvalidDataException("Die SQL-Prüfungsreferenz ist nicht ausführbar: " + expected.Error);
            if (actual.Error != null) { result.Checks.Add(new("Ausführbare Anweisung", 0, 10, actual.Error)); return result; }
            if (task.Operation == "Query")
            {
                valuesCorrect &= ResultComparer.AreEqual(expected.Query, actual.Query, false);
                orderCorrect &= ResultComparer.AreEqual(expected.Query, actual.Query, task.Ordered);
            }
            else valuesCorrect &= expected.State.Count == actual.State.Count && expected.State.All(pair => actual.State.TryGetValue(pair.Key, out QueryResult? value) && ResultComparer.AreEqual(pair.Value, value, true));
        }
        result.Checks.Add(new("Ausführbare Anweisung und Schreibregeln", 2, 2, "Geforderte Anweisungsart, erlaubte Tabellen und ausführbare SQL-Syntax."));
        if (task.Operation == "Query" && task.Ordered)
        {
            result.Checks.Add(new("Ergebniswerte auf beiden Datenbeständen", valuesCorrect ? 6 : 0, 6, "Spaltenreihenfolge, Werte, NULL und Duplikate müssen auf Ausgangs- und Prüfdaten stimmen."));
            result.Checks.Add(new("Geforderte Sortierung", valuesCorrect && orderCorrect ? 2 : 0, 2, "Sortierpunkte setzen auch die richtigen Ergebniswerte voraus."));
        }
        else result.Checks.Add(new(task.Operation == "Query" ? "Ergebniswerte auf beiden Datenbeständen" : "Vollständiger Datenstand und Spaltenstruktur", valuesCorrect ? 8 : 0, 8, "Die fachliche Regel muss auch mit zusätzlichen Daten stimmen. Unbeteiligte Tabellen bleiben unverändert."));
        return result;
    }
}
