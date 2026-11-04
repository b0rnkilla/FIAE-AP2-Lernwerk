using System.Text.Json;
using System.Text.Json.Serialization;

namespace FiaeTrainer.Core.Algorithms.Content;

/// <summary> Lädt den getrennten, eingebetteten Algorithmus-Lernweg. </summary>
public sealed class AlgorithmCatalog
{
    /// <summary> Aufgaben in erzählerischer Reihenfolge. </summary>
    public IReadOnlyList<AlgorithmLesson> Lessons { get; }

    /// <summary> Fehlerhafte Inhalte verhindern einen still unvollständigen Katalog. </summary>
    public AlgorithmCatalog()
    {
        using Stream resource = typeof(AlgorithmCatalog).Assembly.GetManifestResourceStream("FiaeTrainer.Core.Algorithms.Content.lessons.json") ?? throw new InvalidDataException("Die Pseudocode-Aufgaben fehlen.");
        JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new JsonStringEnumConverter());
        Lessons = JsonSerializer.Deserialize<List<AlgorithmLesson>>(resource, options) ?? throw new InvalidDataException("Der Algorithmus-Katalog ist leer.");
        if (Lessons.Count == 0 || Lessons.Where((lesson, index) => lesson.Id != index + 1 || lesson.Hints.Length != 3 || string.IsNullOrWhiteSpace(lesson.Task) || lesson.Tests.Length == 0).Any())
            throw new InvalidDataException("Die Algorithmus-Aufgaben sind unvollständig oder falsch nummeriert.");
        foreach (AlgorithmLesson lesson in Lessons.Where(x => x.FunctionName.Length > 0))
        {
            bool missingInputs = lesson.FunctionInputs.Any(name => !lesson.ExampleInputs.ContainsKey(name) || lesson.Tests.Any(test => !test.Inputs.ContainsKey(name)));
            bool duplicateInputs = lesson.FunctionInputs.Distinct(StringComparer.OrdinalIgnoreCase).Count() != lesson.FunctionInputs.Length;
            if (lesson.Kind is AlgorithmTaskKind.Trace or AlgorithmTaskKind.TestCases || missingInputs || duplicateInputs)
                throw new InvalidDataException("Die Eingaben einer Funktionsaufgabe sind unvollständig oder mehrfach zugeordnet.");
        }
        if (Lessons.Any(lesson => lesson.Kind == AlgorithmTaskKind.TestCases && (string.IsNullOrWhiteSpace(lesson.CaseInputName) || lesson.Coverage.Length == 0 || !lesson.ExampleInputs.ContainsKey(lesson.CaseInputName) || lesson.Tests.Any(test => !test.Inputs.ContainsKey(lesson.CaseInputName)))))
            throw new InvalidDataException("Eingabename oder Abdeckungsgruppen einer Testfallaufgabe fehlen.");
    }
}
