using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using FiaeTrainer.Core.Algorithms.Syntax;
using static FiaeTrainer.Core.Algorithms.Syntax.AlgorithmSyntax;

namespace FiaeTrainer.Core.Algorithms.Execution;

/// <summary> Führt ausschließlich die lokale Pseudocode-Teilmenge mit begrenztem Rechen- und Speicheraufwand aus. </summary>
public sealed class AlgorithmInterpreter
{
    /// <summary> Prüft und startet einen vollständig isolierten Ablauf; normale Lernfehler werden als Ergebnis zurückgegeben. </summary>
    public AlgorithmRun Run(string code, IReadOnlyDictionary<string, JsonElement>? inputs = null, bool trace = false)
        => RunCore(code, inputs, trace, null, []);

    /// <summary> Prüft eine benannte Funktion mit isolierten Argumenten; nur ihr Rückgabewert ist die erwartete Ausgabe. </summary>
    public AlgorithmRun RunFunction(string code, string name, IReadOnlyList<JsonElement> arguments, bool trace = false)
        => RunCore(code, null, trace, name, arguments);

    /// <summary> Beide Bewertungswege teilen Parser, Ausführungsbudgets und deutsche Fehlerbehandlung. </summary>
    private static AlgorithmRun RunCore(string code, IReadOnlyDictionary<string, JsonElement>? inputs, bool trace, string? function, IReadOnlyList<JsonElement> arguments)
    {
        if (string.IsNullOrWhiteSpace(code)) return new([], [], "Schreibe zuerst deinen Ablauf in den Editor.");
        if (code.Length > 12000) return new([], [], "Dein Ablauf ist zu lang. Maximal 12.000 Zeichen sind möglich.");
        Machine machine = new(trace);
        try
        {
            AlgorithmProgram program = new AlgorithmParser(Tokenize(code)).Parse();
            machine.Register(program.Functions);
            if (inputs != null)
                foreach (var input in inputs) machine.Variables[input.Key] = FromJson(input.Value);
            if (function == null) machine.Execute(program.Statements);
            else
            {
                if (program.Statements.Count > 0)
                    throw new AlgorithmException(program.Statements[0].Line, "Diese Aufgabe erwartet nur Funktionsdefinitionen. Die Testfälle rufen deine Funktion automatisch auf; ein Hauptablauf mit AUSGABE gehört hier nicht dazu.");
                machine.PrintResult(machine.Invoke(function, arguments.Select(FromJson).ToArray()), false);
            }
            return new(machine.Output, machine.Steps, TraceTruncated: machine.TraceTruncated);
        }
        catch (AlgorithmException exception) { return new(machine.Output, machine.Steps, exception.Message, machine.TraceTruncated); }
        catch (OverflowException) { return new(machine.Output, machine.Steps, "Die Berechnung überschreitet den Zahlenbereich. Prüfe Faktoren und Schleifengrenzen.", machine.TraceTruncated); }
    }

    /// <summary> Formatiert Ausgaben unabhängig von der eingestellten Betriebssystemsprache. </summary>
    internal static string Format(object value) => value switch
    {
        decimal number => number.ToString("0.############################", CultureInfo.InvariantCulture),
        bool boolean => boolean ? "WAHR" : "FALSCH", object[] items => "[" + string.Join(", ", items.Select(Format)) + "]", _ => (string)value
    };

    private static object FromJson(JsonElement value) => value.ValueKind switch
    {
        JsonValueKind.Number => value.GetDecimal(), JsonValueKind.String => value.GetString() ?? "", JsonValueKind.True => true, JsonValueKind.False => false,
        JsonValueKind.Array when value.GetArrayLength() <= 256 => value.EnumerateArray().Select(FromJson).ToArray(),
        _ => throw new ArgumentException("Eingabedaten benötigen Zahlen, Texte, Wahrheitswerte oder kleine Listen.")
    };

    /// <summary> Ein Ausführungszustand gehört immer genau einem Aufruf; kein Zugriff auf Dateien, Netzwerk oder .NET-Code. </summary>
    private sealed class Machine(bool trace)
    {
        #region Fields

