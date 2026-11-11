# Datenbankentwurf

Stand: **09.11.2026 · Paket 0.1.1-alpha.13 mit Lernansicht-/Storypflege und Diagramm-Erweiterung**. Abschnitt 04 ist mit 42 Lernaufgaben, ER-Ergänzungen, Arbeitsunterlagen, Modellbaukasten und automatisch gezeichnetem relationalem Modell, Spickzettel und drei eigenen Prüfungsfällen umgesetzt. Das Alpha-13-Paket enthält mit UML insgesamt 298 Lernübungen; die neuere Quellfassung Alpha 14 enthält mit Softwareplanung 346. Prüfungsaufgaben bleiben getrennt. Das auf Nutzerauftrag am 09.11.2026 gebaute Alpha-13-ZIP enthält alle 42 Entwurfsaufgaben und den UI-/Storynachlauf. Das frühere Alpha-12-ZIP bleibt unverändert bei 30 Entwurfsaufgaben.

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
| 31–34 | ER-Diagramme vervollständigen | beide Kardinalitätsrichtungen, fehlender Entitätstyp, n:m und Beziehungsattribute |
| 35–36 | Diagramme in Tabellen überführen | ER-Vorlage zu n:m-Zuordnung beziehungsweise optionaler 1:1-Ergänzung |
| 37–39 | Vorgegebene Modelle ergänzen | vollständiger zusammengesetzter FK, neue Zuordnung, eigenständiger Rückfragevorgang |
| 40–42 | Aus Unterlagen entwerfen | 3. NF aus Standliste und Leihschein, konkrete Anomalien |

30 Aufgaben verwenden relationale Modelle, vier ergänzen fachliche ER-Abbildungen über markierte Beschriftungsfelder und acht prüfen Entscheidungen durch Einfach-/Mehrfachauswahl. Die ER-Ergänzung verwendet angebotene Fachbegriffe; sie ist kein frei zeichnender ER-Editor. Jede Aufgabe besitzt drei freiwillige Hinweise und eine erklärte Musterlösung. Eigene Formulierungen, keine mitgelieferten Originalprüfungen oder Abbildungen. Die bereits ausgewerteten alten Prüfungen dienen nur der Aufgabenform- und Anspruchsorientierung.

## Bedienung

Routen: `/datenbankentwurf` beziehungsweise `/datenbankentwurf/{id}`, Spickzettel `/datenbankentwurf/spickzettel`, Prüfungseinstieg `/datenbankentwurf/pruefung`, Runde `/datenbankentwurf/pruefung/runde`. Die Modulkarte bleibt Position 04; WiSo bleibt Position 08. Kopfbereich: Übersicht, Lernplan, Entwurf lernen, Prüfung üben, Entwurf-Spickzettel.

Neue Aufgaben beginnen mit **Ohne Hilfe lösen**. Modus und Entwurf bleiben je Aufgabe gespeichert. Geführt lernen kennzeichnet den Versuch als unterstützt; Hinweise, Musterlösung und Spickzettel bleiben ebenfalls als Unterstützung vermerkt. Ein neuer selbstständiger Versuch ersetzt nur den aktuellen Entwurf, frühere Abschlüsse bleiben erhalten. Bei unverändertem Ausgangsentwurf entfällt die Nachfrage. Bei einem bearbeiteten Entwurf ist die Ersetzung zu bestätigen.

Der Baukasten bietet Tabellen, Attribute, PK-Markierungen, NULL-Erlaubnis und zusätzliche eindeutige Schlüssel. Tabellen- und Anzeigenamen sind frei. Ein Fachattribut wird aus dem ausdrücklich bereitgestellten Vokabular ausgewählt; seine technische Kennung bleibt bei eigener Beschriftung unverändert. Das ist strukturierte Modellierung, keine freie Sprach- oder KI-Auswertung.

Alle PK-Häkchen einer Tabelle bilden einen gemeinsamen Primärschlüssel. Attribute mit derselben UQ-Gruppe bilden zusammen einen zusätzlichen eindeutigen Schlüssel. Verschiedene UQ-Gruppen sind unabhängige Schlüssel. Eine FK-Beziehung nennt abhängige und referenzierte Tabelle sowie alle Attributpaare; auch zusammengesetzte Referenzen sind möglich. Minima und Maximum der abhängigen Sätze sind separat auswählbar. Ein Selbstbezug ist zulässig, wenn der konkrete Auftrag ihn verlangt.

Das Diagramm zeichnet Tabellen, PK/FK/UQ-Markierungen und gerichtete Verbindungslinien automatisch. Eine ausgeschriebene Beziehungsliste nennt die vollständigen Attributpaare und Kardinalitäten. Vergrößern und Verkleinern betreffen nur die Darstellung. Auf schmalen Geräten scrollt ausschließlich die Diagrammfläche horizontal; die Tabellenkarten bleiben untereinander bedienbar. Alle Bearbeitungsaktionen sind normale beschriftete Eingaben und Buttons, ohne notwendiges Ziehen mit der Maus.

## Bewertungsvertrag und Grenzen

