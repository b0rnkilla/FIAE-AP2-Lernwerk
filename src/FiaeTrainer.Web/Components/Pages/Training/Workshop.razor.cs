using FiaeTrainer.Web.Components.Learning;
using Microsoft.AspNetCore.Components;
using FiaeTrainer.Core.Sql.Workshop;
using FiaeTrainer.Web.Learning;
using FiaeTrainer.Web.Sql;

namespace FiaeTrainer.Web.Components.Pages.Training;

/// <summary> Steuert isolierte Schreibübungen; laufende Versuche werden bei einem Aufgabenwechsel abgebrochen. </summary>
public partial class Workshop : IAsyncDisposable
{
    #region Fields

    /// <summary> Aktuelle Aufgabe. </summary>
    private WorkshopLesson? _lesson;
    /// <summary> Lokaler Lernstand für die Fortschrittsanzeige. </summary>
    private TrainingProgress _progress = new();
    /// <summary> Entwurf und Hilfen der aktuellen Aufgabe. </summary>
    private LessonProgress _current = new();
    /// <summary> Inhalt des freien Editors. </summary>
    private string _sql = "";
    private CodeEditor? _editor;
    private ElementReference _output;
    private bool _focusOutput;
    /// <summary> Blendet Unterstützung und Probeausführung aus. </summary>
    private bool _examMode = true;
    /// <summary> Klappt die Lösung unabhängig von der bereits gespeicherten Hilfenutzung auf und zu. </summary>
    private bool _showSolution;
    /// <summary> Verhindert eine irreführende Fehlermeldung, während die Aufgabe noch geladen wird. </summary>
    private bool _loading = true;
    /// <summary> Verhindert überlappende Bedienaktionen innerhalb derselben Aufgabe. </summary>
    private bool _busy;
    /// <summary> Stabile Ausgangsdaten für die Aufgabenstellung. </summary>
    private WorkshopRun? _initial;
    /// <summary> Letzter tatsächlicher Endzustand eines Versuchs. </summary>
    private WorkshopRun? _run;
    /// <summary> Letzte fachliche Bewertung. </summary>
    private WorkshopEvaluation? _evaluation;
    /// <summary> Fehler bei der Fortschrittsspeicherung. </summary>
    private string? _storageError;
    /// <summary> Kontrollierter Abbruch laufender SQL-Ausführungen. </summary>
    private CancellationTokenSource? _queryCancellation;
    /// <summary> Trennt Ergebnisse verschiedener Aufgaben auch bei schnellen Seitenwechseln. </summary>
    private int _revision;

    #endregion

    #region Properties

    /// <summary> Optional über die URL gewählte Aufgabennummer. </summary>
    [Parameter] public int LessonId { get; set; }
    /// <summary> Geprüfte Schreibaufgaben. </summary>
    [Inject] public WorkshopCatalog Catalog { get; set; } = default!;
    /// <summary> Isolierte Ausführung und Endzustandsprüfung. </summary>
    [Inject] public WorkshopEvaluator Evaluator { get; set; } = default!;
    /// <summary> Getrennter persistenter Fortschritt. </summary>
    [Inject] public ProgressStore Store { get; set; } = default!;
    /// <summary> Verfügbarkeit der Trainingsdatenbank. </summary>
    [Inject] public DatabaseState Database { get; set; } = default!;

    #endregion

    #region Methods

