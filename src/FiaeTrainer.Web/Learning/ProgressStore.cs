using System.Text.Json;

namespace FiaeTrainer.Web.Learning;

/// <summary> Speichert den lokalen Einzelbenutzer-Lernstand atomar in einer JSON-Datei. </summary>
public sealed partial class ProgressStore(IWebHostEnvironment environment, IConfiguration? configuration = null)
{
    /// <summary> Zweiter Aufgabenwechsel seit Beta 3, nach den ersten vier Ersetzungen folgt Aufgabe 14. </summary>
    private const int ALGORITHM_LESSON_REVISION = 2;

    /// <summary> Serialisiert Schreibzugriffe aus mehreren Browserfenstern. </summary>
    private readonly SemaphoreSlim _gate = new(1, 1);
    /// <summary> Lokaler Speicherort außerhalb der Versionsverwaltung. </summary>
    private readonly string _path = Path.Combine(configuration?["Training:ProgressDirectory"] ?? Path.Combine(environment.ContentRootPath, "Data", "progress"), "progress.json");

    /// <summary> Lädt den Lernstand; beschädigte Dateien werden nicht still überschrieben. </summary>
    public async Task<TrainingProgress> LoadAsync()
    {
        await _gate.WaitAsync();
        try
        {
            return await ReadFileAsync();
        }
        finally { _gate.Release(); }
    }

    /// <summary> Aktualisiert eine Aufgabe, ohne zwischenzeitliche Änderungen anderer Aufgaben zu verlieren. </summary>
    public async Task SaveLessonAsync(int id, LessonProgress progress, LearningArea area = LearningArea.Investigation, string? verifiedAnswer = null)
    {
        await _gate.WaitAsync();
        try
        {
            TrainingProgress state = await ReadFileAsync();
            Dictionary<int, LessonProgress> lessons = state.GetLessons(area);
            if (lessons.TryGetValue(id, out LessonProgress? saved))
            {
                progress.Completed |= saved.Completed;
                progress.Independent |= saved.Independent;
                progress.IndependentSolution = verifiedAnswer ?? saved.IndependentSolution ?? progress.IndependentSolution;
            }
            lessons[id] = progress;
            if (area == LearningArea.Investigation)
                state.LastLessonId = id;
            else
                state.LastWorkshopLessonId = id;
            await WriteFileAsync(state);
        }
        finally { _gate.Release(); }
    }

    /// <summary> Ergänzt Algorithmus-Fortschritte, ohne SQL-Stände oder Anzeigeeinstellungen zu überschreiben. </summary>
    public async Task SaveAlgorithmAsync(int id, AlgorithmProgress progress, string? verifiedAnswer = null)
    {
        await _gate.WaitAsync();
        try
        {
            TrainingProgress state = await ReadFileAsync();
            if (state.AlgorithmLessons.TryGetValue(id, out AlgorithmProgress? saved))
            {
                progress.Completed |= saved.Completed;
                progress.Independent |= saved.Independent;
                progress.IndependentSolution = verifiedAnswer ?? saved.IndependentSolution ?? progress.IndependentSolution;
            }
            state.AlgorithmLessons[id] = progress;
            state.LastAlgorithmLessonId = id;
            await WriteFileAsync(state);
        }
        finally { _gate.Release(); }
    }

    /// <summary> Speichert die Anzeige der allgemeinen Selbsttest-Hilfe für alle Lernbereiche, ohne Aufgabenstände zu überschreiben. </summary>
    public Task SetSelfTestHelpDismissedAsync(bool dismissed) =>
        UpdatePreferenceAsync(state => state.SelfTestHelpDismissed = dismissed);

    /// <summary> Speichert die Sichtbarkeit des Werkstatthinweises unabhängig von der Selbsttest-Hilfe. </summary>
    public Task SetWorkshopCopyHelpDismissedAsync(bool dismissed) =>
        UpdatePreferenceAsync(state => state.WorkshopCopyHelpDismissed = dismissed);

    /// <summary> Speichert die eingeklappte Aufgabenliste, ohne Entwürfe oder andere Einstellungen zu überschreiben. </summary>
    public Task SetTaskListCollapsedAsync(bool collapsed) =>
        UpdatePreferenceAsync(state => state.TaskListCollapsed = collapsed);

