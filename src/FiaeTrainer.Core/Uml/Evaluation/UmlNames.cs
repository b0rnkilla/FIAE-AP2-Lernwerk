using FiaeTrainer.Core.Diagrams;
using FiaeTrainer.Core.Uml.Content;
using FiaeTrainer.Core.Uml.Models;

namespace FiaeTrainer.Core.Uml.Evaluation;

/// <summary> Übersetzt freie Eingaben für die Bewertung, ohne gespeicherte Altentwürfe umzuschreiben. </summary>
public static class UmlNames
{
    /// <summary> Unbeschriftete technische Knoten benötigen keine künstliche Fachrolle. </summary>
    public static bool Structural(string kind) => kind is "Initial" or "Final" or "Fork" or "Join" or "Merge";

    /// <summary> Erhält alte Fachkennungen; neue Eingaben werden ausschließlich nach bekannten Namen aufgelöst. </summary>
    public static UmlModel Resolve(UmlAssignment task, UmlModel source)
    {
        var model = source.Copy();
        string ResolveText(string value, Dictionary<string, string> vocabulary) => vocabulary.ContainsKey(value) ? value : DiagramNames.Resolve(value, vocabulary, task.NameAliases) is { Length: > 0 } found ? found : value;
        foreach (var node in model.Nodes)
        {
            if (node.Key.Length == 0)
            {
                node.Key = DiagramNames.Resolve(node.Name, task.NodeVocabulary, task.NameAliases);
                if (Structural(node.Kind))
                {
                    var keys = task.Solutions.SelectMany(solution => solution.Nodes).Where(item => item.Kind == node.Kind).Select(item => item.Key).Distinct().ToArray();
                    if (keys.Length == 1) node.Key = keys[0];
                }
            }
            if (Structural(node.Kind) && node.Name.Length == 0) node.Name = UmlKinds.Nodes[node.Kind];
            node.Lane = ResolveText(node.Lane, task.Lanes);
            foreach (var member in node.Members)
                if (member.Key.Length == 0)
                {
                    string name = member.Kind == "Method" && member.Name.TrimEnd().EndsWith("()", StringComparison.Ordinal) ? member.Name.TrimEnd()[..^2] : member.Name;
                    member.Key = DiagramNames.Resolve(name, task.MemberVocabulary, task.NameAliases);
                }
        }
        foreach (var link in model.Links)
        {
            if (link.Meaning.Length == 0 && link.Name.Length > 0) link.Meaning = ResolveText(link.Name, task.DiagramType == "State" ? task.Events : task.Messages);
            link.Guard = ResolveText(link.Guard, task.Conditions);
        }
        foreach (var fragment in model.Fragments)
        {
            if (!task.FragmentVocabulary.ContainsKey(fragment.Key))
            {
                var keys = task.Solutions.SelectMany(solution => solution.Fragments).Where(item => item.Kind == fragment.Kind).Select(item => item.Key).Distinct().ToArray();
                if (keys.Length == 1) fragment.Key = keys[0];
            }
            fragment.Guard = ResolveText(fragment.Guard, task.Conditions);
        }
        return model;
    }
}
