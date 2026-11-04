using System.Globalization;
using System.Text.Json;
using FiaeTrainer.Core.BusinessStudies.Content;
using FiaeTrainer.Core.BusinessStudies.Evaluation;
using FiaeTrainer.Core.BusinessStudies.Practice;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft WiSo-Inhalte mit unabhängigen Rechenergebnissen, aktuellen Rechtsfällen und ungültigen Antworten. </summary>
public sealed class BusinessTests
{
    private static readonly BusinessCatalog CATALOG = new();

    /// <summary> Jede veröffentlichte Aufgabe durchläuft die Ein- und Ausgabeprüfung. </summary>
    public static IEnumerable<object[]> QuestionIds => CATALOG.Questions.Select(question => new object[] { question.Id });

    [Fact]
    public void CatalogContains150DistinctOriginalQuestionsAcrossEightTopicsAndSevenFormats()
    {
        Assert.Equal(Enumerable.Range(1, 150), CATALOG.Questions.Select(question => question.Id));
        Assert.Equal(8, CATALOG.Topics.Count);
        Assert.Equal(Enum.GetValues<BusinessQuestionKind>().Order(), CATALOG.Questions.Select(question => question.Kind).Distinct().Order());
        Assert.All(CATALOG.Topics, topic => Assert.InRange(CATALOG.Questions.Count(question => question.TopicId == topic.Id), 15, 25));
        Assert.Equal(150, CATALOG.Questions.Select(question => question.Task + "|" + question.Scenario).Distinct().Count());
        Assert.All(CATALOG.Questions, question => Assert.NotEmpty(question.SourceIds));
    }

    [Theory]
    [MemberData(nameof(QuestionIds))]
    public void FullReferenceAnswersAreAcceptedAndEmptyAnswersEarnNothing(int id)
    {
        BusinessQuestion question = CATALOG.Questions.Single(question => question.Id == id);
        BusinessEvaluation reference = BusinessEvaluator.Evaluate(question, ReferenceAnswer(question));
        Assert.True(reference.Correct, $"Aufgabe {id}: Referenz wird nicht akzeptiert.");
        Assert.Equal(question.Maximum, reference.Earned);
        BusinessEvaluation empty = BusinessEvaluator.Evaluate(question, new());
        Assert.False(empty.Correct);
        Assert.False(empty.Complete);
        Assert.Equal(0, empty.Earned);
    }

    [Theory]
    [MemberData(nameof(QuestionIds))]
    public void AlteringEachReferencePartCannotKeepFullCredit(int id)
    {
        BusinessQuestion question = CATALOG.Questions.Single(question => question.Id == id);
        BusinessAnswer answer = ReferenceAnswer(question);
        if (question.Kind is BusinessQuestionKind.Single or BusinessQuestionKind.Multiple)
            answer.SelectedIds[0] = question.Options.First(option => !question.CorrectIds.Contains(option.Id)).Id;
        else if (question.Kind == BusinessQuestionKind.Sequence)
            (answer.Order[0], answer.Order[1]) = (answer.Order[1], answer.Order[0]);
        else if (question.Kind == BusinessQuestionKind.Assignment)
            answer.Values[question.Fields[0].Id] = question.Options.First(option => option.Id != question.Fields[0].Expected).Id;
        else
            answer.Values[question.Fields[0].Id] = "falsche Eingabe";
        Assert.False(BusinessEvaluator.Evaluate(question, answer).Correct);
    }

    /// <summary> Erwartungswerte separat berechnet; sie werden nicht aus den JSON-Musterlösungen übernommen. </summary>
    [Theory]
    [InlineData(1, "1", "4")]
    [InlineData(9, "1", null)]
    [InlineData(22, "2", null)]
    [InlineData(23, "2", null)]
    [InlineData(24, "30.04.2026", null)]
    [InlineData(25, "15.03.2026", null)]
    [InlineData(30, "3", null)]
    [InlineData(33, "30", null)]
    [InlineData(34, "45", null)]
    [InlineData(35, "07:30", null)]
    [InlineData(36, "30", null)]
    [InlineData(37, "60", null)]
    [InlineData(38, "12:30", null)]
    [InlineData(39, "07:45", null)]
    [InlineData(40, "27", null)]
    [InlineData(41, "20", null)]
    [InlineData(42, "8", null)]
    [InlineData(47, "16", null)]
    [InlineData(49, "55", null)]
    [InlineData(50, "3", null)]
    [InlineData(51, "4", null)]
    [InlineData(58, "2", null)]
    [InlineData(69, "353,40", null)]
    [InlineData(70, "357,00", null)]
    [InlineData(73, "744,00", null)]
    [InlineData(78, "41,60", null)]
    [InlineData(82, "2070,00", null)]
    [InlineData(83, "3045,00", "283,19")]
    [InlineData(86, "25000", null)]
    [InlineData(92, "18000,00", null)]
    [InlineData(96, "50000", null)]
    [InlineData(121, "15000", null)]
    [InlineData(122, "1,25", null)]
    [InlineData(124, "12,00", null)]
    [InlineData(125, "8", null)]
    [InlineData(126, "120000", null)]
    [InlineData(127, "12,50", null)]
    [InlineData(128, "15,00", null)]
    [InlineData(129, "45,00", "14400,00")]
    [InlineData(130, "1100,00", null)]
    [InlineData(131, "999,60", null)]
    [InlineData(132, "1000,00", null)]
    [InlineData(133, "500", null)]
    [InlineData(134, "5600,00", "5000,00")]
    public void NumericAndDeadlineExamplesMatchIndependentlyCalculatedValues(int id, string first, string? second)
    {
        BusinessQuestion question = CATALOG.Questions.Single(question => question.Id == id);
        BusinessAnswer answer = new() { Values = new() { ["f1"] = first } };
        if (second != null) answer.Values["f2"] = second;
        Assert.True(BusinessEvaluator.Evaluate(question, answer).Correct);
    }

