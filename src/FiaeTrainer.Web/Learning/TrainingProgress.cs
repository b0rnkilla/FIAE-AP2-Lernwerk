namespace FiaeTrainer.Web.Learning;

/// <summary> Lernstand des lokalen Profils. </summary>
public sealed class TrainingProgress
{
    /// <summary> Gemeinsame Anzeigeeinstellung der Aufgabenliste für Ermittlung und Werkstatt. </summary>
    public bool TaskListCollapsed { get; set; }
    /// <summary> Blendet den allgemeinen Bedienhinweis in beiden SQL-Lernwegen aus, bis er wieder eingeschaltet wird. </summary>
    public bool SelfTestHelpDismissed { get; set; }
    /// <summary> Zuletzt bearbeitete Aufgabe. </summary>
    public int LastLessonId { get; set; } = 1;
    /// <summary> Ergebnisse und Entwürfe nach Aufgabennummer. </summary>
    public Dictionary<int, LessonProgress> Lessons { get; set; } = [];
    /// <summary> Zuletzt bearbeitete Werkstattaufgabe; getrennt von der Ermittlungsakte. </summary>
    public int LastWorkshopLessonId { get; set; } = 1;
    /// <summary> Entwürfe und Erfolge der SQL-Werkstatt. </summary>
    public Dictionary<int, LessonProgress> WorkshopLessons { get; set; } = [];

    /// <summary> Wählt den getrennten Fortschrittsbereich. </summary>
    public Dictionary<int, LessonProgress> GetLessons(LearningArea area) => area == LearningArea.Investigation ? Lessons : WorkshopLessons;
}
