using System.Text.Json;
using FiaeTrainer.Core.Algorithms.Content;
using FiaeTrainer.Core.Algorithms.Exams;
using FiaeTrainer.Core.Algorithms.Execution;
using FiaeTrainer.Core.Exams;
using FiaeTrainer.Core.Sql.Exams;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Unabhängige Sollwerte, alternative Lösungen und sichere Grenzen der neuen Prüfungskataloge. </summary>
public sealed class ExamContentTests
{
    private readonly AlgorithmExamCatalog _algorithms = new();
    private readonly AlgorithmInterpreter _interpreter = new();

    [Theory]
    [InlineData(1)] [InlineData(2)] [InlineData(3)] [InlineData(4)] [InlineData(5)]
    [InlineData(6)] [InlineData(7)] [InlineData(8)] [InlineData(9)] [InlineData(10)]
    public void AllExamReferencesMeetTheirIndependentExpectations(int id)
    {
        AlgorithmExamTask task = _algorithms.Tasks.Single(task => task.Lesson.Id == id);
        string answer = task.Lesson.Kind is AlgorithmTaskKind.Trace or AlgorithmTaskKind.TestCases ? task.Lesson.ReferenceAnswer : task.Lesson.ReferenceCode;
        ExamTaskResult result = new AlgorithmExamEvaluator(_interpreter).Evaluate(task, answer);
        Assert.Equal(task.Points, result.Maximum);
        Assert.Equal(task.Points, result.Earned);
        Assert.Equal(0, new AlgorithmExamEvaluator(_interpreter).Evaluate(task, "").Earned);
    }

    [Fact]
    public void ExamPoolStaysSeparateAndRoundsKeepBalancedFiftyPoints()
    {
        Assert.Equal(30, new AlgorithmCatalog().Lessons.Count);
        Assert.Equal(10, _algorithms.Tasks.Count);
        for (int i = 0; i < 100; i++)
        {
            ExamRound round = _algorithms.CreateRound();
            Assert.Equal(3, round.TaskIds.Distinct().Count());
            Assert.InRange(round.TaskIds[0], 1, 3);
            Assert.InRange(round.TaskIds[1], 4, 8);
            Assert.InRange(round.TaskIds[2], 9, 10);
            Assert.Equal(50, round.TaskIds.Sum(id => _algorithms.Tasks.Single(task => task.Lesson.Id == id).Points));
            Assert.InRange(round.SecondsLeft(DateTimeOffset.UtcNow), 2698, 2700);
        }
    }

    [Fact]
    public void AlternativeFunctionParametersAndHelperAreAcceptedButConstantAnswersFail()
    {
        AlgorithmExamTask task = _algorithms.Tasks.Single(task => task.Lesson.Id == 2);
        string alternative = "FUNKTION nutzungspreis(m, e)\nWENN m < 0 DANN\nRÜCKGABE -1\nENDE WENN\nWENN m == 0 DANN\nRÜCKGABE 0\nENDE WENN\nn = m DIV 15\nWENN m MOD 15 > 0 DANN\nn = n + 1\nENDE WENN\np = 5 + 3 * n\nWENN e DANN\np = p + p / 5\nENDE WENN\nRÜCKGABE p\nENDE FUNKTION";
        AlgorithmExamEvaluator evaluator = new(_interpreter);
        Assert.Equal(20, evaluator.Evaluate(task, alternative).Earned);
        Assert.InRange(evaluator.Evaluate(task, "FUNKTION nutzungspreis(a, b)\nRÜCKGABE 11\nENDE FUNKTION").Earned, 1, 19);
        Assert.Equal(0, evaluator.Evaluate(task, "AUSGABE 11").Earned);
    }

    [Fact]
    public void ExtraTestCasesCannotInflatePointsOrConcealIncorrectExpectedOutput()
    {
        AlgorithmExamTask task = _algorithms.Tasks.Single(task => task.Lesson.Id == 10);
        AlgorithmExamEvaluator evaluator = new(_interpreter);
        Assert.Equal(10, evaluator.Evaluate(task, task.Lesson.ReferenceAnswer + "\n2 => kurz\n3 => kurz").Earned);
        Assert.Equal(0, evaluator.Evaluate(task, task.Lesson.ReferenceAnswer + "\n2 => falsch").Earned);
        Assert.InRange(evaluator.Evaluate(task, "-1 => ungueltig\n0 => kurz").Earned, 1, 9);
    }

    [Fact]
    public void ListSpacingDoesNotChangeTracePointsAndInsertCannotCallProcedures()
    {
        var trace = _algorithms.Tasks.Single(task => task.Lesson.Id == 9);
        Assert.Equal(10, new AlgorithmExamEvaluator(_interpreter).Evaluate(trace, trace.Lesson.ReferenceAnswer.Replace(", ", ",")).Earned);
        var item = new SqlExamCatalog().Cases.Single(item => item.Id == "service");
        Assert.NotNull(SqlExamValidator.Prepare(item, item.Tasks[4], "INSERT INTO Arbeitsschritte EXEC sp_help;", out _));
    }

    [Fact]
    public void SortingReferenceMatchesIndependentLinqOrderAndPreservesDuplicatesOfPriority()
    {
        AlgorithmLesson lesson = _algorithms.Tasks.Single(task => task.Lesson.Id == 5).Lesson;
        Random random = new(1215);
        for (int i = 0; i < 40; i++)
        {
            int[][] values = Enumerable.Range(1, random.Next(0, 12)).Select(id => new[] { random.Next(-2, 5), id }).OrderBy(_ => random.Next()).ToArray();
            var expected = values.OrderBy(row => row[0]).ThenBy(row => row[1]).ToArray();
            var run = _interpreter.RunFunction(lesson.ReferenceCode, lesson.FunctionName, [JsonSerializer.SerializeToElement(values)]);
            Assert.Null(run.Error);
            int[][] actual = JsonSerializer.Deserialize<int[][]>(Assert.Single(run.Output))!;
            Assert.Equal(JsonSerializer.Serialize(expected), JsonSerializer.Serialize(actual));
        }
    }

    [Theory]
    [InlineData("EXEC sp_help;")]
    [InlineData("SELECT * FROM master.sys.tables;")]
    [InlineData("SELECT * FROM fall.Sendungen;")]
    [InlineData("SELECT * INTO Andere FROM Artikel;")]
    [InlineData("SELECT * FROM Artikel; DELETE FROM Artikel;")]
    [InlineData("SELECT * FROM OPENROWSET('SQLNCLI','x','x');")]
    [InlineData("SELECT * FROM Artikel WITH (NOLOCK);")]
    [InlineData("select Id from Artikel;")]
    [InlineData("SELECT Id FROM Artikel")]
    public void SqlExamRejectsExternalAndUnrestrictedStatements(string answer)
    {
        SqlExamCase item = new SqlExamCatalog().Cases.Single(item => item.Id == "lager");
        Assert.NotNull(SqlExamValidator.Prepare(item, item.Tasks[0], answer, out _));
    }

    [Fact]
    public void SqlRewritePreservesLiteralsAndAliasesNamedLikeTables()
    {
        SqlExamCase item = new SqlExamCatalog().Cases.Single(item => item.Id == "lager");
        Assert.Null(SqlExamValidator.Prepare(item, item.Tasks[0], "SELECT Artikel.Id, 'Artikel' AS Text FROM Artikel AS Artikel;", out string sql));
        Assert.Contains("#Artikel", sql);
        Assert.Contains("Artikel.Id", sql);
        Assert.DoesNotContain("#Artikel.Id", sql);
        Assert.Contains("'Artikel'", sql);
    }
}
