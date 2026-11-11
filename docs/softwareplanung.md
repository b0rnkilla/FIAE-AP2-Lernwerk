# Softwareplanung und OOP – Abschnitt 05

Quellstand **0.1.1-alpha.14**, Inhaltsvertrag **2026-11-09**. 48 eigene Lernaufgaben und drei eigene Prüfungsfälle mit je drei Teilen. Das vorhandene Windows-ZIP Alpha 13 bleibt unverändert; Abschnitt 05 ist noch nicht als neues Paket gebaut.

## Lernweg und Umfang

Mira König entwickelt die Planung für das Kulturzentrum Nordhafen aus Abschnitt 03 weiter. Jede Aufgabe beschreibt ihre nötigen Annahmen eigenständig. Der Übergang zum Vorgänger erscheint erst nach dessen Abschluss; sonst führt ein Link zum offenen Schritt. Erkenntnisse werden erst nach Erfolg freigeschaltet. Der Bericht enthält ausschließlich bestätigte Schritte bis zur aktuellen Aufgabe; ausgelassene Bereiche bleiben als aufklappbare Gruppen verlinkt.

| Aufgaben | Kapitel | Schwerpunkte |
| --- | --- | --- |
| 01–06 | Anforderungen präzisieren | Funktionale und nichtfunktionale Anforderungen, messbare Akzeptanz, Lasten-/Pflichtenheft, User Story, Grenzen und vereinbarter Umfang |
| 07–12 | Objekte und Verantwortlichkeiten | Identität, Konstruktor, eigener Zustand, statischer Zähler, Kapselung und kontrollierte Zustandsänderung |
| 13–18 | Zusammenarbeit und Erweiterbarkeit | Interfaces, abstrakte Klassen, dynamische Methodenwahl, Überladen/Überschreiben, Zusammensetzung und austauschbare Partner |
| 19–24 | Entwurfsmuster | Observer, Strategy, Factory Method, Adapter, Singleton und dessen Grenzen; eigene Strukturausschnitte |
| 25–30 | Softwarearchitektur | Schichten, Abhängigkeit auf Verträge, ausdrücklich vereinbarte MVC-Variante, Browser-/Servergrenze |
| 31–36 | Schnittstellen | HTTP-Verträge, Ressourcenpfade, Status, JSON-Nachrichten und XML-Beleg |
| 37–42 | Bedienoberflächen | Beschriftung, Feldtypen, Pflichtangaben, Reihenfolge, Gruppen, Hilfen und eigene Formularprototypen |
| 43–48 | Werkzeuge und Wiederverwendung | IDE, Compiler/Interpreter, Debugger, Dokumentation, Bibliothek/Framework, Auswahlkriterien, Git und Übergabe |

Sechs Arbeitsformen: zehn Auswahlaufgaben, 16 Ausarbeitungen, acht Klassenentwürfe, vier Architekturentwürfe, fünf Nachrichten-/API-Aufträge und fünf Formularentwürfe. Neue Entwürfe beginnen leer; Reparaturen übernehmen eine unabhängige Kopie der sichtbaren Vorlage. Die Codeunterlagen werden gelesen und über Zustand-/Rückgabetabellen ausgewertet. Es gibt keine Ausführung beliebigen C#- oder Fremdcodes und keine echten API-Aufrufe.

## Bedienung und Hilfen

Gemeinsame linke TaskRail, LearningModePicker, mittiger Auftrag/Arbeitsbereich und rechte Planungsschritte entsprechen den vorhandenen Modulen. Ohne Hilfe ist der Standard. Geführtes Lernen bietet drei Hinweise, Spickzettel mit lokalem Rückweg und eine bewusst aufgedeckte Lösung. Ein neuer Selbsttest verlangt bei eigenem Entwurf die gemeinsame Bestätigung. Historische Erfolge bleiben erhalten. Leere Antworten sperren die Prüftaste; eine freie Zusatzbegründung allein gilt nicht als abgegebene Fachlösung.

Klassenentwürfe verwenden die bestehenden UML-Darstellungs- und Bearbeitungskomponenten mit eigenen Aufträgen und eigenen Antworten. Architektur und Formulare verwenden einen eigenen Baukasten mit stabilen Fachrollen und freien Anzeigenamen. Hinzufügen, Auswählen, Verbinden, Entfernen sowie Früher/Später sind ohne Ziehen möglich. Diagramme besitzen eine Textalternative, Vergrößerung und einen begrenzten seitlichen Scrollbereich. Lange Rückverbindungen werden unter den Kästen geführt; Verbindungsbeschriftungen liegen außerhalb der Elemente.

