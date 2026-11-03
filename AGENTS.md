# Projektvorgaben

* Beachte docs/coding-guideline.md. Oberfläche, Lerntexte, Kommentare und Dokumentation sind deutsch, Codebezeichner englisch.
* Halte die Markdown-Dokumentation bei jedem Änderungspaket aktuell. Prüfe mindestens README.md, docs/entwicklungsprotokoll.md, docs/projektsteckbrief.md, docs/testprotokoll.md und alle fachlich betroffenen weiteren Dokumente. Beschreibe den tatsächlich umgesetzten Stand und die durchgeführten Prüfungen; frühere Protokolleinträge bleiben als Historie erhalten.
* docs/verbesserungsplan.md verfolgt die Nutzerpunkte. Die elf SQL-Anpassungen sind erledigt und vom Nutzer positiv erprobt. Ziel sind lokale Windows-Releases über GitHub; Login, Registrierung, Benutzertabellen und öffentlicher Homeserver-/nginx-Betrieb sind auf Nutzerwunsch gestrichen. Die bestehende Heimnetz-Vorführung bleibt optional.
* Verwende für Bearbeitungstests einen separaten Port und Training:ProgressDirectory. Bestehende Nutzerentwürfe und Erfolge nicht zurücksetzen oder für Tests überschreiben.

* Release-Pakete müssen selbstständig startbar sein und private Konfiguration, Lernstände, QA-Dateien, Protokolle und Debugsymbole ausschließen. Veröffentlichte Pakete speichern das Profil außerhalb des Programmordners; keine privaten Daten automatisch in ein Release übernehmen.
* Die vorhandene EXE 0.1.0 ist nur eine frühe Alpha-Momentaufnahme. Weitere Release-Pakete erst zum eigentlichen Abschluss beziehungsweise auf ausdrücklichen Nutzerwunsch erstellen. Während der Entwicklung nur die Quellprojektfassung bauen und testen.
* Version zentral in Directory.Build.props pflegen. Updatequelle ist das öffentliche Repository b0rnkilla/FIAE-AP2-Lernwerk; keine Zugangsschlüssel in App oder Repository hinterlegen. Die persönliche SQL-Einrichtung darf nur im lokalen Modus ändern, nicht in der gemeinsam nutzbaren Heimnetz-Vorführung.
