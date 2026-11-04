namespace FiaeTrainer.Core.Algorithms.Content;

/// <summary> Transparente Eingabegruppe mit eingeschlossenen Grenzen; null bedeutet unbeschränkt. </summary>
public sealed record TestCoverage(string Name, decimal? Minimum, decimal? Maximum);
