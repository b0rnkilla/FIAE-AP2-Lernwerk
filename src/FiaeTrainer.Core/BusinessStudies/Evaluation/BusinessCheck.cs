namespace FiaeTrainer.Core.BusinessStudies.Evaluation;

/// <summary> Auflösung eines Antwortbestandteils einschließlich leerer oder ungültiger Eingaben. </summary>
public sealed record BusinessCheck(string Label, string Actual, string Expected, bool Correct, string Explanation);
