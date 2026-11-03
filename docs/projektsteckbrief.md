# Projektsteckbrief

| Merkmal | Stand |
| --- | --- |
| Anwendung | AP2 Lernwerk |
| Projektmappe | FIAE_AP2_Trainer.sln |
| Version | 0.1.1-alpha.3 · 03.11.2026 |
| Plattform | Windows, lokale Browseroberfläche |
| Technik | C#, .NET 10, ASP.NET Core, Blazor, Microsoft SQL Server |
| Verfügbar | 30 SQL-Ermittlungsaufgaben und zehn SQL-Werkstattaufgaben |
| Nutzungsmodell | Private, nichtkommerzielle lokale Nutzung gemäß LICENSE |

## Ziel

Die App unterstützt die Vorbereitung auf die FIAE-Abschlussprüfung Teil 2 durch selbst formulierte Lösungen, nachvollziehbare Rückmeldungen und freiwillige Hilfen. SQL ist das erste Modul einer später breiteren Lernanwendung. Der Code soll verständlich bleiben und vom Projektinhaber weiterentwickelt werden können.

Die Aufgaben, Geschichte und Daten sind eigens für die App erstellt. SQL Murder Mystery dient als konzeptionelle Inspiration; dessen Inhalte wurden nicht übernommen. Das Lernwerk ist kein offizielles IHK-Angebot, keine vollständige Abdeckung des Prüfungskatalogs und keine Zusage eines Prüfungserfolgs.

## Vorhandener Umfang

- **Ermittlung:** SELECT, Filter, Sortierung, NULL, JOINs, Aggregation, GROUP BY/HAVING, Unterabfragen sowie EXISTS/NOT EXISTS.
- **Werkstatt:** UPDATE/INSERT/DELETE, CREATE TABLE, ALTER TABLE ADD, PRIMARY KEY/NOT NULL/CHECK und einfache Transaktionen.
- **Lernablauf:** Moduswahl je Aufgabe, gestufte Hilfen, Musterlösungen, zusammenhängende Fallnotizen und Wiederholungsvorschläge.
- **Bedienung:** vollständig deutsch, helles und dunkles Design, einklappbare Aufgabenliste und ausblendbare Bedienhinweise.
- **Betrieb:** lokales Profil, persönliche SQL-Einrichtung, Versionsanzeige und abschaltbare GitHub-Updateprüfung.

Die Verbindungs- und Sitzungsdialoge sind deutsch und folgen dem gewählten hellen oder dunklen Design.

## Architektur

`FiaeTrainer.Core` enthält die Fachlogik nach Lernmodulen. Das SQL-Modul gliedert sich in Ermittlung, Werkstatt, Prüfung, Ergebnisse, Datenbankzugriff und eingebettete Inhalte. `FiaeTrainer.Web` enthält Oberfläche, Fortschritt, Einrichtung, Netzwerkregeln und Updates in getrennten Ordnern und Namespaces. Ein eigener Testbereich prüft diese Funktionen.

Neue fachliche Module erhalten eigene Unterordner neben `Core/Sql`. Gemeinsame Bedienung und Infrastruktur bleiben davon getrennt. Die konkrete Zuordnung und Einstiegspunkte stehen in [Entwicklung](entwicklung.md); Konventionen in der [Coding Guideline](coding-guideline.md).

Die Anwendung verwendet ausschließlich die gekennzeichnete Datenbank `FIAE_AP2_Training` mit synthetischen Daten. Lernabfragen laufen unter eingeschränkten SQL-Rechten; schreibende Werkstattversuche verwenden sitzungseigene Arbeitskopien. Die Bewertung vergleicht zwei Datenbestände und beweist keine allgemeine SQL-Gleichwertigkeit.

## Veröffentlichung und Lizenz

Verteilt werden lokale Windows-x64-Pakete über den Bereich Releases des [offiziellen Repositorys](https://github.com/b0rnkilla/FIAE-AP2-Lernwerk/releases). Der Nutzer bereitet dessen öffentliche Freigabe vor; es wurde nichts automatisch veröffentlicht. Die neue Alpha 0.1.1-alpha.3 ist ausdrücklich beauftragt, mit aktueller privater Lizenz gebaut und aus dem ZIP geprüft. Das alte Paket 0.1.0 ist kein aktuelles Verteilungspaket. Die README beschreibt ausschließlich den ZIP-/EXE-Einstieg für Anwender; SQL Server bleibt eine separate Voraussetzung. Weitere Pakete entstehen auf ausdrücklichen Wunsch beziehungsweise zum vereinbarten Abschluss.

Für die neue Fassung gelten eigene Bedingungen zur privaten Nutzung. Kommerzielle Nutzung, Programmänderungen und Weiterverbreitung werden nicht eingeräumt. Eine pauschale MIT-Freigabe wird nicht eingeräumt. Zwingende gesetzliche Rechte und Lizenzen der Abhängigkeiten bleiben unberührt. Einzelheiten: [LICENSE](../LICENSE) und [Lizenzwechsel](lizenz.md).

## Weitere Lernmodule

Pseudocode und Algorithmen sollen mit deutschen Konventionen, Fehlersuche, Schreibtischtests und freien Aufgaben folgen. Für UML sind zunächst Diagrammverständnis und begrenzte Modellierungsaufgaben vorgesehen. Datenbankentwurf, Softwareplanung/OOP, Tests, Sicherheit, Projektmanagement und WiSo sind weitere mögliche Module. Sie sind noch nicht implementiert.

## Abgrenzung

Onlinekonten, Registrierung und öffentlicher Serverbetrieb sind nicht geplant. Eine bestehende private Heimnetz-Vorführung ist optional und teilt einen Lernstand. Es gibt keine KI-Anbindung. Eine ausführbare Fremdschlüsselübung und weitere SQL-Themen fehlen noch.

Für die Entwicklung in Visual Studio ist wegen .NET 10 Visual Studio 2026 erforderlich; alternativ genügt das passende SDK mit einem anderen Editor. Prüfungen und bekannte Grenzen dokumentiert das [Testprotokoll](testprotokoll.md).

## Fachliche Orientierung

- [FIAusbV § 13](https://www.gesetze-im-internet.de/fiausbv/__13.html) und [§ 14](https://www.gesetze-im-internet.de/fiausbv/__14.html).
- [IHK Hannover: Prüfungskataloge ab 2025](https://www.ihk.de/hannover/hauptnavigation/ausbildung-und-weiterbildung/ausbildung/ausbildung-a-z/neuordnungen/pruefungskataloge-it-berufe-6438900).
- [SQL Murder Mystery](https://github.com/NUKnightLab/sql-mysteries) als konzeptionelle Inspiration.

Eine vollständige Zuordnung des aktuellen Prüfungskatalogs zu allen geplanten Modulen steht noch aus.
