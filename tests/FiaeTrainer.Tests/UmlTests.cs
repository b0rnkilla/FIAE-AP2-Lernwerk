using System.Text.Json;
using FiaeTrainer.Core.Uml.Content;
using FiaeTrainer.Core.Uml.Evaluation;
using FiaeTrainer.Core.Uml.Models;
using FiaeTrainer.Core.Uml.Practice;
using FiaeTrainer.Web.Learning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Unabhängige Fachbeispiele, fehlerhafte Diagramme, Varianten und isolierte Profilspeicherung. </summary>
public sealed class UmlTests : IDisposable
{
    private readonly UmlCatalog _catalog = new();
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-Uml", Guid.NewGuid().ToString("N"));
    public static IEnumerable<object[]> LearningIds => Enumerable.Range(1, 36).Select(id => new object[] { id });
    public static IEnumerable<object[]> ModelIds => new UmlCatalog().Lessons.Where(task => task.Format != "Read").Select(task => new object[] { task.Id });

    [Theory, MemberData(nameof(LearningIds))]
    public void EachReferencePassesAndAnEmptyAnswerFails(int id)
    {
        var task = _catalog.Lessons[id - 1];
        foreach (var answer in task.Format == "Read" ? new[] { new UmlAnswer { SelectedIds = task.CorrectIds.ToList() } } : task.Solutions.Select(model => new UmlAnswer { Model = model.Copy() })) Assert.True(UmlEvaluator.Evaluate(task, answer).Correct, task.Title);
        Assert.False(UmlEvaluator.Evaluate(task, new()).Correct);
        Assert.Equal(3, task.Hints.Count);
        if (task.Format == "Build") Assert.Empty(task.InitialAnswer().Model.Nodes);
        if (task.Format == "Repair") Assert.False(UmlEvaluator.Evaluate(task, task.InitialAnswer()).Correct);
    }

    [Theory, MemberData(nameof(ModelIds))]
    public void NamesIdsAndNonTemporalOrderDoNotDetermineTheGrade(int id)
    {
        var task = _catalog.Lessons[id - 1]; var model = task.Solutions[0].Copy();
        var ids = model.Nodes.Select(node => node.Id).Concat(model.Fragments.Select(frame => frame.Id)).ToDictionary(key => key, _ => Guid.NewGuid().ToString("N"));
        foreach (var node in model.Nodes) { node.Id = ids[node.Id]; node.Name = "Eigene Bezeichnung " + node.Key; node.Members.Reverse(); foreach (var member in node.Members) { member.Id = Guid.NewGuid().ToString("N"); member.Name = "Eigenes Merkmal " + member.Key; if (member.Parameters.Length > 0) member.Parameters = string.Join(",", member.Parameters.Split(',').Select((part, index) => "p" + index + ": " + part.Split(':').Last())); } }
        foreach (var frame in model.Fragments) frame.Id = ids[frame.Id];
        foreach (var edge in model.Links) { edge.Id = Guid.NewGuid().ToString("N"); edge.Source = ids[edge.Source]; edge.Target = ids[edge.Target]; if (edge.Fragment.Length > 0) edge.Fragment = ids[edge.Fragment]; if (task.DiagramType is "State" or "Sequence" && edge.Meaning.Length > 0) edge.Name = "Eigene Nachricht"; }
        model.Nodes.Reverse(); model.Fragments.Reverse(); if (task.DiagramType != "Sequence") model.Links.Reverse();
        Assert.True(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
    }

    [Theory, MemberData(nameof(ModelIds))]
    public void ExtraNodesAndMissingConnectionsAreNotFullSolutions(int id)
    {
        var task = _catalog.Lessons[id - 1]; var model = task.Solutions[0].Copy();
        model.Nodes.Add(new() { Key = "archive", Name = "Archiv", Kind = UmlKinds.NodeTypes(task.DiagramType)[0] });
        Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        if (task.Solutions[0].Links.Count > 0) { model = task.Solutions[0].Copy(); model.Links.RemoveAt(0); Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct); }
    }

