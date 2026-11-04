namespace FiaeTrainer.Core.Algorithms.Evaluation;

/// <summary> Nachvollziehbares Ergebnis eines Eingabefalls oder Abdeckungskriteriums. </summary>
public sealed record AlgorithmCheck(string Name, string Inputs, string Expected, string Actual, bool Passed);
