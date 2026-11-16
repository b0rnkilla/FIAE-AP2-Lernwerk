using System.Text.Json;
using FiaeTrainer.Core.Exams;

namespace FiaeTrainer.Core.Sql.Exams;

/// <summary> Lädt vier eigene Prüfungsfälle mit fünf Aufgaben und getrennten Prüfdaten. </summary>
public sealed class SqlExamCatalog
{
    /// <summary> Neue Tabellenfälle außerhalb von Ermittlung und Werkstatt. </summary>
    public IReadOnlyList<SqlExamCase> Cases { get; }
    /// <summary> Prüft die feste Zuordnung, bevor eine Runde gestartet werden kann. </summary>
    public SqlExamCatalog()
    {
        using Stream stream = typeof(SqlExamCatalog).Assembly.GetManifestResourceStream("FiaeTrainer.Core.Sql.Exams.cases.json") ?? throw new InvalidDataException("SQL-Prüfungsfälle fehlen.");
        Cases = JsonSerializer.Deserialize<List<SqlExamCase>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
        var tasks = Cases.SelectMany(item => item.Tasks).ToArray();
        if (Cases.Count != 4 || Cases.DistinctBy(item => item.Id).Count() != Cases.Count ||
            tasks.DistinctBy(task => task.Id).Count() != tasks.Length ||
            Cases.Any(item => item.Tasks.Length != 5 || item.Tables.Length == 0 || item.Schema.Length == 0 ||
                item.SeedSql.Length == 0 || item.VerificationSql.Length == 0) ||
            tasks.Any(task => task.ReferenceSql.Length == 0 || task.Task.Length == 0 || task.Explanation.Length == 0))
            throw new InvalidDataException("SQL-Prüfungsfälle sind unvollständig.");

        foreach (var task in tasks.Where(task => task.Criteria.Length > 0))
        {
            if (task.Criteria.Sum(c => c.Points) != 8 || task.Criteria.DistinctBy(c => c.Name).Count() != task.Criteria.Length ||
                task.Criteria.Any(c => c.Points <= 0 || c.Name.Length == 0 || c.Failure.Length == 0 ||
                    c.AtomicTransfer != string.IsNullOrWhiteSpace(c.Sql) || c.AtomicTransfer && task.Operation != "Archive"))
                throw new InvalidDataException($"Die Bewertungskriterien für SQL-Aufgabe {task.Id} sind unvollständig.");
        }
    }

    /// <summary> Wählt einen ganzen Fall; Aufgabenauswahl bleibt bis zur Abgabe unverändert. </summary>
    public ExamRound CreateRound(string? caseId = null)
    {
        SqlExamCase item = caseId == null ? Cases[Random.Shared.Next(Cases.Count)] : Cases.Single(item => item.Id == caseId);
        return new() { Module = ExamModule.Sql, CaseId = item.Id, TaskIds = item.Tasks.Select(task => task.Id).ToList() };
    }
}
