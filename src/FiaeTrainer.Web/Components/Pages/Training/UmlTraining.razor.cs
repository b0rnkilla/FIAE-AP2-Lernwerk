using System.Text.Json;
using FiaeTrainer.Core.Uml.Content;
using FiaeTrainer.Core.Uml.Evaluation;
using FiaeTrainer.Core.Uml.Practice;
using FiaeTrainer.Web.Learning;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FiaeTrainer.Web.Components.Pages.Training;

/// <summary> Koordiniert Lernaufgaben, Entwürfe und Rückmeldungen in UML und Modellierung. </summary>
public partial class UmlTraining
{
    #region Fields

    /// <summary> Profilstand für Aufgabenleiste und bestätigte Planungsschritte. </summary>
    private TrainingProgress _state = new();
    /// <summary> Vollständig geladener fachlicher Auftrag. </summary>
    private UmlAssignment? _task;
    /// <summary> Ausschließlich diesem Auftrag zugeordneter Entwurf. </summary>
    private UmlProgress? _progress;
    /// <summary> Rückmeldung der letzten unveränderten Antwort. </summary>
    private UmlEvaluation? _result;
    /// <summary> Sperrt weitere Schreibaktionen nach einem Speicher- oder Versionsfehler. </summary>
    private string? _error;
    /// <summary> Laufende Aktion, sichtbare Hilfen, Listenwahl und ausstehender Rückmeldungsfokus. </summary>
    private bool _busy, _showSolution, _collapsed, _focusFeedback, _loading;
    /// <summary> Sichtbarer Fokus nach abgeschlossener Lösungsprüfung. </summary>
    private ElementReference _feedback;
    /// <summary> Serialisiert Laden, Speichern und Bewerten innerhalb der Seite. </summary>
    private readonly SemaphoreSlim _actions = new(1, 1);
    /// <summary> Geladene Fachkennung, zugehöriger Routenparameter und Schutz gegen überholte Ladeanfragen. </summary>
    private int _loaded, _loadedRouteId = -1, _loadRevision;

    #endregion

    #region Properties

    /// <summary> Verwendet auch beim Fortsetzungsweg ohne ID die tatsächlich geladene Aufgabe. </summary>
    private int CurrentId => _task?.Id ?? 1;
    /// <summary> Verhindert Eingaben während überholter Ladezustände und gesperrter Aktionen. </summary>
    private bool CanEdit => !_busy && !_loading && _error == null && _task != null && _progress != null;
    /// <summary> Leere Auswahlen und vollständig leere Modelle werden nicht versehentlich abgegeben. </summary>
    private bool HasAnswer => _progress != null && (_task?.Format == "Read" ? _progress.Answer.SelectedIds.Count > 0 : _progress.Answer.Model.Nodes.Count > 0);
    /// <summary> Nur ein tatsächlich bearbeiteter Entwurf verlangt eine Neustartbestätigung. </summary>
    private bool HasOwnDraft => _progress != null && _task != null
        && JsonSerializer.Serialize(_progress.Answer) != JsonSerializer.Serialize(_task.InitialAnswer());

    /// <summary> 0 bedeutet Fortsetzen der zuletzt besuchten Aufgabe; positive IDs sind direkte Einstiege. </summary>
    [Parameter] public int Id { get; set; }
    /// <summary> Getrennter Lern- und Prüfungskatalog. </summary>
    [Inject] public UmlCatalog Catalog { get; set; } = default!;
    /// <summary> Atomare profilbezogene Speicherung mit Revisionsschutz. </summary>
    [Inject] public ProgressStore Store { get; set; } = default!;

    #endregion

    #region Methods

