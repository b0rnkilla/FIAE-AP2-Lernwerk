using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using FiaeTrainer.Core.BusinessStudies.Evaluation;

namespace FiaeTrainer.Core.BusinessStudies.Content;

/// <summary> Lädt und validiert die eigenen WiSo-Inhalte unabhängig von SQL. </summary>
public sealed class BusinessCatalog
{
    /// <summary> Vollständiger Aufgabenbestand in dauerhafter Nummerierung. </summary>
    public IReadOnlyList<BusinessQuestion> Questions { get; }
    /// <summary> Verfügbare Themen mit Einstiegserklärungen. </summary>
    public IReadOnlyList<BusinessTopic> Topics { get; }
    /// <summary> Fachliche Primärquellen der eigenen Inhalte. </summary>
    public IReadOnlyList<BusinessSource> Sources { get; }

    /// <summary> Fehler in eingebetteten Inhalten führen zu einer verständlichen Entwicklungsfehlermeldung. </summary>
    public BusinessCatalog()
    {
        Questions = Read<BusinessQuestion>("questions");
        Topics = Read<BusinessTopic>("topics");
        Sources = Read<BusinessSource>("sources");
        Validate(Questions, Topics, Sources);
    }

    /// <summary> Liest ausschließlich eingebettete, versionierte Ressourcen. </summary>
    private static List<T> Read<T>(string name)
    {
        using Stream stream = typeof(BusinessCatalog).Assembly.GetManifestResourceStream($"FiaeTrainer.Core.BusinessStudies.Content.{name}.json") ?? throw new InvalidDataException("WiSo-Inhalte fehlen.");
        JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new JsonStringEnumConverter());
        return JsonSerializer.Deserialize<List<T>>(stream, options) ?? throw new InvalidDataException("WiSo-Inhalte sind leer.");
    }

    /// <summary> Prüft Kennungen, Referenzen, Antwortverträge und fachlich verwendete Zahlenformate. </summary>
    public static void Validate(IReadOnlyList<BusinessQuestion> questions, IReadOnlyList<BusinessTopic> topics, IReadOnlyList<BusinessSource> sources)
    {
        if (questions.Count == 0 || questions.Select(x => x.Id).Distinct().Count() != questions.Count || topics.Select(x => x.Id).Distinct().Count() != topics.Count || sources.Select(x => x.Id).Distinct().Count() != sources.Count)
            throw new InvalidDataException("WiSo-Kennungen fehlen oder sind doppelt.");
        if (topics.Any(topic => string.IsNullOrWhiteSpace(topic.Title) || string.IsNullOrWhiteSpace(topic.Introduction)) || sources.Any(source => string.IsNullOrWhiteSpace(source.Title) || !Uri.TryCreate(source.Url, UriKind.Absolute, out Uri? uri) || uri.Scheme != "https"))
            throw new InvalidDataException("Themen oder Primärquellen sind unvollständig.");
        foreach (BusinessQuestion question in questions)
        {
            bool invalid = question.Id < 1 || !Enum.IsDefined(question.Kind) || !topics.Any(topic => topic.Id == question.TopicId) || string.IsNullOrWhiteSpace(question.Title) || string.IsNullOrWhiteSpace(question.Scenario) || string.IsNullOrWhiteSpace(question.Task) || string.IsNullOrWhiteSpace(question.Explanation) || string.IsNullOrWhiteSpace(question.MemoryAid) || question.SourceIds.Length == 0 || question.SourceIds.Any(id => !sources.Any(source => source.Id == id)) || !DateOnly.TryParseExact(question.AsOf, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
            invalid |= question.Options.Select(x => x.Id).Distinct().Count() != question.Options.Length || question.Options.Any(x => string.IsNullOrWhiteSpace(x.Id) || string.IsNullOrWhiteSpace(x.Text));
            invalid |= question.Fields.Select(x => x.Id).Distinct().Count() != question.Fields.Length || question.Fields.Any(x => string.IsNullOrWhiteSpace(x.Id) || string.IsNullOrWhiteSpace(x.Label) || string.IsNullOrWhiteSpace(x.Explanation));
            if (question.Kind is BusinessQuestionKind.Single or BusinessQuestionKind.Multiple)
                invalid |= question.Fields.Length != 0 || question.Options.Length < 3 || question.CorrectIds.Length < 1 || question.Kind == BusinessQuestionKind.Single && question.CorrectIds.Length != 1 || question.Kind == BusinessQuestionKind.Multiple && question.CorrectIds.Length < 2 || question.CorrectIds.Distinct().Count() != question.CorrectIds.Length || question.CorrectIds.Any(id => !question.Options.Any(option => option.Id == id)) || question.Options.Any(option => string.IsNullOrWhiteSpace(option.Explanation));
            else if (question.Kind == BusinessQuestionKind.Sequence)
                invalid |= question.Fields.Length != 0 || question.Options.Length < 3 || !question.CorrectOrder.ToHashSet().SetEquals(question.Options.Select(x => x.Id)) || question.CorrectOrder.Length != question.Options.Length;
            else
                invalid |= question.Fields.Length == 0;
            if (question.Kind == BusinessQuestionKind.Assignment)
                invalid |= question.Fields.Any(field => !question.Options.Any(option => option.Id == field.Expected));
            if (question.Kind == BusinessQuestionKind.Number)
                invalid |= question.Fields.Any(field => field.Decimals is < 0 or > 4 || !BusinessEvaluator.TryNumber(field.Expected, out decimal value) || decimal.Round(value, field.Decimals) != value);
            if (question.Kind == BusinessQuestionKind.Date)
                invalid |= question.Fields.Any(field => !DateOnly.TryParseExact(field.Expected, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _));
            if (question.Kind == BusinessQuestionKind.Time)
                invalid |= question.Fields.Any(field => !TimeOnly.TryParseExact(field.Expected, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _));
            if (question.Illustration.Length > 0)
                invalid |= string.IsNullOrWhiteSpace(question.IllustrationDescription) || question.Illustration is not ("line" or "staff" or "matrix" or "warning" or "mandatory" or "prohibition" or "rescue" or "fire");
            invalid |= question.TableRows.Any(row => row.Length != question.TableHeaders.Length) || question.TableHeaders.Length > 0 && question.TableRows.Length == 0;
            if (invalid) throw new InvalidDataException($"WiSo-Aufgabe {question.Id} enthält einen ungültigen Antwortvertrag.");
        }
    }
}
