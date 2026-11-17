using System.Text.Json;
using FiaeTrainer.Core.Algorithms.Content;
using FiaeTrainer.Core.Algorithms.Evaluation;
using FiaeTrainer.Core.Algorithms.Execution;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft fachliche Sollwerte, alternative Lösungen, hilfreiche Fehler und Ausführungsgrenzen. </summary>
public sealed class AlgorithmTests
{
    private readonly AlgorithmInterpreter _interpreter = new();
    private readonly AlgorithmCatalog _catalog = new();

    [Fact]
    public void ExampleInputsUseGermanBooleanValues()
    {
        JsonElement values = JsonSerializer.SerializeToElement(new object[] { true, false, 12 });
        Assert.Equal("[WAHR, FALSCH, 12]", AlgorithmDisplay.Input(values));
    }



    [Fact]
    public void AllReferenceProgramsMatchIndependentlySpecifiedOutputs()
    {
        Assert.Equal(30, _catalog.Lessons.Count);
        foreach (AlgorithmLesson lesson in _catalog.Lessons)
        {
            Assert.Equal(3, lesson.Hints.Length);
            Assert.False(string.IsNullOrWhiteSpace(lesson.Story));
            Assert.False(string.IsNullOrWhiteSpace(lesson.Discovery));
            foreach (AlgorithmTest test in lesson.Tests)
            {
                AlgorithmRun run = lesson.FunctionName.Length == 0
                    ? _interpreter.Run(lesson.ReferenceCode, test.Inputs, true)
                    : _interpreter.RunFunction(lesson.ReferenceCode, lesson.FunctionName, lesson.FunctionInputs.Select(name => test.Inputs[name]).ToArray(), true);
                Assert.True(run.Error == null, $"Aufgabe {lesson.Id}, Fall {test.Name}: {run.Error}");
                Assert.Equal(test.Output, run.Output);
            }
            string answer = lesson.Kind is AlgorithmTaskKind.Trace or AlgorithmTaskKind.TestCases ? lesson.ReferenceAnswer : lesson.ReferenceCode;
            Assert.True(new AlgorithmEvaluator(_interpreter).Evaluate(lesson, answer).Correct, $"Aufgabe {lesson.Id}");
        }
    }

    [Theory]
    [InlineData("AUSGABE 2 + 3 * 4", "14")]
    [InlineData("AUSGABE (2 + 3) * 4", "20")]
    [InlineData("AUSGABE -7 DIV 2", "-3")]
    [InlineData("AUSGABE 7 MOD 2", "1")]
    [InlineData("AUSGABE NICHT 2 = 3", "WAHR")]
    [InlineData("AUSGABE WAHR ODER FALSCH UND FALSCH", "WAHR")]
    [InlineData("AUSGABE FALSCH UND 1 / 0 = 0", "FALSCH")]
    [InlineData("AUSGABE WAHR ODER unbekannt = 0", "WAHR")]
    [InlineData("AUSGABE 'WENN UND SONST'", "WENN UND SONST")]
    [InlineData("x := 5; AUSGABE X", "5")]
    [InlineData("x <- 5\nAUSGABE x", "5")]
    [InlineData("x = 5\nAUSGABE x", "5")]
    [InlineData("x = 5\nWENN x == 5 DANN\nx = x + 1\nENDE WENN\nAUSGABE x", "6")]
    [InlineData("AUSGABE 2.5 * 4 // Kommentar", "10")]
    [InlineData("x ← [10, 20]\nAUSGABE x[0]\nAUSGABE LAENGE(x)", "10|2")]
    [InlineData("FÜR i VON 1 BIS 0\nAUSGABE i\nENDE FÜR\nAUSGABE 0", "0")]
    [InlineData("i ← 0\nSOLANGE i < 2 TUE\ni ← i + 1\nENDE SOLANGE\nAUSGABE i", "2")]
    public void SyntaxAndExecutionFollowDocumentedConventions(string code, string expected)
    {
        AlgorithmRun run = _interpreter.Run(code);
        Assert.Null(run.Error);
        Assert.Equal(expected.Split('|'), run.Output);
    }

