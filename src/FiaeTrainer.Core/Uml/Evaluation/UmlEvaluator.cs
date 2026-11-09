using FiaeTrainer.Core.Uml.Content;
using FiaeTrainer.Core.Uml.Models;

namespace FiaeTrainer.Core.Uml.Evaluation;

/// <summary> Vergleicht Fachrollen und UML-Zusammenhänge mit expliziten Varianten, niemals Zeichenkoordinaten. </summary>
public static class UmlEvaluator
{
    /// <summary> Leseantworten verlangen die richtige Menge; eigene Modelle die vollständig passende Fachstruktur. </summary>
    public static UmlEvaluation Evaluate(UmlAssignment task, UmlAnswer answer)
    {
        if (!UmlRules.Storable(answer)) return Failure("Der Entwurf ist beschädigt oder überschreitet die Grenzen des Baukastens.");
        if (task.Format == "Read")
        {
            bool correct = answer.SelectedIds.Count == answer.SelectedIds.Distinct().Count() && answer.SelectedIds.Order().SequenceEqual(task.CorrectIds.Order());
            return new() { Checks = [new() { Name = "Diagramm verstehen", Maximum = 50, Earned = correct ? 50 : 0, Detail = correct ? "Deine Auswahl erfüllt den Auftrag." : "Deine Auswahl passt noch nicht vollständig. Lies die Darstellung und die Aussagen erneut." }] };
        }
        if (answer.Model.Nodes.Count == 0) return Failure("Lege zunächst die benötigten Diagrammelemente an.");
        var errors = UmlRules.Errors(task, answer.Model);
        if (errors.Count > 0) return new() { Checks = [new() { Name = "Entwurf vervollständigen", Maximum = 50, Detail = string.Join(" ", errors.Select(issue => issue.Text).Distinct().Take(4)) }], Issues = errors };
        return task.Solutions.Select(expected => Compare(task, expected, answer.Model)).OrderByDescending(result => result.Earned).First();
    }

    /// <summary> Drei getrennte Kriterien gewichten Elemente, Verbindungen und Rahmung auf 20/25/5. </summary>
    private static UmlEvaluation Compare(UmlAssignment task, UmlModel expected, UmlModel actual)
    {
        List<UmlIssue> issues = [];
        int nodes = 0;
        foreach (var wanted in expected.Nodes)
        {
            var found = actual.Nodes.SingleOrDefault(node => node.Key == wanted.Key);
            if (found != null && NodeSignature(task, found) == NodeSignature(task, wanted)) nodes++;
            else issues.Add(new() { NodeId = found?.Id ?? "", Text = $"Prüfe {task.NodeVocabulary.GetValueOrDefault(wanted.Key, wanted.Name)}: Elementart, Merkmale, Sichtbarkeit, Typen und gegebenenfalls Verantwortungsbereich müssen zum Auftrag passen." });
        }
        foreach (var extra in actual.Nodes.Where(node => expected.Nodes.All(wanted => wanted.Key != node.Key))) issues.Add(new() { NodeId = extra.Id, Text = "Dieses zusätzliche Element wird im beschriebenen Ausschnitt nicht benötigt." });
        int links = 0; var unused = actual.Links.ToList();
        for (int i = 0; i < expected.Links.Count; i++)
        {
            string signature = LinkSignature(task, expected, expected.Links[i]);
            var found = task.DiagramType == "Sequence" ? actual.Links.ElementAtOrDefault(i) : unused.FirstOrDefault(link => LinkSignature(task, actual, link) == signature);
            if (found != null && LinkSignature(task, actual, found) == signature) { links++; unused.Remove(found); }
            else
            {
                var wanted = expected.Links[i];
                string source = task.NodeVocabulary[expected.Nodes.Single(node => node.Id == wanted.Source).Key], target = task.NodeVocabulary[expected.Nodes.Single(node => node.Id == wanted.Target).Key];
                issues.Add(new() { LinkId = task.DiagramType == "Sequence" ? found?.Id ?? "" : "", Text = task.DiagramType == "Sequence" ? $"Nachricht {i + 1}: Prüfe Absender, Empfänger, Aufruf/Rückgabe, Bedingung, Gruppe und Reihenfolge für {source} → {target}." : $"Prüfe die Verbindung zwischen {source} und {target}: Art, Richtung, Mengenangaben und Bedingungen müssen die beschriebene Regel ausdrücken." });
            }
        }
        foreach (var extra in unused) issues.Add(new() { LinkId = extra.Id, Text = "Diese Verbindung ist zusätzlich vorhanden oder passt nicht zur geforderten Richtung, Art oder Bedingung." });
        bool frame = (task.DiagramType != "UseCase" || string.IsNullOrWhiteSpace(expected.SystemName) == string.IsNullOrWhiteSpace(actual.SystemName)) && Fragments(task, expected) == Fragments(task, actual);
        if (!frame) issues.Add(new() { Text = "Prüfe die beschriftete Systemgrenze beziehungsweise Art und Bedingung der benötigten Sequenzgruppen." });
        return new() { Checks = [Check("Elemente und Eigenschaften", nodes, Math.Max(expected.Nodes.Count, actual.Nodes.Count), 20, issues.Where(issue => issue.LinkId.Length == 0).Select(issue => issue.Text)), Check("Verbindungen und Abläufe", links, Math.Max(expected.Links.Count, actual.Links.Count), 25, issues.Where(issue => issue.LinkId.Length > 0 || issue.Text.StartsWith("Prüfe die Verbindung") || issue.Text.StartsWith("Nachricht")).Select(issue => issue.Text)), new() { Name = "Rahmen und Gruppierung", Maximum = 5, Earned = frame ? 5 : 0, Detail = frame ? "Die Rahmung passt zum Auftrag." : "Ergänze oder korrigiere Systemgrenze und Sequenzgruppen." }], Issues = issues };
    }