Die Referenzen beschreiben konkrete Attributgruppen, funktionale Abhängigkeiten und Geschäftsregeln. Es handelt sich **nicht um einen allgemeinen Normalformbeweiser** oder die perfekte automatische Bewertung jedes beliebigen freien Entwurfs.

Namen, technische IDs, Tabellen-/Attributreihenfolge, Gruppennamen von UQ-Schlüsseln und Zeichenpositionen sind nicht die fachliche Lösung. Verglichen werden:

| Gruppe | Interne Gewichtung |
| --- | ---: |
| Tabellen und vollständige Attributgruppen | 20 |
| Primärschlüssel | 10 |
| Zusätzliche Eindeutigkeit und NULL-Regeln | 5 |
| Gerichtete, vollständige FK-Zuordnungen | 10 |
| Kardinalitäten in beiden Richtungen | 5 |

Die interne Gewichtung bestimmt den Erfüllungsgrad und die Teilpunkte der Prüfung. Im Lernmodus erscheinen ausschließlich erfüllt/noch offen und konkrete Rückmeldungen, keine Punkte oder Prozentnoten. Intern entstehen Teilwerte anteilig je erfülltem Element, abgerundet innerhalb der Gruppe. Fehlende oder zusätzliche Elemente zählen mit. Strukturelle Fehler erhalten eine konkrete zusätzliche Rückmeldung; ein leerer Entwurf erhält null Punkte. Auswahlaufgaben verlangen die vollständige richtige Menge ohne zusätzliche oder doppelte Auswahl. Im Lernen zählen sie als richtig oder noch nicht richtig; intern wird dies für die Prüfungsgewichtung auf 50 oder null abgebildet.

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

## 07.11.2026 – Einheitlicher Lernrahmen

Die Lernansicht verwendet jetzt denselben Aufbau wie SQL und Pseudocode: TaskRail links mit Kapiteln, Fortschritt, aktivem Schritt und einklappbarer Liste; gemeinsame LearningModePicker-Auswahl; Geschichte, Auftrag und Arbeitsbereich in der Mitte; bestätigte Planungsschritte rechts beziehungsweise auf schmalen Ansichten darunter. Auf Handybreite verwendet die Liste dieselbe kompakte Darstellung wie die bestehenden Lernwege. Hilfen, nummerierte Hinweise, Musterlösung und Vor-/Zurücknavigation folgen den gemeinsamen Stilen. Der Modellbaukasten bleibt mit zweispaltigen beziehungsweise mobilen einspaltigen Attributfeldern bedienbar.

Die gemeinsame Moduswahl bestätigt das Verwerfen bearbeiteter Auswahl-/Modellentwürfe. Unveränderte Ausgangsentwürfe benötigen weiterhin keine Bestätigung; frühere Erfolge bleiben erhalten. Fachbewertung, Aufgabenkennungen, Inhaltsversion, Profilformat und Prüfungsansicht sind unverändert. 53 betroffene Tests sowie isolierte Browserprüfung auf Port 5215 erfolgreich; Einzelheiten im Testprotokoll. Kein neues Paket. Alpha 12 wurde inzwischen vom Nutzer veröffentlicht; der aktuelle Gestaltungsnachlauf ist nur im Quellprojekt enthalten.

## 07.11.2026 – Zusammenhängende Geschichte um das Herbstforum

Alle 30 Lernaufgaben besitzen neue Storytexte mit Projektkoordinatorin Lea Berger. Die Vorbereitung des Herbstforums verbindet Kundenlisten, Materialdisposition, Personalplanung, Datenübernahme und Abschlussbesprechung. Jeder Schritt erklärt seinen konkreten Anlass sowie den Zweck der nächsten Modellentscheidung. Wiederaufnahmen früherer Themen sind benannt; Gastbetrieb, besondere Mindestbesetzung und bewusst begrenzte Modellausschnitte bleiben als solche erkennbar.

PreviousContext enthält bei Aufgaben 2–30 den Übergang vom Vorgänger. Er erscheint nur, wenn dieser tatsächlich abgeschlossen ist. Andernfalls zeigt die Storykarte einen Link zum offenen Planungsschritt und erklärt die unabhängige Bearbeitbarkeit. Discovery beschreibt das Ergebnis des aktuellen Schritts und erscheint erst nach richtiger Lösung beziehungsweise im Bericht bestätigter Aufgaben. Der Bericht nennt offene Zwischenstücke, wenn danach bereits Aufgaben gelöst wurden, und zeigt keine Ergebnisse späterer Aufgaben. Explanation bleibt die separate fachliche Erläuterung in der Auswertung und Musterlösung.

Die beiden neuen Inhaltsfelder sind rein erzählerisch. Aufgaben, Hinweise, Optionen, Musterlösungen, Ausgangsmodelle, IDs und Bewertungsverträge bleiben unverändert. Keine Änderung von CONTENT_VERSION oder Profilformat; gespeicherte Entwürfe und Erfolge bleiben gültig, Prüfungsfälle unverändert. 56 betroffene Tests einschließlich drei neuer UI-Verhaltensfälle sowie isolierter Browserbetrieb erfolgreich. Kein neues ZIP.