    [Theory]
    [InlineData("FÜR", "JEDEN")]
    [InlineData("FUER", "JEDEN")]
    [InlineData("FÜR", "JEDE")]
    [InlineData("FUER", "JEDE")]
    [InlineData("FÜR", "JEDES")]
    [InlineData("FUER", "JEDES")]
    [InlineData("FÜR", "ALLE")]
    [InlineData("FUER", "ALLE")]
    public void EachLoopVisitsNestedListsInOrderAndCopiesTheirItems(string keyword, string quantifier)
    {
        string code = $"""
            reihen = [[], [4, 2, 4]]
            {keyword} {quantifier} reihe IN reihen
                FUER JEDES element IN reihe
                    AUSGABE element
                    reihe[0] = 99
                ENDE FUER
            ENDE {keyword}
            AUSGABE reihen
            """;
        AlgorithmRun run = _interpreter.Run(code, trace: true);
        Assert.Null(run.Error);
        Assert.Equal(["4", "2", "4", "[[], [4, 2, 4]]"], run.Output);
    }

    [Theory]
    [InlineData("jeden", "JEDEN")]
    [InlineData("Jede", "JEDE")]
    [InlineData("alle", "ALLE")]
    public void NewLoopWordsRequireUppercaseOnlyAsQuantifiers(string name, string keyword)
    {
        string code = $"// {name}\nFÜR {name} eintrag IN [1]\nAUSGABE eintrag\nENDE FÜR";
        Assert.Contains($"Zeile 2: Schreibe Schlüsselwörter groß: „{name}“ muss „{keyword}“ heißen", _interpreter.Run(code).Error);

        string compatible = $"FUNKTION probe({name})\nFÜR {name} VON 2 BIS 3\nAUSGABE {name}\nENDE FÜR\nRÜCKGABE {name}\nENDE FUNKTION\n{keyword} = 5\nAUSGABE probe({name})\nAUSGABE '{name}'";
        AlgorithmRun run = _interpreter.Run(compatible);
        Assert.Null(run.Error);
        Assert.Equal(["2", "3", "3", name], run.Output);
    }

    [Theory]
    [InlineData("FÜR JEDES")]
    [InlineData("FÜR JEDEN")]
    [InlineData("FUER JEDE")]
    [InlineData("FUER ALLE")]
    public void MaximumLessonAcceptsEachSpellingAndRejectsInvalidLoopHeaders(string header)
    {
        AlgorithmLesson lesson = _catalog.Lessons.Single(x => x.Id == 9);
        AlgorithmEvaluator evaluator = new(_interpreter);
        string answer = lesson.ReferenceCode.Replace("FÜR JEDEN wert", $"{header} wert");
        Assert.Contains($"{header} wert IN abweichungen", answer);
        Assert.True(evaluator.Evaluate(lesson, answer).Correct);
        Assert.Contains("„IN“", evaluator.Evaluate(lesson, answer.Replace("wert IN abweichungen", "wert abweichungen")).Message);
        Assert.Contains("Liste", evaluator.Evaluate(lesson, answer.Replace("IN abweichungen", "IN 42")).Message);
    }

    [Theory]
    [InlineData("WENN")]
    [InlineData("DANN")]
    [InlineData("SONST")]
    [InlineData("ENDE")]
    [InlineData("SOLANGE")]
    [InlineData("FÜR")]
    [InlineData("FUER")]
    [InlineData("JEDES")]
    [InlineData("IN")]
    [InlineData("VON")]
    [InlineData("BIS")]
    [InlineData("TUE")]
    [InlineData("AUSGABE")]
    [InlineData("WAHR")]
    [InlineData("FALSCH")]
    [InlineData("NICHT")]
    [InlineData("UND")]
    [InlineData("ODER")]
    [InlineData("DIV")]
    [InlineData("MOD")]
    [InlineData("LÄNGE")]
    [InlineData("LAENGE")]
    [InlineData("FUNKTION")]
    [InlineData("RÜCKGABE")]
    [InlineData("RUECKGABE")]
    public void LowercaseKeywordsProduceLineAndExactCorrection(string keyword)
    {
        string word = keyword.ToLowerInvariant();
        AlgorithmRun run = _interpreter.Run("// Kommentare enthalten wenn und sonst\n\nx = 1\n" + word);
        Assert.NotNull(run.Error);
        Assert.Contains("Zeile 4:", run.Error);
        Assert.Contains($"„{word}“ muss „{keyword}“ heißen", run.Error);
    }

