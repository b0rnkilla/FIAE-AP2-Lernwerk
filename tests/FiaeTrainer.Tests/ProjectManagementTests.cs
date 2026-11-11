using FiaeTrainer.Core.ProjectManagement.Content;
using FiaeTrainer.Core.ProjectManagement.Evaluation;
using FiaeTrainer.Core.ProjectManagement.Models;
using FiaeTrainer.Core.ProjectManagement.Practice;
using FiaeTrainer.Web.Learning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System.Text.Json;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Unabhängige Rechenbeispiele, alternative Pläne und getrennte unveränderliche Prüfungsprofile. </summary>
public sealed class ProjectManagementTests : IDisposable
{
    private readonly ProjectCatalog _catalog = new();
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-ProjectTests", Guid.NewGuid().ToString("N"));

    [Fact]
    public void AllOwnContentHasCompleteReferencesAndUnsolvedRepairTemplates()
    {
        Assert.Equal(54, _catalog.Lessons.Count); Assert.Equal(9, _catalog.Lessons.GroupBy(task => task.Chapter).Count());
        Assert.Equal(6, _catalog.Lessons.Select(task => task.Kind).Distinct().Count());
        Assert.All(_catalog.Lessons.GroupBy(task => task.Chapter), group => Assert.Equal(6, group.Count()));
        foreach (var task in _catalog.Lessons.Concat(_catalog.Cases.SelectMany(item => item.Tasks)))
        {
            Assert.False(ProjectEvaluator.Evaluate(task, new()).Correct);
            Assert.True(ProjectEvaluator.Evaluate(task, Reference(task)).Correct, task.Title);
            if (task.Format == "Repair") Assert.False(ProjectEvaluator.Evaluate(task, task.InitialAnswer()).Correct);
        }
    }

    [Fact]
    public void NetworkUsesIndependentForwardBackwardAndVirtualFinishValues()
    {
        var main = ProjectScheduler.Calculate(_catalog.Lessons[19].Plan!.Activities);
        Assert.Equal(new ProjectTiming(0, 2, 0, 2, 0, 0), main["analysis"]);
        Assert.Equal(new ProjectTiming(2, 5, 4, 7, 2, 2), main["screen"]);
        Assert.Equal(new ProjectTiming(2, 7, 2, 7, 0, 0), main["api"]);
        Assert.Equal(new ProjectTiming(7, 8, 7, 8, 0, 0), main["integrate"]);
        var buffers = ProjectScheduler.Calculate(_catalog.Lessons[22].Plan!.Activities);
        Assert.Equal(new ProjectTiming(2, 4, 3, 5, 1, 0), buffers["b"]);
        Assert.Equal(new ProjectTiming(4, 6, 5, 7, 1, 1), buffers["d"]);
        var independentEnds = ProjectScheduler.Calculate([new() { Key = "a", Duration = 2 }, new() { Key = "b", Duration = 5 }]);
        Assert.Equal(new ProjectTiming(0, 2, 3, 5, 3, 3), independentEnds["a"]);
    }

    [Fact]
    public void TiedCriticalPathsAndUnorderedDagDoNotLoseBranches()
    {
        var plan = _catalog.Lessons[21].Plan!;
        Assert.All(ProjectScheduler.Calculate(plan.Activities).Values, timing => Assert.Equal(0, timing.TotalFloat));
        Assert.Equal(ProjectScheduler.Calculate(plan.Activities).OrderBy(pair => pair.Key), ProjectScheduler.Calculate(plan.Activities.AsEnumerable().Reverse().ToList()).OrderBy(pair => pair.Key));
        var answer = Reference(_catalog.Lessons[21]); answer.Activities[1].Critical = false;
        Assert.False(ProjectEvaluator.Evaluate(_catalog.Lessons[21], answer).Correct);
        Assert.Throws<InvalidDataException>(() => ProjectScheduler.Calculate([new() { Key = "a", Predecessors = ["b"] }, new() { Key = "b", Predecessors = ["a"] }]));
        Assert.Throws<InvalidDataException>(() => ProjectScheduler.Calculate([new() { Key = "a", Predecessors = ["missing"] }]));
    }

    [Fact]
    public void OtherFeasibleStartsAndDifferentResourceOrderAreAccepted()
    {
        var task = _catalog.Lessons[26]; var answer = Reference(task);
        SetStarts(answer, ("analysis", 1), ("screen", 5), ("api", 3), ("integrate", 8));
        Assert.True(ProjectEvaluator.Evaluate(task, answer).Correct);
        SetStarts(answer, ("integrate", 10)); Assert.False(ProjectEvaluator.Evaluate(task, answer).Correct);
        task = _catalog.Lessons[27]; answer = Reference(task);
        SetStarts(answer, ("frontend", 5), ("backend", 2), ("review", 8));
        Assert.True(ProjectEvaluator.Evaluate(task, answer).Correct);
        SetStarts(answer, ("frontend", 2)); Assert.False(ProjectEvaluator.Evaluate(task, answer).Correct);
        Assert.True(ProjectEvaluator.Evaluate(_catalog.Lessons[28], Reference(_catalog.Lessons[28])).Correct);
    }

