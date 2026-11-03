const { test } = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const vm = require("node:vm");
const path = require("node:path");
const source = fs.readFileSync(path.join(__dirname, "../src/FiaeTrainer.Web/wwwroot/theme.js"), "utf8");

// Browsergrenzen werden nachgebildet; geprüft werden sichtbares Design und gespeicherte Auswahl.
function loadTheme({ cookie = "", dark = false, blocked = false, protocol = "http:" } = {}) {
    const documentEvents = new Map();
    const windowEvents = new Map();
    const mediaEvents = [];
    const selector = { value: "system" };
    const root = { dataset: {}, style: {} };
    let cookieJar = cookie;
    let writtenCookie = null;
    let mutationCallback;
    const media = { matches: dark, addEventListener: (_, callback) => mediaEvents.push(callback) };
    const document = {
        documentElement: root,
        visibilityState: "visible",
        querySelectorAll: () => [selector],
        addEventListener: (name, callback) => documentEvents.set(name, callback)
    };
    Object.defineProperty(document, "cookie", {
        get() { if (blocked) throw new Error("Speicherung gesperrt"); return cookieJar; },
        set(value) { if (blocked) throw new Error("Speicherung gesperrt"); writtenCookie = value; cookieJar = value.split(";")[0]; }
    });
    const window = { location: { protocol }, matchMedia: () => media, addEventListener: (name, callback) => windowEvents.set(name, callback) };
    class Observer {
        constructor(callback) { mutationCallback = callback; }
        observe() { }
    }
    vm.runInNewContext(source, { window, document, MutationObserver: Observer });
    return {
        root, selector,
        getCookie: () => writtenCookie,
        choose(value) { documentEvents.get("change")({ target: { value, matches: () => true } }); },
        changeSystem(dark) { media.matches = dark; for (const callback of mediaEvents) callback(); },
        hydrateHeader() { selector.value = "system"; mutationCallback(); }
    };
}

test("Ohne gespeicherte Wahl folgt das Design dem System und dessen Änderungen", () => {
    const theme = loadTheme({ dark: true });
    assert.equal(theme.root.dataset.theme, "dark");
    assert.equal(theme.root.dataset.themePreference, "system");
    assert.equal(theme.root.style.colorScheme, "dark");
    assert.equal(theme.getCookie(), null);
    theme.changeSystem(false);
    assert.equal(theme.root.dataset.theme, "light");
});

test("Die ausdrückliche Wahl bleibt bei abweichendem System bestehen und wird gespeichert", () => {
    const theme = loadTheme({ dark: true });
    theme.choose("light");
    assert.equal(theme.root.dataset.theme, "light");
    assert.equal(theme.selector.value, "light");
    assert.match(theme.getCookie(), /ap2-theme=light; Max-Age=31536000; Path=\/; SameSite=Lax/);
    theme.changeSystem(false);
    theme.changeSystem(true);
    assert.equal(theme.root.dataset.theme, "light");
    const reloaded = loadTheme({ cookie: theme.getCookie(), dark: true });
    assert.equal(reloaded.root.dataset.theme, "light");
});

test("System kann nach einer manuellen Auswahl wieder aktiviert werden", () => {
    const theme = loadTheme({ dark: false });
    theme.choose("dark");
    theme.choose("system");
    assert.equal(theme.root.dataset.theme, "light");
    theme.changeSystem(true);
    assert.equal(theme.root.dataset.theme, "dark");
    assert.match(theme.getCookie(), /^ap2-theme=system;/);
});

test("Ungültige Cookie-Werte werden ignoriert und gesperrte Cookies verhindern den Wechsel nicht", () => {
    assert.equal(loadTheme({ cookie: "ap2-theme=unbekannt", dark: true }).root.dataset.themePreference, "system");
    const theme = loadTheme({ blocked: true, dark: false });
    theme.choose("dark");
    assert.equal(theme.root.dataset.theme, "dark");
    assert.equal(theme.getCookie(), null);
    theme.choose("unbekannt");
    assert.equal(theme.root.dataset.theme, "dark");
});

test("Nach Blazors Aktivierung zeigt der ersetzte Schalter die gespeicherte Wahl", () => {
    const theme = loadTheme({ cookie: "anderesCookie=1; ap2-theme=dark", dark: false });
    theme.hydrateHeader();
    assert.equal(theme.selector.value, "dark");
    assert.equal(theme.root.dataset.theme, "dark");
});

test("Auf HTTPS wird das Präferenzcookie zusätzlich als Secure markiert", () => {
    const theme = loadTheme({ protocol: "https:" });
    theme.choose("dark");
    assert.match(theme.getCookie(), /; Secure$/);
});
