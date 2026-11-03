(() => {
    "use strict";

    // Der Dialog gehört nicht zum unterbrochenen Circuit und benötigt keine .NET-Ereignishandler.
    const modal = document.getElementById("components-reconnect-modal");
    const title = document.getElementById("reconnect-title");
    const message = document.getElementById("reconnect-message");
    const retryButton = document.getElementById("reconnect-retry");
    const reloadButton = document.getElementById("reconnect-reload");
    const STATES = {
        show: ["Verbindung unterbrochen", "Die Verbindung wird wiederhergestellt. Einen Moment bitte."],
        retrying: ["Verbindung wird gesucht", "Die Anwendung ist noch nicht erreichbar. Wir versuchen es erneut."],
        failed: ["Verbindung nicht möglich", "Prüfe, ob das Lernwerk noch läuft. Versuche dann erneut, dich zu verbinden."],
        rejected: ["Sitzung nicht mehr verfügbar", "Das Lernwerk wurde möglicherweise neu gestartet. Lade die Seite neu, um weiterzulernen."],
        paused: ["Deine Sitzung pausiert", "Setze deine Sitzung fort, um weiterzulernen."],
        "resume-failed": ["Fortsetzen nicht möglich", "Die Sitzung konnte nicht fortgesetzt werden. Versuche es erneut oder lade die Seite neu."]
    };
    let state = "hide";
    let busy = false;

    function showState(nextState) {
        if (nextState === "hide") {
            state = nextState;
            if (modal.open)
                modal.close();
            return;
        }
        const content = STATES[nextState];
        if (!content)
            return;
        state = nextState;
        title.textContent = content[0];
        message.textContent = content[1];
        retryButton.hidden = !["failed", "paused", "resume-failed"].includes(state);
        retryButton.textContent = state === "paused" ? "Weiterlernen" : "Erneut verbinden";
        retryButton.disabled = busy;
        if (!modal.open)
            modal.showModal();
    }

    // Blazor verwaltet seine Klassen; wir übersetzen die Zustandsereignisse in deutsche Rückmeldungen.
    modal.addEventListener("components-reconnect-state-changed", event => showState(event.detail.state));
    reloadButton.addEventListener("click", () => window.location.reload());
    retryButton.addEventListener("click", async () => {
        if (busy)
            return;
        busy = true;
        retryButton.disabled = true;
        const resuming = state === "paused" || state === "resume-failed";
        try {
            let connected = resuming ? await window.Blazor.resumeCircuit() : await window.Blazor.reconnect();
            if (!connected && !resuming && typeof window.Blazor.resumeCircuit === "function")
                connected = await window.Blazor.resumeCircuit();
            showState(connected ? "hide" : "rejected");
        } catch {
            showState(resuming ? "resume-failed" : "failed");
        } finally {
            busy = false;
            retryButton.disabled = false;
        }
    });
})();