    [Fact]
    public void HalfOpenBoundaryAndZeroDayMilestoneRespectTimeAndCapacity()
    {
        var task = _catalog.Lessons[29]; var answer = Reference(task);
        SetStarts(answer, ("prepare", 0), ("rollout", 2), ("launch", 5));
        Assert.True(ProjectEvaluator.Evaluate(task, answer).Correct);
        answer.Activities.Single(item => item.Key == "launch").Duration = "1";
        Assert.False(ProjectEvaluator.Evaluate(task, answer).Correct);
        answer = Reference(task); SetStarts(answer, ("rollout", 1)); Assert.False(ProjectEvaluator.Evaluate(task, answer).Correct);
        answer = Reference(task); answer.Activities[0].Start = int.MaxValue.ToString(); Assert.False(ProjectEvaluator.Evaluate(task, answer).Correct);
    }

    [Theory]
    [InlineData("extra")] [InlineData("missing")] [InlineData("duplicate-role")] [InlineData("duplicate-link")] [InlineData("cycle")] [InlineData("dangling")] [InlineData("duration")] [InlineData("direction")]
    public void DamagedAndWrongNetworksReturnFeedbackWithoutThrowing(string fault)
    {
        var task = _catalog.Lessons[19]; var answer = Reference(task);
        switch (fault)
        {
            case "extra": answer.Activities.Add(new() { Key = "extra" }); break;
            case "missing": answer.Activities.RemoveAt(0); break;
            case "duplicate-role": answer.Activities[1].Key = answer.Activities[0].Key; break;
            case "duplicate-link": answer.Dependencies.Add(new() { SourceId = answer.Dependencies[0].SourceId, TargetId = answer.Dependencies[0].TargetId }); break;
            case "cycle": answer.Dependencies.Add(new() { SourceId = answer.Activities[^1].Id, TargetId = answer.Activities[0].Id }); break;
            case "dangling": answer.Dependencies[0].TargetId = "unknown"; break;
            case "duration": answer.Activities[0].Duration = "-2"; break;
            case "direction": (answer.Dependencies[0].SourceId, answer.Dependencies[0].TargetId) = (answer.Dependencies[0].TargetId, answer.Dependencies[0].SourceId); break;
        }
        Assert.False(ProjectEvaluator.Evaluate(task, answer).Correct);
        Assert.True(ProjectRules.Storable(answer));
    }

    [Fact]
    public void StructureUsesRolesAndParentIdentityInsteadOfLabelsOrListOrder()
    {
        var task = _catalog.Lessons[12]; var answer = Reference(task);
        answer.Activities.Reverse(); foreach (var item in answer.Activities) item.Name = "Eigene Bezeichnung";
        Assert.True(ProjectEvaluator.Evaluate(task, answer).Correct);
        answer.Activities.Single(item => item.Key == "test").ParentId = answer.Activities.Single(item => item.Key == "planning").Id;
        Assert.False(ProjectEvaluator.Evaluate(task, answer).Correct);
        answer = Reference(task); answer.Activities[0].Id = answer.Activities[1].Id;
        Assert.False(ProjectRules.Storable(answer)); Assert.False(ProjectEvaluator.Evaluate(task, answer).Correct);
    }

    [Theory]
    [InlineData(31, "1700;4200;1040;6940")]
    [InlineData(32, "13500;10800;24300")]
    [InlineData(33, "6400;5200;8800;9400;2;1")]
    [InlineData(37, "800;15;1")]
    [InlineData(40, "20;300;301")]
    [InlineData(41, "5700;5400;1100;1")]
    [InlineData(50, "2000;1500;1;2")]
    public void FinancialReferencesMatchIndependentlyCalculatedNumbers(int id, string values)
    {
        var task = _catalog.Lessons[id - 1];
        Assert.Equal(values.Split(';'), task.Fields.Select(field => task.Solutions[0].Values[field.Key]));
        var answer = Reference(task);
        foreach (var field in task.Fields.Where(field => field.Comparison == "Number")) answer.Values[field.Key] = answer.Values[field.Key].Replace('.', ',') + (answer.Values[field.Key].Contains('.') ? "" : ",00");
        Assert.True(ProjectEvaluator.Evaluate(task, answer).Correct);
        answer.Values[task.Fields[0].Key] = "99999"; Assert.False(ProjectEvaluator.Evaluate(task, answer).Correct);
    }

