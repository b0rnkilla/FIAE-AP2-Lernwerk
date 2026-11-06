(() => {
    "use strict";
    // Die Einstellungsseite verwendet echte HTTP-Formulare statt eines unterbrochenen Blazor-Circuits.
    document.addEventListener("click", async event => {
        const button = event.target.closest("[data-network-copy]");
        if (!button) return;
        const status = button.closest(".network-address").querySelector("[data-network-copy-status]");
        try {
            await navigator.clipboard.writeText(button.dataset.networkCopy);
            status.textContent = "Adresse kopiert.";
        } catch {
            status.textContent = "Markiere die Adresse oben und kopiere sie mit Strg+C oder dem Kopieren-Menü.";
        }
    });
    document.addEventListener("submit", event => {
        const form = event.target.closest("[data-network-form]");
        if (!form) return;
        // Erst nach Erzeugung der Formulardaten sperren, damit der geklickte Modus mitgesendet wird.
        setTimeout(() => {
            form.querySelectorAll("button").forEach(button => { button.disabled = true; });
            const progress = form.querySelector("[data-network-progress]");
            if (progress) progress.hidden = false;
        }, 0);
    });
    const waiting = document.querySelector("[data-network-wait]");
    if (!waiting) return;
    const expected = waiting.dataset.networkWait === "true";
    let attempts = 0;
    const poll = async () => {
        try {
            const response = await fetch("/heimnetz/bereit", { cache: "no-store", signal: AbortSignal.timeout(2500) });
            if (response.ok && (await response.json()).enabled === expected) { location.replace("/heimnetz"); return; }
        } catch { /* Während des kontrollierten Neustarts ist die Verbindung vorübergehend geschlossen. */ }
        if (++attempts < 45) setTimeout(poll, 1500);
        else waiting.textContent = "Der Neustart dauert länger. Prüfe das Programmfenster und öffne die Heimnetz-Einstellung erneut.";
    };
    setTimeout(poll, 2500);
})();
