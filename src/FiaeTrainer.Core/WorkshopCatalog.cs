using System.Text.Json;

namespace FiaeTrainer.Core;

/// <summary> Lädt die eigenständigen deutschen Schreibübungen ohne Änderungen an der Ermittlungsakte. </summary>
public sealed class WorkshopCatalog
{
    /// <summary> Lernaufgaben in aufsteigender Schwierigkeit. </summary>
    public IReadOnlyList<WorkshopLesson> Lessons { get; }

    /// <summary> Prüft die strukturelle Vollständigkeit der eingebetteten Inhalte. </summary>
    public WorkshopCatalog()
    {
        using Stream stream = LessonCatalog.OpenContent("workshop.json");
        JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        Lessons = JsonSerializer.Deserialize<List<WorkshopLesson>>(stream, options) ?? [];
        if (Lessons.Count == 0 || Lessons.Select(x => x.Id).Distinct().Count() != Lessons.Count || Lessons.Any(x => x.Hints.Length != 3 || string.IsNullOrWhiteSpace(x.ReferenceSql)))
            throw new InvalidDataException("Der Werkstattkatalog ist unvollständig.");
    }
}
