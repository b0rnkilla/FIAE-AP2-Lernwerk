const { test } = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const vm = require("node:vm");
const path = require("node:path");
const source = fs.readFileSync(path.join(__dirname, "../src/FiaeTrainer.Web/wwwroot/code-editor.js"), "utf8");

// Simuliert Textauswahl und Browserereignisse; echtes Undo und Fokus werden zusätzlich im Browser geprüft.
function editor(value = "", { codeMode = true, nativeInsert = true } = {}) {
    const events = new Map();
    let inputCount = 0;
    const input = {
        value, selectionStart: 0, selectionEnd: 0, selectionDirection: "forward", maxLength: 12000,
        readOnly: false, scrollTop: 0, scrollLeft: 0,
        setSelectionRange(start, end, direction = "forward") { this.selectionStart = start; this.selectionEnd = end; this.selectionDirection = direction; },
        setRangeText(text, start, end) { this.value = this.value.slice(0, start) + text + this.value.slice(end); },
        addEventListener(name, listener) { if (!events.has(name)) events.set(name, new Set()); events.get(name).add(listener); },
        removeEventListener(name, listener) { events.get(name).delete(listener); },
        dispatchEvent(event) { if (event.type === "input") inputCount++; for (const listener of events.get(event.type) ?? []) listener(event); }
    };
    const numbers = { textContent: "", style: {} };
    const status = { textContent: "" };
    const root = {
        dataset: { codeMode: String(codeMode) }, style: { setProperty() {} },
        querySelector(selector) { return selector === "textarea" ? input : selector.includes("pre") ? numbers : status; }
    };
    const window = {};
    const document = { execCommand() {
        if (!nativeInsert) throw new Error("Nicht unterstützt");
        input.setRangeText(arguments[2], input.selectionStart, input.selectionEnd);
        input.dispatchEvent({ type: "input" });
    } };
    vm.runInNewContext(source, { window, document, Event: class { constructor(type) { this.type = type; } } });
    const refresh = () => window.fiaeCodeEditor.refresh(root);
    refresh();
    return {
        input, numbers, status, root, refresh, get inputCount() { return inputCount; },
        select(start, end = start, direction = "forward") { input.setSelectionRange(start, end, direction); },
        key(key, options = {}) {
            const event = { type: "keydown", key, prevented: false, preventDefault() { this.prevented = true; }, ...options };
            input.dispatchEvent(event); return event.prevented;
        },
        event(type) { input.dispatchEvent({ type }); }
    };
}

test("Zeilennummern zählen Leerzeilen, folgen Scrollen und geladenen Entwürfen", () => {
    const e = editor("AUSGABE 1\n\nAUSGABE 2\n");
    assert.equal(e.numbers.textContent, "1\n2\n3\n4");
    e.input.scrollTop = 72; e.event("scroll");
    assert.equal(e.numbers.style.transform, "translateY(-72px)");
    e.input.value = ""; e.refresh();
    assert.equal(e.numbers.textContent, "1");
    e.refresh(); e.key("Tab");
    assert.equal(e.inputCount, 1, "Erneutes Rendern verdoppelt keine Ereignisbehandlung");
});

test("Tab fügt vier Leerzeichen am Cursor ein und erhält Scrollposition", () => {
    const e = editor("x = 1"); e.select(2); e.input.scrollTop = 30; e.input.scrollLeft = 10;
    assert.equal(e.key("Tab"), true);
    assert.equal(e.input.value, "x     = 1");
    assert.equal(e.input.selectionStart, 6);
    assert.equal(e.input.selectionEnd, 6);
    assert.equal(e.input.scrollTop, 30); assert.equal(e.input.scrollLeft, 10);
    assert.equal(e.inputCount, 1);
});

test("Markierte Zeilen lassen sich gemeinsam ein- und ausrücken, Folgelinie bleibt erhalten", () => {
    const e = editor("x = 1\ny = 2\nAUSGABE x"); e.select(0, 12, "backward");
    e.key("Tab");
    assert.equal(e.input.value, "    x = 1\n    y = 2\nAUSGABE x");
    assert.equal(e.input.selectionDirection, "backward");
    e.key("Tab", { shiftKey: true });
    assert.equal(e.input.value, "x = 1\ny = 2\nAUSGABE x");
    assert.equal(e.input.selectionStart, 0); assert.equal(e.input.selectionEnd, 12);
});

test("Ausrücken am Zeilenanfang entfernt Leerzeichen oder einen echten Tab", () => {
    const e = editor("  AUSGABE 1\n\tAUSGABE 2");
    e.key("Tab", { shiftKey: true });
    assert.equal(e.input.value, "AUSGABE 1\n\tAUSGABE 2");
    assert.equal(e.input.selectionStart, 0);
    e.select(11); e.key("Tab", { shiftKey: true });
    assert.equal(e.input.value, "AUSGABE 1\nAUSGABE 2");
    assert.equal(e.input.selectionStart, 10);
    const blank = editor("\nAUSGABE 1"); blank.key("Tab");
    assert.equal(blank.input.value, "    \nAUSGABE 1");
});

test("Eine Auswahl innerhalb der Einrückung bleibt nach Ausrücken gültig", () => {
    const e = editor("    AUSGABE 1"); e.select(1, 2);
    e.key("Tab", { shiftKey: true });
    assert.equal(e.input.value, "AUSGABE 1");
    assert.equal(e.input.selectionStart, 0); assert.equal(e.input.selectionEnd, 0);
});

test("Esc gibt den nächsten Tab in beide Richtungen zur Navigation frei", () => {
    const e = editor("AUSGABE 1");
    e.key("Escape"); assert.equal(e.key("Tab"), false);
    e.key("Escape"); e.key("Shift"); assert.equal(e.key("Tab", { shiftKey: true }), false);
    e.key("Escape"); e.event("blur"); assert.equal(e.key("Tab"), true);
    e.key("Escape"); e.key("ArrowRight"); assert.equal(e.key("Tab"), true);
});

test("Antwortfelder, schreibgeschützte Felder und Browserkombinationen behalten Tab-Navigation", () => {
    const e = editor("42", { codeMode: false }); assert.equal(e.key("Tab"), false);
    e.root.dataset.codeMode = "true"; e.input.readOnly = true; assert.equal(e.key("Tab"), false);
    e.input.readOnly = false;
    for (const option of ["ctrlKey", "altKey", "metaKey", "isComposing"]) assert.equal(e.key("Tab", { [option]: true }), false);
    assert.equal(e.input.value, "42");
});

test("Zeichenlimit verhindert Abschneiden des Codes und Ausrücken bleibt möglich", () => {
    const e = editor("    AUSGABE 1"); e.input.maxLength = e.input.value.length; e.select(0, e.input.value.length);
    e.key("Tab"); assert.equal(e.input.value, "    AUSGABE 1");
    assert.match(e.status.textContent, /Kürze/);
    e.key("Tab", { shiftKey: true }); assert.equal(e.input.value, "AUSGABE 1");
});

test("Ohne native Bearbeitungsfunktion informiert der Ersatzpfad die Entwurfsspeicherung", () => {
    const e = editor("AUSGABE 1", { nativeInsert: false }); e.key("Tab");
    assert.equal(e.input.value, "    AUSGABE 1"); assert.equal(e.inputCount, 1);
});