Die Formularvorschau zeigt die wirklich gewählten Feldarten, Reihenfolge, Gruppen und eigenen Hilfen. Lokale Beispieleingaben und Pflichtfeldmeldungen können ausprobiert werden. Diese Vorschauwerte werden nicht als Lernantwort gespeichert; der Entwurf und die Begründung werden gespeichert. Die Vorschau ist keine echte Buchung und prüft keine vollständige Terminverfügbarkeit oder alle denkbaren Feldvalidierungen.

## Fachliche Bewertung und Grenzen

- Auswahl: vollständige Menge stabiler Optionskennungen, ohne zusätzliche oder doppelte Antworten.
- Ausarbeitung: einzelne beschriftete Felder; angebotene Auswahlwerte exakt. Explizite Zahlenfelder akzeptieren äquivalente Dezimalschreibweisen mit Punkt/Komma; keine beliebige Einheitenauslegung. Vertragsnamen und Ressourcenpfade bleiben verbindlich.
- Klassen: vorhandene UML-Prüflogik für Typen, Sichtbarkeit, Parameterlisten, Abstraktion, statische Merkmale, Beziehungen und Multiplizitäten. Freie Anzeigenamen/Parameternamen sind erlaubt; Fachkennungen bleiben stabil. Es werden klar beschriebene Strukturausschnitte bewertet, keine beliebigen Methodenrümpfe bewiesen.
- Architektur: eindeutige Fachrollen, Elementarten, Gruppen, vollständige gerichtete Verbindungsmengen. Zeichenpositionen und Reihenfolge der Bausteine zählen nicht. Zusätzliche oder doppelte Verbindungen ersetzen keine fehlende Verbindung.
- Formular: eindeutige Rollen, Feldtypen, Gruppen, Pflichtmarkierungen und ausdrücklich geforderte nichtleere Hilfetexte; die vereinbarte Feldreihenfolge zählt. Die sprachliche Qualität eigener Beschriftungen und Hilfetexte bleibt Selbstkontrolle.
- JSON: struktureller Vergleich von Namen, Typen und Werten; Objektfeldreihenfolge frei, Arrayreihenfolge verbindlich. Numerisch gleichwertige Werte sind erlaubt. Fehlende/zusätzliche oder doppelte Felder, falsche Typen und ungültige Syntax scheitern.
- XML: Elementnamen einschließlich Namensräumen, Hierarchie, Attribute, Elementreihenfolge und Textwerte zählen. Attributreihenfolge, Einrückung, Kommentare und erlaubte Deklaration zählen nicht. DTD/externe Entitäten sind gesperrt; kein Zugriff auf Dateien oder fremde Dienste. Die Aufgaben verlangen ausdrücklich ihre konkrete Nachricht, kein allgemeines XML-Schema.

Freie Begründungen werden gespeichert und erhalten eine fachliche Kriterienliste sowie die Mustererklärung. Sie verändern **keine** automatische Bewertung und werden nicht anhand von Schlüsselwörtern benotet. Eine beliebige alternative Architektur wird nicht automatisch als gleichwertig erkannt; die Aufgaben grenzen ihre geprüften Varianten ausdrücklich ein. Keine vollständige AP2-Katalogabdeckung oder amtliche Korrektur behaupten.

Im Lernen enthält genau eine Ergebniskarte Rückmeldung, Erkenntnis und Hilfeneinordnung, ohne Punkte/Prozente/IHK-Zusatz. Falsche Antworten verraten nicht automatisch die vollständige Erklärung. Antwort-/Moduswechsel entfernen alte Rückmeldungen. Die zusätzliche aufklappbare Kriterienliste dient ausschließlich der Selbstkontrolle.

## Prüfungsfälle

| Kennung | Eigener Fall | Drei unabhängig bearbeitbare Ausschnitte |
| --- | --- | --- |
| radverleih | Fahrräder für das Wochenende | Messbarer Suchvertrag, Schichtenarchitektur, API zum Anlegen einer Ausleihe |
| kurse | Kurse im Bildungszentrum | Abstrakter Preisvertrag, partielle API-Änderung, Anmeldemaske |
| reparatur | Reparaturen nachvollziehbar annehmen | User Story, gruppierte Annahmemaske, Observer-Entscheidung |

