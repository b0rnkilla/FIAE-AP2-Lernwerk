# Projektsteckbrief

| Merkmal | Stand |
| --- | --- |
| Anwendung | AP2 Lernwerk |
| Projektmappe | FIAE_AP2_Trainer.sln |
| Entwicklungsfassung | 0.1.1-alpha.16 · 11.11.2026 · Abschnitt 07 Projekte und Wirtschaftlichkeit |
| Zuletzt gebautes Windows-Paket | 0.1.1-alpha.15, bestätigter gepushter Code-Commit 3c7339f, aus dem ZIP geprüft am 11.11.2026; kurze Versionshinweise nach dem Commit ergänzt, laut Nutzer veröffentlicht |
| Alpha-15-Freigabe | Nach Nutzer-Commit/Push ausdrücklich beauftragt, ZIP und native Prüfung erfolgreich; Tag v0.1.1-alpha.15 auf 3c7339f, Veröffentlichung durch Nutzer |
| Plattform | Windows, lokale Browseroberfläche |
| Technik | C#, .NET 10, ASP.NET Core, Blazor, Microsoft SQL Server |
| Verfügbar im Quellstand | 40 SQL-, 30 Algorithmus-, 36 UML-, 42 Datenbankentwurfs-, 48 Softwareplanungs-, 54 Qualitäts-, 54 Projekt- und 150 WiSo-Aufgaben: 454 Lernübungen in acht Modulen; zusätzlich 25 SQL-/Pseudocode-Prüfungsaufgaben sowie je drei Entwurfs-, UML-, Softwareplanungs-, Qualitäts- und Projektprüfungen mit je neun Teilen |
| Inhalt Windows-Paket Alpha 15 | 400 Lernübungen in sieben Modulen, 25 SQL-/Pseudocode-Prüfungsaufgaben und je drei Datenbankentwurfs-, UML-, Softwareplanungs- und Qualitätsprüfungsfälle |
| Historischer Inhalt Windows-Paket Alpha 14 | 346 Lernübungen in sechs Modulen, 25 SQL-/Pseudocode-Prüfungsaufgaben und jeweils drei Datenbankentwurfs-, UML- und Softwareplanungs-Prüfungsfälle |
| Historischer Inhalt Windows-Paket Alpha 13 | 298 Lernübungen in fünf Modulen, 25 SQL-/Pseudocode-Prüfungsaufgaben sowie je drei Datenbankentwurfs- und UML-Prüfungsfälle |
| Historischer Inhalt Windows-Paket Alpha 12 | 250 Lernübungen plus 25 SQL-/Pseudocode-Prüfungsaufgaben und drei Entwurfsfälle; Heimnetz in derselben EXE |
| Historischer Inhalt Windows-Paket Alpha 9 | 220 Lernübungen plus 25 getrennte SQL-/Pseudocode-Prüfungsaufgaben; WiSo-Feinschliff und Projektlogo |
| Nutzungsmodell | Private, nichtkommerzielle lokale Nutzung gemäß LICENSE |

## Ziel

Die App unterstützt die Vorbereitung auf die FIAE-Abschlussprüfung Teil 2 durch eigene Lösungen, nachvollziehbare Rückmeldungen und freiwillige Hilfen. SQL, Algorithmen, UML und Modellierung, Datenbankentwurf, Softwareplanung und OOP, Tests/Qualität/Sicherheit, Projekte/Wirtschaftlichkeit sowie Wirtschafts- und Sozialkunde sind die acht verfügbaren Module. Der Code soll verständlich bleiben und vom Projektinhaber weiterentwickelt werden können.

Die Aufgaben, Geschichte und Daten sind eigens für die App erstellt. SQL Murder Mystery dient als konzeptionelle Inspiration; dessen Inhalte wurden nicht übernommen. Das Lernwerk ist kein offizielles IHK-Angebot, keine vollständige Abdeckung des Prüfungskatalogs und keine Zusage eines Prüfungserfolgs.

## Vorhandener Umfang

