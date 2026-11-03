# AP2 Lernwerk

Deutschsprachige FIAE-AP2-Lernanwendung mit C#, ASP.NET Core Blazor und Microsoft SQL Server. Enthalten sind die **Fallakte Nachtzug** mit 30 interaktiven SQL-Aufgaben sowie eine **SQL-Werkstatt mit zehn freien Schreib-, Struktur- und Transaktionsaufgaben**.

## Windows-Paket ohne Entwicklungsumgebung

Die Veröffentlichung ist als lokales Windows-x64-Paket geplant. Das bereits erzeugte Paket 0.1.0 ist eine frühe Alpha-Momentaufnahme. Weitere Pakete entstehen auf Nutzerwunsch erst am Ende der Entwicklung. Aktuelle Entwicklungsfassung: **0.1.1-alpha.1**; die Browseroberfläche zeigt ihre tatsächlich gebaute Version im Fußbereich und unter **Updates & SQL-Einrichtung**. Das alte ZIP wird während der Entwicklung nicht neu erstellt.

Die spätere EXE enthält .NET und ASP.NET Core. **SQL Server beziehungsweise eine vorhandene LocalDB-Installation bleiben separat erforderlich**; SSMS allein genügt nicht.

Die EXE speichert Lernstand und persönliche Konfiguration unter **%LOCALAPPDATA%\AP2-Lernwerk**. Updates im Programmordner überschreiben diese Dateien nicht. Das Quellprojekt verwendet weiterhin seinen bisherigen lokalen Lernstand. Übernahme, andere SQL-Instanz und Startprobleme: [Anleitung für lokale Releases](docs/lokale-releases.md).

