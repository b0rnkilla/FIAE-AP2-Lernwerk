namespace FiaeTrainer.Core.ProjectManagement.Models;

/// <summary> Eigenes Planelement mit stabiler Identität und frei lesbarer Beschriftung. </summary>
public sealed class ProjectActivity
{
    /// <summary> Bleibt beim Umbenennen und automatischen Anordnen erhalten. </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    /// <summary> Fachrolle aus dem jeweiligen Auftrag. </summary>
    public string Key { get; set; } = "";
    /// <summary> Eigener Anzeigename, keine benotete Formulierung. </summary>
    public string Name { get; set; } = "";
    /// <summary> Übergeordnete Gruppenidentität im Projektstrukturplan. </summary>
    public string ParentId { get; set; } = "";
    /// <summary> Eigene Dauer in ausdrücklich vorgegebenen ganzen Arbeitstagen. </summary>
    public string Duration { get; set; } = "";
    /// <summary> Eigener Beginn auf der Zeitachse ab Tag 0. </summary>
    public string Start { get; set; } = "";
    /// <summary> Gewählte Ressourcenkennung. </summary>
    public string Resource { get; set; } = "";
    /// <summary> Eigene FAZ-/FEZ-/SAZ-/SEZ-/Pufferangaben. </summary>
    public Dictionary<string, string> Values { get; set; } = [];
    /// <summary> Vom Lernenden gesetzte Markierung eines kritischen Vorgangs. </summary>
    public bool Critical { get; set; }
}
