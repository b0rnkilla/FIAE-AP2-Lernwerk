using System.Text.Json;
using FiaeTrainer.Core.Algorithms.Content;
using FiaeTrainer.Core.Algorithms.Evaluation;
using FiaeTrainer.Core.Algorithms.Execution;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft Textgrenzen, alternative Algorithmen, Schleifenabbrüche und die isolierte Tabellenquelle. </summary>
public sealed class AlgorithmTextTests
{
    private readonly AlgorithmInterpreter _interpreter = new();
    private readonly AlgorithmCatalog _catalog = new();

    [Theory]
    [InlineData("AUSGABE LÄNGE('')", "0")]
    [InlineData("AUSGABE 'KLM'[1]", "L")]
    [InlineData("AUSGABE 'R' + ALS_TEXT(27)", "R27")]
    [InlineData("AUSGABE ALS_GANZZAHL('123') + ALS_GANZZAHL('-23')", "100")]
    [InlineData("AUSGABE ALS_GANZZAHL('+004')", "4")]
    [InlineData("AUSGABE ALS_TEXT(WAHR) + ALS_TEXT(2.5)", "WAHR2.5")]
    [InlineData("text = ''\nAUSGABE LÄNGE(text) > 0 UND text[0] == 'X'", "FALSCH")]
    public void TextValuesAreExplicitAndBoundsCanBeShortCircuited(string code, string expected)
    {
        AlgorithmRun run = _interpreter.Run(code, trace: true);
        Assert.Null(run.Error);
        Assert.Equal([expected], run.Output);
    }

    [Theory]
    [InlineData("AUSGABE ''[0]", "Textindex")]
    [InlineData("AUSGABE 'ABC'[3]", "Textindex")]
    [InlineData("AUSGABE 'ABC'[-1]", "Textindex")]
    [InlineData("AUSGABE 'ABC'[0.5]", "ganze Zahl")]
    [InlineData("text = 'ABC'\ntext[0] = 'X'", "Liste")]
    [InlineData("AUSGABE 'A' + 4", "Zahlen")]
    [InlineData("AUSGABE ALS_GANZZAHL('')", "ALS_GANZZAHL")]
    [InlineData("AUSGABE ALS_GANZZAHL('2.5')", "ALS_GANZZAHL")]
    [InlineData("AUSGABE ALS_GANZZAHL(' 12')", "ALS_GANZZAHL")]
    [InlineData("AUSGABE ALS_GANZZAHL('12X')", "ALS_GANZZAHL")]
    [InlineData("AUSGABE ALS_GANZZAHL(12)", "ALS_GANZZAHL")]
    [InlineData("AUSGABE ALS_TEXT([1])", "keine Liste")]
    [InlineData("AUSGABE ALS_TEXT()", "ein Argument")]
    [InlineData("AUSGABE ALS_GANZZAHL('1', '2')", "ein Argument")]
    [InlineData("AUSGABE als_text(2)", "ALS_TEXT")]
    [InlineData("VERLASSE SCHLEIFE", "derselben Funktion")]
    [InlineData("FUNKTION f()\nVERLASSE SCHLEIFE\nENDE FUNKTION\nSOLANGE WAHR\nf()\nENDE SOLANGE", "derselben Funktion")]
    [InlineData("text = 'A'\nSOLANGE WAHR\ntext = text + text\nENDE SOLANGE", "16.000")]
    public void InvalidTextOperationsAndEscapingBreaksFailClearly(string code, string fragment)
    {
        AlgorithmRun run = _interpreter.Run(code, trace: true);
        Assert.Contains(fragment, run.Error);
        Assert.True(run.Steps.Count <= 500);
    }

    [Fact]
    public void BreakExitsOnlyTheInnermostLoopAndReturnStillExitsTheFunction()
    {
        const string code = """
            FUNKTION f(): Integer
                FÜR i VON 1 BIS 2
                    FÜR JEDEN wert IN [4, 5]
                        SOLANGE WAHR
                            AUSGABE i + wert
                            VERLASSE SCHLEIFE
                        ENDE SOLANGE
                        VERLASSE SCHLEIFE
                    ENDE FÜR
                ENDE FÜR
                SOLANGE WAHR
                    RÜCKGABE 9
                ENDE SOLANGE
            ENDE FUNKTION
            AUSGABE f()
            """;
        AlgorithmRun run = _interpreter.Run(code, trace: true);
        Assert.Null(run.Error);
        Assert.Equal(["5", "6", "9"], run.Output);
    }

    [Theory]
    [InlineData("FUNKTION melde(): Void")]
    [InlineData("FUNKTION melde(): KeinRückgabewert")]
    [InlineData("Void melde()")]
    [InlineData("FUNKTION melde()")]
    public void StandaloneCallsSupportVoidAliasesForwardCallsAndEarlyReturn(string header)
    {
        string code = $"{header}\nAUSGABE 'Start'\nRÜCKGABE\nAUSGABE 'falsch'\nENDE FUNKTION\nmelde()";
        AlgorithmRun run = _interpreter.Run(code);
        Assert.Null(run.Error);
        Assert.Equal(["Start"], run.Output);
        AlgorithmRun forward = _interpreter.Run("melde()\nFUNKTION melde(): Void\nAUSGABE 'Start'\nENDE FUNKTION");
        Assert.Null(forward.Error);
        Assert.Equal(["Start"], forward.Output);
    }

