using System.Text.Json;
using FiaeTrainer.Core.Algorithms.Content;
using FiaeTrainer.Core.Algorithms.Evaluation;
using FiaeTrainer.Core.Algorithms.Execution;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Unabhängige Ausgaben und Fehlerfälle für Listenänderungen, Funktionsverträge und den erweiterten Lernweg. </summary>
public sealed class AdvancedAlgorithmTests
{
    /// <summary> Jeder Test verwendet vollständig neue Ausführungszustände. </summary>
    private readonly AlgorithmInterpreter _interpreter = new();
    /// <summary> Stabile Aufgaben-IDs halten die bisherigen Lernstände kompatibel. </summary>
    private readonly AlgorithmCatalog _catalog = new();

    [Theory]
    [InlineData("a = [1, 2]\na[0] = 8\nAUSGABE a", "[8, 2]")]
    [InlineData("a = [[1, 2], [3]]\na[0][1] = 9\nAUSGABE a", "[[1, 9], [3]]")]
    [InlineData("a = [[1], [2]]\nb = a\nb[0][0] = 8\nAUSGABE a\nAUSGABE b", "[[1], [2]]|[[8], [2]]")]
    [InlineData("a = [0]\na[0] = a\nAUSGABE a", "[[0]]")]
    [InlineData("werte = [1, 2]\nFÜR JEDES zahl IN werte\nAUSGABE zahl\nwerte[1] = 9\nENDE FÜR\nAUSGABE werte", "1|2|[1, 9]")]
    [InlineData("FUNKTION doppelt(x)\nRUECKGABE x * 2\nENDE FUNKTION\nAUSGABE doppelt(3) + doppelt(5)", "16")]
    [InlineData("AUSGABE quadrat(3)\nFUNKTION quadrat(wert)\nRÜCKGABE wert * wert\nENDE FUNKTION", "9")]
    [InlineData("FUNKTION probe()\nRÜCKGABE 0\nAUSGABE 9\nENDE FUNKTION\nAUSGABE probe()", "0")]
    [InlineData("FUNKTION probe()\nRÜCKGABE FALSCH\nENDE FUNKTION\nAUSGABE probe()", "FALSCH")]
    [InlineData("FUNKTION probe()\nRÜCKGABE \"\"\nENDE FUNKTION\nAUSGABE LÄNGE([probe()])", "1")]
    [InlineData("FUNKTION probe(a)\nFÜR JEDES x IN a\nWENN x > 2 DANN\nRÜCKGABE x\nENDE WENN\nENDE FÜR\nRÜCKGABE -1\nENDE FUNKTION\nAUSGABE probe([1, 4, 8])", "4")]
    [InlineData("FUNKTION probe()\nFÜR i VON 0 BIS 10\nRÜCKGABE i\nENDE FÜR\nRÜCKGABE -1\nENDE FUNKTION\nAUSGABE probe()", "0")]
    [InlineData("FUNKTION probe()\nSOLANGE WAHR\nRÜCKGABE 42\nENDE SOLANGE\nENDE FUNKTION\nAUSGABE probe()", "42")]
    [InlineData("FUNKTION a(x)\nRÜCKGABE b(x) + 1\nENDE FUNKTION\nFUNKTION b(x)\nRÜCKGABE x * 2\nENDE FUNKTION\nAUSGABE a(3)", "7")]
    public void ExtendedSyntaxProducesIndependentExpectedValues(string code, string expected)
    {
        AlgorithmRun run = _interpreter.Run(code, trace: true);
        Assert.Null(run.Error);
        Assert.Equal(expected.Split('|'), run.Output);
    }

