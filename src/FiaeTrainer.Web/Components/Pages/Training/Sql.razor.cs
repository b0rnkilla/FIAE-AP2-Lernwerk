using Microsoft.AspNetCore.Components;
using FiaeTrainer.Core.Sql;
using FiaeTrainer.Core.Sql.Investigation;
using FiaeTrainer.Core.Sql.Results;
using FiaeTrainer.Web.Learning;
using FiaeTrainer.Web.Sql;

namespace FiaeTrainer.Web.Components.Pages.Training;

/// <summary> Steuert die SQL-Aufgabe; SQL-Bewertung und Speicherung bleiben in eigenen Services. </summary>
public partial class Sql : IAsyncDisposable
{
    #region Fields

    /// <summary> Aktuell ausgewählte Aufgabe. </summary>
    private Lesson? _lesson;
    /// <summary> Geladener Stand des lokalen Profils. </summary>
    private TrainingProgress _progress = new();
    /// <summary> Entwurf und Ergebnisse der aktuellen Aufgabe. </summary>
    private LessonProgress _current = new();
    /// <summary> Inhalt des freien Editors. </summary>
    private string _sql = "";
    /// <summary> Schaltet Erklärung, Hilfen und Probeausführung aus. </summary>
    private bool _examMode = true;
    /// <summary> Trennt einen laufenden Ladevorgang von einer tatsächlich unbekannten Aufgabe. </summary>
    private bool _loading = true;
    /// <summary> Verhindert überlappende Abfragen innerhalb der Seite. </summary>
    private bool _busy;
    /// <summary> Wählt die Erkenntnisliste statt der Tabellenübersicht. </summary>
    private bool _showFindings;
    /// <summary> Sichtbarkeit der Musterlösung; das Schließen nimmt eine bereits protokollierte Hilfenutzung nicht zurück. </summary>
    private bool _showSolution;
    /// <summary> Zuletzt ausgeführte Ergebnistabelle. </summary>
    private QueryResult? _result;
    /// <summary> Zuletzt abgegebene fachliche Bewertung. </summary>
    private Evaluation? _evaluation;
    /// <summary> Sichtbarer Hinweis bei nicht lesbarem oder nicht schreibbarem Lernstand. </summary>
    private string? _storageError;
    /// <summary> Bricht laufende Abfragen beim Verlassen einer Aufgabe ab. </summary>
    private CancellationTokenSource? _queryCancellation;
    /// <summary> Verhindert die Zuordnung verspäteter Ergebnisse zu einer anderen Aufgabe. </summary>
    private int _revision;

    #endregion

    #region Properties

