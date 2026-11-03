# SQL-Verbesserungen und lokale Veröffentlichung

Stand: 03.11.2026. Die elf Punkte aus dem ausführlichen Nutzertest sind umgesetzt. Nach der akzeptierten aufgeräumten Ansicht wurde der Randpfeil verkleinert; auf Wunsch des Nutzers wurden auch die restlichen Pakete fortgeführt. Der Nutzer hat die Gesamtfassung positiv erprobt. Homeserver, Login, Registrierung und Benutzertabellen sind nach seiner neuen Entscheidung gestrichen; die Anwendung wird als lokale Windows-EXE über GitHub-Releases verteilt.

## Übersicht

| Nr. | Wunsch | Paket und Stand |
| --- | --- | --- |
| 1 | Semikolon am Ende verlangen | B · umgesetzt |
| 2 | SQL-Schlüsselwörter in Großbuchstaben verlangen | B · umgesetzt |
| 3 | Modus je Aufgabe merken; neue Aufgaben ohne Hilfe | A · umgesetzt |
| 4 | Bei leerem Editor ohne Neustartdialog wechseln | A · umgesetzt |
| 5 | Sprunglinks zwischen Tabellen und Editor entfernen | A · umgesetzt |
| 6 | Selbsttest-Bedienhinweis aus- und wieder einblenden | A · umgesetzt; Wiederanzeige ausschließlich im Lernplan |
| 7 | Lernplan ohne feste Wochenaufteilung | C · umgesetzt |
| 8 | Navigation als vorherige/nächste Aufgabe beschriften | A · umgesetzt |
| 9 | Zusammenhängende Geschichte und Fallnotizen | D · umgesetzt |
| 10 | Sichtbare Beispiele und Hinweise ohne fall-Präfix | B · umgesetzt |
| 11 | Datums-/Zeitbeispiele als YYYY-MM-DD HH:mm:ss | B · umgesetzt |

## Paket A: Bedienung und gespeicherte Auswahl

* ExamMode wird je Aufgabe getrennt für Ermittlung und Werkstatt gespeichert. Neue Aufgaben starten ohne Hilfe; ältere Dateien ohne Moduswahl verwenden denselben Standard. Leistungen bleiben unabhängig vom später gewählten Modus erhalten.
* Ein leerer Editor startet den Selbsttest unmittelbar. Vorhandene Entwürfe werden erst nach Bestätigung geleert. Unterstützte oder übernommene Entwürfe zählen nicht durch bloßes Neuladen als selbstständiger Versuch.
* Das X blendet die allgemeine Selbsttest-Hilfe profilweit aus. Ausschließlich Lernplan → Bedienhinweise stellt sie wieder her. Eigentliche Aufgabenhilfen bleiben davon unabhängig.
* Sprunglinks zwischen Tabellen und Editor, generische Moduserklärungen, Speicherhinweise und Dein Lernraum entfallen. Navigation verwendet Aufgabe. Im Mittelpunkt stehen Modus, Story, Auftrag und Editor; fachliche Erklärungen im geführten Lernen stehen darunter.
* Die Aufgabenliste ist einklappbar und merkt ihre Anzeige. Der zuvor große Textschalter wurde durch einen 28 Pixel großen Pfeil an der Trennkante ersetzt, mit Titel, zugänglichem Namen und eindeutiger Auf-/Zuklappsteuerung. Mobil sitzt er rechts am Listenrand.

## Paket B: SQL-Schreibregeln und Beispiele

* Semikolon je vollständiger äußerer Anweisung, auch bei mehrteiligen Werkstattskripten und Transaktionen. Text- oder Kommentar-Semikolons genügen nicht; Unterabfragen benötigen keinen eigenen Abschluss.
* Schlüsselwörter werden anhand echter Parser-Tokens geprüft, Funktionsnamen anhand ihrer Syntaxknoten. Namen, Aliase, maskierte Bezeichner, Textwerte und Kommentare bleiben unverändert. Fehlermeldungen nennen den betroffenen Befehl beziehungsweise die Anweisungsnummer.
* Referenzlösungen, Hinweise und Spickzettel verwenden kurze Tabellennamen. Die interne kontrollierte Zuordnung zu Fall- und Prüfbestand bleibt bestehen; ältere qualifizierte Eingaben bleiben kompatibel.
* Beispiele und Ergebnistabellen verwenden YYYY-MM-DD HH:mm:ss, einschließlich Mitternacht. Beide SQL-Ausführungswege setzen DATEFORMAT ymd. Ein Integrationstest mit deutscher Verbindungssprache ergänzt die Prüfung aller Lösungen auf beiden Beständen. Andere gültige Datumsschreibweisen bleiben zulässig.
* Bestehende Entwürfe werden nicht automatisch verändert. Großschreibung und Semikolon sind ausdrückliche Trainingsregeln; eine allgemein verbindliche IHK-Bewertungsregel dazu wird nicht behauptet.

