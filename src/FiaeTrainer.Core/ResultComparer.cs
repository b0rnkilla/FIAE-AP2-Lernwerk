using System.Globalization;
using System.Text.Json;

namespace FiaeTrainer.Core;

/// <summary> Vergleicht Werte und Duplikate; Aliase werden nicht als Lösungskriterium verwendet. </summary>
public static class ResultComparer
{
    /// <summary> Vergleicht Spaltenzahl und Zeilen; nur Sortieraufgaben verlangen eine feste Reihenfolge. </summary>
    public static bool AreEqual(QueryResult expected, QueryResult actual, bool ordered)
    {
        if (expected.Error != null || actual.Error != null || expected.Truncated || actual.Truncated || expected.Columns.Count != actual.Columns.Count || expected.Rows.Count != actual.Rows.Count)
            return false;

        IEnumerable<string> expectedRows = expected.Rows.Select(RowKey);
        IEnumerable<string> actualRows = actual.Rows.Select(RowKey);
        if (!ordered)
        {
            expectedRows = expectedRows.Order(StringComparer.Ordinal);
            actualRows = actualRows.Order(StringComparer.Ordinal);
        }
        return expectedRows.SequenceEqual(actualRows, StringComparer.Ordinal);
    }

    private static string RowKey(object?[] row) => JsonSerializer.Serialize(row.Select(Normalize));

    private static string? Normalize(object? value)
    {
        if (value is null or DBNull)
            return null;
        if (value is byte or short or int or long or decimal or double or float)
        {
            string numberText = Convert.ToString(value, CultureInfo.InvariantCulture) ?? "";
            if (!decimal.TryParse(numberText, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal number) || value is double doubleValue && (double)number != doubleValue || value is float singleValue && (float)number != singleValue)
                return "Gleitkomma:" + numberText;
            return "Zahl:" + number.ToString("G29", CultureInfo.InvariantCulture);
        }
        return value switch
        {
            DateTime date => "Datum:" + date.ToString("O", CultureInfo.InvariantCulture),
            byte[] bytes => "Binaer:" + Convert.ToBase64String(bytes),
            _ => "Text:" + Convert.ToString(value, CultureInfo.InvariantCulture)
        };
    }
}
