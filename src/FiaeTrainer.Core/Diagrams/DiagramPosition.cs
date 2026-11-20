namespace FiaeTrainer.Core.Diagrams;

/// <summary> Gespeicherte Zeichenposition ohne Einfluss auf die fachliche Bewertung. </summary>
public sealed record DiagramPosition(int X, int Y)
{
    /// <summary> Begrenzt die Zeichenfläche und lehnt beschädigte Koordinaten ab. </summary>
    public static bool Valid(Dictionary<string, DiagramPosition>? positions, int maximum) => positions != null && positions.Count <= maximum && positions.All(pair => pair.Key.Length is > 0 and <= 80 && pair.Value is { X: >= 0 and <= 8000, Y: >= 0 and <= 8000 });
}