        /// <summary> Gesamtbudget einschließlich Ausdrucksauswertung und leerer Schleifendurchläufe. </summary>
        private int _operations;
        /// <summary> Ursprüngliche Zeile für fachlich zuordenbare Laufzeitfehler. </summary>
        private int _line = 1;
        /// <summary> Zeichenbudget der tatsächlich erzeugten Ausgaben. </summary>
        private int _outputSize;
        /// <summary> Begrenzung der kopierten Zustandsdaten der Vorschau. </summary>
        private int _traceSize;
        /// <summary> Vor Ausführung registrierte Definitionen; es existieren keine extern aufrufbaren Funktionen. </summary>
        private readonly Dictionary<string, Function> _functions = new(StringComparer.OrdinalIgnoreCase);
        /// <summary> Aktive Aufrufe für Bereichsanzeige und gemeinsame Tiefengrenze einschließlich rekursiver Aufrufe. </summary>
        private readonly Stack<string> _calls = new();

        #endregion

        #region Properties

        /// <summary> Nur für diesen Aufruf gültige Werte; Namen sind unabhängig von Groß-/Kleinschreibung. </summary>
        internal Dictionary<string, object> Variables { get; private set; } = new(StringComparer.OrdinalIgnoreCase);
        /// <summary> Vollständige Ausgabe innerhalb des vorgegebenen Budgets. </summary>
        internal List<string> Output { get; } = [];
        /// <summary> Unveränderliche Momentaufnahmen der ersten begrenzten Ausführungsschritte. </summary>
        internal List<AlgorithmStep> Steps { get; } = [];
        /// <summary> Die Codeausführung läuft weiter, auch wenn die Anzeige ihr eigenes Budget erreicht. </summary>
        internal bool TraceTruncated { get; private set; }

        #endregion

        #region Methods

        /// <summary> Interpretiert bekannte Anweisungen mit gemeinsamem Budget; ein nicht-null-Ergebnis signalisiert die Rückgabe aus einer Funktion. </summary>
        internal object? Execute(IReadOnlyList<Statement> statements)
        {
            foreach (Statement statement in statements)
            {
                _line = statement.Line;
                Tick();
                switch (statement)
                {
                    case Assignment assignment:
                        Variables[assignment.Name] = CopyValue(Evaluate(assignment.Value)); Snapshot($"{assignment.Name} zuweisen"); break;
                    case IndexedAssignment assignment:
                        AssignIndex(assignment); Snapshot($"{assignment.Name}: Listeneintrag zuweisen"); break;
                    case Return result:
                        object returned = CopyValue(Evaluate(result.Value)); Snapshot($"Rückgabe: {Format(returned)}"); return returned;
                    case Print print:
                        PrintResult(Evaluate(print.Value)); break;
                    case Branch branch:
                        bool condition = Boolean(Evaluate(branch.Condition)); Snapshot($"Bedingung: {(condition ? "WAHR" : "FALSCH")}");
                        object? branchResult = Execute(condition ? branch.Then : branch.Else);
                        if (branchResult != null) return branchResult;
                        break;
                    case While loop:
                        while (true)
                        {
                            _line = loop.Line; Tick(); bool repeat = Boolean(Evaluate(loop.Condition)); Snapshot($"Schleifenbedingung: {(repeat ? "WAHR" : "FALSCH")}");
                            if (!repeat) break;
                            object? whileResult = Execute(loop.Body);
                            if (whileResult != null) return whileResult;
                        }
                        break;
                    case For loop:
                        decimal start = Integer(Evaluate(loop.Start)), end = Integer(Evaluate(loop.End));
                        for (decimal i = start; i <= end; i++)
                        {
                            _line = loop.Line; Tick(); Variables[loop.Name] = i; Snapshot($"Schleifendurchlauf: {loop.Name}");
                            object? forResult = Execute(loop.Body);
                            if (forResult != null) return forResult;
                        }
                        _line = loop.Line; Snapshot("Zählschleife beendet"); break;
                    case Each loop:
                        foreach (object item in Array(Evaluate(loop.Array)))
                        {
                            _line = loop.Line; Tick(); Variables[loop.Name] = CopyValue(item); Snapshot($"Nächster Eintrag: {loop.Name}");
                            object? eachResult = Execute(loop.Body);
                            if (eachResult != null) return eachResult;
                        }
                        _line = loop.Line; Snapshot("Listenschleife beendet"); break;
                }
            }
            return null;
        }

        /// <summary> Macht den zurückgegebenen Wert eines Testaufrufs wie eine gewöhnliche Ausgabe sichtbar. </summary>
        internal void PrintResult(object value, bool snapshot = true)
        {
            string text = Format(value);
            _outputSize += text.Length;
            if (Output.Count >= 200 || _outputSize > 16000) Fail("Zu viele Ausgaben. Prüfe deine Schleife oder gib nur das gesuchte Endergebnis aus.");
            Output.Add(text);
            if (snapshot) Snapshot("Ausgabe");
        }

