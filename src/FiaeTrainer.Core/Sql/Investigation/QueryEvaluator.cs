using FiaeTrainer.Core.Sql;
using FiaeTrainer.Core.Sql.Database;
using FiaeTrainer.Core.Sql.Results;
using FiaeTrainer.Core.Sql.Validation;

namespace FiaeTrainer.Core.Sql.Investigation;

/// <summary> Bewertet Abfragen mit Referenzergebnissen und einem zweiten, abweichenden Datenbestand. </summary>
public sealed class QueryEvaluator(TrainingDatabase database, SqlQueryValidator validator)
{
    /// <summary> Führt SQL ohne Bewertung und ohne Änderung des Lernfortschritts aus. </summary>
    public async Task<QueryResult> PreviewAsync(string sql, CancellationToken cancellationToken = default)
    {
        string? error = validator.Validate(sql);
        return error == null ? await database.ExecuteAsync(validator.ForCaseData(sql), cancellationToken) : new QueryResult { Error = error };
    }

    /// <summary> Prüft Ergebnisse einschließlich Duplikaten und bei Sortieraufgaben einschließlich der Reihenfolge. </summary>
    public async Task<Evaluation> EvaluateAsync(Lesson lesson, string sql, CancellationToken cancellationToken = default)
    {
        QueryResult actual = await PreviewAsync(sql, cancellationToken);
        if (actual.Error != null)
            return new Evaluation(false, actual.Error, actual, actual.TechnicalFailure);
        if (actual.Truncated)
            return new Evaluation(false, "Das Ergebnis enthält mehr als 200 Zeilen. Prüfe deine Filter und JOIN-Bedingungen.", actual);

        QueryResult expected = await database.ExecuteAsync(lesson.ReferenceSql, cancellationToken);
        if (expected.Error != null)
            return new Evaluation(false, "Die Musterlösung konnte technisch nicht geprüft werden. Dein Versuch wird nicht als fachlich falsch gewertet.", actual, true);
        if (!ResultComparer.AreEqual(expected, actual, lesson.Ordered))
        {
            string feedback = actual.Columns.Count != expected.Columns.Count ? $"Gesucht sind {expected.Columns.Count} Ausgabespalten in der angegebenen Reihenfolge. Deine Abfrage liefert {actual.Columns.Count}." : $"Dein Ergebnis stimmt noch nicht vollständig. Erwartet werden {expected.Rows.Count} Zeilen; deine Abfrage liefert {actual.Rows.Count}. Prüfe Werte, Duplikate{(lesson.Ordered ? " und Sortierung" : "")}.";
            return new Evaluation(false, feedback, actual);
        }

        QueryResult variantExpected = await database.ExecuteAsync(validator.ForVerificationData(lesson.ReferenceSql), cancellationToken);
        QueryResult variantActual = await database.ExecuteAsync(validator.ForVerificationData(sql), cancellationToken);
        if (variantExpected.Error != null)
            return new Evaluation(false, "Der zusätzliche Prüfdatenbestand ist gerade nicht verfügbar. Wiederhole die Bewertung nach der Verbindungsprüfung.", actual, true);
        if (variantActual.TechnicalFailure)
            return new Evaluation(false, variantActual.Error ?? "Die Zusatzprüfung konnte technisch nicht abgeschlossen werden.", actual, true);
        if (!ResultComparer.AreEqual(variantExpected, variantActual, lesson.Ordered))
            return new Evaluation(false, "Für die sichtbaren Daten passt das Ergebnis. Mit zusätzlichen Prüfdaten stimmt es noch nicht: Formuliere die gesuchte Regel allgemein, statt nur einzelne bereits bekannte Treffer auszuwählen.", actual);

        return new Evaluation(true, "Richtig gelöst. Deine Abfrage liefert auch mit zusätzlichen Prüfdaten das gesuchte Ergebnis.", actual);
    }
}
