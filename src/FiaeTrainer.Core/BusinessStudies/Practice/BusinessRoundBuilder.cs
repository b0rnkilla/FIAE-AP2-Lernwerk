using System.Security.Cryptography;
using System.Text;
using FiaeTrainer.Core.BusinessStudies.Content;

namespace FiaeTrainer.Core.BusinessStudies.Practice;

/// <summary> Stellt kurze Lernrunden und thematisch ausgewogene Prüfungen zusammen. </summary>
public static class BusinessRoundBuilder
{
    /// <summary> Wählt ohne Duplikate; Prüfungen enthalten alle acht Themen und verschiedene Antwortformate. </summary>
    public static BusinessRound Create(BusinessCatalog catalog, BusinessRoundKind kind, string topicId = "", IEnumerable<int>? priorityIds = null)
    {
        if (!Enum.IsDefined(kind) || kind == BusinessRoundKind.Topic && !catalog.Topics.Any(topic => topic.Id == topicId))
            throw new ArgumentException("Der gewählte WiSo-Lernweg existiert nicht.");
        List<BusinessQuestion> pool = catalog.Questions.Where(question => kind != BusinessRoundKind.Topic || question.TopicId == topicId).ToList();
        Random.Shared.Shuffle(System.Runtime.InteropServices.CollectionsMarshal.AsSpan(pool));
        List<int> selected = [];
        if (kind == BusinessRoundKind.Exam)
        {
            foreach (BusinessTopic topic in catalog.Topics)
                selected.AddRange(pool.Where(question => question.TopicId == topic.Id).Take(2).Select(question => question.Id));
            foreach (BusinessQuestionKind format in Enum.GetValues<BusinessQuestionKind>())
            {
                BusinessQuestion? additional = pool.FirstOrDefault(question => question.Kind == format && !selected.Contains(question.Id));
                if (additional != null && !selected.Any(id => catalog.Questions.Single(question => question.Id == id).Kind == format)) selected.Add(additional.Id);
            }
        }
        else if (priorityIds != null)
        {
            HashSet<int> priorities = priorityIds.ToHashSet();
            selected.AddRange(pool.Where(question => priorities.Contains(question.Id)).Take(kind == BusinessRoundKind.Review ? 20 : 5).Select(question => question.Id));
        }
        int count = kind switch { BusinessRoundKind.Exam => 30, BusinessRoundKind.Topic => pool.Count, BusinessRoundKind.Review => Math.Min(20, selected.Count), _ => 10 };
        if (kind == BusinessRoundKind.Review && count == 0) throw new InvalidOperationException("Aktuell gibt es keine Antworten zur Fehlerwiederholung.");
        if (kind != BusinessRoundKind.Review) selected.AddRange(pool.Where(question => !selected.Contains(question.Id)).Take(Math.Max(0, count - selected.Count)).Select(question => question.Id));
        int[] order = selected.Take(count).ToArray();
        Random.Shared.Shuffle(order);
        return new() { Kind = kind, TopicId = topicId, QuestionIds = order.ToList(), RunningSinceUtc = kind == BusinessRoundKind.Exam ? DateTimeOffset.UtcNow : null };
    }

    /// <summary> Mischt Auswahlmöglichkeiten reproduzierbar; die Reihenfolge bleibt nach einem Neuladen erhalten. </summary>
    public static IReadOnlyList<BusinessOption> Options(BusinessRound round, BusinessQuestion question)
    {
        return question.Options.OrderBy(option => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(round.Id + ":" + question.Id + ":" + option.Id))), StringComparer.Ordinal).ToArray();
    }
}
