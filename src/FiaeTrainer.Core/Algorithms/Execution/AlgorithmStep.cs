namespace FiaeTrainer.Core.Algorithms.Execution;

/// <summary> Zustand nach einer Anweisung oder Bedingungsprüfung, mit lokalen Werten des benannten Ausführungsbereichs. </summary>
public sealed record AlgorithmStep(int Line, string Action, IReadOnlyDictionary<string, string> Variables, IReadOnlyList<string> Output, string Scope = "Hauptablauf", int CallDepth = 0);
