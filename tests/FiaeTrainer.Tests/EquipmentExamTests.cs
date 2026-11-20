using System.Text.Json;
using FiaeTrainer.Core.Algorithms.Exams;
using FiaeTrainer.Core.Algorithms.Execution;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Unabhängige Grenzprüfungen des zusätzlichen Kulturfestfalls und seines bestehenden Rundenvertrags. </summary>
public sealed class EquipmentExamTests
{
    [Theory]
    [InlineData(11, 20)] [InlineData(12, 20)] [InlineData(13, 10)]
    public void NewReferencesEarnTheirPointsAndRequireDeclaredTypes(int id, int points)
    {
        var task = new AlgorithmExamCatalog().Tasks.Single(task => task.Lesson.Id == id);
        var evaluator = new AlgorithmExamEvaluator(new());
        string answer = id == 13 ? task.Lesson.ReferenceAnswer : task.Lesson.ReferenceCode;
        var result = evaluator.Evaluate(task, answer);
        Assert.True(result.Earned == points, string.Join("; ", result.Checks.Select(check => check.Detail)));
        if (id == 12) Assert.Equal(0, evaluator.Evaluate(task, answer.Replace("bestand: Integer, menge: Integer): Integer", "bestand, menge)")).Earned);
    }

    [Fact]
    public void HalfOpenIntervalsAndSeparateEquipmentUseIndependentCounts()
    {
        var task = new AlgorithmExamCatalog().Tasks.Single(task => task.Lesson.Id == 11);
        var values = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>("""{"bestaende":[[1,4],[2,4],[3,0]],"buchungen":[[1,0,30,4],[1,60,80,4],[2,29,31,3]],"beginn":30,"endzeit":60,"bedarf":2}""")!;
        var run = task.RunFunction(new(), task.Lesson.ReferenceCode, values, AlgorithmExamCatalog.TYPED_VERSION);
        Assert.Null(run.Error); Assert.Equal("1", Assert.Single(run.Output));
        var wrong = task.Lesson.ReferenceCode.Replace("buchung[1] < endzeit", "buchung[1] <= endzeit");
        Assert.InRange(new AlgorithmExamEvaluator(new()).Evaluate(task, wrong).Earned, 1, 19);
    }

    [Fact]
    public void FixedCaseDoesNotEnterRandomPoolAndBoundaryMistakesLosePoints()
    {
        var catalog = new AlgorithmExamCatalog(); var round = catalog.CreateRound(AlgorithmExamCatalog.EQUIPMENT_CASE);
        Assert.Equal(new[] { 11, 12, 13 }, round.TaskIds); Assert.Equal(50, round.TaskIds.Sum(id => catalog.Tasks.Single(task => task.Lesson.Id == id).Points));
        Assert.InRange(round.SecondsLeft(DateTimeOffset.UtcNow), 2698, 2700);
        for (int i = 0; i < 30; i++) Assert.All(catalog.CreateRound().TaskIds, id => Assert.InRange(id, 1, 10));
        Assert.Throws<InvalidOperationException>(() => catalog.CreateRound("unbekannt"));
        var task = catalog.Tasks.Single(task => task.Lesson.Id == 13);
        Assert.Equal(0, new AlgorithmExamEvaluator(new()).Evaluate(task, task.Lesson.ReferenceAnswer.Replace("30 => frei", "30 => belegt")).Earned);
    }
}
