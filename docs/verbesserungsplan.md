# Stand und weitere Entwicklung

Entwicklungsstand: **0.1.1-alpha.8 · 04.11.2026**, mit 150 WiSo-Aufgaben und insgesamt 220 Übungen. Zuletzt gebautes Windows-Paket: **0.1.1-alpha.7**, weiterhin 70 Übungen ohne WiSo. Kein neues Paket für dieses Änderungspaket erstellt. Detaillierte Historie: [Entwicklungsprotokoll](entwicklungsprotokoll.md). Nachweise: [Testprotokoll](testprotokoll.md).

## Umgesetzte Rückmeldungen

| Bereich | Ergebnis |
| --- | --- |
| SQL-Schreibweise | Große Schlüsselwörter/Funktionen und Semikolon je äußerer Anweisung; konkrete Korrekturhinweise |
| Lernmodus | Ohne Hilfe als Standard, Auswahl je Aufgabe gespeichert, Rückfrage nur bei vorhandenem Entwurf |
| Übersichtlichkeit | Weniger Erklärungstext, einklappbare Aufgabenliste, ausblendbarer Bedienhinweis |
| Navigation | Einheitlich Aufgabe statt Mission, unnötige Sprunglinks entfernt |
| Modulnavigation | Übersicht und Lernplan zuerst; anschließend SQL-, Algorithmus- oder WiSo-Links passend zum besuchten Bereich |
| Pseudocode-Einstieg | Acht Aufgaben mit eigener Story, vier Aufgabenarten, lokalem Interpreter, Prüffällen und Ablaufspur |
| Pseudocode-Ausbau | Zunächst 16, anschließend sechs weitere Aufgaben; Kennzahlen, lineare/binäre Suche, Sortierung, verschachtelte Listen und Funktionen; insgesamt 30 Algorithmus-Aufgaben und zusammen mit SQL 70 Übungen; mit WiSo insgesamt 220 |
| SQL-Spickzettel-Rückweg | Einheitlicher Button zur konkreten Aufgabe aus Ermittlung oder Werkstatt; Ziel bleibt beim Neuladen erhalten |
| Funktionsbewertung | Benannte Funktion mit Argumenten und Rückgabevertrag; eigene lokale Bereiche, Wertkopien und Bereichsanzeige in der Vorschau |
| Pseudocode-Feinschliff | Technischen Zusatz auf der Themenkarte entfernt; Zuweisungen in Beispielen, Hinweisen und Lösungen mit `=`, Gleichheitsvergleiche bevorzugt mit `==` |
| Pseudocode-Editor | Nummerierte Zeilen einschließlich Leerzeilen, synchrones Scrollen, Tab-/Blockeinrückung und Ausrücken; Esc vor Tab für Tastaturnavigation |
| Pseudocode-Schreibweise | Große Schlüsselwörter, Operatorwörter, Wahrheitswerte und Längenfunktion verpflichtend; konkrete Korrektur mit Zeilennummer, Texte/Kommentare und Variablenschreibung unberührt |
| Android im Heimnetz | Chrome-Zugriff nach Android-Berechtigung unter „Geräte in der Nähe“ vom Nutzer bestätigt; Anleitung ergänzt, keine Einzel-IP-Freigaben nötig |
| Scrollposition | Liste bleibt bei asynchronen Aufgabenwechseln innerhalb beider SQL-Lernwege stehen |
| Geführte Hilfen | Einheitliche Zeilen mit Beschreibung links und Aktion rechts; Spickzettel im selben Tab, Musterlösung mit Ansehen/Ausblenden |
| Lernplan | Profilbezogene nächste Schritte statt fester Wochen |
| Geschichte | Verbundene Storyschritte und chronologische Fallnotizen mit offenen Lücken |
| SQL-Beispiele | Kurze Tabellennamen und Datum/Uhrzeit als YYYY-MM-DD HH:mm:ss |
| Verbindungsfehler | Deutsche Dialoge mit lesbaren Hell-/Dunkelfarben und ausdrücklichem Neuladen |
| Darstellung | Helle blaue und dunkle grau-blaue Palette, Systemwahl und gespeicherte Präferenz |
| Einrichtung | Versionsanzeige, lokale SQL-Instanzauswahl und öffentliche GitHub-Updateprüfung |
| Projektpflege | Kurze README, eigene Entwickleranleitung, Modulordner und passende Namespaces |
| Lizenz | Ausschließlich private Nutzung; keine pauschale MIT-Freigabe, Drittanbieterlizenzen bleiben eigenständig |
| WiSo-Lernbereich | 150 eigene Fragen, acht Themen und sieben Antwortformen; nachvollziehbare Erklärungen, Merksätze, eigene Abbildungen, Quellen und Spickzettel mit Rückweg |
| WiSo-Runden | Themenlernen, gemischte Zehnerrunden, Fehler-/Unsicherheitswiederholung sowie 30 Aufgaben mit 60 Minuten und Auswertung nach Abgabe |
| WiSo-Profil | Fortsetzbare Antworten, Kontrollmarkierungen, Pausen, getrennte Fortschritte und letzte Ergebnisse; Revision schützt vor veralteten Fenstern |
| WiSo-Themenposition | Ursprüngliche Übersicht von 01 bis 08 wiederhergestellt; WiSo bleibt letzter Themenbereich |
| WiSo-Quellen | Quellentitel ohne wiederholte externe Tab-Zusätze; ein gemeinsamer Hinweis im Spickzettel beziehungsweise Quellenbereich |
| WiSo-Abgabe | Bestätigung wird nach dem Rendern fokussiert und sichtbar; anschließend auch die neue Auswertung |
| WiSo-Bedienhilfe | Erklärung zu Unsicherheit und Kontrollmarkierung oben im Spickzettel; keine zusätzliche dauerhaft eingeblendete Hilfe in den Aufgaben |

