using System.Text.Json;
using FiaeTrainer.Core.QualityAssurance.Evaluation;
using FiaeTrainer.Core.QualityAssurance.Models;

namespace FiaeTrainer.Core.QualityAssurance.Content;

/// <summary> Eigener Lernkatalog mit ausführbaren lokalen Testverträgen und geprüften Referenzen. </summary>
public sealed class QualityCatalog
{
    /// <summary> Neue Profilfelder beginnen mit einem unabhängigen Inhaltsvertrag. </summary>
    public const string CONTENT_VERSION = "2026-11-11";
    /// <summary> Neun Kapitel mit je sechs eigenen Aufträgen. </summary>
    public IReadOnlyList<QualityAssignment> Lessons { get; } = Read<List<QualityAssignment>>("lessons.json");
    /// <summary> Drei eigenständige Prüfungsfälle. </summary>
    public IReadOnlyList<QualityExamCase> Cases { get; } = Read<List<QualityExamCase>>("exams.json");
    /// <summary> Gemeinsame Bezeichnungen der Arbeitsbereiche. </summary>
    public static readonly Dictionary<string, string> Kinds = new() { ["Choice"] = "Entscheiden", ["Worksheet"] = "Ausarbeiten", ["Review"] = "Review", ["TestSuite"] = "Testwerkstatt", ["Matrix"] = "Rechte planen", ["Order"] = "Ablauf erstellen" };

    /// <summary> Inhaltsfehler, unerreichbare Anforderungen und falsche Musterlösungen verhindern den Katalogstart. </summary>
    public QualityCatalog()
    {
        if (!Lessons.Select(task => task.Id).SequenceEqual(Enumerable.Range(1, 54)) || Lessons.GroupBy(task => task.Chapter).Count() != 9 || Lessons.GroupBy(task => task.Chapter).Any(group => group.Count() != 6) || Cases.Count != 3 || Cases.Select(item => item.Id).Distinct().Count() != 3 || Cases.Any(item => !item.Points.SequenceEqual([20, 20, 10]) || !item.Tasks.Select(task => task.Id).SequenceEqual([1, 2, 3]))) throw new InvalidDataException("Der Qualitätskatalog ist unvollständig.");
        if (QualityPrograms.All.Count != 8 || QualityPrograms.All.Select(item => item.Key).Distinct().Count() != 8) throw new InvalidDataException("Die lokalen Testverträge sind unvollständig.");
        foreach (var task in Lessons.Concat(Cases.SelectMany(item => item.Tasks)))
        {
            if (!Kinds.ContainsKey(task.Kind) || task.Format is not ("Build" or "Repair" or "Analyze") || task.Hints.Count != 3 || string.IsNullOrWhiteSpace(task.Story) || string.IsNullOrWhiteSpace(task.Task) || string.IsNullOrWhiteSpace(task.Explanation) || !QualityRules.Storable(task.InitialAnswer()) || task.Fields.Count > 24 || task.MatrixRows.Count * task.MatrixColumns.Count > 24 || task.MaximumCases is < 1 or > 16 || task.Fields.Select(field => field.Key).Distinct().Count() != task.Fields.Count || task.Fields.Any(field => field.Kind is not ("Select" or "Text" or "Number") || field.Comparison is not ("Exact" or "Text" or "Number"))) throw new InvalidDataException($"Qualitätsauftrag {task.Id} ist unvollständig.");
            if (task.Kind is "Choice" or "Review")
            {
                if (task.CorrectIds.Count == 0 || task.CorrectIds.Distinct().Count() != task.CorrectIds.Count || task.CorrectIds.Any(id => !task.Options.ContainsKey(id))) throw new InvalidDataException("Die Auswahlreferenz fehlt.");
            }
            else if (task.Solutions.Count == 0 || task.Solutions.Any(answer => !QualityRules.Storable(answer) || task.Fields.Any(field => !answer.Values.ContainsKey(field.Key) || field.Kind == "Select" && !field.Options.ContainsKey(answer.Values[field.Key])) || !QualityEvaluator.Evaluate(task, answer).Correct)) throw new InvalidDataException($"Die Qualitätsreferenz {task.Id} ({task.Title}) ist widersprüchlich.");
            if (task.Kind == "TestSuite")
            {
                var program = QualityPrograms.Find(task.ProgramKey) ?? throw new InvalidDataException("Der Testvertrag fehlt.");
                if (task.RequiredPartitions.Except(program.Partitions.Keys).Any() || task.RequiredStatements.Except(program.Statements.Keys).Any() || task.RequiredBranches.Except(program.Branches.Keys).Any() || task.RequiredPaths.Except(program.Paths.Keys).Any() || task.RequiredFaults.Except(program.Faults.Keys).Any()) throw new InvalidDataException("Ein Abdeckungsziel gehört nicht zum Testvertrag.");
            }
            if (task.Format == "Repair" && QualityEvaluator.Evaluate(task, task.InitialAnswer()).Correct) throw new InvalidDataException("Die Reparaturvorlage ist bereits gelöst.");
        }
        if (Lessons.Any(task => string.IsNullOrWhiteSpace(task.Discovery) || task.Id > 1 && string.IsNullOrWhiteSpace(task.PreviousContext))) throw new InvalidDataException("Die Geschichte des Probebetriebs ist unvollständig.");
    }
    private static T Read<T>(string name)
    {
        using var stream = typeof(QualityCatalog).Assembly.GetManifestResourceStream("FiaeTrainer.Core.QualityAssurance.Content." + name) ?? throw new InvalidDataException("Der Qualitätsinhalt fehlt.");
        return JsonSerializer.Deserialize<T>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new InvalidDataException("Der Qualitätsinhalt ist leer.");
    }
}
