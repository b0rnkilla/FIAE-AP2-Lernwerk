namespace FiaeTrainer.Core.Sql.Workshop;

/// <summary> Bewertung einer Werkstattaufgabe mit sichtbarem Endzustand. </summary>
public sealed record WorkshopEvaluation(bool Correct, string Message, WorkshopRun Run, bool TechnicalFailure = false);