Die elf ursprünglichen SQL-Anpassungen sind abgeschlossen. Die nachfolgenden Wünsche zur Bedienung, Einrichtung, Dokumentation und Codeordnung sind ebenfalls umgesetzt. Eine Bewertung anhand zweier Datenbestände bleibt eine begrenzte fachliche Prüfung, keine allgemeine SQL-Beweisführung.

## Projektauftritt und Unterstützung – umgesetzt

Nach der positiven WiSo-Rückmeldung hat der Nutzer die Vorlage und zusätzlich den Issues-Verweis als fünften Punkt freigegeben. Alle fünf Punkte sind im Quellstand umgesetzt und mit eigenen Profilen geprüft:

1. **Freiwillige Unterstützung:** dezente Karte am Ende der Übersicht und auf der Projektseite sowie ein Fußzeilenlink. Freigegebener öffentlicher PayPal-Link: https://www.paypal.com/paypalme/b0rnkillafpv. Zahlung extern, Betrag frei wählbar.
2. **Projekt-/Urheberseite:** `/ueber-das-lernwerk` nennt Christopher Weismantel und b0rnkilla, beschreibt die Entstehung und verweist auf Nutzungsbedingungen und Drittanbieterhinweise. Das spätere Original-Logo kann die jetzige Wortmarke ergänzen.
3. **GitHub-README:** kurzer Download-Einstieg, vier neue eigene Browserbilder, kompakte Lernbereichsgalerie, drei Startschritte und Projektaktionen. Die README beschreibt auf Nutzerwunsch die aktuelle App-Fassung; Vorschau-/Paketvergleich, FIAE-Präfix im Leitsatz und der Designzusatz in der Downloadzeile sind entfernt. Technische Dokumente halten den tatsächlichen Paketstand weiter fest.
4. **GitHub-Anerkennung:** „Auf GitHub unterstützen“ mit Erklärung des einzelnen Star-Schalters. Keine erfundene Fünf-Sterne-Bewertung und keine automatische Sternvergabe.
5. **Fehler und Vorschläge:** eigener Link zur GitHub-Issues-Übersicht auf Übersicht, Projektseite und in der Fußzeile. Die Projektseite erklärt hilfreiche Angaben und die Öffentlichkeit der Meldungen; keine automatischen Uploads oder Meldungen.

