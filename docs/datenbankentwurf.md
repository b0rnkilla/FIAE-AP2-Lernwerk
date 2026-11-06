# Datenbankentwurf

Stand: **06.11.2026 · Quellfassung 0.1.1-alpha.11**. Abschnitt 04 ist mit 30 Lernaufgaben, Modellbaukasten, automatisch gezeichnetem relationalem Modell, Spickzettel und drei eigenen Prüfungsfällen umgesetzt. Insgesamt stehen 250 Lernübungen bereit; Prüfungsaufgaben bleiben getrennt. Das bisher gebaute Alpha-9-ZIP wird nicht verändert und enthält dieses Modul noch nicht.

## Lernweg und Inhalt

Die Eventagentur Lichtblick entwickelt aus verstreuten Unterlagen einen Datenentwurf. Jede Aufgabe benennt den konkreten Planungsanlass und ist unabhängig von einer bereits gelösten Vorgängeraufgabe bearbeitbar. Die bestätigten Planungsschritte zeigen nur Erklärungen bereits richtig gelöster Aufgaben bis zur aktuellen Aufgabe.

| IDs | Kapitel | Schwerpunkte |
| --- | --- | --- |
| 1–5 | Daten verstehen | Entität und Attribut, Datensatz, Geschäftsregel statt Momentaufnahme, Kontaktlisten, erste Tabelle |
| 6–10 | Schlüssel finden | Eindeutigkeit, PK, FK, vollständiger zusammengesetzter Schlüssel |
| 11–15 | Beziehungen modellieren | Minima in beiden Richtungen, optionaler FK, 1:1, n:m mit Zuordnungstabelle, Selbstreferenz |
| 16–20 | Tabellen normalisieren | Wiederholungsgruppen, partielle und transitive Abhängigkeit, künstliche ID, keine erfundene PLZ-Abhängigkeit |
| 21–25 | Modelle untersuchen | fehlende Referenz, falscher Positions-PK, fehlende 1:1-Eindeutigkeit, falsche Mindestzahl, historische Preise |
| 26–30 | Selbstständig entwerfen | vollständige Ausschnitte, Zuordnungsattribute, alternative Schlüssel, Mindestzahl zwei, Gesamtmodell |

23 Aufgaben verwenden eigene Modelle, sieben prüfen fachliche Entscheidungen durch Einfach-/Mehrfachauswahl. Es gibt keine Lückentexte. Jede Aufgabe besitzt drei freiwillige Hinweise und eine erklärte Musterlösung. Eigene Formulierungen, keine mitgelieferten Originalprüfungen oder Abbildungen. Die bereits ausgewerteten alten Prüfungen dienen nur der Aufgabenform- und Anspruchsorientierung.

## Bedienung

Routen: `/datenbankentwurf` beziehungsweise `/datenbankentwurf/{id}`, Spickzettel `/datenbankentwurf/spickzettel`, Prüfungseinstieg `/datenbankentwurf/pruefung`, Runde `/datenbankentwurf/pruefung/runde`. Die Modulkarte bleibt Position 04; WiSo bleibt Position 08. Kopfbereich: Übersicht, Lernplan, Entwurf lernen, Prüfung üben, Entwurf-Spickzettel.

Neue Aufgaben beginnen mit **Ohne Hilfe lösen**. Modus und Entwurf bleiben je Aufgabe gespeichert. Geführt lernen kennzeichnet den Versuch als unterstützt; Hinweise, Musterlösung und Spickzettel bleiben ebenfalls als Unterstützung vermerkt. Ein neuer selbstständiger Versuch ersetzt nur den aktuellen Entwurf, frühere Abschlüsse bleiben erhalten. Bei unverändertem Ausgangsentwurf entfällt die Nachfrage. Bei einem bearbeiteten Entwurf ist die Ersetzung zu bestätigen.

Der Baukasten bietet Tabellen, Attribute, PK-Markierungen, NULL-Erlaubnis und zusätzliche eindeutige Schlüssel. Tabellen- und Anzeigenamen sind frei. Ein Fachattribut wird aus dem ausdrücklich bereitgestellten Vokabular ausgewählt; seine technische Kennung bleibt bei eigener Beschriftung unverändert. Das ist strukturierte Modellierung, keine freie Sprach- oder KI-Auswertung.

