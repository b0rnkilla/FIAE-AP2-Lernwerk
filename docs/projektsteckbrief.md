# Projektsteckbrief

| Merkmal | Stand |
| --- | --- |
| Anwendung | AP2 Lernwerk |
| Projektmappe | FIAE_AP2_Trainer.sln |
| Entwicklungsfassung | 0.1.1-alpha.7 · 04.11.2026 |
| Zuletzt gebautes Windows-Paket | 0.1.1-alpha.7, aus dem ZIP geprüft |
| Plattform | Windows, lokale Browseroberfläche |
| Technik | C#, .NET 10, ASP.NET Core, Blazor, Microsoft SQL Server |
| Verfügbar im Quellstand | 30 SQL-Ermittlungsaufgaben, zehn SQL-Werkstattaufgaben und 30 Algorithmus-Aufgaben |
| Inhalt Windows-Paket Alpha 7 | Beide SQL-Lernwege und alle 30 Algorithmus-Aufgaben, insgesamt 70 Übungen |
| Nutzungsmodell | Private, nichtkommerzielle lokale Nutzung gemäß LICENSE |

## Ziel

Die App unterstützt die Vorbereitung auf die FIAE-Abschlussprüfung Teil 2 durch selbst formulierte Lösungen, nachvollziehbare Rückmeldungen und freiwillige Hilfen. SQL und der erste Algorithmus-Lernweg bilden die verfügbaren Module einer später breiteren Lernanwendung. Der Code soll verständlich bleiben und vom Projektinhaber weiterentwickelt werden können.

Die Aufgaben, Geschichte und Daten sind eigens für die App erstellt. SQL Murder Mystery dient als konzeptionelle Inspiration; dessen Inhalte wurden nicht übernommen. Das Lernwerk ist kein offizielles IHK-Angebot, keine vollständige Abdeckung des Prüfungskatalogs und keine Zusage eines Prüfungserfolgs.

## Vorhandener Umfang

- **Ermittlung:** SELECT, Filter, Sortierung, NULL, JOINs, Aggregation, GROUP BY/HAVING, Unterabfragen sowie EXISTS/NOT EXISTS.
- **Werkstatt:** UPDATE/INSERT/DELETE, CREATE TABLE, ALTER TABLE ADD, PRIMARY KEY/NOT NULL/CHECK und einfache Transaktionen.
- **Algorithmen:** eigener Lernweg „Leitstelle im Ausnahmezustand“ mit 30 Aufgaben: freie Lösungen, Reparaturen, Schreibtischtests und Testfälle; Variablen, Bedingungen, Schleifen, Kennzahlen, lineare/binäre Suche, Sortierung, verschachtelte Listen, Funktionen, Prüfziffern, Grenztests und Rekursion. Lokale Ausführung mit Prüffällen und freiwilliger Ablaufspur, unabhängig von SQL Server. Funktionsaufgaben prüfen den benannten Einstieg mit Parametern und Rückgabewerten; die Vorschau zeigt den lokalen Bereich.
- **Pseudocode-Schreibweise:** sichtbare Zuweisungen mit `=`, Gleichheitsvergleiche bevorzugt mit `==`; bisherige alternative Zuweisungen bleiben kompatibel. Die Themenkarte zeigt nur Geschichte und Aufgabenzahl.
- **Lernablauf:** Moduswahl je Aufgabe, gestufte Hilfen, Musterlösungen, zusammenhängende Fallnotizen und Wiederholungsvorschläge.
- **Bedienung:** vollständig deutsch, helles und dunkles Design, einklappbare Aufgabenliste und ausblendbare Bedienhinweise.
- **Pseudocode-Editor:** Zeilennummern, synchrones Scrollen, Tab/Umschalt+Tab für Einrückungen, Esc vor Tab zum Verlassen des Felds. Vorschau und Bewertung verlangen große Schlüsselwörter und nennen die konkrete Korrektur; Variablen und Texte bleiben erhalten. Diese Ergänzungen und alle 30 Aufgaben sind im Alpha-7-Paket enthalten.
- **Kopfnavigation:** Übersicht und Lernplan stehen immer vorn; fachliche Links wechseln mit dem besuchten Modul. Der Modulkontext bleibt innerhalb der Browsersitzung auf allgemeinen Seiten erhalten.
- **Betrieb:** lokales Profil, persönliche SQL-Einrichtung, Versionsanzeige und abschaltbare GitHub-Updateprüfung.

Die Verbindungs- und Sitzungsdialoge sind deutsch und folgen dem gewählten hellen oder dunklen Design.

In der aktuellen Entwicklungsfassung bleibt die Scrollposition der Aufgabenliste bei Wechseln innerhalb eines SQL-Lernwegs erhalten. Im geführten Lernen sind Hinweise, Spickzettel und Musterlösung gleich aufgebaut: Titel und Beschreibung links, Aktion rechts. Der SQL-Spickzettel öffnet im selben Tab; der Entwurf bleibt beim Zurückkehren erhalten. Die Musterlösung besitzt die Aktionen „Ansehen“ und „Ausblenden“; ihre Hilfenutzung bleibt auch nach dem Schließen gespeichert.

## Architektur