    /// <summary> Ändert eine Anzeigeeinstellung unter derselben Dateisperre wie die Aufgabenentwürfe. </summary>
    private async Task UpdatePreferenceAsync(Action<TrainingProgress> update)
    {
        await _gate.WaitAsync();
        try
        {
            TrainingProgress state = await ReadFileAsync();
            update(state);
            await WriteFileAsync(state);
        }
        finally { _gate.Release(); }
    }

    /// <summary> Liest ausschließlich unter der vom Aufrufer gehaltenen Dateisperre. </summary>
    private async Task<TrainingProgress> ReadFileAsync()
    {
        if (!File.Exists(_path))
            return new TrainingProgress { Beta3AlgorithmArchive = [], AlgorithmLessonRevision = ALGORITHM_LESSON_REVISION };
        await using FileStream stream = File.OpenRead(_path);
        TrainingProgress state = Validate(await JsonSerializer.DeserializeAsync<TrainingProgress>(stream));
        if (state.AlgorithmLessonRevision < ALGORITHM_LESSON_REVISION)
        {
            // Die erste Entwicklungsfassung nach Beta 3 führte das Archiv noch ohne Revisionsnummer ein.
            int[] replacedIds = state.Beta3AlgorithmArchive == null ? [1, 2, 3, 12, 14] : [14];
            state.Beta3AlgorithmArchive ??= [];
            foreach (int id in replacedIds)
                if (state.AlgorithmLessons.Remove(id, out AlgorithmProgress? previous))
                    state.Beta3AlgorithmArchive.Add(id, previous);
            state.AlgorithmLessonRevision = ALGORITHM_LESSON_REVISION;
        }
        return state;
    }

    /// <summary> Schreibt das gesamte Profil atomar unter der vom Aufrufer gehaltenen Dateisperre. </summary>
    private async Task WriteFileAsync(TrainingProgress state)
    {
        Validate(state);
        Directory.CreateDirectory(Path.GetDirectoryName(_path) ?? throw new InvalidOperationException("Der Speicherpfad fehlt."));
        string temporaryPath = _path + ".tmp";
        await File.WriteAllTextAsync(temporaryPath, JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true }));
        File.Move(temporaryPath, _path, true);
    }

    /// <summary> Verhindert das Überschreiben strukturell beschädigter Profile; fehlende ältere Einstellungen behalten ihre Vorgaben. </summary>
    private static TrainingProgress Validate(TrainingProgress? progress)
    {
        if (progress == null || progress.Lessons == null || progress.WorkshopLessons == null || progress.AlgorithmLessons == null || progress.LastLessonId < 1 || progress.LastWorkshopLessonId < 1 || progress.LastAlgorithmLessonId < 1)
            throw new InvalidDataException("Der Lernstand enthält ungültige Angaben und wird nicht überschrieben.");
        foreach (var entry in progress.Lessons.Concat(progress.WorkshopLessons))
            if (entry.Key < 1 || entry.Value == null || entry.Value.Sql == null || entry.Value.HintLevel is < 0 or > 3 || entry.Value.Attempts < 0 || entry.Value.Independent && !entry.Value.Completed)
                throw new InvalidDataException("Ein Aufgabeneintrag ist beschädigt und wird nicht überschrieben.");
        foreach (var entry in progress.AlgorithmLessons.Concat(progress.Beta3AlgorithmArchive ?? []))
            if (entry.Key < 1 || entry.Value == null || entry.Value.HintLevel is < 0 or > 3 || entry.Value.Attempts < 0 || entry.Value.Independent && !entry.Value.Completed)
                throw new InvalidDataException("Ein Algorithmus-Eintrag ist beschädigt und wird nicht überschrieben.");
        ValidateBusinessProgress(progress);
        ValidateExamProgress(progress);
        ValidateDesignProgress(progress);
        ValidateUmlProgress(progress);
        ValidateSoftwareProgress(progress);
        ValidateQualityProgress(progress);
        ValidateProjectProgress(progress);
        return progress;
    }

    /// <summary> Löscht das lokale Lernprofil samt Anzeigeeinstellungen; SQL-Datenbank und persönliche Verbindung bleiben erhalten. </summary>
    public async Task ResetAsync()
    {
        await _gate.WaitAsync();
        try
        {
            if (File.Exists(_path))
                File.Delete(_path);
        }
        finally { _gate.Release(); }
    }
}
