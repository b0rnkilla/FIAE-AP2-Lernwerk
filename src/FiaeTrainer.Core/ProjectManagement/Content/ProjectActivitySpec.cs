namespace FiaeTrainer.Core.ProjectManagement.Content;

/// <summary> Fachlich vereinbarter Vorgang; seine Solltermine werden nicht als Vorschau angezeigt. </summary>
public sealed class ProjectActivitySpec
{
    /// <summary> Fachkennung für einen vorgegebenen Projektbestandteil. </summary>
    public string Key { get; set; } = "";
    /// <summary> Lesbare Bedeutung der Fachrolle. </summary>
    public string Label { get; set; } = "";
    /// <summary> Group, Work oder Milestone. </summary>
    public string Kind { get; set; } = "Work";
    /// <summary> Vereinbarte Dauer; Meilensteine besitzen null Tage. </summary>
    public int Duration { get; set; }
    /// <summary> Vereinbarte übergeordnete Fachrolle im Strukturplan. </summary>
    public string ParentKey { get; set; } = "";
    /// <summary> Ausschließlich direkte Vorgänger, ohne transitive Zusatzpfeile. </summary>
    public List<string> Predecessors { get; set; } = [];
    /// <summary> Feste Zuständigkeit, falls diese im Auftrag vorgegeben ist. </summary>
    public string Resource { get; set; } = "";
}
