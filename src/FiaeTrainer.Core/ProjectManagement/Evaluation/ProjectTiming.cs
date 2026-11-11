namespace FiaeTrainer.Core.ProjectManagement.Evaluation;

/// <summary> Früheste/späteste Zeitpunkte und Puffer im ausdrücklich definierten Vorgangsknotenmodell. </summary>
public sealed record ProjectTiming(int EarlyStart, int EarlyFinish, int LateStart, int LateFinish, int TotalFloat, int FreeFloat)
{
    /// <summary> Stabile Feldkennungen für die Bewertung eigener Zeitangaben. </summary>
    public int Value(string key) => key switch { "es" => EarlyStart, "ef" => EarlyFinish, "ls" => LateStart, "lf" => LateFinish, "tf" => TotalFloat, "ff" => FreeFloat, _ => throw new ArgumentException("Unbekanntes Zeitfeld.") };
}
