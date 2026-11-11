using System.Text.Json;

namespace FiaeTrainer.Core.QualityAssurance.Models;

/// <summary> Eigener Testsatz und strukturierte Prüfunterlagen, getrennt von den bisherigen Modulen. </summary>
public sealed class QualityAnswer
{
    /// <summary> Fachliche Antworten auf benannte Felder und Matrixzellen. </summary>
    public Dictionary<string, string> Values { get; set; } = [];
    /// <summary> Stabile Kennungen gewählter Aussagen oder Reviewbefunde. </summary>
    public List<string> SelectedIds { get; set; } = [];
    /// <summary> Vom Lernenden angelegte Testfälle mit eigenen Eingaben und Sollwerten. </summary>
    public List<QualityTestCase> TestCases { get; set; } = [];
    /// <summary> Selbst angelegte Reihenfolge angebotener Arbeitsschritte. </summary>
    public List<string> Order { get; set; } = [];
    /// <summary> Altes Profilfeld zur verlustfreien Übernahme vorhandener Dateien; keine Eingabe oder Anzeige mehr. </summary>
    public string Reasoning { get; set; } = "";
    /// <summary> Verhindert gemeinsame Referenzen zwischen Vorlage, Lösung und eigenem Versuch. </summary>
    public QualityAnswer Copy() => JsonSerializer.Deserialize<QualityAnswer>(JsonSerializer.Serialize(this))!;
}
