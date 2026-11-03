using FiaeTrainer.Core.Sql;

namespace FiaeTrainer.Core.Sql.Workshop;

/// <summary> Ergänzt die gemeinsame Lernaufgabe um die fachlich benötigte Schreib- oder Strukturänderung. </summary>
public sealed class WorkshopLesson : Lesson
{
    /// <summary> Verlangter SQL-Baustein; Ergebnisse allein reichen bei Transaktionen nicht aus. </summary>
    public WorkshopOperation Operation { get; init; }
}
