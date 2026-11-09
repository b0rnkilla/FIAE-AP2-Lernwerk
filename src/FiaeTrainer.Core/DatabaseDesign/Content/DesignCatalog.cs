using System.Text.Json;
using FiaeTrainer.Core.DatabaseDesign.Evaluation;

namespace FiaeTrainer.Core.DatabaseDesign.Content;

/// <summary> Getrennter unveränderlicher Bestand eigener Lernaufgaben und Prüfungsfälle. </summary>
public sealed class DesignCatalog
{
    /// <summary> Vertragsversion; inkompatible Änderungen dürfen alte Entwürfe nicht stillschweigend neu bewerten. </summary>
    public const string CONTENT_VERSION = "2026-11-06";
    /// <summary> Zweiundvierzig Lernaufgaben; die ursprünglichen Kennungen 1–30 bleiben unverändert. </summary>
    public IReadOnlyList<DesignAssignment> Lessons { get; } = Read<List<DesignAssignment>>("lessons.json").Concat(Read<List<DesignAssignment>>("diagram-lessons.json")).ToArray();
    /// <summary> Drei eigene Prüfungsfälle, getrennt vom Lernfortschritt. </summary>
    public IReadOnlyList<DesignExamCase> Cases { get; } = Read<List<DesignExamCase>>("exams.json");

    public DesignCatalog()
    {
        var tasks = Lessons.Concat(Cases.SelectMany(item => item.Tasks)).ToArray();
        if (Lessons.Any(task => string.IsNullOrWhiteSpace(task.Story) || string.IsNullOrWhiteSpace(task.Discovery) || task.Id > 1 && string.IsNullOrWhiteSpace(task.PreviousContext)))
            throw new InvalidDataException("Die Geschichte eines Entwurfslernschritts ist unvollständig.");
        if (!Lessons.Select(task => task.Id).SequenceEqual(Enumerable.Range(1, 42)) || Cases.Count != 3 || Cases.Select(item => item.Id).Distinct().Count() != 3 || Cases.Any(item => item.Tasks.Count != 3 || item.Points.Count != 3 || item.Points.Sum() != 50 || item.Tasks.Select(task => task.Id).Distinct().Count() != 3)) throw new InvalidDataException("Der Datenbankentwurf-Katalog ist unvollständig.");
        foreach (var task in tasks)
        {
            if (task.Id < 1 || task.Hints.Count != 3 || string.IsNullOrWhiteSpace(task.Task) || string.IsNullOrWhiteSpace(task.Explanation) || task.Kind is not ("Model" or "ErCompletion" or "Choice" or "Multiple")) throw new InvalidDataException("Ein Entwurfsauftrag ist unvollständig.");
            if (task.Kind == "Model" && (task.Solutions.Count == 0 || task.Solutions.Any(model => DesignModelRules.Errors(model).Count > 0 || model.Tables.SelectMany(table => table.Columns).Any(column => !task.Attributes.ContainsKey(column.Attribute))))) throw new InvalidDataException($"Die Musterlösung von Entwurf {task.Id} ist ungültig.");
            if (task.Kind is "Choice" or "Multiple" && (task.CorrectIds.Count == 0 || task.CorrectIds.Any(id => !task.Options.ContainsKey(id)) || task.Kind == "Choice" && task.CorrectIds.Count != 1)) throw new InvalidDataException("Der Auswahlvertrag fehlt.");
            ValidateMaterial(task);
        }
    }

    /// <summary> Sichert vollständige Unterlagen und eindeutig zugeordnete Diagrammlücken vor dem Anzeigen. </summary>
    private static void ValidateMaterial(DesignAssignment task)
    {
        if (task.Evidence is { } evidence && (string.IsNullOrWhiteSpace(evidence.Title) || evidence.Columns.Count == 0 || evidence.Rows.Count == 0 || evidence.Rows.Any(row => row.Count != evidence.Columns.Count))) throw new InvalidDataException("Eine Arbeitsunterlage ist unvollständig.");
        if (task.ShowStarter && task.Starter.Tables.Count == 0) throw new InvalidDataException("Das vorgegebene relationale Modell fehlt.");
        if (task.Kind == "ErCompletion" && (task.SourceDiagram == null || task.DiagramSlots.Count is < 1 or > 20 || task.DiagramSlots.Any(slot => string.IsNullOrWhiteSpace(slot.Id) || string.IsNullOrWhiteSpace(slot.Prompt) || !slot.Options.ContainsKey(slot.CorrectId)) || task.DiagramSlots.Select(slot => slot.Id).Distinct().Count() != task.DiagramSlots.Count)) throw new InvalidDataException("Der ER-Ergänzungsvertrag fehlt.");
        if (task.SourceDiagram is not { } diagram) return;
        if (diagram.Entities.Count is < 2 or > 8 || diagram.Entities.Any(entity => string.IsNullOrWhiteSpace(entity.Id) || entity.Attributes.Count > 2) || diagram.Entities.Select(entity => entity.Id).Distinct().Count() != diagram.Entities.Count || diagram.Relationships.Count is < 1 or > 8 || diagram.Relationships.Any(relation => relation.Attributes.Count > 1 || !diagram.Entities.Any(entity => entity.Id == relation.Left) || !diagram.Entities.Any(entity => entity.Id == relation.Right))) throw new InvalidDataException("Die ER-Abbildung ist unvollständig oder überschreitet die Darstellungsgrenzen.");
        string[] labels = diagram.Entities.SelectMany(entity => entity.Attributes.Prepend(entity.Name)).Concat(diagram.Relationships.SelectMany(relation => relation.Attributes.Concat([relation.Name, relation.LeftCardinality, relation.RightCardinality]))).ToArray();
        var references = labels.Where(label => label.StartsWith('$')).Select(label => label[1..]).ToHashSet();
        if (!references.SetEquals(task.DiagramSlots.Select(slot => slot.Id)) || task.Kind != "ErCompletion" && references.Count > 0) throw new InvalidDataException("Die ER-Platzhalter und Ergänzungsfelder passen nicht zusammen.");
    }
    /// <summary> Liest ausschließlich mitgelieferte eigene Inhalte aus der Core-Assembly. </summary>
    private static T Read<T>(string name)
    {
        using Stream stream = typeof(DesignCatalog).Assembly.GetManifestResourceStream("FiaeTrainer.Core.DatabaseDesign.Content." + name) ?? throw new InvalidDataException("Der Datenbankentwurf-Inhalt fehlt.");
        return JsonSerializer.Deserialize<T>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new InvalidDataException("Der Datenbankentwurf-Inhalt ist leer.");
    }
}
