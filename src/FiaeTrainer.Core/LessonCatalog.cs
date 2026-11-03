using System.Text.Json;

namespace FiaeTrainer.Core;

/// <summary> Lädt die deutschen Missionen aus der eingebetteten Inhaltsdatei. </summary>
public sealed class LessonCatalog
{
    /// <summary> Alle Missionen in Lernreihenfolge. </summary>
    public IReadOnlyList<Lesson> Lessons { get; }

    /// <summary> Lädt und prüft die strukturelle Vollständigkeit des Lernstoffs. </summary>
    public LessonCatalog()
    {
        using Stream stream = OpenContent("lessons.json");
        Lessons = JsonSerializer.Deserialize<List<Lesson>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
        if (Lessons.Count == 0 || Lessons.Select(x => x.Id).Distinct().Count() != Lessons.Count || Lessons.Any(x => x.Hints.Length != 3 || string.IsNullOrWhiteSpace(x.ReferenceSql)))
            throw new InvalidDataException("Der Missionskatalog ist unvollständig oder enthält doppelte Nummern.");
    }

    /// <summary> Öffnet eine in die Bibliothek eingebettete Inhaltsdatei. </summary>
    public static Stream OpenContent(string fileName) => typeof(LessonCatalog).Assembly.GetManifestResourceStream($"FiaeTrainer.Core.Content.{fileName}") ?? throw new FileNotFoundException("Die Inhaltsdatei fehlt.", fileName);
}
