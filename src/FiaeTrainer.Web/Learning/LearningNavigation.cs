namespace FiaeTrainer.Web.Learning;

/// <summary> Merkt den Lernbereich innerhalb der Browsersitzung auch auf Übersicht und Lernplan. </summary>
public sealed class LearningNavigation
{
    /// <summary> Wählt die kontextbezogenen Pseudocode-Einträge; ohne Auswahl bleibt SQL der Einstieg. </summary>
    public bool Algorithms { get; private set; }

    /// <summary> Nur fachliche Zielseiten ändern den Kontext; allgemeine Seiten behalten die bisherige Wahl. </summary>
    public void Visit(string relativePath)
    {
        string path = relativePath.Split('?', '#')[0].Trim('/');
        if (path.Equals("algorithmen", StringComparison.OrdinalIgnoreCase) || path.StartsWith("algorithmen/", StringComparison.OrdinalIgnoreCase))
            Algorithms = true;
        else if (path.Equals("sql", StringComparison.OrdinalIgnoreCase) || path.StartsWith("sql/", StringComparison.OrdinalIgnoreCase) || path.Equals("sql-werkstatt", StringComparison.OrdinalIgnoreCase) || path.StartsWith("sql-werkstatt/", StringComparison.OrdinalIgnoreCase) || path.Equals("nachschlagen", StringComparison.OrdinalIgnoreCase))
            Algorithms = false;
    }
}