Alle PK-Häkchen einer Tabelle bilden einen gemeinsamen Primärschlüssel. Attribute mit derselben UQ-Gruppe bilden zusammen einen zusätzlichen eindeutigen Schlüssel. Verschiedene UQ-Gruppen sind unabhängige Schlüssel. Eine FK-Beziehung nennt abhängige und referenzierte Tabelle sowie alle Attributpaare; auch zusammengesetzte Referenzen sind möglich. Minima und Maximum der abhängigen Sätze sind separat auswählbar. Ein Selbstbezug ist zulässig, wenn der konkrete Auftrag ihn verlangt.

Das Diagramm zeichnet Tabellen, PK/FK/UQ-Markierungen und gerichtete Verbindungslinien automatisch. Eine ausgeschriebene Beziehungsliste nennt die vollständigen Attributpaare und Kardinalitäten. Vergrößern und Verkleinern betreffen nur die Darstellung. Auf schmalen Geräten scrollt ausschließlich die Diagrammfläche horizontal; die Tabellenkarten bleiben untereinander bedienbar. Alle Bearbeitungsaktionen sind normale beschriftete Eingaben und Buttons, ohne notwendiges Ziehen mit der Maus.

## Bewertungsvertrag und Grenzen

Die Referenzen beschreiben konkrete Attributgruppen, funktionale Abhängigkeiten und Geschäftsregeln. Es handelt sich **nicht um einen allgemeinen Normalformbeweiser** oder die perfekte automatische Bewertung jedes beliebigen freien Entwurfs.

Namen, technische IDs, Tabellen-/Attributreihenfolge, Gruppennamen von UQ-Schlüsseln und Zeichenpositionen sind nicht die fachliche Lösung. Verglichen werden:

| Gruppe | Punkte je Lernmodell |
| --- | ---: |
| Tabellen und vollständige Attributgruppen | 20 |
| Primärschlüssel | 10 |
| Zusätzliche Eindeutigkeit und NULL-Regeln | 5 |
| Gerichtete, vollständige FK-Zuordnungen | 10 |
| Kardinalitäten in beiden Richtungen | 5 |

Teilpunkte entstehen anteilig je erfülltem Element, abgerundet innerhalb der Gruppe. Fehlende oder zusätzliche Elemente zählen mit. Strukturelle Fehler erhalten eine konkrete zusätzliche Rückmeldung; ein leerer Entwurf erhält null Punkte. Auswahlaufgaben verlangen die vollständige richtige Menge ohne zusätzliche oder doppelte Auswahl und erhalten 50 oder null Punkte.

Bei Modellen zählt die beste ausdrücklich hinterlegte Variante. Aufgabe 28 erlaubt entweder den zusammengesetzten Positions-PK oder eine zusätzliche PositionId mit dem weiterhin eindeutigen fachlichen Paar. Andere Aufgaben begrenzen das Attributvokabular und nennen ihre Annahmen. Zusätzliche fachliche Attribute, Tabellen oder Eindeutigkeitsregeln können deshalb vom Vertrag abweichen. Eine künstliche ID allein beseitigt keine funktionalen Abhängigkeiten.

FK müssen vollständige eindeutige Zielschlüssel referenzieren. Ein optionaler Elternbezug benötigt passende NULL-Erlaubnis; ein PK darf nicht NULL sein. Eine 1:1-Referenz benötigt Eindeutigkeit auf der abhängigen Seite. Ein FK allein erzwingt weder mindestens einen noch mindestens zwei abhängige Sätze je Elternobjekt: entsprechende Minima sind hier fachliche Modellregeln, keine Behauptung einer ausreichenden technischen SQL-Constraint. Kreisfreiheit einer Personalhierarchie ist in Aufgabe 15 ausdrücklich nicht mitgeprüft.

Die Prüfung begrenzt Entwürfe auf acht Tabellen, 20 Attribute je Tabelle, 16 Beziehungen und acht Attributpaare je FK. Unvollständige Editorentwürfe sind speicherbar, beschädigte/nullbehaftete Strukturen oder überschrittene Grenzen werden abgewiesen. Unbekannte Fachattribute erhalten eine Rückmeldung, statt durch Beschriftungs- oder Signaturüberschneidungen als bekannte Attribute behandelt zu werden. Beschriftungen sind auf 80 Zeichen begrenzt.

## Prüfungsrunden

Drei eigene Fälle: Reparaturwerkstatt, Medienhaus und Manufaktur. Jeder Fall enthält zwei unabhängig beschriebene Modellaufträge zu je 20 Punkten und eine fachliche Mehrfachauswahl zu zehn Punkten. Die drei Fälle besitzen insgesamt neun Teilaufgaben außerhalb der 30 Lernaufgaben.

