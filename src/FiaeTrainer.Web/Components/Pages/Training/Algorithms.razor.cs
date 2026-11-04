using Microsoft.AspNetCore.Components;
using FiaeTrainer.Core.Algorithms.Content;
using FiaeTrainer.Core.Algorithms.Execution;
using FiaeTrainer.Core.Algorithms.Evaluation;
using FiaeTrainer.Web.Learning;

namespace FiaeTrainer.Web.Components.Pages.Training;

/// <summary> Steuert den ersten Algorithmus-Lernweg unabhängig von SQL Server und SQL-Lernständen. </summary>
public partial class Algorithms : IAsyncDisposable
{
    #region Fields

    /// <summary> Sichtbare Aufgabe und ihr isolierter Lernversuch. </summary>
    private AlgorithmLesson? _lesson;
    /// <summary> Gesamtes Profil für Navigation, Einstellungen und den Einsatzbericht. </summary>
    private TrainingProgress _progress = new();
    /// <summary> Getrennter Stand der ausgewählten Algorithmus-Aufgabe. </summary>
    private AlgorithmProgress _current = new();
    /// <summary> Aktuelle Eingabe; Reparaturaufgaben beginnen mit dem gegebenen fehlerhaften Code. </summary>
    private string _draft = "";
    /// <summary> Hält die Aufgabenliste während späterer asynchroner Wechsel im DOM. </summary>
    private bool _loading = true;
    /// <summary> Verhindert überlappende Abgaben und Moduswechsel. </summary>
    private bool _busy;
    /// <summary> Momentane Anzeige, getrennt von der protokollierten Hilfenutzung. </summary>
    private bool _showSolution;
    /// <summary> Stoppt weitere Schreibversuche nach einem Profilfehler. </summary>
    private string? _storageError;
    /// <summary> Fachliches Feedback des letzten unveränderten Entwurfs. </summary>
    private AlgorithmEvaluation? _evaluation;
    /// <summary> Begrenzte Ablaufspur der letzten Vorschau. </summary>
    private AlgorithmRun? _run;
    /// <summary> Unveränderter Code, zu dem die sichtbare Ablaufspur gehört. </summary>
    private string _previewCode = "";
    /// <summary> Einmal aufgeteilte Codezeilen statt erneuter Zerlegung bei jedem angezeigten Schritt. </summary>
    private string[] _previewLines = [];
    /// <summary> Index des aktuell sichtbaren Ausführungsschritts. </summary>
    private int _step;
    /// <summary> Verhindert verspätete Speicher- und Bewertungszuordnungen nach einem Aufgabenwechsel. </summary>
    private int _revision;

    #endregion

    #region Properties

    /// <summary> Optionale Aufgabennummer aus der Adresse. </summary>
    [Parameter] public int LessonId { get; set; }
    /// <summary> Eigener Aufgabenkatalog des Moduls. </summary>
    [Inject] public AlgorithmCatalog Catalog { get; set; } = default!;
    /// <summary> Begrenzte lokale Ausführung für die Ablaufvorschau. </summary>
    [Inject] public AlgorithmInterpreter Interpreter { get; set; } = default!;
    /// <summary> Fachliche Prüfung der vier Aufgabenarten. </summary>
    [Inject] public AlgorithmEvaluator Evaluator { get; set; } = default!;
    /// <summary> Gemeinsame Datei mit getrennten Fortschrittsbereichen. </summary>
    [Inject] public ProgressStore Store { get; set; } = default!;
    /// <summary> Fachlich vorgegebener Startcode; freie Antworten beginnen leer. </summary>
    private string InitialDraft => _lesson?.Kind == AlgorithmTaskKind.Repair ? _lesson.GivenCode : "";
    /// <summary> Unveränderter Reparaturcode ist noch kein eigener Entwurf und braucht keine Löschbestätigung. </summary>
    private bool HasOwnDraft => !string.IsNullOrWhiteSpace(_draft) && _draft != InitialDraft;
    private string KindLabel => _lesson?.Kind switch { AlgorithmTaskKind.Repair => "Fehlersuche", AlgorithmTaskKind.Trace => "Schreibtischtest", AlgorithmTaskKind.TestCases => "Testfälle entwerfen", _ => "Eigene Lösung" };
    private string EditorLabel => _lesson?.Kind switch { AlgorithmTaskKind.Trace => "Deine vorhergesagten Ausgaben", AlgorithmTaskKind.TestCases => "Deine Testfälle", _ => "Dein Pseudocode" };
    private string EditorPlaceholder => _lesson?.Kind switch { AlgorithmTaskKind.Trace => "Eine Ausgabe pro Zeile …", AlgorithmTaskKind.TestCases => "Zum Beispiel: 50 => zulässig", _ => "Schreibe hier deinen Ablauf …" };

    #endregion

    #region Methods

