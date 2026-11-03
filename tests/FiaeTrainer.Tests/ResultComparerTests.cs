using FiaeTrainer.Core.Sql.Results;
using Xunit;

namespace FiaeTrainer.Tests;

/// <summary> Prüft fachliche Vergleichsfälle, bei denen reine Mengenvergleiche falsche Ergebnisse liefern würden. </summary>
public sealed class ResultComparerTests
{
    [Fact]
    public void DuplicatesAreNotDiscarded()
    {
        Assert.False(ResultComparer.AreEqual(Table([1], [1], [2]), Table([1], [2], [2]), false));
    }

    [Fact]
    public void OrderIsRequiredOnlyWhenTaskRequestsIt()
    {
        Assert.True(ResultComparer.AreEqual(Table([1], [2]), Table([2], [1]), false));
        Assert.False(ResultComparer.AreEqual(Table([1], [2]), Table([2], [1]), true));
    }

    [Fact]
    public void NullAndLiteralNullRemainDifferent()
    {
        Assert.False(ResultComparer.AreEqual(Table(new object?[] { null }), Table(["NULL"]), false));
    }

    [Fact]
    public void CompatibleNumericTypesAndAliasesAreAccepted()
    {
        QueryResult expected = Table([2]);
        QueryResult actual = new() { Columns = ["AndereBezeichnung"], Rows = [[2.00m]] };
        Assert.True(ResultComparer.AreEqual(expected, actual, false));
    }

    [Fact]
    public void TruncatedResultsCannotPass()
    {
        Assert.False(ResultComparer.AreEqual(Table([1]), new QueryResult { Columns = ["Wert"], Rows = [[1]], Truncated = true }, false));
    }

    [Fact]
    public void TextIsNotSilentlyAcceptedAsNumericResult()
    {
        Assert.False(ResultComparer.AreEqual(Table([1]), Table(["1"]), false));
        Assert.True(ResultComparer.AreEqual(Table([1e100]), Table([1e100]), false));
        Assert.False(ResultComparer.AreEqual(Table([0m]), Table([1e-100]), false));
    }

    private static QueryResult Table(params object?[][] rows) => new() { Columns = ["Wert"], Rows = rows.ToList() };
}
