using System.Text.Json;

namespace FiaeTrainer.Web;

/// <summary> Speichert den lokalen Einzelbenutzer-Lernstand atomar in einer JSON-Datei. </summary>
public sealed class ProgressStore(IWebHostEnvironment environment, IConfiguration? configuration = null)
{
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

    /// <summary> Aktualisiert eine Mission, ohne zwischenzeitliche Änderungen anderer Missionen zu verlieren. </summary>
    public async Task SaveLessonAsync(int id, LessonProgress progress, LearningArea area = LearningArea.Investigation)
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

    /// <summary> Speichert die Anzeige der allgemeinen Selbsttest-Hilfe für beide Lernbereiche, ohne Aufgabenstände zu überschreiben. </summary>
    public async Task SetSelfTestHelpDismissedAsync(bool dismissed)
    {
        await _gate.WaitAsync();
        try
        {
            TrainingProgress state = await ReadFileAsync();
            state.SelfTestHelpDismissed = dismissed;
            await WriteFileAsync(state);
        }
        finally { _gate.Release(); }
    }

    /// <summary> Speichert die eingeklappte Aufgabenliste, ohne Entwürfe oder andere Einstellungen zu überschreiben. </summary>
    public async Task SetTaskListCollapsedAsync(bool collapsed)
    {
        await _gate.WaitAsync();
        try
        {
            TrainingProgress state = await ReadFileAsync();
            state.TaskListCollapsed = collapsed;
            await WriteFileAsync(state);
        }
        finally { _gate.Release(); }
    }

    /// <summary> Liest ausschließlich unter der vom Aufrufer gehaltenen Dateisperre. </summary>
    private async Task<TrainingProgress> ReadFileAsync()
    {
        if (!File.Exists(_path))
            return new TrainingProgress();
        await using FileStream stream = File.OpenRead(_path);
        return Validate(await JsonSerializer.DeserializeAsync<TrainingProgress>(stream));
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

    private static TrainingProgress Validate(TrainingProgress? progress)
    {
        if (progress == null || progress.Lessons == null || progress.WorkshopLessons == null || progress.LastLessonId < 1 || progress.LastWorkshopLessonId < 1)
            throw new InvalidDataException("Der Lernstand enthält ungültige Angaben und wird nicht überschrieben.");
        foreach (var entry in progress.Lessons.Concat(progress.WorkshopLessons))
            if (entry.Key < 1 || entry.Value == null || entry.Value.Sql == null || entry.Value.HintLevel is < 0 or > 3 || entry.Value.Attempts < 0 || entry.Value.Independent && !entry.Value.Completed)
                throw new InvalidDataException("Ein Missionseintrag ist beschädigt und wird nicht überschrieben.");
        return progress;
    }

    /// <summary> Setzt nur den Lernstand zurück; die Falltabellen bleiben erhalten. </summary>
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

/// <summary> Lernstand des lokalen Profils. </summary>
public sealed class TrainingProgress
{
    /// <summary> Gemeinsame Anzeigeeinstellung der Aufgabenliste für Ermittlung und Werkstatt. </summary>
    public bool TaskListCollapsed { get; set; }
    /// <summary> Blendet den allgemeinen Bedienhinweis in beiden SQL-Lernwegen aus, bis er wieder eingeschaltet wird. </summary>
    public bool SelfTestHelpDismissed { get; set; }
    /// <summary> Zuletzt bearbeitete Mission. </summary>
    public int LastLessonId { get; set; } = 1;
    /// <summary> Ergebnisse und Entwürfe nach Missionsnummer. </summary>
    public Dictionary<int, LessonProgress> Lessons { get; set; } = [];
    /// <summary> Zuletzt bearbeitete Werkstattaufgabe; getrennt von der Ermittlungsakte. </summary>
    public int LastWorkshopLessonId { get; set; } = 1;
    /// <summary> Entwürfe und Erfolge der SQL-Werkstatt. </summary>
    public Dictionary<int, LessonProgress> WorkshopLessons { get; set; } = [];

    /// <summary> Wählt den getrennten Fortschrittsbereich. </summary>
    public Dictionary<int, LessonProgress> GetLessons(LearningArea area) => area == LearningArea.Investigation ? Lessons : WorkshopLessons;
}

/// <summary> Trennt Fortschritte gleich nummerierter Aufgaben verschiedener SQL-Bereiche. </summary>
public enum LearningArea { Investigation, Workshop }

/// <summary> Unterscheidet Abschluss, eigenständigen Erfolg und Unterstützung im aktuellen Versuch. </summary>
public sealed class LessonProgress
{
    /// <summary> Individueller Lernmodus dieser Aufgabe; neue und ältere Einträge beginnen ohne Hilfe. </summary>
    public bool ExamMode { get; set; } = true;
    /// <summary> Letzter SQL-Entwurf. </summary>
    public string Sql { get; set; } = "";
    /// <summary> Unterstützung im aktuellen Versuch; null kennzeichnet ältere Fortschrittsdateien. </summary>
    public bool? AttemptAssisted { get; set; }
    /// <summary> Ältere Entwürfe gelten vorsichtig als unterstützt, damit ein Neuladen keinen selbstständigen Erfolg erzeugt. </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public bool UsedAssistance => AttemptAssisted ?? (!string.IsNullOrWhiteSpace(Sql) || HintLevel > 0 || SolutionViewed);
    /// <summary> Mindestens eine korrekte Abgabe. </summary>
    public bool Completed { get; set; }
    /// <summary> Mindestens eine korrekte Abgabe im Modus ohne Hilfen. </summary>
    public bool Independent { get; set; }
    /// <summary> Höchste im laufenden Lernversuch angezeigte Hilfestufe. </summary>
    public int HintLevel { get; set; }
    /// <summary> Gibt an, ob die Musterlösung im laufenden Versuch aufgedeckt wurde. </summary>
    public bool SolutionViewed { get; set; }
    /// <summary> Anzahl der abgegebenen Antworten. </summary>
    public int Attempts { get; set; }

    /// <summary> Beginnt einen leeren Selbsttest; bisherige Erfolge und die Anzahl der Abgaben bleiben erhalten. </summary>
    public void StartIndependentAttempt()
    {
        ExamMode = true;
        Sql = "";
        HintLevel = 0;
        SolutionViewed = false;
        AttemptAssisted = false;
    }
}
