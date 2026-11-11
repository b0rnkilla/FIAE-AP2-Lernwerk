using System.Text.Json;
using FiaeTrainer.Core.ProjectManagement.Evaluation;
using FiaeTrainer.Core.ProjectManagement.Models;

namespace FiaeTrainer.Core.ProjectManagement.Content;

/// <summary> Eigene Projektaufträge mit geprüften Referenzen und eindeutigen Zeit-/Geldverträgen. </summary>
public sealed class ProjectCatalog
{
    /// <summary> Neue Profilfelder beginnen mit einem eigenständigen Inhaltsvertrag. </summary>
    public const string CONTENT_VERSION = "2026-11-11";
    /// <summary> Neun Kapitel mit je sechs eigenen Lernaufgaben. </summary>
    public IReadOnlyList<ProjectAssignment> Lessons { get; } = Read<List<ProjectAssignment>>("lessons.json");
    /// <summary> Drei unbekannte Fälle mit drei unabhängig bearbeitbaren Teilen. </summary>
    public IReadOnlyList<ProjectExamCase> Cases { get; } = Read<List<ProjectExamCase>>("exams.json");
    /// <summary> Gemeinsame Bezeichnungen der Arbeitsformen. </summary>
    public static readonly Dictionary<string, string> Kinds = new() { ["Choice"] = "Entscheiden", ["Review"] = "Plan prüfen", ["Worksheet"] = "Ausarbeiten", ["Structure"] = "Projektstrukturplan", ["Network"] = "Netzplan", ["Schedule"] = "Gantt-Werkstatt" };

    /// <summary> Verhindert widersprüchliche Referenzen und beschädigte Planverträge bereits beim Laden. </summary>
    public ProjectCatalog()
    {
        if (!Lessons.Select(task => task.Id).SequenceEqual(Enumerable.Range(1, 54)) || Lessons.GroupBy(task => task.Chapter).Count() != 9 || Lessons.GroupBy(task => task.Chapter).Any(group => group.Count() != 6) || Cases.Count != 3 || Cases.Select(item => item.Id).Distinct().Count() != 3 || Cases.Any(item => !item.Points.SequenceEqual([20, 20, 10]) || !item.Tasks.Select(task => task.Id).SequenceEqual([1, 2, 3]))) throw new InvalidDataException("Der Projektkatalog ist unvollständig.");
        foreach (var task in Lessons.Concat(Cases.SelectMany(item => item.Tasks)))
        {
            if (!Kinds.ContainsKey(task.Kind) || task.Format is not ("Build" or "Repair" or "Analyze") || task.Hints.Count != 3 || string.IsNullOrWhiteSpace(task.Story) || string.IsNullOrWhiteSpace(task.Task) || string.IsNullOrWhiteSpace(task.Explanation) || task.Fields.Count > 64 || task.Fields.Select(field => field.Key).Distinct().Count() != task.Fields.Count || task.Fields.Any(field => field.Kind is not ("Select" or "Number" or "Text") || field.Comparison is not ("Exact" or "Text" or "Number") || field.Kind == "Select" && field.Options.Count == 0) || !ProjectRules.Storable(task.InitialAnswer())) throw new InvalidDataException($"Projektauftrag {task.Id} ist unvollständig.");
            if (task.Kind is "Structure" or "Network" or "Schedule") ValidatePlan(task.Plan ?? throw new InvalidDataException("Der Planvertrag fehlt."));
            else if (task.Plan != null) throw new InvalidDataException("Ein reiner Antwortauftrag darf keinen Planbaukasten enthalten.");
            if (task.Kind is "Choice" or "Review")
            {
                if (task.CorrectIds.Count == 0 || task.CorrectIds.Distinct().Count() != task.CorrectIds.Count || task.CorrectIds.Any(id => !task.Options.ContainsKey(id))) throw new InvalidDataException("Die Auswahlreferenz fehlt.");
            }
            else if (task.Solutions.Count == 0 || task.Solutions.Any(answer => !ProjectRules.Storable(answer) || task.Fields.Any(field => !answer.Values.ContainsKey(field.Key) || field.Kind == "Select" && !field.Options.ContainsKey(answer.Values[field.Key])) || !ProjectEvaluator.Evaluate(task, answer).Correct)) throw new InvalidDataException($"Die Projektreferenz {task.Id} ({task.Title}) ist widersprüchlich.");
            if (task.Format == "Repair" && ProjectEvaluator.Evaluate(task, task.InitialAnswer()).Correct) throw new InvalidDataException("Die Reparaturvorlage ist bereits gelöst.");
        }
        if (Lessons.Any(task => string.IsNullOrWhiteSpace(task.Discovery) || task.Id > 1 && string.IsNullOrWhiteSpace(task.PreviousContext))) throw new InvalidDataException("Die Nordhafen-Geschichte ist unvollständig.");
    }
    private static void ValidatePlan(ProjectPlan plan)
    {
        var roles = plan.Activities.Select(item => item.Key).ToHashSet();
        if (plan.Activities.Count is < 1 or > 16 || roles.Count != plan.Activities.Count || plan.Horizon is < 1 or > 60 || plan.Deadline < 0 || plan.Deadline > plan.Horizon || plan.RequireEarliest && plan.Resources.Count > 0 || plan.TimingFields.Distinct().Count() != plan.TimingFields.Count || plan.TimingFields.Any(key => !ProjectEvaluator.TimeLabels.ContainsKey(key)) || !plan.Resources.Keys.ToHashSet().SetEquals(plan.Capacities.Keys) || plan.Capacities.Values.Any(capacity => capacity is < 1 or > 8) || plan.Activities.Any(item => string.IsNullOrWhiteSpace(item.Key) || item.Kind is not ("Group" or "Work" or "Milestone") || item.Duration is < 0 or > 60 || item.Kind == "Milestone" && item.Duration != 0 || item.ParentKey.Length > 0 && !roles.Contains(item.ParentKey) || item.Resource.Length > 0 && !plan.Resources.ContainsKey(item.Resource))) throw new InvalidDataException("Ein Planvertrag besitzt unzulässige Rollen oder Grenzen.");
        ProjectScheduler.Calculate(plan.Activities);
        foreach (var item in plan.Activities)
        {
            HashSet<string> visited = []; var current = item;
            while (current.ParentKey != "")
            {
                if (!visited.Add(current.Key)) throw new InvalidDataException("Die Projektstruktur enthält einen Kreis.");
                current = plan.Activities.Single(other => other.Key == current.ParentKey);
            }
        }
    }
    private static T Read<T>(string name)
    {
        using var stream = typeof(ProjectCatalog).Assembly.GetManifestResourceStream("FiaeTrainer.Core.ProjectManagement.Content." + name) ?? throw new InvalidDataException("Der Projektinhalt fehlt.");
        return JsonSerializer.Deserialize<T>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new InvalidDataException("Der Projektinhalt ist leer.");
    }
}
