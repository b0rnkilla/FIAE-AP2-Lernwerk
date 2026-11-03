using System.Text.Json;
using FiaeTrainer.Core.Sql;
using FiaeTrainer.Core.Sql.Content;

namespace FiaeTrainer.Core.Sql.Investigation;

/// <summary> Lädt die deutschen Aufgaben aus der eingebetteten Inhaltsdatei. </summary>
public sealed class LessonCatalog
{
    /// <summary> Alle Aufgaben in Lernreihenfolge. </summary>
    public IReadOnlyList<Lesson> Lessons { get; }

    /// <summary> Lädt und prüft die strukturelle Vollständigkeit des Lernstoffs. </summary>
    public LessonCatalog()
    {
        using Stream stream = SqlContent.Open("lessons.json");
        Lessons = JsonSerializer.Deserialize<List<Lesson>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
        if (Lessons.Count == 0 || Lessons.Select(x => x.Id).Distinct().Count() != Lessons.Count || Lessons.Any(x => x.Hints.Length != 3 || string.IsNullOrWhiteSpace(x.ReferenceSql)))
            throw new InvalidDataException("Der Aufgabenkatalog ist unvollständig oder enthält doppelte Nummern.");
    }
}
