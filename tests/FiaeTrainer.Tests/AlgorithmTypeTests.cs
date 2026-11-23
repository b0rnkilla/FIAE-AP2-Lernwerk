using System.Text.Json;
using FiaeTrainer.Core.Algorithms.Content;
using FiaeTrainer.Core.Algorithms.Evaluation;
using FiaeTrainer.Core.Algorithms.Exams;
using FiaeTrainer.Core.Algorithms.Execution;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Typisierte Köpfe, lokale Deklarationen und die Verträge neuer und früherer Aufgaben. </summary>
public sealed class AlgorithmTypeTests
{
    private readonly AlgorithmInterpreter _interpreter = new();

    [Theory]
    [InlineData("FUNKTION probe(werte: Integer[]): Boolean")]
    [InlineData("FUNKTION probe(werte: Liste<Ganzzahl>): bool")]
    [InlineData("Wahrheitswert probe(List<int> anders)")]
    [InlineData("probe(INT[] anders): boolean")]
    [InlineData("FUNKTION Boolean probe(lIsTe<iNtEgEr> anders)")]
    public void EquivalentHeadersUseTheSameListAndBooleanContract(string header)
    {
        string name = header.Contains("anders") ? "anders" : "werte";
        string code = $"{header}\nRÜCKGABE LÄNGE({name}) == 2\nENDE FUNKTION";
        AlgorithmRun run = _interpreter.RunFunction(code, "probe", [JsonSerializer.SerializeToElement(new[] { 6, 6 })], true);
        Assert.Null(run.Error);
        Assert.Equal(["WAHR"], run.Output);
    }

    [Theory]
    [InlineData("Double x = 2\nkommazahl y = 3.5\nz: DEZIMALZAHL = x + y\nAUSGABE z", "5.5")]
    [InlineData("String a = 'grün'\nText b = a\nc: Zeichenkette = b\nAUSGABE c", "grün")]
    [InlineData("werte: Liste<List<int>>=[[4], []]\nwerte[0][0] = 7\nAUSGABE werte", "[[7], []]")]
    [InlineData("FUNKTION liste(int)\ntext = int\nRÜCKGABE text\nENDE FUNKTION\nAUSGABE liste(8)", "8")]
    [InlineData("Boolean x = WAHR\nFUNKTION f(int x): int\nRÜCKGABE x + 1\nENDE FUNKTION\nAUSGABE f(4)\nAUSGABE x", "5|WAHR")]
    public void LocalTypesAndLegacyNamesRemainUsable(string code, string expected)
    {
        AlgorithmRun run = _interpreter.Run(code, trace: true);
        Assert.Null(run.Error);
        Assert.Equal(expected.Split('|'), run.Output);
    }

    [Theory]
    [InlineData("Integer x = 0\nx = 2.5", 2, "Integer")]
    [InlineData("texte: List<Text> = []\ntexte = [1]", 2, "String[]")]
    [InlineData("Integer[][] x = [[1], []]\nx[0][0] = FALSCH", 2, "Integer[][]")]
    [InlineData("Integer i = 0\nFÜR JEDE i IN ['abc']\nENDE FÜR", 2, "Integer")]
    [InlineData("Text i = 'abc'\nFÜR i VON 0 BIS 1\nENDE FÜR", 2, "String")]
    [InlineData("FUNKTION f(x: bool): Boolean\nRÜCKGABE 'WAHR'\nENDE FUNKTION\nAUSGABE f(WAHR)", 2, "ein Text")]
    [InlineData("FUNKTION f(x: Integer)\nRÜCKGABE x\nENDE FUNKTION\nAUSGABE f(1.5)", 1, "Integer")]
    [InlineData("x: Iteger = 3", 1, "Datentyp")]
    [InlineData("Integer x: int = 3", 1, "nur einmal")]
    [InlineData("x: Liste<> = []", 1, "Elementtyp")]
    [InlineData("x: List<int = []", 1, "„>“")]
    [InlineData("int x = 1\nText x = 'a'", 2, "bereits als Integer")]
    public void TypeErrorsIdentifyTheOriginalLine(string code, int line, string message)
    {
        string? error = _interpreter.Run(code).Error;
        Assert.Contains($"Zeile {line}:", error);
        Assert.Contains(message, error);
    }

