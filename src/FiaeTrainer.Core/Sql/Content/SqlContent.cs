namespace FiaeTrainer.Core.Sql.Content;

/// <summary> Öffnet ausschließlich in die Core-Bibliothek eingebettete SQL-Lerninhalte. </summary>
internal static class SqlContent
{
    /// <summary> Liefert eine Ressource des SQL-Moduls oder meldet eine fehlende Inhaltsdatei. </summary>
    public static Stream Open(string fileName) => typeof(SqlContent).Assembly.GetManifestResourceStream($"FiaeTrainer.Core.Sql.Content.{fileName}") ?? throw new FileNotFoundException("Die SQL-Inhaltsdatei fehlt.", fileName);
}
