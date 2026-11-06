namespace FiaeTrainer.Web.Networking;

/// <summary> Fordert einen kontrollierten Neustart des Webhosts innerhalb derselben EXE an. </summary>
public sealed class NetworkRestart
{
    /// <summary> Eine Aktion darf erst nach dem vollständigen Hostwechsel erneut beginnen. </summary>
    public SemaphoreSlim Gate { get; } = new(1, 1);
    /// <summary> Nur ein ausdrücklich angeforderter Wechsel setzt den Programmablauf fort. </summary>
    public bool Requested { get; private set; }
    /// <summary> Überschreibt beim internen Neustart ältere Startparameter. </summary>
    public bool? NextMode { get; private set; }
    /// <summary> Wird vor Aufbau des neuen Hosts zurückgesetzt. </summary>
    public void BeginHost()
    {
        if (Requested) Gate.Release();
        Requested = false;
    }
    /// <summary> Die HTTP-Antwort darf vor dem Herunterfahren vollständig übertragen werden. </summary>
    public void Request(bool enabled, IHostApplicationLifetime lifetime)
    {
        NextMode = enabled;
        Requested = true;
        _ = StopAsync(lifetime);
    }
    /// <summary> Trennt die Abgabe der Umschaltantwort vom Ende der bestehenden Verbindungen. </summary>
    private static async Task StopAsync(IHostApplicationLifetime lifetime)
    {
        await Task.Delay(1200);
        lifetime.StopApplication();
    }
}
