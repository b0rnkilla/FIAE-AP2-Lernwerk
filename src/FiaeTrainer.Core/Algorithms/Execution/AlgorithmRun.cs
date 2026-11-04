namespace FiaeTrainer.Core.Algorithms.Execution;

/// <summary> Ausgabe und begrenzte Ablaufspur einer einzelnen Ausführung. </summary>
public sealed record AlgorithmRun(IReadOnlyList<string> Output, IReadOnlyList<AlgorithmStep> Steps, string? Error = null, bool TraceTruncated = false);
