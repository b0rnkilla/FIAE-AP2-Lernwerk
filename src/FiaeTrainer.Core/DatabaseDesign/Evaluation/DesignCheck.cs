namespace FiaeTrainer.Core.DatabaseDesign.Evaluation;

/// <summary> Eine transparente Bewertungsgruppe mit ihren eigenen Trainingspunkten. </summary>
public sealed record DesignCheck(string Name, int Earned, int Maximum, string Detail);
