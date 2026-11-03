namespace FiaeTrainer.Web.Updates;

/// <summary> Prüft im Hintergrund einmal pro Programmstart, ohne den Webstart auf das Internet warten zu lassen. </summary>
public sealed class UpdateCheckWorker(UpdateChecker checker) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => checker.CheckAsync(stoppingToken);
}