- **Ermittlung:** SELECT, Filter, Sortierung, NULL, JOINs, Aggregation, GROUP BY/HAVING, Unterabfragen sowie EXISTS/NOT EXISTS.
- **Werkstatt:** UPDATE/INSERT/DELETE, CREATE TABLE, ALTER TABLE ADD, PRIMARY KEY/NOT NULL/CHECK und einfache Transaktionen.
- **Algorithmen:** eigener Lernweg „Leitstelle im Ausnahmezustand“ mit 30 Aufgaben: freie Lösungen, Reparaturen, Schreibtischtests und Testfälle; Variablen, Bedingungen, Schleifen, Kennzahlen, lineare/binäre Suche, Sortierung, verschachtelte Listen, Funktionen, Prüfziffern, Grenztests und Rekursion. Lokale Ausführung mit Prüffällen und freiwilliger Ablaufspur, unabhängig von SQL Server. Funktionsaufgaben prüfen den benannten Einstieg mit Parametern und Rückgabewerten; die Vorschau zeigt den lokalen Bereich.
- **Pseudocode-Schreibweise:** sichtbare Zuweisungen mit `=`, Gleichheitsvergleiche bevorzugt mit `==`; bisherige alternative Zuweisungen bleiben kompatibel. Die Themenkarte zeigt nur Geschichte und Aufgabenzahl.
- **UML und Modellierung (03):** 36 eigene Aufgaben zur Geschichte Kulturzentrum Nordhafen mit Mira König. Fünf Diagrammarten lesen, reparieren und selbst mit einem automatisch angeordneten Baukasten erstellen. Spickzettel, drei 45-Minuten-Prüfungsfälle, eigenes Profil; keine Lernpunkte. Details: [UML](uml.md).
- **Softwareplanung und OOP (05):** 48 eigene Aufgaben, eigene Klassen-/Architektur-/API-/Formularentwürfe, konkrete Zustands-/Rückgabetabellen und drei neue Prüfungsfälle. Details: [Softwareplanung](softwareplanung.md).
- **Tests, Qualität und Sicherheit (06):** 54 Aufgaben, eigene Testsätze gegen acht lokale Modelle, konkrete Abdeckung/Fehlervarianten, Review, Rechte-Matrix und Abläufe. Drei Prüfungsfälle, eigener Prüfbericht. Details: [Qualitätssicherung](qualitaet.md).
- **Projekte und Wirtschaftlichkeit (07):** 54 eigene Aufgaben, Struktur-/Netz-/Gantt-Baukästen, Rechentabellen und Nutzwertmatrizen, drei Prüfungsfälle mit je drei Teilen. Alternative gültige Terminpläne werden akzeptiert. Details: [Projekte](projekte.md).
- **Lernablauf:** Moduswahl je Aufgabe, gestufte Hilfen, Musterlösungen, zusammenhängende Fallnotizen und Wiederholungsvorschläge.
- **WiSo:** 150 eigene Aufgaben in acht Themenbereichen mit sieben Antwortformen, eigenen Abbildungen, Erklärungen, Merksätzen und fachlichen Quellen. Themenlernen, gemischte Zehnerrunden, Fehlerwiederholung und 30er-Prüfungsrunden mit 60 Minuten. Eigene transparente Punktevergabe, keine amtliche IHK-Note. Antworten, Markierungen, Pausen und die letzte Runde bleiben erhalten; der Spickzettel führt zurück zur Aufgabe. Fachlicher Prüfstand 04.11.2026; Rechenannahmen stehen ausdrücklich im Fall. Details: [WiSo](wiso.md).
- **Bedienung:** getrennte, vollständig anklickbare Auswahl-/Reviewkarten mit festen Checkboxen und Tastaturfokus auch in Abschnitt 07; vollständig deutsch, helles und dunkles Design, einklappbare Aufgabenliste und ausblendbare Bedienhinweise.
- **Pseudocode-Editor:** Zeilennummern, synchrones Scrollen, Tab/Umschalt+Tab für Einrückungen, Esc vor Tab zum Verlassen des Felds. Vorschau und Bewertung verlangen große Schlüsselwörter und nennen die konkrete Korrektur; Variablen und Texte bleiben erhalten. Diese Ergänzungen und alle 30 Aufgaben sind im Alpha-7-Paket enthalten.
- **Kopfnavigation:** Übersicht und Lernplan stehen immer vorn; fachliche Links wechseln mit dem besuchten Modul. Der Modulkontext bleibt innerhalb der Browsersitzung auf allgemeinen Seiten erhalten.
- **WiSo-Bedienkorrekturen:** Die Themenübersicht behält ihre ursprüngliche Reihenfolge mit WiSo an Position 08. Quellenlisten zeigen nur Titel; ein gemeinsamer Hinweis erklärt externe Tabs. Der Spickzettel erläutert Unsicherheit und Kontrollmarkierung. Abgabebestätigung und neue Auswertung werden in den sichtbaren Tastaturfokus gebracht, auch beim Start am Seitenende.
- **Betrieb:** lokales Profil, persönliche SQL-Einrichtung, Versionsanzeige und abschaltbare GitHub-Updateprüfung.
- **Projektauftritt:** Unterstützungskarte auf der Übersicht, Urheberseite mit Christopher Weismantel/b0rnkilla, öffentlicher PayPal-Link und GitHub-Verweise für einen Stern sowie Fehler/Vorschläge. Die Fußzeile hält diese Wege erreichbar; die README zeigt neue eigene Screenshots. Es werden keine Konten, Zahlungen oder Meldungen innerhalb der App verwaltet. Details: [Projektauftritt](projektauftritt.md).

