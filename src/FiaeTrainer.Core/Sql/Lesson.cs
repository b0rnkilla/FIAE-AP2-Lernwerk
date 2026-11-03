namespace FiaeTrainer.Core.Sql;

/// <summary> Beschreibt eine SQL-Aufgabe einschließlich geprüfter Referenzlösung. </summary>
public class Lesson
{
    /// <summary> Stabile Nummer der Aufgabe. </summary>
    public int Id { get; init; }
    /// <summary> Kapitel der Ermittlung. </summary>
    public string Chapter { get; init; } = "";
    /// <summary> Deutscher Titel. </summary>
    public string Title { get; init; } = "";
    /// <summary> Trainierter SQL-Baustein. </summary>
    public string Topic { get; init; } = "";
    /// <summary> Einstieg in die Geschichte ohne Vorwegnahme der Lösung. </summary>
    public string Story { get; init; } = "";
    /// <summary> Fachliche Erklärung im geführten Lernen. </summary>
    public string Briefing { get; init; } = "";
    /// <summary> Eindeutige Aufgabenstellung einschließlich Ausgabespalten. </summary>
    public string Task { get; init; } = "";
    /// <summary> Drei zunehmend konkrete Hilfestellungen. </summary>
    public string[] Hints { get; init; } = [];
    /// <summary> Ausführbare Referenz für die Bewertung; im geführten Modus auf Wunsch als Musterlösung sichtbar. </summary>
    public string ReferenceSql { get; init; } = "";
    /// <summary> Erläuterung der Musterlösung und typischer Fehler. </summary>
    public string Explanation { get; init; } = "";
    /// <summary> Neue Erkenntnis nach erfolgreicher Abgabe. </summary>
    public string Discovery { get; init; } = "";
    /// <summary> Prüft zusätzlich die Reihenfolge der Ergebniszeilen. </summary>
    public bool Ordered { get; init; }
}
