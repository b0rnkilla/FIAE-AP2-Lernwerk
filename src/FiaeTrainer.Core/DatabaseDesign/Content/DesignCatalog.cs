using System.Text.Json;
using FiaeTrainer.Core.DatabaseDesign.Evaluation;

namespace FiaeTrainer.Core.DatabaseDesign.Content;

/// <summary> Getrennter unveränderlicher Bestand eigener Lernaufgaben und Prüfungsfälle. </summary>
public sealed class DesignCatalog
{
    /// <summary> Vertragsversion; inkompatible Änderungen dürfen alte Entwürfe nicht stillschweigend neu bewerten. </summary>
    public const string CONTENT_VERSION = "2026-11-06";
    /// <summary> Dreißig Lernaufgaben mit stabilen IDs. </summary>
    public IReadOnlyList<DesignAssignment> Lessons { get; } = Read<List<DesignAssignment>>("lessons.json");
    /// <summary> Drei eigene Prüfungsfälle, getrennt vom Lernfortschritt. </summary>
    public IReadOnlyList<DesignExamCase> Cases { get; } = Read<List<DesignExamCase>>("exams.json");

    public DesignCatalog()
    {
        var tasks = Lessons.Concat(Cases.SelectMany(item => item.Tasks)).ToArray();
        if (Lessons.Count != 30 || Lessons.Select(task => task.Id).Distinct().Count() != 30 || Cases.Count != 3 || Cases.Select(item => item.Id).Distinct().Count() != 3 || Cases.Any(item => item.Tasks.Count != 3 || item.Points.Count != 3 || item.Points.Sum() != 50 || item.Tasks.Select(task => task.Id).Distinct().Count() != 3)) throw new InvalidDataException("Der Datenbankentwurf-Katalog ist unvollständig.");
        foreach (var task in tasks)
        {
            if (task.Id < 1 || task.Hints.Count != 3 || string.IsNullOrWhiteSpace(task.Task) || string.IsNullOrWhiteSpace(task.Explanation) || task.Kind is not ("Model" or "Choice" or "Multiple")) throw new InvalidDataException("Ein Entwurfsauftrag ist unvollständig.");
            if (task.Kind == "Model" && (task.Solutions.Count == 0 || task.Solutions.Any(model => DesignModelRules.Errors(model).Count > 0 || model.Tables.SelectMany(table => table.Columns).Any(column => !task.Attributes.ContainsKey(column.Attribute))))) throw new InvalidDataException($"Die Musterlösung von Entwurf {task.Id} ist ungültig.");
            if (task.Kind != "Model" && (task.CorrectIds.Count == 0 || task.CorrectIds.Any(id => !task.Options.ContainsKey(id)) || task.Kind == "Choice" && task.CorrectIds.Count != 1)) throw new InvalidDataException("Der Auswahlvertrag fehlt.");
        }
    }
    /// <summary> Liest ausschließlich mitgelieferte eigene Inhalte aus der Core-Assembly. </summary>
    private static T Read<T>(string name)
    {
        using Stream stream = typeof(DesignCatalog).Assembly.GetManifestResourceStream("FiaeTrainer.Core.DatabaseDesign.Content." + name) ?? throw new InvalidDataException("Der Datenbankentwurf-Inhalt fehlt.");
        return JsonSerializer.Deserialize<T>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new InvalidDataException("Der Datenbankentwurf-Inhalt ist leer.");
    }
}
