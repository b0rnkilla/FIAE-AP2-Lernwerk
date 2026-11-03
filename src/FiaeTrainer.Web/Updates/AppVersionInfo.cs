using System.Reflection;

namespace FiaeTrainer.Web.Updates;

/// <summary> Zeigt dieselbe beim Build festgelegte Version in Oberfläche und Updateprüfung. </summary>
public sealed class AppVersionInfo
{
    /// <summary> Auswertbare aktuelle Version. </summary>
    public SemanticVersion? Parsed { get; }
    /// <summary> Lesbare Version ohne angehängte Commitmetadaten. </summary>
    public string Display => Parsed?.Display ?? "unbekannt";
    /// <summary> Kennzeichnet jede SemVer-Vorabversion für die Auswahl des Updatekanals. </summary>
    public bool IsPrerelease => Parsed?.IsPrerelease == true;

    /// <summary> Liest die Version der tatsächlich gestarteten Webassembly. </summary>
    public AppVersionInfo() : this(typeof(AppVersionInfo).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "") { }

    /// <summary> Erlaubt gezielte Vergleiche anderer Versionsstände in Tests. </summary>
    public AppVersionInfo(string version) => Parsed = SemanticVersion.Parse(version);
}
