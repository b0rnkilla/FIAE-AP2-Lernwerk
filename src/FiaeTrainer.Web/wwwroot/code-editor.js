// Zeilen und Einrückung reagieren lokal, auch bevor der Server die Eingabe bestätigt.
(() => {
    "use strict";
    const editors = new WeakMap();
    const INDENT = "    ";

    function connect(root) {
        const input = root.querySelector("textarea");
        const numbers = root.querySelector(".code-editor-gutter pre");
        const status = root.querySelector("[role='status']");
        let releaseTab = false;
        let lineCount = 0;

        function refresh() {
            const count = input.value.split("\n").length;
            if (count !== lineCount) {
                numbers.textContent = Array.from({ length: count }, (_, i) => i + 1).join("\n");
                root.style.setProperty("--line-digits", Math.max(2, String(count).length));
                lineCount = count;
            }
            numbers.style.transform = `translateY(${-input.scrollTop}px)`;
        }

        // insertText erhält im Browser die native Rückgängig-Funktion. Der Ersatzpfad
        // bleibt für Browser ohne diese Bearbeitungsfunktion verfügbar.
        function replace(start, end, text, selectionStart, selectionEnd, direction) {
            const before = input.value;
            const scrollTop = input.scrollTop;
            const scrollLeft = input.scrollLeft;
            input.setSelectionRange(start, end);
            let receivedInput = false;
            const detectInput = () => { receivedInput = true; };
            input.addEventListener("input", detectInput);
            try { document.execCommand("insertText", false, text); }
            catch { /* Ältere Browser verwenden den Textfeld-Ersatzpfad. */ }
            input.removeEventListener("input", detectInput);
            if (input.value === before) input.setRangeText(text, start, end, "end");
            if (!receivedInput) input.dispatchEvent(new Event("input", { bubbles: true }));
            input.setSelectionRange(selectionStart, selectionEnd, direction);
            input.scrollTop = scrollTop;
            input.scrollLeft = scrollLeft;
            refresh();
        }

        function indent(outdent) {
            const value = input.value;
            const start = input.selectionStart;
            const end = input.selectionEnd;
            const direction = input.selectionDirection;
            const lineStart = start === 0 ? 0 : value.lastIndexOf("\n", start - 1) + 1;
            let from = start;
            let to = end;
            let replacement = INDENT;
            let nextStart = start + INDENT.length;
            let nextEnd = nextStart;

            if (outdent || start !== end) {
                from = lineStart;
                // Eine Auswahl bis zum Anfang der nächsten Zeile umfasst diese nicht.
                const lastCovered = end > start ? end - 1 : end;
                to = value.indexOf("\n", lastCovered);
                if (to < 0) to = value.length;
                const lines = value.slice(from, to).split("\n");
                const changes = lines.map(line => outdent ? -(line.match(/^(\t| {1,4})/)?.[0].length ?? 0) : INDENT.length);
                replacement = lines.map((line, index) => outdent ? line.slice(-changes[index]) : INDENT + line).join("\n");
                function move(position) {
                    let offset = from;
                    let moved = position;
                    for (let i = 0; i < lines.length; i++) {
                        if (position >= offset) moved += outdent ? -Math.min(-changes[i], position - offset) : changes[i];
                        offset += lines[i].length + 1;
                    }
                    return moved;
                }
                nextStart = move(start);
                nextEnd = move(end);
                if (replacement === value.slice(from, to)) return;
            }
            if (input.maxLength >= 0 && value.length - (to - from) + replacement.length > input.maxLength) {
                status.textContent = "Kein Platz für weitere Einrückung. Kürze zuerst deinen Entwurf.";
                return;
            }
            replace(from, to, replacement, nextStart, nextEnd, direction);
        }

        input.addEventListener("input", refresh);
        input.addEventListener("scroll", refresh);
        input.addEventListener("blur", () => { releaseTab = false; status.textContent = ""; });
        input.addEventListener("keydown", event => {
            if (event.isComposing || event.ctrlKey || event.altKey || event.metaKey || event.key === "Shift") return;
            if (event.key === "Escape") {
                releaseTab = true;
                status.textContent = "Mit Tab zum nächsten Feld, mit Umschalt+Tab zum vorherigen Feld wechseln.";
                return;
            }
            if (event.key === "Tab" && releaseTab) { releaseTab = false; return; }
            releaseTab = false;
            status.textContent = "";
            if (event.key !== "Tab" || root.dataset.codeMode !== "true" || input.readOnly) return;
            event.preventDefault();
            indent(event.shiftKey);
        });
        return { refresh };
    }

    window.fiaeCodeEditor = {
        refresh(root) {
            if (!editors.has(root)) editors.set(root, connect(root));
            editors.get(root).refresh();
        }
    };
})();