Die Verbindungs- und Sitzungsdialoge sind deutsch und folgen dem gewählten hellen oder dunklen Design.

In der aktuellen Entwicklungsfassung bleibt die Scrollposition der Aufgabenliste bei Wechseln innerhalb eines SQL-Lernwegs erhalten. Im geführten Lernen sind Hinweise, Spickzettel und Musterlösung gleich aufgebaut: Titel und Beschreibung links, Aktion rechts. Der SQL-Spickzettel öffnet im selben Tab; der Entwurf bleibt beim Zurückkehren erhalten. Die Musterlösung besitzt die Aktionen „Ansehen“ und „Ausblenden“; ihre Hilfenutzung bleibt auch nach dem Schließen gespeichert.

## Architektur

`FiaeTrainer.Core` enthält die Fachlogik nach Lernmodulen. Das SQL-Modul gliedert sich in Ermittlung, Werkstatt, Prüfung, Ergebnisse, Datenbankzugriff und eingebettete Inhalte. `Algorithms` enthält eigene Inhalte, Parser, Interpreter und Bewertung. `BusinessStudies` enthält WiSo-Inhalte, strukturierte Bewertung und Rundenplanung. `Uml` enthält fünf Diagrammarten, `DatabaseDesign` relationale und ER-Entwürfe; `SoftwareDesign` die Anforderungen, Architektur-/API-/Formularaufgaben und deren eigene Bewertung. `QualityAssurance` ergänzt eigene Testfälle, feste lokale Ausführung, Überdeckung und Qualitäts-/Sicherheitsentscheidungen. `ProjectManagement` enthält eigene Projektpläne, CPM, Ressourcen-/Terminprüfung und Wirtschaftlichkeitsaufgaben. `FiaeTrainer.Web` enthält Oberfläche, Fortschritt, Einrichtung, Netzwerkregeln und Updates in getrennten Ordnern und Namespaces. Ein eigener Testbereich prüft diese Funktionen.

Neue fachliche Module erhalten eigene Unterordner neben `Core/Sql`. Gemeinsame Bedienung und Infrastruktur bleiben davon getrennt. Die konkrete Zuordnung und Einstiegspunkte stehen in [Entwicklung](entwicklung.md); Konventionen in der [Coding Guideline](coding-guideline.md).

Die Anwendung verwendet ausschließlich die gekennzeichnete Datenbank `FIAE_AP2_Training` mit synthetischen Daten. Lernabfragen laufen unter eingeschränkten SQL-Rechten; schreibende Werkstattversuche verwenden sitzungseigene Arbeitskopien. Die Bewertung vergleicht zwei Datenbestände und beweist keine allgemeine SQL-Gleichwertigkeit.

## Veröffentlichung und Lizenz

