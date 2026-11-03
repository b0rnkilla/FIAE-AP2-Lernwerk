# Projektsteckbrief

## Projektname

Technischer Name: `FIAE_AP2_Trainer`  
Anwendungsname: **AP2 Lernwerk**  
Erstes Lernmodul: **Fallakte Nachtzug – Die verschwundene Lieferung**

Aktueller Entwicklungsstand 0.1.1-alpha.1 vom 03.11.2026: 30 Ermittlungsaufgaben und zehn ergänzende SQL-Werkstattaufgaben. Die folgenden Angaben zur ersten Version werden durch den dokumentierten Werkstattausbau ergänzt.

Die Startseite ist inzwischen als allgemeiner AP2-Themenkompass aufgebaut. Systemabhängiges helles/dunkles Design und eine im Präferenzcookie gespeicherte manuelle Wahl sind umgesetzt. Die Ermittlungsakte akzeptiert bekannte Tabellennamen sowohl mit als auch ohne Schemapräfix.

Die Oberfläche verwendet eine blaue helle und eine grau-blaue dunkle Palette, kräftige serifenlose Überschriften, abgerundete Karten und klar beschriftete Lernschritte. Neue Aufgaben beider SQL-Lernwege starten im Modus **Ohne Hilfe lösen**; die letzte Auswahl wird je Aufgabe gespeichert. Der alternative Modus **Geführt lernen** erhält bestehende Entwürfe. Ein Wechsel zum Selbsttest benötigt nur bei vorhandenem Editorinhalt eine Bestätigung. Der allgemeine Selbsttest-Hinweis ist dauerhaft ausblendbar und ausschließlich im Lernplan wiederherstellbar. Die Aufgabenliste ist über einen kleinen Randpfeil einklappbar und merkt ihre Anzeige im lokalen Profil. Die Ermittlung stellt Lernmodus, Story, Auftrag und Editor in den Mittelpunkt; allgemeine Moduserklärungen, Speicherhinweise und der doppelte Übersichtslink entfallen. Fachliche Erklärungen im geführten Modus stehen unter dem Editor bei den Hilfen. Die Navigation verwendet Aufgabe; die zusätzlichen Tabellen-/Editor-Sprunglinks wurden entfernt. Unterstützung und bisherige Erfolge bleiben nachvollziehbar.

Die elf Verbesserungswünsche sind umgesetzt: Paket A vereinfacht Bedienung und Moduswahl; Paket B verlangt große SQL-Schlüsselwörter und Funktionen sowie ein Semikolon je Anweisung, bereinigt sichtbare Schemapräfixe und vereinheitlicht Datums-/Zeitangaben. Paket C ersetzt Kalenderwochen durch profilbezogene Empfehlungen für neue Aufgaben, offene Versuche und selbstständige Wiederholungen. Paket D verbindet alle 30 Storyschritte und stellt Fallnotizen chronologisch mit erkennbaren Lücken dar. Der Nutzer hat die Gesamtfassung positiv erprobt. Die neue Zielrichtung ist ein lokales Windows-x64-Release mit eingebauter .NET-Laufzeit; Login, Registrierung, Benutzertabelle und öffentlicher Homeserver-/nginx-Betrieb sind gestrichen. docs/verbesserungsplan.md dokumentiert den Stand; Markdown-Pflege ist in AGENTS.md dauerhaft festgehalten.

---

## Ausgangssituation

Für die in etwa drei Wochen anstehende FIAE-Abschlussprüfung Teil 2 sollen insbesondere SQL-Grundlagen und das selbstständige Formulieren von Lösungen wieder sicher verfügbar werden. Der Nutzer arbeitet mit C# und Microsoft SQL Server, möchte möglichst wenig Entwicklungszeit aufbringen und den entstehenden Code später selbst nachvollziehen, debuggen und erweitern können.

Vorhandene Unterlagen des anderen Projekts dienen ausschließlich als Struktur- und Stilvorlagen. Fachliche Rahmenbedingungen, Unternehmensinformationen und Ergebnisse dieses anderen Projekts werden nicht übernommen. Dieses Dokument ist kein IHK-Projektantrag und kein Nachweis einer eigenständig erbrachten betrieblichen Projektarbeit.

