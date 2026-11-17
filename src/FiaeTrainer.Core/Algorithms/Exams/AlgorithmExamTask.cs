using FiaeTrainer.Core.Algorithms.Content;
using FiaeTrainer.Core.Algorithms.Execution;
using System.Text.Json;

namespace FiaeTrainer.Core.Algorithms.Exams;

/// <summary> Eigener Prüfungsauftrag; taucht nicht im geführten Lernkatalog auf. </summary>
public sealed class AlgorithmExamTask
{
    /// <summary> Geprüfter Ausführungs- und Antwortvertrag. </summary>
    public AlgorithmLesson Lesson { get; init; } = new();
    /// <summary> Programmieren und Reparieren zählen zwanzig, Analyseaufgaben zehn Punkte. </summary>
    public int Points => Lesson.Kind is AlgorithmTaskKind.Trace or AlgorithmTaskKind.TestCases ? 10 : 20;

    /// <summary> Gespeicherte Runden vor der Einführung typisierter Prüfungen behalten ihren Vertrag. </summary>
    public bool RequiresTypes(string contentVersion) => contentVersion == AlgorithmExamCatalog.TYPED_VERSION && Lesson.RequiresTypedSignature;

    /// <summary> Für alte Prüfungen bleibt auch der damalige Vergleich der formatierten Rückgabe erhalten. </summary>
    public AlgorithmRun RunFunction(AlgorithmInterpreter interpreter, string answer, IReadOnlyDictionary<string, JsonElement> inputs, string contentVersion, bool trace = false)
        => contentVersion == AlgorithmExamCatalog.TYPED_VERSION
            ? interpreter.RunFunction(answer, Lesson, inputs, trace, RequiresTypes(contentVersion))
            : interpreter.RunFunction(answer, Lesson.FunctionName, Lesson.FunctionInputs.Select(name => inputs[name]).ToArray(), trace);
}