    [Theory]
    [InlineData("a = [1]\na[-1] = 2", "Listenindex -1")]
    [InlineData("a = [1]\na[1] = 2", "Listenindex 1")]
    [InlineData("a = [1]\na[0.5] = 2", "ganze Zahlen")]
    [InlineData("a = 1\na[0] = 2", "Liste")]
    [InlineData("a[0] = 2", "noch keinen Wert")]
    [InlineData("a = [[1]]\na[0][1] = 2", "Listenindex 1")]
    [InlineData("RÜCKGABE 1", "gehört in eine Funktion")]
    [InlineData("AUSGABE unbekannt(1)", "nicht definiert")]
    [InlineData("FUNKTION f(x)\nRÜCKGABE x\nENDE FUNKTION\nAUSGABE f()", "erwartet 1 Argumente")]
    [InlineData("FUNKTION f()\nRÜCKGABE 1\nENDE FUNKTION\nAUSGABE f(2)", "erwartet 0 Argumente")]
    [InlineData("FUNKTION f(x, X)\nRÜCKGABE 0\nENDE FUNKTION", "nur einmal")]
    [InlineData("FUNKTION f()\nRÜCKGABE 0\nENDE FUNKTION\nFUNKTION F()\nRÜCKGABE 1\nENDE FUNKTION", "mehrfach definiert")]
    [InlineData("FUNKTION f()\nFUNKTION g()\nENDE FUNKTION\nENDE FUNKTION", "außerhalb")]
    [InlineData("FUNKTION f()\nx = 1\nENDE FUNKTION\nAUSGABE f()", "ohne RÜCKGABE")]
    [InlineData("FUNKTION f(x)\nWENN x > 0 DANN\nRÜCKGABE 1\nENDE WENN\nENDE FUNKTION\nAUSGABE f(0)", "ohne RÜCKGABE")]
    [InlineData("FUNKTION f()\nRÜCKGABE f()\nENDE FUNKTION\nAUSGABE f()", "16 Aufrufebenen")]
    [InlineData("FUNKTION f()\nRÜCKGABE g()\nENDE FUNKTION\nFUNKTION g()\nRÜCKGABE f()\nENDE FUNKTION\nAUSGABE f()", "16 Aufrufebenen")]
    [InlineData("x = 7\nFUNKTION f()\nRÜCKGABE x\nENDE FUNKTION\nAUSGABE f()", "noch keinen Wert")]
    [InlineData("FUNKTION f()\nRÜCKGABE 1 / 0\nENDE FUNKTION\nAUSGABE f()", "Zeile 2:")]
    public void InvalidExtensionsHaveUsefulBoundedFeedback(string code, string fragment)
    {
        AlgorithmRun run = _interpreter.Run(code, trace: true);
        Assert.NotNull(run.Error);
        Assert.Contains(fragment, run.Error);
        Assert.True(run.Steps.Count <= 500);
    }

    [Fact]
    public void FunctionFramesAndListArgumentsCannotOverwriteCallerOrEarlierSnapshots()
    {
        const string code = "x = 100\na = [[1], [2]]\nFUNKTION veraendere(x, a)\nx = 5\na[0][0] = 9\nRÜCKGABE a\nENDE FUNKTION\nb = veraendere(x, a)\nAUSGABE x\nAUSGABE a\nAUSGABE b\nAUSGABE veraendere(3, [[7]])";
        AlgorithmRun run = _interpreter.Run(code, trace: true);
        Assert.Null(run.Error);
        Assert.Equal(["100", "[[1], [2]]", "[[9], [2]]", "[[9]]"], run.Output);
        Assert.Contains(run.Steps, step => step.Scope == "Funktion veraendere" && step.Variables["x"] == "5");
        Assert.Contains(run.Steps, step => step.Scope == "Hauptablauf" && step.Variables.TryGetValue("a", out string? list) && list == "[[1], [2]]");
        Assert.Equal("[[1], [2]]", run.Steps.First(step => step.Variables.ContainsKey("a")).Variables["a"]);
    }