    /// <summary> Optional über die URL gewählte Aufgabennummer. </summary>
    [Parameter] public int LessonId { get; set; }
    /// <summary> Geprüfte Lerninhalte. </summary>
    [Inject] public LessonCatalog Catalog { get; set; } = default!;
    /// <summary> Ausführung und Ergebnisvergleich. </summary>
    [Inject] public QueryEvaluator Evaluator { get; set; } = default!;
    /// <summary> Persistenter lokaler Lernstand. </summary>
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
        catch (Exception exception) when (exception is IOException or System.Text.Json.JsonException or UnauthorizedAccessException)
        {
            _storageError = "Der gespeicherte Lernstand ist nicht lesbar. Du kannst üben; die bestehende Datei wird nicht überschrieben. Hinweise stehen in der Anleitung.";
        }
        if (revision != _revision)
            return;
        _progress = loaded;
        _lesson = Catalog.Lessons.FirstOrDefault(x => x.Id == (requestedId == 0 ? _progress.LastLessonId : requestedId));
        _result = null;
        _evaluation = null;
        _current = _lesson != null && _progress.Lessons.TryGetValue(_lesson.Id, out LessonProgress? saved) ? saved : new LessonProgress();
        _showSolution = _current.SolutionViewed;
        _examMode = _current.ExamMode;
        _current.AttemptAssisted = _current.UsedAssistance || !_examMode;
        _sql = _current.Sql;
        _loading = false;
    }

    /// <summary> Übernimmt die Moduswahl für diese Aufgabe; die UI klärt einen vorhandenen Entwurf vor dem Start eines leeren Selbsttests. </summary>
    private async Task SetModeAsync(bool examMode)
    {
        if (_busy || !examMode && !_examMode)
            return;
        _examMode = examMode;
        _current.ExamMode = examMode;
        if (examMode)
        {
            _current.StartIndependentAttempt();
            _sql = "";
        }
        else
            _current.AttemptAssisted = true;
        _showSolution = _current.SolutionViewed;
        _result = null;
        _evaluation = null;
        await SaveAsync();
    }

    /// <summary> Führt im geführten Modus eine Probeabfrage aus und verwirft verspätete Ergebnisse nach einem Aufgabenwechsel. </summary>
    private async Task PreviewAsync()
    {
        if (_busy || _examMode)
            return;
        _busy = true;
        _evaluation = null;
        int revision = _revision;
        string sql = _sql;
        _queryCancellation?.Dispose();
        _queryCancellation = new CancellationTokenSource();
        CancellationToken token = _queryCancellation.Token;
        try
        {
            await SaveAsync();
            QueryResult result = await Evaluator.PreviewAsync(sql, token);
            if (revision == _revision)
                _result = result;
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        finally { if (revision == _revision) _busy = false; }
    }

    /// <summary> Bewertet die aktuelle Antwort und speichert Abschlüsse nur für die weiterhin ausgewählte Aufgabe. </summary>
    private async Task EvaluateAsync()
    {
        if (_busy || _lesson == null)
            return;
        _busy = true;
        int revision = _revision;
        Lesson lesson = _lesson;
        string sql = _sql;
        _queryCancellation?.Dispose();
        _queryCancellation = new CancellationTokenSource();
        CancellationToken token = _queryCancellation.Token;
        try
        {
            await SaveAsync();
            Evaluation evaluation = await Evaluator.EvaluateAsync(lesson, sql, token);
            if (revision != _revision)
                return;
            _evaluation = evaluation;
            _result = _evaluation.Result;
            if (!_evaluation.TechnicalFailure)
                _current.Attempts++;
            _current.Completed |= _evaluation.Correct;
            _current.Independent |= _evaluation.Correct && _examMode && !_current.UsedAssistance;
            await SaveAsync();
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        finally { if (revision == _revision) _busy = false; }
    }

    private async Task ShowHintAsync()
    {
        if (_current.HintLevel < 3 && !_examMode)
            _current.HintLevel++;
        await SaveAsync();
    }

    /// <summary> Klappt die Lösung auf oder zu und merkt eine einmal erfolgte Anzeige dauerhaft für diesen Versuch. </summary>
    private async Task ToggleSolutionAsync()
    {
        if (_examMode || _busy)
            return;
        _showSolution = !_showSolution;
        _current.SolutionViewed |= _showSolution;
        await SaveAsync();
    }

    /// <summary> Sichert Entwurf und Aufgabenmodus; nach einem Speicherfehler bleibt das vorhandene Profil vor weiteren Schreibversuchen geschützt. </summary>
    private async Task SaveAsync()
    {
        if (_lesson == null || _storageError != null)
            return;
        _current.Sql = _sql;
        _current.ExamMode = _examMode;
        _progress.Lessons[_lesson.Id] = _current;
        try { await Store.SaveLessonAsync(_lesson.Id, _current); }
        catch (Exception exception) when (exception is IOException or System.Text.Json.JsonException or UnauthorizedAccessException)
        {
            _storageError = "Dein Lernstand konnte nicht gespeichert werden. Kopiere deine Abfrage vor dem Schließen und prüfe die Schreibrechte des Projektordners.";
        }
    }

    /// <summary> Merkt die allgemeine Selbsttest-Hilfe im Profil; alle Aufgaben verwenden dieselbe Anzeigeeinstellung. </summary>
    private async Task SetSelfTestHelpHiddenAsync(bool hidden)
    {
        _progress.SelfTestHelpDismissed = hidden;
        if (_storageError != null)
            return;
        try { await Store.SetSelfTestHelpDismissedAsync(hidden); }
        catch (Exception exception) when (exception is IOException or System.Text.Json.JsonException or UnauthorizedAccessException)
        {
            _storageError = "Die Anzeigeeinstellung konnte nicht gespeichert werden. Kopiere deine Abfrage vor dem Schließen und prüfe die Schreibrechte des Projektordners.";
        }
    }

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

    /// <summary> Bricht die aktuelle SQL-Ausführung ab und gibt ihren Abbruchmechanismus frei. </summary>
    private void CancelQuery()
    {
        _queryCancellation?.Cancel();
        _queryCancellation?.Dispose();
        _queryCancellation = null;
        _busy = false;
    }

    /// <summary> Speichert den letzten Entwurf bei interner Navigation und beendet laufende Abfragen. </summary>
    public async ValueTask DisposeAsync()
    {
        _revision++;
        CancelQuery();
        await SaveAsync();
    }

    #endregion
}
