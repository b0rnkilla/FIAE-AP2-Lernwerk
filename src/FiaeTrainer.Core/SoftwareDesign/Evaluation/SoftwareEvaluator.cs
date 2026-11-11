using System.Globalization;
using FiaeTrainer.Core.SoftwareDesign.Content;
using FiaeTrainer.Core.SoftwareDesign.Models;
using FiaeTrainer.Core.Uml.Evaluation;

namespace FiaeTrainer.Core.SoftwareDesign.Evaluation;

/// <summary> Prüft eindeutige Fachverträge und explizite Entwurfsvarianten, ohne freie Begründungen zu benoten. </summary>
public static class SoftwareEvaluator
{
    /// <summary> Ein leerer oder beschädigter Entwurf ist keine abgegebene Fachlösung. </summary>
    public static SoftwareEvaluation Evaluate(SoftwareAssignment task, SoftwareAnswer answer)
    {
        if (!SoftwareRules.Storable(answer)) return Failure("Der Entwurf ist beschädigt oder überschreitet die Speichergrenzen.");
        if (!SoftwareRules.HasAnswer(answer)) return Failure("Ergänze zuerst deine fachliche Antwort.");
        if (answer.Values.Keys.Any(key => task.Fields.All(field => field.Key != key))) return Failure("Der Entwurf enthält Antwortfelder, die nicht zu dieser Aufgabe gehören.");
        if (task.Kind == "Choice")
        {
            bool correct = answer.SelectedIds.Count == answer.SelectedIds.Distinct().Count() && answer.SelectedIds.Order().SequenceEqual(task.CorrectIds.Order());
            return new() { Checks = [new() { Name = "Fachliche Entscheidung", Maximum = 50, Earned = correct ? 50 : 0, Detail = correct ? "Deine Auswahl erfüllt den Auftrag." : "Deine Auswahl passt noch nicht vollständig zum beschriebenen Fall. Vergleiche die Aussagen erneut mit den Anforderungen." }] };
        }
        if (task.Kind == "Diagram")
        {
            var result = UmlEvaluator.Evaluate(task.Diagram!, answer.Diagram);
            return new() { Checks = result.Checks.Select(check => new SoftwareCheck { Name = check.Name, Earned = check.Earned, Maximum = check.Maximum, Detail = check.Detail }).ToList() };
        }
        return task.Solutions.Select(expected => Compare(task, expected, answer)).OrderByDescending(result => result.Earned).First();
    }