        /// <summary> Vorwärtsaufrufe sind möglich; jede Definition wurde zuvor vollständig geparst. </summary>
        internal void Register(IReadOnlyList<Function> functions)
        {
            foreach (Function function in functions) _functions.Add(function.Name, function);
        }

        /// <summary> Ein Aufruf besitzt eigene Variablen und Wertkopien der Argumente; Rückgabe beendet auch verschachtelte Blöcke. </summary>
        internal object Invoke(string name, IReadOnlyList<object> arguments)
        {
            Tick();
            if (!_functions.TryGetValue(name, out Function? function)) throw new AlgorithmException(_line, $"Die Funktion „{name}“ ist nicht definiert. Prüfe ihren Namen und FUNKTION … ENDE FUNKTION.");
            if (arguments.Count != function.Parameters.Count) Fail($"„{name}“ erwartet {function.Parameters.Count} Argumente, übergeben wurden {arguments.Count}.");
            if (_calls.Count >= 16) Fail("Zu viele verschachtelte Funktionsaufrufe. Verwende höchstens 16 Aufrufebenen. Prüfe bei Rekursion den Basisfall und ob sich das Argument diesem Fall nähert.");
            Dictionary<string, object> caller = Variables;
            int callerLine = _line;
            Variables = new(StringComparer.OrdinalIgnoreCase);
            _calls.Push(function.Name);
            try
            {
                for (int i = 0; i < arguments.Count; i++) Variables[function.Parameters[i]] = CopyValue(arguments[i]);
                _line = function.Line; Snapshot($"Funktion {function.Name} aufrufen");
                object? result = Execute(function.Body);
                if (result == null) Fail($"Die Funktion „{function.Name}“ endet ohne RÜCKGABE. Jeder ausgeführte Pfad braucht einen Rückgabewert.");
                return result;
            }
            finally { _calls.Pop(); Variables = caller; _line = callerLine; }
        }

        /// <summary> Ersetzt Einträge auf einer Wertkopie, damit Aliase und zyklische Listen keine Nebenwirkungen erzeugen. </summary>
        private void AssignIndex(IndexedAssignment assignment)
        {
            if (!Variables.TryGetValue(assignment.Name, out object? original)) Fail($"„{assignment.Name}“ hat noch keinen Wert. Lege zuerst die Liste an.");
            object[] root = (object[])CopyValue(Array(original));
            object[] target = root;
            for (int i = 0; i < assignment.Indices.Count; i++)
            {
                int position = Position(target, Evaluate(assignment.Indices[i]));
                if (i + 1 < assignment.Indices.Count) target = Array(target[position]);
                else target[position] = CopyValue(Evaluate(assignment.Value));
            }
            int size = 0; CheckValue(root, 0, ref size);
            Variables[assignment.Name] = root;
        }

        /// <summary> Listen besitzen Wertsemantik, auch bei Parametern, Rückgaben und verschachtelten Einträgen. </summary>
        private object CopyValue(object value)
        {
            if (value is not object[] items) return value;
            int size = 0; CheckValue(items, 0, ref size);
            return items.Select(CopyValue).ToArray();
        }

        private int Position(object[] array, object value)
        {
            decimal position = Integer(value);
            if (position < 0 || position >= array.Length) Fail($"Der Listenindex {position} ist außerhalb der Liste. Der erste Index ist 0, der letzte ist LÄNGE(liste) − 1.");
            return (int)position;
        }