Je **45 Minuten**, **50 Trainingspunkte**, Gewichtung **20/20/10**. Start erst nach Aktion, offene Prüfung nur nach zusätzlicher Ersetzungsbestätigung. UTC-Frist, Pause mit ausgeblendeten Aufgaben, Kontrollmarkierungen, Nachschlagen mit Kennzeichnung, endgültige Antwortsperre und höchstens 20 Historieneinträge je Modul. Browserende hält die Uhr nicht an; ausdrücklich gespeicherte Pause überlebt den Hostneustart. Nach Fristablauf zählen ausschließlich zuvor gespeicherte Antworten. Doppelte/veraltete Abgaben werden zurückgewiesen; die Historie zählt einmal.

Die internen Kriterien summieren sich je Teil auf 50 Anteile. Prüfungsbewertung skaliert ganzzahlig auf die Teilgewichtung; freie Begründungen bringen keine Punkte. Diese Trainingsdauer bildet einen begrenzten Modulausschnitt ab. Der offizielle schriftliche Bereich Planen eines Softwareproduktes dauert 90 Minuten. Keine amtliche IHK-Note.

## Speicherung, Struktur und Grenzen

`Core/SoftwareDesign` trennt Models, Content, Evaluation und Practice; `Web/Components/SoftwareDesign` enthält die fachliche Darstellung. Lern-/Prüfungsseiten liegen unter `/softwareplanung`, `/softwareplanung/pruefung` und `/softwareplanung/spickzettel`. Inhalt ist als eigene JSON-Ressource eingebettet. Keine Originalprüfungsseiten, -formulierungen oder -lösungen mitliefern.

Zusätzliche Profilfelder: `SoftwareLessons`, `LastSoftwareLessonId`, `SoftwareExam`, `SoftwareHistory`. Fehlende Felder alter Profile erhalten leere Vorgaben; SQL, Werkstatt, Algorithmen, UML, Entwurf und WiSo bleiben erhalten. Revisions- und Inhaltsversionsschutz sowie dieselbe atomare Dateisperre gelten für alle Schreibvorgänge. Lesender Moduleinstieg legt keinen Entwurf an. Der letzte eigene Auftrag wird fortgesetzt. Prüfungs-Autospeicherung erhält die Referenzen der sichtbaren Editorelemente, damit unmittelbar folgende Eingabeereignisse gültig bleiben; die Datei erhält dennoch eine unabhängige Übergabekopie.

Unvollständige Entwürfe bleiben speicherbar. Maximal 16 Architektur-/Formularelemente, 32 Verbindungen, 24 Antwortfelder, 500 Zeichen je Antwortwert, 80 Zeichen für Kennungen/Anzeigenamen, 200 für Hilfetext, 8.000 für Nachricht, 4.000 für Begründung. JSON/XML-Verschachtelung ist begrenzt; UML-Entwürfe verwenden deren bestehende Grenzen. Alle schreibenden Editorhandler beachten ReadOnly und Grenzen, auch nach gesperrter Prüfungsabgabe.

## Quellenstand

Fachliche Orientierung geprüft am 09.11.2026. Eigene Trainingsfälle; Altprüfungen dienten ausschließlich der Themen-/Formorientierung.

- [Ausbildungsordnung § 13](https://www.gesetze-im-internet.de/fiausbv/__13.html): Anforderungen, Entwurf, Qualität und Prüfungsbereich.
- [Microsoft: Architekturprinzipien](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles): Zuständigkeiten und Abhängigkeiten.
- [IETF RFC 9110](https://www.rfc-editor.org/rfc/rfc9110.html): HTTP-Semantik; konkrete API-Verträge bleiben Aufgabenannahmen.
- [W3C WAI: Forms Tutorial](https://www.w3.org/WAI/tutorials/forms/): Beschriftung, Gruppen, Hinweise und Rückmeldungen.

Am 11.11.2026 hat der Nutzer nach kurzem Einblick und einigen gelösten Aufgaben die Umsetzung positiv bewertet. Die ausführliche Erprobung aller Aufgaben/Prüfungsfälle und Zeitkalibrierung bleiben offen. Prüfnachweise stehen im [Testprotokoll](testprotokoll.md). ZIP und Release-Informationen für Alpha 14 sind nach Nutzer-Commit und Push beauftragt; noch kein neues Paket gebaut.