    [Theory]
    [InlineData("Void melde()")]
    [InlineData("Integer melde()")]
    [InlineData("List<Integer> melde()")]
    public void StandaloneCallBeforeTypedDefinitionRemainsAnInvocation(string header)
    {
        string result = header.StartsWith("Void") ? "" : header.StartsWith("List") ? "RÜCKGABE [1]\n" : "RÜCKGABE 1\n";
        AlgorithmRun run = _interpreter.Run($"melde()\n{header}\nAUSGABE 'Start'\n{result}ENDE FUNKTION");
        Assert.Null(run.Error);
        Assert.Equal(["Start"], run.Output);
    }

    [Theory]
    [InlineData("FUNKTION f(): Void\nRÜCKGABE 1\nENDE FUNKTION\nf()", "keinen Wert zurückgeben")]
    [InlineData("FUNKTION f(): Void\nENDE FUNKTION\nx = f()", "liefert keinen Wert")]
    [InlineData("FUNKTION f(): Integer\nRÜCKGABE\nENDE FUNKTION\nf()", "Rückgabewert")]
    [InlineData("FUNKTION f(): Integer\nENDE FUNKTION\nf()", "RÜCKGABE")]
    [InlineData("Void x = ''", "nur als Rückgabetyp")]
    [InlineData("FUNKTION f(x: Void)\nENDE FUNKTION", "nur als Rückgabetyp")]
    [InlineData("FUNKTION f(): List<Void>\nENDE FUNKTION", "Werttyp")]
    [InlineData("schreibe_header()", "nicht definiert")]
    public void VoidCannotReplaceAValueOrExposeTaskHelpersElsewhere(string code, string fragment) =>
        Assert.Contains(fragment, _interpreter.Run(code).Error);

    [Fact]
    public void CompressionAcceptsShortCircuitScanningWithoutBreakAndChecksThreshold()
    {
        AlgorithmLesson lesson = _catalog.Lessons[30];
        string alternate = lesson.ReferenceCode.Replace("SOLANGE j < LÄNGE(text)\n            WENN text[j] != zeichen DANN\n                VERLASSE SCHLEIFE\n            ENDE WENN", "SOLANGE j < LÄNGE(text) UND text[j] == zeichen");
        Assert.NotEqual(lesson.ReferenceCode, alternate);
        AlgorithmEvaluator evaluator = new(_interpreter);
        Assert.True(evaluator.Evaluate(lesson, alternate).Correct);
        Assert.False(evaluator.Evaluate(lesson, alternate.Replace("anzahl >= 4", "anzahl > 4")).Correct);
        Assert.False(evaluator.Evaluate(lesson, alternate.Replace("anzahl >= 4", "anzahl >= 3")).Correct);
        Assert.False(evaluator.Evaluate(lesson, alternate.Replace("ALS_TEXT(anzahl)", "ALS_TEXT(anzahl - 1)")).Correct);
    }

    [Fact]
    public void DecompressionAcceptsDigitConditionAndRejectsTheSuppliedDraftTypos()
    {
        AlgorithmLesson lesson = _catalog.Lessons[31];
        string alternate = lesson.ReferenceCode.Replace("SOLANGE i < LÄNGE(text)\n                WENN text[i] < \"0\" ODER text[i] > \"9\" DANN\n                    VERLASSE SCHLEIFE\n                ENDE WENN", "SOLANGE i < LÄNGE(text) UND text[i] >= \"0\" UND text[i] <= \"9\"");
        Assert.NotEqual(lesson.ReferenceCode, alternate);
        AlgorithmEvaluator evaluator = new(_interpreter);
        Assert.True(evaluator.Evaluate(lesson, alternate).Correct);
        Assert.False(evaluator.Evaluate(lesson, lesson.ReferenceCode.Replace("zeichen = text[i + 1]", "zeichen = i + 1")).Correct);
        Assert.False(evaluator.Evaluate(lesson, lesson.ReferenceCode.Replace("ziffern = ziffern +", "ziffern = ziffer +")).Correct);
    }

    [Fact]
    public void TextTransformationsRejectAddedWhitespaceAndExplainTheEmptyResult()
    {
        AlgorithmEvaluator evaluator = new(_interpreter);
        foreach (AlgorithmLesson lesson in _catalog.Lessons.Where(lesson => lesson.Id is 31 or 32))
        {
            Assert.False(evaluator.Evaluate(lesson, lesson.ReferenceCode.Replace("RÜCKGABE ergebnis", "RÜCKGABE ergebnis + ' '")).Correct);
            Assert.False(evaluator.Evaluate(lesson, lesson.ReferenceCode.Replace("RÜCKGABE ergebnis", "RÜCKGABE ' ' + ergebnis")).Correct);
            var evaluation = evaluator.Evaluate(lesson, lesson.ReferenceCode);
            Assert.True(evaluation.Correct);
            Assert.Contains(evaluation.Checks, check => check.Expected == "\"\"" && check.Actual == "\"\"" && check.Passed);
        }
    }

