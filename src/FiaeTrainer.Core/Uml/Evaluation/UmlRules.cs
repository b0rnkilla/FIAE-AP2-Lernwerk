using FiaeTrainer.Core.Uml.Content;
using FiaeTrainer.Core.Uml.Models;

namespace FiaeTrainer.Core.Uml.Evaluation;

/// <summary> Begrenzt speicherbare Entwürfe und prüft die ausdrücklich unterstützte UML-Teilmenge. </summary>
public static class UmlRules
{
    /// <summary> Unvollständige Eingaben bleiben speicherbar; nullbehaftete und übergroße Strukturen nicht. </summary>
    public static bool Storable(UmlAnswer? answer)
    {
        if (answer?.Model is not { } model || !Text(model.SystemName, 80) || answer.SelectedIds == null || answer.SelectedIds.Count > 20 || answer.SelectedIds.Any(id => !Text(id, 80)) || model.Nodes == null || model.Links == null || model.Fragments == null || model.Nodes.Count > 16 || model.Links.Count > 32 || model.Fragments.Count > 4) return false;
        if (model.Nodes.Any(node => node == null || !Id(node.Id) || !Text(node.Key, 80) || !Text(node.Name, 80) || !UmlKinds.Nodes.ContainsKey(node.Kind ?? "") || !Text(node.Lane, 80) || node.Members == null || node.Members.Count > 16 || node.Members.Any(member => member == null || !Id(member.Id) || !Text(member.Key, 80) || !Text(member.Name, 80) || member.Kind is not ("Attribute" or "Method" or "Entry" or "Do" or "Exit") || !Text(member.Type, 80) || !Text(member.Parameters, 250) || member.Visibility is not ("+" or "-" or "#" or "~")))) return false;
        if (model.Nodes.Select(node => node.Id).Distinct().Count() != model.Nodes.Count || model.Nodes.SelectMany(node => node.Members).Select(member => member.Id).Distinct().Count() != model.Nodes.Sum(node => node.Members.Count)) return false;
        if (model.Links.Any(link => link == null || !Id(link.Id) || !Text(link.Source, 80) || !Text(link.Target, 80) || !UmlKinds.Links.ContainsKey(link.Kind ?? "") || !Text(link.Meaning, 80) || !Text(link.Name, 80) || !Text(link.SourceMultiplicity, 12) || !Text(link.TargetMultiplicity, 12) || !Text(link.Guard, 80) || !Text(link.Fragment, 80)) || model.Links.Select(link => link.Id).Distinct().Count() != model.Links.Count) return false;
        return model.Fragments.All(fragment => fragment != null && Id(fragment.Id) && Text(fragment.Key, 80) && fragment.Kind is "alt" or "opt" or "loop" && Text(fragment.Guard, 80)) && model.Fragments.Select(fragment => fragment.Id).Distinct().Count() == model.Fragments.Count;
    }

