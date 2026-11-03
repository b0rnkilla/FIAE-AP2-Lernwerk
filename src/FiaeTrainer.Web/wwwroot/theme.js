(() => {
    "use strict";

    const COOKIE_NAME = "ap2-theme";
    const CHOICES = ["system", "light", "dark"];
    const media = window.matchMedia("(prefers-color-scheme: dark)");
    let preference = readPreference() ?? "system";

    // Die gespeicherte Wahl wird noch vor dem Stylesheet angewendet, damit keine helle Seite aufblitzt.
    function readPreference() {
        try {
            const entry = document.cookie.split(";").map(value => value.trim()).find(value => value.startsWith(COOKIE_NAME + "="));
            const value = entry?.slice(COOKIE_NAME.length + 1);
            return CHOICES.includes(value) ? value : null;
        } catch {
            return null;
        }
    }

    function applyTheme() {
        const resolved = preference === "system" ? (media.matches ? "dark" : "light") : preference;
        document.documentElement.dataset.theme = resolved;
        document.documentElement.dataset.themePreference = preference;
        document.documentElement.style.colorScheme = resolved;
        synchronizeSelectors();
    }

    function synchronizeSelectors() {
        for (const select of document.querySelectorAll("[data-theme-selector]"))
            if (select.value !== preference)
                select.value = preference;
    }

    function setPreference(value) {
        if (!CHOICES.includes(value))
            return;
        preference = value;
        try {
            const secure = window.location.protocol === "https:" ? "; Secure" : "";
            document.cookie = `${COOKIE_NAME}=${value}; Max-Age=31536000; Path=/; SameSite=Lax${secure}`;
        } catch {
            // Bei gesperrten Cookies bleibt die Auswahl wenigstens für die laufende Seite erhalten.
        }
        applyTheme();
    }

    document.addEventListener("change", event => {
        if (event.target.matches?.("[data-theme-selector]"))
            setPreference(event.target.value);
    });
    document.addEventListener("DOMContentLoaded", applyTheme);
    // Blazor kann den Kopfbereich beim Aktivieren der Oberfläche neu einsetzen.
    new MutationObserver(synchronizeSelectors).observe(document.documentElement, {childList: true, subtree: true});
    media.addEventListener("change", () => {
        if (preference === "system")
            applyTheme();
    });
    window.addEventListener("pageshow", () => {
        preference = readPreference() ?? preference;
        applyTheme();
    });
    document.addEventListener("visibilitychange", () => {
        if (document.visibilityState === "visible") {
            preference = readPreference() ?? preference;
            applyTheme();
        }
    });
    applyTheme();
})();
