using FiaeTrainer.Core.Sql.Results;

namespace FiaeTrainer.Core.Sql.Investigation;

/// <summary> Trennt die erfolgreiche SQL-Ausführung von der fachlichen Bewertung. </summary>
public sealed record Evaluation(bool Correct, string Message, QueryResult Result, bool TechnicalFailure = false);