    [Fact]
    public void KeywordRuleAppliesToPreviewAndEvaluationWithoutChangingTextsOrVariables()
    {
        string code = "Rest = kapazitaet - beladen\nAusgabe rest";
        Assert.Contains("„Ausgabe“ muss „AUSGABE“ heißen", _interpreter.Run(code, trace: true).Error);
        AlgorithmEvaluation result = new AlgorithmEvaluator(_interpreter).Evaluate(_catalog.Lessons[0], code);
        Assert.False(result.Correct);
        Assert.Contains("Zeile 2", result.Message);
        AlgorithmRun valid = _interpreter.Run("Rest = 42 // ausgabe wenn dann\nAUSGABE rest\nAUSGABE 'wenn Dann sonst'\nAUSGABE \"ausgabe\"\nFUER i VON 1 BIS 1\nAUSGABE LAENGE([1, 2])\nENDE FUER");
        Assert.Null(valid.Error);
        Assert.Equal(["42", "wenn Dann sonst", "ausgabe", "2"], valid.Output);
    }

    [Fact]
    public void MissingThenKeepsTheOriginalPhysicalLineNumberIncludingBlankLines()
    {
        string code = "WENN FALSCH DANN\n    AUSGABE 1\nSONST\n    WENN WAHR\n        AUSGABE 2\n    ENDE WENN\nENDE WENN";
        Assert.Contains("Zeile 4: Hier fehlt „DANN“", _interpreter.Run(code).Error);
        Assert.Contains("Zeile 5: Hier fehlt „DANN“", _interpreter.Run("\n" + code).Error);
    }

    [Theory]
    [InlineData("AUSGABE unbekannt", "noch keinen Wert")]
    [InlineData("AUSGABE 10 / 0", "Division durch 0")]
    [InlineData("AUSGABE [10][1]", "Listenindex")]
    [InlineData("AUSGABE [10][-1]", "Listenindex")]
    [InlineData("WENN 1 DANN\nAUSGABE 1\nENDE WENN", "Wahrheitswert")]
    [InlineData("WENN WAHR DANN\nAUSGABE 1", "ENDE")]
    [InlineData("AUSGABE \"Text", "Anführungszeichen")]
    [InlineData("AUSGABE 1 + '2'", "Zahlen")]
    [InlineData("AUSGABE 1 = '1'", "verschiedene Typen")]
    [InlineData("WENN WAHR DANN AUSGABE 1\nENDE WENN", "fehlt")]
    [InlineData("SOLANGE WAHR\nENDE SOLANGE", "10.000")]
    [InlineData("x ← 1\nSOLANGE WAHR\nx ← [x, x]\nENDE SOLANGE", "Liste ist zu groß")]
    public void InvalidProgramsProduceUsefulFeedbackInsteadOfCrashing(string code, string fragment)
    {
        AlgorithmRun run = _interpreter.Run(code, trace: true);
        Assert.NotNull(run.Error);
        Assert.Contains(fragment, run.Error);
        Assert.True(run.Steps.Count <= 500);
    }

