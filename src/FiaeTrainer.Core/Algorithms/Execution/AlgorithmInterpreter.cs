using System.Globalization;
using System.Text.Json;
using FiaeTrainer.Core.Algorithms.Syntax;
using static FiaeTrainer.Core.Algorithms.Syntax.AlgorithmSyntax;

namespace FiaeTrainer.Core.Algorithms.Execution;

/// <summary> Führt ausschließlich die lokale Pseudocode-Teilmenge mit begrenztem Rechen- und Speicheraufwand aus. </summary>
public sealed class AlgorithmInterpreter
{
    /// <summary> Prüft und startet einen vollständig isolierten Ablauf; normale Lernfehler werden als Ergebnis zurückgegeben. </summary>
    public AlgorithmRun Run(string code, IReadOnlyDictionary<string, JsonElement>? inputs = null, bool trace = false)
    {
        if (string.IsNullOrWhiteSpace(code)) return new([], [], "Schreibe zuerst deinen Ablauf in den Editor.");
        if (code.Length > 12000) return new([], [], "Dein Ablauf ist zu lang. Maximal 12.000 Zeichen sind möglich.");
        Machine machine = new(trace);
        try
        {
            IReadOnlyList<Statement> program = new AlgorithmParser(Tokenize(code)).Parse();
            if (inputs != null)
                foreach (var input in inputs) machine.Variables[input.Key] = FromJson(input.Value);
            machine.Execute(program);
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

        #endregion

        #region Properties

        /// <summary> Nur für diesen Aufruf gültige Werte; Namen sind unabhängig von Groß-/Kleinschreibung. </summary>
        internal Dictionary<string, object> Variables { get; } = new(StringComparer.OrdinalIgnoreCase);
        /// <summary> Vollständige Ausgabe innerhalb des vorgegebenen Budgets. </summary>
        internal List<string> Output { get; } = [];
        /// <summary> Unveränderliche Momentaufnahmen der ersten begrenzten Ausführungsschritte. </summary>
        internal List<AlgorithmStep> Steps { get; } = [];
        /// <summary> Die Codeausführung läuft weiter, auch wenn die Anzeige ihr eigenes Budget erreicht. </summary>
        internal bool TraceTruncated { get; private set; }

        #endregion

        #region Methods

        /// <summary> Interpretiert nur bekannte Anweisungen und zählt jeden Schleifenzyklus gegen das Budget. </summary>
        internal void Execute(IReadOnlyList<Statement> statements)
        {
            foreach (Statement statement in statements)
            {
                _line = statement.Line;
                Tick();
                switch (statement)
                {
                    case Assignment assignment:
                        Variables[assignment.Name] = Evaluate(assignment.Value); Snapshot($"{assignment.Name} zuweisen"); break;
                    case Print print:
                        string text = Format(Evaluate(print.Value));
                        _outputSize += text.Length;
                        if (Output.Count >= 200 || _outputSize > 16000) Fail("Zu viele Ausgaben. Prüfe deine Schleife oder gib nur das gesuchte Endergebnis aus.");
                        Output.Add(text); Snapshot("Ausgabe"); break;
                    case Branch branch:
                        bool condition = Boolean(Evaluate(branch.Condition)); Snapshot($"Bedingung: {(condition ? "WAHR" : "FALSCH")}"); Execute(condition ? branch.Then : branch.Else); break;
                    case While loop:
                        while (true)
                        {
                            _line = loop.Line; Tick(); bool repeat = Boolean(Evaluate(loop.Condition)); Snapshot($"Schleifenbedingung: {(repeat ? "WAHR" : "FALSCH")}");
                            if (!repeat) break;
                            Execute(loop.Body);
                        }
                        break;
                    case For loop:
                        decimal start = Integer(Evaluate(loop.Start)), end = Integer(Evaluate(loop.End));
                        for (decimal i = start; i <= end; i++)
                        {
                            _line = loop.Line; Tick(); Variables[loop.Name] = i; Snapshot($"Schleifendurchlauf: {loop.Name}"); Execute(loop.Body);
                        }
                        _line = loop.Line; Snapshot("Zählschleife beendet"); break;
                    case Each loop:
                        foreach (object item in Array(Evaluate(loop.Array)))
                        {
                            _line = loop.Line; Tick(); Variables[loop.Name] = item; Snapshot($"Nächster Eintrag: {loop.Name}"); Execute(loop.Body);
                        }
                        _line = loop.Line; Snapshot("Listenschleife beendet"); break;
                }
            }
        }

        private object Evaluate(Expression expression)
        {
            Tick();
            switch (expression)
            {
                case Literal literal: return literal.Value;
                case Variable variable: return Variables.TryGetValue(variable.Name, out object? value) ? value : throw new AlgorithmException(_line, $"„{variable.Name}“ hat noch keinen Wert. Prüfe den Namen oder weise der Variablen zuerst etwas zu.");
                case Unary unary:
                    object operand = Evaluate(unary.Operand);
                    return unary.Operator switch { "NICHT" => !Boolean(operand), "-" => -Number(operand), _ => Number(operand) };
                case ArrayLiteral literal:
                    object[] items = literal.Items.Select(Evaluate).ToArray();
                    int size = 0; CheckValue(items, 0, ref size); return items;
                case Length length: return (decimal)Array(Evaluate(length.Array)).Length;
                case ListIndex index:
                    object[] array = Array(Evaluate(index.Array)); decimal position = Integer(Evaluate(index.Position));
                    if (position < 0 || position >= array.Length) Fail($"Der Listenindex {position} ist außerhalb der Liste. Der erste Index ist 0, der letzte ist LÄNGE(liste) − 1.");
                    return array[(int)position];
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
            Steps.Add(new(_line, action, values, Output.ToArray()));
        }
        /// <summary> Verhindert übergroße oder exponentiell verschachtelte Listen auch bei wiederverwendeten Werten. </summary>
        private void CheckValue(object value, int depth, ref int size)
        {
            size += value is string text ? text.Length : 1;
            if (depth > 16 || size > 2048) Fail("Diese Liste ist zu groß oder zu tief verschachtelt. Verwende kleinere Werte für die Aufgabe.");
            if (value is object[] items)
                foreach (object item in items) CheckValue(item, depth + 1, ref size);
        }
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
