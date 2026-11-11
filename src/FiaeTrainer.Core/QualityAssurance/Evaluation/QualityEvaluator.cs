using FiaeTrainer.Core.QualityAssurance.Content;
using FiaeTrainer.Core.QualityAssurance.Models;

namespace FiaeTrainer.Core.QualityAssurance.Evaluation;

/// <summary> Bewertet Fachwerte, Ausführung, geforderte Abdeckung und Fehlererkennung als getrennte Kriterien. </summary>
public static class QualityEvaluator
{
    /// <summary> Freie Begründungen werden nie über Schlüsselwörter oder Textähnlichkeit benotet. </summary>
    public static QualityEvaluation Evaluate(QualityAssignment task, QualityAnswer answer)
    {
        if (!QualityRules.Storable(answer)) return Failure("Der Entwurf ist beschädigt oder überschreitet die Speichergrenzen.");
        if (!QualityRules.HasAnswer(answer)) return Failure("Ergänze zuerst deine fachliche Antwort.");
        if (task.Kind == "TestSuite") return TestSuite(task, answer);
        var allowed = task.Kind == "Matrix" ? task.MatrixRows.Keys.SelectMany(row => task.MatrixColumns.Keys.Select(column => row + "." + column)).ToHashSet() : task.Fields.Select(field => field.Key).ToHashSet();
        if (answer.Values.Keys.Any(key => !allowed.Contains(key))) return Failure("Ein Antwortfeld gehört nicht zu diesem Auftrag.");
        var variants = task.Solutions.Count == 0 ? new List<QualityAnswer> { new() } : task.Solutions;
        return variants.Select(expected => Compare(task, expected, answer)).OrderByDescending(result => result.Earned).First();
    }
    private static QualityEvaluation Compare(QualityAssignment task, QualityAnswer expected, QualityAnswer answer)
    {
        List<(string Name, int Found, int Total, string Detail)> checks = [];
        if (task.Kind is "Choice" or "Review")
        {
            bool match = answer.SelectedIds.Count == answer.SelectedIds.Distinct().Count() && answer.SelectedIds.Order().SequenceEqual(task.CorrectIds.Order());
            checks.Add((task.Kind == "Review" ? "Reviewbefunde" : "Fachliche Entscheidung", match ? 1 : 0, 1, match ? "Deine Auswahl erfüllt den Auftrag." : "Prüfe die gewählten Aussagen beziehungsweise die konkret markierten Befunde anhand der Unterlage."));
        }
        if (task.Kind == "Order")
        {
            bool match = answer.Order.SequenceEqual(expected.Order);
            checks.Add(("Vollständiger Ablauf", match ? 1 : 0, 1, match ? "Schritte und Reihenfolge erfüllen die ausdrücklich vereinbarten Voraussetzungen." : "Prüfe Vollständigkeit und Reihenfolge der Schritte. Maßgeblich sind die im Auftrag genannten Abhängigkeiten."));
        }
        if (task.Kind == "Matrix")
            foreach (var row in task.MatrixRows)
                foreach (var column in task.MatrixColumns)
                {
                    string key = row.Key + "." + column.Key; bool match = answer.Values.GetValueOrDefault(key, "") == expected.Values[key];
                    checks.Add((row.Value + " · " + column.Value, match ? 1 : 0, 1, match ? "Die Entscheidung passt zum ausdrücklich beschriebenen Rollen- und Objektbezug." : "Prüfe diese Berechtigung anhand der Regel. Eine Anmeldung allein erlaubt nicht jede Aktion."));
                }
        else foreach (var field in task.Fields)
        {
            string actual = answer.Values.GetValueOrDefault(field.Key, ""), wanted = expected.Values[field.Key];
            bool match = field.Comparison == "Number" ? QualitySimulator.NumberEqual(wanted, actual) : field.Comparison == "Text" ? string.Join(" ", wanted.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)).Equals(string.Join(" ", actual.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)), StringComparison.OrdinalIgnoreCase) : wanted.Trim() == actual.Trim();
            checks.Add((field.Label, match ? 1 : 0, 1, match ? "Diese Angabe passt zur Unterlage." : string.IsNullOrWhiteSpace(actual) ? "Hier fehlt noch deine Angabe." : "Prüfe diese Angabe anhand der vereinbarten Fachregel oder Beobachtung."));
        }
        return Score(checks);
    }
    /// <summary> Führt nur bekannte Modelle aus und zählt Fehlervarianten ausschließlich bei fachlich richtigen Sollwerten. </summary>
    private static QualityEvaluation TestSuite(QualityAssignment task, QualityAnswer answer)
    {
        var program = QualityPrograms.Find(task.ProgramKey);
        if (program == null || answer.TestCases.Count == 0) return Failure("Lege zuerst einen eigenen Testfall an.");
        if (answer.TestCases.Count > task.MaximumCases) return Failure($"Dieser Auftrag erlaubt höchstens {task.MaximumCases} Testfälle.");
        QualityProbe probe = new(); HashSet<string> partitions = [], faults = [];
        foreach (var test in answer.TestCases)
        {
            var execution = QualitySimulator.Run(program, test.Inputs);
            bool correct = execution != null && QualitySimulator.ExpectedEqual(program, test.Expected, execution.Output);
            var row = new QualityRowResult { Id = test.Id, Inputs = string.Join(", ", program.Inputs.Select(input => input.Label + " = " + input.Options.GetValueOrDefault(test.Inputs.GetValueOrDefault(input.Key, ""), test.Inputs.GetValueOrDefault(input.Key, "")))), Expected = test.Expected, Execution = execution, ExpectedCorrect = correct, Detail = execution == null ? "Ergänze alle Parameter im angegebenen Testdatenbereich und Format." : correct ? "Der Sollwert stimmt mit dem Fachvertrag überein." : "Der Sollwert passt noch nicht zum Fachvertrag; eine dadurch fehlgeschlagene Assertion ist kein nachgewiesener Programmfehler." };
            if (execution != null) partitions.UnionWith(QualitySimulator.Partitions(program, test.Inputs));
            if (correct)
                foreach (string fault in program.Faults.Keys)
                    if (QualitySimulator.Run(program, test.Inputs, fault)?.Output != execution!.Output) { row.DetectedFaults.Add(fault); faults.Add(fault); }
            probe.Rows.Add(row);
        }
        List<(string Name, int Found, int Total, string Detail)> checks = [("Testdaten und Sollwerte", probe.Rows.Count(row => row.ExpectedCorrect), probe.Rows.Count, "Jeder Fall braucht gültige Eingaben und einen unabhängig aus der Spezifikation abgeleiteten Sollwert.")];
        void Add(string name, List<string> required, HashSet<string> reached, IReadOnlyDictionary<string, string> labels)
        {
            if (required.Count == 0) return;
            string[] missing = required.Where(key => !reached.Contains(key)).Select(key => labels.GetValueOrDefault(key, key)).ToArray();
            checks.Add((name, required.Count - missing.Length, required.Count, missing.Length == 0 ? "Die geforderte Abdeckung ist erreicht." : "Noch nicht abgedeckt: " + string.Join("; ", missing) + "."));
        }
        Add("Klassen und Grenzen", task.RequiredPartitions, partitions, program.Partitions);
        Add("Gezählte Anweisungen", task.RequiredStatements, probe.Statements, program.Statements);
        Add("Bedingte Zweige", task.RequiredBranches, probe.Branches, program.Branches);
        Add("Erreichbare Pfade", task.RequiredPaths, probe.Paths, program.Paths);
        Add("Gezielte Fehlervarianten", task.RequiredFaults, faults, program.Faults);
        var result = Score(checks); result.Probe = probe; return result;
    }
    /// <summary> Verteilt die internen Anteile exakt auf 50; die Lernansicht zeigt keine Punkte. </summary>
    private static QualityEvaluation Score(List<(string Name, int Found, int Total, string Detail)> checks)
    {
        var result = new QualityEvaluation();
        for (int i = 0; i < checks.Count; i++)
        {
            var check = checks[i]; int maximum = 50 * (i + 1) / checks.Count - 50 * i / checks.Count;
            result.Checks.Add(new() { Name = check.Name, Maximum = maximum, Earned = maximum * check.Found / Math.Max(1, check.Total), Detail = check.Detail });
        }
        return result;
    }
    private static QualityEvaluation Failure(string detail) => new() { Checks = [new() { Name = "Antwort vervollständigen", Maximum = 50, Detail = detail }] };
}
