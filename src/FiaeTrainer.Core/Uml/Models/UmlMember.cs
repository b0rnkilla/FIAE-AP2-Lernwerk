namespace FiaeTrainer.Core.Uml.Models;

/// <summary> Fachlich zugeordnetes Attribut, Methode oder Zustandsaktion. </summary>
public sealed class UmlMember
{
    /// <summary> Lokale Kennung für Bearbeitung und Darstellung. </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    /// <summary> Fachliche Bedeutung aus dem ausdrücklich bereitgestellten Vokabular. </summary>
    public string Key { get; set; } = "";
    /// <summary> Eigene sichtbare Beschriftung. </summary>
    public string Name { get; set; } = "";
    /// <summary> Attribute, Method, Entry, Do oder Exit. </summary>
    public string Kind { get; set; } = "Attribute";
    /// <summary> Datentyp oder Rückgabetyp, bei Zustandsaktionen leer. </summary>
    public string Type { get; set; } = "";
    /// <summary> Sichtbarkeit +, -, # oder ~. </summary>
    public string Visibility { get; set; } = "-";
    /// <summary> Methodenparameter in der Form name: Typ, getrennt durch Kommas. </summary>
    public string Parameters { get; set; } = "";
    /// <summary> Klassenbezogenes Merkmal. </summary>
    public bool Static { get; set; }
    /// <summary> Abstrakte Methode ohne eigene Umsetzung. </summary>
    public bool Abstract { get; set; }
}
