using FiaeTrainer.Core.ProjectManagement.Content;
using FiaeTrainer.Core.ProjectManagement.Models;

namespace FiaeTrainer.Core.ProjectManagement.Evaluation;

/// <summary> Prüft konkrete Verträge und gültige eigene Pläne statt Zeichenpositionen oder Referenzstarttagen. </summary>
public static class ProjectEvaluator
{
    #region Fields

    /// <summary> Sichtbare deutsche Bezeichnungen für die eigenen Rechenzellen. </summary>
    public static readonly Dictionary<string, string> TimeLabels = new() { ["es"] = "FAZ", ["ef"] = "FEZ", ["ls"] = "SAZ", ["lf"] = "SEZ", ["tf"] = "Gesamtpuffer", ["ff"] = "Freier Puffer" };

    #endregion

    #region Methods

    /// <summary> Jede Bewertung ist rein fachlich; vorher werden keine Antworten oder Pläne verändert. </summary>
    public static ProjectEvaluation Evaluate(ProjectAssignment task, ProjectAnswer answer)
    {
        if (!ProjectRules.Storable(answer)) return Failure("Der Entwurf ist beschädigt oder überschreitet die Speichergrenzen.");
        if (!ProjectRules.HasAnswer(answer)) return Failure("Ergänze zuerst deine Antwort oder deinen eigenen Plan.");
        if (answer.Values.Keys.Except(task.Fields.Select(field => field.Key)).Any()) return Failure("Ein Antwortfeld gehört nicht zu diesem Auftrag.");
        if (task.Kind is not ("Choice" or "Review") && answer.SelectedIds.Count > 0) return Failure("Dieser Auftrag verwendet Fachfelder oder einen Plan statt einer Aussagenwahl.");
        if (task.Plan == null && (answer.Activities.Count > 0 || answer.Dependencies.Count > 0)) return Failure("Dieser Auftrag enthält keinen Planbaukasten.");
        var variants = task.Solutions.Count == 0 ? new List<ProjectAnswer> { new() } : task.Solutions;
        return variants.Select(expected => Compare(task, expected, answer)).OrderByDescending(result => result.Earned).First();
    }

    private static ProjectEvaluation Compare(ProjectAssignment task, ProjectAnswer expected, ProjectAnswer answer)
    {
        List<(string Name, int Found, int Total, string Detail)> checks = [];
        if (task.Kind is "Choice" or "Review")
        {
            bool match = answer.SelectedIds.Count == answer.SelectedIds.Distinct().Count() && answer.SelectedIds.Order().SequenceEqual(task.CorrectIds.Order());
            checks.Add((task.Kind == "Review" ? "Konkrete Befunde" : "Fachliche Entscheidung", match ? 1 : 0, 1, match ? "Deine Auswahl erfüllt den beschriebenen Auftrag." : "Prüfe die vollständige Auswahl anhand der gegebenen Annahmen und Entscheidungskriterien."));
        }
        if (task.Fields.Count > 0)
        {
            var wrong = task.Fields.Where(field => !Matches(field, expected.Values[field.Key], answer.Values.GetValueOrDefault(field.Key, ""))).Select(field => field.Label).ToArray();
            checks.Add(("Fachangaben und Rechenzellen", task.Fields.Count - wrong.Length, task.Fields.Count, wrong.Length == 0 ? "Die Fachwerte und Entscheidungen passen zur Unterlage." : "Prüfe diese Angaben: " + string.Join("; ", wrong) + ". Beachte Einheit, Betrachtungszeitraum und Rundung."));
        }
        if (task.Plan is { } plan) AddPlan(task, plan, answer, checks);
        return Score(checks);
    }

    private static bool Matches(ProjectField field, string expected, string actual)
        => field.Comparison == "Number" ? ProjectRules.Number(expected, out var first) && ProjectRules.Number(actual, out var second) && first == second
            : field.Comparison == "Text" ? expected.Trim().Equals(actual.Trim(), StringComparison.OrdinalIgnoreCase) : expected == actual.Trim();