    [Fact]
    public void NamedFunctionContractRejectsMainProgramAndAcceptsDifferentParameterNames()
    {
        AlgorithmLesson lesson = _catalog.Lessons[20];
        AlgorithmEvaluator evaluator = new(_interpreter);
        Assert.True(evaluator.Evaluate(lesson, "FUNKTION pruefeGewicht(wert)\nWENN wert > 100 DANN\nRÜCKGABE \"zu schwer\"\nENDE WENN\nWENN wert >= 0 DANN\nRÜCKGABE \"zulässig\"\nSONST\nRÜCKGABE \"ungültig\"\nENDE WENN\nENDE FUNKTION").Correct);
        Assert.False(evaluator.Evaluate(lesson, "AUSGABE \"zulässig\"").Correct);
        Assert.Contains("Funktionsdefinitionen", evaluator.Evaluate(lesson, lesson.ReferenceCode + "\nAUSGABE 1").Message);
        Assert.Contains("nicht definiert", evaluator.Evaluate(lesson, "FUNKTION andererName(x)\nRÜCKGABE x\nENDE FUNKTION").Message);
    }

    [Fact]
    public void FunctionInputsAreDeepCopiedAndEachCallUsesANewState()
    {
        JsonElement argument = JsonSerializer.SerializeToElement(new[] { 1, 2 });
        const string code = "FUNKTION f(liste)\nliste[0] = 9\nRÜCKGABE liste\nENDE FUNKTION";
        for (int i = 0; i < 2; i++)
        {
            AlgorithmRun run = _interpreter.RunFunction(code, "f", [argument], true);
            Assert.Null(run.Error);
            Assert.Equal(["[9, 2]"], run.Output);
            Assert.Equal(1, argument[0].GetInt32());
        }
        Assert.NotNull(_interpreter.RunFunction("FUNKTION f()\nRÜCKGABE liste\nENDE FUNKTION", "f", []).Error);
    }

    [Fact]
    public void AllRepairDraftsFailAtLeastOneIndependentCaseAndTraceAnswersMatchGivenPrograms()
    {
        AlgorithmEvaluator evaluator = new(_interpreter);
        foreach (AlgorithmLesson lesson in _catalog.Lessons)
        {
            Assert.False(string.IsNullOrWhiteSpace(lesson.Briefing));
            Assert.False(string.IsNullOrWhiteSpace(lesson.Explanation));
            if (lesson.Id > 1) Assert.False(string.IsNullOrWhiteSpace(lesson.PreviousContext));
            if (lesson.Kind == AlgorithmTaskKind.Repair) Assert.False(evaluator.Evaluate(lesson, lesson.GivenCode).Correct, $"Aufgabe {lesson.Id}");
            if (lesson.Kind == AlgorithmTaskKind.Trace)
            {
                AlgorithmRun given = _interpreter.Run(lesson.GivenCode, lesson.ExampleInputs);
                Assert.Null(given.Error);
                Assert.Equal(lesson.Tests[0].Output, given.Output);
                Assert.Equal(string.Join("\n", given.Output), lesson.ReferenceAnswer);
            }
        }
    }

    [Fact]
    public void AlternativeSortAndSearchPassWhileTypicalAlmostCorrectProgramsFail()
    {
        AlgorithmEvaluator evaluator = new(_interpreter);
        string selectionSort = "FÜR i VON 0 BIS LÄNGE(werte) - 2\nminimum = i\nFÜR j VON i + 1 BIS LÄNGE(werte) - 1\nWENN werte[j] < werte[minimum] DANN\nminimum = j\nENDE WENN\nENDE FÜR\nhilfe = werte[i]\nwerte[i] = werte[minimum]\nwerte[minimum] = hilfe\nENDE FÜR\nAUSGABE werte";
        Assert.True(evaluator.Evaluate(_catalog.Lessons[16], selectionSort).Correct);
        string linear = "position = -1\nFÜR i VON 0 BIS LÄNGE(nummern) - 1\nWENN nummern[i] == gesucht UND position == -1 DANN\nposition = i\nENDE WENN\nENDE FÜR\nAUSGABE position";
        Assert.True(evaluator.Evaluate(_catalog.Lessons[10], linear).Correct);
        Assert.False(evaluator.Evaluate(_catalog.Lessons[10], linear.Replace(" UND position == -1", "")).Correct);
        Assert.False(evaluator.Evaluate(_catalog.Lessons[8], _catalog.Lessons[8].ReferenceCode.Replace("maximum = abweichungen[0]", "maximum = 0")).Correct);
        Assert.False(evaluator.Evaluate(_catalog.Lessons[17], _catalog.Lessons[17].ReferenceCode.Replace("reihensumme > maximum", "reihensumme >= maximum")).Correct);
        Assert.False(evaluator.Evaluate(_catalog.Lessons[23], _catalog.Lessons[23].ReferenceCode.Replace("gewicht <= rest", "gewicht < rest")).Correct);
        Assert.False(evaluator.Evaluate(_catalog.Lessons[23], _catalog.Lessons[23].ReferenceCode.Replace("gewicht >= 0", "gewicht > 0")).Correct);
    }