        private object Evaluate(Expression expression)
        {
            Tick();
            switch (expression)
            {
                case Literal literal: return literal.Value;
                case Variable variable: return Variables.TryGetValue(variable.Name, out object? value) ? value : throw new AlgorithmException(_line, $"„{variable.Name}“ hat noch keinen Wert. Prüfe den Namen oder weise der Variablen zuerst etwas zu.");
                case Call call: return Invoke(call.Name, call.Arguments.Select(Evaluate).ToArray());
                case Unary unary:
                    object operand = Evaluate(unary.Operand);
                    return unary.Operator switch { "NICHT" => !Boolean(operand), "-" => -Number(operand), _ => Number(operand) };
                case ArrayLiteral literal:
                    object[] items = literal.Items.Select(Evaluate).ToArray();
                    int size = 0; CheckValue(items, 0, ref size); return items;
                case Length length: return (decimal)Array(Evaluate(length.Array)).Length;
                case ListIndex index:
                    object[] array = Array(Evaluate(index.Array));
                    return array[Position(array, Evaluate(index.Position))];
                case Binary binary:
                    object left = Evaluate(binary.Left);
                    if (binary.Operator == "UND") return Boolean(left) && Boolean(Evaluate(binary.Right));
                    if (binary.Operator == "ODER") return Boolean(left) || Boolean(Evaluate(binary.Right));
                    object right = Evaluate(binary.Right);
                    return binary.Operator switch
                    {
                        "+" => Number(left) + Number(right), "-" => Number(left) - Number(right), "*" => Number(left) * Number(right),
                        "/" => Number(left) / NonZero(right), "DIV" => decimal.Truncate(Number(left) / NonZero(right)), "MOD" or "%" => Number(left) % NonZero(right),
                        "=" or "==" => Equal(left, right), "<>" or "!=" => !Equal(left, right),
                        "<" => Compare(left, right) < 0, "<=" => Compare(left, right) <= 0, ">" => Compare(left, right) > 0, ">=" => Compare(left, right) >= 0,
                        _ => throw new AlgorithmException(_line, "Dieser Operator wird noch nicht unterstützt.")
                    };
                default: throw new InvalidOperationException("Unbekanntes Syntaxelement.");
            }
        }

        private void Tick() { if (++_operations > 10000) Fail("Der Ablauf wurde nach 10.000 Rechenschritten angehalten. Prüfe, ob sich die Schleifenbedingung verändert und die Schleife enden kann."); }
        private void Snapshot(string action)
        {
            if (!trace || TraceTruncated) return;
            if (Steps.Count >= 500) { TraceTruncated = true; return; }
            Dictionary<string, string> values = Variables.ToDictionary(x => x.Key, x => Format(x.Value));
            _traceSize += values.Sum(x => x.Key.Length + x.Value.Length) + Output.Sum(x => x.Length);
            if (_traceSize > 200000) { TraceTruncated = true; return; }
            Steps.Add(new(_line, action, values, Output.ToArray(), _calls.TryPeek(out string? name) ? $"Funktion {name}" : "Hauptablauf", _calls.Count));
        }
        /// <summary> Verhindert übergroße oder exponentiell verschachtelte Listen auch bei wiederverwendeten Werten. </summary>
        private void CheckValue(object value, int depth, ref int size)
        {
            size += value is string text ? text.Length : 1;
            if (depth > 16 || size > 2048) Fail("Diese Liste ist zu groß oder zu tief verschachtelt. Verwende kleinere Werte für die Aufgabe.");
            if (value is object[] items)
                foreach (object item in items) CheckValue(item, depth + 1, ref size);
        }
        [DoesNotReturn]
        private void Fail(string message) => throw new AlgorithmException(_line, message);
        private decimal Number(object value) => value is decimal number ? number : throw new AlgorithmException(_line, "Diese Berechnung braucht Zahlen. Ein Text oder Wahrheitswert kann hier nicht verwendet werden.");
        private decimal Integer(object value) { decimal number = Number(value); if (decimal.Truncate(number) != number) Fail("Listenindizes und Zählschleifengrenzen müssen ganze Zahlen sein."); return number; }
        private decimal NonZero(object value) { decimal number = Number(value); if (number == 0) Fail("Eine Division durch 0 ist nicht möglich. Prüfe den Nenner und den Fall einer leeren Liste."); return number; }
        private bool Boolean(object value) => value is bool boolean ? boolean : throw new AlgorithmException(_line, "WENN, SOLANGE, UND, ODER und NICHT brauchen einen Wahrheitswert. Verwende einen Vergleich wie „anzahl > 0“.");
        private object[] Array(object value) => value as object[] ?? throw new AlgorithmException(_line, "Hier wird eine Liste erwartet. Verwende eine Eingabeliste oder zum Beispiel [10, 20, 30].");
        private bool Equal(object left, object right)
        {
            if (left.GetType() != right.GetType()) Fail("Die Vergleichswerte haben verschiedene Typen. Zahlen werden ohne Anführungszeichen geschrieben.");
            if (left is object[]) Fail("Vergleiche einzelne Listeneinträge statt ganzer Listen.");
            return left.Equals(right);
        }
        private int Compare(object left, object right) => left is decimal a && right is decimal b ? a.CompareTo(b) : left is string x && right is string y ? string.CompareOrdinal(x, y) : throw new AlgorithmException(_line, "Vergleiche hier zwei Zahlen oder zwei Texte desselben Typs.");

        #endregion
    }
}