    /// <summary> Unfertige oder doppelte Rollen erzeugen eine Rückmeldung statt fehlerhafter Wörterbuchzugriffe. </summary>
    private static void AddPlan(ProjectAssignment task, ProjectPlan plan, ProjectAnswer answer, List<(string Name, int Found, int Total, string Detail)> checks)
    {
        bool complete = answer.Activities.Count == plan.Activities.Count && answer.Activities.Select(item => item.Key).Distinct().Count() == plan.Activities.Count && answer.Activities.All(item => plan.Activities.Any(spec => spec.Key == item.Key));
        checks.Add(("Vollständige Arbeitspakete", complete ? 1 : 0, 1, complete ? "Jede benötigte Fachrolle ist genau einmal vorhanden." : "Ergänze jede verlangte Fachrolle genau einmal. Zusätzliche oder doppelte Rollen ersetzen keine fehlende Rolle."));
        if (!complete) return;
        var nodes = answer.Activities.ToDictionary(item => item.Key);
        var byId = answer.Activities.ToDictionary(item => item.Id);
        if (task.Kind == "Structure")
        {
            bool grouping = answer.Dependencies.Count == 0 && plan.Activities.All(spec => spec.ParentKey == "" ? nodes[spec.Key].ParentId == "" : byId.TryGetValue(nodes[spec.Key].ParentId, out var parent) && parent.Key == spec.ParentKey);
            checks.Add(("Gliederung des Strukturplans", grouping ? 1 : 0, 1, grouping ? "Die Arbeitspakete sind den ausdrücklich verlangten Gruppen zugeordnet." : "Prüfe Obergruppen und Zugehörigkeiten. Ein Projektstrukturplan beschreibt hier die Gliederung, keine zeitliche Vorgängerfolge."));
            return;
        }
        var durationWrong = plan.Activities.Where(spec => !ProjectRules.Integer(nodes[spec.Key].Duration, out int duration) || duration != spec.Duration).Select(spec => spec.Label).ToArray();
        checks.Add(("Vereinbarte Dauern", plan.Activities.Count - durationWrong.Length, plan.Activities.Count, durationWrong.Length == 0 ? "Die Dauern und nulltägigen Meilensteine passen zur Unterlage." : "Prüfe die Dauer von: " + string.Join("; ", durationWrong) + "."));
        var wanted = plan.Activities.SelectMany(spec => spec.Predecessors.Select(key => key + ">" + spec.Key)).ToHashSet();
        var actual = answer.Dependencies.Where(link => byId.ContainsKey(link.SourceId) && byId.ContainsKey(link.TargetId)).Select(link => byId[link.SourceId].Key + ">" + byId[link.TargetId].Key).ToList();
        bool dependencies = actual.Count == answer.Dependencies.Count && actual.Distinct().Count() == actual.Count && wanted.SetEquals(actual);
        checks.Add(("Direkte Abhängigkeiten", dependencies ? 1 : 0, 1, dependencies ? "Die Ende-Anfang-Beziehungen entsprechen der vorgegebenen direkten Vorgängerliste." : "Prüfe Richtung und Vollständigkeit der direkten Vorgänger. Selbstverbindungen, zusätzliche oder doppelte Pfeile gehören nicht zum vereinbarten Plan."));
        var timing = ProjectScheduler.Calculate(plan.Activities);
        if (task.Kind == "Network")
        {
            List<string> wrong = [];
            foreach (var spec in plan.Activities)
                foreach (string key in plan.TimingFields)
                    if (!ProjectRules.Integer(nodes[spec.Key].Values.GetValueOrDefault(key, ""), out int value) || value != timing[spec.Key].Value(key)) wrong.Add(spec.Label + " · " + TimeLabels[key]);
            if (plan.TimingFields.Count > 0) checks.Add(("Zeitpunkte und Puffer", plan.Activities.Count * plan.TimingFields.Count - wrong.Count, plan.Activities.Count * plan.TimingFields.Count, wrong.Count == 0 ? "Vorwärts-/Rückwärtsrechnung und Puffer stimmen für den angegebenen Netzplan." : "Prüfe diese Zellen: " + string.Join("; ", wrong) + ". Ausgangspunkt ist Tag 0; alle Endvorgänge beziehen sich auf das gemeinsame früheste Projektende."));
            if (plan.CheckCritical)
            {
                bool critical = plan.Activities.All(spec => nodes[spec.Key].Critical == (timing[spec.Key].TotalFloat == 0));
                checks.Add(("Kritische Vorgänge", critical ? 1 : 0, 1, critical ? "Sämtliche kritischen Vorgänge sind markiert, auch bei mehreren gleich langen Pfaden." : "Prüfe alle Vorgänge anhand ihres Gesamtpuffers. Es kann mehrere kritische Pfade geben."));
            }
        }
        if (task.Kind == "Schedule") AddSchedule(plan, nodes, timing, checks);
    }

