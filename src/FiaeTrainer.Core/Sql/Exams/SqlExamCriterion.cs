namespace FiaeTrainer.Core.Sql.Exams;

/// <summary> Vergleicht ein fachliches Teilergebnis mit der Referenz auf beiden Datenständen.
/// Eine Verhaltensprobe läuft in einem eigenen rücksetzbaren Prüfschritt. </summary>
public sealed record SqlExamCriterion(string Name, int Points, string Sql, string Failure, bool Probe = false, bool AtomicTransfer = false);
