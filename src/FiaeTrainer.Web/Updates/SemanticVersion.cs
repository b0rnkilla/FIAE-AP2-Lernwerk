using System.Text.RegularExpressions;

namespace FiaeTrainer.Web.Updates;

/// <summary> Vergleicht Release- und Vorabversionen nach SemVer; Buildmetadaten ändern die Reihenfolge nicht. </summary>
public sealed class SemanticVersion : IComparable<SemanticVersion>
{
    /// <summary> Numerische Hauptbestandteile. </summary>
    private readonly int[] _numbers;
    /// <summary> Vorabkennzeichnung mit numerischen oder alphabetischen Teilstücken. </summary>
    private readonly string[] _prerelease;
    /// <summary> Version ohne optionales v-Präfix und ohne Buildmetadaten. </summary>
    public string Display { get; }
    /// <summary> Kennzeichnet eine Vorabversion. </summary>
    public bool IsPrerelease => _prerelease.Length > 0;

    private SemanticVersion(int[] numbers, string[] prerelease, string display)
    {
        _numbers = numbers;
        _prerelease = prerelease;
        Display = display;
    }

    /// <summary> Liest drei Versionszahlen; GitHub-Tags dürfen mit v beginnen. Ungültige Werte werden nicht geraten. </summary>
    public static SemanticVersion? Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 200)
            return null;
        string text = value.Trim();
        if (text.StartsWith('v') || text.StartsWith('V'))
            text = text[1..];
        Match match = Regex.Match(text, @"^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+[0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*)?$", RegexOptions.CultureInvariant);
        if (!match.Success)
            return null;
        int[] numbers = new int[3];
        for (int index = 0; index < 3; index++)
            if (!int.TryParse(match.Groups[index + 1].Value, out numbers[index]))
                return null;
        string[] prerelease = match.Groups[4].Success ? match.Groups[4].Value.Split('.') : [];
        if (prerelease.Any(x => x.All(char.IsAsciiDigit) && x.Length > 1 && x[0] == '0'))
            return null;
        return new SemanticVersion(numbers, prerelease, text.Split('+')[0]);
    }

    /// <summary> Vergleicht Zahlen und Vorabkennzeichnungen nach SemVer; eine stabile Version folgt auf ihre Vorabversionen. </summary>
    public int CompareTo(SemanticVersion? other)
    {
        if (other == null)
            return 1;
        for (int index = 0; index < 3; index++)
            if (_numbers[index] != other._numbers[index])
                return _numbers[index].CompareTo(other._numbers[index]);
        if (!IsPrerelease || !other.IsPrerelease)
            return IsPrerelease == other.IsPrerelease ? 0 : IsPrerelease ? -1 : 1;
        for (int index = 0; index < Math.Min(_prerelease.Length, other._prerelease.Length); index++)
        {
            string left = _prerelease[index], right = other._prerelease[index];
            bool leftNumeric = left.All(char.IsAsciiDigit), rightNumeric = right.All(char.IsAsciiDigit);
            int comparison = leftNumeric && rightNumeric ? left.Length.CompareTo(right.Length) : leftNumeric != rightNumeric ? (leftNumeric ? -1 : 1) : 0;
            if (comparison == 0)
                comparison = string.CompareOrdinal(left, right);
            if (comparison != 0)
                return comparison;
        }
        return _prerelease.Length.CompareTo(other._prerelease.Length);
    }
}
