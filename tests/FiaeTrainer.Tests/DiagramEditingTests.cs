using System.Reflection;
using System.Text.Json;
using FiaeTrainer.Core.DatabaseDesign.Content;
using FiaeTrainer.Core.DatabaseDesign.Evaluation;
using FiaeTrainer.Core.DatabaseDesign.Models;
using FiaeTrainer.Core.Diagrams;
using FiaeTrainer.Core.Uml.Content;
using FiaeTrainer.Core.Uml.Evaluation;
using FiaeTrainer.Core.Uml.Models;
using FiaeTrainer.Web.Components.Diagrams;
using FiaeTrainer.Web.Components.Uml;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft freie Beschriftungen, alte Fachkennungen und die vom Fachmodell getrennte direkte Bearbeitung. </summary>
public sealed class DiagramEditingTests
{
    [Fact]
    public void EveryUmlReferenceCanBeEnteredByNameWithoutHiddenRoleSelection()
    {
        UmlCatalog catalog = new();
        foreach (var task in catalog.Lessons.Concat(catalog.Cases.SelectMany(item => item.Tasks)).Where(task => task.Format != "Read"))
            foreach (var expected in task.Solutions)
            {
                var model = expected.Copy();
                foreach (var node in model.Nodes)
                {
                    node.Key = ""; node.Name = UmlNames.Structural(node.Kind) ? "" : "  " + node.Name.ToUpperInvariant() + "  ";
                    node.Lane = task.Lanes.GetValueOrDefault(node.Lane, node.Lane);
                    foreach (var member in node.Members) { member.Key = ""; member.Name = member.Name.ToUpperInvariant(); }
                }
                foreach (var link in model.Links) { link.Meaning = ""; link.Guard = task.Conditions.GetValueOrDefault(link.Guard, link.Guard); }
                foreach (var fragment in model.Fragments) { fragment.Key = ""; fragment.Guard = task.Conditions.GetValueOrDefault(fragment.Guard, fragment.Guard); }
                string before = JsonSerializer.Serialize(model);
                var result = UmlEvaluator.Evaluate(task, new() { Model = model });
                Assert.True(result.Correct, $"{task.Title}: {string.Join("; ", result.Issues.Select(issue => issue.Text))}");
                Assert.Equal(before, JsonSerializer.Serialize(model));
            }
    }

    [Fact]
    public void EveryRelationalReferenceCanBeEnteredWithFreeAttributeNames()
    {
        DesignCatalog catalog = new();
        foreach (var task in catalog.Lessons.Concat(catalog.Cases.SelectMany(item => item.Tasks)).Where(task => task.Kind == "Model"))
            foreach (var expected in task.Solutions)
            {
                var answer = new DesignAnswer { Model = expected.Copy() };
                foreach (var column in answer.Model.Tables.SelectMany(table => table.Columns)) { column.Attribute = ""; column.Name = " " + column.Name.ToUpperInvariant() + " "; }
                string before = JsonSerializer.Serialize(answer);
                Assert.True(DesignEvaluator.Evaluate(task, answer).Correct, task.Title);
                Assert.Equal(before, JsonSerializer.Serialize(answer));
            }
    }

    [Fact]
    public void ErSlotsAcceptTypedLabelsButNotInventedCardinalities()
    {
        foreach (var task in new DesignCatalog().Lessons.Where(task => task.Kind == "ErCompletion"))
        {
            var answer = task.InitialAnswer();
            answer.DiagramValues = task.DiagramSlots.ToDictionary(slot => slot.Id, slot => slot.Options[slot.CorrectId].ToUpperInvariant());
            Assert.True(DesignEvaluator.Evaluate(task, answer).Correct, task.Title);
            answer.DiagramValues[task.DiagramSlots[0].Id] = "99..100";
            Assert.False(DesignEvaluator.Evaluate(task, answer).Correct);
        }
        var first = new DesignCatalog().Lessons[30];
        Assert.True(DesignEvaluator.Evaluate(first, new() { DiagramValues = new() { ["A"] = "1", ["B"] = "0..*" } }).Correct);
    }

