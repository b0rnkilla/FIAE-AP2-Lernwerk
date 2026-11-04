using FiaeTrainer.Core.BusinessStudies.Content;
using FiaeTrainer.Core.BusinessStudies.Evaluation;
using FiaeTrainer.Core.BusinessStudies.Practice;
using FiaeTrainer.Web.Learning;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FiaeTrainer.Web.Components.Pages.Training;

/// <summary> Führt fortsetzbare WiSo-Runden mit gesperrter Auflösung und ausdrücklicher Abgabe aus. </summary>
public partial class Business : IDisposable
{
    #region Fields

    /// <summary> Verhindert sich überholende Speicher- und Navigationsaktionen im selben Browser. </summary>
    private readonly SemaphoreSlim _actions = new(1, 1);
    /// <summary> Geladene Runde; abhängige Properties werden erst im vorhandenen Rundenzweig gerendert. </summary>
    private BusinessRound? _round;
    /// <summary> Aktualisiert die Prüfungsanzeige, ohne jede Sekunde auf die Datei zu schreiben. </summary>
    private Timer? _timer;
    /// <summary> Sperrt Bedienelemente während einer laufenden Änderung. </summary>
    private bool _busy;
    /// <summary> Verhindert weitere Anzeigeaufrufe nach dem Verlassen der Seite. </summary>
    private bool _disposed;
    /// <summary> Zeigt die ausdrückliche Abgabebestätigung. </summary>
    private bool _confirmFinish;
    /// <summary> Setzt den Fokus nach dem Rendern auf die neue Bestätigung beziehungsweise Auswertung. </summary>
    private bool _focusRoundStatus;
    /// <summary> Aktuell sichtbarer Statusbereich, den auch Tastaturnutzer unmittelbar erreichen. </summary>
    private ElementReference _roundStatus;
    /// <summary> Beendet die Bearbeitung bei einem nicht sicher fortsetzbaren Speicherfehler. </summary>
    private string? _error;
    /// <summary> Rückmeldung zu noch unvollständigen Antwortfeldern. </summary>
    private string? _validation;

    #endregion

    #region Properties

    /// <summary> Eigenständiger fachlicher Katalog. </summary>
    [Inject] private BusinessCatalog Catalog { get; set; } = default!;
    /// <summary> Lokales Profil unter der gemeinsamen Dateisperre. </summary>
    [Inject] private ProgressStore Store { get; set; } = default!;
    /// <summary> Aufgabe an der aktuellen Position der geladenen Runde. </summary>
    private BusinessQuestion CurrentQuestion => QuestionById(_round!.QuestionIds[_round.Index]);
    /// <summary> Bearbeitbare beziehungsweise bereits gesperrte Antwort dieser Aufgabe. </summary>
    private BusinessAnswer CurrentAnswer => Answer(CurrentQuestion.Id);
    /// <summary> Innerhalb der Runde unverändert gemischte Antwortmöglichkeiten. </summary>
    private IReadOnlyList<BusinessOption> CurrentOptions => BusinessRoundBuilder.Options(_round!, CurrentQuestion);
    /// <summary> Aufgedeckte Antworten können erst in einer neuen Runde erneut bearbeitet werden. </summary>
    private bool IsLocked => _round!.FinishedUtc != null || _round.CheckedIds.Contains(CurrentQuestion.Id);
    /// <summary> Zählt fehlende oder formal ungültige Antworten vor der Abgabe. </summary>
    private int UnansweredCount => _round!.QuestionIds.Count(id => !IsComplete(id));
    /// <summary> Gerundeter Anzeigewert der eigenen Trainingspunkte. </summary>
    private string Percentage => _round!.Maximum == 0 ? "0" : (100m * _round.Earned / _round.Maximum).ToString("0.#");
    /// <summary> Zeigt auch die vollen 60 Minuten und vermeidet den Stundenumbruch einer TimeSpan-Minutenkomponente. </summary>
    private string TimeLeft
    {
        get
        {
            int seconds = _round!.SecondsLeft(DateTimeOffset.UtcNow);
            return $"{seconds / 60:00}:{seconds % 60:00}";
        }
    }

    #endregion