---

## Projektziel

Eine vollständig deutschsprachige, browserbediente Lernanwendung soll SQL durch selbst geschriebene, tatsächlich ausgeführte Abfragen vermitteln. Eine geführte Ermittlung steigert sich von SELECT-Grundlagen bis zu komplexeren Beziehungen und Unterabfragen. Ziel ist die Übertragung auf selbstständige Prüfungsantworten, nicht das bloße Wiedererkennen von Musterlösungen. Ein Prüfungserfolg oder eine bestimmte Punktzahl kann daraus nicht abgeleitet werden.

---

## Verbindliche Anforderungen

* C# mit ASP.NET Core und Blazor; SQL Server als Datenbankplattform.
* Deutsche Oberfläche, Aufgaben, Hinweise, Fehlermeldungen, Kommentare und Dokumentation.
* Englische Codebezeichner entsprechend der Coding Guideline.
* Hauptmenü mit aktivem SQL-Modul und klar gekennzeichneten zukünftigen Modulen.
* Schrittweise Anleitung, freiwillige Hinweise und erklärte Musterlösungen.
* Freie SQL-Eingabe und fachliche Bewertung alternativer korrekter Lösungen.
* Modus zum selbstständigen Lösen ohne Hilfen und ohne Probeausführung.
* Persistenter Lernfortschritt und Wiederholungsvorschläge ohne feste Wochenaufteilung.
* Spätere Windows-x64-Veröffentlichung mit eingebauter .NET-Laufzeit und separater SQL-Voraussetzung. Version in der Oberfläche; Updatehinweis aus öffentlichen GitHub-Releases ohne automatischen Dateiaustausch.
* Einmalige persönliche Instanzauswahl mit Windows-Anmeldung, Verbindungsprüfung und Vorbereitung ausschließlich der eigenen Trainingsdatenbank. Speicherung außerhalb der öffentlichen Vorgaben; keine Konfigurationsänderung durch Heimnetz-Besucher.
* SQL-Schreibregeln im Trainer: große Schlüsselwörter und Funktionen, Semikolon je vollständiger Anweisung; Datums-/Zeitbeispiele im Format YYYY-MM-DD HH:mm:ss.
* Verständliche Architektur, Entwicklungsprotokoll und überprüfte Lerninhalte.
* Keine Lückentexte als vorgesehene Hauptlernform für Pseudocode.
* Keine automatisch entstehenden API-Kosten oder zusätzlichen Abonnements.

---

## Technischer Lösungsansatz

Die .NET-10-Projektmappe enthält eine Core-Klassenbibliothek, eine Blazor-Webanwendung und ein Testprojekt. Die Anwendung läuft auf dem lokalen Rechner und wird im Browser bedient. Seit 03.11.2026 gibt es zusätzlich einen ausdrücklich aktivierten Heimnetz-Startmodus mit privater IP-Zugriffskontrolle und einer auf das private lokale Subnetz beschränkten Windows-Firewallregel für TCP 5080. Der Standardstart bleibt lokal. Die Quellprojekt-/Heimnetzfassung verwendet ein gemeinsames lokales Profil. Die veröffentlichte EXE speichert pro Windows-Benutzer unter LocalApplicationData/AP2-Lernwerk. Das erste geprüfte Paket 0.1.0 bleibt eine frühe Momentaufnahme; weitere Pakete entstehen erst am Entwicklungsende beziehungsweise auf ausdrücklichen Wunsch. Die aktuelle Quellprojektfassung 0.1.1-alpha.1 zeigt ihre Version, prüft öffentliche GitHub-Releases und bietet eine persönliche SQL-Einrichtung. Die Verteilung erfolgt künftig über GitHub-Releases; Onlinekonten und öffentlicher nginx-Betrieb sind nicht mehr geplant. Einzelheiten stehen in heimnetz-und-serverbetrieb.md.

