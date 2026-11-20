using FiaeTrainer.Core.Uml.Content;
using FiaeTrainer.Core.Uml.Models;

namespace FiaeTrainer.Web.Components.Uml;

/// <summary> Ordnet kleine Lernmodelle nach Diagrammart und Kontrollfluss automatisch an. </summary>
public static class UmlLayout
{
    /// <summary> Verhindert, dass frei gewählte Erstellreihenfolge als fachlicher Ablauf missverstanden wird. </summary>
    public static Dictionary<string, UmlBox> Place(UmlAssignment task, UmlModel model)
    {
        Dictionary<string, UmlBox> result = [];
        if (task.DiagramType == "Sequence")
        {
            for (int i = 0; i < model.Nodes.Count; i++) result[model.Nodes[i].Id] = new(40 + i * 230, 30, 170, 48);
        }
        else if (task.DiagramType == "UseCase")
        {
            int actor = 0, useCase = 0;
            foreach (var node in model.Nodes)
            {
                if (node.Kind == "Actor") result[node.Id] = new(40, 60 + actor++ * 145, 150, 100);
                else { result[node.Id] = new(280 + useCase % 2 * 270, 70 + useCase / 2 * 150, 220, 75); useCase++; }
            }
        }
        else if (task.DiagramType == "Class")
        {
            int y = 35;
            for (int row = 0; row * 2 < model.Nodes.Count; row++)
            {
                var nodes = model.Nodes.Skip(row * 2).Take(2).ToArray();
                for (int i = 0; i < nodes.Length; i++) result[nodes[i].Id] = new(30 + i * 360, y, 290, 100 + nodes[i].Members.Count * 28);
                y += nodes.Max(node => 100 + node.Members.Count * 28) + 120;
            }
        }
        else
        {
            Dictionary<string, int> levels = [];
            Queue<string> queue = new();
            foreach (var start in model.Nodes.Where(node => node.Kind == "Initial")) { levels[start.Id] = 0; queue.Enqueue(start.Id); }
            while (queue.TryDequeue(out string? current))
                foreach (var link in model.Links.Where(link => link.Source == current))
                    if (model.Nodes.Any(node => node.Id == link.Target) && !levels.ContainsKey(link.Target)) { levels[link.Target] = levels[current] + 1; queue.Enqueue(link.Target); }
            if (task.DiagramType == "Activity")
            {
                // Bei verschieden langen Alternativen steht das gemeinsame Ende unter allen Zweigen.
                var incoming = model.Nodes.ToDictionary(node => node.Id, node => model.Links.Count(link => link.Target == node.Id && model.Nodes.Any(source => source.Id == link.Source)));
                Queue<string> roots = new(model.Nodes.Where(node => incoming[node.Id] == 0).Select(node => node.Id));
                Dictionary<string, int> ordered = roots.ToDictionary(id => id, _ => 0);
                int visited = 0;
                while (roots.TryDequeue(out string? source))
                {
                    visited++;
                    foreach (var link in model.Links.Where(link => link.Source == source && incoming.ContainsKey(link.Target)))
                    {
                        ordered[link.Target] = Math.Max(ordered.GetValueOrDefault(link.Target), ordered[source] + 1);
                        if (--incoming[link.Target] == 0) roots.Enqueue(link.Target);
                    }
                }
                if (visited == model.Nodes.Count) levels = ordered;
            }
            foreach (var node in model.Nodes.Where(node => !levels.ContainsKey(node.Id))) levels[node.Id] = levels.Count == 0 ? 0 : levels.Values.Max() + 1;
            var lanes = task.Lanes.Keys.ToArray();
            foreach (var group in model.Nodes.GroupBy(node => levels[node.Id]))
            {
                var nodes = group.ToArray();
                for (int i = 0; i < nodes.Length; i++)
                {
                    var node = nodes[i]; int width = node.Kind is "Initial" or "Final" ? 34 : node.Kind is "Decision" or "Merge" ? 130 : 210;
                    int height = node.Kind is "Initial" or "Final" ? 34 : node.Kind is "Fork" or "Join" ? 14 : node.Members.Count > 0 ? 78 + node.Members.Count * 28 : 60;
                    string lane = task.Lanes.ContainsKey(node.Lane) ? node.Lane : FiaeTrainer.Core.Diagrams.DiagramNames.Resolve(node.Lane, task.Lanes);
                    int column = lanes.Length > 0 ? Math.Max(0, Array.IndexOf(lanes, lane)) : i;
                    int center = lanes.Length > 0 ? 130 + column * 260 : 390 + (2 * i - nodes.Length + 1) * 130;
                    result[node.Id] = new(center - width / 2, 75 + group.Key * (task.DiagramType == "Activity" ? 135 : 190), width, height);
                }
            }
        }
        foreach (var (id, position) in model.Positions)
            if (result.TryGetValue(id, out var box)) result[id] = box with { X = position.X, Y = task.DiagramType == "Sequence" ? box.Y : position.Y };
        if (task.DiagramType == "Activity")
            foreach (var node in model.Nodes.Where(node => node.Kind is "Fork" or "Join" && !model.Positions.ContainsKey(node.Id)))
            {
                var adjacent = model.Links.Where(link => node.Kind == "Fork" ? link.Source == node.Id : link.Target == node.Id).Select(link => node.Kind == "Fork" ? link.Target : link.Source).Where(result.ContainsKey).Select(id => result[id]).ToArray();
                if (adjacent.Length < 2) continue;
                int left = adjacent.Min(box => box.X + box.Width / 2) - 40, right = adjacent.Max(box => box.X + box.Width / 2) + 40;
                result[node.Id] = result[node.Id] with { X = Math.Max(0, left), Width = Math.Max(210, right - left) };
            }
        return result;
    }
}
