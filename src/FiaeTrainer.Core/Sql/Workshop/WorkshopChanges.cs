using FiaeTrainer.Core.Sql.Results;

namespace FiaeTrainer.Core.Sql.Workshop;

/// <summary> Beschreibt die tatsächlich beobachtete Wirkung im Vergleich zur frischen Arbeitskopie. </summary>
public static class WorkshopChanges
{
    public static IEnumerable<string> Describe(WorkshopRun before, WorkshopRun after)
    {
        foreach (var (name, table) in after.Tables)
        {
            if (!before.Tables.TryGetValue(name, out var old))
            {
                yield return $"{name} angelegt · Spalten: {string.Join(", ", table.Columns)}.";
                continue;
            }
            foreach (string change in DataChanges(name, old, table)) yield return change;
            if (after.Structures.TryGetValue(name, out var schema) && before.Structures.TryGetValue(name, out var previous))
                foreach (var column in schema.Rows.Where(row => !previous.Rows.Any(prior => prior.SequenceEqual(row))))
                    yield return $"{name}.{column[0]} · Spaltendefinition: {string.Join(", ", schema.Columns.Skip(1).Zip(column.Skip(1), (key, value) => $"{key} = {Show(value)}"))}.";
        }
        foreach (var rule in after.ConstraintChecks.Rows.Where(row => !before.ConstraintChecks.Rows.Any(old => old.SequenceEqual(row))))
        {
            string value = Show(rule[1]);
            if (value.StartsWith("FK ", StringComparison.Ordinal))
                yield return $"Fremdschlüssel angelegt: {rule[0]}.{string.Join(" ", value.Split(' ').Skip(1).Take(3))}.";
            else if (rule[0]?.ToString()?.StartsWith("Bestellung 10 löschen", StringComparison.Ordinal) == true && value == "Verweis verletzt")
                yield return "Löschversuch für Bestellung 10 blockiert. Zugehörige Rückgaben verweisen weiterhin auf sie.";
            else if (value == "Verweis verletzt")
                yield return $"Probe „{rule[0]}“: vom Fremdschlüssel blockiert.";
            else
                yield return $"{rule[0]} · {value}.";
        }
    }

    private static IEnumerable<string> DataChanges(string name, QueryResult old, QueryResult table)
    {
        int key = table.Columns.IndexOf("Id");
        if (key >= 0 && old.Columns.IndexOf("Id") == key && table.Rows.Concat(old.Rows).All(row => row[key] != null)
            && old.Rows.Select(row => row[key]).Distinct().Count() == old.Rows.Count && table.Rows.Select(row => row[key]).Distinct().Count() == table.Rows.Count)
        {
            var prior = old.Rows.ToDictionary(row => row[key]!);
            foreach (var row in table.Rows)
            {
                if (!prior.Remove(row[key]!, out var original))
                    yield return $"{name} · hinzugefügt: {PrintRow(table, row)}.";
                else
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        int previous = old.Columns.IndexOf(table.Columns[i]);
                        if ((previous >= 0 && !Equals(original[previous], row[i])) || (previous < 0 && row[i] != null))
                            yield return $"{name} · Id {row[key]}, {table.Columns[i]}: {(previous >= 0 ? Show(original[previous]) : "neu")} → {Show(row[i])}.";
                    }
            }
            foreach (var removed in prior.Values) yield return $"{name} · entfernt: {PrintRow(old, removed)}.";
        }
        else
        {
            var remaining = old.Rows.ToList();
            foreach (var row in table.Rows)
            {
                int match = remaining.FindIndex(previous => previous.SequenceEqual(row));
                if (match >= 0) remaining.RemoveAt(match);
                else yield return $"{name} · hinzugefügt: {PrintRow(table, row)}.";
            }
            foreach (var row in remaining) yield return $"{name} · entfernt: {PrintRow(old, row)}.";
        }
    }

    private static string PrintRow(QueryResult table, object?[] row) =>
        string.Join(", ", table.Columns.Zip(row, (col, value) => $"{col} = {Show(value)}"));
    private static string Show(object? value) => value?.ToString() ?? "NULL";
}
