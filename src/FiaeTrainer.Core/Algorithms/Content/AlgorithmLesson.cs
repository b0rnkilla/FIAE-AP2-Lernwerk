using System.Text.Json;

namespace FiaeTrainer.Core.Algorithms.Content;

/// <summary> Auftrag, Geschichte und Bewertungsdaten einer Pseudocode-Aufgabe. </summary>
public sealed class AlgorithmLesson
{
    /// <summary> Stabile Aufgabennummer. </summary>
    public int Id { get; init; }
    /// <summary> Abschnitt des Lernwegs. </summary>
    public string Chapter { get; init; } = "";
    /// <summary> Kurzer Titel für Navigation und Lernplan. </summary>
    public string Title { get; init; } = "";
    /// <summary> Kennzeichnet Schreiben, Reparieren, Schreibtischtest oder Testfälle. </summary>
    public AlgorithmTaskKind Kind { get; init; }
    /// <summary> Kontext vor der Bearbeitung ohne Vorwegnahme der Lösung. </summary>
    public string Story { get; init; } = "";
    /// <summary> Expliziter Bezug zur vorherigen Aufgabe, falls diese bereits gelöst ist. </summary>
    public string PreviousContext { get; init; } = "";
    /// <summary> Eindeutiger Arbeitsauftrag samt Ausgabeformat. </summary>
    public string Task { get; init; } = "";
    /// <summary> Sichtbarer fehlerhafter beziehungsweise zu untersuchender Ablauf. </summary>
    public string GivenCode { get; init; } = "";
    /// <summary> Eingaben für Vorschau und Schreibtischtest. </summary>
    public Dictionary<string, JsonElement> ExampleInputs { get; init; } = [];
    /// <summary> Fachliche Einführung nur im geführten Modus. </summary>
    public string Briefing { get; init; } = "";
    /// <summary> Drei stufenweise aufdeckbare Denkanstöße. </summary>
    public string[] Hints { get; init; } = [];
    /// <summary> Geprüfter Referenzablauf für Codeaufgaben und die Testfallaufgabe. </summary>
    public string ReferenceCode { get; init; } = "";
    /// <summary> Musterantwort für Schreibtischtests und Testfälle. </summary>
    public string ReferenceAnswer { get; init; } = "";
    /// <summary> Optionaler Funktionsvertrag: Die Bewertung ruft diese Funktion statt eines Hauptablaufs auf. </summary>
    public string FunctionName { get; init; } = "";
    /// <summary> Reihenfolge der Testeingaben für den Funktionsaufruf; eigene Parameternamen sind frei wählbar. </summary>
    public string[] FunctionInputs { get; init; } = [];
    /// <summary> Erläutert die fachliche Entscheidung in der Musterlösung. </summary>
    public string Explanation { get; init; } = "";
    /// <summary> Erkenntnis, die erst nach erfolgreicher Bearbeitung freigeschaltet wird. </summary>
    public string Discovery { get; init; } = "";
    /// <summary> Unabhängig festgelegte Eingaben und Sollausgaben. </summary>
    public AlgorithmTest[] Tests { get; init; } = [];
    /// <summary> Benötigte Grenz- und Normalfallgruppen für selbst entworfene Testfälle. </summary>
    public TestCoverage[] Coverage { get; init; } = [];
    /// <summary> Eingabename der skalaren Testfallaufgabe; das Antwortformat bleibt Zahl => Ausgabe. </summary>
    public string CaseInputName { get; init; } = "gewicht";
}
