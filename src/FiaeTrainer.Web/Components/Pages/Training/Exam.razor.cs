using FiaeTrainer.Core.Algorithms.Content;
using FiaeTrainer.Core.Exams;
using FiaeTrainer.Core.Sql.Exams;
using FiaeTrainer.Web.Exams;
using FiaeTrainer.Web.Learning;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FiaeTrainer.Web.Components.Pages.Training;

/// <summary> Gemeinsame Prüfungsbedienung, eigene Fachbewertung und gespeicherte UTC-Frist. </summary>
public partial class Exam : IDisposable
{
    #region Fields

    /// <summary> Serialisiert Timer, Entwurfspeicherung und ausdrückliche Aktionen. </summary>
    private readonly SemaphoreSlim _actions = new(1, 1);
    /// <summary> Aktuelle Prüfung und lokaler Editorentwurf. </summary>
    private ExamRound? _round;
    /// <summary> Erkennt Modulwechsel auch dann, wenn Blazor denselben Seitentyp wiederverwendet. </summary>
    private ExamModule? _loadedModule;
    private string _draft = "";
    /// <summary> Lade-, Speicher- und Abgabestatus. </summary>
    private bool _loading = true, _busy, _dirty, _confirm, _stale, _disposed, _focusResult;
    /// <summary> Fehler wird ohne Verlust gespeicherter Antworten angezeigt. </summary>
    private string? _error;
    /// <summary> Aktualisiert Zeit und sichert geänderte Entwürfe ungefähr einmal pro Sekunde. </summary>
    private Timer? _timer;
    /// <summary> Sichtbarer Fokus nach der endgültigen Bewertung. </summary>
    private ElementReference _status;
    #endregion

    #region Properties

    /// <summary> Atomarer Zugriff auf den getrennten Prüfungslernstand. </summary>
    [Inject] private ProgressStore Store { get; set; } = default!;
    /// <summary> Feste lokale Prüfungs- und Rücksprungziele. </summary>
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    /// <summary> Fachliche Aufgaben und Bewertung ohne direkte Datenbanklogik in der Seite. </summary>
    [Inject] private ExamService Exams { get; set; } = default!;
    /// <summary> Nur die beiden fest vorgegebenen lokalen Routen wählen das Modul. </summary>
    private ExamModule Module => Navigation.ToBaseRelativePath(Navigation.Uri).StartsWith("sql/", StringComparison.Ordinal) ? ExamModule.Sql : ExamModule.Algorithms;
    private string Label => Module == ExamModule.Sql ? "SQL" : "Pseudocode";
    private string HomePath => Module == ExamModule.Sql ? "/sql/pruefung" : "/algorithmen/pruefung";
    private string RoundPath => HomePath + "/runde";
    private int CurrentId => _round!.TaskIds[_round.Index];
    private AlgorithmLesson AlgorithmTask => Module == ExamModule.Algorithms ? Exams.AlgorithmTask(CurrentId).Lesson : new();
    private SqlExamTask SqlTask => Exams.SqlCase(_round!).Tasks.Single(task => task.Id == CurrentId);
    private int Points => Module == ExamModule.Sql ? 10 : Exams.AlgorithmTask(CurrentId).Points;
    private string TimeLeft { get { int remaining = _round!.SecondsLeft(DateTimeOffset.UtcNow); return $"{remaining / 60:00}:{remaining % 60:00}"; } }

    #endregion

    #region Methods

