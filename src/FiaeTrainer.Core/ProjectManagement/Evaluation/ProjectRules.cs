using System.Globalization;
using FiaeTrainer.Core.ProjectManagement.Models;

namespace FiaeTrainer.Core.ProjectManagement.Evaluation;

/// <summary> Unfertige Pläne bleiben speicherbar; Größe und Struktur schützen das gemeinsame Profil. </summary>
public static class ProjectRules
{
    /// <summary> Prüft Speichergrenzen, ohne fachlich falsche Arbeitspakete zu verwerfen. </summary>
    public static bool Storable(ProjectAnswer? answer) => answer != null && Values(answer.Values, 64, 500)
        && answer.SelectedIds != null && answer.SelectedIds.Count <= 24 && answer.SelectedIds.All(value => Text(value, 80))
        && answer.Activities != null && answer.Activities.Count <= 16 && answer.Activities.All(item => item != null && !string.IsNullOrWhiteSpace(item.Id) && Text(item.Id, 80) && Text(item.Key, 80) && Text(item.Name, 80) && Text(item.ParentId, 80) && Text(item.Duration, 12) && Text(item.Start, 12) && Text(item.Resource, 80) && Values(item.Values, 6, 12))
        && answer.Activities.Select(item => item.Id).Distinct().Count() == answer.Activities.Count
        && answer.Dependencies != null && answer.Dependencies.Count <= 32 && answer.Dependencies.All(item => item != null && !string.IsNullOrWhiteSpace(item.Id) && Text(item.Id, 80) && Text(item.SourceId, 80) && Text(item.TargetId, 80))
        && answer.Dependencies.Select(item => item.Id).Distinct().Count() == answer.Dependencies.Count;
    /// <summary> Ein tatsächlich angelegter Vorgang ist bereits ein bearbeiteter Plan. </summary>
    public static bool HasAnswer(ProjectAnswer answer) => answer.SelectedIds.Count > 0 || answer.Values.Values.Any(value => !string.IsNullOrWhiteSpace(value)) || answer.Activities.Count > 0 || answer.Dependencies.Count > 0;
    /// <summary> Ganze Zeitwerte ohne Tausendertrennzeichen. </summary>
    public static bool Integer(string value, out int number) => int.TryParse(value.Trim(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out number);
    /// <summary> Geld-/Prozentwerte akzeptieren Punkt oder Komma, keine geratenen Einheiten. </summary>
    public static bool Number(string value, out decimal number) => decimal.TryParse(value.Trim().Replace(',', '.'), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out number);
    private static bool Text(string? value, int limit) => value != null && value.Length <= limit;
    private static bool Values(Dictionary<string, string>? values, int count, int limit) => values != null && values.Count <= count && values.All(pair => Text(pair.Key, 80) && Text(pair.Value, limit));
}
