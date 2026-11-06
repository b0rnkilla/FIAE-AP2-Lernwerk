namespace FiaeTrainer.Core.DatabaseDesign.Models;

/// <summary> Eine frei benennbare Relation; alle PK-Markierungen bilden gemeinsam ihren Primärschlüssel. </summary>
public sealed class DesignTable
{
    /// <summary> Technische Kennung, unabhängig vom bearbeitbaren Namen. </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    /// <summary> Frei gewählter Tabellenname. </summary>
    public string Name { get; set; } = "NeueTabelle";
    /// <summary> Zugeordnete Attribute einschließlich Schlüsselrollen. </summary>
    public List<DesignColumn> Columns { get; set; } = [];
}
