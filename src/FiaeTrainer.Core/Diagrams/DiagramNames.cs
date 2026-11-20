namespace FiaeTrainer.Core.Diagrams;

/// <summary> Erkennt ausdrücklich bekannte Bezeichnungen; errät keine fachliche Bedeutung. </summary>
public static class DiagramNames
{
    /// <summary> Ignoriert Großschreibung, Leerzeichen und die Umschrift deutscher Umlaute. Operatoren bleiben erhalten. </summary>
    public static string Normalize(string value) => string.Concat(value.Trim().ToLowerInvariant().Replace("ä", "ae").Replace("ö", "oe").Replace("ü", "ue").Replace("ß", "ss").Where(character => !char.IsWhiteSpace(character)));

    /// <summary> Vereinheitlicht gleiche Mengenangaben, ohne Mindestzahlen zu verändern. </summary>
    public static string Multiplicity(string value) => value.Trim().Replace(" ", "").Replace("n", "*").Replace("N", "*") switch
    {
        "*" => "0..*",
        "1..1" => "1",
        var text => text
    };

    /// <summary> Gibt bei unbekannten oder mehrdeutigen Namen keine Zuordnung zurück. </summary>
    public static string Resolve(string value, IReadOnlyDictionary<string, string> vocabulary, IReadOnlyDictionary<string, string>? aliases = null)
    {
        string normalized = Normalize(value);
        if (normalized.Length == 0) return "";
        var matches = vocabulary.Where(pair => Normalize(pair.Key) == normalized || Normalize(pair.Value) == normalized).Select(pair => pair.Key).ToList();
        if (aliases != null) matches.AddRange(aliases.Where(pair => Normalize(pair.Key) == normalized && vocabulary.ContainsKey(pair.Value)).Select(pair => pair.Value));
        var unique = matches.Distinct().ToArray();
        return unique.Length == 1 ? unique[0] : "";
    }
}
