using System.Text.Json;
using System.Text.Json.Serialization;
using FiaeTrainer.Core.Algorithms.Content;
using FiaeTrainer.Core.Exams;

namespace FiaeTrainer.Core.Algorithms.Exams;

/// <summary> Zehn eigenständige Prüfungsaufgaben mit ausgewogener Auswahl von drei Aufgaben. </summary>
public sealed class AlgorithmExamCatalog
{
    /// <summary> Ab dieser Runde gehört der vollständige Funktionskopf zum ersten Prüfungsauftrag. </summary>
    public const string TYPED_VERSION = "2026-11-16";
    /// <summary> Ausschließlich für Prüfungsrunden vorhandene Aufgaben. </summary>
    public IReadOnlyList<AlgorithmExamTask> Tasks { get; }
    /// <summary> Lädt eigene Inhalte, keine Originalprüfungen oder Lösungen aus Benutzerdateien. </summary>
    public AlgorithmExamCatalog()
    {
        using Stream stream = typeof(AlgorithmExamCatalog).Assembly.GetManifestResourceStream("FiaeTrainer.Core.Algorithms.Exams.tasks.json") ?? throw new InvalidDataException("Prüfungsaufgaben fehlen.");
        JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new JsonStringEnumConverter());
        Tasks = (JsonSerializer.Deserialize<List<AlgorithmLesson>>(stream, options) ?? []).Select(lesson => new AlgorithmExamTask { Lesson = lesson }).ToArray();
        foreach (AlgorithmExamTask task in Tasks) task.Lesson.ValidateSignature();
        if (Tasks.Count != 10 || Tasks.Select(task => task.Lesson.Id).Distinct().Count() != 10 || Tasks.Any(task => task.Lesson.Id < 1 || task.Lesson.Task.Length == 0 || task.Lesson.Tests.Length == 0 || task.Lesson.Explanation.Length == 0))
            throw new InvalidDataException("Der Prüfungsbestand ist unvollständig.");
    }
    /// <summary> Zwei Codeaufgaben aus verschiedenen Gruppen und eine Analyse; feste Reihenfolge nach dem Start. </summary>
    public ExamRound CreateRound()
    {
        int first = Random.Shared.GetItems(Tasks.Where(task => task.Lesson.Id <= 3).ToArray(), 1)[0].Lesson.Id;
        int second = Random.Shared.GetItems(Tasks.Where(task => task.Lesson.Id is >= 4 and <= 8).ToArray(), 1)[0].Lesson.Id;
        int analysis = Random.Shared.Next(9, 11);
        return new() { Module = ExamModule.Algorithms, ContentVersion = TYPED_VERSION, TaskIds = [first, second, analysis] };
    }
}
