namespace FiaeTrainer.Core.QualityAssurance.Content;

/// <summary> Eindeutig typisierter Testparameter einschließlich des zugelassenen Versuchsdatenbereichs. </summary>
public sealed class QualityInput
{
    /// <summary> Fachkennung. </summary>
    public string Key { get; set; } = "";
    /// <summary> Sichtbarer Parametername. </summary>
    public string Label { get; set; } = "";
    /// <summary> Number für ganze Zahlen oder Select für angebotene Zustände/Wahrheitswerte. </summary>
    public string Kind { get; set; } = "Number";
    /// <summary> Untere einschließlich erlaubte Testdatengrenze, auch für fachlich ungültige Fälle. </summary>
    public int Minimum { get; set; } = -100;
    /// <summary> Obere einschließlich erlaubte Testdatengrenze. </summary>
    public int Maximum { get; set; } = 100000;
    /// <summary> Zulässige diskrete Eingaben. </summary>
    public Dictionary<string, string> Options { get; set; } = [];
}
