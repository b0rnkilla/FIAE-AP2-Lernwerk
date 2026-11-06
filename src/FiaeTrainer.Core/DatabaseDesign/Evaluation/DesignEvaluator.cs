using FiaeTrainer.Core.DatabaseDesign.Content;
using FiaeTrainer.Core.DatabaseDesign.Models;

namespace FiaeTrainer.Core.DatabaseDesign.Evaluation;

/// <summary> Bewertet explizite fachliche Modellverträge statt Tabellenbeschriftung oder grafischer Position. </summary>
public static class DesignEvaluator
{
    /// <summary> Auswahl muss vollständig stimmen; bei Entwürfen zählt die beste ausdrücklich zugelassene Variante. </summary>
    public static DesignEvaluation Evaluate(DesignAssignment task, DesignAnswer answer)
    {
        if (!DesignModelRules.Storable(answer)) return Failure("Der Entwurf ist beschädigt oder überschreitet die Grenzen des Baukastens.");
        if (task.Kind != "Model")
        {
            bool correct = answer.SelectedIds.Count == answer.SelectedIds.Distinct().Count() && answer.SelectedIds.Order().SequenceEqual(task.CorrectIds.Order());
            return new() { Checks = [new("Fachliche Entscheidung", correct ? 50 : 0, 50, correct ? task.Explanation : "Prüfe alle Bedingungen des Auftrags. " + task.Explanation)] };
        }
        if (answer.Model.Tables.SelectMany(table => table.Columns).Any(column => column.Attribute.Length > 0 && !task.Attributes.ContainsKey(column.Attribute))) return Failure("Ein Fachattribut gehört nicht zum Vokabular dieses Auftrags. Wähle seine Bedeutung aus der vorgegebenen Liste.");
        List<string> errors = DesignModelRules.Errors(answer.Model);
        if (answer.Model.Tables.Count == 0) return Failure("Lege zunächst mindestens eine Tabelle mit Attributen an.");
        DesignEvaluation result = task.Solutions.Select(expected => Compare(expected, answer.Model)).OrderByDescending(item => item.Earned).FirstOrDefault() ?? Failure("Für diesen Auftrag fehlt der Bewertungsvertrag.");
        if (errors.Count > 0)
        {
            if (result.Correct) return Failure(string.Join(" ", errors.Take(5)));
            result.Checks.Add(new("Konsistenz prüfen", 0, 0, string.Join(" ", errors.Take(5))));
        }
        return result;
    }

