namespace FiaeTrainer.Core.Algorithms.Evaluation;

/// <summary> Bewertung mit Gegenbeispielen statt Vergleich mit einem einzigen Lösungstext. </summary>
public sealed record AlgorithmEvaluation(bool Correct, string Message, IReadOnlyList<AlgorithmCheck> Checks);