    /// <summary> Akzeptiert alle Pläne innerhalb der expliziten Grenzen, nicht nur eine versteckte Referenzanordnung. </summary>
    private static void AddSchedule(ProjectPlan plan, Dictionary<string, ProjectActivity> nodes, Dictionary<string, ProjectTiming> timing, List<(string Name, int Found, int Total, string Detail)> checks)
    {
        var starts = new Dictionary<string, int>();
        bool valid = plan.Activities.All(spec => ProjectRules.Integer(nodes[spec.Key].Start, out int start) && start >= 0 && (long)start + spec.Duration <= plan.Horizon);
        if (valid) foreach (var spec in plan.Activities) starts[spec.Key] = int.Parse(nodes[spec.Key].Start.Trim(), System.Globalization.CultureInfo.InvariantCulture);
        checks.Add(("Beginn und Zeitachse", valid ? 1 : 0, 1, valid ? "Alle Vorgänge liegen innerhalb der angegebenen Zeitachse." : "Trage ganze Anfangstage ab Tag 0 ein. Vorgänge müssen einschließlich ihrer Dauer in die Zeitachse passen."));
        if (!valid) return;
        bool order = plan.Activities.All(spec => spec.Predecessors.All(key => starts[spec.Key] >= starts[key] + plan.Activities.Single(item => item.Key == key).Duration));
        checks.Add(("Zeitliche Reihenfolge", order ? 1 : 0, 1, order ? "Kein Vorgang beginnt vor dem Ende eines direkten Vorgängers." : "Prüfe Vorgängerenden und Nachfolgeranfänge. Ein Vorgang von Tag 2 mit Dauer 3 belegt den Zeitraum [2, 5)."));
        bool finish = (plan.Deadline == 0 || plan.Activities.All(spec => starts[spec.Key] + spec.Duration <= plan.Deadline)) && (!plan.RequireEarliest || plan.Activities.All(spec => starts[spec.Key] == timing[spec.Key].EarlyStart));
        checks.Add((plan.RequireEarliest ? "Früheste Terminierung" : "Verbindlicher Endtermin", finish ? 1 : 0, 1, finish ? "Die im Auftrag verlangte Terminbedingung ist erfüllt." : plan.RequireEarliest ? "Hier sind ausdrücklich die frühestmöglichen Starts ohne Ressourcenbegrenzung verlangt." : "Verschiebe oder ordne die Vorgänge so, dass der verbindliche Endtermin eingehalten wird."));
        if (plan.Resources.Count == 0) return;
        bool roles = plan.Activities.All(spec => spec.Kind == "Milestone" || plan.Resources.ContainsKey(nodes[spec.Key].Resource) && (spec.Resource == "" || nodes[spec.Key].Resource == spec.Resource));
        bool capacity = roles;
        if (roles)
            foreach (var resource in plan.Capacities)
                for (int day = 0; day < plan.Horizon; day++)
                    if (plan.Activities.Count(spec => spec.Kind != "Milestone" && nodes[spec.Key].Resource == resource.Key && starts[spec.Key] <= day && starts[spec.Key] + spec.Duration > day) > resource.Value) capacity = false;
        checks.Add(("Zuständigkeit und Kapazität", capacity ? 1 : 0, 1, capacity ? "Ressourcenzuordnung und gleichzeitige Belegung erfüllen die Vorgaben." : "Prüfe die verlangten Zuständigkeiten und die gleichzeitige Auslastung. Ein Meilenstein mit Dauer 0 belegt keine Arbeitstage."));
    }

    private static ProjectEvaluation Score(List<(string Name, int Found, int Total, string Detail)> checks)
    {
        if (checks.Count == 0) return Failure("Für diesen Auftrag fehlen die fachlichen Prüfkriterien.");
        var result = new ProjectEvaluation();
        for (int i = 0; i < checks.Count; i++)
        {
            var check = checks[i]; int maximum = 50 * (i + 1) / checks.Count - 50 * i / checks.Count;
            result.Checks.Add(new() { Name = check.Name, Earned = maximum * check.Found / Math.Max(1, check.Total), Maximum = maximum, Detail = check.Detail });
        }
        return result;
    }
    private static ProjectEvaluation Failure(string detail) => new() { Checks = [new() { Name = "Antwort vervollständigen", Maximum = 50, Detail = detail }] };

    #endregion
}