    [Theory]
    [InlineData(56, "a")]
    [InlineData(57, "a")]
    [InlineData(87, "a")]
    [InlineData(90, "a")]
    [InlineData(94, "a")]
    [InlineData(123, "a")]
    [InlineData(148, "a")]
    public void ImportantDistinctionsUseCurrentAndPreciseReferenceClaims(int id, string selected)
    {
        Assert.True(BusinessEvaluator.Evaluate(CATALOG.Questions.Single(question => question.Id == id), new() { SelectedIds = [selected] }).Correct);
    }

    [Fact]
    public void MultipleChoiceRequiresExactSetWithoutDuplicatesOrExtraSelections()
    {
        BusinessQuestion question = CATALOG.Questions.Single(question => question.Id == 3);
        Assert.True(BusinessEvaluator.Evaluate(question, new() { SelectedIds = ["c", "a"] }).Correct);
        foreach (string[] selections in new[] { new[] { "a" }, new[] { "a", "a" }, new[] { "a", "b" }, new[] { "a", "c", "d" }, new[] { "a", "unknown" } })
            Assert.Equal(0, BusinessEvaluator.Evaluate(question, new() { SelectedIds = selections.ToList() }).Earned);
    }

    [Fact]
    public void PartlyCorrectAssignmentsEarnOnlyCorrectFieldPointsAndMayReuseOptions()
    {
        BusinessQuestion question = CATALOG.Questions.Single(question => question.Id == 113);
        BusinessAnswer answer = new() { Values = new() { ["f1"] = "a", ["f2"] = "b", ["f3"] = "c", ["f4"] = "c" } };
        Assert.Equal(4, BusinessEvaluator.Evaluate(question, answer).Earned);
        answer.Values["f2"] = "a";
        BusinessEvaluation partial = BusinessEvaluator.Evaluate(question, answer);
        Assert.True(partial.Complete);
        Assert.False(partial.Correct);
        Assert.Equal(3, partial.Earned);
        answer.Values.Remove("f1");
        Assert.False(BusinessEvaluator.Evaluate(question, answer).Complete);
        Assert.Equal(2, BusinessEvaluator.Evaluate(question, answer).Earned);
    }

    [Theory]
    [InlineData("353.40", true)]
    [InlineData(" 353,40 ", true)]
    [InlineData("353,400", true)]
    [InlineData("353,404", false)]
    [InlineData("706,80", false)]
    [InlineData("353 EUR", false)]
    [InlineData("3.53,40", false)]
    [InlineData("3e2", false)]
    [InlineData("NaN", false)]
    [InlineData("", false)]
    public void NumbersAcceptBothDecimalSeparatorsButDoNotRoundWrongAnswersIntoCorrectOnes(string text, bool correct)
    {
        Assert.Equal(correct, BusinessEvaluator.Evaluate(CATALOG.Questions.Single(question => question.Id == 69), new() { Values = new() { ["f1"] = text } }).Correct);
    }

    [Theory]
    [InlineData("30.04.2026", true)]
    [InlineData("2026-04-30", true)]
    [InlineData("04/30/2026", false)]
    [InlineData("31.04.2026", false)]
    [InlineData("15.04.2026", false)]
    public void DatesAreUnambiguousAndRejectImpossibleDays(string text, bool correct)
    {
        Assert.Equal(correct, BusinessEvaluator.Evaluate(CATALOG.Questions.Single(question => question.Id == 24), new() { Values = new() { ["f1"] = text } }).Correct);
    }

    [Fact]
    public void SequenceCannotPassWithRepeatedMissingOrForeignSteps()
    {
        BusinessQuestion question = CATALOG.Questions.Single(question => question.Id == 110);
        Assert.True(BusinessEvaluator.Evaluate(question, new() { Order = ["a", "b", "c", "d"] }).Correct);
        foreach (List<string> order in new List<string>[] { ["a", "a", "c", "d"], ["a", "b", "c"], ["a", "b", "c", "unknown"], ["b", "a", "c", "d"] })
            Assert.Equal(0, BusinessEvaluator.Evaluate(question, new() { Order = order }).Earned);
    }