    [Fact]
    public void InterfaceAbstractionAndActivityEndRespectEquivalentMeaning()
    {
        var task = _catalog.Lessons[16]; var model = task.Solutions[0].Copy();
        model.Nodes.Single(node => node.Kind == "Interface").Members[0].Abstract = true;
        Assert.True(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        var activity = _catalog.Lessons[23]; model = activity.Solutions[0];
        var boxes = FiaeTrainer.Web.Components.Uml.UmlLayout.Place(activity, model);
        var end = model.Nodes.Single(node => node.Kind == "Final");
        Assert.All(model.Nodes.Where(node => node.Id != end.Id), node => Assert.True(boxes[node.Id].Y < boxes[end.Id].Y));
        Assert.Equal(model.Nodes.Count, boxes.Count);
    }

    [Fact]
    public void UseCaseHasIndependentIncludeAndExtendDirectionContracts()
    {
        UmlModel Include(string source, string target) => new() { SystemName = "Meine Anwendung", Nodes = [new() { Id = "person", Key = "guest", Name = "Gast", Kind = "Actor" }, new() { Id = "base", Key = "reserve", Name = "Absenden", Kind = "UseCase" }, new() { Id = "part", Key = "check", Name = "Prüfen", Kind = "UseCase" }], Links = [new() { Source = "person", Target = "base", Kind = "Association" }, new() { Source = source, Target = target, Kind = "Include" }] };
        var task = _catalog.Lessons[6];
        Assert.True(UmlEvaluator.Evaluate(task, new() { Model = Include("base", "part") }).Correct);
        Assert.False(UmlEvaluator.Evaluate(task, new() { Model = Include("part", "base") }).Correct);
        var optional = _catalog.Lessons[7]; var model = optional.Solutions[0].Copy(); var extension = model.Links.Single(link => link.Kind == "Extend");
        (extension.Source, extension.Target) = (extension.Target, extension.Source);
        Assert.False(UmlEvaluator.Evaluate(optional, new() { Model = model }).Correct);
        model = task.Solutions[0].Copy(); model.SystemName = ""; Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
    }

    [Fact]
    public void ClassParameterTypesMultiplicityAndRelationshipKindAreIndependent()
    {
        var task = _catalog.Lessons[12]; var model = task.Solutions[0].Copy(); var method = model.Nodes[0].Members.Single(item => item.Kind == "Method");
        method.Parameters = "personen: int"; method.Type = "bool";
        Assert.True(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        method.Parameters = "personen: String"; Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        method.Parameters = "personen"; Assert.NotEmpty(UmlRules.Errors(task, model));
        task = _catalog.Lessons[13]; model = new() { Nodes = [new() { Id = "k", Key = "guest", Name = "Kunde", Kind = "Class" }, new() { Id = "r", Key = "booking", Name = "Reservierung", Kind = "Class" }], Links = [new() { Source = "r", Target = "k", Kind = "Association", SourceMultiplicity = "0..n", TargetMultiplicity = "1..1" }] };
        Assert.True(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        model.Links[0].SourceMultiplicity = "1"; Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        task = _catalog.Lessons[16]; model = task.Solutions[0].Copy(); model.Links[0].Kind = "Generalization"; Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        task = _catalog.Lessons[17]; model = task.Solutions[0].Copy(); model.Links.Single(item => item.Kind == "Composition").Kind = "Aggregation"; Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
    }

    [Fact]
    public void ActivityMergeForkJoinAndResponsibilityHaveSeparateMeaning()
    {
        var task = _catalog.Lessons[20]; var model = task.Solutions[0].Copy(); model.Nodes.Single(node => node.Key == "merge").Kind = "Join";
        Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        task = _catalog.Lessons[21]; model = task.Solutions[0].Copy(); model.Nodes.Single(node => node.Key == "fork").Kind = "Decision";
        Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        task = _catalog.Lessons[22]; model = task.Solutions[0].Copy(); model.Nodes.Single(node => node.Key == "check").Lane = "guest";
        Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        Assert.Equal(2, _catalog.Lessons[19].Solutions.Count);
        Assert.All(_catalog.Lessons[19].Solutions, variant => Assert.True(UmlEvaluator.Evaluate(_catalog.Lessons[19], new() { Model = variant }).Correct));
    }

    [Fact]
    public void SequenceOrderReturnsAndFragmentsAreNotInterchangeable()
    {
        var task = _catalog.Lessons[25]; var model = task.Solutions[0].Copy(); (model.Links[1], model.Links[2]) = (model.Links[2], model.Links[1]);
        Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        model = task.Solutions[0].Copy(); model.Links[2].Kind = "Message";
        Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        foreach (int id in new[] { 28, 29, 30 })
        {
            task = _catalog.Lessons[id - 1]; model = task.Solutions[0].Copy(); model.Fragments[0].Kind = id == 28 ? "loop" : "alt";
            Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
            model = task.Solutions[0].Copy(); model.Links.First(link => link.Fragment.Length > 0).Fragment = "";
            Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        }
    }

    [Fact]
    public void StateGuardsEventsAndActionKindsAreIndependent()
    {
        var task = _catalog.Lessons[32]; var model = task.Solutions[0].Copy(); model.Links.Single(link => link.Guard == "free").Guard = "full";
        Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        task = _catalog.Lessons[33]; model = task.Solutions[0].Copy(); model.Nodes.Single(node => node.Key == "ready").Members.Single(member => member.Key == "signal").Kind = "Exit";
        Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        task = _catalog.Lessons[34]; model = task.Solutions[0].Copy(); model.Nodes.Single(node => node.Key == "cancelled").Kind = "Final";
        Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
    }

    [Fact]
    public void EveryReadTaskRejectsMissingExtraAndDuplicateSelectionsWithoutLeakingExplanation()
    {
        foreach (var task in _catalog.Lessons.Where(task => task.Format == "Read"))
            foreach (var ids in new[] { new List<string>(), task.CorrectIds.Skip(1).ToList(), task.CorrectIds.Concat(["unknown"]).ToList(), task.CorrectIds.Concat(task.CorrectIds).ToList() })
            {
                var result = UmlEvaluator.Evaluate(task, new() { SelectedIds = ids }); Assert.False(result.Correct); Assert.DoesNotContain(task.Explanation, result.Checks[0].Detail);
            }
    }

    [Fact]
    public void MalformedAndOversizedModelsFailWithoutDestroyingDrafts()
    {
        Assert.False(UmlRules.Storable(new() { Model = null! }));
        Assert.False(UmlRules.Storable(new() { Model = new() { Nodes = null! } }));
        Assert.False(UmlRules.Storable(new() { Model = new() { Nodes = Enumerable.Range(0, 17).Select(_ => new UmlNode()).ToList() } }));
        var answer = _catalog.Lessons[25].InitialAnswer(); answer.Model = _catalog.Lessons[25].Solutions[0].Copy(); answer.Model.Links[0].Target = "missing";
        Assert.True(UmlRules.Storable(answer)); Assert.False(UmlEvaluator.Evaluate(_catalog.Lessons[25], answer).Correct);
        var copy = answer.Copy(); copy.Model.Nodes[0].Name = "Geändert"; Assert.NotEqual(copy.Model.Nodes[0].Name, answer.Model.Nodes[0].Name);
    }

    [Fact]
    public async Task LegacyFieldsDraftsRevisionsAndLearningAchievementsSurvive()
    {
        Directory.CreateDirectory(_directory);
        await File.WriteAllTextAsync(Path.Combine(_directory, "progress.json"), """
            {"LastLessonId":17,"Lessons":{"17":{"Sql":"SELECT 1;","Completed":true}},
             "WorkshopLessons":{"4":{"Sql":"SELECT 2;","Attempts":1}},
             "LastAlgorithmLessonId":8,"AlgorithmLessons":{"8":{"Draft":"AUSGABE 123","Completed":true}},
             "BusinessQuestions":{"7":{"Attempts":2,"Completed":true,"Independent":true}},
             "LastDesignLessonId":37,"DesignLessons":{"37":{"ContentVersion":"2026-11-06","Completed":true}}}
            """);
        var task = _catalog.Lessons[4]; var progress = new UmlProgress { Answer = new() { Model = task.Solutions[0].Copy() } }; var store = Store();
        Assert.True((await store.SaveUmlAsync(5, progress, _catalog, true))!.Correct);
        var state = await Store().LoadAsync(); Assert.True(state.UmlLessons[5].Independent); Assert.Equal("SELECT 1;", state.Lessons[17].Sql); Assert.True(state.DesignLessons[37].Completed);
        Assert.Equal("SELECT 2;", state.WorkshopLessons[4].Sql); Assert.Equal("AUSGABE 123", state.AlgorithmLessons[8].Draft); Assert.Equal(8, state.LastAlgorithmLessonId); Assert.True(state.BusinessQuestions[7].Independent); Assert.Equal(2, state.BusinessQuestions[7].Attempts);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveUmlAsync(5, new(), _catalog));
        progress.Answer = task.InitialAnswer(); progress.Assisted = false; await store.SaveUmlAsync(5, progress, _catalog); Assert.True((await store.LoadAsync()).UmlLessons[5].Completed);
        progress.ContentVersion = "alt"; await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveUmlAsync(5, progress, _catalog));
    }

    [Theory]
    [InlineData("proberaum")] [InlineData("ausgabe")] [InlineData("abend")]
    public async Task ExamsScoreFiftyAndFinalAnswersAreImmutable(string caseId)
    {
        var round = new UmlExamRound { CaseId = caseId }; var store = Store(); await store.StartUmlExamAsync(round);
        foreach (var task in _catalog.Cases.Single(item => item.Id == caseId).Tasks) round.Answers[task.Id] = new() { Model = task.Solutions[0].Copy() };
        round = await store.SaveUmlExamAsync(round, _catalog, true);
        Assert.Equal(50, round.Earned); Assert.Single((await store.LoadAsync()).UmlHistory);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveUmlExamAsync(round, _catalog, true));
        Assert.Single((await Store().LoadAsync()).UmlHistory);
    }

