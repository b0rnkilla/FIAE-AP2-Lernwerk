using FiaeTrainer.Core.Sql;

namespace FiaeTrainer.Core.Sql.Workshop;

/// <summary> Ergänzt die gemeinsame Lernaufgabe um die fachlich benötigte Schreib- oder Strukturänderung. </summary>
public sealed class WorkshopLesson : Lesson
{
    /// <summary> Verlangter SQL-Baustein; Ergebnisse allein reichen bei Transaktionen nicht aus. </summary>
    public WorkshopOperation Operation { get; init; }

    /// <summary> Aufgabenspezifischer Hinweis bei abweichendem Tabellenstand. </summary>
    public string? ResultHint { get; init; }

    /// <summary> Fachfall mit eigenen Ausgangstabellen, leer bei den ersten zehn Aufgaben. </summary>
    public string Scenario { get; init; } = "";
    /// <summary> Zusätzlicher Ausgangszustand aus dem eingebetteten Katalog, vor jedem Versuch neu hergestellt. </summary>
    public string[] Preparation { get; init; } = [];
    /// <summary> Erwartete Arbeitstechnik bei Aufgaben, die ausdrücklich eine Methode verlangen. </summary>
    public string Technique { get; init; } = "";
}