Die feste Datenbank `FIAE_AP2_Training` enthält ausschließlich synthetische Daten. Der SQL-Server-Zugriff verwendet die vorhandene Windows-Anmeldung. Die Anwendung prüft Datenbankname und Eigentumskennzeichnung, bevor sie ihr Schema initialisiert. Wiederholte Starts verändern bestehende Falltabellen nicht.

Lernabfragen werden mit Microsoft ScriptDom geparst. Die Ermittlungsakte erlaubt nur eine SELECT-Anweisung auf das Fallschema und einen definierten Funktionsumfang. Zusätzlich erfolgt die Ausführung als Datenbankbenutzer ohne Login und ohne Schreibrechte, ohne Möglichkeit zur Rückkehr zum ursprünglichen Kontext. Nicht gepoolte Verbindungen, vier Sekunden Abfragelimit und maximal 200 Ergebniszeilen begrenzen Fehlversuche. Der Standardstart erlaubt lokale Zugriffe; das zusätzliche Heimnetz-Profil erlaubt ausdrücklich private Netzadressen. Öffentlicher Appbetrieb und eigene Konten gehören nicht mehr zum geplanten Umfang.

Bewertet wird das Ergebnis im Vergleich mit einer Referenzabfrage. Eine zweite, abweichende Datenbasis erschwert zufällige oder auf bekannte IDs zugeschnittene Erfolge. Eine allgemeine Beweisführung über beliebige SQL-Abfragen wird nicht behauptet.

---

## Pflichtumfang der ersten Version

* 30 Missionen in fünf Kapiteln mit eigener deutscher Geschichte.
* SELECT, WHERE, AND/OR, Klammern, ORDER BY, TOP, BETWEEN, LIKE, IN, NULL und DISTINCT.
* INNER JOIN, LEFT JOIN, mehrere Beziehungen und fehlende Zuordnungen.
* COUNT, SUM, AVG, MIN, MAX, GROUP BY und HAVING.
* Skalare und korrelierte Unterabfragen, EXISTS und NOT EXISTS.
* SQL-Editor, Ergebnistabelle, Tabellenübersicht und erspielte Erkenntnisse.
* Geführter Lernmodus und selbstständiger Lösungsmodus.
* Fortschrittsdatei, SQL-Spickzettel und Dreiwochen-Lernvorschlag.
* Automatisierte Inhalts-/Sicherheitstests und manuelle Browserprüfung.
* Startanleitung sowie Hinweise zum Debuggen und Erweitern.

---

## Abgrenzung und bekannte Grenzen

Die App ist noch keine vollständige AP2-Vorbereitung. Datenänderungen, CREATE TABLE, ALTER TABLE ADD, PRIMARY KEY/NOT NULL/CHECK und grundlegende Transaktionen sind inzwischen in zehn Werkstattaufgaben interaktiv vorhanden. Berechtigungen, ausführbare Fremdschlüsselaufgaben und weitere SQL-Themen fehlen noch. Ebenso sind Pseudocode, UML und die übrigen Hauptmenüthemen noch Platzhalter.

Keine Benutzerverwaltung, Mehrbenutzerfreigabe, Cloudbereitstellung oder KI-Bewertung. Kein beliebiger SQL-Server-Administrationszugriff aus dem Lerneditor. Keine automatische Datenbanklöschung und keine Änderungen an vorhandenen fremden Datenbeständen. Die Prüfungsnähe ist eine didaktische Orientierung; es werden eigene Aufgaben und keine Originalprüfungsaufgaben angeboten.

Das verwendete .NET 10 erfordert für offiziell unterstütztes Targeting in Visual Studio die Version 2026; das installierte SDK funktioniert unabhängig davon über die Kommandozeile. Visual Studio 2022 bleibt eine technische Einschränkung für späteres Debuggen in genau dieser IDE.

---

## Weitere Ausbaustufen

