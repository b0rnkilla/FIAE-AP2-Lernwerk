using Microsoft.AspNetCore.Antiforgery;

namespace FiaeTrainer.Web.Networking;

/// <summary> Verwaltung erfolgt über echte HTTP-Anfragen; ein Blazor-Circuit oder gefälschter Header berechtigt nicht zum Umschalten. </summary>
public static class HomeNetworkEndpoints
{
    /// <summary> Schützt jede Änderung vor entfernten Besuchern und fremden Formularen. </summary>
    public static void MapHomeNetwork(this WebApplication app)
    {
        app.MapGet("/heimnetz/bereit", (HttpContext context, NetworkAccessPolicy access, NetworkRestart restart) =>
        {
            context.Response.Headers.CacheControl = "no-store";
            if (!HomeNetworkBinding.CanManage(context)) return Results.StatusCode(403);
            return restart.Requested ? Results.StatusCode(503) : Results.Json(new { enabled = access.AllowLan });
        });
        app.MapPost("/heimnetz/umschalten", SwitchAsync);
    }

    /// <summary> Änderungen sind seriell, tokengeprüft und werden erst nach erfolgreicher Einrichtung angewendet. </summary>
    private static async Task<IResult> SwitchAsync(HttpContext context, IAntiforgery antiforgery, HomeNetworkService network, NetworkRestart restart, IHostApplicationLifetime lifetime, ILoggerFactory logs)
    {
        context.Response.Headers.CacheControl = "no-store";
        if (!HomeNetworkBinding.CanManage(context)) return Results.StatusCode(403);
        try { await antiforgery.ValidateRequestAsync(context); }
        catch (AntiforgeryValidationException) { return Results.BadRequest("Die Bestätigung ist abgelaufen. Öffne die Heimnetz-Einstellung erneut."); }
        if (!await restart.Gate.WaitAsync(0)) return Results.Conflict("Die Betriebsart wird bereits gewechselt. Warte einen Moment und lade die Seite neu.");
        try
        {
            IFormCollection form = await context.Request.ReadFormAsync();
            string mode = form["mode"].ToString();
            if (form["mode"].Count != 1 || mode is not ("enable" or "disable")) return Results.BadRequest();
            bool enabled = form["mode"] == "enable";
            string? error = await network.PrepareAsync(enabled);
            if (error != null) return ErrorPage(error);
            restart.Request(enabled, lifetime);
            return Results.Content($"""
                <!doctype html><html lang="de"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Heimnetz wird umgestellt · AP2 Lernwerk</title><script src="/theme.js"></script><link rel="stylesheet" href="/app.css"></head>
                <body><main class="page reading-page"><h1>Einen Moment bitte.</h1><p>Die Betriebsart wird gewechselt. Deine gespeicherten Lernstände bleiben erhalten.</p><p role="status" id="network-wait" data-network-wait="{enabled.ToString().ToLowerInvariant()}">Das Lernwerk startet neu und öffnet diese Seite anschließend wieder.</p><a class="button secondary" href="/heimnetz" data-enhance-nav="false">Heimnetz-Einstellung öffnen</a></main><script src="/home-network.js"></script></body></html>
                """, "text/html; charset=utf-8");
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or System.Text.Json.JsonException or InvalidOperationException)
        {
            logs.CreateLogger("Heimnetz").LogWarning(exception, "Die Betriebsart konnte nicht gespeichert werden.");
            return ErrorPage("Die Betriebsart konnte nicht gespeichert werden. Die bisherige Einstellung bleibt erhalten.");
        }
        finally { if (!restart.Requested) restart.Gate.Release(); }
    }

    /// <summary> Ein abgebrochener Windows-Dialog erhält eine verständliche Rückkehr statt einer leeren Fehlerantwort. </summary>
    private static IResult ErrorPage(string message) => Results.Content($"""
        <!doctype html><html lang="de"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Heimnetz-Einstellung</title><script src="/theme.js"></script><link rel="stylesheet" href="/app.css"></head><body><main class="page reading-page"><h1>Die Betriebsart bleibt erhalten.</h1><p class="notice warning" role="alert">{System.Net.WebUtility.HtmlEncode(message)}</p><a class="button primary" href="/heimnetz">Zurück zur Heimnetz-Einstellung</a></main></body></html>
        """, "text/html; charset=utf-8");
}