## Paket C: Lernplan im eigenen Tempo

* Keine Wochen, festen Stunden oder Fristen. Fortschritt und selbstständige Erfolge beider Lernwege sind getrennt sichtbar.
* Nächste neue Aufgabe je Lernweg, begonnene ungelöste Aufgaben und gelöste Aufgaben ohne selbstständigen Erfolg erscheinen getrennt. Auch ein Entwurf ohne Abgabe zählt als begonnen.
* Sind alle Aufgaben begonnen, verweist der Plan auf offene Versuche; sind alle gelöst, empfiehlt er Wiederholung und unbekannte Papieraufgaben. Die Zahlen sind keine Bestehensprognose.

## Paket D: Verständliche Fallgeschichte

* Alle 30 Storytexte verbinden die vorherige Datenfrage mit dem aktuellen Auftrag und dessen Zweck für den Warenweg der Sensoren. Bestands- und Vergleichsaufgaben werden als Lagebericht beziehungsweise Gegenprobe eingeordnet.
* Übersprungene Aufgaben werden nicht als bereits nachgewiesene Fakten vorausgesetzt. Der Storybereich nennt die unmittelbar vorherige noch offene Spur und bietet einen direkten Einstieg dorthin.
* Fallnotizen zeigen bestätigte Erkenntnisse in Aufgabenreihenfolge mit verlinkten Titeln. Zwischen fehlenden Abschnitten erscheint eine Lücke statt einer vorweggenommenen Lösung. Verdachtsmomente, fehlende Daten und die abschließende kombinierte Datenfrage bleiben unterscheidbar.
* Die eigentlichen SQL-Aufträge und erwarteten Ergebnisse bleiben erhalten. Die Geschichte ist fiktiv; der Fund der Sensoren am Ende wird ausdrücklich als fiktiver Abschluss eingeordnet.

## Prüfung und nächster Schritt

230 .NET-Testfälle einschließlich ausdrücklich aktivierter SQL-Integrationstests im aktuellen Entwicklungsstand; die ursprünglichen SQL-Pakete bleiben erfolgreich geprüft. Browserprüfungen mit separatem Profil umfassen Schreibfehler, Korrekturen, Datumsfilter und -darstellung, Storysprünge, Fallnotizen, profilbezogenen Lernplan sowie den kleinen Pfeil in heller/dunkler und mobiler Ansicht. Einzelheiten stehen in docs/testprotokoll.md.

Die elf Korrekturen sind abgeschlossen. Die nächste konkrete Arbeit ist die lokale Release-Verteilung: eigenständige Windows-x64-EXE mit Browseroberfläche, stabilem Benutzerprofil, sauberem ZIP und Prüfsumme. SQL Server bleibt eine separate Voraussetzung. Markdown-Dokumentation wird bei jeder weiteren Änderung gepflegt; historische Protokolleinträge bleiben erhalten.

## Stand der lokalen Release-Vorbereitung

Windows-Publishprofil, Release-erstellen.ps1, benutzerspezifische Konfiguration und Startanleitung sind ergänzt. Private Einstellungen, Lernstände, QA-Dateien, Debugsymbole und Logs werden ausgeschlossen; Abhängigkeits- und Laufzeit-Lizenzhinweise werden beigefügt. Paket 0.1.0 ist lokal gebaut und aus dem entpackten ZIP geprüft. 179 .NET-Tests und sechs JavaScript-Prüfungen erfolgreich; MIT-Lizenz auf Nutzerwunsch ergänzt. Es wird nichts automatisch zu GitHub hochgeladen. Neue Lernmodule wie Pseudocode und UML bleiben eigenständige weitere Ausbauschritte.

## Laufende Entwicklung nach der frühen Momentaufnahme

Das Paket 0.1.0 war vorgezogen und bleibt als Early Alpha erhalten. Auf Nutzerwunsch keine weiteren Pakete bis zum eigentlichen Abschluss. Der aktuelle Quellstand 0.1.1-alpha.1 zeigt die Buildversion im Browser, prüft öffentliche GitHub-Releases mit separatem Alpha-/Stable-Verhalten und bietet eine lokale SQL-Einrichtung. Persönliche Instanznamen sind frei wählbar; nur die eigene Trainingsdatenbank ist fest. Lokale Instanzen werden als Hinweise erkannt, eine fehlgeschlagene Verbindung nicht als fehlende Installation ausgegeben. Microsoft-Download verlinkt. Private Konfiguration und Updatewahl werden atomar gespeichert. Die .gitignore ist erweitert und in einem separaten Repository geprüft. Die Live-GitHub-API meldete beim bisherigen Test 404; Updateauswahl und Hinweis sind mit simulierten Antworten getestet.