    [Fact]
    public void SearchAndSortReferencesMatchIndependentLibraryResultsForVariedLists()
    {
        Random random = new(240611);
        for (int sample = 0; sample < 100; sample++)
        {
            int[] values = Enumerable.Range(0, random.Next(0, 13)).Select(_ => random.Next(-8, 9)).ToArray();
            int wanted = random.Next(-10, 11);
            Dictionary<string, JsonElement> inputs = new() { ["werte"] = JsonSerializer.SerializeToElement(values), ["nummern"] = JsonSerializer.SerializeToElement(values), ["gesucht"] = JsonSerializer.SerializeToElement(wanted) };
            AlgorithmRun sorted = _interpreter.Run(_catalog.Lessons[16].ReferenceCode, inputs);
            Assert.Null(sorted.Error);
            Assert.Equal(["[" + string.Join(", ", values.Order()) + "]"], sorted.Output);
            AlgorithmRun linear = _interpreter.Run(_catalog.Lessons[10].ReferenceCode, inputs);
            Assert.Null(linear.Error);
            Assert.Equal([Array.IndexOf(values, wanted).ToString()], linear.Output);
            int[] unique = values.Distinct().Order().ToArray();
            inputs["nummern"] = JsonSerializer.SerializeToElement(unique);
            AlgorithmRun binary = _interpreter.Run(_catalog.Lessons[13].ReferenceCode, inputs);
            Assert.Null(binary.Error);
            Assert.Equal([Array.IndexOf(unique, wanted).ToString()], binary.Output);
        }
    }

    [Fact]
    public void FunctionParameterLimitsRejectOversizedSignaturesAndCallsBeforeExecution()
    {
        string parameters = string.Join(", ", Enumerable.Range(0, 17).Select(i => $"x{i}"));
        Assert.Contains("16 Parameter", _interpreter.Run($"FUNKTION f({parameters})\nRÜCKGABE 0\nENDE FUNKTION").Error);
        string arguments = string.Join(", ", Enumerable.Repeat("0", 17));
        Assert.Contains("16 Argumente", _interpreter.Run($"AUSGABE f({arguments})").Error);
    }

    [Fact]
    public void NewFeaturesKeepCallDepthValueSizeAndOperationBudgets()
    {
        string calls = string.Join("\n", Enumerable.Range(0, 17).Select(i => $"FUNKTION f{i}()\nRÜCKGABE {(i == 16 ? "1" : $"f{i + 1}() ")}\nENDE FUNKTION")) + "\nAUSGABE f0()";
        Assert.Contains("16 Aufrufebenen", _interpreter.Run(calls).Error);
        Assert.Contains("10.000", _interpreter.Run("FUNKTION f()\nSOLANGE WAHR\nENDE SOLANGE\nRÜCKGABE 1\nENDE FUNKTION\nAUSGABE f()").Error);
        Assert.Contains("zu groß", _interpreter.Run("a = [1]\nSOLANGE WAHR\na[0] = [a, a]\nENDE SOLANGE").Error);
        string trace = "FUNKTION f(x)\nRÜCKGABE x\nENDE FUNKTION\nFÜR i VON 0 BIS 700\nx = f(i)\nENDE FÜR";
        AlgorithmRun run = _interpreter.Run(trace, trace: true);
        Assert.Null(run.Error);
        Assert.Equal(500, run.Steps.Count);
        Assert.True(run.TraceTruncated);
    }
}
