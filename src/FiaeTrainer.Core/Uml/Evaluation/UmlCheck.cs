namespace FiaeTrainer.Core.Uml.Evaluation;

/// <summary> Nachvollziehbare fachliche Teilprüfung. </summary>
public sealed class UmlCheck
{
    /// <summary> Name des Kriteriums. </summary>
    public string Name { get; set; } = "";
    /// <summary> Intern erfüllter Anteil; im Lernen keine Punkteanzeige. </summary>
    public int Earned { get; set; }
    /// <summary> Höchstwert innerhalb der Prüfung. </summary>
    public int Maximum { get; set; }
    /// <summary> Deutscher Hinweis auf die verbleibende Anforderung. </summary>
    public string Detail { get; set; } = "";
}
