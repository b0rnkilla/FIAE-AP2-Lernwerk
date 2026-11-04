# Stand und weitere Entwicklung

Entwicklungsstand und zuletzt gebautes Windows-Paket: **0.1.1-alpha.7 · 04.11.2026**. Nummerierter Editor und alle 30 Algorithmus-Aufgaben sind im aus dem ZIP geprüften Paket enthalten. Detaillierte Historie: [Entwicklungsprotokoll](entwicklungsprotokoll.md). Nachweise: [Testprotokoll](testprotokoll.md).

## Umgesetzte Rückmeldungen

| Bereich | Ergebnis |
| --- | --- |
| SQL-Schreibweise | Große Schlüsselwörter/Funktionen und Semikolon je äußerer Anweisung; konkrete Korrekturhinweise |
| Lernmodus | Ohne Hilfe als Standard, Auswahl je Aufgabe gespeichert, Rückfrage nur bei vorhandenem Entwurf |
| Übersichtlichkeit | Weniger Erklärungstext, einklappbare Aufgabenliste, ausblendbarer Bedienhinweis |
| Navigation | Einheitlich Aufgabe statt Mission, unnötige Sprunglinks entfernt |
| Modulnavigation | Übersicht und Lernplan zuerst; anschließend SQL- oder Algorithmus-Links passend zum besuchten Bereich |
| Pseudocode-Einstieg | Acht Aufgaben mit eigener Story, vier Aufgabenarten, lokalem Interpreter, Prüffällen und Ablaufspur |
| Pseudocode-Ausbau | Zunächst 16, anschließend sechs weitere Aufgaben; Kennzahlen, lineare/binäre Suche, Sortierung, verschachtelte Listen und Funktionen; insgesamt 30 Aufgaben und 70 Übungen über alle Lernwege |
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

Die elf ursprünglichen SQL-Anpassungen sind abgeschlossen. Die nachfolgenden Wünsche zur Bedienung, Einrichtung, Dokumentation und Codeordnung sind ebenfalls umgesetzt. Eine Bewertung anhand zweier Datenbestände bleibt eine begrenzte fachliche Prüfung, keine allgemeine SQL-Beweisführung.

## Weitere Lerninhalte

1. Zusätzliche SQL-Transferaufgaben und weitere Datenbankthemen nach Bedarf.
2. Pseudocode und Algorithmen: abgeschlossener Basisumfang von 30 Aufgaben. Die sechs Ergänzungen behandeln Prüfziffern, konkreten Schleifenaufwand, weitere Grenztests, rekursiven Rückweg und Fortschritt sowie eindeutige Positionspaare. Aus der bisherigen Aufgabenplanung bleibt nichts offen; keine vollständige Katalogabdeckung zugesagt.
3. UML: Diagramme verstehen und Fehler finden, anschließend begrenzte Modellierungsaufgaben.
4. Weitere AP2-Bereiche sowie systematische Zuordnung zum aktuellen Prüfungskatalog.

## Spätere Veröffentlichung

Alpha 7 wurde auf ausdrücklichen Wunsch aus der aktuellen, noch uncommitteten Arbeitsfassung gebaut und aus dem ZIP geprüft. Benutzeranleitung, Versionshinweise, aktuelle Projektlizenz und Drittanbieterhinweise liegen bei. EXE-Start, Pseudocode ohne SQL, Funktionen/Rekursion, SELECT, Werkstattänderung und Übernahme eines früheren isolierten Alpha-4-Testprofils sind geprüft; keine privaten Dateien im Paket. ZIP und SHA256-Datei liegen lokal vor. Vor dem Tag aktuelle Änderungen committen und pushen; der alte Basiscommit ist kein passendes Release-Ziel. Der Nutzer übernimmt die Veröffentlichung; es gibt keinen automatischen Upload.

Die früheren Alpha-3- und Alpha-4-Pakete bleiben unverändert. Die README führt Anwender zu Releases → Windows-ZIP → Entpacken → EXE. Das alte 0.1.0-Paket wird nicht zur Verteilung empfohlen. Ein Test auf einem weiteren frisch eingerichteten Windows-Rechner wurde hier nicht durchgeführt. Weitere Pakete bleiben ausdrücklich zu beauftragen beziehungsweise dem vereinbarten Abschluss vorbehalten.

Login, Registrierung, Benutzertabellen und öffentlicher Homeserver-/nginx-Betrieb sind gestrichen. Die Algorithmus-Bewertung benötigt keine KI-Anbindung. Weitere KI-Funktionen würden eine gesonderte Entscheidung voraussetzen.