    protected override async Task OnParametersSetAsync()
    {
        _loading = true;
        int revision = ++_revision;
        int requestedId = LessonId;
        CancelQuery();
        await SaveAsync();
        _storageError = null;
        TrainingProgress loaded = new();
        try { loaded = await Store.LoadAsync(); }
        catch (Exception exception) when (IsStorageError(exception))
        {
            _storageError = "Dein Lernstand konnte nicht gelesen werden. Du kannst üben; die bestehende Datei wird nicht überschrieben. Hinweise stehen in der Anleitung.";
        }
        if (revision != _revision)
            return;
        _progress = loaded;
        _lesson = Catalog.Lessons.FirstOrDefault(x => x.Id == (requestedId == 0 ? loaded.LastWorkshopLessonId : requestedId));
        _current = _lesson != null && loaded.WorkshopLessons.TryGetValue(_lesson.Id, out LessonProgress? saved) ? saved : new LessonProgress();
        _showSolution = _current.SolutionViewed;
        _examMode = _current.ExamMode;
        _current.AttemptAssisted = _current.UsedAssistance || !_examMode;
        _sql = _current.Sql;
        if (_current.CanReview)
        {
            _examMode = _current.ExamMode = true;
            _sql = _current.IndependentSolution ?? _sql;
            _showSolution = false;
        }
        _loading = false;
        _focusOutput = false;
        _run = null;
        _evaluation = null;
        _initial = null;
        if (_lesson == null || !Database.Ready)
            return;
        _busy = true;
        _queryCancellation = new CancellationTokenSource();
        CancellationToken token = _queryCancellation.Token;
        try
        {
            WorkshopRun initial = await Evaluator.GetInitialStateAsync(_lesson, token);
            if (revision == _revision)
                _initial = initial;
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        finally { if (revision == _revision) _busy = false; }
    }

    /// <summary> Speichert die Moduswahl dieser Aufgabe; ein neuer Selbsttest erhält Erfolge und leert den zuvor durch die UI geklärten Entwurf. </summary>
    private async Task SetModeAsync(bool examMode)
    {
        if (_busy || !examMode && !_examMode)
            return;
        _examMode = examMode;
        _current.ExamMode = examMode;
        if (examMode)
        {
            _current.RepeatInProgress = _current.Independent;
            _current.StartIndependentAttempt();
            _sql = "";
        }
        else
        {
            if (_current.CanReview) _current.BeginReading(_sql);
            _current.AttemptAssisted = true;
        }
        _showSolution = _current.SolutionViewed;
        _focusOutput = false;
        _run = null;
        _evaluation = null;
        await SaveAsync();
    }

    /// <summary> Führt das Skript als Probe oder bewertete Lösung aus; überholte Ergebnisse werden keiner anderen Aufgabe zugeordnet. </summary>
    private async Task RunAsync(bool grade)
    {
        if (_busy || _lesson == null || _current.ShowingSavedAnswer || _examMode && !grade)
            return;
        _busy = true;
        _evaluation = null;
        int revision = _revision;
        WorkshopLesson lesson = _lesson;
        string sql = _sql;
        _queryCancellation?.Dispose();
        _queryCancellation = new CancellationTokenSource();
        CancellationToken token = _queryCancellation.Token;
        try
        {
            if (!grade && _editor != null)
            {
                string selection = await _editor.SelectedTextAsync();
                if (selection.Length > 0) sql = selection;
            }
            if (revision != _revision) return;
            await SaveAsync();
            if (grade)
            {
                WorkshopEvaluation evaluation = await Evaluator.EvaluateAsync(lesson, sql, token);
                if (revision != _revision)
                    return;
                _evaluation = evaluation;
                _run = evaluation.Run;
                if (_current.IsReading) return;
                if (!evaluation.TechnicalFailure)
                    _current.Attempts++;
                _current.Completed |= evaluation.Correct;
                bool independentSuccess = evaluation.Correct && _examMode && !_current.UsedAssistance;
                if (independentSuccess) _current.RememberIndependentSolution(sql);
                await PersistAsync(independentSuccess ? sql : null);
            }
            else
            {
                WorkshopRun run = await Evaluator.PreviewAsync(lesson, sql, token);
                if (revision == _revision)
                {
                    _run = run;
                    _focusOutput = true;
                }
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        finally { if (revision == _revision) _busy = false; }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!_focusOutput || _run == null) return;
        _focusOutput = false;
        try { await _output.FocusAsync(); }
        catch (Microsoft.JSInterop.JSDisconnectedException) { }
    }

    private void ReturnToAnswer()
    {
        if (_busy || !_current.IsReading) return;
        _sql = _current.ReturnFromReading(_sql);
        _examMode = true;
        _current.ExamMode = true;
        _current.HintLevel = 0;
        _current.SolutionViewed = _showSolution = false;
        _current.AttemptAssisted = false;
        _focusOutput = false;
        _run = null;
        _evaluation = null;
    }

    private async Task ShowHintAsync()
    {
        if (!_examMode && _current.HintLevel < 3)
            _current.HintLevel++;
        await SaveAsync();
    }

    /// <summary> Ändert nur die Sichtbarkeit; eine aufgedeckte Musterlösung bleibt als Unterstützung im Versuch vermerkt. </summary>
    private async Task ToggleSolutionAsync()
    {
        if (_examMode || _busy)
            return;
        _showSolution = !_showSolution;
        _current.SolutionViewed |= _showSolution;
        await SaveAsync();
    }

    /// <summary> Sichert nur den Werkstattbereich und vermeidet weitere Schreibversuche nach einem bekannten Speicherfehler. </summary>
    private Task SaveAsync() => PersistAsync(null);

    private async Task PersistAsync(string? verifiedAnswer)
    {
        if (_lesson == null || _storageError != null || _current.IsReading)
            return;
        _current.Sql = _sql;
        _current.ExamMode = _examMode;
        _progress.WorkshopLessons[_lesson.Id] = _current;
        try { await Store.SaveLessonAsync(_lesson.Id, _current, LearningArea.Workshop, verifiedAnswer); }
        catch (Exception exception) when (IsStorageError(exception))
        {
            _storageError = "Der Lernstand konnte nicht gespeichert werden. Kopiere dein Skript vor dem Schließen und prüfe die Hinweise in der Anleitung.";
        }
    }

    /// <summary> Verwendet für die allgemeine Selbsttest-Hilfe dieselbe Profileinstellung wie die Ermittlungsaufgaben. </summary>
    private async Task SetSelfTestHelpHiddenAsync(bool hidden)
    {
        _progress.SelfTestHelpDismissed = hidden;
        if (_storageError != null)
            return;
        try { await Store.SetSelfTestHelpDismissedAsync(hidden); }
        catch (Exception exception) when (IsStorageError(exception))
        {
            _storageError = "Die Anzeigeeinstellung konnte nicht gespeichert werden. Kopiere dein Skript vor dem Schließen und prüfe die Schreibrechte.";
        }
    }

    private async Task DismissWorkshopCopyHelpAsync()
    {
        try
        {
            await Store.SetWorkshopCopyHelpDismissedAsync(true);
            _progress.WorkshopCopyHelpDismissed = true;
        }
        catch (Exception exception) when (IsStorageError(exception))
        {
            _storageError = "Der Hinweis zur Arbeitskopie konnte nicht ausgeblendet werden. Prüfe die Schreibrechte für deinen Lernstand.";
        }
    }

    private static bool IsStorageError(Exception exception) => exception is IOException or System.Text.Json.JsonException or UnauthorizedAccessException;

    /// <summary> Merkt den Platzbedarf der Aufgabenliste gemeinsam für beide SQL-Lernbereiche. </summary>
    private async Task SetTaskListCollapsedAsync(bool collapsed)
    {
        _progress.TaskListCollapsed = collapsed;
        if (_storageError != null)
            return;
        try { await Store.SetTaskListCollapsedAsync(collapsed); }
        catch (Exception exception) when (exception is IOException or System.Text.Json.JsonException or UnauthorizedAccessException)
        {
            _storageError = "Die Anzeigeeinstellung konnte nicht gespeichert werden. Kopiere deine Abfrage vor dem Schließen und prüfe die Schreibrechte des Projektordners.";
        }
    }

    /// <summary> Fordert den Abbruch der aktuellen Arbeitskopie an und gibt den Abbruchmechanismus frei. </summary>
    private void CancelQuery()
    {
        _queryCancellation?.Cancel();
        _queryCancellation?.Dispose();
        _queryCancellation = null;
        _busy = false;
    }

    /// <summary> Sichert den letzten Entwurf und beendet die Arbeitskopie beim Verlassen der Seite. </summary>
    public async ValueTask DisposeAsync()
    {
        _revision++;
        CancelQuery();
        await SaveAsync();
    }

    #endregion
}
