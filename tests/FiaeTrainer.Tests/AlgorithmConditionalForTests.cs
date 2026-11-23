using FiaeTrainer.Core.Algorithms.Content;
using FiaeTrainer.Core.Algorithms.Evaluation;
using FiaeTrainer.Core.Algorithms.Execution;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft die drei Phasen der bedingten FÜR-Schleife und beide Lösungen des Tabellenauftrags. </summary>
public sealed class AlgorithmConditionalForTests
{
    private readonly AlgorithmInterpreter _interpreter = new();

    [Theory]
    [InlineData("FÜR", "; ")]
    [InlineData("FUER", "; ")]
    [InlineData("FÜR", "\n")]
    [InlineData("FUER", "\n")]
    public void HeaderRunsInOrderAndAllowsLineBreaks(string keyword, string separator)
    {
        string code = $"{keyword} rest = 7{separator}rest > 0{separator}rest = rest - 3\nAUSGABE rest\nENDE {keyword}\nAUSGABE rest";
        AlgorithmRun run = _interpreter.Run(code, trace: true);
        Assert.Null(run.Error);
        Assert.Equal(["7", "4", "1", "-2"], run.Output);
        Assert.Equal(4, run.Steps.Count(step => step.Action.StartsWith("FÜR-Bedingung:")));
    }

    [Theory]
    [InlineData("=")]
    [InlineData(":=")]
    [InlineData("←")]
    [InlineData("<-")]
    public void ExistingAssignmentFormsRemainValidInBothHeaderAssignments(string assignment)
    {
        AlgorithmRun run = _interpreter.Run($"FÜR i {assignment} 1; i <= 2; i {assignment} i + 1 TUE\nAUSGABE i\nENDE FÜR");
        Assert.Null(run.Error);
        Assert.Equal(["1", "2"], run.Output);
    }

    [Theory]
    [InlineData("alle")]
    [InlineData("jede")]
    [InlineData("jeden")]
    public void QuantifiersRemainUsableAsNamesOutsideEachLoops(string name)
    {
        AlgorithmRun run = _interpreter.Run($"FÜR {name} = 0; {name} < 2; {name} = {name} + 1\nAUSGABE {name}\nENDE FÜR");
        Assert.Null(run.Error);
        Assert.Equal(["0", "1"], run.Output);
    }

    [Fact]
    public void FalseInitialConditionSkipsBodyAndAdvanceButNotInitialization()
    {
        AlgorithmRun run = _interpreter.Run("FÜR i = 5; i < 0; i = 1 / 0\nAUSGABE 'falsch'\nENDE FÜR\nAUSGABE i");
        Assert.Null(run.Error);
        Assert.Equal(["5"], run.Output);
    }

    [Fact]
    public void BreakSkipsAdvanceAndExitsOnlyItsOwnLoop()
    {
        const string code = """
            FÜR a VON 1 BIS 2
                FÜR i = 1; WAHR; i = 1 / 0
                    AUSGABE a
                    VERLASSE SCHLEIFE
                ENDE FÜR
            ENDE FÜR
            AUSGABE i
            """;
        AlgorithmRun run = _interpreter.Run(code);
        Assert.Null(run.Error);
        Assert.Equal(["1", "2", "1"], run.Output);
    }

    [Fact]
    public void NestedBreakAndFunctionReturnKeepTheirOriginalTargets()
    {
        const string code = """
            FUNKTION probe(): Integer
                FÜR i = 1; i < 4; i = i + 1
                    FÜR JEDEN wert IN [8, 9]
                        VERLASSE SCHLEIFE
                    ENDE FÜR
                    WENN i == 2 DANN
                        RÜCKGABE i
                    ENDE WENN
                    AUSGABE i
                ENDE FÜR
                RÜCKGABE 99
            ENDE FUNKTION
            AUSGABE probe()
            """;
        AlgorithmRun run = _interpreter.Run(code);
        Assert.Null(run.Error);
        Assert.Equal(["1", "2"], run.Output);
        Assert.Null(_interpreter.Run("FÜR i = 1; WAHR; i = 1 / 0\nFÜR j = 1; FALSCH; j = j + 1\nENDE FÜR\nVERLASSE SCHLEIFE\nENDE FÜR").Error);
    }

