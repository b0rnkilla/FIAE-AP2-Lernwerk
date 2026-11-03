using System.Text.Json;
using FiaeTrainer.Core.Sql.Content;

namespace FiaeTrainer.Core.Sql.Workshop;

/// <summary> Lädt die eigenständigen deutschen Schreibübungen ohne Änderungen an der Ermittlungsakte. </summary>
public sealed class WorkshopCatalog
{
    /// <summary> Lernaufgaben in aufsteigender Schwierigkeit. </summary>
    public IReadOnlyList<WorkshopLesson> Lessons { get; }

    /// <summary> Prüft die strukturelle Vollständigkeit der eingebetteten Inhalte. </summary>
    public WorkshopCatalog()
    {
        using Stream stream = SqlContent.Open("workshop.json");
        JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        Lessons = JsonSerializer.Deserialize<List<WorkshopLesson>>(stream, options) ?? [];
        if (Lessons.Count == 0 || Lessons.Select(x => x.Id).Distinct().Count() != Lessons.Count || Lessons.Any(x => x.Hints.Length != 3 || string.IsNullOrWhiteSpace(x.ReferenceSql)))
            throw new InvalidDataException("Der Werkstattkatalog ist unvollständig.");
    }
}
