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
        var failedCriteria = new HashSet<string>();
        bool valuesCorrect = true;
        bool orderCorrect = true;
        string? difference = null;
        for (int index = 0; index < 2; index++)
        {
            SqlExamRun actual = await database.ExecuteExamAsync(item, task, answer, index == 1);
            SqlExamRun expected = await database.ExecuteExamAsync(item, task, task.ReferenceSql, index == 1);
            if (actual.TechnicalFailure || expected.TechnicalFailure) throw new IOException("Die SQL-Verbindung ist für die Auswertung nicht verfügbar. Deine Abgabe bleibt gespeichert und gesperrt. Richte SQL ein und versuche die Auswertung erneut.");
            if (expected.Error != null) throw new InvalidDataException("Die SQL-Prüfungsreferenz ist nicht ausführbar: " + expected.Error);
            if (actual.Error != null) { result.Checks.Add(new("Ausführbare Anweisung", 0, 10, actual.Error)); return result; }
            if (task.Operation == "Query")
            {
                bool equal = ResultComparer.AreEqual(expected.Query, actual.Query, false);
                if (!equal && difference == null)
                {
                    string dataset = index == 0 ? "Bei den Ausgangsdaten" : "Bei den zusätzlichen Prüfdaten";
                    difference = expected.Query.Columns.Count != actual.Query.Columns.Count
                        ? $"{dataset} lieferst du {actual.Query.Columns.Count} statt {expected.Query.Columns.Count} Spalten. Prüfe deine SELECT-Liste."
                        : expected.Query.Rows.Count != actual.Query.Rows.Count
                            ? $"{dataset} lieferst du {actual.Query.Rows.Count} statt {expected.Query.Rows.Count} Zeilen. Prüfe, welche Datensätze dein Ausdruck einschließt und ob Zeilen mehrfach vorkommen."
                            : $"{dataset} stimmt die Zeilenzahl. Mindestens ein Wert, seine Spaltenposition oder die Häufigkeit einer Ergebniszeile weicht ab.";
                }
                valuesCorrect &= equal;
                orderCorrect &= ResultComparer.AreEqual(expected.Query, actual.Query, task.Ordered);
            }
            else if (task.Criteria.Length > 0)
            {
                foreach (var criterion in task.Criteria)
                    if (!ResultComparer.AreEqual(expected.State[criterion.Name], actual.State[criterion.Name], false))
                        failedCriteria.Add(criterion.Name);
            }
            else
            {
                foreach (var (name, state) in expected.State)
                    if (!actual.State.TryGetValue(name, out QueryResult? value) || !ResultComparer.AreEqual(state, value, true))
                    {
                        valuesCorrect = false;
                        string source = index == 0 ? "den Ausgangsdaten" : "den zusätzlichen Prüfdaten";
                        difference ??= $"Bei {source} weicht {(name.EndsWith(":Spalten") ? "die Spaltenstruktur" : "der Datenstand")} von {name.Split(':')[0]} nach deiner Anweisung ab.";
                    }
            }
        }
        result.Checks.Add(new("Ausführbare Anweisung und Schreibregeln", 2, 2, "Geforderte Anweisungsart, erlaubte Tabellen und ausführbare SQL-Syntax."));
        if (task.Criteria.Length > 0)
        {
            foreach (var criterion in task.Criteria)
            {
                bool passed = !failedCriteria.Contains(criterion.Name);
                result.Checks.Add(new(criterion.Name, passed ? criterion.Points : 0, criterion.Points,
                    passed ? "Auf beiden Datenständen erfüllt." : criterion.Failure));
            }
        }
        else if (task.Operation == "Query" && task.Ordered)
        {
            result.Checks.Add(new("Ergebniswerte auf beiden Datenbeständen", valuesCorrect ? 6 : 0, 6, difference ?? "Spalten, Werte und Wiederholungen stimmen auf beiden Datenbeständen."));
            result.Checks.Add(new("Geforderte Sortierung", valuesCorrect && orderCorrect ? 2 : 0, 2, !valuesCorrect ? "Die Sortierung wird bei korrekten Ergebniswerten gewertet." : orderCorrect ? "Die Zeilen stehen in der verlangten Reihenfolge." : "Alle Ergebniswerte stimmen. Prüfe die Richtung und die weiteren Sortierschlüssel bei Gleichstand in ORDER BY."));
        }
        else result.Checks.Add(new(task.Operation == "Query" ? "Ergebniswerte auf beiden Datenbeständen" : "Vollständiger Datenstand und Spaltenstruktur", valuesCorrect ? 8 : 0, 8, difference ?? "Das Ergebnis stimmt mit den Ausgangsdaten und den zusätzlichen Prüfdaten."));
        return result;
    }
}
