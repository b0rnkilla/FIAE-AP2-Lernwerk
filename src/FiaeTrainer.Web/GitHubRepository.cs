using System.Text.RegularExpressions;

namespace FiaeTrainer.Web;

/// <summary> Begrenzt die Updatequelle auf ein öffentliches Repository bei github.com. </summary>
public sealed record GitHubRepository(string Owner, string Name)
{
    /// <summary> Öffentliche Releases ohne Zugangsschlüssel; höchstens 100 Einträge. </summary>
    public Uri ApiAddress => new($"https://api.github.com/repos/{Owner}/{Name}/releases?per_page=100");
    /// <summary> Öffentliche Repositoryadresse für die Oberfläche. </summary>
    public string WebAddress => $"https://github.com/{Owner}/{Name}";
    /// <summary> Erzeugt ausschließlich einen GitHub-Link; fremde Links aus API-Antworten werden nicht übernommen. </summary>
    public string ReleaseAddress(string tag) => $"{WebAddress}/releases/tag/{Uri.EscapeDataString(tag)}";

    /// <summary> Akzeptiert owner/repo oder eine normale HTTPS-Repositoryadresse, keine Tokens oder beliebigen Server. </summary>
    public static GitHubRepository? Parse(string? input)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Length > 300)
            return null;
        string path = input.Trim().TrimEnd('/');
        if (path.Contains("://", StringComparison.Ordinal))
        {
            if (!Uri.TryCreate(path, UriKind.Absolute, out Uri? uri) || uri.Scheme != "https" || uri.Host != "github.com" || !uri.IsDefaultPort || uri.UserInfo.Length > 0 || uri.Query.Length > 0 || uri.Fragment.Length > 0)
                return null;
            path = uri.AbsolutePath.Trim('/');
        }
        string[] parts = path.Split('/');
        if (parts.Length != 2)
            return null;
        if (parts[1].EndsWith(".git", StringComparison.OrdinalIgnoreCase))
            parts[1] = parts[1][..^4];
        return Regex.IsMatch(parts[0], @"^[A-Za-z0-9][A-Za-z0-9-]{0,38}$") && Regex.IsMatch(parts[1], @"^[A-Za-z0-9_.-]{1,100}$") && parts[1] is not "." and not ".." ? new(parts[0], parts[1]) : null;
    }
}