Weitere Details: [Projektauftritt](projektauftritt.md). Kein neues Windows-Paket erstellt. Weitere Release-Erstellung weiterhin nur auf ausdrücklichen Auftrag; als optionale Ergänzung fehlt lediglich das später vom Nutzer gelieferte Logo.

## Weitere Lerninhalte

1. Zusätzliche SQL-Transferaufgaben und weitere Datenbankthemen nach Bedarf.
2. Pseudocode und Algorithmen: abgeschlossener Basisumfang von 30 Aufgaben. Die sechs Ergänzungen behandeln Prüfziffern, konkreten Schleifenaufwand, weitere Grenztests, rekursiven Rückweg und Fortschritt sowie eindeutige Positionspaare. Aus der bisherigen Aufgabenplanung bleibt nichts offen; keine vollständige Katalogabdeckung zugesagt.
3. WiSo: vereinbarter Umfang mit 150 Aufgaben umgesetzt; Quellenstand, Eingabeformen und eigene Punktevergabe dokumentiert. Keine vollständige Katalogabdeckung zugesagt. Inhaltserweiterungen bei konkretem Bedarf.
4. UML: Diagramme verstehen und Fehler finden, anschließend begrenzte Modellierungsaufgaben.
5. Weitere AP2-Bereiche sowie systematische Zuordnung zum aktuellen Prüfungskatalog.

## Spätere Veröffentlichung

Alpha 7 wurde auf ausdrücklichen Wunsch aus der damaligen, noch uncommitteten Arbeitsfassung ohne WiSo gebaut und aus dem ZIP geprüft. Benutzeranleitung, Versionshinweise, aktuelle Projektlizenz und Drittanbieterhinweise liegen bei. EXE-Start, Pseudocode ohne SQL, Funktionen/Rekursion, SELECT, Werkstattänderung und Übernahme eines früheren isolierten Alpha-4-Testprofils sind geprüft; keine privaten Dateien im Paket. ZIP und SHA256-Datei liegen lokal vor. Vor dem Tag aktuelle Änderungen committen und pushen; der alte Basiscommit ist kein passendes Release-Ziel. Der Nutzer übernimmt die Veröffentlichung; es gibt keinen automatischen Upload.

Die früheren Alpha-3- und Alpha-4-Pakete bleiben unverändert. Die README führt Anwender zu Releases → Windows-ZIP → Entpacken → EXE. Das alte 0.1.0-Paket wird nicht zur Verteilung empfohlen. Ein Test auf einem weiteren frisch eingerichteten Windows-Rechner wurde hier nicht durchgeführt. Weitere Pakete bleiben ausdrücklich zu beauftragen beziehungsweise dem vereinbarten Abschluss vorbehalten.

Login, Registrierung, Benutzertabellen und öffentlicher Homeserver-/nginx-Betrieb sind gestrichen. Die Algorithmus-Bewertung benötigt keine KI-Anbindung. Weitere KI-Funktionen würden eine gesonderte Entscheidung voraussetzen.


## Nächster Schritt nach README-Freigabe

Die README-Korrektur wurde am 04.11.2026 vom Nutzer freigegeben. Der Committext für den vollständigen WiSo-/Projektauftritt-Stand wird jetzt geliefert. Versionshinweise für Alpha 8 liegen vor dem Commit bereit. Anschließend folgt nach dem Nutzer-Commit/Push die neue ZIP-Erstellung mit Prüfung und vollständigen Release-Angaben. Noch kein Paket gebaut oder veröffentlicht.
