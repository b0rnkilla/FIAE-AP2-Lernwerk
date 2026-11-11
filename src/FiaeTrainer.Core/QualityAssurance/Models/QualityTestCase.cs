namespace FiaeTrainer.Core.QualityAssurance.Models;

/// <summary> Ein unabhängiger Testfall; Vorzustände sind ausdrücklich benannte Eingaben. </summary>
public sealed class QualityTestCase
{
    /// <summary> Bleibt beim Eingeben, Umordnen und Speichern stabil. </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    /// <summary> Eigene Werte für die im Auftrag beschriebenen Eingabeparameter. </summary>
    public Dictionary<string, string> Inputs { get; set; } = [];
    /// <summary> Selbst aus dem Fachvertrag abgeleitetes erwartetes Ergebnis. </summary>
    public string Expected { get; set; } = "";
}