    [Fact]
    public void ExamsHaveThirtyUniqueQuestionsAllThemesAndEverySupportedFormat()
    {
        for (int iteration = 0; iteration < 100; iteration++)
        {
            BusinessRound round = BusinessRoundBuilder.Create(CATALOG, BusinessRoundKind.Exam);
            Assert.Equal(30, round.QuestionIds.Count);
            Assert.Equal(30, round.QuestionIds.Distinct().Count());
            BusinessQuestion[] selected = round.QuestionIds.Select(id => CATALOG.Questions.Single(question => question.Id == id)).ToArray();
            Assert.All(CATALOG.Topics, topic => Assert.True(selected.Count(question => question.TopicId == topic.Id) >= 2));
            Assert.Equal(7, selected.Select(question => question.Kind).Distinct().Count());
            Assert.NotNull(round.RunningSinceUtc);
        }
    }

    [Fact]
    public void TopicAndReviewRoundsUseOnlyRequestedPoolAndMixedPrioritizesReview()
    {
        BusinessRound topic = BusinessRoundBuilder.Create(CATALOG, BusinessRoundKind.Topic, "employment");
        Assert.Equal(25, topic.QuestionIds.Count);
        Assert.All(topic.QuestionIds, id => Assert.Equal("employment", CATALOG.Questions.Single(question => question.Id == id).TopicId));
        BusinessRound review = BusinessRoundBuilder.Create(CATALOG, BusinessRoundKind.Review, priorityIds: [2, 47, 83]);
        Assert.Equal(new[] { 2, 47, 83 }, review.QuestionIds.Order());
        BusinessRound mixed = BusinessRoundBuilder.Create(CATALOG, BusinessRoundKind.Mixed, priorityIds: [2, 47]);
        Assert.Equal(10, mixed.QuestionIds.Count);
        Assert.Contains(2, mixed.QuestionIds);
        Assert.Contains(47, mixed.QuestionIds);
        Assert.Throws<InvalidOperationException>(() => BusinessRoundBuilder.Create(CATALOG, BusinessRoundKind.Review));
        Assert.Throws<ArgumentException>(() => BusinessRoundBuilder.Create(CATALOG, BusinessRoundKind.Topic, "unknown"));
    }

    [Fact]
    public void OptionShuffleIsStableAcrossSerializationAndDoesNotMutateSource()
    {
        BusinessRound round = BusinessRoundBuilder.Create(CATALOG, BusinessRoundKind.Mixed);
        BusinessQuestion question = CATALOG.Questions[1];
        string[] order = BusinessRoundBuilder.Options(round, question).Select(option => option.Id).ToArray();
        BusinessRound restored = JsonSerializer.Deserialize<BusinessRound>(JsonSerializer.Serialize(round))!;
        Assert.Equal(order, BusinessRoundBuilder.Options(restored, question).Select(option => option.Id));
        Assert.Equal(new[] { "a", "b", "c", "d" }, question.Options.Select(option => option.Id));
        Assert.Equal(4, order.Distinct().Count());
    }

    [Fact]
    public void TimerUsesElapsedUtcTimeAndPauseRetainsOnlyRemainingSeconds()
    {
        DateTimeOffset start = new(2026, 11, 4, 10, 0, 0, TimeSpan.Zero);
        BusinessRound round = new() { Kind = BusinessRoundKind.Exam, RunningSinceUtc = start };
        Assert.Equal(3540, round.SecondsLeft(start.AddMinutes(1)));
        round.Pause(start.AddMinutes(15));
        Assert.Equal(2700, round.SecondsLeft(start.AddHours(3)));
        Assert.True(round.Paused);
        round.RunningSinceUtc = start.AddHours(3);
        Assert.Equal(0, round.SecondsLeft(start.AddHours(4)));
    }

    [Fact]
    public void CatalogRejectsDuplicateIdsBadReferencesAndMalformedContracts()
    {
        Assert.Throws<InvalidDataException>(() => BusinessCatalog.Validate([CATALOG.Questions[0], CATALOG.Questions[0]], CATALOG.Topics, CATALOG.Sources));
        Assert.Throws<InvalidDataException>(() => BusinessCatalog.Validate(CATALOG.Questions, CATALOG.Topics, []));
        BusinessQuestion invalid = new() { Id = 1, TopicId = "education", Title = "Test", Task = "Test", Scenario = "Test", Explanation = "Test", MemoryAid = "Test", SourceIds = ["bbig20"], Kind = BusinessQuestionKind.Number, Fields = [new() { Id = "f", Label = "Ergebnis", Expected = "NaN", Explanation = "Test" }] };
        Assert.Throws<InvalidDataException>(() => BusinessCatalog.Validate([invalid], CATALOG.Topics, CATALOG.Sources));
    }

    /// <summary> Prüft die Bewertungstechnik gegen den veröffentlichten Vertrag; fachliche Zahlen sind oben unabhängig belegt. </summary>
    internal static BusinessAnswer ReferenceAnswer(BusinessQuestion question) => new()
    {
        SelectedIds = question.CorrectIds.ToList(),
        Values = question.Fields.ToDictionary(field => field.Id, field => field.Expected),
        Order = question.CorrectOrder.ToList()
    };
}
