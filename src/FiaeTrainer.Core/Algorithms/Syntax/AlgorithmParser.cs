using System.Globalization;
using static FiaeTrainer.Core.Algorithms.Syntax.AlgorithmSyntax;

namespace FiaeTrainer.Core.Algorithms.Syntax;

/// <summary> Liest die dokumentierte deutsche Pseudocode-Teilmenge mit begrenzter Verschachtelung. </summary>
internal sealed class AlgorithmParser(List<Token> tokens)
{
    #region Fields

    /// <summary> Aktuelle Position innerhalb der bereits begrenzten Tokenliste. </summary>
    private int _position;
    /// <summary> Gemeinsame Verschachtelungsgrenze für Blöcke und Ausdrücke. </summary>
    private int _depth;
    /// <summary> Rückgaben sind nur innerhalb einer Funktionsdefinition zulässig. </summary>
    private bool _insideFunction;

    #endregion

    #region Properties

    /// <summary> Nächstes noch nicht verarbeitetes Token samt ursprünglicher Zeile. </summary>
    private Token Current => tokens[_position];

    #endregion

    #region Methods

    /// <summary> Liest das gesamte Programm und weist überzählige Blockabschlüsse zurück. </summary>
    internal AlgorithmProgram Parse()
    {
        List<Statement> statements = [];
        List<Function> functions = [];
        SkipLines();
        while (!Is("<EOF>"))
        {
            int line = Current.Line;
            bool functionKeyword = Take("FUNKTION");
            AlgorithmType? prefix = ReadPrefixType();
            if (functionKeyword || (IsName(Current) && tokens[_position + 1] is { Quoted: false, Text: "(" }))
            {
                string name = Name(); Require("(");
                List<Binding> parameters = [];
                if (!Is(")"))
                    do
                    {
                        Binding parameter = ReadBinding(ReadPrefixType());
                        if (parameters.Any(p => p.Name.Equals(parameter.Name, StringComparison.OrdinalIgnoreCase))) throw Error("Ein Parametername darf in einer Funktion nur einmal vorkommen.");
                        if (parameters.Count >= 16) throw Error("Verwende höchstens 16 Parameter je Funktion.");
                        parameters.Add(parameter);
                    } while (Take(","));
                Require(")");
                AlgorithmType? returnType = ReadSuffixType(prefix);
                EndLine();
                _insideFunction = true;
                IReadOnlyList<Statement> body = Block();
                _insideFunction = false;
                Require("ENDE"); Require("FUNKTION");
                if (functions.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))) throw Error($"Die Funktion „{name}“ ist mehrfach definiert.");
                functions.Add(new(line, name, parameters, returnType, body));
            }
            else statements.Add(ReadStatement(prefix));
            EndLine();
        }
        return new(statements, functions);
    }

    /// <summary> Liest bis zum Ende des Programms oder bis zum Abschluss des umgebenden Blocks. </summary>
    private IReadOnlyList<Statement> Block()
    {
        Enter();
        List<Statement> statements = [];
        SkipLines();
        while (!Is("<EOF>") && !Is("ENDE") && !Is("SONST"))
        {
            statements.Add(ReadStatement());
            EndLine();
        }
        _depth--;
        return statements;
    }

    private Statement ReadStatement(AlgorithmType? prefix = null)
    {
        int line = Current.Line;
        if (Is("FUNKTION")) throw Error("Definiere Funktionen außerhalb von Bedingungen, Schleifen und anderen Funktionen.");
        if (Take("RÜCKGABE") || Take("RUECKGABE"))
        {
            if (!_insideFunction) throw Error("RÜCKGABE gehört in eine Funktion. Im Hauptablauf verwendest du AUSGABE.");
            return new Return(line, ReadExpression());
        }
        if (Take("AUSGABE")) return new Print(line, ReadExpression());
        if (Take("WENN"))
        {
            Expression condition = ReadExpression(); Require("DANN"); EndLine();
            IReadOnlyList<Statement> yes = Block();
            IReadOnlyList<Statement> no = [];
            if (Take("SONST")) { EndLine(); no = Block(); }
            Require("ENDE"); Require("WENN");
            return new Branch(line, condition, yes, no);
        }
        if (Take("SOLANGE"))
        {
            Expression condition = ReadExpression(); Take("TUE"); EndLine();
            IReadOnlyList<Statement> body = Block(); Require("ENDE"); Require("SOLANGE");
            return new While(line, condition, body);
        }
        if (Take("FÜR") || Take("FUER"))
        {
            bool each = TakeEachQuantifier();
            string name = Name();
            if (each)
            {
                Require("IN"); Expression array = ReadExpression(); Take("TUE"); EndLine();
                IReadOnlyList<Statement> body = Block(); Require("ENDE"); RequireFor();
                return new Each(line, name, array, body);
            }
            Require("VON"); Expression start = ReadExpression(); Require("BIS"); Expression end = ReadExpression(); Take("TUE"); EndLine();
            IReadOnlyList<Statement> loop = Block(); Require("ENDE"); RequireFor();
            return new For(line, name, start, end, loop);
        }
        Binding binding = ReadBinding(prefix ?? ReadPrefixType());
        List<Expression> indices = [];
        if (binding.Type != null && Is("[")) throw Error("Deklariere die ganze Variable mit einem Startwert. Einzelne Einträge erhalten den Elementtyp der Liste.");
        while (Take("[")) { indices.Add(ReadExpression()); Require("]"); }
        if (!(Take("←") || Take("<-") || Take(":=") || Take("=")))
            throw Error("Nach dem Variablennamen fehlt eine Zuweisung, zum Beispiel „anzahl = 0“.");
        Expression value = ReadExpression();
        return indices.Count == 0 ? new Assignment(line, binding.Name, value, binding.Type) : new IndexedAssignment(line, binding.Name, indices, value);
    }

    /// <summary> Prioritäten: ODER, UND, Vergleiche, Addition, Multiplikation, unäre Operatoren. </summary>
    private Expression ReadExpression(int minimum = 0)
    {
        Enter();
        int start = _position;
        Expression left;
        if (Is("NICHT") || Is("-") || Is("+"))
        {
            string op = Current.Text.ToUpperInvariant(); _position++;
            left = new Unary(op, ReadExpression(op == "NICHT" ? 3 : 6));
        }
        else left = Atom();
        while (Priority(Current) >= minimum && Priority(Current) > 0)
        {
            string op = Current.Text.ToUpperInvariant(); int priority = Priority(Current); _position++;
            left = new Binary(left, op, ReadExpression(priority + 1));
        }
        if (_position - start > 256) throw Error("Der Ausdruck ist zu lang. Teile die Berechnung mit Zwischenvariablen auf.");
        _depth--;
        return left;
    }

    private Expression Atom()
    {
        Token token = Current;
        Expression value;
        if (token.Quoted) { _position++; value = new Literal(token.Text); }
        else if (decimal.TryParse(token.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal number)) { _position++; value = new Literal(number); }
        else if (Take("WAHR")) value = new Literal(true);
        else if (Take("FALSCH")) value = new Literal(false);
        else if (Take("(")) { value = ReadExpression(); Require(")"); }
        else if (Take("["))
        {
            List<Expression> items = [];
            if (!Is("]"))
            {
                do { if (items.Count >= 256) throw Error("Eine Liste darf höchstens 256 Einträge enthalten."); items.Add(ReadExpression()); } while (Take(","));
            }
            Require("]"); value = new ArrayLiteral(items);
        }
        else if (Take("LÄNGE") || Take("LAENGE")) { Require("("); value = new Length(ReadExpression()); Require(")"); }
        else
        {
            string name = Name();
            if (Take("("))
            {
                List<Expression> arguments = [];
                if (!Is(")"))
                    do
                    {
                        if (arguments.Count >= 16) throw Error("Verwende höchstens 16 Argumente je Funktionsaufruf.");
                        arguments.Add(ReadExpression());
                    } while (Take(","));
                Require(")"); value = new Call(name, arguments);
            }
            else value = new Variable(name);
        }
        while (Take("[")) { Expression index = ReadExpression(); Require("]"); value = new ListIndex(value, index); }
        return value;
    }

    private static int Priority(Token token) => token.Quoted ? 0 : token.Text.ToUpperInvariant() switch
    {
        "ODER" => 1, "UND" => 2, "=" or "==" or "<>" or "!=" or "<" or ">" or "<=" or ">=" => 3,
        "+" or "-" => 4, "*" or "/" or "%" or "MOD" or "DIV" => 5, _ => 0
    };
    /// <summary> Typnamen bleiben in bisherigen Programmen als Variablen-, Parameter- und Funktionsnamen zulässig. </summary>
    private AlgorithmType? ReadPrefixType()
    {
        if (!IsName(Current)) return null;
        Token next = tokens[_position + 1];
        bool list = Is("List") || Is("Liste");
        if (!list && AlgorithmType.ScalarName(Current.Text) == null) return null;
        bool arraySuffix = next is { Quoted: false, Text: "[" } && tokens[_position + 2] is { Quoted: false, Text: "]" };
        return IsName(next) || arraySuffix || (list && next is { Quoted: false, Text: "<" or "<>" }) ? ReadType() : null;
    }

    private Binding ReadBinding(AlgorithmType? prefix) => new(Name(), ReadSuffixType(prefix));

    private AlgorithmType? ReadSuffixType(AlgorithmType? prefix)
    {
        if (!Take(":")) return prefix;
        if (prefix != null) throw Error("Gib den Datentyp nur einmal an, vor dem Namen oder nach dem Doppelpunkt.");
        return ReadType();
    }

    /// <summary> Normalisiert generische Listen und Arrayklammern mit derselben Verschachtelungsgrenze wie Listenwerte. </summary>
    private AlgorithmType ReadType(int nesting = 0)
    {
        if (nesting > AlgorithmType.MAX_DEPTH) throw Error("Der Listentyp ist zu tief verschachtelt.");
        AlgorithmType type;
        if (Take("List") || Take("Liste"))
        {
            if (!Take("<")) throw Error("Eine Liste braucht einen Elementtyp, zum Beispiel Liste<Ganzzahl>.");
            type = ReadType(nesting + 1);
            // Nach einem generischen Typ darf ohne Leerzeichen die Zuweisung folgen.
            if (Is(">=")) tokens[_position] = new("=", Current.Line);
            else Require(">");
            type = type with { Depth = type.Depth + 1 };
        }
        else
        {
            string? scalar = Current.Quoted ? null : AlgorithmType.ScalarName(Current.Text);
            if (scalar == null) throw Error($"Unbekannter Datentyp „{Current.Text.Trim()}“. Verwende etwa Integer, Double, Boolean, String oder List<Integer>.");
            _position++;
            type = new(scalar);
        }
        while (Take("[")) { Require("]"); type = type with { Depth = type.Depth + 1 }; }
        if (type.Depth > AlgorithmType.MAX_DEPTH) throw Error("Der Listentyp ist zu tief verschachtelt.");
        return type;
    }

    private static bool IsName(Token token) => !token.Quoted && token.Text.Length > 0 && (char.IsLetter(token.Text[0]) || token.Text[0] == '_') && !KEYWORDS.Contains(token.Text);

    private string Name()
    {
        Token token = Current;
        if (!IsName(token))
            throw Error("Hier fehlt ein Variablenname oder ein Wert. Prüfe auch Klammern und das Ende der vorherigen Anweisung.");
        _position++;
        return token.Text;
    }
    private bool Is(string text) => !Current.Quoted && Current.Text.Equals(text, StringComparison.OrdinalIgnoreCase);
    private bool Take(string text) { if (!Is(text)) return false; _position++; return true; }
    private void Require(string text) { if (!Take(text)) throw Error($"Hier fehlt „{text}“. Prüfe den Beginn und das Ende dieses Ausdrucks oder Blocks."); }
    private void RequireFor() { if (!Take("FÜR")) Require("FUER"); }

    /// <summary> Die zusätzlichen Formen sind nur im Listenschleifenkopf Schlüsselwörter. </summary>
    private bool TakeEachQuantifier()
    {
        if (Take("JEDES")) return true;
        if (!Is("JEDEN") && !Is("JEDE") && !Is("ALLE")) return false;
        // In „FÜR alle VON …“ bleibt alle der frei gewählte Name des Zählers.
        if (tokens[_position + 1] is { Quoted: false, Text: "VON" }) return false;
        RequireUppercase(Current.Text, Current.Line);
        _position++;
        return true;
    }

    private void SkipLines() { while (Take("\n")) { } }
    private void EndLine()
    {
        if (!Take("\n")) throw Error("Hier fehlt ein Zeilenumbruch. Schreibe jede Anweisung und jeden Blockanfang in eine eigene Zeile; alternativ trennt ein Semikolon die Anweisungen.");
        SkipLines();
    }
    private AlgorithmException Error(string message) => new(Current.Line, message);
    private void Enter() { if (++_depth > 64) throw Error("Der Ausdruck ist zu tief verschachtelt. Teile ihn in mehrere Schritte auf."); }

    #endregion
}
