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

    /// <summary> Zuletzt gespeicherte Aufgabe im Datenbankentwurf. </summary>
    public int LastDesignLessonId { get; set; } = 1;
    /// <summary> Individuelle Modellversuche neben SQL, Algorithmen und WiSo. </summary>
    public Dictionary<int, FiaeTrainer.Core.DatabaseDesign.Practice.DesignProgress> DesignLessons { get; set; } = [];
    /// <summary> Eigene aktuelle Modellprüfung mit strukturierten Entwürfen. </summary>
    public FiaeTrainer.Core.DatabaseDesign.Practice.DesignExamRound? DesignExam { get; set; }
    /// <summary> Letzte zwanzig abgegebene Modellprüfungen. </summary>
    public List<FiaeTrainer.Core.DatabaseDesign.Practice.DesignExamRound> DesignHistory { get; set; } = [];

    /// <summary> Zuletzt besuchte UML-Aufgabe; alte Profile beginnen bei eins. </summary>
    public int LastUmlLessonId { get; set; } = 1;
    /// <summary> UML-Entwürfe und Lernabschlüsse unabhängig von allen bisherigen Modulen. </summary>
    public Dictionary<int, FiaeTrainer.Core.Uml.Practice.UmlProgress> UmlLessons { get; set; } = [];
    /// <summary> Eigene aktuelle UML-Prüfung. </summary>
    public FiaeTrainer.Core.Uml.Practice.UmlExamRound? UmlExam { get; set; }
    /// <summary> Höchstens zwanzig endgültige UML-Ergebnisse. </summary>
    public List<FiaeTrainer.Core.Uml.Practice.UmlExamRound> UmlHistory { get; set; } = [];

    /// <summary> Zuletzt bearbeitete Aufgabe aus Abschnitt 05. </summary>
    public int LastSoftwareLessonId { get; set; } = 1;
    /// <summary> Eigene Softwareplanungsentwürfe neben allen bisherigen Modulen. </summary>
    public Dictionary<int, FiaeTrainer.Core.SoftwareDesign.Practice.SoftwareProgress> SoftwareLessons { get; set; } = [];
    /// <summary> Aktuelle oder zuletzt abgegebene Softwareplanungsprüfung. </summary>
    public FiaeTrainer.Core.SoftwareDesign.Practice.SoftwareExamRound? SoftwareExam { get; set; }
    /// <summary> Höchstens zwanzig endgültige Softwareplanungsergebnisse. </summary>
    public List<FiaeTrainer.Core.SoftwareDesign.Practice.SoftwareExamRound> SoftwareHistory { get; set; } = [];

    /// <summary> Wählt den getrennten Fortschrittsbereich. </summary>
    public Dictionary<int, LessonProgress> GetLessons(LearningArea area) => area == LearningArea.Investigation ? Lessons : WorkshopLessons;
    /// <summary> Letzter besuchter Schritt der Qualitätssicherung. </summary>
    public int LastQualityLessonId { get; set; } = 1;
    /// <summary> Getrennte Testentwürfe und Abschlüsse. </summary>
    public Dictionary<int, FiaeTrainer.Core.QualityAssurance.Practice.QualityProgress> QualityLessons { get; set; } = [];
    /// <summary> Aktuelle oder zuletzt abgegebene Qualitätsprüfung. </summary>
    public FiaeTrainer.Core.QualityAssurance.Practice.QualityExamRound? QualityExam { get; set; }
    /// <summary> Höchstens zwanzig endgültige Qualitätsprüfungen. </summary>
    public List<FiaeTrainer.Core.QualityAssurance.Practice.QualityExamRound> QualityHistory { get; set; } = [];
}
