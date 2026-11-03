const { test } = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const vm = require("node:vm");
const path = require("node:path");
const source = fs.readFileSync(path.join(__dirname, "../src/FiaeTrainer.Web/wwwroot/reconnect.js"), "utf8");

// Simuliert die Browser- und Blazor-Grenzen, nicht die Verbindungslogik des Frameworks.
function loadReconnect({ reconnect = async () => true, resume = async () => true } = {}) {
    const elements = new Map();
    let reloads = 0;
    for (const id of ["components-reconnect-modal", "reconnect-title", "reconnect-message", "reconnect-retry", "reconnect-reload"])
        elements.set(id, {
            open: false, hidden: false, disabled: false, textContent: "", events: new Map(),
            addEventListener(name, callback) { this.events.set(name, callback); },
            showModal() { assert.equal(this.open, false); this.open = true; },
            close() { this.open = false; }
        });
    const window = { Blazor: { reconnect, resumeCircuit: resume }, location: { reload() { reloads++; } } };
    vm.runInNewContext(source, { document: { getElementById: id => elements.get(id) }, window });
    const modal = elements.get("components-reconnect-modal");
    return {
        modal, title: elements.get("reconnect-title"), message: elements.get("reconnect-message"),
        retry: elements.get("reconnect-retry"),
        state(value) { modal.events.get("components-reconnect-state-changed")({ detail: { state: value } }); },
        clickRetry() { return elements.get("reconnect-retry").events.get("click")(); },
        reload() { elements.get("reconnect-reload").events.get("click")(); },
        get reloads() { return reloads; }
    };
}

test("Alle Verbindungszustände öffnen deutsche Meldungen; hide schließt den Dialog", () => {
    const ui = loadReconnect();
    const expected = { show: "Verbindung unterbrochen", retrying: "Verbindung wird gesucht", failed: "Verbindung nicht möglich", rejected: "Sitzung nicht mehr verfügbar", paused: "Deine Sitzung pausiert", "resume-failed": "Fortsetzen nicht möglich" };
    for (const [state, title] of Object.entries(expected)) {
        ui.state(state);
        assert.equal(ui.modal.open, true);
        assert.equal(ui.title.textContent, title);
        assert.equal(ui.retry.hidden, !["failed", "paused", "resume-failed"].includes(state));
    }
    ui.state("hide");
    assert.equal(ui.modal.open, false);
    ui.state("unbekannt");
    assert.equal(ui.modal.open, false);
});

test("Erfolgreiches Wiederverbinden schließt ohne Neuladen", async () => {
    const ui = loadReconnect();
    ui.state("failed");
    await ui.clickRetry();
    assert.equal(ui.modal.open, false);
    assert.equal(ui.reloads, 0);
});

test("Netzfehler bleibt verständlich und erlaubt einen weiteren Versuch", async () => {
    const ui = loadReconnect({ reconnect: async () => { throw new Error("offline"); } });
    ui.state("failed");
    await ui.clickRetry();
    assert.equal(ui.title.textContent, "Verbindung nicht möglich");
    assert.equal(ui.retry.disabled, false);
    assert.equal(ui.retry.hidden, false);
    assert.equal(ui.reloads, 0);
});

test("Abgelehnte Sitzung wird erst nach ausdrücklichem Klick neu geladen", async () => {
    const ui = loadReconnect({ reconnect: async () => false, resume: async () => false });
    ui.state("failed");
    await ui.clickRetry();
    assert.equal(ui.title.textContent, "Sitzung nicht mehr verfügbar");
    assert.equal(ui.retry.hidden, true);
    assert.equal(ui.reloads, 0);
    ui.reload();
    assert.equal(ui.reloads, 1);
});

test("Pause und fehlgeschlagenes Fortsetzen nutzen die Resume-API", async () => {
    let resumeCalls = 0;
    const ui = loadReconnect({
        reconnect: async () => { assert.fail("Keine normale Wiederverbindung für eine Pause"); },
        resume: async () => { if (++resumeCalls === 1) throw new Error("offline"); return true; }
    });
    ui.state("paused");
    assert.equal(ui.retry.textContent, "Weiterlernen");
    await ui.clickRetry();
    assert.equal(ui.title.textContent, "Fortsetzen nicht möglich");
    await ui.clickRetry();
    assert.equal(ui.modal.open, false);
    assert.equal(resumeCalls, 2);
});

test("Wiederverbindung kann auf eine fortsetzbare Sitzung wechseln", async () => {
    let resumed = false;
    const ui = loadReconnect({ reconnect: async () => false, resume: async () => { resumed = true; return true; } });
    ui.state("failed");
    await ui.clickRetry();
    assert.equal(resumed, true);
    assert.equal(ui.modal.open, false);
});

test("Mehrfachklick startet keine parallelen Verbindungsversuche", async () => {
    let complete;
    let calls = 0;
    const ui = loadReconnect({ reconnect: () => { calls++; return new Promise(resolve => { complete = resolve; }); } });
    ui.state("failed");
    const pending = ui.clickRetry();
    assert.equal(ui.retry.disabled, true);
    await ui.clickRetry();
    assert.equal(calls, 1);
    complete(true);
    await pending;
    assert.equal(ui.retry.disabled, false);
});
