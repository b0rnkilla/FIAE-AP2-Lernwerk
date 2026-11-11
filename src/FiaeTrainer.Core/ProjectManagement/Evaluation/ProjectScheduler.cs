using FiaeTrainer.Core.ProjectManagement.Content;

namespace FiaeTrainer.Core.ProjectManagement.Evaluation;

/// <summary> Vorwärts-/Rückwärtsrechnung ab Tag 0, ohne Ressourcenoptimierung oder Kalenderannahmen. </summary>
public static class ProjectScheduler
{
    /// <summary> Die Rechenansicht ruft dies niemals als automatische Lösungsvorschau auf. </summary>
    public static Dictionary<string, ProjectTiming> Calculate(IReadOnlyList<ProjectActivitySpec> activities)
    {
        if (activities.Count is < 1 or > 16 || activities.Select(item => item.Key).Distinct().Count() != activities.Count || activities.Any(item => item.Duration is < 0 or > 60 || item.Predecessors.Distinct().Count() != item.Predecessors.Count || item.Predecessors.Any(key => !activities.Any(other => other.Key == key)))) throw new InvalidDataException("Der Netzplanvertrag ist unvollständig.");
        List<ProjectActivitySpec> ordered = [];
        while (ordered.Count < activities.Count)
        {
            var next = activities.FirstOrDefault(item => !ordered.Contains(item) && item.Predecessors.All(key => ordered.Any(done => done.Key == key)));
            if (next == null) throw new InvalidDataException("Der Netzplan enthält einen Kreis.");
            ordered.Add(next);
        }
        Dictionary<string, (int Start, int Finish)> early = [];
        foreach (var item in ordered)
        {
            int start = item.Predecessors.Count == 0 ? 0 : item.Predecessors.Max(key => early[key].Finish);
            early[item.Key] = (start, start + item.Duration);
        }
        int end = early.Values.Max(value => value.Finish);
        Dictionary<string, ProjectTiming> result = [];
        foreach (var item in ordered.AsEnumerable().Reverse())
        {
            var successors = activities.Where(other => other.Predecessors.Contains(item.Key)).ToArray();
            int finish = successors.Length == 0 ? end : successors.Min(other => result[other.Key].LateStart);
            int free = (successors.Length == 0 ? end : successors.Min(other => early[other.Key].Start)) - early[item.Key].Finish;
            result[item.Key] = new(early[item.Key].Start, early[item.Key].Finish, finish - item.Duration, finish, finish - item.Duration - early[item.Key].Start, free);
        }
        return result;
    }
}