    /// <summary> Eigene Namen und Kennungen bleiben frei; Fachrolle und technische Merkmale zählen. </summary>
    private static string NodeSignature(UmlAssignment task, UmlNode node) => string.Join("|", node.Key, node.Kind, node.Abstract, task.DiagramType == "Activity" ? node.Lane : "", string.Join(";", node.Members.Select(member => MemberSignature(task, member, node.Kind == "Interface")).Order(StringComparer.Ordinal)));
    /// <summary> Eine explizite abstrakt-Markierung ändert den impliziten Vertrag der hier verwendeten Interface-Operationen nicht. </summary>
    private static string MemberSignature(UmlAssignment task, UmlMember member, bool isInterface) => task.DiagramType == "State" ? member.Key + ":" + member.Kind : string.Join(":", member.Key, member.Kind, UmlRules.NormalizeType(member.Type), member.Visibility, member.Static, member.Abstract && !isInterface, member.Kind == "Method" ? string.Join(",", member.Parameters.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(part => UmlRules.NormalizeType(part.Split(':').Last()))) : "");
    /// <summary> Ungerichtete Assoziationen dürfen mit vertauschten Enden und entsprechend vertauschten Mengen entstehen. </summary>
    private static string LinkSignature(UmlAssignment task, UmlModel model, UmlLink link)
    {
        string source = model.Nodes.Single(node => node.Id == link.Source).Key, target = model.Nodes.Single(node => node.Id == link.Target).Key;
        string first = task.DiagramType == "Class" && link.Kind is "Association" or "Aggregation" or "Composition" ? UmlRules.NormalizeMultiplicity(link.SourceMultiplicity) : "", second = task.DiagramType == "Class" && link.Kind is "Association" or "Aggregation" or "Composition" ? UmlRules.NormalizeMultiplicity(link.TargetMultiplicity) : "";
        if (link.Kind == "Association" && string.CompareOrdinal(source, target) > 0) { (source, target) = (target, source); (first, second) = (second, first); }
        string fragment = model.Fragments.SingleOrDefault(item => item.Id == link.Fragment)?.Key ?? "";
        return string.Join("|", source, target, link.Kind, first, second, task.DiagramType is "Sequence" or "State" ? link.Meaning : "", task.DiagramType is "Activity" or "Sequence" or "State" ? link.Guard : "", fragment);
    }
    private static string Fragments(UmlAssignment task, UmlModel model) => string.Join(";", model.Fragments.Select(fragment => string.Join("|", fragment.Key, fragment.Kind, fragment.Guard)).Order(StringComparer.Ordinal));
    private static UmlCheck Check(string name, int count, int total, int maximum, IEnumerable<string> errors) => new() { Name = name, Maximum = maximum, Earned = total == 0 ? maximum : maximum * count / total, Detail = count == total ? "Dieser Teil des Auftrags ist erfüllt." : string.Join(" ", errors.Distinct().Take(3)) is var text && text.Length > 0 ? text : "Ergänze die fehlenden Elemente oder entferne unzulässige Ergänzungen." };
    private static UmlEvaluation Failure(string text) => new() { Checks = [new() { Name = "Auftrag vervollständigen", Maximum = 50, Detail = text }], Issues = [new() { Text = text }] };
}
