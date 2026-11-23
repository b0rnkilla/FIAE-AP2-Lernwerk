using System.Text.Json;
using FiaeTrainer.Core.Algorithms.Syntax;

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
    /// <summary> Die aufrufende Anwendung benötigt einen echten Wahrheitswert als Rückgabe. </summary>
    public bool RequiresBooleanReturn { get; init; }
    /// <summary> Reihenfolge der Testeingaben für den Funktionsaufruf; eigene Parameternamen sind frei wählbar. </summary>
    public string[] FunctionInputs { get; init; } = [];
    /// <summary> Fachlich vorgegebene Parametertypen in Aufrufreihenfolge, in der Kurzform Integer[] für Listen. </summary>
    public string[] ParameterTypes { get; init; } = [];
    /// <summary> Ergebnistyp des Funktionsauftrags. </summary>
    public string ReturnType { get; init; } = "";
    /// <summary> Die Aufgabe verlangt bei aktueller Inhaltsversion einen vollständig typisierten Einstieg. </summary>
    public bool RequiresTypedSignature { get; init; }
    /// <summary> Stellt ausschließlich für den Tabellenauftrag die vier vorgegebenen lokalen Hilfsfunktionen bereit. </summary>
    public bool UsesTableFunctions { get; init; }
    /// <summary> Texttransformationen müssen auch äußere Leerzeichen exakt erhalten beziehungsweise vermeiden. </summary>
    public bool ExactTextOutput { get; init; }
    /// <summary> Gemeinsames Rechenbudget des Auftrags einschließlich aller Hilfsaufrufe. </summary>
    public int MaxOperations { get; init; } = 10000;
    /// <summary> Lesbare Signatur für den Hinweis am Auftrag; eigene Parameternamen bleiben möglich. </summary>
    public string Signature => $"FUNKTION {FunctionName}({string.Join(", ", FunctionInputs.Zip(ParameterTypes, (name, type) => $"{name}: {type}"))}): {ReturnType}";

    /// <summary> Beide Kataloge benötigen einen vollständigen, ausführbaren Vertrag für jede Funktionsaufgabe. </summary>
    internal void ValidateSignature()
    {
        if (FunctionName.Length == 0) return;
        if (FunctionInputs.Length != ParameterTypes.Length || ReturnType.Length == 0)
            throw new InvalidDataException($"Die Typangaben für {FunctionName} sind unvollständig.");
        foreach (string type in ParameterTypes)
            if (AlgorithmType.FromCatalog(type).IsVoid) throw new InvalidDataException("Parameter benötigen einen Werttyp, nicht Void.");
        AlgorithmType.FromCatalog(ReturnType);
    }
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