    [Fact]
    public void MultiLineHeaderPreservesLineNumbersAndSnapshots()
    {
        const string code = "FÜR i = 1\ni <= 1\ni = i + 1\nAUSGABE i\nENDE FÜR";
        AlgorithmRun run = _interpreter.Run(code, trace: true);
        Assert.Null(run.Error);
        Assert.Contains(run.Steps, step => step.Line == 1 && step.Variables["i"] == "1");
        Assert.Contains(run.Steps, step => step.Line == 2 && step.Action == "FÜR-Bedingung: WAHR");
        Assert.Contains(run.Steps, step => step.Line == 3 && step.Variables["i"] == "2");
        Assert.Contains("Zeile 3:", _interpreter.Run(code.Replace("i = i + 1", "i = 1 / 0")).Error);
        Assert.Contains("Zeile 2:", _interpreter.Run(code.Replace("i <= 1", "'kein Wahrheitswert'")).Error);
    }

    [Theory]
    [InlineData("FÜR i = 0; WAHR; i = i\nENDE FÜR", "10.000")]
    [InlineData("FÜR i = 0; WAHR; AUSGABE i\nENDE FÜR", "dritte Teil")]
    [InlineData("FÜR i = 0; 1; i = i + 1\nENDE FÜR", "Wahrheitswert")]
    [InlineData("FÜR i = 0; i < 1; i = i + 1\nAUSGABE i\nENDE SOLANGE", "FUER")]
    public void InvalidHeadersAndNonProgressingLoopsReturnUsefulErrors(string code, string fragment) =>
        Assert.Contains(fragment, _interpreter.Run(code).Error);

    [Fact]
    public void CountedEachAndConditionalLoopsCanBeCombined()
    {
        const string code = "FÜR a VON 1 BIS 2\nFÜR JEDEN b IN [4]\nFÜR i = 0; i < 2; i = i + 1\nAUSGABE a + b + i\nENDE FÜR\nENDE FÜR\nENDE FÜR";
        AlgorithmRun run = _interpreter.Run(code);
        Assert.Null(run.Error);
        Assert.Equal(["5", "6", "6", "7"], run.Output);
    }

    [Fact]
    public void TableReferenceAndWhileAlternativePassEveryDataset()
    {
        AlgorithmLesson lesson = new AlgorithmCatalog().Lessons[32];
        const string original = """
            FUNKTION output()
                schreibe_header()
                daten = hole_daten()
                SOLANGE daten != ""
                    schreibe_datensatz(daten)
                    daten = hole_daten()
                ENDE SOLANGE
                schreibe_footer()
            ENDE FUNKTION
            """;
        string multiline = lesson.ReferenceCode.Replace("; ", "\n");
        AlgorithmEvaluator evaluator = new(_interpreter);
        Assert.Contains("FÜR daten = hole_daten(); daten != \"\"; daten = hole_daten()", lesson.ReferenceCode);
        foreach (string code in new[] { lesson.ReferenceCode, multiline, original })
        {
            Assert.True(evaluator.Evaluate(lesson, code).Correct);
            foreach (AlgorithmTest test in lesson.Tests)
            {
                AlgorithmRun run = _interpreter.RunFunction(code, lesson, test.Inputs, true);
                Assert.Null(run.Error);
                Assert.Equal(test.Output, run.Output);
            }
        }
    }

    [Fact]
    public void CorrectedConversionSpellingWorksInReferenceAndCustomCode()
    {
        AlgorithmRun run = _interpreter.Run("AUSGABE ALS_GANZZAHL('123') + 1");
        Assert.Null(run.Error);
        Assert.Equal(["124"], run.Output);
        AlgorithmLesson lesson = new AlgorithmCatalog().Lessons[31];
        Assert.Contains("ALS_GANZZAHL(ziffern)", lesson.ReferenceCode);
        Assert.True(new AlgorithmEvaluator(_interpreter).Evaluate(lesson, lesson.ReferenceCode).Correct);
    }
}
