using FiaeTrainer.Core.SoftwareDesign.Content;
using FiaeTrainer.Core.SoftwareDesign.Evaluation;
using FiaeTrainer.Core.SoftwareDesign.Models;
using FiaeTrainer.Core.SoftwareDesign.Practice;
using FiaeTrainer.Web.Learning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Unabhängige Nachrichten, Fachfehler, Profilbestand und Prüfungsfristen für Abschnitt 05. </summary>
public sealed class SoftwareDesignTests : IDisposable
{
    private readonly SoftwareCatalog _catalog = new();
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "FiaeTrainer-SoftwareTests", Guid.NewGuid().ToString("N"));

    [Fact]
    public void CatalogHasEightChaptersAndEveryReferenceIsConsistent()
    {
        Assert.Equal(48, _catalog.Lessons.Count);
        Assert.Equal(8, _catalog.Lessons.GroupBy(task => task.Chapter).Count());
        Assert.All(_catalog.Lessons.GroupBy(task => task.Chapter), chapter => Assert.Equal(6, chapter.Count()));
        Assert.Equal(6, _catalog.Lessons.Select(task => task.Kind).Distinct().Count());
        Assert.Equal(3, _catalog.Cases.Count);
        foreach (var task in _catalog.Lessons.Concat(_catalog.Cases.SelectMany(item => item.Tasks)))
        {
            Assert.False(SoftwareEvaluator.Evaluate(task, new()).Correct);
            foreach (var answer in task.Kind == "Choice" ? [Reference(task)] : task.Solutions)
                Assert.True(SoftwareEvaluator.Evaluate(task, answer).Correct, task.Title);
            if (task.Format == "Repair") Assert.False(SoftwareEvaluator.Evaluate(task, task.InitialAnswer()).Correct, task.Title);
        }
    }

    [Theory]
    [InlineData(1)] [InlineData(7)] [InlineData(13)] [InlineData(19)] [InlineData(25)] [InlineData(31)] [InlineData(37)] [InlineData(43)] [InlineData(45)] [InlineData(47)]
    public void SelectionsRejectMissingExtraUnknownAndDuplicates(int id)
    {
        var task = _catalog.Lessons[id - 1];
        foreach (var ids in new[] { new List<string>(), task.CorrectIds.Skip(1).ToList(), task.CorrectIds.Concat(["unknown"]).ToList(), task.CorrectIds.Concat(task.CorrectIds).ToList() })
        {
            var result = SoftwareEvaluator.Evaluate(task, new() { SelectedIds = ids, Reasoning = task.Explanation });
            Assert.False(result.Correct);
            Assert.DoesNotContain(task.Explanation, string.Join(" ", result.Checks.Select(check => check.Detail)));
        }
    }

    [Fact]
    public void ObjectStateAndBoundariesUseIndependentExpectedValues()
    {
        var stateTask = _catalog.Lessons[7];
        Assert.Equal(new[] { "41", "42", "angefragt", "angefragt" }, stateTask.Fields.Select(field => field.Kind == "Select" ? field.Options[stateTask.Solutions[0].Values[field.Key]] : stateTask.Solutions[0].Values[field.Key]));
        var callTask = _catalog.Lessons[14];
        Assert.Contains("30", callTask.Solutions[0].Values.Values);
        Assert.Contains("24", callTask.Solutions[0].Values.Values);
        var answer = Reference(callTask);
        var resultField = callTask.Fields.First(field => answer.Values[field.Key] == "24");
        answer.Values[resultField.Key] = "30";
        Assert.False(SoftwareEvaluator.Evaluate(callTask, answer).Correct);
        answer.Values[resultField.Key] = "24,00";
        Assert.True(SoftwareEvaluator.Evaluate(callTask, answer).Correct);
        var criterion = _catalog.Lessons[1]; answer = Reference(criterion);
        var seconds = criterion.Fields.First(field => answer.Values[field.Key] == "2");
        answer.Values[seconds.Key] = "2.0"; Assert.True(SoftwareEvaluator.Evaluate(criterion, answer).Correct);
        answer.Values[seconds.Key] = "2000"; Assert.False(SoftwareEvaluator.Evaluate(criterion, answer).Correct);
    }

    [Theory]
    [InlineData("{\"b\":true,\"a\":1.00}", true)]
    [InlineData("{\"a\":\"1\",\"b\":true}", false)]
    [InlineData("{\"a\":1,\"b\":\"true\"}", false)]
    [InlineData("{\"a\":1}", false)]
    [InlineData("{\"a\":1,\"b\":true,\"c\":0}", false)]
    [InlineData("{\"a\":1,\"a\":1,\"b\":true}", false)]
    [InlineData("{\"a\":1,\"b\":true,}", false)]
    public void JsonComparesContractAndTypesInsteadOfFormatting(string actual, bool correct)
        => Assert.Equal(correct, SoftwarePayload.Equivalent("Json", "{\"a\":1,\"b\":true}", actual));

    [Fact]
    public void ArrayOrderPayloadDepthAndHttpFieldsCannotBeBypassedByReasoning()
    {
        Assert.False(SoftwarePayload.Equivalent("Json", "[1,2]", "[2,1]"));
        Assert.False(SoftwarePayload.Equivalent("Json", "{}", new string('[', 20) + "0" + new string(']', 20)));
        var task = _catalog.Lessons[31]; var answer = Reference(task);
        answer.Payload = "{\"participants\":5.0,\"date\":\"2027-02-14\",\"roomId\":12}";
        Assert.True(SoftwareEvaluator.Evaluate(task, answer).Correct);
        answer.Values[task.Fields.First(field => field.Label.Contains("Methode")).Key] = "GET";
        answer.Reasoning = task.Explanation;
        Assert.False(SoftwareEvaluator.Evaluate(task, answer).Correct);
        Assert.False(SoftwareRules.HasAnswer(new() { Reasoning = task.Explanation }));
    }

    [Theory]
    [InlineData("<?xml version=\"1.0\"?><r b=\"2\" a=\"1\">\n <!--frei--> <x>12</x><y>true</y>\n</r>", true)]
    [InlineData("<r a=\"1\" b=\"2\"><y>true</y><x>12</x></r>", false)]
    [InlineData("<r a=\"1\" b=\"2\"><x>012</x><y>true</y></r>", false)]
    [InlineData("<r a=\"1\" b=\"2\"><x>12</x><y>True</y></r>", false)]
    [InlineData("<!DOCTYPE r [<!ENTITY e SYSTEM 'file:///C:/private.txt'>]><r a=\"1\" b=\"2\"><x>&e;</x><y>true</y></r>", false)]
    public void XmlPreservesHierarchyValuesAndOrderAndRejectsEntities(string actual, bool correct)
        => Assert.Equal(correct, SoftwarePayload.Equivalent("Xml", "<r a=\"1\" b=\"2\"><x>12</x><y>true</y></r>", actual));

    [Fact]
    public void XmlNamespacesAndDepthAreControlled()
    {
        Assert.True(SoftwarePayload.Equivalent("Xml", "<a:r xmlns:a='urn:x'><a:n>1</a:n></a:r>", "<b:r xmlns:b='urn:x'><b:n>1</b:n></b:r>"));
        Assert.False(SoftwarePayload.Equivalent("Xml", "<r/>", "<r xmlns='urn:other'/>"));
        Assert.False(SoftwarePayload.Equivalent("Xml", "<r/>", string.Concat(Enumerable.Repeat("<r>", 20)) + string.Concat(Enumerable.Repeat("</r>", 20))));
    }

    [Fact]
    public void ArchitectureAcceptsFreeNamesAndOrderButChecksDirectionsAndLayerRoles()
    {
        var task = _catalog.Lessons[25]; var answer = Reference(task);
        answer.Board.Nodes.Reverse(); answer.Board.Links.Reverse();
        foreach (var node in answer.Board.Nodes) node.Name = "Eigener " + node.Key;
        Assert.True(SoftwareEvaluator.Evaluate(task, answer).Correct);
        var link = answer.Board.Links[0]; (link.Source, link.Target) = (link.Target, link.Source);
        Assert.False(SoftwareEvaluator.Evaluate(task, answer).Correct);
        answer = Reference(task); answer.Board.Nodes[0].Group = task.Groups.Keys.Last();
        Assert.False(SoftwareEvaluator.Evaluate(task, answer).Correct);
        answer = Reference(task); answer.Board.Links.Add(new() { Source = answer.Board.Links[0].Source, Target = answer.Board.Links[0].Target, Kind = answer.Board.Links[0].Kind });
        Assert.False(SoftwareEvaluator.Evaluate(task, answer).Correct);
        answer = Reference(task); answer.Board.Nodes[1].Key = answer.Board.Nodes[0].Key;
        Assert.False(SoftwareEvaluator.Evaluate(task, answer).Correct);
    }

    [Fact]
    public void OwnFormChecksFieldTypesRequiredHintsAndKeyboardOrder()
    {
        var task = _catalog.Lessons[37]; var answer = Reference(task);
        answer.Board.Nodes[0].Name = "Deine Raumauswahl";
        Assert.True(SoftwareEvaluator.Evaluate(task, answer).Correct);
        answer.Board.Nodes.Reverse(); Assert.False(SoftwareEvaluator.Evaluate(task, answer).Correct);
        answer = Reference(task); answer.Board.Nodes[0].Required = false; Assert.False(SoftwareEvaluator.Evaluate(task, answer).Correct);
        answer = Reference(task); answer.Board.Nodes[0].Kind = "Text"; Assert.False(SoftwareEvaluator.Evaluate(task, answer).Correct);
        task = _catalog.Lessons[40]; answer = Reference(task); answer.Board.Nodes[0].Hint = "";
        Assert.False(SoftwareEvaluator.Evaluate(task, answer).Correct);
        answer.Board.Nodes[0].Hint = "Mein eigener verständlicher Hinweis.";
        Assert.True(SoftwareEvaluator.Evaluate(task, answer).Correct);
    }

    [Fact]
    public void ClassDesignRejectsWrongVisibilityAndRelationshipDirection()
    {
        var task = _catalog.Lessons[9]; var answer = Reference(task);
        var member = answer.Diagram.Model.Nodes.SelectMany(node => node.Members).First(item => item.Visibility == "-");
        member.Visibility = "+";
        Assert.False(SoftwareEvaluator.Evaluate(task, answer).Correct);
        task = _catalog.Lessons[13]; answer = Reference(task);
        var link = answer.Diagram.Model.Links[0]; (link.Source, link.Target) = (link.Target, link.Source);
        Assert.False(SoftwareEvaluator.Evaluate(task, answer).Correct);
    }

    [Fact]
    public void IncompleteDraftsRemainStorableButDamageAndOversizeDoNot()
    {
        var answer = new SoftwareAnswer { Board = new() { Nodes = [new()] } };
        Assert.True(SoftwareRules.Storable(answer));
        answer.Board.Nodes.Add(new() { Id = answer.Board.Nodes[0].Id }); Assert.False(SoftwareRules.Storable(answer));
        Assert.False(SoftwareRules.Storable(new() { Board = null! }));
        Assert.False(SoftwareRules.Storable(new() { Payload = new string('x', 8001) }));
        Assert.False(SoftwareRules.Storable(new() { Reasoning = new string('x', 4001) }));
        Assert.False(SoftwareRules.Storable(new() { Board = new() { Nodes = Enumerable.Range(0, 17).Select(_ => new SoftwareNode()).ToList() } }));
        var copy = answer.Copy(); copy.Board.Nodes[0].Name = "Kopie"; Assert.NotEqual(copy.Board.Nodes[0].Name, answer.Board.Nodes[0].Name);
    }

    [Fact]
    public async Task LegacyModulesAndRevisionsSurviveOwnDraftsAndIndependentAchievements()
    {
        Directory.CreateDirectory(_directory);
        await File.WriteAllTextAsync(Path.Combine(_directory, "progress.json"), """
            {"LastLessonId":17,"Lessons":{"17":{"Sql":"SELECT 1;","Completed":true}},
             "WorkshopLessons":{"4":{"Sql":"SELECT 2;","Attempts":1}},
             "LastAlgorithmLessonId":8,"AlgorithmLessons":{"8":{"Draft":"AUSGABE 123","Completed":true}},
             "BusinessQuestions":{"7":{"Attempts":2,"Completed":true,"Independent":true}},
             "LastDesignLessonId":37,"DesignLessons":{"37":{"ContentVersion":"2026-11-06","Completed":true}},
             "LastUmlLessonId":12,"UmlLessons":{"12":{"ContentVersion":"2026-11-08","Completed":true}}}
            """);
        var store = Store(); var task = _catalog.Lessons[31];
        var progress = new SoftwareProgress { Answer = Reference(task), Completed = true, Independent = true, Attempts = 100 };
        await store.SaveSoftwareAsync(task.Id, progress, _catalog);
        Assert.False(progress.Completed); Assert.False(progress.Independent); Assert.Equal(0, progress.Attempts);
        Assert.True((await store.SaveSoftwareAsync(task.Id, progress, _catalog, true))!.Correct);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveSoftwareAsync(task.Id, new(), _catalog));
        progress.Answer = new() { Reasoning = "Mein unfertiger neuer Ansatz" }; await store.SaveSoftwareAsync(task.Id, progress, _catalog);
        var state = await Store().LoadAsync();
        Assert.True(state.SoftwareLessons[task.Id].Independent); Assert.Equal(1, state.SoftwareLessons[task.Id].Attempts);
        Assert.Equal("Mein unfertiger neuer Ansatz", state.SoftwareLessons[task.Id].Answer.Reasoning);
        Assert.Equal("SELECT 1;", state.Lessons[17].Sql); Assert.Equal("SELECT 2;", state.WorkshopLessons[4].Sql);
        Assert.Equal("AUSGABE 123", state.AlgorithmLessons[8].Draft); Assert.True(state.BusinessQuestions[7].Independent);
        Assert.True(state.DesignLessons[37].Completed); Assert.True(state.UmlLessons[12].Completed);
        Assert.Equal(8, state.LastAlgorithmLessonId); Assert.Equal(37, state.LastDesignLessonId); Assert.Equal(12, state.LastUmlLessonId);
        progress.ContentVersion = "alt"; await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveSoftwareAsync(task.Id, progress, _catalog));
    }

    [Theory]
    [InlineData("radverleih")] [InlineData("kurse")] [InlineData("reparatur")]
    public async Task ExamsScoreFiftyAndFinalAnswersAndHistoryAreImmutable(string caseId)
    {
        var store = Store(); var round = new SoftwareExamRound { CaseId = caseId };
        await store.StartSoftwareExamAsync(round, _catalog);
        foreach (var task in _catalog.Cases.Single(item => item.Id == caseId).Tasks) round.Answers[task.Id] = Reference(task);
        round = await store.SaveSoftwareExamAsync(round, _catalog, true);
        Assert.Equal(50, round.Earned); Assert.Single((await Store().LoadAsync()).SoftwareHistory);
        round.Answers.Clear();
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveSoftwareExamAsync(round, _catalog, true));
        Assert.Equal(3, (await Store().LoadAsync()).SoftwareExam!.Answers.Count);
        Assert.Single((await Store().LoadAsync()).SoftwareHistory);
    }

    [Fact]
    public async Task PauseRestartStaleWindowsAndDeadlineUseSavedUtcBudget()
    {
        var store = Store(); var round = new SoftwareExamRound { CaseId = "radverleih", RunningSinceUtc = DateTimeOffset.UtcNow.AddSeconds(-80) };
        await store.StartSoftwareExamAsync(round, _catalog);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.StartSoftwareExamAsync(new() { CaseId = "kurse" }, _catalog));
        round.Pause(DateTimeOffset.UtcNow); round.FlaggedIds.Add(2); round.UsedReference = true;
        round = await store.SaveSoftwareExamAsync(round, _catalog);
        Assert.InRange(round.RemainingSeconds, 2618, 2620); Assert.Null((await Store().LoadAsync()).SoftwareExam!.RunningSinceUtc);
        var stale = round.Copy(); round.RemainingSeconds = 2700; round.RunningSinceUtc = DateTimeOffset.UtcNow; round.UsedReference = false;
        round = await store.SaveSoftwareExamAsync(round, _catalog);
        Assert.InRange(round.SecondsLeft(DateTimeOffset.UtcNow), 2617, 2620); Assert.True(round.UsedReference);
        Assert.Contains(2, round.FlaggedIds);
        await Assert.ThrowsAsync<InvalidOperationException>(() => store.SaveSoftwareExamAsync(stale, _catalog));
        var expired = new SoftwareExamRound { CaseId = "kurse", RunningSinceUtc = DateTimeOffset.UtcNow.AddMinutes(-46) };
        await store.StartSoftwareExamAsync(expired, _catalog, round.Id);
        expired.Answers[1] = Reference(_catalog.Cases[1].Tasks[0]); expired.Pause(DateTimeOffset.UtcNow);
        var submitted = await store.SaveSoftwareExamAsync(expired, _catalog);
        Assert.NotNull(submitted.FinishedUtc); Assert.Empty(submitted.Answers); Assert.Equal(0, submitted.Earned);
    }

    private static SoftwareAnswer Reference(SoftwareAssignment task) => task.Kind == "Choice" ? new() { SelectedIds = task.CorrectIds.ToList() } : task.Solutions[0].Copy();
    private ProgressStore Store() => new(new EnvironmentStub(), new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["Training:ProgressDirectory"] = _directory }).Build());
    public void Dispose() { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
    private sealed class EnvironmentStub : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests"; public string EnvironmentName { get; set; } = "Tests"; public string ContentRootPath { get; set; } = ""; public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider(); public string WebRootPath { get; set; } = ""; public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
