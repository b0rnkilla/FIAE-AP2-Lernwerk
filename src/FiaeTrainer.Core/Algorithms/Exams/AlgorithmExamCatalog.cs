using System.Text.Json;
using System.Text.Json.Serialization;
using FiaeTrainer.Core.Algorithms.Content;
using FiaeTrainer.Core.Exams;

namespace FiaeTrainer.Core.Algorithms.Exams;

/// <summary> Gemischter Prüfungspool und ein zusätzlicher zusammenhängender Fall mit unabhängigen Teilaufgaben. </summary>
public sealed class AlgorithmExamCatalog
{
    /// <summary> Ab dieser Runde gehört der vollständige Funktionskopf zum ersten Prüfungsauftrag. </summary>
    public const string TYPED_VERSION = "2026-11-16";
    /// <summary> Stabile Kennung des gezielt auswählbaren Kulturfestfalls. </summary>
    public const string EQUIPMENT_CASE = "kulturfest-ausleihe";
    /// <summary> Gemeinsamer Kontext ohne Abhängigkeit der drei Antworten voneinander. </summary>
    public const string EQUIPMENT_TITLE = "Geräteausleihe für ein Kulturfest";
    /// <summary> Einmalige Ausgangslage für die Fallauswahl und die laufende Prüfung. </summary>
    public const string EQUIPMENT_SCENARIO = "Das Kulturfest benötigt eine zuverlässige Geräteausleihe. Du prüfst die Verfügbarkeit, reparierst eine Bestandsänderung und entwirfst Grenztests für Buchungszeiten. Jede Teilaufgabe enthält alle benötigten Angaben und lässt sich unabhängig lösen.";
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
        if (Tasks.Count != 13 || Tasks.Select(task => task.Lesson.Id).Order().SequenceEqual(Enumerable.Range(1, 13)) == false || Tasks.Any(task => task.Lesson.Task.Length == 0 || task.Lesson.Tests.Length == 0 || task.Lesson.Explanation.Length == 0))
            throw new InvalidDataException("Der Prüfungsbestand ist unvollständig.");
    }
    /// <summary> Zwei Codeaufgaben aus verschiedenen Gruppen und eine Analyse; feste Reihenfolge nach dem Start. </summary>
    public ExamRound CreateRound(string? caseId = null)
    {
        if (caseId == EQUIPMENT_CASE) return new() { Module = ExamModule.Algorithms, ContentVersion = TYPED_VERSION, CaseId = EQUIPMENT_CASE, TaskIds = [11, 12, 13] };
        if (!string.IsNullOrEmpty(caseId)) throw new InvalidOperationException("Dieser Pseudocode-Prüfungsfall existiert nicht.");
        int first = Random.Shared.GetItems(Tasks.Where(task => task.Lesson.Id <= 3).ToArray(), 1)[0].Lesson.Id;
        int second = Random.Shared.GetItems(Tasks.Where(task => task.Lesson.Id is >= 4 and <= 8).ToArray(), 1)[0].Lesson.Id;
        int analysis = Random.Shared.Next(9, 11);
        return new() { Module = ExamModule.Algorithms, ContentVersion = TYPED_VERSION, TaskIds = [first, second, analysis] };
    }
}