    [Fact]
    public void PreviewSnapshotsAreIndependentAndCorrespondToExecutedLines()
    {
        AlgorithmRun run = _interpreter.Run("x ← 1\nx ← x + 1\nAUSGABE x", trace: true);
        Assert.Null(run.Error);
        Assert.Equal(3, run.Steps.Count);
        Assert.Equal("1", run.Steps[0].Variables["x"]);
        Assert.Equal("2", run.Steps[1].Variables["x"]);
        Assert.Empty(run.Steps[1].Output);
        Assert.Equal(["2"], run.Steps[2].Output);
        Assert.Equal(3, run.Steps[2].Line);
    }

    [Fact]
    public void TraceAndExpressionBudgetsLimitLargeAnswers()
    {
        AlgorithmRun trace = _interpreter.Run("FÜR i VON 1 BIS 1000\nx ← i\nENDE FÜR", trace: true);
        Assert.Null(trace.Error);
        Assert.Equal(500, trace.Steps.Count);
        Assert.True(trace.TraceTruncated);
        Assert.NotNull(_interpreter.Run("AUSGABE " + string.Join("+", Enumerable.Repeat("1", 300))).Error);
        Assert.NotNull(_interpreter.Run("AUSGABE " + new string('(', 100) + "1" + new string(')', 100)).Error);
        Assert.NotNull(_interpreter.Run(new string('x', 12001)).Error);
        Assert.NotNull(_interpreter.Run("FÜR i VON 1 BIS 201\nAUSGABE i\nENDE FÜR").Error);
    }

    [Fact]
    public void OneRunCannotReadVariablesFromAnotherRun()
    {
        Assert.Null(_interpreter.Run("x ← 42").Error);
        Assert.Contains("noch keinen Wert", _interpreter.Run("AUSGABE x").Error);
    }

    [Fact]
    public void ValidAlternativesPassAndHardcodedExampleFails()
    {
        AlgorithmEvaluator evaluator = new(_interpreter);
        Assert.True(evaluator.Evaluate(_catalog.Lessons[0], "AUSGABE gewicht <= frei UND (eilauftrag ODER frei - gewicht >= reserve)").Correct);
        Assert.False(evaluator.Evaluate(_catalog.Lessons[0], "AUSGABE WAHR").Correct);
        Assert.True(evaluator.Evaluate(_catalog.Lessons[4], "AUSGABE anzahl * einzelgewicht").Correct);
        string indexed = "anzahl ← 0\ngesamt ← 0\nFÜR i VON 0 BIS LÄNGE(gewichte) - 1\nWENN gewichte[i] >= 0 UND gewichte[i] <= 100 DANN\nanzahl ← anzahl + 1\ngesamt ← gesamt + gewichte[i]\nENDE WENN\nENDE FÜR\nAUSGABE anzahl\nAUSGABE gesamt";
        Assert.True(evaluator.Evaluate(_catalog.Lessons[7], indexed).Correct);
    }

    [Fact]
    public void RadioTraceIncludesTheWindowEndAndPrintsTheAttemptCountLast()
    {
        AlgorithmLesson lesson = _catalog.Lessons[1];
        AlgorithmEvaluator evaluator = new(_interpreter);
        Assert.True(evaluator.Evaluate(lesson, "0\n2\n6\n14\n4").Correct);
        Assert.False(evaluator.Evaluate(lesson, "0\n2\n6\n3").Correct);
        Assert.False(evaluator.Evaluate(lesson, "0\n2\n6\n14\n30").Correct);
    }

    [Theory]
    [InlineData("AUSGABE eilauftrag ODER gewicht <= frei - reserve")]
    [InlineData("AUSGABE gewicht <= frei")]
    [InlineData("AUSGABE gewicht < frei UND (eilauftrag ODER gewicht < frei - reserve)")]
    public void DispatchRejectsWrongReserveAndCapacityRules(string answer) =>
        Assert.False(new AlgorithmEvaluator(_interpreter).Evaluate(_catalog.Lessons[0], answer).Correct);

