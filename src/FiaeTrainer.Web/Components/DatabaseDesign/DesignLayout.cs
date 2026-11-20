using FiaeTrainer.Core.DatabaseDesign.Models;
using FiaeTrainer.Web.Components.Uml;

namespace FiaeTrainer.Web.Components.DatabaseDesign;

/// <summary> Ordnet referenzierte Tabellen über ihren abhängigen Tabellen an und erhält freie Positionen. </summary>
public static class DesignLayout
{
    /// <summary> Bestimmt Ebenen auch bei Selbstbezügen und Zyklen ohne endlose Rekursion. </summary>
    public static Dictionary<string, UmlBox> Place(DesignModel model)
    {
        Dictionary<string, int> levels = [];
        int Level(string id, HashSet<string> path)
        {
            if (levels.TryGetValue(id, out int known)) return known;
            if (!path.Add(id)) return 0;
            var parents = model.Relations.Where(relation => relation.SourceTable == id && relation.TargetTable != id && model.Tables.Any(table => table.Id == relation.TargetTable)).Select(relation => relation.TargetTable).Distinct().ToArray();
            int level = parents.Length == 0 ? 0 : parents.Max(parent => Level(parent, new(path))) + 1;
            return levels[id] = Math.Min(level, 7);
        }
        foreach (var table in model.Tables) Level(table.Id, []);
        Dictionary<string, UmlBox> boxes = [];
        int y = 50;
        foreach (var group in model.Tables.GroupBy(table => levels[table.Id]).OrderBy(group => group.Key))
        {
            var row = group.ToArray();
            for (int i = 0; i < row.Length; i++)
            {
                var table = row[i];
                int x = 50 + i % 2 * 410 + (row.Length == 1 ? 205 : 0);
                int top = y + i / 2 * (row.Max(item => 80 + item.Columns.Count * 28) + 140);
                boxes[table.Id] = new(x, top, 310, 80 + table.Columns.Count * 28);
            }
            y = boxes.Values.Max(box => box.Y + box.Height) + 160;
        }
        foreach (var (id, position) in model.Positions)
            if (boxes.TryGetValue(id, out var box)) boxes[id] = box with { X = position.X, Y = position.Y };
        return boxes;
    }
}