    protected override async Task OnParametersSetAsync()
    {
        _loading = true;
        int revision = ++_revision;
        int requested = LessonId;
        await SaveAsync();
        _storageError = null;
        TrainingProgress loaded = new();
        try { loaded = await Store.LoadAsync(); }
        catch (Exception exception) when (IsStorageError(exception)) { _storageError = "Dein Lernstand ist nicht lesbar. Du kannst üben; die bestehende Datei wird nicht überschrieben."; }
        if (revision != _revision) return;
        _progress = loaded;
        _lesson = Catalog.Lessons.FirstOrDefault(x => x.Id == (requested == 0 ? loaded.LastAlgorithmLessonId : requested));
        _current = _lesson != null && loaded.AlgorithmLessons.TryGetValue(_lesson.Id, out AlgorithmProgress? saved) ? saved : new();
        _draft = _current.Draft ?? InitialDraft;
        _showSolution = _current.SolutionViewed;
        _busy = false;
        ClearFeedback();
        _loading = false;
    }

    private void ClearFeedback() { _evaluation = null; _run = null; _step = 0; }

    /// <summary> Ein neuer unabhängiger Versuch startet bei Reparaturen wieder mit dem unveränderten Aufgabencode. </summary>
    private async Task SetModeAsync(bool examMode)
    {
        if (_busy) return;
        if (examMode) { _current.StartIndependentAttempt(InitialDraft); _draft = InitialDraft; }
        else { _current.ExamMode = false; _current.AttemptAssisted = true; }
        _showSolution = _current.SolutionViewed;
        ClearFeedback();
        await SaveAsync();
    }

    /// <summary> Zeigt die Beispielausführung erst nach Wahl des geführten Lernens. </summary>
    private async Task PreviewAsync()
    {
        if (_current.ExamMode || _busy || _lesson == null) return;
        _current.AttemptAssisted = true;
        _previewCode = _lesson.Kind is AlgorithmTaskKind.Trace or AlgorithmTaskKind.TestCases ? _lesson.ReferenceCode : _draft;
        _previewLines = _previewCode.Replace("\r", "").Split('\n');
        AlgorithmRun run = Interpreter.Run(_previewCode, _lesson.ExampleInputs, true);
        _run = run;
        _step = 0;
        _evaluation = null;
        await SaveAsync();
    }

    private async Task EvaluateAsync()
    {
        if (_busy || _lesson == null) return;
        _busy = true;
        int revision = _revision;
        AlgorithmLesson lesson = _lesson;
        string draft = _draft;
        try
        {
            await SaveAsync();
            if (revision != _revision) return;
            _evaluation = Evaluator.Evaluate(lesson, draft);
            _run = null;
            _current.Attempts++;
            _current.Completed |= _evaluation.Correct;
            _current.Independent |= _evaluation.Correct && _current.ExamMode && !_current.AttemptAssisted;
            await SaveAsync();
        }
        finally { if (revision == _revision) _busy = false; }
    }

    private async Task ShowHintAsync()
    {
        if (_current.ExamMode || _busy) return;
        if (_current.HintLevel < 3) _current.HintLevel++;
        _current.AttemptAssisted = true;
        await SaveAsync();
    }

    private async Task ToggleSolutionAsync()
    {
        if (_current.ExamMode || _busy) return;
        _showSolution = !_showSolution;
        _current.SolutionViewed |= _showSolution;
        _current.AttemptAssisted = true;
        await SaveAsync();
    }

    /// <summary> Schützt bei beschädigten Dateien den vorhandenen Lernstand vor weiterem Überschreiben. </summary>
    private async Task SaveAsync()
    {
        if (_lesson == null || _storageError != null) return;
        int id = _lesson.Id;
        AlgorithmProgress current = _current;
        current.Draft = _draft;
        _progress.AlgorithmLessons[id] = current;
        try { await Store.SaveAlgorithmAsync(id, current); }
        catch (Exception exception) when (IsStorageError(exception)) { _storageError = "Deine Eingabe konnte nicht gespeichert werden. Kopiere sie vor dem Schließen und prüfe den gespeicherten Lernstand und die Schreibrechte."; }
    }

    private async Task SetCollapsedAsync(bool collapsed)
    {
        _progress.TaskListCollapsed = collapsed;
        if (_storageError != null) return;
        try { await Store.SetTaskListCollapsedAsync(collapsed); }
        catch (Exception exception) when (IsStorageError(exception)) { _storageError = "Die Anzeigeeinstellung konnte nicht gespeichert werden."; }
    }

    private async Task SetHelpHiddenAsync(bool hidden)
    {
        _progress.SelfTestHelpDismissed = hidden;
        if (_storageError != null) return;
        try { await Store.SetSelfTestHelpDismissedAsync(hidden); }
        catch (Exception exception) when (IsStorageError(exception)) { _storageError = "Die Anzeigeeinstellung konnte nicht gespeichert werden."; }
    }

    private static bool IsStorageError(Exception exception) => exception is IOException or System.Text.Json.JsonException or UnauthorizedAccessException;
    /// <summary> Sichert auch bei internen Links zum Spickzettel und beim Verlassen der Aufgabe. </summary>
    public async ValueTask DisposeAsync() { _revision++; await SaveAsync(); }

    #endregion
}
