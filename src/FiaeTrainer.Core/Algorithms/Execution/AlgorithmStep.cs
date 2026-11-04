namespace FiaeTrainer.Core.Algorithms.Execution;

/// <summary> Zustand nach einer Anweisung beziehungsweise nach Prüfung einer Schleifenbedingung. </summary>
public sealed record AlgorithmStep(int Line, string Action, IReadOnlyDictionary<string, string> Variables, IReadOnlyList<string> Output);