`FiaeTrainer.Core` enthält die Fachlogik nach Lernmodulen. Das SQL-Modul gliedert sich in Ermittlung, Werkstatt, Prüfung, Ergebnisse, Datenbankzugriff und eingebettete Inhalte. `Algorithms` enthält eigene Inhalte, Parser, Interpreter und Bewertung. `FiaeTrainer.Web` enthält Oberfläche, Fortschritt, Einrichtung, Netzwerkregeln und Updates in getrennten Ordnern und Namespaces. Ein eigener Testbereich prüft diese Funktionen.

Neue fachliche Module erhalten eigene Unterordner neben `Core/Sql`. Gemeinsame Bedienung und Infrastruktur bleiben davon getrennt. Die konkrete Zuordnung und Einstiegspunkte stehen in [Entwicklung](entwicklung.md); Konventionen in der [Coding Guideline](coding-guideline.md).

Die Anwendung verwendet ausschließlich die gekennzeichnete Datenbank `FIAE_AP2_Training` mit synthetischen Daten. Lernabfragen laufen unter eingeschränkten SQL-Rechten; schreibende Werkstattversuche verwenden sitzungseigene Arbeitskopien. Die Bewertung vergleicht zwei Datenbestände und beweist keine allgemeine SQL-Gleichwertigkeit.

## Veröffentlichung und Lizenz

Verteilt werden lokale Windows-x64-Pakete über den Bereich Releases des [offiziellen Repositorys](https://github.com/b0rnkilla/FIAE-AP2-Lernwerk/releases). Die Alpha 0.1.1-alpha.7 ist auf ausdrücklichen Wunsch aus der aktuellen Arbeitsfassung gebaut und aus dem ZIP geprüft. Die Änderungen waren beim Bau noch nicht committed; für den Release-Tag müssen sie zuvor versioniert und gepusht werden. Es wurde nichts automatisch veröffentlicht. Die README beschreibt ausschließlich den ZIP-/EXE-Einstieg für Anwender; SQL Server bleibt für die SQL-Lernwege eine separate Voraussetzung. Weitere Pakete entstehen auf ausdrücklichen Wunsch beziehungsweise zum vereinbarten Abschluss. Das alte Paket 0.1.0 ist kein aktuelles Verteilungspaket.

Für die neue Fassung gelten eigene Bedingungen zur privaten Nutzung. Kommerzielle Nutzung, Programmänderungen und Weiterverbreitung werden nicht eingeräumt. Eine pauschale MIT-Freigabe wird nicht eingeräumt. Zwingende gesetzliche Rechte und Lizenzen der Abhängigkeiten bleiben unberührt. Einzelheiten: [LICENSE](../LICENSE) und [Lizenzwechsel](lizenz.md).

## Weitere Lernmodule

Der Pseudocode-Basisumfang ist mit 30 Aufgaben abgeschlossen. Die sechs Ergänzungen zur ursprünglichen 24er-Planung trainieren Prüfziffern, Aufwand, Grenztests, Rekursion und paarweise Vergleiche. Weitere Aufgaben sind aus dieser Planung nicht offen; der Lernweg behauptet keine vollständige Abdeckung aller Algorithmusthemen. Für UML sind zunächst Diagrammverständnis und begrenzte Modellierungsaufgaben vorgesehen. Datenbankentwurf, Softwareplanung/OOP, Tests, Sicherheit, Projektmanagement und WiSo sind weitere mögliche Module. Sie sind noch nicht implementiert. Details: [Pseudocode](pseudocode.md).

## Abgrenzung

Onlinekonten, Registrierung und öffentlicher Serverbetrieb sind nicht geplant. Eine bestehende private Heimnetz-Vorführung ist optional und teilt einen Lernstand. Es gibt keine KI-Anbindung. Eine ausführbare Fremdschlüsselübung und weitere SQL-Themen fehlen noch.

Der Heimnetzzugriff wurde vom Nutzer auch mit einem Samsung-Handy bestätigt. Chrome benötigte dafür seine Android-Berechtigung für lokale Netzwerkzugriffe unter „Geräte in der Nähe“. Die einmalige Windows-Subnetzfreigabe blieb unverändert; Einzelheiten stehen in der [Heimnetz-Anleitung](heimnetz-und-serverbetrieb.md).

Für die Entwicklung in Visual Studio ist wegen .NET 10 Visual Studio 2026 erforderlich; alternativ genügt das passende SDK mit einem anderen Editor. Prüfungen und bekannte Grenzen dokumentiert das [Testprotokoll](testprotokoll.md).

## Fachliche Orientierung

- [FIAusbV § 13](https://www.gesetze-im-internet.de/fiausbv/__13.html) und [§ 14](https://www.gesetze-im-internet.de/fiausbv/__14.html).
- [IHK Hannover: Prüfungskataloge ab 2025](https://www.ihk.de/hannover/hauptnavigation/ausbildung-und-weiterbildung/ausbildung/ausbildung-a-z/neuordnungen/pruefungskataloge-it-berufe-6438900).
- [SQL Murder Mystery](https://github.com/NUKnightLab/sql-mysteries) als konzeptionelle Inspiration.

Eine vollständige Zuordnung des aktuellen Prüfungskatalogs zu allen geplanten Modulen steht noch aus.
