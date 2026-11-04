using System.Globalization;
using System.Text.Json;
using FiaeTrainer.Core.Algorithms.Content;
using FiaeTrainer.Core.Algorithms.Execution;

namespace FiaeTrainer.Core.Algorithms.Evaluation;

/// <summary> Prüft Code, Ausgabevorhersagen und selbst gewählte Testfälle nach ihrem fachlichen Ergebnis. </summary>
public sealed class AlgorithmEvaluator(AlgorithmInterpreter interpreter)
{
    /// <summary> Unterstützt verschiedene korrekte Algorithmen; die Prüfmenge ist endlich und keine IHK-Benotung. </summary>
    public AlgorithmEvaluation Evaluate(AlgorithmLesson lesson, string answer)
    {
        if (string.IsNullOrWhiteSpace(answer)) return new(false, "Trage zuerst deine Lösung ein.", []);
        if (answer.Length > 12000) return new(false, "Deine Antwort darf höchstens 12.000 Zeichen enthalten.", []);
        if (lesson.Kind == AlgorithmTaskKind.TestCases) return EvaluateCases(lesson, answer);
        List<AlgorithmCheck> checks = [];
        if (lesson.Kind == AlgorithmTaskKind.Trace)
        {
            string[] expected = lesson.Tests[0].Output;
            string[] actual = Lines(answer);
            checks.Add(new("Vorhergesagte Ausgaben", Inputs(lesson.ExampleInputs), string.Join(" → ", expected), string.Join(" → ", actual), Same(expected, actual)));
        }
        else
        {
            foreach (AlgorithmTest test in lesson.Tests)
            {
                AlgorithmRun run = lesson.FunctionName.Length == 0
                    ? interpreter.Run(answer, test.Inputs)
                    : interpreter.RunFunction(answer, lesson.FunctionName, lesson.FunctionInputs.Select(name => test.Inputs[name]).ToArray());
                checks.Add(new(test.Name, Inputs(test.Inputs), string.Join(" → ", test.Output), run.Error ?? (run.Output.Count == 0 ? "Keine Ausgabe" : string.Join(" → ", run.Output)), run.Error == null && Same(test.Output, run.Output)));
                if (run.Error != null) return new(false, run.Error, checks);
            }
        }
        return Finish(checks);
    }

    /// <summary> Erwartet pro Zeile „Zahl => Ausgabe“ für die benannte Eingabe und prüft Korrektheit sowie geforderte Eingabegruppen. </summary>
    private AlgorithmEvaluation EvaluateCases(AlgorithmLesson lesson, string answer)
    {
        List<AlgorithmCheck> checks = [];
        List<decimal> values = [];
        string[] lines = Lines(answer);
        if (lines.Length > 24) return new(false, "Verwende höchstens 24 Testfälle, jeweils einen pro Zeile.", []);
        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split("=>", 2, StringSplitOptions.TrimEntries);
            if (parts.Length != 2 || !decimal.TryParse(parts[0], NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal input) || input != decimal.Truncate(input))
                return new(false, $"Antwortzeile {i + 1}: Schreibe eine ganze Zahl für {lesson.CaseInputName}, dann => und die erwartete Ausgabe. Verwende pro Zeile genau einen Testfall.", checks);
            Dictionary<string, JsonElement> inputs = new() { [lesson.CaseInputName] = JsonSerializer.SerializeToElement(input) };
            AlgorithmRun run = interpreter.Run(lesson.ReferenceCode, inputs);
            if (run.Error != null) throw new InvalidDataException("Die Testfall-Referenz ist nicht ausführbar: " + run.Error);
            bool correct = Same(run.Output, [parts[1]]);
            checks.Add(new($"Testfall {i + 1}", $"{lesson.CaseInputName} = {input.ToString(CultureInfo.InvariantCulture)}", string.Join(" → ", run.Output), parts[1], correct));
            if (correct) values.Add(input);
        }
        foreach (TestCoverage group in lesson.Coverage)
        {
            bool covered = values.Any(x => (!group.Minimum.HasValue || x >= group.Minimum) && (!group.Maximum.HasValue || x <= group.Maximum));
            checks.Add(new(group.Name, "Eingabegruppen", "Mindestens ein korrekter Testfall", covered ? "Abgedeckt" : "Noch nicht abgedeckt", covered));
        }
        return Finish(checks);
    }

    private static AlgorithmEvaluation Finish(List<AlgorithmCheck> checks)
    {
        bool correct = checks.Count > 0 && checks.All(x => x.Passed);
        return new(correct, correct ? "Deine Lösung erfüllt alle Prüffälle dieser Aufgabe." : $"{checks.Count(x => x.Passed)} von {checks.Count} Prüfungen erfüllt. Schau dir den ersten abweichenden Fall an.", checks);
    }
    private static bool Same(IReadOnlyList<string> expected, IReadOnlyList<string> actual) => expected.Count == actual.Count && expected.Zip(actual).All(x => Normalize(x.First) == Normalize(x.Second));
    private static string Normalize(string value) => value.Trim();
    private static string[] Lines(string text) => text.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    private static string Inputs(IReadOnlyDictionary<string, JsonElement> inputs) => inputs.Count == 0 ? "Keine zusätzlichen Eingaben" : string.Join(", ", inputs.Select(x => $"{x.Key} = {AlgorithmDisplay.Input(x.Value)}"));
}