45 Minuten und 50 Trainingspunkte je Runde. Einstieg liest nur; erst Prüfung starten erzeugt eine Runde und Uhr. Ein offener Versuch wird nur mit dessen bestätigter Kennung ersetzt. Wechseln und Kontrollmarkierung sind vor Abgabe möglich. Es gibt keine vorzeitige Bewertung, Hinweise oder Musterlösung.

Die gespeicherte UTC-Zeit läuft beim Verlassen der Seite oder Schließen der App weiter. Pause hält die Restzeit an und blendet die Aufgabe aus. Beim Spickzettelbesuch wird pausiert und mit Nachschlagen vermerkt. Bereits abgelaufene Runden werden dabei abgegeben, statt nachträglich verlängert zu werden. Nach Ablauf werden verspätet eingehende Antworten verworfen und die zuletzt gespeicherten Entwürfe ausgewertet. Nach Abgabe sind Antworten unveränderlich; ein weiterer Abgabeversuch wird abgewiesen und erzeugt keinen zweiten Historieneintrag.

Modellkriterien verwenden intern denselben Vertrag wie Lernmodelle. Die zwei Modellaufträge werden auf 20 Punkte, die Auswahl auf zehn Punkte gewichtet. Die Oberfläche zeigt die gewichtete Teilpunktzahl und den Erfüllungsanteil der Kriterien. Ergebnis und bestätigte Abgabe werden sichtbar fokussiert. Punkte und Prozentwert sind keine amtliche IHK-Note; Zeitansätze sind nicht durch komplette 45-minütige Endnutzerläufe kalibriert.

## Speicherung und Aufbau

Core unter `DatabaseDesign/Models`, `Content`, `Evaluation` und `Practice`. Eigene eingebettete JSON-Dateien: `lessons.json` und `exams.json`. Web-Baukasten und Diagramm unter `Components/DatabaseDesign`, Seiten unter `Components/Pages` beziehungsweise `Pages/Training`. Keine neue Bibliothek, SQL-Verbindung, KI-Anbindung oder kostenpflichtige Schnittstelle erforderlich.

Profilfelder: `LastDesignLessonId`, `DesignLessons`, `DesignExam`, `DesignHistory`. Bestehende SQL-/Werkstatt-/Algorithmus-/WiSo- und Prüfungsfelder bleiben erhalten. Neue Felder beginnen in alten Profilen leer. `ProgressStore.Design.cs` verwendet dieselbe Dateisperre und atomare Speicherung wie die bestehenden Module. Prüfungs-ID und Revision schützen konkurrierende Fenster; Lernaufgaben besitzen ebenfalls Revisionen. Erfolge werden beim Wiederholen nicht zurückgenommen. Die Historie hält höchstens 20 Entwurfsprüfungen.

Vertragsversion: `DesignCatalog.CONTENT_VERSION = 2026-11-06`. Bei inkompatiblen Änderungen die Version und die Verträglichkeitsprüfung gemeinsam pflegen; nicht still alte Entwürfe neu interpretieren. Spickzettel-Rückwege verwenden ausschließlich eine gültige lokale Aufgabennummer beziehungsweise den festen Prüfungsweg. In der optionalen Heimnetz-Vorführung bleibt das Profil gemeinsam.

## Nachweise

Alle 30 Referenzantworten, die zugelassene alternative Schlüssellösung und die drei vollständigen Prüfungsfälle sind geprüft. Tests erfassen Umbenennen/Umordnen, falsche zusammengesetzte PK, zusätzliche Eindeutigkeit, optionale FK, Mindestzahl zwei, fehlende Referenzen, doppelte Fachattribute, beschädigte Strukturen, alte Profile, veraltete Revisionen, Fristablauf und gesperrte Abgaben. Komponentenprüfungen sichern den Einstieg ohne Uhr und verborgene Aufgaben während Pause.

Vollständiger Testlauf: 840 .NET-Tests erfolgreich einschließlich aktivierter SQL-Integration. Nach einer abschließenden Sicherung der Aufgabenkennung während Speichervorgängen zusätzlich 53 betroffene Tests erfolgreich und Web-Build ohne Warnungen/Fehler. Im isolierten Browser: zwei Lernmodelle richtig, eigener Tabellen-/Attributname erhalten, neue Prüfung 50/50 mit Nachschlagen, Pause/Neuladen, Rückweg, Abgabesperre, Diagrammvergrößerung, 390 × 844, helle/dunkle Darstellung und Erhalt nach Serverneustart. Einzelheiten und Einschränkungen im [Testprotokoll](testprotokoll.md). Kein neues Windows-Paket erstellt.
