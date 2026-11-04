using System.Globalization;

namespace FiaeTrainer.Core.Algorithms.Syntax;

/// <summary> Interne Syntaxelemente; sie führen selbst weder Code noch externe Zugriffe aus. </summary>
internal static class AlgorithmSyntax
{
    /// <summary> Reservierte Befehle, Operatoren, Wahrheitswerte und die Listenfunktion des Trainers. </summary>
    internal static readonly HashSet<string> KEYWORDS = new(StringComparer.OrdinalIgnoreCase)
    {
        "WENN", "SONST", "ENDE", "DANN", "SOLANGE", "FÜR", "FUER", "JEDES", "IN", "VON", "BIS", "TUE",
        "AUSGABE", "WAHR", "FALSCH", "NICHT", "UND", "ODER", "DIV", "MOD", "LÄNGE", "LAENGE",
        "FUNKTION", "RÜCKGABE", "RUECKGABE"
    };

    internal sealed record Token(string Text, int Line, bool Quoted = false);
    internal abstract record Expression;
    internal sealed record Literal(object Value) : Expression;
    internal sealed record Variable(string Name) : Expression;
    internal sealed record Unary(string Operator, Expression Operand) : Expression;
    internal sealed record Binary(Expression Left, string Operator, Expression Right) : Expression;
    internal sealed record ArrayLiteral(IReadOnlyList<Expression> Items) : Expression;
    internal sealed record ListIndex(Expression Array, Expression Position) : Expression;
    internal sealed record Length(Expression Array) : Expression;
    internal sealed record Call(string Name, IReadOnlyList<Expression> Arguments) : Expression;
    internal sealed record Function(int Line, string Name, IReadOnlyList<string> Parameters, IReadOnlyList<Statement> Body);
    internal sealed record AlgorithmProgram(IReadOnlyList<Statement> Statements, IReadOnlyList<Function> Functions);
    internal abstract record Statement(int Line);
    internal sealed record Assignment(int Line, string Name, Expression Value) : Statement(Line);
    internal sealed record IndexedAssignment(int Line, string Name, IReadOnlyList<Expression> Indices, Expression Value) : Statement(Line);
    internal sealed record Return(int Line, Expression Value) : Statement(Line);
    internal sealed record Print(int Line, Expression Value) : Statement(Line);
    internal sealed record Branch(int Line, Expression Condition, IReadOnlyList<Statement> Then, IReadOnlyList<Statement> Else) : Statement(Line);
    internal sealed record While(int Line, Expression Condition, IReadOnlyList<Statement> Body) : Statement(Line);
    internal sealed record For(int Line, string Name, Expression Start, Expression End, IReadOnlyList<Statement> Body) : Statement(Line);
    internal sealed record Each(int Line, string Name, Expression Array, IReadOnlyList<Statement> Body) : Statement(Line);

    /// <summary> Erhält Zeilen für Fehlermeldungen und erkennt Kommentare außerhalb von Textwerten. </summary>
    internal static List<Token> Tokenize(string code)
    {
        List<Token> tokens = [];
        int line = 1;
        for (int i = 0; i < code.Length;)
        {
            char c = code[i];
            if (c == '\r' || c == ' ' || c == '\t') { i++; continue; }
            if (c == '\n' || c == ';') { tokens.Add(new("\n", line)); if (c == '\n') line++; i++; }
            else if (c == '/' && i + 1 < code.Length && code[i + 1] == '/')
            {
                while (i < code.Length && code[i] != '\n') i++;
            }
            else if (c is '\'' or '"')
            {
                int start = ++i;
                while (i < code.Length && code[i] != c && code[i] != '\n') i++;
                if (i >= code.Length || code[i] != c)
                    throw new AlgorithmException(line, "Der Text braucht ein schließendes Anführungszeichen in derselben Zeile.");
                tokens.Add(new(code[start..i], line, true)); i++;
            }
            else if (char.IsLetter(c) || c == '_')
            {
                int start = i++;
                while (i < code.Length && (char.IsLetterOrDigit(code[i]) || code[i] == '_')) i++;
                string word = code[start..i];
                if (KEYWORDS.Contains(word) && word != word.ToUpperInvariant())
                    throw new AlgorithmException(line, $"Schreibe Schlüsselwörter groß: „{word}“ muss „{word.ToUpperInvariant()}“ heißen. Variablennamen und Texte bleiben unverändert.");
                tokens.Add(new(word, line));
            }
            else if (char.IsDigit(c))
            {
                int start = i++;
                while (i < code.Length && (char.IsDigit(code[i]) || code[i] == '.')) i++;
                string number = code[start..i];
                if (!decimal.TryParse(number, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out _))
                    throw new AlgorithmException(line, "Die Zahl ist ungültig. Verwende einen Punkt als Dezimaltrennzeichen.");
                tokens.Add(new(number, line));
            }
            else
            {
                string pair = i + 1 < code.Length ? code.Substring(i, 2) : "";
                if (pair is "<-" or ":=" or "<=" or ">=" or "<>" or "!=" or "==") { tokens.Add(new(pair, line)); i += 2; }
                else if ("←+-*/%=<>()[],".Contains(c)) { tokens.Add(new(c.ToString(), line)); i++; }
                else throw new AlgorithmException(line, $"Das Zeichen „{c}“ gehört nicht zur unterstützten Schreibweise. Der Spickzettel zeigt passende Beispiele.");
            }
            if (tokens.Count > 4000)
                throw new AlgorithmException(line, "Der Ablauf ist zu lang. Begrenze deine Lösung auf die aktuelle Aufgabe.");
        }
        tokens.Add(new("\n", line));
        tokens.Add(new("<EOF>", line));
        return tokens;
    }
}
