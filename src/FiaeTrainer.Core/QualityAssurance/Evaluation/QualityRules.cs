using FiaeTrainer.Core.QualityAssurance.Models;

namespace FiaeTrainer.Core.QualityAssurance.Evaluation;

/// <summary> Unvollständige Antworten bleiben speicherbar; beschädigte und übergroße Profile werden nicht überschrieben. </summary>
public static class QualityRules
{
    /// <summary> Prüft Struktur, stabile Kennungen und Größen, ohne fachlich falsche Entwürfe zu löschen. </summary>
    public static bool Storable(QualityAnswer? answer)
        => answer != null && Text(answer.Reasoning, 4000) && Values(answer.Values, 64, 500) && List(answer.SelectedIds) && List(answer.Order)
           && answer.TestCases != null && answer.TestCases.Count <= 16 && answer.TestCases.All(row => row != null && !string.IsNullOrWhiteSpace(row.Id) && Text(row.Id, 80) && Values(row.Inputs, 8, 200) && Text(row.Expected, 200)) && answer.TestCases.Select(row => row.Id).Distinct().Count() == answer.TestCases.Count;
    /// <summary> Freie Begründungen allein ersetzen keine fachliche Antwort. </summary>
    public static bool HasAnswer(QualityAnswer answer) => answer.SelectedIds.Count > 0 || answer.Order.Count > 0 || answer.Values.Values.Any(value => !string.IsNullOrWhiteSpace(value)) || answer.TestCases.Any(row => row.Inputs.Values.Any(value => !string.IsNullOrWhiteSpace(value)) || !string.IsNullOrWhiteSpace(row.Expected));
    /// <summary> Zusätzliche gespeicherte Ausführungsansichten sind ebenfalls begrenzt. </summary>
    public static bool Storable(QualityProbe? probe) => probe == null || probe.Rows != null && probe.Rows.Count <= 16 && probe.Rows.All(row => row != null && Text(row.Id, 80) && Text(row.Inputs, 2000) && Text(row.Expected, 200) && Text(row.Detail, 1000) && Set(row.DetectedFaults) && (row.Execution == null || Text(row.Execution.Output, 200) && Text(row.Execution.Path, 300) && Set(row.Execution.Statements) && Set(row.Execution.Branches)));
    private static bool Text(string? value, int maximum) => value != null && value.Length <= maximum;
    private static bool Values(Dictionary<string, string>? values, int count, int maximum) => values != null && values.Count <= count && values.All(pair => Text(pair.Key, 80) && Text(pair.Value, maximum));
    private static bool List(List<string>? list) => list != null && list.Count <= 24 && list.All(value => Text(value, 80));
    private static bool Set(HashSet<string>? set) => set != null && set.Count <= 64 && set.All(value => Text(value, 80));
}