## 07.11.2026 – Vertiefte Kontrolle nach Rückmeldung zur Auswertung

Lernen verwendet DesignLearningFeedback mit genau einer Ergebnis-/Erkenntniskarte im bestehenden SQL-/Pseudocode-Stil. Keine Trainingspunkte, Prozentwerte oder IHK-Zusätze in der Lernrückmeldung. Modellkriterien erscheinen bei Bedarf als erfüllt/noch offen; falsche Auswahlantworten verraten nicht automatisch die Lösungserklärung. DesignFeedback mit Punkten bleibt ausschließlich in der abgegebenen Prüfung. Die mathematische Modellgewichtung und bereits gespeicherte Prüfungsresultate ändern sich nicht.

Leere Antworten sperren die Prüftaste. Antwort- und Moduswechsel entfernen alte Rückmeldungen. Doppelklicks zählen nur einmal; überholte Ladeanfragen kombinieren keine alte Aufgabe mit einem neueren Entwurf. /datenbankentwurf setzt die letzte Aufgabe fort. Der Baukasten erzeugt auch nach Tabellenentfernung freie Namen; alle Änderungsbefehle respektieren Schreibschutz und die vorhandenen Grenzen. Fachkennungen, Datenverträge und alte Erfolge bleiben erhalten. Ausführliche Nachweise: Testprotokoll und artifacts/qa-design-audit/verification.json. 903 .NET-/31 JavaScript-Tests erfolgreich, keine Fehler/übersprungen, Build ohne Warnungen. Kein ZIP.

## 07.11.2026 – Diagramme und Arbeitsunterlagen (Aufgaben 31–42)

Die zwölf eigenen Aufgaben setzen die Lichtblick-Geschichte mit dem Begleitprogramm zum Herbstforum fort. Die ursprünglichen Aufgaben 1–30 und alle drei Prüfungsfälle behalten ihre fachlichen Verträge, IDs und Entwürfe. SQL und Pseudocode werden auf Nutzerwunsch nicht erweitert; kein weiterer Abschnitt beginnt in dieser Alpha.

Fachliche ER-Abbildungen verwenden Rechtecke, Beziehungsrauten und Attributovale. Kardinalitäten am jeweiligen Ende zählen Objekte dieser Seite je Objekt der Gegenseite; die Textliste erklärt beide Richtungen ausdrücklich. Auch Beziehungsattribute sind dargestellt. Vier Aufgaben besitzen markierte Felder A–D mit vorgegebenen, stabil gekennzeichneten Ergänzungen. Die Auswahl aktualisiert das Diagramm. Leere, falsche und fremde Ergänzungen erhalten Rückmeldungen, keine automatisch aufgedeckte Lösung. Ein frei zeichnender Editor oder eine allgemeine ER-Modellbewertung wird damit nicht behauptet.

Zwei Übersetzungsaufgaben verlangen eigene relationale Tabellen anhand einer unveränderlichen ER-Vorlage. Drei Ergänzungsaufgaben behalten das ursprüngliche relationale Modell als Arbeitsunterlage sichtbar; bearbeitet wird seine unabhängige Ausgangskopie. Die Standliste und der Leihschein sind eigene synthetische Unterlagen, keine Prüfungsabbildungen. Anomalien werden anhand derselben Standliste konkret beurteilt. Datentypen, Statuswechsel, zeitliche Raumüberschneidungen und Auslastungsgrenzen sind keine zusätzlich behaupteten Strukturprüfungen.

`diagram-lessons.json` ergänzt den Katalog additiv. `DesignAnswer.DiagramValues` speichert ER-Ergänzungen als begrenzte Zuordnung aus Feld- und Optionskennung; alte JSON-Profile erhalten eine leere Zuordnung. Unvollständige Entwürfe sind speicherbar. `CONTENT_VERSION` bleibt 2026-11-06, weil keine alten Antwortverträge neu interpretiert werden. Neue Erfolge zählen in Übersicht/Lernplan dynamisch; 42 Entwurfs- und insgesamt 262 Lernaufgaben. Der Spickzettel akzeptiert auch Rückwege zu Aufgaben 31–42 und erklärt ER-Notation, Überführung und Belegauswertung. Lernrückmeldungen bleiben ohne Punkte, vorhandene Prüfungen unverändert.

Prüfnachweise stehen im Testprotokoll. Das veröffentlichte Alpha-12-Paket enthält weiterhin 30 Entwurfs- und insgesamt 250 Lernaufgaben; diese Erweiterung gehört nur zur Quellfassung. Kein ZIP, Commit, Push oder Upload.

## 09.11.2026 – Paket Alpha 13

Die zuvor ausschließlich im Quellprojekt liegenden Erweiterungen sind im ausdrücklich beauftragten Alpha-13-ZIP enthalten. Native EXE ohne SQL mit eigenem Profil geprüft; ER-Aufgabe 31 korrekt ergänzt und bewertet, bisheriger Entwurfserfolg 37 erhalten. Fachverträge und Prüfungsfälle unverändert. Älteres Alpha-12-ZIP unverändert; Details in [lokale Releases](lokale-releases.md).
