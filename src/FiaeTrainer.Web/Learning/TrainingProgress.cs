using FiaeTrainer.Core.BusinessStudies.Practice;
using FiaeTrainer.Core.Exams;

namespace FiaeTrainer.Web.Learning;

/// <summary> Lernstand des lokalen Profils. </summary>
public sealed class TrainingProgress
{
    /// <summary> Gemeinsame Anzeigeeinstellung der Aufgabenlisten aller Lernbereiche. </summary>
    public bool TaskListCollapsed { get; set; }
    /// <summary> Blendet den allgemeinen Bedienhinweis in allen Lernwegen aus, bis er wieder eingeschaltet wird. </summary>
    public bool SelfTestHelpDismissed { get; set; }
    /// <summary> Zuletzt bearbeitete Aufgabe. </summary>
    public int LastLessonId { get; set; } = 1;
    /// <summary> Ergebnisse und Entwürfe nach Aufgabennummer. </summary>
    public Dictionary<int, LessonProgress> Lessons { get; set; } = [];
    /// <summary> Zuletzt bearbeitete Werkstattaufgabe; getrennt von der Ermittlungsakte. </summary>
    public int LastWorkshopLessonId { get; set; } = 1;
    /// <summary> Entwürfe und Erfolge der SQL-Werkstatt. </summary>
    public Dictionary<int, LessonProgress> WorkshopLessons { get; set; } = [];

    /// <summary> Zuletzt bearbeitete Algorithmus-Aufgabe, unabhängig von den SQL-Bereichen. </summary>
    public int LastAlgorithmLessonId { get; set; } = 1;
    /// <summary> Eigene Entwürfe und Erfolge des Pseudocode-Lernwegs. </summary>
    public Dictionary<int, AlgorithmProgress> AlgorithmLessons { get; set; } = [];

    /// <summary> Getrennte WiSo-Erfolge; ältere Profile beginnen mit einem leeren Bereich. </summary>
    public Dictionary<int, BusinessProgress> BusinessQuestions { get; set; } = [];
    /// <summary> Aktuelle oder zuletzt abgeschlossene WiSo-Runde einschließlich Entwürfen. </summary>
    public BusinessRound? BusinessRound { get; set; }
    /// <summary> Die letzten zwanzig abgegebenen Runden mit ihren damaligen Ergebnissen. </summary>
    public List<BusinessRound> BusinessHistory { get; set; } = [];

    /// <summary> Getrennte SQL- und Algorithmus-Prüfungsrunden; ältere Profile beginnen ohne Prüfungsversuche. </summary>
    public Dictionary<ExamModule, ExamRound> ExamRounds { get; set; } = [];
    /// <summary> Höchstens zwanzig endgültig bewertete Prüfungen je Modul. </summary>
    public List<ExamRound> ExamHistory { get; set; } = [];

    /// <summary> Wählt den getrennten Fortschrittsbereich. </summary>
    public Dictionary<int, LessonProgress> GetLessons(LearningArea area) => area == LearningArea.Investigation ? Lessons : WorkshopLessons;
}