    [Fact]
    public async Task OldProfileModulesAndLegacyTextSurviveNewDraftsAndStaleWrites()
    {
        var store = Store(); await store.SaveLessonAsync(1, new() { Sql = "synthetisch", Completed = true });
        var state = await store.LoadAsync(); state.SoftwareLessons[1] = new() { Answer = new() { Reasoning = "Altbestand 05" } }; state.QualityLessons[1] = new() { Answer = new() { Reasoning = "Altbestand 06" } };
        state.WorkshopLessons[2] = new() { Sql = "Werkstattentwurf" };
        state.AlgorithmLessons[3] = new() { Draft = "AUSGABE 17" }; state.LastAlgorithmLessonId = 3;
        state.BusinessQuestions[4] = new() { Completed = true, Attempts = 2, LastCorrect = true };
        state.DesignLessons[5] = new() { Answer = new() { DiagramValues = new() { ["own"] = "Entwurfswert" } } };
        state.UmlLessons[6] = new() { Answer = new() { SelectedIds = ["eigene-wahl"] } };
        using var before = JsonDocument.Parse(JsonSerializer.Serialize(state));
        await File.WriteAllTextAsync(Path.Combine(_directory, "progress.json"), JsonSerializer.Serialize(state));
        var draft = new ProjectProgress { Answer = new() { Values = new() { ["own"] = "noch unvollständig" } } };
        await store.SaveProjectAsync(2, draft, _catalog);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveProjectAsync(2, new(), _catalog));
        state = await Store().LoadAsync(); Assert.Equal("synthetisch", state.Lessons[1].Sql); Assert.True(state.Lessons[1].Completed);
        Assert.Equal("Altbestand 05", state.SoftwareLessons[1].Answer.Reasoning); Assert.Equal("Altbestand 06", state.QualityLessons[1].Answer.Reasoning);
        Assert.Equal("noch unvollständig", state.ProjectLessons[2].Answer.Values["own"]);
        using var after = JsonDocument.Parse(JsonSerializer.Serialize(state));
        foreach (var property in before.RootElement.EnumerateObject().Where(property => !property.Name.StartsWith("Project") && property.Name != "LastProjectLessonId"))
            Assert.Equal(property.Value.GetRawText(), after.RootElement.GetProperty(property.Name).GetRawText());
    }

    [Theory]
    [InlineData("bibliothek")] [InlineData("sportverein")] [InlineData("museum")]
    public async Task AllExamsScoreFiftyAndCanOnlyBeSubmittedOnce(string caseId)
    {
        var store = Store(); var round = new ProjectExamRound { CaseId = caseId }; await store.StartProjectExamAsync(round, _catalog);
        foreach (var task in _catalog.Cases.Single(item => item.Id == caseId).Tasks) round.Answers[task.Id] = Reference(task);
        round = await store.SaveProjectExamAsync(round, _catalog, true); Assert.Equal(50, round.Earned);
        round.Answers.Clear(); await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveProjectExamAsync(round, _catalog, true));
        var loaded = await Store().LoadAsync(); Assert.Single(loaded.ProjectHistory); Assert.Equal(3, loaded.ProjectExam!.Answers.Count);
    }

    [Fact]
    public async Task PauseRestartStaleWindowsAndExpiredAnswersRespectUtcBudget()
    {
        var store = Store(); var round = new ProjectExamRound { CaseId = "bibliothek", RunningSinceUtc = DateTimeOffset.UtcNow.AddSeconds(-80) }; await store.StartProjectExamAsync(round, _catalog);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.StartProjectExamAsync(new() { CaseId = "museum" }, _catalog));
        round.Pause(DateTimeOffset.UtcNow); round.UsedReference = true; round.FlaggedIds.Add(2); round = await store.SaveProjectExamAsync(round, _catalog);
        Assert.InRange(round.RemainingSeconds, 2618, 2620); Assert.Null((await Store().LoadAsync()).ProjectExam!.RunningSinceUtc);
        var stale = round.Copy(); round.RemainingSeconds = 2700; round.RunningSinceUtc = DateTimeOffset.UtcNow; round.UsedReference = false;
        round = await store.SaveProjectExamAsync(round, _catalog); Assert.InRange(round.SecondsLeft(DateTimeOffset.UtcNow), 2617, 2620); Assert.True(round.UsedReference);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveProjectExamAsync(stale, _catalog));
        var expired = new ProjectExamRound { CaseId = "museum", RunningSinceUtc = DateTimeOffset.UtcNow.AddMinutes(-46) }; await store.StartProjectExamAsync(expired, _catalog, round.Id);
        expired.Answers[1] = Reference(_catalog.Cases[2].Tasks[0]); expired = await store.SaveProjectExamAsync(expired, _catalog);
        Assert.NotNull(expired.FinishedUtc); Assert.Empty(expired.Answers); Assert.Equal(0, expired.Earned);
    }

    private static void SetStarts(ProjectAnswer answer, params (string Key, int Start)[] starts) { foreach (var item in starts) answer.Activities.Single(node => node.Key == item.Key).Start = item.Start.ToString(); }
    private static ProjectAnswer Reference(ProjectAssignment task) => task.Kind is "Choice" or "Review" ? new() { SelectedIds = task.CorrectIds.ToList() } : task.Solutions[0].Copy();
    private ProgressStore Store() => new(new EnvironmentStub(), new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Training:ProgressDirectory"] = _directory }).Build());
    public void Dispose() { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
    private sealed class EnvironmentStub : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests"; public string EnvironmentName { get; set; } = "Tests"; public string ContentRootPath { get; set; } = ""; public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider(); public string WebRootPath { get; set; } = ""; public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
