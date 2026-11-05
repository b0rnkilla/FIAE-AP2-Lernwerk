using System.Text.Json;
using FiaeTrainer.Core.Exams;

namespace FiaeTrainer.Core.Sql.Exams;

/// <summary> Lädt drei eigene Prüfungsfälle mit fünf Aufgaben und getrennten Prüfdaten. </summary>
public sealed class SqlExamCatalog
{
    /// <summary> Neue Tabellenfälle außerhalb von Ermittlung und Werkstatt. </summary>
    public IReadOnlyList<SqlExamCase> Cases { get; }
    /// <summary> Prüft die feste Zuordnung, bevor eine Runde gestartet werden kann. </summary>
    public SqlExamCatalog()
    {
        using Stream stream = typeof(SqlExamCatalog).Assembly.GetManifestResourceStream("FiaeTrainer.Core.Sql.Exams.cases.json") ?? throw new InvalidDataException("SQL-Prüfungsfälle fehlen.");
        Cases = JsonSerializer.Deserialize<List<SqlExamCase>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
        if (Cases.Count != 3 || Cases.Select(item => item.Id).Distinct().Count() != 3 || Cases.SelectMany(item => item.Tasks).Select(task => task.Id).Distinct().Count() != 15 || Cases.Any(item => item.Tasks.Length != 5 || item.Tables.Length == 0 || item.Schema.Length == 0 || item.SeedSql.Length == 0 || item.VerificationSql.Length == 0 || item.Tasks.Any(task => task.ReferenceSql.Length == 0 || task.Task.Length == 0)))
            throw new InvalidDataException("SQL-Prüfungsfälle sind unvollständig.");
    }
    /// <summary> Wählt einen ganzen Fall; Aufgabenauswahl bleibt bis zur Abgabe unverändert. </summary>
    public ExamRound CreateRound(string? caseId = null)
    {
        SqlExamCase item = caseId == null ? Cases[Random.Shared.Next(Cases.Count)] : Cases.Single(item => item.Id == caseId);
        return new() { Module = ExamModule.Sql, CaseId = item.Id, TaskIds = item.Tasks.Select(task => task.Id).ToList() };
    }
}
