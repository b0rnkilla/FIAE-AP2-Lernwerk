namespace FiaeTrainer.Web.Project;

/// <summary> Gemeinsame öffentliche Projektangaben für Übersicht, Urheberseite und Fußzeile. </summary>
public static class ProjectInformation
{
    /// <summary> Vom Projektinhaber gewünschte Namensnennung. </summary>
    public const string AUTHOR_NAME = "Christopher Weismantel";
    /// <summary> Öffentlicher GitHub-Name des Projektinhabers. </summary>
    public const string AUTHOR_HANDLE = "b0rnkilla";
    /// <summary> Offizielles Repository für Projektinformationen und einen freiwilligen GitHub-Stern. </summary>
    public const string REPOSITORY_URL = "https://github.com/b0rnkilla/FIAE-AP2-Lernwerk";
    /// <summary> Bestehende Meldungen zuerst ansehen und bei Bedarf eine neue Meldung auf GitHub anlegen. </summary>
    public const string ISSUES_URL = REPOSITORY_URL + "/issues";
    /// <summary> Öffentliche Windows-Pakete; verweist nicht auf Quellcodearchive. </summary>
    public const string RELEASES_URL = REPOSITORY_URL + "/releases";
    /// <summary> Vollständige private Nutzungsbedingungen des Projekts. </summary>
    public const string LICENSE_URL = REPOSITORY_URL + "/blob/master/LICENSE";
    /// <summary> Vom Projektinhaber bereitgestellter öffentlicher Link für freiwillige Unterstützung. </summary>
    public const string PAYPAL_URL = "https://www.paypal.com/paypalme/b0rnkillafpv";
    /// <summary> Verhindert eine leere oder nur scheinbar funktionierende Unterstützungsaktion. </summary>
    public static bool HasPayPal => !string.IsNullOrWhiteSpace(PAYPAL_URL);
}
