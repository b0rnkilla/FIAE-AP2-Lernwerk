using Microsoft.AspNetCore.WebUtilities;

namespace FiaeTrainer.Web.Learning;

/// <summary> Merkt den Lernbereich innerhalb der Browsersitzung auch auf Übersicht und Lernplan. </summary>
public sealed class LearningNavigation
{
    /// <summary> Wählt die kontextbezogenen Pseudocode-Einträge; ohne Auswahl bleibt SQL der Einstieg. </summary>
    public bool Algorithms => Module == LearningModule.Algorithms;
    /// <summary> Der zuletzt besuchte fachliche Bereich bleibt auf allgemeinen Seiten erhalten. </summary>
    public LearningModule Module { get; private set; }
    /// <summary> Letzte SQL-Aufgabenseite dieser Browsersitzung; ein direkter Einstieg verwendet die Ermittlungsakte. </summary>
    public string SqlTaskPath { get; private set; } = "/sql";
    /// <summary> Erhält das lokale Rücksprungziel auch beim Neuladen des Spickzettels. </summary>
    public string SqlReferenceHref => "/nachschlagen?zurueck=" + Uri.EscapeDataString(SqlTaskPath);

    /// <summary> Nur SQL-Aufgabenseiten sind Rücksprungziele; externe und sonstige Adressen werden nicht übernommen. </summary>
    public string SqlReturnPath(string? requested) => requested is not null && IsSqlTaskPath(requested) ? requested : SqlTaskPath;

    /// <summary> Erlaubt ausschließlich die zwei SQL-Einstiege und deren positive numerische Aufgabenpfade. </summary>
    private static bool IsSqlTaskPath(string? path)
    {
        if (path is "/sql" or "/sql-werkstatt") return true;
        string[] parts = path?.Split('/') ?? [];
        return parts.Length == 3 && parts[0] == "" && parts[1] is "sql" or "sql-werkstatt"
            && int.TryParse(parts[2], System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out int id) && id > 0;
    }

    /// <summary> Nur fachliche Zielseiten ändern den Kontext; allgemeine Seiten behalten die bisherige Wahl. </summary>
    public void Visit(string relativePath)
    {
        string path = relativePath.Split('?', '#')[0].Trim('/');
        string sqlPath = "/" + path.ToLowerInvariant();
        if (IsSqlTaskPath(sqlPath)) SqlTaskPath = sqlPath;
        int queryStart = relativePath.IndexOf('?');
        if (path.Equals("nachschlagen", StringComparison.OrdinalIgnoreCase) && queryStart >= 0)
        {
            var query = QueryHelpers.ParseQuery(relativePath[queryStart..].Split('#')[0]);
            if (query.TryGetValue("zurueck", out var requested))
            {
                string target = requested.ToString();
                if (IsSqlTaskPath(target)) SqlTaskPath = target;
            }
        }
        if (path.Equals("algorithmen", StringComparison.OrdinalIgnoreCase) || path.StartsWith("algorithmen/", StringComparison.OrdinalIgnoreCase))
            Module = LearningModule.Algorithms;
        else if (path.Equals("wiso", StringComparison.OrdinalIgnoreCase) || path.StartsWith("wiso/", StringComparison.OrdinalIgnoreCase))
            Module = LearningModule.BusinessStudies;
        else if (path.Equals("sql", StringComparison.OrdinalIgnoreCase) || path.StartsWith("sql/", StringComparison.OrdinalIgnoreCase) || path.Equals("sql-werkstatt", StringComparison.OrdinalIgnoreCase) || path.StartsWith("sql-werkstatt/", StringComparison.OrdinalIgnoreCase) || path.Equals("nachschlagen", StringComparison.OrdinalIgnoreCase))
            Module = LearningModule.Sql;
    }
}