    [Fact]
    public void SoftwareClassDiagramsAlsoAcceptNamesAndKeepObjectStateNotation()
    {
        var catalog = new FiaeTrainer.Core.SoftwareDesign.Content.SoftwareCatalog();
        foreach (var task in catalog.Lessons.Where(task => task.Kind == "Diagram"))
        {
            var answer = task.Solutions[0].Copy();
            foreach (var node in answer.Diagram.Model.Nodes)
            {
                node.Key = "";
                foreach (var member in node.Members) { member.Key = ""; if (member.Kind == "Method") member.Name += "()"; }
            }
            Assert.True(FiaeTrainer.Core.SoftwareDesign.Evaluation.SoftwareEvaluator.Evaluate(task, answer).Correct, task.Title);
        }
        var capacity = catalog.Lessons[10];
        Assert.Contains("FUNKTION setCapacity(neu: Integer): Boolean", capacity.Material);
        Assert.Contains("capacity = 12", capacity.Material);
        Assert.Contains("ENDE WENN", capacity.Material);
        Assert.Contains("RÜCKGABE FALSCH", capacity.Material);
        Assert.DoesNotContain("true", capacity.Material);
    }

    [Fact]
    public void ExistingCustomDisplayNamesRemainValidButNewUnknownNamesFail()
    {
        var task = new UmlCatalog().Lessons[12]; var model = task.Solutions[0].Copy();
        model.Nodes[0].Name = "Mein frei benannter Altentwurf";
        Assert.True(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        model.Nodes[0].Key = "";
        Assert.False(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        Assert.NotEqual(DiagramNames.Normalize("x > 3"), DiagramNames.Normalize("x < 3"));
        Assert.Equal("schluessel", DiagramNames.Normalize(" Schlüssel "));
    }

    [Fact]
    public void LayoutAndHistoryRemainIndependentFromEvaluationAndCopies()
    {
        var task = new UmlCatalog().Lessons[4]; var model = task.Solutions[0].Copy();
        var history = new DiagramHistory<UmlModel>(); history.Reset(model);
        model.Positions[model.Nodes[0].Id] = new(900, 700); history.Record(model);
        Assert.True(UmlEvaluator.Evaluate(task, new() { Model = model }).Correct);
        var previous = history.Move(-1)!; Assert.Empty(previous.Positions);
        var restored = history.Move(1)!; Assert.Equal(new(900, 700), restored.Positions[model.Nodes[0].Id]);
        restored.Positions.Clear(); Assert.NotEmpty(model.Positions);
        history.Move(-1); previous.SystemName = "Neue Richtung"; history.Record(previous); Assert.False(history.CanRedo);
        model.Positions[model.Nodes[0].Id] = new(-1, 0); Assert.False(UmlRules.Storable(new() { Model = model }));
    }

    [Fact]
    public async Task CanvasGesturesRenameMoveConnectUndoAndRespectReadOnly()
    {
        var task = new UmlCatalog().Lessons[4]; var answer = new UmlAnswer { Model = task.Solutions[0].Copy() };
#pragma warning disable BL0005
        var editor = new UmlEditor { Task = task, Answer = answer };
#pragma warning restore BL0005
        typeof(UmlEditor).GetMethod("OnParametersSet", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(editor, null);
        async Task Gesture(DiagramAction action) => await (Task)typeof(UmlEditor).GetMethod("CanvasAsync", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(editor, [action])!;
        string id = answer.Model.Nodes[0].Id;
        await Gesture(new() { Kind = "rename", Id = id, Text = "besucher" });
        Assert.Empty(answer.Model.Nodes[0].Key); Assert.True(UmlEvaluator.Evaluate(task, answer).Correct);
        await Gesture(new() { Kind = "move", Id = id, X = 150, Y = 60 }); Assert.True(answer.Model.Positions.ContainsKey(id));
        await Gesture(new() { Kind = "undo" }); Assert.Empty(answer.Model.Positions);
        await Gesture(new() { Kind = "redo" }); Assert.NotEmpty(answer.Model.Positions);
        await Gesture(new() { Kind = "connect", Id = id, Target = answer.Model.Nodes[1].Id }); Assert.Equal(2, answer.Model.Links.Count);
#pragma warning disable BL0005
        editor.ReadOnly = true;
#pragma warning restore BL0005
        string before = JsonSerializer.Serialize(answer);
        foreach (string kind in new[] { "delete", "move", "rename", "connect", "undo", "redo", "layout" }) await Gesture(new() { Kind = kind, Id = id, Target = id, Text = "Verändert", X = 40 });
        Assert.Equal(before, JsonSerializer.Serialize(answer));
    }
}