**Windows-Paket 0.1.1-alpha.8: gebaut und aus dem entpackten ZIP geprüft am 05.11.2026**, aus dem sauberen Commit `b4e953fbb628b9c39ce695538db2bf1343f13bcf`. Der Nutzer hat den Commit gepusht; der Remote-Stand wurde bestätigt. Enthalten sind 220 Übungen: 40 SQL-, 30 Algorithmus- und 150 WiSo-Aufgaben sowie der neue Projektbereich. Veröffentlichung und Upload erfolgen durch den Nutzer.

Die freigegebene öffentliche README führt Anwender ausschließlich über Releases zum Windows-ZIP und zur entpackten EXE. SQL Server bleibt für die SQL-Lernwege separat erforderlich. Ältere Pakete bleiben unverändert. Weitere Pakete nur auf ausdrücklichen Auftrag beziehungsweise zum vereinbarten Abschluss.

Für die neue Fassung gelten eigene Bedingungen zur privaten Nutzung. Kommerzielle Nutzung, Programmänderungen und Weiterverbreitung werden nicht eingeräumt. Eine pauschale MIT-Freigabe wird nicht eingeräumt. Zwingende gesetzliche Rechte und Lizenzen der Abhängigkeiten bleiben unberührt. Einzelheiten: [LICENSE](../LICENSE) und [Lizenzwechsel](lizenz.md).

## Weitere Lernmodule

Der Pseudocode-Basisumfang ist mit 30 Aufgaben abgeschlossen. Die sechs Ergänzungen zur ursprünglichen 24er-Planung trainieren Prüfziffern, Aufwand, Grenztests, Rekursion und paarweise Vergleiche. Weitere Aufgaben sind aus dieser Planung nicht offen; der Lernweg behauptet keine vollständige Abdeckung aller Algorithmusthemen. WiSo ist im Quellstand mit 150 Aufgaben umgesetzt. UML ist seit Alpha 13 mit 36 Aufgaben und eigenen Diagrammen in fünf Notationen umgesetzt. Datenbankentwurf ist seit Alpha 11 umgesetzt und besitzt in der aktuellen Quellfassung 42 Aufgaben einschließlich ER-Diagrammen, Unterlagen und eigenen Prüfungsfällen. Softwareplanung/OOP ist seit Alpha 14 mit 48 Aufgaben, eigenen Architektur-/API-/Formularentwürfen und drei Prüfungsfällen umgesetzt. Tests, Qualität und Sicherheit sind seit der Quellfassung Alpha 15 umgesetzt. Projekte und Wirtschaftlichkeit sind seit Alpha 16 mit 54 Aufgaben, eigenen Struktur-/Netz-/Terminplänen und drei Prüfungsfällen umgesetzt. Alle acht Themenkarten sind damit nutzbar; die vollständige Abdeckung sämtlicher AP2-Themen ist weiterhin kein zugesagter Umfang. Details: [Pseudocode](pseudocode.md), [WiSo](wiso.md).

## Abgrenzung

Onlinekonten, Registrierung und öffentlicher Serverbetrieb sind nicht geplant. Seit Alpha 12 lässt sich die optionale Heimnetz-Freigabe in derselben EXE unter **Heimnetz** einschalten und beenden. Windows-Diagnose, bei Bedarf erhöhte Einrichtung der appbezogenen Firewall-Regel, Kopieradresse und lokal erzeugter QR-Code ersetzen für Anwender die Quellprojektstarter. Alle verbundenen Geräte teilen einen Lernstand. Es gibt keine KI-Anbindung. Eine ausführbare Fremdschlüsselübung und weitere SQL-Themen fehlen noch.

Der Heimnetzzugriff wurde vom Nutzer auch mit einem Samsung-Handy bestätigt. Chrome benötigte dafür seine Android-Berechtigung für lokale Netzwerkzugriffe unter „Geräte in der Nähe“. Die einmalige Windows-Subnetzfreigabe blieb unverändert; Einzelheiten stehen in der [Heimnetz-Anleitung](heimnetz-und-serverbetrieb.md).

Für die Entwicklung in Visual Studio ist wegen .NET 10 Visual Studio 2026 erforderlich; alternativ genügt das passende SDK mit einem anderen Editor. Prüfungen und bekannte Grenzen dokumentiert das [Testprotokoll](testprotokoll.md).

## Fachliche Orientierung