    /// <summary> Prüft vollständige Fachzuordnung, Endpunkte und zum Diagramm passende Elementarten. </summary>
    public static List<UmlIssue> Errors(UmlAssignment task, UmlModel model)
    {
        if (!Storable(new() { Model = model })) return [new() { Text = "Der Entwurf enthält beschädigte oder zu viele Elemente." }];
        List<UmlIssue> errors = [];
        foreach (var node in model.Nodes)
        {
            if (!task.NodeVocabulary.ContainsKey(node.Key) || string.IsNullOrWhiteSpace(node.Name) || !UmlKinds.NodeTypes(task.DiagramType).Contains(node.Kind)) errors.Add(new() { NodeId = node.Id, Text = "Ergänze Fachrolle, Namen und passende Elementart." });
            if (model.Nodes.Count(item => item.Key == node.Key) > 1) errors.Add(new() { NodeId = node.Id, Text = "Diese Fachrolle ist mehrfach angelegt. Prüfe, ob ein einziges Element gemeint ist." });
            if (node.Lane.Length > 0 && (task.DiagramType != "Activity" || !task.Lanes.ContainsKey(node.Lane))) errors.Add(new() { NodeId = node.Id, Text = "Dieser Verantwortungsbereich gehört nicht zum Auftrag." });
            if (node.Abstract && node.Kind != "Class") errors.Add(new() { NodeId = node.Id, Text = "Die Markierung abstrakt ist hier nur für Klassen vorgesehen." });
            foreach (var member in node.Members)
            {
                bool classMember = task.DiagramType == "Class" && member.Kind is "Attribute" or "Method";
                bool stateAction = task.DiagramType == "State" && node.Kind == "State" && member.Kind is "Entry" or "Do" or "Exit";
                if (!task.MemberVocabulary.ContainsKey(member.Key) || string.IsNullOrWhiteSpace(member.Name) || !classMember && !stateAction || classMember && string.IsNullOrWhiteSpace(member.Type) || member.Kind == "Method" && !ValidParameters(member.Parameters)) errors.Add(new() { NodeId = node.Id, Text = "Prüfe Fachrolle, Art und Typ des Merkmals; Methodenparameter haben die Form name: Typ." });
                if (node.Members.Count(item => item.Key == member.Key) > 1) errors.Add(new() { NodeId = node.Id, Text = "Ein Merkmal derselben Fachrolle ist doppelt angelegt." });
            }
        }
        foreach (var fragment in model.Fragments)
            if (task.DiagramType != "Sequence" || !task.FragmentVocabulary.ContainsKey(fragment.Key) || model.Fragments.Count(item => item.Key == fragment.Key) > 1 || fragment.Guard.Length > 0 && !task.Conditions.ContainsKey(fragment.Guard)) errors.Add(new() { Text = "Prüfe Fachrolle und Bedingung des Sequenzfragments." });
        foreach (var link in model.Links)
        {
            var source = model.Nodes.SingleOrDefault(node => node.Id == link.Source); var target = model.Nodes.SingleOrDefault(node => node.Id == link.Target);
            if (source == null || target == null || !UmlKinds.LinkTypes(task.DiagramType).Contains(link.Kind) || link.Guard.Length > 0 && !task.Conditions.ContainsKey(link.Guard) || link.Fragment.Length > 0 && !model.Fragments.Any(fragment => fragment.Id == link.Fragment)) { errors.Add(new() { LinkId = link.Id, Text = "Wähle beide Endpunkte und eine passende Verbindung mit gültiger Bedingung und Gruppe." }); continue; }
            if (task.DiagramType == "Sequence" && (!task.Messages.ContainsKey(link.Meaning) || string.IsNullOrWhiteSpace(link.Name)) || task.DiagramType == "State" && link.Meaning.Length > 0 && (!task.Events.ContainsKey(link.Meaning) || string.IsNullOrWhiteSpace(link.Name))) errors.Add(new() { LinkId = link.Id, Text = "Ordne Nachricht oder Ereignis einer angebotenen Fachbedeutung zu und beschrifte es." });
            if (task.DiagramType == "UseCase" && (link.Kind is "Include" or "Extend" && (source.Kind != "UseCase" || target.Kind != "UseCase") || link.Kind == "Generalization" && source.Kind != target.Kind || link.Kind == "Association" && !(source.Kind == "Actor" && target.Kind == "UseCase" || source.Kind == "UseCase" && target.Kind == "Actor"))) errors.Add(new() { LinkId = link.Id, Text = "Diese Beziehung passt nicht zu den beteiligten Akteuren oder Anwendungsfällen." });
            if (task.DiagramType == "Class" && (link.Kind == "Realization" && (source.Kind != "Class" || target.Kind != "Interface") || link.Kind == "Generalization" && source.Kind != target.Kind)) errors.Add(new() { LinkId = link.Id, Text = "Prüfe Ausgang und Ziel von Vererbung oder Interface-Realisierung." });
            if (task.DiagramType == "Class" && link.Kind is "Association" or "Aggregation" or "Composition" && (!Multiplicity(link.SourceMultiplicity) || !Multiplicity(link.TargetMultiplicity))) errors.Add(new() { LinkId = link.Id, Text = "Trage an beiden Enden eine gültige Multiplizität ein, etwa 1, 0..1 oder 0..*." });
            if (source.Kind == "Final" || target.Kind == "Initial") errors.Add(new() { LinkId = link.Id, Text = "Ein Endknoten hat keinen ausgehenden Übergang; ein Startknoten keinen eingehenden." });
        }
        return errors;
    }

    /// <summary> Akzeptiert eigene Parameternamen, vergleicht später aber Anzahl und Datentypen. </summary>
    private static bool ValidParameters(string parameters) => string.IsNullOrWhiteSpace(parameters) || parameters.Split(',').All(part => part.Split(':') is var parts && parts.Length == 2 && parts.All(value => !string.IsNullOrWhiteSpace(value)));
    /// <summary> Gültige, bewusst begrenzte Multiplizitäten; n und * sind gleichbedeutend. </summary>
    private static bool Multiplicity(string value) => NormalizeMultiplicity(value) is "1" or "0..1" or "0..*" or "1..*" or "2..*";
    /// <summary> Entfernt lediglich Schreibvarianten, keine fachlichen Unterschiede. </summary>
    public static string NormalizeMultiplicity(string value) => value.Trim().Replace(" ", "").Replace("n", "*").Replace("N", "*") is var text && text == "1..1" ? "1" : text;
    /// <summary> Vereinheitlicht übliche Typnamen; Double und Decimal bleiben verschiedene Typen. </summary>
    public static string NormalizeType(string value) => value.Trim().ToLowerInvariant() switch { "integer" or "ganzzahl" => "int", "boolean" or "wahrheitswert" => "bool", "text" or "zeichenkette" => "string", "nichts" => "void", var text => text };
    /// <summary> Begrenzt lokale Texte, ohne Entwürfe beim Speichern fachlich vorzeitig abzulehnen. </summary>
    private static bool Text(string? value, int length) => value != null && value.Length <= length;
    private static bool Id(string? value) => Text(value, 80) && value!.Length > 0;
}
