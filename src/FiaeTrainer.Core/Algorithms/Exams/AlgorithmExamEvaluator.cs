using System.Globalization;
using System.Text.Json;
using FiaeTrainer.Core.Algorithms.Content;
using FiaeTrainer.Core.Algorithms.Execution;
using FiaeTrainer.Core.Exams;

namespace FiaeTrainer.Core.Algorithms.Exams;

/// <summary> Vergibt feste Teilpunkte pro Prüffall, ohne vorzeitig abzubrechen oder beliebige Zusatzfälle zu belohnen. </summary>
public sealed class AlgorithmExamEvaluator(AlgorithmInterpreter interpreter)
{
    /// <summary> Bewertet erst die abgegebene Antwort; Syntaxfehler erhalten keine erfundenen Ansatzpunkte. </summary>
    public ExamTaskResult Evaluate(AlgorithmExamTask task, string answer)
    {
        AlgorithmLesson lesson = task.Lesson;
        List<(string Name, bool Passed, string Detail)> checks = [];
        if (answer.Length > 12000) return new() { TaskId = lesson.Id, Checks = [new("Antwortumfang", 0, task.Points, "Die Antwort überschreitet 12.000 Zeichen.")] };
        string[] lines = answer.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (lesson.Kind == AlgorithmTaskKind.Trace)
        {
            string[] expected = lesson.Tests[0].Output;
            for (int i = 0; i < expected.Length; i++)
                checks.Add(($"Ausgabe {i + 1}", lines.Length == expected.Length && EquivalentLine(lines[i], expected[i]), $"Erwartet: {expected[i]} · Deine Ausgabe: {(i < lines.Length ? lines[i] : "fehlt")}. Genau {expected.Length} Zeilen sind gefordert."));
        }
        else if (lesson.Kind == AlgorithmTaskKind.TestCases)
        {
            List<decimal> validValues = [];
            bool valid = lines.Length is > 0 and <= 24;
            foreach (string line in lines)
            {
                string[] parts = line.Split("=>", 2, StringSplitOptions.TrimEntries);
                if (parts.Length != 2 || !decimal.TryParse(parts[0], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal value)) { valid = false; break; }
                AlgorithmRun run = interpreter.Run(lesson.ReferenceCode, new Dictionary<string, JsonElement> { [lesson.CaseInputName] = JsonSerializer.SerializeToElement(value) });
                if (run.Error != null || !run.Output.SequenceEqual([parts[1]])) { valid = false; break; }
                validValues.Add(value);
            }
            foreach (TestCoverage coverage in lesson.Coverage)
                checks.Add((coverage.Name, valid && validValues.Any(value => (coverage.Minimum == null || value >= coverage.Minimum) && (coverage.Maximum == null || value <= coverage.Maximum)), valid ? "Geforderte Eingabegruppe mit richtiger Sollausgabe abdecken." : "Mindestens ein Testfall fehlt oder besitzt ein falsches Format beziehungsweise eine falsche Sollausgabe."));
        }
        else
        {
            foreach (AlgorithmTest test in lesson.Tests)
            {
                AlgorithmRun run = interpreter.RunFunction(answer, lesson.FunctionName, lesson.FunctionInputs.Select(name => test.Inputs[name]).ToArray());
                bool passed = run.Error == null && run.Output.SequenceEqual(test.Output);
                checks.Add((test.Name, passed, $"Eingaben: {JsonSerializer.Serialize(test.Inputs)} · Erwartet: {string.Join(" → ", test.Output)} · Deine Lösung: {run.Error ?? string.Join(" → ", run.Output)}"));
            }
        }
        ExamTaskResult result = new() { TaskId = lesson.Id };
        for (int i = 0; i < checks.Count; i++)
        {
            int points = task.Points / checks.Count + (i < task.Points % checks.Count ? 1 : 0);
            result.Checks.Add(new(checks[i].Name, checks[i].Passed ? points : 0, points, checks[i].Detail));
        }
        return result;
    }
    /// <summary> Unwichtige Leerzeichen in ausgegebenen Listen sind kein fachlicher Fehler. </summary>
    private static bool EquivalentLine(string actual, string expected)
    {
        if (actual == expected) return true;
        if (!expected.StartsWith('[')) return false;
        try { using JsonDocument left = JsonDocument.Parse(actual); using JsonDocument right = JsonDocument.Parse(expected); return JsonElement.DeepEquals(left.RootElement, right.RootElement); }
        catch (JsonException) { return false; }
    }
}
