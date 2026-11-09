using System.Text.Json;
using FiaeTrainer.Core.Uml.Evaluation;

namespace FiaeTrainer.Core.Uml.Content;

/// <summary> Eigener UML-Lernweg und getrennte Prüfungsfälle mit vorab validierten Referenzen. </summary>
public sealed class UmlCatalog
{
    /// <summary> Neue Inhaltskennung ohne Änderung bestehender Modulverträge. </summary>
    public const string CONTENT_VERSION = "2026-11-09";
    /// <summary> Sechsunddreißig stabile Lernaufgaben. </summary>
    public IReadOnlyList<UmlAssignment> Lessons { get; } = Read<List<UmlAssignment>>("lessons.json");
    /// <summary> Drei eigene Fälle mit jeweils drei unabhängigen Teilaufgaben. </summary>
    public IReadOnlyList<UmlExamCase> Cases { get; } = Read<List<UmlExamCase>>("exams.json");
    /// <summary> Prüft neben der Vollständigkeit jede fachliche Referenz mit denselben Grenzen wie Nutzermodelle. </summary>
    public UmlCatalog()
    {
        if (!Lessons.Select(task => task.Id).SequenceEqual(Enumerable.Range(1, 36)) || Cases.Count != 3 || Cases.Select(item => item.Id).Distinct().Count() != 3 || Cases.Any(item => item.Tasks.Count != 3 || !item.Tasks.Select(task => task.Id).SequenceEqual([1, 2, 3]) || !item.Points.SequenceEqual([20, 20, 10]))) throw new InvalidDataException("Der UML-Katalog ist unvollständig.");
        foreach (var task in Lessons.Concat(Cases.SelectMany(item => item.Tasks)))
        {
            if (!FiaeTrainer.Core.Uml.Models.UmlKinds.Diagrams.ContainsKey(task.DiagramType) || task.Format is not ("Build" or "Repair" or "Read") || task.Hints.Count != 3 || string.IsNullOrWhiteSpace(task.Task) || string.IsNullOrWhiteSpace(task.Story) || string.IsNullOrWhiteSpace(task.Explanation) || !UmlRules.Storable(task.InitialAnswer())) throw new InvalidDataException($"UML-Auftrag {task.Id} ist unvollständig.");
            if (task.Format == "Read") { if (task.CorrectIds.Count == 0 || task.CorrectIds.Any(id => !task.Options.ContainsKey(id))) throw new InvalidDataException("Die UML-Leseantwort fehlt."); }
            else if (task.Solutions.Count == 0 || task.Solutions.Any(model => UmlRules.Errors(task, model).Count > 0 || !UmlEvaluator.Evaluate(task, new() { Model = model }).Correct)) throw new InvalidDataException($"Die UML-Referenz {task.Id} ist widersprüchlich.");
        }
        if (Lessons.Any(task => string.IsNullOrWhiteSpace(task.Discovery) || task.Id > 1 && string.IsNullOrWhiteSpace(task.PreviousContext))) throw new InvalidDataException("Die UML-Geschichte ist unvollständig.");
    }
    /// <summary> Lädt ausschließlich die eingebetteten eigenen Aufgaben. </summary>
    private static T Read<T>(string name)
    {
        using var stream = typeof(UmlCatalog).Assembly.GetManifestResourceStream("FiaeTrainer.Core.Uml.Content." + name) ?? throw new InvalidDataException("Der UML-Inhalt fehlt.");
        return JsonSerializer.Deserialize<T>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new InvalidDataException("Der UML-Inhalt ist leer.");
    }
}