    protected override async Task OnParametersSetAsync()
    {
        int requested = Id;
        if (requested == _loadedRouteId) return;
        int revision = ++_loadRevision;
        _loading = true;
        _focusFeedback = false;
        await _actions.WaitAsync();
        try
        {
            if (revision != _loadRevision) return;
            TrainingProgress state = await Store.LoadAsync();
            if (revision != _loadRevision || Id != requested) return;
            int id = requested == 0 ? state.LastUmlLessonId : requested;
            UmlAssignment? task = Catalog.Lessons.SingleOrDefault(item => item.Id == id);
            _state = state;
            _collapsed = state.TaskListCollapsed;
            _task = task;
            _loaded = id;
            _loadedRouteId = requested;
            _result = null;
            _error = null;
            _showSolution = false;
            _progress = task == null ? null : state.UmlLessons.GetValueOrDefault(id) ?? new() { Answer = task.InitialAnswer() };
            if (task == null) _error = "Diese UML-Aufgabe existiert nicht.";
            else if (_progress!.ContentVersion != UmlCatalog.CONTENT_VERSION) _error = "Der gespeicherte Entwurf gehört zu einem älteren Inhaltsvertrag und wird nicht überschrieben.";
        }
        catch (Exception exception) when (IsStorageError(exception))
        {
            if (revision == _loadRevision) _error = "Dein Entwurf konnte nicht geladen werden. Die Datei wird nicht überschrieben.";
        }
        finally
        {
            _actions.Release();
            if (revision == _loadRevision) _loading = false;
        }
    }

    /// <summary> Bindet auch noch laufende Speichervorgänge an ihre ursprüngliche Aufgabe. </summary>
    private async Task SaveAsync()
    {
        if (_error != null || _progress == null || _loading) return;
        int id = _loaded;
        UmlProgress progress = _progress;
        await _actions.WaitAsync();
        try
        {
            await Store.SaveUmlAsync(id, progress, Catalog);
            _state.UmlLessons[id] = progress;
        }
        catch (Exception exception) when (IsStorageError(exception))
        {
            if (_loaded == id) _error = exception.Message;
        }
        finally { _actions.Release(); }
    }

    /// <summary> Zählt Doppelklicks nicht mehrfach und ordnet verspätete Ergebnisse keiner neuen Aufgabe zu. </summary>
    private async Task CheckAsync()
    {
        if (!CanEdit || !HasAnswer) return;
        int id = _loaded;
        int revision = _loadRevision;
        UmlProgress progress = _progress!;
        _busy = true;
        await _actions.WaitAsync();
        try
        {
            var result = await Store.SaveUmlAsync(id, progress, Catalog, check: true);
            _state.UmlLessons[id] = progress;
            if (revision == _loadRevision)
            {
                _result = result;
                _focusFeedback = true;
            }
        }
        catch (Exception exception) when (IsStorageError(exception))
        {
            if (revision == _loadRevision) _error = exception.Message;
        }
        finally { _actions.Release(); _busy = false; }
    }

    private async Task EditorChangedAsync()
    {
        if (!CanEdit) return;
        _result = null;
        _focusFeedback = false;
        await SaveAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!_focusFeedback || _result == null) return;
        _focusFeedback = false;
        try { await _feedback.FocusAsync(); }
        catch (Microsoft.JSInterop.JSDisconnectedException) { }
    }

    /// <summary> Entfernt veraltetes Feedback, wenn sich die Unterstützung des Versuchs ändert. </summary>
    private async Task SetModeAsync(bool independent)
    {
        if (!CanEdit) return;
        _result = null;
        _focusFeedback = false;
        if (independent)
        {
            _progress!.Answer = _task!.InitialAnswer();
            _progress.IndependentMode = true;
            _progress.Assisted = false;
            _progress.HintLevel = 0;
            _progress.SolutionViewed = false;
            _showSolution = false;
        }
        else
        {
            _progress!.IndependentMode = false;
            _progress.Assisted = true;
        }
        await SaveAsync();
    }

    private async Task HintAsync()
    {
        if (!CanEdit || _progress!.IndependentMode || _progress.HintLevel >= 3) return;
        _progress.HintLevel++;
        _progress.Assisted = true;
        await SaveAsync();
    }

    private async Task SolutionAsync()
    {
        if (!CanEdit || _progress!.IndependentMode) return;
        _showSolution = !_showSolution;
        if (!_showSolution) return;
        _progress.SolutionViewed = true;
        _progress.Assisted = true;
        await SaveAsync();
    }

    private async Task SetCollapsedAsync(bool collapsed)
    {
        if (_loading || _error != null) return;
        _collapsed = collapsed;
        try { await Store.SetTaskListCollapsedAsync(collapsed); }
        catch (Exception exception) when (IsStorageError(exception)) { _error = exception.Message; }
    }

    private static bool IsStorageError(Exception exception) => exception is IOException or UnauthorizedAccessException or InvalidOperationException or JsonException;

    #endregion
}