**Login, Registrierung, Benutzertabelle und öffentlicher Homeserver-/nginx-Betrieb entfallen auf Nutzerwunsch.** Jeder führt seine eigene lokale Kopie aus. Die konfigurierte Updatequelle ist [b0rnkilla/FIAE-AP2-Lernwerk](https://github.com/b0rnkilla/FIAE-AP2-Lernwerk). Beim Programmstart prüft die aktuelle Entwicklungsfassung im Hintergrund öffentliche Releases. Ein Hinweis führt zum Download; die App ersetzt keine Dateien selbst. Uploads sind nicht automatisiert.

## Starten

1. **Trainer-starten.cmd** im Projektordner doppelt anklicken.
2. Sobald die Anwendung bereit ist, öffnet sich **http://localhost:5080** automatisch. Falls das nicht klappt, die Adresse im Browser öffnen.
3. Auf **Ermittlung starten** beziehungsweise **Weiter ermitteln** klicken.

Das Konsolenfenster während des Lernens geöffnet lassen. Zum Beenden Strg+C drücken. Ein schon laufender Trainer muss nicht erneut gestartet werden.

Alternativ im Projektverzeichnis:

```powershell
dotnet run --project src/FiaeTrainer.Web
```

Voraussetzungen: .NET-10-SDK, eine erreichbare lokale SQL-Server-Instanz und beim ersten Paketdownload Internetzugang. Danach benötigt der Trainer keine externen Dienste und keine KI-API. Standard ist `.\SQLEXPRESS` mit Windows-Anmeldung. **SSMS ist ein Verwaltungswerkzeug; die Verbindung erfolgt zur SQL-Server-Instanz.**

Die Anwendung erstellt beim ersten Start ausschließlich die eigene Datenbank `FIAE_AP2_Training`. Dafür benötigt die Windows-Anmeldung einmalig die Berechtigung zur Datenbankanlage. Bestehende andere Datenbanken werden nicht initialisiert. Eine gleichnamige Datenbank ohne Trainer-Kennzeichnung wird abgewiesen. Die SQL-Server-Konfiguration bleibt unverändert. Für die ausdrücklich gewünschte Heimnetzfreigabe wurde eine gezielte Windows-Firewallregel eingerichtet.

## Vom anderen Rechner im Heimnetz öffnen

**Trainer-im-Netzwerk-starten.cmd** startet den zusätzlichen Heimnetz-Modus. Einen noch laufenden lokalen Trainer vorher beenden, wenn er Port 5080 belegt. Das Startfenster zeigt die Adresse für die anderen Rechner, bei der Einrichtung am 03.11.2026 **http://192.168.1.21:5080**. Auf dem Trainer-Rechner funktioniert weiterhin **http://localhost:5080**. Die Adresse kann sich bei einer neuen Netzwerkzuweisung ändern.

Die Windows-Firewallregel **FIAE_AP2_Trainer-LAN-5080** erlaubt nur TCP 5080 aus dem lokalen Subnetz im privaten Netzwerk. Die übrige Firewall bleibt aktiv. **Heimnetz-Firewall-einrichten.ps1** enthält die einmalige Regelanlage mit Administratorrechten. Die SQL-Datenbank bleibt ausschließlich Aufgabe des Trainer-Rechners; auf dem zweiten Rechner genügen Browser und Netzwerkverbindung.

**In der optionalen Heimnetz-Vorführung teilen alle Besucher denselben Lernstand.** Für die Verteilung erhält jeder eine eigene lokale Kopie. Eigene Konten und nginx-Betrieb sind nicht mehr geplant. Eine Teams-Bildschirmfreigabe funktioniert weiterhin; die private Heimnetzadresse ist von außerhalb nicht direkt erreichbar.

Anleitung und Fehlersuche für die optionale Vorführung: [Heimnetz und Serverbetrieb](docs/heimnetz-und-serverbetrieb.md).

## Lernen

Die elf Anpassungen aus dem ausführlichen Nutzertest sind umgesetzt und im [Verbesserungsplan](docs/verbesserungsplan.md) nachverfolgbar: individuelle Aufgabenmodi, schlankere Bedienung, SQL-Schreibregeln, ein Lernplan im eigenen Tempo und eine zusammenhängende Fallakte. Die anschließende positive Rückmeldung ist berücksichtigt; statt Homeserver-Ausbau folgt die lokale Windows-Veröffentlichung.

Die Startseite ist als allgemeiner FIAE-AP2-Themenkompass aufgebaut. SQL ist der erste verfügbare Themenbereich mit zwei Lernwegen; die weiteren Bereiche sind ausdrücklich als in Vorbereitung gekennzeichnet. Der Gesamtfortschritt zählt alle tatsächlich verfügbaren Aufgaben, aktuell 30 Ermittlungsmissionen und zehn Werkstattaufgaben.

**Ermittlung starten** öffnet die erste Aufgabe. Sobald du begonnen hast, führt **Weiter ermitteln** zur zuletzt bearbeiteten Aufgabe. Syntaxfehler werden mit dem betroffenen SQL-Ausschnitt und konkreten Hinweisen etwa zu Anführungszeichen oder Klammern erklärt. Identische Fehlermeldungen erscheinen nach einer Bewertung nur einmal.

- **Ohne Hilfe lösen (Standard für neue Aufgaben):** SQL selbst schreiben und mit **Lösung prüfen** bewerten lassen. Keine Erklärung, Hinweise, Musterlösung oder Probeausführung. Die Tabellenübersicht bleibt als Aufgabenmaterial sichtbar. Der zuletzt gewählte Modus wird für jede Aufgabe einzeln gespeichert, auch über Aufgabenwechsel und Neuladen hinweg.
- **Geführt lernen:** Erklärung lesen, SQL selbst schreiben, ausprobieren und prüfen lassen. Bei Bedarf drei gestufte Hinweise oder die erklärte Musterlösung öffnen. Der Wechsel in diesen Modus erhält den aktuellen Entwurf.
- **Neuer Selbsttest:** Bei leerem Editor startet der Modus ohne Hilfe direkt. Ist ein Entwurf vorhanden, fragt die Oberfläche vor dem Leeren nach; **Entwurf behalten** bricht den Wechsel ab. Gelöste Aufgaben bleiben erhalten. Gespeicherte unterstützte Entwürfe zählen auch nach einem Neuladen nicht als selbstständige Lösung; ältere Dateien ohne diese Kennzeichnung werden vorsichtig genauso behandelt. Fehlt bei einer älteren Datei die gespeicherte Moduswahl, beginnt die Aufgabe im Standardmodus ohne Hilfe.
- **Allgemeiner Bedienhinweis:** Das X an „So funktioniert dein Selbsttest“ blendet das Fenster im aktuellen Profil für alle Aufgaben beider SQL-Lernwege aus. **Lernplan → Bedienhinweise → Selbsttest-Hinweise wieder anzeigen** stellt es wieder her. Aufgabenhinweise und Musterlösungen werden dadurch nicht beeinflusst.
- **Fallnotizen:** Zeigt die bereits erspielten Hinweise der Geschichte. **Tabellen & Spalten** erklärt die Datenbasis.
- **SQL-Schreibregeln:** Schlüsselwörter und Funktionen groß schreiben; jede vollständige Anweisung mit Semikolon abschließen, auch BEGIN TRANSACTION, COMMIT und ROLLBACK in der Werkstatt. Bei Fehlern nennt die App die konkrete Korrektur. Texte, Kommentare, Tabellen, Spalten und Aliase werden nicht pauschal verändert. Bestehende Entwürfe bleiben erhalten und müssen bei Bedarf selbst an die neuen Regeln angepasst werden.
- **Datumswerte:** Beispiele und Ergebnisse verwenden YYYY-MM-DD HH:mm:ss, zum Beispiel `2026-10-12 20:00:00`. Die SQL-Verbindungen setzen ausdrücklich DATEFORMAT ymd. Andere gültige Datumsschreibweisen werden weiterhin akzeptiert.
- **Aufgeräumte Aufgabenansicht:** Lernmodus, Story, Auftrag und SQL-Editor stehen im Mittelpunkt. Allgemeine Moduserklärungen, Speicherhinweise und der doppelte Link Dein Lernraum sind entfernt. Erklärungen im geführten Modus stehen unter dem Editor bei den übrigen Hilfen. Ein kleiner Pfeil am Rand der Aufgabenliste klappt sie ein oder aus; die Auswahl bleibt im aktuellen Profil für beide SQL-Bereiche gespeichert.
- **Lernplan:** Zeigt die nächste neue Aufgabe je Lernweg, begonnene ungelöste Aufgaben und gelöste Aufgaben ohne bisherigen selbstständigen Erfolg. Keine Kalenderwochen oder feste Lernzeit. Entwürfe gelten auch ohne Abgabe als begonnen.
- **Fallgeschichte:** Alle 30 Storytexte erklären die Verbindung zum vorherigen Schritt. Fallnotizen bilden eine chronologische Zusammenfassung gelöster Aufgaben; offene Zwischenstücke werden als Lücken markiert. Ein Sprung weist auf die unmittelbar vorherige noch offene Spur hin.
- **SQL-Spickzettel:** Enthält die Regeln und Beispiele für diese Aufgaben, ohne Schemapräfix.

Die Abfragen werden tatsächlich von SQL Server ausgeführt. Korrekte alternative Formulierungen sind willkommen. Bewertet werden Spaltenreihenfolge, Werte, Duplikate und bei Sortieraufgaben die Zeilenreihenfolge; abweichende Spaltenaliase sind erlaubt. Ein zweiter Datenbestand prüft zusätzlich die allgemeine Regel. Das ist kein mathematischer Beweis der Gleichwertigkeit beliebiger SQL-Abfragen und keine IHK-Punktevergabe.

Entwürfe werden beim Verlassen des Editors sowie bei Ausführung, Abgabe oder Nutzung von Hilfen gespeichert. Bei einem abrupt geschlossenen Tab kann der letzte noch fokussierte Entwurf fehlen. Der Lernstand liegt unter `src/FiaeTrainer.Web/Data/progress/progress.json`. Es gibt **ein lokales Profil**, das auch bei einem anderen Browser auf demselben Rechner verwendet wird. Ein Erfolg im Modus ohne Hilfen wird getrennt von einem geführten Abschluss vermerkt; er belegt noch keine dauerhafte Beherrschung.

## SQL-Werkstatt

Auf der Startseite oder in der Navigation **SQL-Werkstatt** öffnen. Zehn Aufgaben üben UPDATE, INSERT, DELETE, CREATE TABLE mit PRIMARY KEY/NOT NULL/CHECK, ALTER TABLE ADD sowie BEGIN TRANSACTION, COMMIT und ROLLBACK.

Jede Ausführung beginnt mit frischen Ausgangsdaten. Die vollständige Lösung muss daher in einem Skript stehen; mehrere nacheinander ausgeführte Versuche bauen nicht aufeinander auf. **Ausgangsdaten ansehen** zeigt die Ausgangstabellen, die Ergebnislisten den tatsächlichen Stand nach deinem Skript. Ergänzende SELECT-Anweisungen sind erlaubt, ihre Ausgabe wird jedoch nicht separat angezeigt: Bewertet und angezeigt wird der gesamte Tabellenstand.

Bei ROLLBACK lässt sich zusätzlich der Zwischenstand vor der Rücknahme öffnen. Die Bewertung prüft so auch den versuchsweisen Abzug. Bei Strukturaufgaben werden Datentypen, Spalten, NULL-Zulässigkeit und Schlüsselmerkmale verglichen sowie ungültige Einfügeversuche gezielt getestet.

Die lesende Ermittlungsakte und die schreibende Werkstatt haben getrennte Entwürfe und Fortschritte. Bestehende Fortschrittsdateien der ersten Version bleiben lesbar. Das Zurücksetzen im Lernplan betrifft weiterhin das gesamte lokale Profil.

Technisch werden die logischen Tabellennamen auf lokale temporäre SQL-Server-Tabellen derselben Sitzung umgeleitet. Die üblichen SQL-Schreibweisen bleiben im Editor erhalten. COMMIT bestätigt ausschließlich diese Arbeitskopie. Dauerhafte Tabellen erhalten keine zusätzlichen Schreibrechte. Fremdschlüssel sind in diesen isolierten Strukturübungen noch nicht enthalten, da SQL Server FOREIGN KEY auf temporären Tabellen nicht erzwingt.

## Darstellung und Tabellennamen

Oben rechts wählst du **System**, **Hell** oder **Dunkel**. Ohne eigene Auswahl folgt die App der vom Browser gemeldeten Systempräferenz, auch wenn diese sich während der Nutzung ändert. Die ausdrückliche Auswahl wird im Präferenzcookie `ap2-theme` mit einer maximalen Laufzeit von einem Jahr gespeichert; blockierte oder gelöschte Cookies können die dauerhafte Speicherung verhindern. Bei HTTPS erhält das Cookie zusätzlich das Secure-Merkmal. Ein Zurücksetzen des Lernstands ändert diese Designwahl nicht.

Beide Farbschemata umfassen Editor, Tabellen, Hinweise, Rückmeldungen und Nachschlagehilfe. Während einer Abfrage bleibt der Editor lesbar und auswählbar, ist aber vorübergehend schreibgeschützt. Textcursor und Mauszeiger sind ausdrücklich kontrastreich gestaltet; das native Browserfarbschema wird passend mitgesetzt.

Oberfläche, Hinweise und Musterlösungen verwenden kurze Tabellennamen wie **Sendungen**. Intern ordnet der Syntaxbaum sie dem Fallbestand beziehungsweise dem separaten Prüfbestand zu. Bestehende korrekt qualifizierte Entwürfe bleiben kompatibel. Andere Schemas, fremde Datenbanken und unbekannte Tabellen bleiben ausgeschlossen. Die Werkstatt verwendet ihre eigenen logischen Tabellennamen.

## SQL-Verbindung einrichten

Beim Start wird die gespeicherte Verbindung geprüft. Scheitert sie, bietet die Übersicht **SQL-Verbindung einrichten** an. Unter **Updates & SQL-Einrichtung** kannst du registrierte lokale Instanzen auswählen oder einen eigenen Servernamen eintragen, beispielsweise MEIN-PC\MEINE-INSTANZ oder (localdb)\MSSQLLocalDB. Die App prüft die Windows-Anmeldung, bereitet ausschließlich FIAE_AP2_Training vor und speichert erst nach Erfolg. Lernstände bleiben erhalten. Bereits offene Lernseiten nach einem Verbindungswechsel neu öffnen.

Die lokale Erkennung ist eine Auswahlhilfe: Ein fehlender Treffer beweist nicht, dass kein SQL Server vorhanden ist; ein registrierter Eintrag garantiert weder laufenden Dienst noch passende Rechte. Die Einrichtungsseite verlinkt den offiziellen Microsoft-Download. Es erfolgt keine automatische Installation und kein willkürliches Ausprobieren anderer Server. Auch die auf diesem Rechner vorhandene LocalDB wurde mit allen SQL-Integrationstests geprüft.

Die Einrichtung ist im gemeinsamen Heimnetz-Modus schreibgeschützt. Zum Ändern die Vorführung beenden und im lokalen Startprofil starten. So können andere Vorführungsbesucher keine lokale Verbindung umstellen oder neue Datenbankziele wählen.

## Private Einstellungen und Git

Die generische Windows-Anmeldungsverbindung in appsettings.json enthält kein Kennwort und bleibt öffentlich. Persönliche Angaben gehören in die ignorierte appsettings.Local.json: im Quellprojekt unter src/FiaeTrainer.Web, in späteren Release-Paketen unter %LOCALAPPDATA%\AP2-Lernwerk. Für QA kann LocalRelease:SettingsFile einen eigenen absoluten Pfad bestimmen. Bereits vorhandene übrige Einstellungen werden beim Speichern erhalten; beschädigte Dateien nicht still überschrieben.

Die .gitignore schließt unter anderem lokale Konfiguration einschließlich temporärer Schreibdateien, Entwicklungs-/Produktionskonfiguration, .env-Dateien, Schlüssel/Zertifikate, Datenbankdateien, Lernstände, Buildausgaben, Logs und Testartefakte aus. Öffentliche Vorlagen, Publishprofil, Inhalte und LICENSE bleiben aufnehmbar. Sie erkennt keine Geheimnisse innerhalb beliebiger C#-/Markdown-Dateien und entfernt keine bereits versionierten Dateien aus einer Git-Historie. Vor dem ersten Push die VS-Änderungsliste kontrollieren. Eine isolierte Git-Prüfung bestätigt die vorgesehenen privaten und öffentlichen Dateibeispiele.

## Updatehinweise vorbereiten

Die automatische Prüfung ist in der Einrichtung abschaltbar und benötigt für öffentliche Releases keinen GitHub-Schlüssel. Offlinefehler, API-Limits, ein noch nicht erreichbares Repository oder fehlende Releases blockieren das Lernen nicht. Aktuell antwortete die GitHub-API für die genannte Quelle beim Test mit 404; eine funktionierende öffentliche Releasequelle wird nicht vorgetäuscht.

Für spätere Releases: Versionsnummer zentral in Directory.Build.props setzen und einen passenden Tag wie v0.1.1-alpha.2 veröffentlichen. Ein fertiges Windows-Paket mit Namen *-win-x64.zip als Release-Anhang hochladen. Reine Git-Tags und GitHubs automatisch angebotene Quellcode-ZIPs genügen nicht. Alpha-Versionen berücksichtigen neuere Vorab- und stabile Versionen; stabile Fassungen berücksichtigen stabile Releases. Bis zu 100 Release-Einträge werden nach SemVer verglichen, nicht alphabetisch oder nach bloßem Veröffentlichungsdatum. Nach einem Hinweis: App beenden, vollständiges ZIP in einen neuen Ordner entpacken und die neue EXE starten. Ein automatischer Dateiaustausch findet nicht statt.

## Debuggen und erweitern

Die Projektmappe ist `FIAE_AP2_Trainer.sln`. Startprojekt: `FiaeTrainer.Web`.

Das Projekt verwendet das auf diesem Rechner installierte .NET 10. **Für die vollständige .NET-10-Unterstützung in Visual Studio wird Visual Studio 2026 benötigt.** Mit dem vorhandenen SDK funktionieren außerdem die Kommandozeile und ein entsprechend eingerichteter anderer Editor. Visual Studio 2022 unterstützt das Targeting von .NET 10 nicht offiziell. [Microsoft-Kompatibilitätstabelle](https://learn.microsoft.com/en-us/dotnet/core/porting/versioning-sdk-msbuild-vs)

| Wo? | Wofür? |
| --- | --- |
| `src/FiaeTrainer.Core/Content/lessons.json` | Aufgaben, verbundene Story-/Notiztexte, drei Hinweise und Musterlösungen |
| `src/FiaeTrainer.Core/Content/database.sql` | Versionierte synthetische Fall- und Prüfdaten |
| `src/FiaeTrainer.Web/UpdateChecker.cs` | Nicht blockierende öffentliche GitHub-Releaseprüfung |
| `src/FiaeTrainer.Web/LocalSettingsStore.cs` | Geprüfte individuelle SQL-Verbindung und persönliche Updatewahl |
| `src/FiaeTrainer.Web/LocalReleaseConfiguration.cs` | Benutzerordner und Konfigurationsvorrang der Windows-EXE |
| `Release-erstellen.ps1` | Windows-Paket, Ausschluss privater Dateien, Hinweise und SHA256 |
| `src/FiaeTrainer.Core/SqlQueryValidator.cs` | T-SQL-Syntax und erlaubter Befehlsumfang |
| `src/FiaeTrainer.Core/SqlWritingRules.cs` | Trainingsregeln für Großschreibung und Semikolon je Anweisung |
| `src/FiaeTrainer.Core/SqlSyntaxFeedback.cs` | Konkrete Syntaxhinweise mit SQL-Ausschnitten |
| `src/FiaeTrainer.Core/TrainingDatabase.cs` | Datenbankanlage und eingeschränkte Abfrageausführung |
| `src/FiaeTrainer.Core/QueryEvaluator.cs` | Vergleich mit beiden Referenzergebnissen |
| `src/FiaeTrainer.Core/ResultComparer.cs` | Datentypen, Duplikate und Reihenfolge |
| `src/FiaeTrainer.Core/Content/workshop.json` | Zehn deutsche Werkstattaufgaben mit Lösungen und Hilfen |
| `src/FiaeTrainer.Core/WorkshopSqlValidator.cs` | Erlaubte Schreibsprache und Umleitung auf Arbeitskopien |
| `src/FiaeTrainer.Core/TrainingDatabase.Workshop.cs` | Sitzungsisolation, Zwischenstände und Constraint-Prüfungen |
| `src/FiaeTrainer.Core/WorkshopEvaluator.cs` | Vergleich vollständiger Endzustände auf zwei Startdatenbeständen |
| `src/FiaeTrainer.Web/Components/Pages/Workshop.razor` und `.razor.cs` | Oberfläche und Ablauf der Schreibübungen |
| `src/FiaeTrainer.Web/Components/Pages/Sql.razor` und `.razor.cs` | Oberfläche und Ablauf einer Aufgabe |
| `src/FiaeTrainer.Web/ProgressStore.cs` | Dateiablage des Lernfortschritts |
| `src/FiaeTrainer.Web/Components/SelfTestHelp.razor` | Ausblendbarer allgemeiner Bedienhinweis und Wiederanzeige |
| `src/FiaeTrainer.Web/NetworkAccessPolicy.cs` | Zugriffsgrenzen für lokalen Modus und ausdrücklich aktivierte Heimnetz-Vorführung |
| `src/FiaeTrainer.Web/wwwroot/app.css` | Gestaltung und Bildschirmgrößen |
| `src/FiaeTrainer.Web/wwwroot/theme.js` | Systempräferenz, Designwahl und Präferenzcookie |

Gute erste Haltepunkte: `QueryEvaluator.EvaluateAsync`, `SqlQueryValidator.Validate` und `Sql.EvaluateAsync`. UI, Lerntexte und Fachlogik sind getrennt. Inhalte sind eingebettete Ressourcen; nach einer JSON-Änderung muss neu gebaut werden. Neue Datenbankinhalte benötigen eine gezielte Versionierung; vorhandene Trainingsdaten werden beim Neustart absichtlich nicht überschrieben.

## Prüfen

```powershell
dotnet build FIAE_AP2_Trainer.sln
dotnet test FIAE_AP2_Trainer.sln
```

Der zweite Befehl führt die Unit-Tests aus. Tests gegen den echten SQL Server werden ausdrücklich eingeschaltet:

```powershell
$env:FIAE_SQL_TESTS = '1'
dotnet test FIAE_AP2_Trainer.sln
```

Eine andere Verbindung kann ausschließlich für diese Tests über `FIAE_SQL_CONNECTION` gesetzt werden; als Datenbank bleibt `FIAE_AP2_Training` vorgeschrieben. Die SQL-Tests prüfen unter anderem alle 30 Lösungen auf beiden Datenbeständen und zurückgewiesene Schreibversuche. Sie sind nur für die synthetische Trainingsdatenbank vorgesehen. Testdetails stehen im [Testprotokoll](docs/testprotokoll.md).

Die sechs unabhängigen Browserlogik-Tests für die Designwahl benötigen nur Node.js und keine zusätzlichen Pakete:

```powershell
node --test tests/theme.test.cjs
```

Sie prüfen automatische Systemwahl, manuelle Überschreibung, Cookie-Persistenz, Rückkehr zu System, ungültige/gesperrte Cookies, den neu eingesetzten Blazor-Kopfbereich und Secure auf HTTPS.

## Umfang dieser Version

Interaktiv enthalten sind lesende SQL-Abfragen: SELECT, WHERE, logische Bedingungen, Sortierung, TOP, BETWEEN, LIKE, IN, NULL, DISTINCT, INNER/LEFT JOIN, mehrere JOINs, Aggregation, GROUP BY, HAVING, Unterabfragen, EXISTS und NOT EXISTS.

Die Werkstatt ergänzt zehn Aufgaben zu Datenänderungen, CREATE TABLE, ALTER TABLE ADD, einfachen Constraints und Transaktionen. Berechtigungen, Fremdschlüssel als ausführbare Strukturaufgabe, UNION und weitere SQL-Themen fehlen noch. Pseudocode, UML und die übrigen Module sind auf der Übersicht als **In Vorbereitung** gekennzeichnet. Die App ist deshalb noch kein vollständiger AP2-Trainer. Der genaue Zukunftsumfang steht im [Projektsteckbrief](docs/projektsteckbrief.md).

## Wenn etwas nicht funktioniert

- **Port 5080 ist belegt:** Häufig läuft der Trainer bereits. Öffne die Adresse. Andere Prozesse werden vom Startskript nicht beendet.
- **Datenbank nicht erreichbar:** SQL-Instanz, Windows-Anmeldung und Instanznamen prüfen. Über die Übersicht erneut verbinden. Einzelheiten stehen im Konsolenfenster.
- **Lernstand beschädigt:** Anwendung beenden und `progress.json` zuerst sichern. Anschließend die beschädigte Datei aus dem Fortschrittsordner verschieben. Der nächste Start beginnt mit einem leeren Profil. Die App überschreibt unlesbare Lernstände nicht stillschweigend.
- **Fenster meldet eine unterbrochene Verbindung:** Trainer gegebenenfalls neu starten und Browserseite neu laden.
- **Pakete lassen sich nicht laden:** Das projektlokale `NuGet.Config` verwendet ausschließlich nuget.org, keine Firmenpakete oder Firmenanmeldungen.

Weitere Grundlagen: [Coding Guideline](docs/coding-guideline.md), [Entwicklungsprotokoll](docs/entwicklungsprotokoll.md), [Lernkonzept](docs/lernkonzept.md).

## Lizenz

Das Projekt steht unter der [MIT-Lizenz](LICENSE). Das Windows-Paket enthält zusätzlich DRITTANBIETER.md und den Ordner Lizenzen mit den Hinweisen der Abhängigkeiten und der eingebauten Laufzeiten.
