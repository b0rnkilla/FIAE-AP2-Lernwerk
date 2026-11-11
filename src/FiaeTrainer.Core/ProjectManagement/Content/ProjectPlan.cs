namespace FiaeTrainer.Core.ProjectManagement.Content;

/// <summary> Begrenzter eigener Planungsvertrag mit expliziten Zeit- und Kapazitätsannahmen. </summary>
public sealed class ProjectPlan
{
    /// <summary> Vollständige geforderte Rollenmenge. </summary>
    public List<ProjectActivitySpec> Activities { get; set; } = [];
    /// <summary> Sichtbare Ressourcenbezeichnungen. </summary>
    public Dictionary<string, string> Resources { get; set; } = [];
    /// <summary> Verfügbare gleichzeitige Einheiten je Ressource. </summary>
    public Dictionary<string, int> Capacities { get; set; } = [];
    /// <summary> Ende der sichtbaren Zeitachse; Beginn ist immer Tag 0. </summary>
    public int Horizon { get; set; } = 20;
    /// <summary> Verbindlicher spätester Abschluss, falls größer als null. </summary>
    public int Deadline { get; set; }
    /// <summary> Nur ausdrücklich verlangte früheste Planung wird als solche bewertet. </summary>
    public bool RequireEarliest { get; set; }
    /// <summary> Zusätzliche eigene Markierung sämtlicher kritischer Vorgänge. </summary>
    public bool CheckCritical { get; set; }
    /// <summary> Geforderte eigene Zeitangaben: es, ef, ls, lf, tf, ff. </summary>
    public List<string> TimingFields { get; set; } = [];
}