    [Fact]
    public async Task PauseRestartDeadlineAndStaleWindowsUseTheSavedClock()
    {
        var store = Store(); var round = new UmlExamRound { CaseId = "proberaum", RunningSinceUtc = DateTimeOffset.UtcNow.AddSeconds(-80) }; await store.StartUmlExamAsync(round);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.StartUmlExamAsync(new() { CaseId = "abend" }));
        round.Pause(DateTimeOffset.UtcNow); round.FlaggedIds.Add(2); round = await store.SaveUmlExamAsync(round, _catalog);
        Assert.InRange(round.RemainingSeconds, 2618, 2620); Assert.Null((await Store().LoadAsync()).UmlExam!.RunningSinceUtc); Assert.Contains(2, (await Store().LoadAsync()).UmlExam!.FlaggedIds);
        var stale = round.Copy(); round.RemainingSeconds = 2700; round.RunningSinceUtc = DateTimeOffset.UtcNow; round = await store.SaveUmlExamAsync(round, _catalog); Assert.InRange(round.SecondsLeft(DateTimeOffset.UtcNow), 2617, 2620);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveUmlExamAsync(stale, _catalog));
        var expired = new UmlExamRound { CaseId = "abend", RunningSinceUtc = DateTimeOffset.UtcNow.AddMinutes(-46) }; await store.StartUmlExamAsync(expired, round.Id);
        expired.Answers[1] = new() { Model = _catalog.Cases[2].Tasks[0].Solutions[0].Copy() }; expired.Pause(DateTimeOffset.UtcNow);
        var submitted = await store.SaveUmlExamAsync(expired, _catalog); Assert.NotNull(submitted.FinishedUtc); Assert.Empty(submitted.Answers); Assert.Equal(0, submitted.Earned);
    }
    private ProgressStore Store() => new(new EnvironmentStub(), new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Training:ProgressDirectory"] = _directory }).Build());
    public void Dispose() { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
    private sealed class EnvironmentStub : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests"; public string EnvironmentName { get; set; } = "Tests"; public string ContentRootPath { get; set; } = ""; public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider(); public string WebRootPath { get; set; } = ""; public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
