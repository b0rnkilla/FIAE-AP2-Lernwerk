namespace FiaeTrainer.Core.BusinessStudies.Content;

/// <summary> Themenbereich samt kurzer Orientierung für das gezielte Training. </summary>
public sealed class BusinessTopic
{
    /// <summary> Dauerhafte Themenkennung. </summary>
    public string Id { get; init; } = "";
    /// <summary> Deutscher Themenname. </summary>
    public string Title { get; init; } = "";
    /// <summary> Lernziele in einem kurzen Satz. </summary>
    public string Description { get; init; } = "";
    /// <summary> Einstiegserklärung zu häufigen Verwechslungen. </summary>
    public string Introduction { get; init; } = "";
}
