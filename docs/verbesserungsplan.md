# Stand und weitere Entwicklung

Stand: **0.1.1-alpha.3 · 03.11.2026**. Detaillierte Historie: [Entwicklungsprotokoll](entwicklungsprotokoll.md). Nachweise: [Testprotokoll](testprotokoll.md).

## Umgesetzte Rückmeldungen

| Bereich | Ergebnis |
| --- | --- |
| SQL-Schreibweise | Große Schlüsselwörter/Funktionen und Semikolon je äußerer Anweisung; konkrete Korrekturhinweise |
| Lernmodus | Ohne Hilfe als Standard, Auswahl je Aufgabe gespeichert, Rückfrage nur bei vorhandenem Entwurf |
| Übersichtlichkeit | Weniger Erklärungstext, einklappbare Aufgabenliste, ausblendbarer Bedienhinweis |
| Navigation | Einheitlich Aufgabe statt Mission, unnötige Sprunglinks entfernt |
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
2. Pseudocode und Algorithmen: Fehlersuche, Schreibtischtests und freie Aufgaben mit festgelegten deutschen Konventionen.
3. UML: Diagramme verstehen und Fehler finden, anschließend begrenzte Modellierungsaufgaben.
4. Weitere AP2-Bereiche sowie systematische Zuordnung zum aktuellen Prüfungskatalog.

## Spätere Veröffentlichung

Lokale Windows-x64-Pakete über das offizielle GitHub-Repository bleiben das Ziel; das Repository ist derzeit privat. Das vorhandene Paket 0.1.0 bleibt eine nicht öffentlich verteilte frühe Testfassung und ist keine Vorlage für die aktuelle Lizenz. Weitere Pakete entstehen erst am Entwicklungsende oder nach ausdrücklicher Anweisung. Vorher stehen ein Test auf einem weiteren Windows-Rechner, passende Versionshinweise und eine Prüfung des vollständigen Paketumfangs aus. Es gibt keinen automatischen Upload.

Login, Registrierung, Benutzertabellen und öffentlicher Homeserver-/nginx-Betrieb sind gestrichen. KI-Bewertung ist nicht implementiert; sie setzt eine gesonderte Entscheidung zu Kosten, Datenübertragung und fachlicher Verlässlichkeit voraus.