    /// <summary> Prüft Tabellenaufteilung, vollständige PK-/UNIQUE-Schlüssel, NULL-Regeln und gerichtete Referenzen. </summary>
    private static DesignEvaluation Compare(DesignModel expected, DesignModel actual)
    {
        Dictionary<string, DesignTable> mapping = [];
        HashSet<string> used = [];
        foreach (DesignTable table in expected.Tables.OrderByDescending(table => table.Columns.Count))
        {
            string wanted = Attributes(table);
            DesignTable? match = actual.Tables.Where(candidate => !used.Contains(candidate.Id)).OrderByDescending(candidate => Attributes(candidate) == wanted ? 1000 : candidate.Columns.Count(column => table.Columns.Any(wantedColumn => wantedColumn.Attribute == column.Attribute))).FirstOrDefault();
            if (match == null) continue;
            mapping[table.Id] = match;
            used.Add(match.Id);
        }
        bool SameTable(DesignTable table) => mapping.TryGetValue(table.Id, out var found) && Attributes(found) == Attributes(table);
        List<string> tableErrors = [], keyErrors = [], ruleErrors = [], foreignErrors = [], cardinalErrors = [];
        int tables = 0, keys = 0, rules = 0, foreign = 0, cardinal = 0;
        foreach (DesignTable table in expected.Tables)
        {
            if (SameTable(table)) tables++;
            else tableErrors.Add($"Die Attribute für {table.Name} gehören vollständig zusammen: {string.Join(", ", table.Columns.Select(column => column.Name))}.");
            if (!SameTable(table)) { keyErrors.Add($"Prüfe zuerst die Tabellenaufteilung für {table.Name}."); ruleErrors.Add($"Die Attributregeln für {table.Name} sind noch nicht vollständig zugeordnet."); continue; }
            DesignTable found = mapping[table.Id];
            if (DesignModelRules.Signature(table.Columns.Where(column => column.PrimaryKey).Select(column => column.Attribute)) == DesignModelRules.Signature(found.Columns.Where(column => column.PrimaryKey).Select(column => column.Attribute))) keys++;
            else keyErrors.Add($"Der PK von {found.Name} benötigt {string.Join(" + ", table.Columns.Where(column => column.PrimaryKey).Select(column => column.Name))}.");
            if (DesignModelRules.Keys(table).SequenceEqual(DesignModelRules.Keys(found)) && table.Columns.All(column => found.Columns.Single(item => item.Attribute == column.Attribute).Nullable == column.Nullable)) rules++;
            else ruleErrors.Add($"Prüfe NULL-Erlaubnis und zusätzliche eindeutige Schlüssel in {found.Name}; auch zusätzliche Einschränkungen verändern den Vertrag.");
        }
        foreach (DesignRelation relation in expected.Relations)
        {
            var source = expected.Tables.Single(table => table.Id == relation.SourceTable);
            var target = expected.Tables.Single(table => table.Id == relation.TargetTable);
            DesignRelation? found = SameTable(source) && SameTable(target) ? actual.Relations.FirstOrDefault(candidate => candidate.SourceTable == mapping[source.Id].Id && candidate.TargetTable == mapping[target.Id].Id && PairSignature(actual, candidate) == PairSignature(expected, relation)) : null;
            if (found != null) foreign++;
            else foreignErrors.Add($"Ergänze die vollständige FK-Zuordnung von {source.Name} nach {target.Name}.");
            if (found != null && found.ParentMinimum == relation.ParentMinimum && found.ChildMinimum == relation.ChildMinimum && found.ChildMaximum == relation.ChildMaximum) cardinal++;
            else cardinalErrors.Add($"Beziehung {source.Name} → {target.Name}: je abhängigem Satz {relation.ParentMinimum}..1 Elternsätze, je Elternsatz {relation.ChildMinimum}..{relation.ChildMaximum} abhängige Sätze.");
        }
        if (actual.Tables.Count > expected.Tables.Count) tableErrors.Add("Der Auftrag benötigt keine zusätzlichen Tabellen.");
        if (actual.Relations.Count > expected.Relations.Count) foreignErrors.Add("Zusätzliche Beziehungen können fachlich unabhängige Vorgänge unzulässig koppeln.");
        int tableTotal = Math.Max(expected.Tables.Count, actual.Tables.Count), relationTotal = Math.Max(expected.Relations.Count, actual.Relations.Count);
        if (relationTotal == 0) { relationTotal = 1; foreign = cardinal = tables == expected.Tables.Count ? 1 : 0; }
        return new() { Checks = [Check("Tabellen und Attributgruppen", tables, tableTotal, 20, tableErrors), Check("Primärschlüssel", keys, expected.Tables.Count, 10, keyErrors), Check("Eindeutigkeit und NULL", rules, expected.Tables.Count, 5, ruleErrors), Check("Fremdschlüssel", foreign, relationTotal, 10, foreignErrors), Check("Kardinalitäten", cardinal, relationTotal, 5, cardinalErrors)] };
    }

    /// <summary> Vergleicht fachliche Attributmengen ohne technische Kennungen oder Namen. </summary>
    private static string Attributes(DesignTable table) => DesignModelRules.Signature(table.Columns.Select(column => column.Attribute));
    /// <summary> Vergleicht Attributpaare einschließlich Richtung; Reihenfolge zusammengesetzter FK-Paare ist frei. </summary>
    private static string PairSignature(DesignModel model, DesignRelation relation)
    {
        DesignTable from = model.Tables.Single(table => table.Id == relation.SourceTable), to = model.Tables.Single(table => table.Id == relation.TargetTable);
        return DesignModelRules.Signature(relation.Pairs.Select(pair => (from.Columns.SingleOrDefault(column => column.Id == pair.Source)?.Attribute ?? "?") + ">" + (to.Columns.SingleOrDefault(column => column.Id == pair.Target)?.Attribute ?? "?")));
    }
    /// <summary> Vergibt Teilpunkte je erfülltem Vertragselement und erklärt verbleibende Abweichungen. </summary>
    private static DesignCheck Check(string name, int passed, int total, int maximum, List<string> errors) => new(name, maximum * passed / Math.Max(1, total), maximum, errors.Count == 0 ? "Dieser Teil des Auftrags ist erfüllt." : string.Join(" ", errors.Take(4)));
    /// <summary> Unvollständige oder strukturell widersprüchliche Entwürfe erhalten eine konkrete Korrekturaufforderung. </summary>
    private static DesignEvaluation Failure(string detail) => new() { Checks = [new("Modell vervollständigen", 0, 50, detail)] };
}