    /// <summary> Verteilt Kriterien exakt auf 50 Anteile; mehrere vollständige Varianten können gleichwertig sein. </summary>
    private static SoftwareEvaluation Compare(SoftwareAssignment task, SoftwareAnswer expected, SoftwareAnswer answer)
    {
        List<SoftwareCheck> checks = [];
        if (task.Kind is "Architecture" or "Form") return CompareBoard(task, expected.Board, answer.Board);
        int budget = task.PayloadFormat == "None" ? 50 : task.Fields.Count == 0 ? 0 : 30;
        for (int i = 0; i < task.Fields.Count; i++)
        {
            var field = task.Fields[i]; string value = answer.Values.GetValueOrDefault(field.Key, "");
            bool correct = FieldEqual(field, expected.Values[field.Key], value);
            int maximum = budget * (i + 1) / task.Fields.Count - budget * i / task.Fields.Count;
            checks.Add(new() { Name = field.Label, Earned = correct ? maximum : 0, Maximum = maximum, Detail = correct ? "Dieses Feld passt zum Auftrag." : string.IsNullOrWhiteSpace(value) ? "Hier fehlt noch deine Antwort." : "Prüfe diese Angabe anhand der im Auftrag beschriebenen Rolle, Regel oder Randbedingung." });
        }
        if (task.PayloadFormat != "None")
        {
            bool correct = SoftwarePayload.Equivalent(task.PayloadFormat, expected.Payload, answer.Payload);
            checks.Add(new() { Name = task.PayloadFormat == "Json" ? "JSON-Nachricht" : "XML-Nachricht", Maximum = 50 - budget, Earned = correct ? 50 - budget : 0, Detail = correct ? "Struktur, Datentypen und Werte erfüllen den Nachrichtenvertrag." : "Prüfe Syntax, Feldnamen, Verschachtelung, Datentypen und die ausdrücklich geforderten Werte. Die Einrückung zählt nicht." });
        }
        return new() { Checks = checks };
    }
    /// <summary> Freie Namen verändern keine Fachrolle; Formularreihenfolge und Verbindungen bleiben überprüfbar. </summary>
    private static SoftwareEvaluation CompareBoard(SoftwareAssignment task, SoftwareBoard expected, SoftwareBoard actual)
    {
        if (actual.Nodes.Count == 0) return Failure("Lege zuerst die benötigten Elemente an.");
        if (actual.Nodes.Any(node => !task.NodeVocabulary.ContainsKey(node.Key) || !task.NodeKinds.ContainsKey(node.Kind) || !task.Groups.ContainsKey(node.Group) || string.IsNullOrWhiteSpace(node.Name)) || actual.Nodes.Select(node => node.Key).Distinct().Count() != actual.Nodes.Count) return Failure("Ordne jedem Element eine eindeutige angebotene Fachrolle, passende Art, Gruppe und einen lesbaren Namen zu.");
        if (actual.Links.Any(link => !task.LinkKinds.ContainsKey(link.Kind) || actual.Nodes.All(node => node.Id != link.Source) || actual.Nodes.All(node => node.Id != link.Target))) return Failure("Wähle für jede Verbindung gültige Endpunkte und eine angebotene Verbindungsart.");
        int nodes = expected.Nodes.Count(wanted => actual.Nodes.Any(node => node.Key == wanted.Key && node.Kind == wanted.Kind && node.Group == wanted.Group && (task.Kind != "Form" || node.Required == wanted.Required && (string.IsNullOrWhiteSpace(wanted.Hint) || !string.IsNullOrWhiteSpace(node.Hint)))));
        int nodeMaximum = task.Kind == "Form" ? 40 : 30;
        List<SoftwareCheck> checks = [new() { Name = "Elemente und Zuständigkeiten", Maximum = nodeMaximum, Earned = nodeMaximum * nodes / Math.Max(expected.Nodes.Count, actual.Nodes.Count), Detail = nodes == expected.Nodes.Count && actual.Nodes.Count == expected.Nodes.Count ? "Rollen, Arten und Gruppenzuordnungen passen." : "Prüfe Vollständigkeit, Elementarten und Gruppenzuordnung. Im Formular zählen auch Pflichtfelder und die verlangten Hilfetexte. Zusätzliche Elemente erfüllen den beschriebenen Ausschnitt nicht." }];
        if (task.Kind == "Form")
        {
            bool order = expected.Nodes.Select(node => node.Key).SequenceEqual(actual.Nodes.Select(node => node.Key)) && actual.Links.Count == 0;
            checks.Add(new() { Name = "Bedienreihenfolge", Maximum = 10, Earned = order ? 10 : 0, Detail = order ? "Die Reihenfolge entspricht dem Auftrag." : "Ordne die Felder und Aktionen wie im Auftrag beschrieben. Formularfelder benötigen keine Architekturverbindungen." });
        }
        else
        {
            string Signature(SoftwareBoard model, SoftwareLink link) => string.Join("|", model.Nodes.Single(node => node.Id == link.Source).Key, model.Nodes.Single(node => node.Id == link.Target).Key, link.Kind);
            var unused = actual.Links.ToList(); int links = 0;
            foreach (var wanted in expected.Links)
            {
                string signature = Signature(expected, wanted); var found = unused.FirstOrDefault(link => Signature(actual, link) == signature);
                if (found != null) { links++; unused.Remove(found); }
            }
            int denominator = Math.Max(expected.Links.Count, actual.Links.Count);
            checks.Add(new() { Name = "Richtung und Art der Abhängigkeiten", Maximum = 20, Earned = denominator == 0 ? 20 : 20 * links / denominator, Detail = links == expected.Links.Count && unused.Count == 0 ? "Die Zusammenarbeit erfüllt die vorgegebenen Abhängigkeitsregeln." : "Prüfe jede Verbindung: Wer ruft wen auf beziehungsweise hängt von wem ab? Zusätzliche oder doppelte Verbindungen sind keine Ersatzlösung." });
        }
        return new() { Checks = checks };
    }
    /// <summary> Vertragsfelder bleiben exakt; ausdrücklich freigegebene Text-/Zahlenschreibweisen sind toleranter. </summary>
    private static bool FieldEqual(SoftwareField field, string expected, string actual)
    {
        if (field.Comparison == "Number")
        {
            bool Number(string value, out decimal number) => decimal.TryParse(value.Trim().Replace(',', '.'), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out number);
            return Number(expected, out var a) && Number(actual, out var b) && a == b;
        }
        if (field.Comparison == "Text") return string.Join(" ", expected.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)).Equals(string.Join(" ", actual.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)), StringComparison.OrdinalIgnoreCase);
        return expected.Trim() == actual.Trim();
    }
    private static SoftwareEvaluation Failure(string text) => new() { Checks = [new() { Name = "Entwurf vervollständigen", Maximum = 50, Detail = text }] };
}
