// Nur echte Aufgabenwechsel über die linke Liste bewahren die Position der Gesamtseite.
(() => {
    "use strict";
    let pending = null;
    const area = path => /^\/(sql|sql-werkstatt|algorithmen)(?:\/\d+)?\/?$/.exec(path)?.[1];
    document.addEventListener("click", event => {
        const link = event.target.closest?.(".mission-rail a.mission-link");
        if (!link || event.button !== 0 || event.ctrlKey || event.metaKey || event.shiftKey || event.altKey) { pending = null; return; }
        const target = new URL(link.href, location.href);
        const currentArea = area(location.pathname);
        if (target.origin !== location.origin || !currentArea || area(target.pathname) !== currentArea || target.hash) { pending = null; return; }
        const rail = link.closest(".mission-rail").querySelector("nav");
        pending = { path: target.pathname, x: scrollX, y: scrollY, railY: rail?.scrollTop ?? 0 };
    }, true);
    addEventListener("popstate", () => { pending = null; });
    window.fiaeTaskNavigation = {
        restore(path) {
            if (!pending || pending.path !== path) return;
            const saved = pending;
            pending = null;
            // Nach dem vollständigen Blazor-Rendern und dessen eigener Scrollbehandlung wiederherstellen.
            requestAnimationFrame(() => requestAnimationFrame(() => {
                if (location.pathname !== saved.path || pending) return;
                const rail = document.querySelector(".mission-rail nav");
                if (rail) rail.scrollTop = saved.railY;
                scrollTo({ left: saved.x, top: saved.y, behavior: "instant" });
            }));
        }
    };
})();
