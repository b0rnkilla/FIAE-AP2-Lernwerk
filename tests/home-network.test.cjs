const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const vm = require('node:vm');
const source = fs.readFileSync('src/FiaeTrainer.Web/wwwroot/home-network.js', 'utf8');

function page(expected, responses = []) {
    const listeners = {}, timers = [], navigations = [], writes = [];
    const waiting = expected === undefined ? null : { dataset: { networkWait: String(expected) }, textContent: '' };
    vm.runInNewContext(source, {
        document: { addEventListener: (name, handler) => listeners[name] = handler, querySelector: () => waiting },
        navigator: { clipboard: { writeText: async text => writes.push(text) } },
        setTimeout: handler => timers.push(handler), AbortSignal: { timeout: () => ({}) },
        fetch: async () => { const response = responses.shift(); if (response instanceof Error) throw response; return response ?? { ok: false }; },
        location: { replace: url => navigations.push(url) }
    });
    return { listeners, timers, navigations, writes, waiting };
}

test('Normale Einstellungsseiten starten keine Hintergrundabfragen', () => {
    assert.equal(page(undefined).timers.length, 0);
});

test('Neustart wartet auf Erreichbarkeit und die gewünschte Betriebsart', async () => {
    const p = page(true, [new Error('Neustart'), { ok: false }, { ok: true, json: async () => ({ enabled: false }) }, { ok: true, json: async () => ({ enabled: true }) }]);
    for (let i = 0; i < 3; i++) { await p.timers.shift()(); assert.equal(p.navigations.length, 0); }
    await p.timers.shift()(); assert.deepEqual(p.navigations, ['/heimnetz']); assert.equal(p.timers.length, 0);
});

test('Dauerhaft fehlgeschlagener Neustart bietet eine verständliche manuelle Rückkehr', async () => {
    const p = page(false);
    for (let i = 0; i < 45; i++) await p.timers.shift()();
    assert.equal(p.timers.length, 0); assert.equal(p.navigations.length, 0);
    assert.match(p.waiting.textContent, /Prüfe das Programmfenster/);
});

test('Kopierknopf übernimmt ausschließlich die angezeigte Adresse und bestätigt den Erfolg', async () => {
    const p = page(undefined); const status = { textContent: '' };
    const button = { dataset: { networkCopy: 'http://192.168.1.21:5213/' }, closest: () => ({ querySelector: () => status }) };
    await p.listeners.click({ target: { closest: () => button } });
    assert.deepEqual(p.writes, ['http://192.168.1.21:5213/']); assert.equal(status.textContent, 'Adresse kopiert.');
});

test('Formular sperrt erst nach der Übernahme des angeklickten Modus', () => {
    const p = page(undefined), button = { disabled: false }, progress = { hidden: true };
    p.listeners.submit({ target: { closest: () => ({ querySelectorAll: () => [button], querySelector: () => progress }) } });
    assert.equal(button.disabled, false); p.timers.shift()(); assert.equal(button.disabled, true); assert.equal(progress.hidden, false);
});