    #region Methods

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _round = (await Store.LoadAsync()).BusinessRound;
            if (_round != null && (_round.ContentVersion != "2026-11-04" || _round.QuestionIds.Any(id => !Catalog.Questions.Any(question => question.Id == id))))
            {
                _error = "Diese gespeicherte Runde gehört zu einem anderen Aufgabenstand. Deine Erfolge bleiben erhalten; wähle im WiSo-Training eine neue Runde.";
                return;
            }
            PrepareSequence();
            if (_round is { Kind: BusinessRoundKind.Exam, FinishedUtc: null } && _round.SecondsLeft(DateTimeOffset.UtcNow) == 0) await FinishAsync();
        }
        catch (Exception exception) when (exception is IOException or System.Text.Json.JsonException or UnauthorizedAccessException) { _error = "Deine Runde konnte nicht gelesen werden. Der gespeicherte Lernstand wird nicht überschrieben."; }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Das asynchrone Laden kann erst nach dem ersten Rendern fertig sein.
        if (_timer == null && !_disposed && _round is { Kind: BusinessRoundKind.Exam, FinishedUtc: null } && _error == null)
            _timer = new Timer(_ => { if (!_disposed) _ = InvokeAsync(TickAsync); }, null, 1000, 1000);
        if (_focusRoundStatus && !_disposed)
        {
            _focusRoundStatus = false;
            await _roundStatus.FocusAsync();
        }
    }

    private BusinessQuestion QuestionById(int id) => Catalog.Questions.Single(question => question.Id == id);
    /// <summary> Eine unbearbeitete Antwort existiert erst im Arbeitsspeicher und wird beim nächsten Speichern übernommen. </summary>
    private BusinessAnswer Answer(int id)
    {
        if (!_round!.Answers.TryGetValue(id, out BusinessAnswer? answer)) _round.Answers[id] = answer = new();
        return answer;
    }
    private BusinessEvaluation Evaluation(int id) => BusinessEvaluator.Evaluate(QuestionById(id), _round!.Answers.GetValueOrDefault(id) ?? new());
    private bool IsComplete(int id) => _round!.Answers.TryGetValue(id, out BusinessAnswer? answer) && BusinessEvaluator.Evaluate(QuestionById(id), answer).Complete;

    /// <summary> Eine Sortieraufgabe startet mit einer stabilen, ausdrücklich nicht bereits richtigen Reihenfolge. </summary>
    private void PrepareSequence()
    {
        if (_round == null || _round.Paused || _round.FinishedUtc != null) return;
        BusinessQuestion question = CurrentQuestion;
        if (question.Kind != BusinessQuestionKind.Sequence || CurrentAnswer.Order.Count > 0 || IsLocked) return;
        CurrentAnswer.Order = CurrentOptions.Select(option => option.Id).ToList();
        if (CurrentAnswer.Order.SequenceEqual(question.CorrectOrder))
            CurrentAnswer.Order = CurrentAnswer.Order.Skip(1).Concat(CurrentAnswer.Order.Take(1)).ToList();
    }

    private Task SaveAsync() => SaveChangeAsync(() => { _validation = null; });
    private Task SetUnsureAsync(ChangeEventArgs args) => SaveChangeAsync(() => CurrentAnswer.Unsure = args.Value is true);
    private Task ToggleFlagAsync() => SaveChangeAsync(() => { if (!_round!.FlaggedIds.Remove(CurrentQuestion.Id)) _round.FlaggedIds.Add(CurrentQuestion.Id); });
    private Task GoAsync(int index) => SaveChangeAsync(() => { _round!.Index = Math.Clamp(index, 0, _round.QuestionIds.Count - 1); _validation = null; PrepareSequence(); });
    private Task TogglePauseAsync() => SaveChangeAsync(() =>
    {
        if (_round!.Paused) _round.RunningSinceUtc = DateTimeOffset.UtcNow;
        else _round.Pause(DateTimeOffset.UtcNow);
        PrepareSequence();
    });

    private async Task CheckAsync()
    {
        if (_round == null || IsLocked) return;
        if (!BusinessEvaluator.Evaluate(CurrentQuestion, CurrentAnswer).Complete)
        {
            _validation = "Bitte beantworte alle Teile im angegebenen Format. Bei Mehrfachauswahl ist die genaue Anzahl erforderlich; bei Zahlen keine Tausendertrennzeichen verwenden.";
            return;
        }
        await SaveChangeAsync(() => _validation = null, CurrentQuestion.Id);
    }
    private Task FinishAsync() => SaveChangeAsync(() => _confirmFinish = false, finish: true);

    /// <summary> Macht die Abgabebestätigung auch beim Auslösen am Seitenende sofort sichtbar. </summary>
    private void RequestFinish()
    {
        _confirmFinish = true;
        _focusRoundStatus = true;
    }

    /// <summary> Persistiert auch vor Aufgabenwechseln; nach Abgabe ändert sich nur die Leseposition im Arbeitsspeicher. </summary>
    private async Task SaveChangeAsync(Action change, int? checkId = null, bool finish = false)
    {
        await _actions.WaitAsync();
        try
        {
            if (_round == null || _error != null || _disposed) return;
            _busy = true;
            bool wasFinished = _round.FinishedUtc != null;
            change();
            if (_round.FinishedUtc == null) await Store.SaveBusinessRoundAsync(_round, Catalog, checkId, finish);
            if (_round.FinishedUtc != null)
            {
                _timer?.Dispose();
                if (!wasFinished) _focusRoundStatus = true;
            }
        }
        catch (Exception exception) when (exception is IOException or System.Text.Json.JsonException or UnauthorizedAccessException or InvalidOperationException)
        {
            _error = exception is InvalidOperationException ? exception.Message : "Deine letzte Änderung konnte nicht gespeichert werden. Prüfe die Schreibrechte und lade den gespeicherten Stand neu.";
        }
        finally { _busy = false; _actions.Release(); }
    }

    /// <summary> Die Uhr tickt nur für die Anzeige; die Abgabe prüft zusätzlich die UTC-basierte Frist. </summary>
    private async Task TickAsync()
    {
        if (_disposed || _round is not { Kind: BusinessRoundKind.Exam, FinishedUtc: null } || _error != null) return;
        if (_round.SecondsLeft(DateTimeOffset.UtcNow) == 0 && !_busy) await FinishAsync();
        if (!_disposed) StateHasChanged();
    }
    private string OverviewClass(int id, int index) => "business-question-number" + (index == _round!.Index ? " current" : "") + (_round.CheckedIds.Contains(id) ? Evaluation(id).Correct ? " correct" : " retry" : "");
    private string OverviewLabel(int id, int index) => $"Aufgabe {index + 1}: {QuestionById(id).Title}" + (_round!.FlaggedIds.Contains(id) ? ", zur Kontrolle markiert" : "") + (_round.CheckedIds.Contains(id) ? ", geprüft" : IsComplete(id) ? ", vollständig beantwortet" : ", noch offen");
    /// <summary> Beendet nur die Anzeigeuhr dieser Komponente; die gespeicherte Prüfungszeit läuft ohne Pause weiter. </summary>
    public void Dispose() { _disposed = true; _timer?.Dispose(); }

    #endregion
}