    protected override async Task OnParametersSetAsync()
    {
        if (_loadedModule == Module) return;
        _loadedModule = Module;
        _timer?.Dispose();
        _timer = null;
        _loading = true;
        _stale = false;
        _error = null;
        _confirm = false;
        _dirty = false;
        try
        {
            _round = (await Store.LoadAsync()).ExamRounds.GetValueOrDefault(Module);
            if (_round != null)
            {
                if (!Exams.IsCompatible(_round)) { _stale = true; _error = "Der Aufgabenstand dieser Prüfung ist nicht mehr verfügbar. Wähle einen neuen Versuch; deine Lernaufgaben bleiben erhalten."; _round = null; }
                else { _draft = _round.Drafts.GetValueOrDefault(CurrentId, ""); if (!_round.Submitted && _round.SecondsLeft(DateTimeOffset.UtcNow) == 0) await SaveCoreAsync(true); if (_round.Submitted && _round.EvaluatedUtc == null) await EvaluateCoreAsync(); }
            }
        }
        catch (Exception exception) when (IsExpected(exception)) { _error = exception.Message; _stale = exception is not IOException; }
        finally { _loading = false; }
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_timer == null && !_disposed && _round is { Submitted: false }) _timer = new Timer(_ => { if (!_disposed) _ = InvokeAsync(TickAsync); }, null, 1000, 1000);
        if (_focusResult && (_confirm || _round?.EvaluatedUtc != null)) { _focusResult = false; await _status.FocusAsync(); }
    }
    private void SetDraft(string value) { _draft = value; _dirty = true; }
    /// <summary> Hintergrundspeicherung beim Fokuswechsel deaktiviert keine Schaltfläche zwischen Maustaste und Klick. </summary>
    private async Task SaveDraftAsync(FocusEventArgs _)
    {
        if (_disposed || _stale) return;
        await _actions.WaitAsync();
        try { if (_dirty && _round is { Submitted: false }) await SaveCoreAsync(); }
        catch (Exception exception) when (IsExpected(exception)) { _error = exception.Message; _stale = exception is InvalidOperationException; }
        finally { _actions.Release(); }
    }
    /// <summary> Bewahrt neue Eingaben, die während des atomaren Schreibens angekommen sind. </summary>
    private async Task SaveCoreAsync(bool submit = false)
    {
        if (_round == null || _round.Submitted) return;
        string saved = _draft;
        _round.Drafts[CurrentId] = saved;
        _round = await Store.SaveExamAsync(_round, submit);
        if (_round.Submitted) { _draft = _round.Drafts.GetValueOrDefault(CurrentId, ""); _dirty = false; }
        else _dirty = _draft != saved;
    }
    private Task GoAsync(int index) => ActAsync(async () => { if (_round == null) return; if (!_round.Submitted) { _round.Index = Math.Clamp(_round.Index, 0, _round.TaskIds.Count - 1); await SaveCoreAsync(); } _round.Index = Math.Clamp(index, 0, _round.TaskIds.Count - 1); _draft = _round.Drafts.GetValueOrDefault(CurrentId, ""); _dirty = false; if (!_round.Submitted) await SaveCoreAsync(); });
    private Task FlagAsync() => ActAsync(async () => { if (_round == null || _round.Submitted) return; if (!_round.FlaggedIds.Remove(CurrentId)) _round.FlaggedIds.Add(CurrentId); await SaveCoreAsync(); });
    private Task TogglePauseAsync() => ActAsync(async () => { if (_round == null || _round.Submitted) return; if (_round.Paused) _round.RunningSinceUtc = DateTimeOffset.UtcNow; else _round.Pause(DateTimeOffset.UtcNow); await SaveCoreAsync(); });
    /// <summary> Macht auch die am Seitenende ausgelöste Abgabebestätigung sofort sichtbar. </summary>
    private void RequestSubmit() { _confirm = true; _focusResult = true; }
    private Task SubmitAsync() => ActAsync(async () => { _confirm = false; await SaveCoreAsync(true); await EvaluateCoreAsync(); });
    private Task EvaluateAsync() => ActAsync(EvaluateCoreAsync);
    private async Task EvaluateCoreAsync()
    {
        if (_round is not { Submitted: true, EvaluatedUtc: null }) return;
        _round = await Store.CompleteExamAsync(_round, await Exams.EvaluateAsync(_round));
        _focusResult = true;
        _error = null;
    }
    private Task ReferenceAsync() => ActAsync(async () => { if (_round == null || _round.Submitted) return; _round.UsedReference = true; _round.Pause(DateTimeOffset.UtcNow); await SaveCoreAsync(); if (!_round.Submitted) Navigation.NavigateTo(Module == ExamModule.Sql ? "/nachschlagen?pruefung=true" : "/algorithmen/spickzettel?pruefung=true"); });
    /// <summary> Bei abgelaufener Uhr erfolgt die Abgabe auch ohne Benutzereingabe; Änderungen nach der Frist werden nicht übernommen. </summary>
    private async Task TickAsync()
    {
        if (_disposed || _stale || _round is not { Submitted: false } || !await _actions.WaitAsync(0)) return;
        try { if (_dirty || _round.SecondsLeft(DateTimeOffset.UtcNow) == 0) { await SaveCoreAsync(); if (_round.Submitted) await EvaluateCoreAsync(); } StateHasChanged(); }
        catch (Exception exception) when (IsExpected(exception)) { _error = exception.Message; _stale = exception is InvalidOperationException; }
        finally { _actions.Release(); }
    }
    private async Task ActAsync(Func<Task> action)
    {
        if (_stale || _disposed) return;
        await _actions.WaitAsync();
        _busy = true;
        try { await action(); }
        catch (Exception exception) when (IsExpected(exception)) { _error = exception.Message; _stale = exception is InvalidOperationException; }
        finally { _busy = false; _actions.Release(); }
    }
    private static bool IsExpected(Exception exception) => exception is IOException or UnauthorizedAccessException or System.Text.Json.JsonException or InvalidOperationException;
    /// <summary> Beendet nur den Anzeigetimer; die gespeicherte UTC-Frist läuft weiter. </summary>
    public void Dispose() { _disposed = true; _timer?.Dispose(); }

    #endregion
}
