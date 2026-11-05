const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const vm = require('node:vm');
const source = fs.readFileSync('src/FiaeTrainer.Web/wwwroot/task-navigation.js', 'utf8');

function page(path = '/sql/3') {
    const listeners = {}, frames = [], calls = [];
    const rail = { scrollTop: 240 };
    const location = { pathname: path, origin: 'http://localhost:5200', href: 'http://localhost:5200' + path };
    const window = {};
    vm.runInNewContext(source, {
        window, location, URL, scrollX: 0, scrollY: 175,
        document: { addEventListener: (name, action) => { listeners[name] = action; }, querySelector: () => rail },
        addEventListener: (name, action) => { listeners[name] = action; },
        requestAnimationFrame: action => frames.push(action), scrollTo: value => calls.push(value)
    });
    const click = (targetPath, extra = {}) => listeners.click({
        button: 0, target: { closest: () => ({ href: 'http://localhost:5200' + targetPath, closest: () => ({ querySelector: () => rail }) }) }, ...extra
    });
    return { window, listeners, location, frames, calls, rail, click, flush() { while (frames.length) frames.shift()(); } };
}

test('Listenklick erhält Gesamtseite und Listenposition erst nach dem fertigen Zielrender', () => {
    const p = page(); p.click('/sql/4'); p.rail.scrollTop = 0;
    p.window.fiaeTaskNavigation.restore('/sql/3'); assert.equal(p.frames.length, 0);
    p.location.pathname = '/sql/4'; p.window.fiaeTaskNavigation.restore('/sql/4');
    assert.equal(p.calls.length, 0); p.flush();
    assert.equal(p.calls[0].top, 175); assert.equal(p.calls[0].behavior, 'instant'); assert.equal(p.rail.scrollTop, 240);
    p.window.fiaeTaskNavigation.restore('/sql/4'); p.flush(); assert.equal(p.calls.length, 1);
});

test('Andere Module, Prüfungen, Hashes und neue Tabs erhalten keine erzwungene Position', () => {
    for (const target of ['/wiso/runde', '/sql/pruefung', '/algorithmen/1', '/sql/4#hinweis']) {
        const p = page(); p.click(target); p.location.pathname = target; p.window.fiaeTaskNavigation.restore(target); p.flush(); assert.equal(p.calls.length, 0);
    }
    const p = page(); p.click('/sql/4', { ctrlKey: true }); p.location.pathname = '/sql/4'; p.window.fiaeTaskNavigation.restore('/sql/4'); p.flush(); assert.equal(p.calls.length, 0);
});

test('Zurücknavigation und überholte Zielseiten verwerfen alte Scrollwerte', () => {
    const p = page(); p.click('/sql/4'); p.listeners.popstate(); p.location.pathname = '/sql/4'; p.window.fiaeTaskNavigation.restore('/sql/4'); p.flush(); assert.equal(p.calls.length, 0);
    p.location.pathname = '/sql/3'; p.click('/sql/4'); p.location.pathname = '/sql/4'; p.window.fiaeTaskNavigation.restore('/sql/4'); p.location.pathname = '/sql/5'; p.flush(); assert.equal(p.calls.length, 0);
});

test('SQL-Werkstatt und Algorithmen verwenden denselben begrenzten Mechanismus', () => {
    for (const prefix of ['sql-werkstatt', 'algorithmen']) {
        const p = page('/' + prefix + '/1'); p.click('/' + prefix + '/2'); p.location.pathname = '/' + prefix + '/2'; p.window.fiaeTaskNavigation.restore(p.location.pathname); p.flush(); assert.equal(p.calls.length, 1);
    }
});