1. SQL-Werkstatt weiter ausbauen: Die ersten zehn Datenänderungs-, Tabellen- und Transaktionsaufgaben sind umgesetzt. Es folgen bei Bedarf weitere Datenbankthemen und zusätzliche Transferfälle.
2. Pseudocode: deutsche Konventionen wie WENN/SONST/ENDE WENN, Fehlersuche, Schreibtischtests und freie Aufgaben. Eine ausführbare Teilmenge muss eindeutig definiert werden; gängige Schreibweisen bilden noch keinen einheitlichen Parserstandard.
3. UML: Diagramme lesen, Fehler lokalisieren und begrenzte freie Modelle erstellen; erst danach eine umfangreichere Zeichenoberfläche erwägen.
4. Datenbankentwurf, Softwareplanung/OOP, Tests/Qualität/Sicherheit, Projektmanagement/Wirtschaftlichkeit und WiSo.
5. Zusätzliche unbekannte Transferaufgaben und zeitgesteuerte Übungssets.

---

## Qualitätssicherung

Alle Referenzlösungen werden auf echtem SQL Server ausgeführt. Zeilen- und Spaltenzahlen sowie zentrale Aggregat- und Fallauflösungsergebnisse besitzen unabhängige erwartete Werte. Die Tests prüfen typische falsche Antworten, alternative Lösungen, Duplikate, NULL, Ergebnisreihenfolge und eingeschränkte Zugriffsrechte. Der tatsächliche Stand wird getrennt im Testprotokoll festgehalten.

Die zehn Werkstattaufgaben laufen pro Versuch in einer eigenen nicht gepoolten Sitzung mit ausschließlich lokalen temporären Tabellen unter dem vorhandenen eingeschränkten Benutzer. Es werden keine dauerhaften Schreibrechte ergänzt. Beide Ausgangsdatenbestände werden auf vollständige Endzustände, Spalten-/Typmerkmale und ausgewählte tatsächlich erzwungene Datenregeln geprüft. Bei ROLLBACK wird außerdem der Zustand vor der Rücknahme verglichen. Ein Versuch ist insgesamt auf acht Sekunden begrenzt; jeder SQL-Befehl zusätzlich auf vier Sekunden.

Der Werkstattfortschritt ist von der Ermittlungsakte getrennt und zu vorhandenen Fortschrittsdateien abwärtskompatibel. Wiederholungen erscheinen im gemeinsamen Lernplan. Die gesamte Anwendung enthält weiterhin keine KI-Anbindung.

---

## OpenAI-Anbindung

Nach der am 02.11.2026 geprüften offiziellen Dokumentation können berechtigte Pro- und Business-Abos aktivierbares API-Guthaben enthalten; Plus ist für diesen Vorteil nicht berechtigt. Anspruch, Aktivierung, Ablauf und Verbrauch wären für den konkreten Account gesondert zu prüfen. Eine allgemein kostenlose automatische Bewertung über einen beliebigen ChatGPT-Account wird deshalb nicht zugesagt. Die Anwendung enthält keine API-Aufrufe, Schlüssel oder automatische Abrechnung. [Offizielle Dokumentation](https://developers.openai.com/api/docs/guides/chatgpt-api-credits)

---

## Quellen für die inhaltliche Ausrichtung

* [FIAusbV § 13: Planen eines Softwareproduktes](https://www.gesetze-im-internet.de/fiausbv/__13.html)
* [FIAusbV § 14: Entwicklung und Umsetzung von Algorithmen](https://www.gesetze-im-internet.de/fiausbv/__14.html)
* [IHK Hannover: überarbeitete Prüfungskataloge ab 2025](https://www.ihk.de/hannover/hauptnavigation/ausbildung-und-weiterbildung/ausbildung/ausbildung-a-z/neuordnungen/pruefungskataloge-it-berufe-6438900)
* [SQL Murder Mystery als konzeptionelle Inspiration](https://github.com/NUKnightLab/sql-mysteries)

Geschichte, Aufgaben und Falldaten des Lernwerks wurden eigenständig für diese App verfasst. Es wurden keine Originaltexte, Bilder oder Datenbanken aus SQL Murder Mystery übernommen. Eine vollständige Zuordnung sämtlicher aktueller Prüfungsgegenstände zum vollständigen Prüfungskatalog bleibt eine eigene spätere Arbeit.