    [Fact]
    public void ThreeDigitRunsUseTheirAnnouncedBudgetAndPreviewStillStopsAtFiveHundred()
    {
        AlgorithmLesson compress = _catalog.Lessons[30], decompress = _catalog.Lessons[31];
        var original = new Dictionary<string, JsonElement> { ["text"] = JsonSerializer.SerializeToElement(new string('Z', 999)) };
        var packed = new Dictionary<string, JsonElement> { ["text"] = JsonSerializer.SerializeToElement("%Z%999") };
        AlgorithmRun encoded = _interpreter.RunFunction(compress.ReferenceCode, compress, original, true);
        AlgorithmRun decoded = _interpreter.RunFunction(decompress.ReferenceCode, decompress, packed, true);
        Assert.Null(encoded.Error);
        Assert.Null(decoded.Error);
        Assert.Equal(["%Z%999"], encoded.Output);
        Assert.Equal([new string('Z', 999)], decoded.Output);
        Assert.True(encoded.TraceTruncated);
        Assert.True(encoded.Steps.Count <= 500);
        Assert.Contains("100.000", _interpreter.RunFunction("FUNKTION komprimiere(text)\nSOLANGE WAHR\nENDE SOLANGE\nENDE FUNKTION", compress, original).Error);
        Assert.Contains("10.000", _interpreter.Run("SOLANGE WAHR\nENDE SOLANGE").Error);
    }

    [Fact]
    public void TableSourceSupportsAlternativeLoopAndRestartsForEachRun()
    {
        AlgorithmLesson lesson = _catalog.Lessons[32];
        const string code = """
            FUNKTION output()
                schreibe_header()
                SOLANGE WAHR
                    zeile = hole_daten()
                    WENN zeile == "" DANN
                        VERLASSE SCHLEIFE
                    ENDE WENN
                    schreibe_datensatz(zeile)
                ENDE SOLANGE
                schreibe_footer()
            ENDE FUNKTION
            """;
        Assert.True(new AlgorithmEvaluator(_interpreter).Evaluate(lesson, code).Correct);
        foreach (AlgorithmTest test in lesson.Tests.Reverse())
        {
            AlgorithmRun run = _interpreter.RunFunction(code, lesson, test.Inputs, true);
            Assert.Null(run.Error);
            Assert.Equal(test.Output, run.Output);
        }
        AlgorithmRun first = _interpreter.RunFunction(lesson.ReferenceCode, lesson, lesson.ExampleInputs, true);
        AlgorithmRun second = _interpreter.RunFunction(lesson.ReferenceCode, lesson, lesson.ExampleInputs, true);
        Assert.Equal(first.Output, second.Output);
        Assert.DoesNotContain(first.Steps, step => step.Variables.ContainsKey("datensaetze"));
        Assert.Empty(first.Steps[0].Output);
        Assert.Contains(first.Steps, step => step.Output.Count == 1);
        Assert.DoesNotContain("", first.Output);
    }

    [Theory]
    [InlineData("schreibe_header()", "")]
    [InlineData("schreibe_footer()", "")]
    [InlineData("schreibe_header()", "schreibe_footer()")]
    [InlineData("schreibe_datensatz(daten)", "schreibe_datensatz('falsch')")]
    [InlineData("schreibe_datensatz(daten)", "schreibe_datensatz(daten)\n        schreibe_datensatz(daten)")]
    [InlineData("FÜR daten = hole_daten();", "FÜR daten = '';")]
    [InlineData("; daten = hole_daten()", "; daten = daten")]
    [InlineData("schreibe_footer()", "schreibe_datensatz('')\n    schreibe_footer()")]
    [InlineData("schreibe_header()", "AUSGABE 'Kundennummer | Name | Ort'")]
    [InlineData("schreibe_header()", "schreibe_header(1)")]
    [InlineData("schreibe_footer()", "schreibe_footer()\n    RÜCKGABE ''")]
    public void TableContractRejectsMissingSkippedRepeatedAndFabricatedOutput(string before, string after)
    {
        AlgorithmLesson lesson = _catalog.Lessons[32];
        string answer = lesson.ReferenceCode.Replace(before, after);
        Assert.NotEqual(lesson.ReferenceCode, answer);
        Assert.False(new AlgorithmEvaluator(_interpreter).Evaluate(lesson, answer).Correct);
    }

    [Fact]
    public void TableContextCannotBeReplacedOrReadAsAGlobalVariable()
    {
        AlgorithmLesson lesson = _catalog.Lessons[32];
        string replacement = lesson.ReferenceCode + "\nFUNKTION hole_daten()\nRÜCKGABE ''\nENDE FUNKTION";
        Assert.Contains("bereits vorgegeben", _interpreter.RunFunction(replacement, lesson, lesson.ExampleInputs).Error);
        Assert.Contains("noch keinen Wert", _interpreter.RunFunction("FUNKTION output()\nx = datensaetze\nENDE FUNKTION", lesson, lesson.ExampleInputs).Error);
    }
}