    [Fact]
    public void ShiftCanBeSolvedByElapsedMinutesButMustExcludeTheEnd()
    {
        AlgorithmLesson lesson = _catalog.Lessons[2];
        AlgorithmEvaluator evaluator = new(_interpreter);
        const string elapsed = "dauer = (dienstende - dienstbeginn + 1440) MOD 1440\nverstrichen = (jetzt - dienstbeginn + 1440) MOD 1440\nAUSGABE verstrichen < dauer";
        Assert.True(evaluator.Evaluate(lesson, elapsed).Correct);
        Assert.False(evaluator.Evaluate(lesson, elapsed.Replace("<", "<=")).Correct);
        Assert.False(evaluator.Evaluate(lesson, "AUSGABE jetzt >= dienstbeginn ODER jetzt < dienstende").Correct);
    }

    [Fact]
    public void LeapYearAcceptsAlternativeFunctionAndRejectsMissingExceptionsAndText()
    {
        AlgorithmLesson lesson = _catalog.Lessons[11];
        AlgorithmEvaluator evaluator = new(_interpreter);
        const string predicate = "y MOD 400 == 0 ODER (y MOD 4 == 0 UND y MOD 100 != 0)";
        const string prefix = "FUNKTION istSchaltjahr(y)\n";
        const string end = "\nENDE FUNKTION";
        Assert.True(evaluator.Evaluate(lesson, prefix + "RÜCKGABE " + predicate + end).Correct);
        Assert.False(evaluator.Evaluate(lesson, prefix + "RÜCKGABE y MOD 4 == 0" + end).Correct);
        Assert.False(evaluator.Evaluate(lesson, prefix + "RÜCKGABE y MOD 4 == 0 UND y MOD 100 != 0" + end).Correct);
        Assert.False(evaluator.Evaluate(lesson, "AUSGABE WAHR").Correct);
        string textAnswer = prefix + "WENN " + predicate + " DANN\nRÜCKGABE 'WAHR'\nSONST\nRÜCKGABE 'FALSCH'\nENDE WENN" + end;
        Assert.Contains("Wahrheitswert", evaluator.Evaluate(lesson, textAnswer).Message);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    public void FaultyGivenProgramsFailTheRelevantBoundaryCase(int id)
    {
        AlgorithmLesson lesson = _catalog.Lessons[id - 1];
        AlgorithmEvaluation evaluation = new AlgorithmEvaluator(_interpreter).Evaluate(lesson, lesson.GivenCode);
        Assert.False(evaluation.Correct);
        Assert.Contains(evaluation.Checks, x => !x.Passed);
    }

    [Fact]
    public void CaseDesignRequiresCorrectPredictionsAndCoverageButAllowsDifferentValues()
    {
        AlgorithmLesson lesson = _catalog.Lessons[6];
        AlgorithmEvaluator evaluator = new(_interpreter);
        Assert.True(evaluator.Evaluate(lesson, "-50 => ungültig\n0 => zulässig\n100 => zulässig\n200 => zu schwer").Correct);
        AlgorithmEvaluation incomplete = evaluator.Evaluate(lesson, "50 => zulässig\n50 => zulässig\n50 => zulässig\n50 => zulässig");
        Assert.False(incomplete.Correct);
        Assert.Equal(4, incomplete.Checks.Count(x => !x.Passed));
        Assert.False(evaluator.Evaluate(lesson, "-1 => ungültig\n0 => zulässig\n100 => zu schwer\n101 => zu schwer").Correct);
        Assert.Contains("Antwortzeile 1", evaluator.Evaluate(lesson, "abc").Message);
    }

    [Fact]
    public void TraceAnswersRequireTheActualOutputOrderAndDoNotExecuteUserCode()
    {
        AlgorithmEvaluator evaluator = new(_interpreter);
        AlgorithmLesson lesson = _catalog.Lessons[5];
        Assert.True(evaluator.Evaluate(lesson, "40\r\n10\r\n-20\r\n3").Correct);
        Assert.False(evaluator.Evaluate(lesson, "40\n10\n3\n-20").Correct);
        Assert.False(evaluator.Evaluate(lesson, lesson.ReferenceCode).Correct);
    }
}