    [Fact]
    public void TypeNestingIsBoundedBeforeExecution()
    {
        string type = string.Concat(Enumerable.Repeat("List<", 17)) + "int" + new string('>', 17);
        Assert.Contains("zu tief", _interpreter.Run($"x: {type} = []").Error);
        Assert.Contains("zu tief", _interpreter.Run("x: int" + string.Concat(Enumerable.Repeat("[]", 17)) + " = []").Error);
    }

    [Fact]
    public void DeclaredSignatureIsCheckedEvenForEmptyInputAndUnusedParameters()
    {
        var lesson = new AlgorithmLesson { FunctionName = "f", FunctionInputs = ["a"], ParameterTypes = ["Integer[]"], ReturnType = "Integer" };
        var inputs = new Dictionary<string, JsonElement> { ["a"] = JsonSerializer.SerializeToElement(Array.Empty<int>()) };
        const string body = "\nRÜCKGABE 0\nENDE FUNKTION";
        Assert.Contains("Integer[]", _interpreter.RunFunction("FUNKTION f(text: String[])" + body, lesson, inputs).Error);
        Assert.Contains("Rückgabetyp", _interpreter.RunFunction("Double f(int[] x)" + body, lesson, inputs).Error);
        Assert.Contains("Rückgabetyp", _interpreter.RunFunction("FUNKTION f(int[] x)" + body, lesson, inputs, requireTypes: true).Error);
        Assert.Contains("Parameter", _interpreter.RunFunction("FUNKTION f(x): int" + body, lesson, inputs, requireTypes: true).Error);
        Assert.Null(_interpreter.RunFunction("FUNKTION f(x)" + body, lesson, inputs).Error);
        Assert.Contains("ein Text", _interpreter.RunFunction("FUNKTION f(x)\nRÜCKGABE '0'\nENDE FUNKTION", lesson, inputs).Error);
    }

    [Fact]
    public void FormerLearningAnswersAndExamRoundsKeepTheirContract()
    {
        AlgorithmEvaluator learning = new(_interpreter);
        var lessons = new AlgorithmCatalog().Lessons.Where(x => x.FunctionName.Length > 0).ToList();
        Assert.Equal(12, lessons.Count);
        foreach (AlgorithmLesson lesson in lessons)
        {
            string oldHeader = $"FUNKTION {lesson.FunctionName}({string.Join(", ", lesson.FunctionInputs)})";
            Assert.Contains(lesson.Signature, lesson.ReferenceCode);
            Assert.True(learning.Evaluate(lesson, lesson.ReferenceCode.Replace(lesson.Signature, oldHeader)).Correct);
        }
        AlgorithmExamCatalog exams = new();
        string current = exams.CreateRound().ContentVersion;
        AlgorithmExamEvaluator grading = new(_interpreter);
        foreach (var task in exams.Tasks.Where(x => x.Lesson.FunctionName.Length > 0))
        {
            var lesson = task.Lesson;
            string old = lesson.ReferenceCode.Replace(lesson.Signature, $"FUNKTION {lesson.FunctionName}({string.Join(", ", lesson.FunctionInputs)})");
            Assert.NotEqual(old, lesson.ReferenceCode);
            Assert.Equal(task.Points, grading.Evaluate(task, old, "2026-11-05").Earned);
            Assert.Equal(task.Points, grading.Evaluate(task, lesson.ReferenceCode, current).Earned);
            Assert.Equal(task.RequiresTypes(current) ? 0 : task.Points, grading.Evaluate(task, old, current).Earned);
        }
    }
}
