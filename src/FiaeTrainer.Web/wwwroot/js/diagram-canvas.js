// SVG-Gesten ändern ausschließlich lokale Entwürfe über die geprüften Blazor-Handler.
const instances = new WeakMap();

export function attach(root, bridge) {
    detach(root);
    const controller = new AbortController();
    const signal = controller.signal;
    const stage = root.querySelector('.diagram-stage');
    const status = root.querySelector('.diagram-status');
    let drag = null, selected = '', connecting = false, source = '', rename = null, view = null, lastClick = null;
    let queue = Promise.resolve();
    const readOnly = () => root.dataset.readonly === 'true';
    const svg = () => stage.querySelector('svg');
    const say = text => { status.textContent = text; };
    const send = action => {
        queue = queue.then(() => bridge.invokeMethodAsync('SendAsync', action))
            .catch(() => say('Die Änderung konnte nicht übernommen werden. Prüfe die Verbindung zur App.'));
    };
    const point = event => {
        const image = svg();
        if (!image) return { x: 0, y: 0 };
        return new DOMPoint(event.clientX, event.clientY).matrixTransform(image.getScreenCTM().inverse());
    };
    const item = target => target.closest('[data-node-id], [data-link-id], [data-slot-id]');
    const id = element => element?.dataset.nodeId || element?.dataset.linkId || element?.dataset.slotId || '';
    const select = element => {
        selected = id(element);
        root.querySelectorAll('.diagram-selected').forEach(node => node.classList.remove('diagram-selected'));
        element?.classList.add('diagram-selected');
        send({ kind: 'select', id: selected });
    };
    const connect = target => {
        if (!source) { source = target; say('Jetzt das Ziel anklicken. Dasselbe Element ergibt eine Selbstverbindung.'); }
        else { send({ kind: 'connect', id: source, target }); source = ''; connecting = false; syncConnect(); say('Verbindung angelegt. Ergänze ihre Eigenschaften.'); }
    };
    const syncConnect = () => root.querySelector('[data-canvas-command="connect"]')?.setAttribute('aria-pressed', String(connecting));
    const setView = value => { view = value; svg()?.setAttribute('viewBox', value.join(' ')); };
    const fit = () => {
        const image = svg();
        if (!image) return;
        const box = image.getBBox();
        setView([box.x - 30, box.y - 30, Math.max(400, box.width + 60), Math.max(250, box.height + 60)]);
    };
    const zoom = factor => {
        const image = svg();
        if (!image) return;
        const box = image.viewBox.baseVal, width = Math.max(160, Math.min(12000, box.width * factor));
        const height = box.height * width / box.width;
        setView([box.x + (box.width - width) / 2, box.y + (box.height - height) / 2, width, height]);
    };
    const finishRename = commit => {
        if (!rename) return;
        const current = rename; rename = null;
        if (commit && !readOnly()) send({ kind: 'rename', id: current.dataset.id, text: current.value });
        current.remove(); stage.focus();
    };
    const startRename = element => {
        if (readOnly() || !element || element.dataset.rename === 'false') return;
        finishRename(true);
        const field = document.createElement('input');
        const box = element.getBoundingClientRect(), parent = stage.getBoundingClientRect();
        field.className = 'diagram-inline-name'; field.maxLength = 80; field.value = element.dataset.label || '';
        field.dataset.id = id(element); field.setAttribute('aria-label', 'Name direkt im Diagramm');
        field.style.left = `${Math.max(4, Math.min(parent.width - 220, box.left - parent.left))}px`;
        field.style.top = `${Math.max(4, Math.min(parent.height - 44, box.top - parent.top))}px`;
        stage.append(field); rename = field; field.focus(); field.select();
        field.addEventListener('blur', () => finishRename(true), { once: true });
        field.addEventListener('keydown', event => {
            if (event.key === 'Enter' || event.key === 'Escape') { event.preventDefault(); event.stopPropagation(); finishRename(event.key === 'Enter'); }
        });
    };
    root.addEventListener('click', event => {
        const command = event.target.closest('[data-canvas-command]')?.dataset.canvasCommand;
        if (command === 'fit') fit();
        if (command === 'zoom-in') zoom(0.8);
        if (command === 'zoom-out') zoom(1.25);
        if (command === 'connect' && !readOnly()) { connecting = !connecting; source = ''; syncConnect(); say(connecting ? 'Ausgang und anschließend Ziel anklicken.' : 'Verbinden beendet.'); }
    }, { signal });
    stage.addEventListener('pointerdown', event => {
        if (event.target.closest('input, button, summary') || event.button !== 0 || !svg()) return;
        const element = item(event.target), target = id(element);
        const port = event.target.closest('[data-port-id]');
        if (port && !readOnly()) {
            const start = point(event);
            const preview = document.createElementNS('http://www.w3.org/2000/svg', 'line');
            preview.classList.add('diagram-connecting'); preview.setAttribute('x1', start.x); preview.setAttribute('y1', start.y);
            preview.setAttribute('x2', start.x); preview.setAttribute('y2', start.y); svg().append(preview);
            drag = { port: port.dataset.portId, preview, start };
            stage.setPointerCapture(event.pointerId); event.preventDefault(); return;
        }
        if (connecting && target && element.dataset.nodeId && !readOnly()) { connect(target); return; }
        if (element) select(element);
        if (element && readOnly()) return;
        const start = point(event), currentView = svg().viewBox.baseVal;
        drag = { element, id: target, start, clientX: event.clientX, clientY: event.clientY, moved: false,
            view: [currentView.x, currentView.y, currentView.width, currentView.height] };
        stage.setPointerCapture(event.pointerId);
        event.preventDefault();
    }, { signal });
    stage.addEventListener('pointermove', event => {
        if (!drag || !svg()) return;
        const current = point(event);
        if (drag.port) { drag.preview.setAttribute('x2', current.x); drag.preview.setAttribute('y2', current.y); return; }
        drag.dx = Math.round(current.x - drag.start.x); drag.dy = Math.round(current.y - drag.start.y);
        drag.moved ||= Math.abs(event.clientX - drag.clientX) + Math.abs(event.clientY - drag.clientY) > 5;
        if (!drag.moved) return;
        if (drag.element?.dataset.nodeId || drag.element?.dataset.linkId && svg().dataset.diagramType === 'Sequence')
            drag.element.setAttribute('transform', `translate(${drag.element.dataset.linkId ? 0 : drag.dx},${svg().dataset.diagramType === 'Sequence' && drag.element.dataset.nodeId ? 0 : drag.dy})`);
        else if (!drag.element) {
            const scale = drag.view[2] / svg().getBoundingClientRect().width;
            setView([drag.view[0] - (event.clientX - drag.clientX) * scale, drag.view[1] - (event.clientY - drag.clientY) * scale, drag.view[2], drag.view[3]]);
        }
    }, { signal });
    const finishDrag = (event, commit) => {
        if (!drag) return;
        const current = drag; drag = null;
        lastClick = !current.moved && !current.port ? current.element : null;
        if (current.port) {
            current.preview.remove();
            if (stage.hasPointerCapture(event.pointerId)) stage.releasePointerCapture(event.pointerId);
            const target = document.elementFromPoint(event.clientX, event.clientY)?.closest('[data-node-id]')?.dataset.nodeId;
            if (commit && target && !readOnly()) { send({ kind: 'connect', id: current.port, target }); say('Verbindung angelegt. Ergänze ihre Eigenschaften.'); }
            return;
        }
        current.element?.removeAttribute('transform');
        if (stage.hasPointerCapture(event.pointerId)) stage.releasePointerCapture(event.pointerId);
        if (commit && current.moved && current.id && !readOnly()) send({ kind: 'move', id: current.id, x: current.dx || 0, y: current.dy || 0 });
    };
    stage.addEventListener('pointerup', event => finishDrag(event, true), { signal });
    stage.addEventListener('pointercancel', event => finishDrag(event, false), { signal });
    stage.addEventListener('dblclick', event => startRename(item(event.target) || lastClick), { signal });
    stage.addEventListener('keydown', event => {
        if (event.target.closest('input, textarea, select, button, summary')) return;
        const element = item(event.target);
        if (event.key === 'Escape') { connecting = false; source = ''; syncConnect(); say('Verbinden beendet.'); return; }
        if (event.key === 'Enter' && element) {
            event.preventDefault();
            if (connecting && element.dataset.nodeId && !readOnly()) connect(id(element));
            else select(element);
        }
        if (readOnly()) return;
        if (event.key === 'F2') { event.preventDefault(); startRename(element); }
        if (event.key === 'Delete' && (id(element) || selected)) { event.preventDefault(); send({ kind: 'delete', id: id(element) || selected }); }
        const arrows = { ArrowLeft: [-10, 0], ArrowRight: [10, 0], ArrowUp: [0, -10], ArrowDown: [0, 10] };
        if (arrows[event.key] && element) { event.preventDefault(); send({ kind: 'move', id: id(element), x: arrows[event.key][0], y: arrows[event.key][1] }); }
        if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'z') { event.preventDefault(); send({ kind: event.shiftKey ? 'redo' : 'undo' }); }
    }, { signal });
    // Blazor aktualisiert Inhalte; die gewählte Ansicht bleibt dabei erhalten.
    const observer = new MutationObserver(() => {
        selected = root.dataset.selectedId ?? selected;
        if (view && svg() && svg().getAttribute('viewBox') !== view.join(' ')) svg().setAttribute('viewBox', view.join(' '));
        root.querySelectorAll('[data-node-id], [data-link-id], [data-slot-id]').forEach(element => element.classList.toggle('diagram-selected', id(element) === selected));
    });
    observer.observe(stage, { childList: true, subtree: true, attributes: true, attributeFilter: ['viewBox'] });
    observer.observe(root, { attributes: true, attributeFilter: ['data-selected-id', 'data-readonly'] });
    selected = root.dataset.selectedId || '';
    root.querySelectorAll('[data-node-id], [data-link-id], [data-slot-id]').forEach(element => element.classList.toggle('diagram-selected', id(element) === selected));
    instances.set(root, { controller, observer, cancel: () => finishRename(false) });
}

export function detach(root) {
    const instance = instances.get(root);
    if (!instance) return;
    instance.controller.abort(); instance.observer.disconnect(); instance.cancel(); instances.delete(root);
}
