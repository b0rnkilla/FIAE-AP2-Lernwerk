using System.Text.Json;
using FiaeTrainer.Core.Algorithms.Content;
using FiaeTrainer.Core.Algorithms.Evaluation;
using FiaeTrainer.Core.Algorithms.Execution;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft die Abschlussaufgaben mit unabhängigen Erwartungen sowie Rekursionsgrenzen und lokalen Zuständen. </summary>
public sealed class AlgorithmCompletionTests
{
    /// <summary> Vollständig isolierte Zustände je Ausführung. </summary>
    private readonly AlgorithmInterpreter _interpreter = new();
    /// <summary> Aufgaben mit stabilen bisherigen IDs. </summary>
    private readonly AlgorithmCatalog _catalog = new();

    [Fact]
    public void RecursiveTraceUnwindsWithSeparateLocalsAndCorrectDepth()
    {
        AlgorithmRun run = _interpreter.Run(_catalog.Lessons[27].GivenCode, trace: true);
        Assert.Null(run.Error);
        Assert.Equal(["1", "3", "6", "6"], run.Output);
        AlgorithmStep[] returns = run.Steps.Where(step => step.Action.StartsWith("Rückgabe:", StringComparison.Ordinal)).ToArray();
        Assert.Equal([4, 3, 2, 1], returns.Select(step => step.CallDepth));
        Assert.Equal(["0", "1", "2", "3"], returns.Select(step => step.Variables["n"]));
        Assert.Equal(["Rückgabe: 0", "Rückgabe: 1", "Rückgabe: 3", "Rückgabe: 6"], returns.Select(step => step.Action));
        Assert.Equal("Hauptablauf", run.Steps[^1].Scope);
        Assert.Equal(0, run.Steps[^1].CallDepth);
        Assert.Empty(run.Steps[^1].Variables);
    }

    [Fact]
    public void ExactDepthBoundaryAndIndirectRecursionRemainBounded()
    {
        const string sum = "FUNKTION summe(n)\nWENN n == 0 DANN\nRÜCKGABE 0\nENDE WENN\nRÜCKGABE n + summe(n - 1)\nENDE FUNKTION";
        AlgorithmRun allowed = _interpreter.RunFunction(sum, "summe", [JsonSerializer.SerializeToElement(15)], true);
        Assert.Null(allowed.Error);
        Assert.Equal(["120"], allowed.Output);
        Assert.Equal(16, allowed.Steps.Max(step => step.CallDepth));
        Assert.Contains("16 Aufrufebenen", _interpreter.RunFunction(sum, "summe", [JsonSerializer.SerializeToElement(16)]).Error);
        const string mutual = "FUNKTION gerade(n)\nWENN n == 0 DANN\nRÜCKGABE WAHR\nENDE WENN\nRÜCKGABE ungerade(n - 1)\nENDE FUNKTION\nFUNKTION ungerade(n)\nWENN n == 0 DANN\nRÜCKGABE FALSCH\nENDE WENN\nRÜCKGABE gerade(n - 1)\nENDE FUNKTION\nAUSGABE gerade(6)\nAUSGABE gerade(7)";
        AlgorithmRun result = _interpreter.Run(mutual);
        Assert.Null(result.Error);
        Assert.Equal(["WAHR", "FALSCH"], result.Output);
    }

    [Fact]
    public void BranchingRecursionSharesTheGlobalOperationBudget()
    {
        const string code = "FUNKTION baum(n)\nWENN n == 0 DANN\nRÜCKGABE 1\nENDE WENN\nRÜCKGABE baum(n - 1) + baum(n - 1)\nENDE FUNKTION\nAUSGABE baum(14)";
        AlgorithmRun run = _interpreter.Run(code, trace: true);
        Assert.Contains("10.000", run.Error);
        Assert.True(run.TraceTruncated);
        Assert.True(run.Steps.Count <= 500);
        Assert.True(run.Steps.All(step => step.CallDepth <= 16));
        Assert.Null(_interpreter.Run("AUSGABE 42").Error);
    }

    [Fact]
    public void QuantityCasesUseTheirOwnInputAndRequireBothSidesOfEveryBoundary()
    {
        AlgorithmLesson lesson = _catalog.Lessons[26];
        AlgorithmEvaluator evaluator = new(_interpreter);
        Assert.True(evaluator.Evaluate(lesson, "-20 => ungültig\n0 => 0\n9 => 0\n10 => 5\n49 => 5\n50 => 10").Correct);
        AlgorithmEvaluation wrong = evaluator.Evaluate(lesson, "-1 => ungültig\n0 => 0\n9 => 0\n10 => 0\n49 => 5\n50 => 10");
        Assert.False(wrong.Correct);
        Assert.Contains(wrong.Checks, check => check.Name == "Erste Staffel: 10" && !check.Passed);
        AlgorithmEvaluation missing = evaluator.Evaluate(lesson, "-1 => ungültig\n0 => 0\n9 => 0\n10 => 5\n49 => 5\n100 => 10");
        Assert.Contains(missing.Checks, check => check.Name == "Zweite Staffel: 50" && !check.Passed);
        Assert.Contains("anzahl", evaluator.Evaluate(lesson, "zehn => 5").Message);
        Assert.All(evaluator.Evaluate(lesson, lesson.ReferenceAnswer).Checks.Take(6), check => Assert.StartsWith("anzahl = ", check.Inputs));
    }

    [Fact]
    public void PairCountingMatchesIndependentFrequencyCombinations()
    {
        Random random = new(27030);
        AlgorithmLesson lesson = _catalog.Lessons[29];
        for (int sample = 0; sample < 40; sample++)
        {
            int[] values = Enumerable.Range(0, random.Next(0, 21)).Select(_ => random.Next(-3, 4)).ToArray();
            int expected = values.GroupBy(x => x).Sum(group => group.Count() * (group.Count() - 1) / 2);
            AlgorithmRun run = _interpreter.RunFunction(lesson.ReferenceCode, lesson.FunctionName, [JsonSerializer.SerializeToElement(values)]);
            Assert.Null(run.Error);
            Assert.Equal([expected.ToString()], run.Output);
        }
    }

    [Fact]
    public void IterativeAlternativePassesAndUnchangedRecursiveArgumentFails()
    {
        AlgorithmLesson lesson = _catalog.Lessons[28];
        AlgorithmEvaluator evaluator = new(_interpreter);
        const string iterative = "FUNKTION fakultaet(wert)\nergebnis = 1\nFÜR i VON 2 BIS wert\nergebnis = ergebnis * i\nENDE FÜR\nRÜCKGABE ergebnis\nENDE FUNKTION";
        Assert.True(evaluator.Evaluate(lesson, iterative).Correct);
        AlgorithmEvaluation wrong = evaluator.Evaluate(lesson, lesson.GivenCode);
        Assert.False(wrong.Correct);
        Assert.Contains("Basisfall", wrong.Message);
    }
}
