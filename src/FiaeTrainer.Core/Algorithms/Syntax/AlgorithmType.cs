namespace FiaeTrainer.Core.Algorithms.Syntax;

/// <summary> Einheitlicher Werttyp für alle Schreibweisen, einschließlich Listen und Arrays derselben Tiefe. </summary>
internal sealed record AlgorithmType(string Scalar, int Depth = 0)
{
    internal const int MAX_DEPTH = 16;

    /// <summary> Deutsche und englische Namen bezeichnen denselben skalaren Typ. </summary>
    internal static string? ScalarName(string word) => word.ToUpperInvariant() switch
    {
        "INTEGER" or "INT" or "GANZZAHL" => "Integer",
        "DOUBLE" or "KOMMAZAHL" or "DEZIMALZAHL" => "Double",
        "BOOLEAN" or "BOOL" or "WAHRHEITSWERT" => "Boolean",
        "STRING" or "TEXT" or "ZEICHENKETTE" => "String",
        "VOID" or "KEINRUECKGABEWERT" or "KEINRÜCKGABEWERT" => "Void",
        _ => null
    };

    /// <summary> Der eingebettete Katalog verwendet ausschließlich die kanonische Kurzform, etwa Integer[][]. </summary>
    internal static AlgorithmType FromCatalog(string name)
    {
        string scalar = name;
        int depth = 0;
        while (scalar.EndsWith("[]", StringComparison.Ordinal))
        {
            scalar = scalar[..^2];
            depth++;
        }
        if (depth > MAX_DEPTH || ScalarName(scalar) != scalar || scalar == "Void" && depth > 0)
            throw new InvalidDataException($"Unbekannter Typ im Aufgabenkatalog: {name}.");
        return new(scalar, depth);
    }

    /// <summary> Prüft den tatsächlichen Wert einschließlich aller Einträge. Eine leere Liste erfüllt jeden Elementtyp. </summary>
    internal bool Accepts(object value) => Accepts(value, Depth);

    /// <summary> Kein Werttyp für Variablen, sondern ausschließlich ein Funktionsvertrag ohne Rückgabewert. </summary>
    internal bool IsVoid => Scalar == "Void" && Depth == 0;

    /// <summary> Erklärt besonders den Unterschied zwischen einem Text in Anführungszeichen und seinem Inhalt. </summary>
    internal static string Describe(object value) => value switch
    {
        string => "ein Text (String)", bool => "ein Wahrheitswert (Boolean)", object[] => "eine Liste",
        decimal number when decimal.Truncate(number) == number => "eine ganze Zahl", _ => "eine Zahl mit Nachkommateil"
    };

    private bool Accepts(object value, int depth)
    {
        if (depth > 0) return value is object[] list && list.All(item => Accepts(item, depth - 1));
        return Scalar switch
        {
            "Integer" => value is decimal number && decimal.Truncate(number) == number,
            "Double" => value is decimal,
            "Boolean" => value is bool,
            "String" => value is string,
            _ => false
        };
    }

    /// <summary> Die Kataloge verwenden diese eindeutige Kurzform auch für verschachtelte Listen. </summary>
    public override string ToString() => Scalar + string.Concat(Enumerable.Repeat("[]", Depth));
}
