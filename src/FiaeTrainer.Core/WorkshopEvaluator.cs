namespace FiaeTrainer.Core;

/// <summary> Vergleicht komplette Arbeitskopien und Schemaeigenschaften nach einem Schreibskript. </summary>
public sealed class WorkshopEvaluator(TrainingDatabase database, WorkshopSqlValidator validator)
{
    /// <summary> Zeigt die unveränderten Startdaten für die Aufgabenstellung. </summary>
    public Task<WorkshopRun> GetInitialStateAsync(CancellationToken cancellationToken = default) => database.ExecuteWorkshopAsync("", cancellationToken: cancellationToken);

    /// <summary> Führt ein Skript ohne Wertung aus; der nächste Versuch beginnt wieder mit frischen Daten. </summary>
    public async Task<WorkshopRun> PreviewAsync(string sql, CancellationToken cancellationToken = default)
    {
        string? error = validator.Validate(sql);
        return error == null ? await database.ExecuteWorkshopAsync(validator.ToSessionTables(sql), cancellationToken: cancellationToken) : new WorkshopRun { Error = error };
    }

    /// <summary> Prüft den verlangten Baustein sowie Daten, Struktur und Constraint-Verhalten auf zwei Startdatenbeständen. </summary>
    public async Task<WorkshopEvaluation> EvaluateAsync(WorkshopLesson lesson, string sql, CancellationToken cancellationToken = default)
    {
        string? error = validator.Validate(sql, lesson.Operation);
        if (error != null)
            return new WorkshopEvaluation(false, error, new WorkshopRun { Error = error });
        string sessionSql = validator.ToSessionTables(sql);
        string referenceSql = validator.ToSessionTables(lesson.ReferenceSql);
        WorkshopRun actual = await database.ExecuteWorkshopAsync(sessionSql, cancellationToken: cancellationToken);
        if (actual.Error != null)
            return new WorkshopEvaluation(false, actual.Error, actual, actual.TechnicalFailure);
        WorkshopRun expected = await database.ExecuteWorkshopAsync(referenceSql, cancellationToken: cancellationToken);
        if (expected.Error != null)
            return new WorkshopEvaluation(false, "Die Referenz konnte technisch nicht geprüft werden. Dein Versuch wird nicht fachlich gewertet.", actual, true);
        if (!AreEqual(expected, actual))
            return new WorkshopEvaluation(false, "Der Endzustand stimmt noch nicht vollständig. Prüfe betroffene Zeilen, berechnete Werte, Spalten und Datenregeln. Auch die übrigen Tabellen müssen erhalten bleiben.", actual);
        WorkshopRun alternativeExpected = await database.ExecuteWorkshopAsync(referenceSql, true, cancellationToken);
        WorkshopRun alternativeActual = await database.ExecuteWorkshopAsync(sessionSql, true, cancellationToken);
        if (alternativeExpected.Error != null || alternativeActual.TechnicalFailure)
            return new WorkshopEvaluation(false, "Die Zusatzprüfung konnte technisch nicht abgeschlossen werden. Dein Versuch wird nicht fachlich gewertet.", actual, true);
        if (!AreEqual(alternativeExpected, alternativeActual))
            return new WorkshopEvaluation(false, "Für die sichtbaren Startdaten passt dein Skript. Mit weiteren Artikeln oder anderen Beträgen stimmt es noch nicht. Verwende die allgemeine Regel aus dem Auftrag statt fest ausgerechneter Ergebnisse.", actual);
        return new WorkshopEvaluation(true, "Richtig: Datenstand und Tabellenregeln stimmen auch mit veränderten Ausgangsdaten. Die Arbeitskopie wird danach verworfen.", actual);
    }

    /// <summary> Vergleicht alle Tabellen und geprüften Strukturregeln, ohne Duplikate oder Datentypen zu verlieren. </summary>
    public static bool AreEqual(WorkshopRun expected, WorkshopRun actual)
    {
        if (expected.Error != null || actual.Error != null || !expected.Tables.Keys.Order().SequenceEqual(actual.Tables.Keys.Order()) || !expected.Structures.Keys.Order().SequenceEqual(actual.Structures.Keys.Order()))
            return false;
        foreach (string table in expected.Tables.Keys)
            if (!ResultComparer.AreEqual(expected.Tables[table], actual.Tables[table], true) || !ResultComparer.AreEqual(NormalizeColumnNames(expected.Structures[table]), NormalizeColumnNames(actual.Structures[table]), true))
                return false;
        if (!expected.BeforeRollback.Keys.Order().SequenceEqual(actual.BeforeRollback.Keys.Order()))
            return false;
        foreach (string table in expected.BeforeRollback.Keys)
            if (!ResultComparer.AreEqual(expected.BeforeRollback[table], actual.BeforeRollback[table], true))
                return false;
        return ResultComparer.AreEqual(expected.ConstraintChecks, actual.ConstraintChecks, true);
    }

    private static QueryResult NormalizeColumnNames(QueryResult structure) => new()
    {
        Columns = structure.Columns,
        Rows = structure.Rows.Select(row => row.Select((value, index) => index == 0 && value is string name ? name.ToUpperInvariant() : value).ToArray()).ToList(),
        Error = structure.Error,
        Truncated = structure.Truncated
    };
}
