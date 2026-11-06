using FiaeTrainer.Core.DatabaseDesign.Models;

namespace FiaeTrainer.Core.DatabaseDesign.Evaluation;

/// <summary> Prüft begrenzte speicherbare Entwürfe und grundlegende relationale Integrität. </summary>
public static class DesignModelRules
{
    /// <summary> Auch unvollständige Editorentwürfe sind speicherbar, beschädigte oder unbegrenzt große Objekte nicht. </summary>
    public static bool Storable(DesignAnswer? answer)
    {
        if (answer?.Model?.Tables == null || answer.Model.Relations == null || answer.SelectedIds == null || answer.SelectedIds.Count > 20 || answer.SelectedIds.Any(id => !Text(id, 80)) || answer.Model.Tables.Count > 8 || answer.Model.Relations.Count > 16) return false;
        var tables = answer.Model.Tables;
        if (tables.Any(table => table == null || !Text(table.Id, 80) || table.Id.Length == 0 || !Text(table.Name, 80) || table.Columns == null || table.Columns.Count > 20) || tables.Select(table => table.Id).Distinct().Count() != tables.Count) return false;
        var columns = tables.SelectMany(table => table.Columns).ToList();
        if (columns.Any(column => column == null || !Text(column.Id, 80) || column.Id.Length == 0 || !Text(column.Name, 80) || !Text(column.Attribute, 80) || !Text(column.UniqueGroup, 8)) || columns.Select(column => column.Id).Distinct().Count() != columns.Count) return false;
        return answer.Model.Relations.All(relation => relation != null && Text(relation.Id, 80) && relation.Id.Length > 0 && Text(relation.SourceTable, 80) && Text(relation.TargetTable, 80) && relation.ParentMinimum is 0 or 1 && relation.ChildMinimum is >= 0 and <= 2 && relation.ChildMaximum is "1" or "n" && relation.Pairs != null && relation.Pairs.Count is > 0 and <= 8 && relation.Pairs.All(pair => pair != null && Text(pair.Source, 80) && Text(pair.Target, 80))) && answer.Model.Relations.Select(relation => relation.Id).Distinct().Count() == answer.Model.Relations.Count;
    }

    /// <summary> Liefert eindeutige Attributmengen aller Primär- und alternativen Schlüssel, unabhängig von Gruppennamen. </summary>
    public static List<string> Keys(DesignTable table) => (table.Columns.Any(column => column.PrimaryKey) ? new[] { Signature(table.Columns.Where(column => column.PrimaryKey).Select(column => column.Attribute)) } : []).Concat(table.Columns.Where(column => column.UniqueGroup.Length > 0).GroupBy(column => column.UniqueGroup).Select(group => Signature(group.Select(column => column.Attribute)))).Distinct().Order().ToList();

    /// <summary> Eine eindeutige mengenartige Signatur verhindert Einflüsse von Reihenfolge und Beschriftung. </summary>
    public static string Signature(IEnumerable<string> values) => string.Join("|", values.Order(StringComparer.Ordinal));

    /// <summary> Prüft vor der fachlichen Zuordnung PK, FK, Schlüsselreferenz und konsistente Kardinalitäten. </summary>
    public static List<string> Errors(DesignModel model)
    {
        List<string> errors = [];
        if (!Storable(new() { Model = model })) return ["Der Entwurf enthält ungültige oder zu viele Elemente."];
        if (model.Tables.Count == 0) return ["Lege zunächst mindestens eine Tabelle mit Attributen an."];
        if (model.Tables.Select(table => table.Name.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() != model.Tables.Count) errors.Add("Vergib unterschiedliche Tabellennamen.");
        foreach (DesignTable table in model.Tables)
        {
            if (string.IsNullOrWhiteSpace(table.Name) || table.Columns.Count == 0 || table.Columns.Any(column => string.IsNullOrWhiteSpace(column.Name) || column.Attribute.Length == 0)) errors.Add($"Ergänze Namen und fachliche Attribute in der Tabelle {table.Name}.");
            if (table.Columns.Select(column => column.Attribute).Distinct().Count() != table.Columns.Count || table.Columns.Select(column => column.Name.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() != table.Columns.Count) errors.Add($"Attribute dürfen in {table.Name} nicht doppelt vorkommen.");
            if (!table.Columns.Any(column => column.PrimaryKey)) errors.Add($"Markiere den Primärschlüssel von {table.Name}.");
            if (table.Columns.Any(column => column.PrimaryKey && column.Nullable)) errors.Add($"Ein Primärschlüssel in {table.Name} darf nicht NULL sein.");
        }
        foreach (DesignRelation relation in model.Relations)
        {
            DesignTable? source = model.Tables.SingleOrDefault(table => table.Id == relation.SourceTable), target = model.Tables.SingleOrDefault(table => table.Id == relation.TargetTable);
            if (source == null || target == null || relation.Pairs.Any(pair => !source.Columns.Any(column => column.Id == pair.Source) || !target.Columns.Any(column => column.Id == pair.Target))) { errors.Add("Wähle für jede Beziehung beide Tabellen und alle zugehörigen Attributpaare."); continue; }
            var from = relation.Pairs.Select(pair => source.Columns.Single(column => column.Id == pair.Source)).ToList();
            var to = relation.Pairs.Select(pair => target.Columns.Single(column => column.Id == pair.Target)).ToList();
            if (from.Select(column => column.Id).Distinct().Count() != from.Count || to.Select(column => column.Id).Distinct().Count() != to.Count) errors.Add("Ein FK-Attribut darf innerhalb derselben Beziehung nicht doppelt zugeordnet werden.");
            if (!Keys(target).Contains(Signature(to.Select(column => column.Attribute)))) errors.Add($"Der Fremdschlüssel auf {target.Name} muss einen vollständigen eindeutigen Schlüssel referenzieren.");
            if (relation.ParentMinimum == 1 && from.Any(column => column.Nullable) || relation.ParentMinimum == 0 && from.All(column => !column.Nullable)) errors.Add($"Pflichtbeziehung und NULL-Erlaubnis in {source.Name} passen nicht zusammen.");
            if (relation.ChildMaximum == "1" && !Keys(source).Contains(Signature(from.Select(column => column.Attribute)))) errors.Add($"Für höchstens einen abhängigen Satz muss der Fremdschlüssel in {source.Name} eindeutig sein.");
            if (relation.ChildMaximum == "1" && relation.ChildMinimum > 1) errors.Add("Die Mindestanzahl kann nicht größer als die Höchstanzahl sein.");
        }
        return errors.Distinct().ToList();
    }

    /// <summary> Begrenzt lokale Beschriftungen und Kennungen. </summary>
    private static bool Text(string? value, int maximum) => value != null && value.Length <= maximum;
}
