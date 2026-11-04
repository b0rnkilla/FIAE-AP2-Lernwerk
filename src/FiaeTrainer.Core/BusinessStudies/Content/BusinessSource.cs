namespace FiaeTrainer.Core.BusinessStudies.Content;

/// <summary> Fachliche Primärquelle; wird erst in der Lernauflösung verlinkt. </summary>
public sealed class BusinessSource
{
    /// <summary> Referenz für Aufgaben und Spickzettel. </summary>
    public string Id { get; init; } = "";
    /// <summary> Herausgeber und Thema. </summary>
    public string Title { get; init; } = "";
    /// <summary> Öffentliche HTTPS-Adresse der Quelle. </summary>
    public string Url { get; init; } = "";
}
