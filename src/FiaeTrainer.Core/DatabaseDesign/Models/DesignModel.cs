using System.Text.Json;

namespace FiaeTrainer.Core.DatabaseDesign.Models;

/// <summary> Relationaler Entwurf mit stabilen Kennungen; Darstellung und Benennung beeinflussen die Fachbedeutung nicht. </summary>
public sealed class DesignModel
{
    /// <summary> Selbst angelegte Tabellenkarten. </summary>
    public List<DesignTable> Tables { get; set; } = [];
    /// <summary> Fremdschlüssel mit vollständigen Spaltenpaaren und fachlichen Kardinalitäten. </summary>
    public List<DesignRelation> Relations { get; set; } = [];
    /// <summary> Erzeugt eine unabhängige Kopie ohne gemeinsam veränderbare Listen. </summary>
    public DesignModel Copy() => JsonSerializer.Deserialize<DesignModel>(JsonSerializer.Serialize(this))!;
}
