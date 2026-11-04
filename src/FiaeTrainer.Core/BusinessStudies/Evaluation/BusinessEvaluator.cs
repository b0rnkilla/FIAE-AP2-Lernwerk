using System.Globalization;
using FiaeTrainer.Core.BusinessStudies.Content;

namespace FiaeTrainer.Core.BusinessStudies.Evaluation;

/// <summary> Prüft Auswahlmengen, Reihenfolgen und strukturierte Ergebnisse deterministisch. </summary>
public static class BusinessEvaluator
{
    /// <summary> Leere oder syntaktisch ungültige Teile zählen null Punkte; fremde Optionskennungen werden abgewiesen. </summary>
    public static BusinessEvaluation Evaluate(BusinessQuestion question, BusinessAnswer answer)
    {
        List<BusinessCheck> checks = [];
        bool complete;
        if (question.Kind is BusinessQuestionKind.Single or BusinessQuestionKind.Multiple)
        {
            complete = answer.SelectedIds.Count == question.CorrectIds.Length && answer.SelectedIds.Distinct().Count() == answer.SelectedIds.Count && answer.SelectedIds.All(id => question.Options.Any(option => option.Id == id));
            bool correct = complete && answer.SelectedIds.ToHashSet().SetEquals(question.CorrectIds);
            string Display(IEnumerable<string> ids) => string.Join(" · ", ids.Select(id => question.Options.FirstOrDefault(option => option.Id == id)?.Text ?? "Ungültige Auswahl"));
            string reasons = string.Join(" ", question.Options.Where(option => answer.SelectedIds.Contains(option.Id) && !question.CorrectIds.Contains(option.Id)).Select(option => option.Explanation));
            checks.Add(new("Auswahl", Display(answer.SelectedIds), Display(question.CorrectIds), correct, reasons));
        }
        else if (question.Kind == BusinessQuestionKind.Sequence)
        {
            complete = answer.Order.Count == question.Options.Length && answer.Order.Distinct().Count() == answer.Order.Count && answer.Order.All(id => question.Options.Any(option => option.Id == id));
            string Display(IEnumerable<string> ids) => string.Join(" → ", ids.Select(id => question.Options.FirstOrDefault(option => option.Id == id)?.Text ?? "Ungültiger Schritt"));
            checks.Add(new("Reihenfolge", Display(answer.Order), Display(question.CorrectOrder), complete && answer.Order.SequenceEqual(question.CorrectOrder), question.Explanation));
        }
        else
        {
            complete = true;
            foreach (BusinessField field in question.Fields)
            {
                string actual = answer.Values.GetValueOrDefault(field.Id, "").Trim();
                bool valid;
                bool correct;
                string expected = field.Expected;
                switch (question.Kind)
                {
                    case BusinessQuestionKind.Number:
                        valid = TryNumber(actual, out decimal number);
                        decimal target = decimal.Parse(field.Expected, CultureInfo.InvariantCulture);
                        correct = valid && number == target;
                        expected = target.ToString("F" + field.Decimals, CultureInfo.GetCultureInfo("de-DE"));
                        break;
                    case BusinessQuestionKind.Date:
                        valid = DateOnly.TryParseExact(actual, ["dd.MM.yyyy", "yyyy-MM-dd"], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date);
                        correct = valid && date == DateOnly.ParseExact(field.Expected, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        expected = DateOnly.ParseExact(field.Expected, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd.MM.yyyy");
                        break;
                    case BusinessQuestionKind.Time:
                        valid = TimeOnly.TryParseExact(actual, ["HH:mm", "H:mm"], CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly time);
                        correct = valid && time == TimeOnly.ParseExact(field.Expected, "HH:mm", CultureInfo.InvariantCulture);
                        break;
                    default:
                        valid = question.Options.Any(option => option.Id == actual);
                        correct = valid && actual == field.Expected;
                        expected = question.Options.Single(option => option.Id == field.Expected).Text;
                        actual = question.Options.FirstOrDefault(option => option.Id == actual)?.Text ?? "";
                        break;
                }
                complete &= valid;
                checks.Add(new(field.Label, actual.Length == 0 ? "Keine Antwort" : actual, expected + (field.Unit.Length > 0 ? " " + field.Unit : ""), correct, field.Explanation));
            }
        }
        return new(checks.Count(check => check.Correct), question.Maximum, complete, checks);
    }

    /// <summary> Akzeptiert Dezimalkomma oder Dezimalpunkt, aber keine mehrdeutigen Tausendertrennzeichen, Exponenten oder Währungstexte. </summary>
    public static bool TryNumber(string text, out decimal number) => decimal.TryParse(text.Trim().Replace(',', '.'), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out number);
}
