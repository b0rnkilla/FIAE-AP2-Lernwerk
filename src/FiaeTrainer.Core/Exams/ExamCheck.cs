namespace FiaeTrainer.Core.Exams;

/// <summary> Nachvollziehbare Teilanforderung; wird erst nach Abgabe angezeigt. </summary>
public sealed record ExamCheck(string Name, int Earned, int Maximum, string Detail);
