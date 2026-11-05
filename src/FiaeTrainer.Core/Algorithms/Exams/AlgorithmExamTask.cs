using FiaeTrainer.Core.Algorithms.Content;

namespace FiaeTrainer.Core.Algorithms.Exams;

/// <summary> Eigener Prüfungsauftrag; taucht nicht im geführten Lernkatalog auf. </summary>
public sealed class AlgorithmExamTask
{
    /// <summary> Geprüfter Ausführungs- und Antwortvertrag. </summary>
    public AlgorithmLesson Lesson { get; init; } = new();
    /// <summary> Programmieren und Reparieren zählen zwanzig, Analyseaufgaben zehn Punkte. </summary>
    public int Points => Lesson.Kind is AlgorithmTaskKind.Trace or AlgorithmTaskKind.TestCases ? 10 : 20;
}
