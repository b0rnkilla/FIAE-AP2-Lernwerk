using System.Text.Json;
using FiaeTrainer.Core.SoftwareDesign.Evaluation;

namespace FiaeTrainer.Core.SoftwareDesign.Content;

/// <summary> Eingebetteter eigener Lernweg mit validierten Aufgaben, Fachrollen und Referenzen. </summary>
public sealed class SoftwareCatalog
{
    /// <summary> Eigenständiger Inhaltsvertrag für neue Profileigenschaften. </summary>
    public const string CONTENT_VERSION = "2026-11-09";
    /// <summary> Acht Kapitel mit je sechs eigenen Lernaufgaben. </summary>
    public IReadOnlyList<SoftwareAssignment> Lessons { get; } = Read<List<SoftwareAssignment>>("lessons.json");
    /// <summary> Drei unbekannte Fälle mit getrennten Teilaufträgen. </summary>
    public IReadOnlyList<SoftwareExamCase> Cases { get; } = Read<List<SoftwareExamCase>>("exams.json");
    /// <summary> Benutzeroberfläche und Spickzettel verwenden dieselben Aufgabenarten. </summary>
    public static readonly Dictionary<string, string> Kinds = new() { ["Choice"] = "Entscheiden", ["Worksheet"] = "Ausarbeiten", ["Diagram"] = "Klassenentwurf", ["Architecture"] = "Architektur", ["Api"] = "Schnittstellenvertrag", ["Form"] = "Oberflächenentwurf" };
    /// <summary> Falsche oder unvollständige Referenzen werden bereits beim Laden sichtbar. </summary>
    public SoftwareCatalog()
    {
        if (!Lessons.Select(task => task.Id).SequenceEqual(Enumerable.Range(1, 48)) || Lessons.GroupBy(task => task.Chapter).Count() != 8 || Lessons.GroupBy(task => task.Chapter).Any(group => group.Count() != 6) || Cases.Count != 3 || Cases.Select(item => item.Id).Distinct().Count() != 3 || Cases.Any(item => !item.Points.SequenceEqual([20, 20, 10]) || !item.Tasks.Select(task => task.Id).SequenceEqual([1, 2, 3]))) throw new InvalidDataException("Der Softwareplanungs-Katalog ist unvollständig.");
        foreach (var task in Lessons.Concat(Cases.SelectMany(item => item.Tasks)))
        {
            if (!Kinds.ContainsKey(task.Kind) || task.Format is not ("Build" or "Repair" or "Analyze") || task.Hints.Count != 3 || task.ReflectionCriteria.Count == 0 || string.IsNullOrWhiteSpace(task.Story) || string.IsNullOrWhiteSpace(task.Task) || string.IsNullOrWhiteSpace(task.Explanation) || !SoftwareRules.Storable(task.InitialAnswer()) || task.Fields.Select(field => field.Key).Distinct().Count() != task.Fields.Count || task.Fields.Any(field => field.Kind is not ("Select" or "Text" or "Number") || field.Comparison is not ("Exact" or "Text" or "Number"))) throw new InvalidDataException($"Softwareauftrag {task.Id} ist unvollständig.");
            if (task.Kind == "Choice")
            {
                if (task.CorrectIds.Count == 0 || task.CorrectIds.Distinct().Count() != task.CorrectIds.Count || task.CorrectIds.Any(id => !task.Options.ContainsKey(id))) throw new InvalidDataException("Die Auswahlreferenz fehlt.");
            }
            else if (task.Solutions.Count == 0 || task.Kind == "Diagram" && task.Diagram == null || task.Solutions.Any(answer => !SoftwareRules.Storable(answer) || task.Fields.Any(field => !answer.Values.ContainsKey(field.Key) || field.Kind == "Select" && !field.Options.ContainsKey(answer.Values[field.Key])) || !SoftwareEvaluator.Evaluate(task, answer).Correct)) throw new InvalidDataException($"Die Softwareplanungs-Referenz {task.Id} ist widersprüchlich.");
            if (task.Format == "Repair" && SoftwareEvaluator.Evaluate(task, task.InitialAnswer()).Correct) throw new InvalidDataException("Die Reparaturvorlage ist bereits vollständig gelöst.");
        }
        if (Lessons.Any(task => string.IsNullOrWhiteSpace(task.Discovery) || task.Id > 1 && string.IsNullOrWhiteSpace(task.PreviousContext))) throw new InvalidDataException("Die Nordhafen-Geschichte ist unvollständig.");
    }
    private static T Read<T>(string name)
    {
        using var stream = typeof(SoftwareCatalog).Assembly.GetManifestResourceStream("FiaeTrainer.Core.SoftwareDesign.Content." + name) ?? throw new InvalidDataException("Der Softwareplanungs-Inhalt fehlt.");
        return JsonSerializer.Deserialize<T>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new InvalidDataException("Der Softwareplanungs-Inhalt ist leer.");
    }
}