- [FIAusbV § 13](https://www.gesetze-im-internet.de/fiausbv/__13.html) und [§ 14](https://www.gesetze-im-internet.de/fiausbv/__14.html).
- [IHK Hannover: Prüfungskataloge ab 2025](https://www.ihk.de/hannover/hauptnavigation/ausbildung-und-weiterbildung/ausbildung/ausbildung-a-z/neuordnungen/pruefungskataloge-it-berufe-6438900).
- [SQL Murder Mystery](https://github.com/NUKnightLab/sql-mysteries) als konzeptionelle Inspiration.

Eine vollständige Zuordnung des aktuellen Prüfungskatalogs zu allen geplanten Modulen steht noch aus.

## Oberflächenpflege in Alpha 9

WiSo gewichtet den Sachverhalt stärker als den Arbeitsauftrag. Der Falltext besitzt eine größere Schrift und eine eigene Abschnittsüberschrift; Aufgabentitel und Arbeitsauftrag sind kompakter. Eingabeeinheiten brechen bei Platzmangel um. Das nachgereichte b0rnkilla-Gaming-Logo ist auf der Projektseite eingebunden. Keine neue Aufgabenbewertung oder Lernstandmigration. Die Siegelabbildung Blauer Engel wurde auf Nutzerwunsch gestrichen. Das veröffentlichte Alpha-8-Paket bleibt unverändert; Alpha 9 wurde inzwischen auf Auftrag aus dem ZIP geprüft.

Mehrfachauswahl in WiSo zeigt die erforderliche Anzahl nun ausgeschrieben und fett hervorgehoben auf einer dezenten Akzentfläche. Bewertung und Profilformat bleiben unverändert.

## Prüfungsrunden in Alpha 9

SQL und Pseudocode bieten zusätzlich eigene 45-Minuten-Prüfungen mit 50 Trainingspunkten, gespeicherten Entwürfen, Pause, Kontrollmarkierung, Nachschlagen und Auswertung nach Abgabe. Pseudocode wählt drei Aufgaben aus einem neuen Zehnerpool, SQL fünf Aufgaben aus einem von drei neuen Fällen. Historie und Prüfungsstände bleiben von den Lernaufgaben getrennt. Die Ausgabe ist keine amtliche IHK-Note; bei technischen SQL-Störungen bleibt die Abgabe unbenotet gespeichert. Der Aufgabenwechsel über die linke Liste erhält nun auch die Gesamtseitenposition. Einzelheiten und Grenzen: [Prüfungsmodus](pruefungsmodus.md).

## Einheitlicher Prüfungseinstieg in Alpha 10

Alle drei aktiven Themenkarten führen über einen eigenen Button ohne Zeitangabe zu Prüfungsinfos. Bei SQL heißt der Fortsetzungsbutton Weiter lernen; Pseudocode und WiSo ordnen Prüfung üben unter dem Lernbutton an. WiSo verwendet /wiso/pruefung als eigenen Einstieg mit 30 Aufgaben und 60 Minuten; erst Prüfung starten legt eine Runde samt laufender Uhr an. Offene Lern- und Prüfungsrunden bleiben bis zur bestätigten Ersetzung erhalten. Der zusätzliche Prüfungskasten auf der WiSo-Lernseite entfällt; in der Kopfnavigation steht Prüfung üben. SQL/Pseudocode-Infos sind gekürzt, mit ausdrücklichem Spickzettelhinweis und den gewünschten Zeilenumbrüchen. Kein Inhalts-/Profilwechsel und kein neues ZIP; Alpha-9-Paket bleibt unverändert.

## Weiterer Feinschliff in Alpha 10

Verbindungshinweise trennen Meldung und nächste Handlung durch ausdrückliche Zeilenumbrüche, auch bei fehlgeschlagenem Fortsetzen und im statischen Ausgangstext. WiSo-Lernwegkarten lassen 24 Pixel Abstand zwischen Beschreibung und Button. Wiederverbindungsfunktionen, Bewertung und Lernstände unverändert; sieben bestehende Wiederverbindungstests, Quellbuild und isolierter Browserbetrieb erfolgreich. Kein neues Windows-Paket.

## Datenbankentwurf seit Alpha 11

Abschnitt 04 bietet 30 Aufgaben in sechs Kapiteln: Daten verstehen, Schlüssel, Beziehungen, Normalisierung, Fehlersuche und vollständige Modelle. Der Baukasten erfasst frei benannte Tabellen und Attribute mit festen Fachkennungen, PK/FK, gemeinsame UQ-Gruppen und Kardinalitäten. Das SVG-Modell entsteht automatisch und lässt sich vergrößern; eine ausgeschriebene Beziehungsliste unterstützt die Lesbarkeit. Kein SQL Server erforderlich.

Drei eigene Prüfungsfälle mit jeweils zwei Modellaufträgen und einer Verständnisaufgabe, 45 Minuten und 50 Trainingspunkten. Lokale Bewertung ausdrücklich definierter Modellvarianten, getrennte Entwürfe/Modi und Historie, Pause und Spickzettel-Rückweg. Vorhandene Modulstände bleiben erhalten. Modellprüfung und Zeitansätze haben dokumentierte Grenzen. Quellstand geprüft, kein neues Windows-Paket. [Fachvertrag](datenbankentwurf.md).

### Entwicklung ohne neues Release testen

Trainer-starten.cmd beziehungsweise Visual Studio mit Profil FiaeTrainer startet die neueste Quellfassung und übernimmt die gespeicherte Heimnetzwahl. Der separate LAN-Starter ist optional und erzwingt den Netzwerkmodus. Eine vorhandene Build-EXE unter bin/Release ist kein portables Release-Paket.

### Nächster Paketstand

Windows-ZIP 0.1.1-alpha.12 samt Release-Informationen ausdrücklich beauftragt, Bau erst nach Nutzer-Commit/Push. Versionshinweise vorbereitet; derzeit noch kein Alpha-12-Paket. Heimnetz vom Nutzer bestätigt, eigene Nutzerprüfung des Datenbankentwurfs noch offen.

### Paketprüfung Alpha 12 abgeschlossen

Das ausdrücklich beauftragte Windows-ZIP wurde nach Nutzer-Commit und bestätigtem Push aus e3af6aa gebaut und isoliert geprüft. 59 Dateien, 51.363.008 Bytes, keine privaten Inhalte. Aufgabenbewertung mit/ohne SQL, neue Entwurfsrunde, native Heimnetz-Diagnose/QR, Moduswechsel und Profilerhalt geprüft. Weitere Daten unter lokale-releases.md; Upload und Release-Tag übernimmt der Nutzer. Die nachträgliche Prüfdokumentation ändert das Paket nicht.

## 07.11.2026 – Datenbankentwurf an bestehende Lernansichten angeglichen

Linke einklappbare Aufgabenleiste, gemeinsame Lernmoduswahl und vertraute Geschichte-/Auftrag-/Arbeitsbereich-/Hilfendarstellung. Bestätigte Planungsschritte stehen wie der Algorithmusbericht rechts, mobil darunter. Auswahl- und Modellantworten sowie Erfolge bleiben erhalten; keine Inhalts- oder Profilmigration. 53 betroffene Tests und separater Browserbetrieb erfolgreich. Die Änderung gehört nur zur Quellfassung, das veröffentlichte Alpha-12-ZIP bleibt unverändert.

## 07.11.2026 – Storyüberarbeitung für Abschnitt 04

30 neue zusammenhängende Geschichten um Projektkoordinatorin Lea Berger und das Herbstforum. Fortschrittsabhängige Vorgängerbezüge und erzählerische Planungsergebnisse erklären die Verbindung der Aufgaben; offene Schritte erhalten Rücklinks und bleiben im Bericht erkennbar. Fachinhalte, Bewertung, Prüfungen und Profilvertrag unverändert. 56 betroffene Tests und isolierte Browserprüfung erfolgreich. Nur Quellprojekt, kein neues Paket.

## 07.11.2026 – Vertiefte Prüfung und Korrektur von Abschnitt 04

Einheitliche einzelne Lernrückmeldung ohne Trainingspunkte; gewichtete Punkte ausschließlich in der Prüfung. Falsche Auswahlen verraten keine komplette Erklärung, leere Antworten werden nicht versehentlich geprüft. Schnelle Navigation, Doppelabgaben, Moduswechsel, Fortsetzung und schreibgeschützte Baukastenaktionen abgesichert; neue Tabellennamen nach Entfernen bleiben eindeutig. Gesamtlauf 903 .NET-Tests einschließlich SQL und 31 JavaScript-Prüfungen erfolgreich, Build ohne Warnungen/Fehler. Browser mit eigenem Profil: richtige/falsche Auswahl, Modellreparatur, Pause/Neustart, Abgabe mit 10/50 beziehungsweise 50/50 und mobile Ansicht. Nur Quellfassung; Alpha-12-ZIP unverändert.

## 07.11.2026 – Abschnitt 04 um Diagramm- und Unterlagenaufgaben erweitert

42 statt 30 Entwurfsaufgaben: vier ER-Ergänzungen, zwei ER-zu-Tabellen-Aufträge, drei relationale Modellerweiterungen, zwei Entwürfe aus Unterlagen und eine konkrete Anomaliebeurteilung ergänzen den bisherigen Bestand. Insgesamt 262 Lernübungen in denselben vier aktiven Modulen. Lernstände und Prüfungsfälle bleiben erhalten; neue Ergänzungen werden je Aufgabe gespeichert. SQL/Pseudocode und weitere Abschnitte bleiben in dieser Alpha unberührt. Das veröffentlichte Alpha-12-Paket bleibt bei 250 Übungen; kein neues Paket.

## Paket Alpha 13

Am 09.11.2026 auf Nutzerauftrag aus gepushtem Anwendungscode b4224b0 gebaut und aus der entpackten nativen EXE geprüft. Enthalten sind die 36 UML-Aufgaben sowie der gesamte Datenbankentwurfsnachlauf mit 42 Aufgaben. Frühere Lernstände bleiben erhalten; kein automatischer Upload. [Paket und Prüfsumme](lokale-releases.md). Nutzererprobung und Zeitkalibrierung bleiben offen.

## Softwareplanung und OOP (05)

48 Aufgaben in acht Kapiteln setzen Nordhafen mit Mira König fort: Anforderungen, OOP, Erweiterbarkeit, Muster, Architektur, Schnittstellen, Oberflächen und Werkzeuge. Eigene strukturierte Spezifikationen, Klassenentwürfe, Architekturübersichten, JSON-/XML-Nachrichten und Formularprototypen; OOP-Unterlagen mit Zustands-/Rückgabetabellen. Freie Begründung gespeichert und ausdrücklich nur zur Selbstkontrolle. Drei neue 45-Minuten-Fälle, je 50 Trainingspunkte; eigene Profile, unveränderte alte Lernstände. [Fachvertrag](softwareplanung.md). Quellfassung Alpha 14; das zuletzt geprüfte Alpha-13-Paket bleibt bei 298 Übungen.

## Nutzerstand und Release-Vorbereitung am 11.11.2026

Der Nutzer hat einige Aufgaben aus Softwareplanung/OOP gelöst und die Umsetzung zunächst positiv bewertet. Das ist eine erste Erprobung, keine vollständige Abnahme aller 48 Aufgaben und drei Prüfungsfälle. Die README zeigt alle sechs Lernbereiche gleichwertig in zwei kompakten Dreierreihen; Vorschaubilder besitzen dieselbe Anzeigehöhe und verlinken die vorhandenen großen Bilder. 346 Lernübungen bleiben der aktuelle Quellumfang. Versionshinweise Alpha 14 vorbereitet; ZIP und Release-Informationen folgen erst nach dem Nutzer-Commit/Push. Kein neues Kapitel vor diesem Release-Schritt begonnen.

## Paket Alpha 14

Am 11.11.2026 aus dem bestätigten sauberen Commit 33f09a7 gebaut und aus dem ZIP geprüft: 346 Lernübungen, sechs Module und neue Softwareplanungsprüfungen. 51.548.922 Bytes, 59 Dateien; Projektlizenz, Drittanbieterhinweise und Helfer vollständig, keine privaten Dateien. Alte Modulstände erhalten; native API-/XML-/Formularaufgaben und 50/50-Prüfung mit Pause/Neustart erfolgreich. Alle Paketdateien unverändert. Prüfdokumentation nach dem Code-Commit separat; Tag und Upload übernimmt der Nutzer.
