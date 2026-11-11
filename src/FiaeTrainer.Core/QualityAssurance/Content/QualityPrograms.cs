using System.Text.Json;

namespace FiaeTrainer.Core.QualityAssurance.Content;

/// <summary> Öffentliche Verträge der fest implementierten, vollständig lokalen Beispielprogramme. </summary>
public static class QualityPrograms
{
    /// <summary> Acht begrenzte Programme; keine Interpretation oder Ausführung von Nutzercode. </summary>
    public static IReadOnlyList<QualityProgram> All { get; } = Read();
    /// <summary> Liefert ausschließlich eine bekannte Programmkennung. </summary>
    public static QualityProgram? Find(string key) => All.SingleOrDefault(program => program.Key == key);
    private static List<QualityProgram> Read()
    {
        using var stream = typeof(QualityPrograms).Assembly.GetManifestResourceStream("FiaeTrainer.Core.QualityAssurance.Content.programs.json") ?? throw new InvalidDataException("Die lokalen Testmodelle fehlen.");
        return JsonSerializer.Deserialize<List<QualityProgram>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new InvalidDataException("Die Testmodelle sind leer.");
    }
}
