using FiaeTrainer.Core.SoftwareDesign.Models;
using FiaeTrainer.Core.Uml.Evaluation;

namespace FiaeTrainer.Core.SoftwareDesign.Evaluation;

/// <summary> Hält auch unvollständige eigene Entwürfe speicherbar und begrenzt deren Umfang. </summary>
public static class SoftwareRules
{
    /// <summary> Prüft ausschließlich Struktur und Grenzen; fachliche Fehler bleiben normale Antworten. </summary>
    public static bool Storable(SoftwareAnswer? answer)
    {
        if (answer == null || !Text(answer.Payload, 8000) || !Text(answer.Reasoning, 4000) || answer.Values == null || answer.Values.Count > 24 || answer.Values.Any(pair => !Text(pair.Key, 80) || !Text(pair.Value, 500)) || answer.SelectedIds == null || answer.SelectedIds.Count > 20 || answer.SelectedIds.Any(id => !Text(id, 80)) || !UmlRules.Storable(answer.Diagram) || answer.Board?.Nodes == null || answer.Board.Links == null || answer.Board.Nodes.Count > 16 || answer.Board.Links.Count > 32) return false;
        var nodes = answer.Board.Nodes; var links = answer.Board.Links;
        return nodes.All(node => node != null && Id(node.Id) && Text(node.Key, 80) && Text(node.Name, 80) && Text(node.Kind, 80) && Text(node.Group, 80) && Text(node.Hint, 200))
            && links.All(link => link != null && Id(link.Id) && Text(link.Source, 80) && Text(link.Target, 80) && Text(link.Kind, 80))
            && nodes.Select(node => node.Id).Distinct().Count() == nodes.Count && links.Select(link => link.Id).Distinct().Count() == links.Count;
    }
    /// <summary> Eine freie Zusatzbegründung allein ist noch keine fachliche Antwort. </summary>
    public static bool HasAnswer(SoftwareAnswer answer) => answer.SelectedIds.Count > 0 || answer.Values.Values.Any(value => !string.IsNullOrWhiteSpace(value)) || answer.Board.Nodes.Count > 0 || answer.Diagram.Model.Nodes.Count > 0 || !string.IsNullOrWhiteSpace(answer.Payload);
    private static bool Text(string? value, int maximum) => value != null && value.Length <= maximum;
    private static bool Id(string? value) => Text(value, 80) && !string.IsNullOrWhiteSpace(value);
}
