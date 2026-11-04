using System.Text.Json;

namespace FiaeTrainer.Core.Algorithms.Execution;

/// <summary> Stellt festgelegte Eingabedaten mit denselben deutschen Wahrheitswerten wie der Interpreter dar. </summary>
public static class AlgorithmDisplay
{
    /// <summary> Behält Zahlen und Texte eindeutig und übersetzt Wahrheitswerte auch innerhalb von Listen. </summary>
    public static string Input(JsonElement value) => value.ValueKind switch
    {
        JsonValueKind.True => "WAHR", JsonValueKind.False => "FALSCH",
        JsonValueKind.Array => "[" + string.Join(", ", value.EnumerateArray().Select(Input)) + "]",
        _ => value.GetRawText()
    };
}
