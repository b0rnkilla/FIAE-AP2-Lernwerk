# Entwicklung und Projektstruktur

Quellstand: **0.1.1-alpha.13 mit Abschnitt 03 UML und Modellierung**, 298 Lernübungen plus 25 SQL-/Pseudocode-Prüfungsaufgaben, drei Entwurfsprüfungen und drei UML-Prüfungsfälle mit jeweils neun Teilaufgaben je Modul. Zuletzt gebautes Windows-Paket: **0.1.1-alpha.12 mit 250 Lernübungen und den separaten Prüfungsrunden**, am 06.11.2026 aus sauberem, gepushtem Commit `e3af6aa` gebaut und aus dem ZIP geprüft. Anwendungscode und Paketdokumente stammen aus diesem Commit; die Prüfdokumentation wird anschließend getrennt ergänzt. Das Pre-release wurde am 07.11.2026 auf GitHub bestätigt. Diese Anleitung dient dem Projektinhaber zur Pflege und Erweiterung. Auch ein Zugriff auf die Dateien räumt keine Open-Source-Lizenz ein; es gelten die [Nutzungsbedingungen](../LICENSE). Die Entstehung und Zuordnung älterer Pakete stehen unter [lokale Releases](lokale-releases.md).

## Orientierung

```text
src/
  FiaeTrainer.Core/
    Uml/
      Models/           Elemente, Merkmale, Verbindungen, Sequenzgruppen und Antworten
      Content/          36 eigene Lernaufgaben, drei Prüfungsfälle und Katalog
      Evaluation/       Speichergrenzen, Fachrollen und geprüfte UML-Varianten
      Practice/         Getrennte Lernversuche und 45-Minuten-Prüfungen
    DatabaseDesign/
      Models/           Tabellen, Attribute, FK-Paare und strukturierte Antworten
      Content/          42 Lernaufgaben, ER-Vorlagen/Unterlagen, drei Prüfungsfälle und Katalog
      Evaluation/       Relationale Integrität und explizite Modellvarianten
      Practice/         Individuelle Versuche und 45-Minuten-Prüfungen
    Algorithms/
      Content/          30 JSON-Aufgaben, Modelle und Katalog
      Syntax/           Tokenisierung und begrenzte deutsche Grammatik
      Execution/        Isolierter Interpreter und Ablaufspur
      Evaluation/       Code-, Ausgabe- und Testfallprüfung
      Exams/            Zehn eigene Prüfungsaufgaben, Auswahl und Teilpunkte
    BusinessStudies/
      Content/          150 eigene Fragen, acht Themen, Quellen und Katalog
      Evaluation/       Strukturierte Antworten und deterministische Teilprüfung
      Practice/         Runde, Prüfungsuhr und Aufgabenauswahl
    Exams/              Gemeinsame Prüfungsrunden, Frist und Ergebnisse
    Sql/
      Content/          JSON-Aufgaben, SQL-Startdaten, Ressourcenloader
      Database/         Datenbankanlage und SQL-Ausführung
      Investigation/    Lesender Aufgabenkatalog und Bewertung
      Results/          Ergebnistabellen und Wertevergleich
      Validation/       Syntax, Schreibregeln und Zugriffsgrenzen
      Workshop/         Schreibübungen, Modelle und Bewertung
      Exams/            Drei Prüfungsfälle, Validator und Bewertung
      Lesson.cs         Gemeinsames Aufgabenmodell der SQL-Lernwege
  FiaeTrainer.Web/
    Components/
      Layout/           Kopfbereich und Seitenrahmen
      Learning/         Gemeinsame Lernbedienung
      Project/          Dezente Unterstützungskarte und Projektverweise
      Uml/              Bedienbarer Diagrammbaukasten, SVG-Anordnung und Rückmeldungen
      DatabaseDesign/   Modellbaukasten und eigene ER-/Tabellendarstellungen
      BusinessStudies/  WiSo-Eingaben, Auflösung und eigene Abbildungen
      Exams/            Nachbereitung und Prüfungsrückweg
      Pages/Training/   Lern- und Prüfungsseiten aller fünf Module samt Code-behind
      Sql/              SQL-Ergebnisse, Fallnotizen und Tabellenübersicht
      Updates/          Updatehinweis
    Exams/              Koordination der zwei fachlichen Prüfungen
    Configuration/      Persönliche Einstellungen und Release-Profil
    Learning/           Profilmodelle und Fortschrittsspeicherung
    Networking/         Heimnetz-Einstellung, Firewall, Zugriff und interner Hostneustart
    Project/            Gemeinsame öffentliche Urheberangaben und Links
    Sql/                Verbindungsstatus und lokale Instanzerkennung
    Updates/            Version, GitHub-Quelle und Releaseprüfung
    Program.cs          Start und Dienstregistrierung
tests/
  FiaeTrainer.Tests/    Fach-, Integrations- und Komponentenprüfungen
  theme.test.cjs        Browserlogik der Designwahl
```

Namespaces folgen den Ordnern. Fachliche SQL-Typen liegen unter `FiaeTrainer.Core.Sql.*`, allgemeine Webdienste beispielsweise unter `FiaeTrainer.Web.Learning`. Öffentliche Typen haben eigene Dateien. Die beiden Teile von `TrainingDatabase` liegen gemeinsam in `Sql/Database`.

## Einstieg ins Debuggen

Für den normalen Entwicklungstest **Trainer-starten.cmd** verwenden. Der Starter baut die aktuelle Quellfassung und öffnet deren tatsächliche Browseradresse. Alternativ das Startprojekt FiaeTrainer.Web mit Profil FiaeTrainer in Visual Studio starten. Das Standardprofil erzwingt keine applicationUrl und keinen Netzwerkmodus; die Grundadresse localhost:5080 steht in appsettings.json und wird von der persönlichen Heimnetzwahl überschrieben. Der Browser wird über FIAE_OPEN_BROWSER geöffnet, nicht zusätzlich durch launchBrowser.

Trainer-im-Netzwerk-starten.cmd bleibt ausschließlich ein optionaler Entwicklerkurzweg: FiaeTrainerLan erzwingt die LAN-Bindung. Zum Testen der gespeicherten Wahl den normalen Starter nehmen. Beide CMDs sind Komfortstarter und für Visual Studio nicht erforderlich; künftige Anwender starten die veröffentlichte EXE. Die Builddatei src/FiaeTrainer.Web/bin/Release/net10.0/FiaeTrainer.Web.exe ist kein selbstständig verteilbares Release. Kein neues ZIP wurde erzeugt.

Die Projektmappe `FIAE_AP2_Trainer.sln` in Visual Studio 2026 öffnen und `FiaeTrainer.Web` als Startprojekt wählen. Das .NET-10-SDK ist erforderlich. [Microsoft: SDK-/Visual-Studio-Kompatibilität](https://learn.microsoft.com/en-us/dotnet/core/porting/versioning-sdk-msbuild-vs).

| Aufgabe | Einstieg |
| --- | --- |
| SQL-Abgabe verfolgen | `Components/Pages/Training/Sql.razor.cs` → `QueryEvaluator.EvaluateAsync` |
| Pseudocode-Abgabe verfolgen | `Components/Pages/Training/Algorithms.razor.cs` → `AlgorithmEvaluator.Evaluate` |
| Pseudocode schrittweise ausführen | `Algorithms/Execution/AlgorithmInterpreter.cs` im Core-Projekt |
| WiSo-Antwort bewerten | `BusinessStudies/Evaluation/BusinessEvaluator.cs` im Core-Projekt |
| WiSo-Runde auswählen | `BusinessStudies/Practice/BusinessRoundBuilder.cs` im Core-Projekt |
| WiSo-Runde speichern | `Learning/ProgressStore.Business.cs` im Webprojekt |
| Funktionsaufgabe bewerten | `AlgorithmLesson.FunctionName` / `FunctionInputs` → `AlgorithmInterpreter.RunFunction` |
| SQL-Zulässigkeit prüfen | `Sql/Validation/SqlQueryValidator.cs` im Core-Projekt |
| Schreibskript bewerten | `Sql/Workshop/WorkshopEvaluator.cs` im Core-Projekt |
| Verbindung einrichten | `Sql/DatabaseState.cs` und `Configuration/LocalSettingsStore.cs` im Webprojekt |
| UML-Entwurf bewerten | `Uml/Evaluation/UmlEvaluator.cs` im Core-Projekt |
| UML-Diagramm darstellen | `Components/Uml/UmlDiagram.razor` und `UmlLayout.cs` |
| UML-Lern-/Prüfungsstand speichern | `Learning/ProgressStore.Uml.cs` |
| Lernstand speichern | `Learning/ProgressStore.cs` im Webprojekt |
| Updateauswahl nachvollziehen | `Updates/UpdateChecker.cs` im Webprojekt |

## Verbindungsdialog

`Components/Layout/ReconnectModal.razor` liegt außerhalb des interaktiven Lernbereichs. `wwwroot/reconnect.js` reagiert auf Blazors `components-reconnect-state-changed` und bedient Wiederverbinden, Fortsetzen und ausdrückliches Neuladen auch ohne aktive .NET-Verbindung. Dialog und Fehlerleiste verwenden semantische Farben aus `app.css`. Ein Neuladen erfolgt ausschließlich auf Klick, damit ungesicherte Eingaben nicht automatisch verloren gehen. Browserprüfungen müssen einen separaten QA-Server unterbrechen, nie die normale Lerninstanz.

## Inhalte und spätere Module

### Aufgabenwechsel und Hilfebereich

Die SQL-Seiten zeigen den ersten Ladezustand weiterhin als eigene Seite. Bei späteren asynchronen Aufgabenwechseln bleiben Workspace und TaskRail jedoch im DOM; nur der Hauptinhalt zeigt vorübergehend den Ladehinweis. Dadurch bleibt derselbe scrollende Navigationsknoten mit seiner Position erhalten. Es gibt dafür keinen zusätzlichen JavaScript-Speicher und keine neue Profileigenschaft. Ein vollständiges Neuladen oder der Wechsel zu einem anderen Lernbereich erzeugt weiterhin eine neue Ansicht.

Hinweise, Spickzettel und Musterlösung erscheinen ausschließlich im geführten Hilfebereich beider SQL-Lernwege als gleich aufgebaute `.help-row`: links Überschrift und Beschreibung, rechts die Aktion. Der Spickzettel verwendet einen normalen internen Link ohne `target` und öffnet im selben Tab. Die vorhandene Entwurfsspeicherung erhält den Text bei Navigation und Rückkehr. `_showSolution` hält die momentane Anzeige getrennt von `LessonProgress.SolutionViewed`; die Aktionen „Ansehen“ und „Ausblenden“ setzen die gespeicherte Hilfenutzung nicht zurück. Die bisherigen JSON-Eigenschaften und Bewertungsregeln bleiben unverändert.

### Lerninhalte

Die SQL-Aufgaben liegen unter `Core/Sql/Content` in `lessons.json` und `workshop.json`. SQL-Dateien enthalten nur synthetische Startdaten. Alle vier Inhaltsdateien werden eingebettet; `SqlContent` lädt sie aus der Assembly. Nach Änderungen neu bauen. Eine bestehende Trainingsdatenbank wird beim Neustart absichtlich nicht neu befüllt; Änderungen am Datenbestand brauchen eine ausdrücklich geplante Versionierung.

`Algorithms` ist als zweites Modul neben `Sql` umgesetzt. SQL-Parser und SQL-Datenbankzugriff bleiben im SQL-Modul. Die Algorithmus-Inhalte liegen eingebettet unter `Core/Algorithms/Content/lessons.json`; fachliche Referenzen und unabhängig festgelegte Sollausgaben werden automatisiert geprüft. `AlgorithmProgress` verwendet einen eigenen Entwurf statt der SQL-Eigenschaft `Sql`. Ältere JSON-Profile erhalten den fehlenden Bereich `AlgorithmLessons` mit leeren Vorgaben; `LastAlgorithmLessonId` startet bei 1. Speichern hält die getrennten Lernbereiche und Einstellungen unter derselben Dateisperre zusammen. Details und Grenzen: [Pseudocode](pseudocode.md).

Sichtbare Pseudocode-Zuweisungen verwenden bevorzugt `=`, Gleichheitsvergleiche `==`. Die alten Zuweisungsvarianten bleiben im Parser zulässig; gespeicherte Nutzerentwürfe werden bei der Inhaltsänderung nicht automatisch ersetzt. Jede Änderung eingebetteter Aufgaben benötigt einen Neubau.

`AlgorithmSyntax.KEYWORDS` enthält die reservierten Wörter für Tokenizer und Variablennamenprüfung. Bereits beim Tokenisieren werden kleine oder gemischte Schlüsselwörter mit Zeile und großer Ersatzschreibweise zurückgewiesen; Kommentare und Textliterale durchlaufen diese Prüfung nicht. Vorschau und Abgabe verwenden denselben Interpreter.

Der Parser liefert Hauptanweisungen und Funktionsdefinitionen getrennt. Die Machine registriert alle Definitionen vor der Ausführung. Listen haben durch `CopyValue` konsistente Wertsemantik; Indexzuweisungen ersetzen Einträge einer geprüften Kopie und können keine zyklischen Aliase herstellen. `Invoke` wertet mit eigenem Variablenbereich aus und stellt den Aufrufer auch im Fehlerfall wieder her. Nicht-null-Rückgabewerte von `Execute` beenden den Aufruf und werden durch Bedingungen/Schleifen nach oben weitergereicht; null bedeutet ausschließlich „keine Rückgabe“. Mehr als 16 gleichzeitig aktive Aufrufebenen werden abgefangen; das gilt auch für Rekursion. Alle Aufrufe teilen Rechen-, Ausgabe- und Tracebudgets.

Funktionsaufgaben definieren `FunctionName` und die Eingabereihenfolge `FunctionInputs`. Der Katalog prüft deren Zuordnung zu Beispielen und Tests. Bewertung und Vorschau verwenden `RunFunction`; Hauptanweisungen sind in diesem Weg unzulässig. Der Prüfaufruf macht den Rückgabewert als Ergebnis vergleichbar, ohne einen erfundenen AUSGABE-Schritt in die Spur einzufügen. `AlgorithmStep.Scope` nennt Hauptablauf oder aktive Funktion. Das UI zeigt den Beispiel-Rückgabewert separat. Andere Parameternamen und eigene Hilfsfunktionen bleiben erlaubt.

Die bisherigen IDs und das JSON-Profilformat bleiben stabil. Neue Aufgaben benötigen keine Migration; Lernplan und Themenkarte verwenden Katalogzahlen: SQL und Algorithmen umfassen 70, zusammen mit WiSo 220 und seit Alpha 11 mit Datenbankentwurf 250 und mit dem Diagrammausbau der aktuellen Alpha-12-Quellfassung 262 Lernaufgaben. Die Endphase der Algorithmusgeschichte ist an Aufgabe 30 gebunden. Prüffälle enthalten ausdrücklich unabhängige Sollausgaben; generierte Such-/Sortierfälle vergleichen zusätzlich mit Bibliotheksergebnissen. Laufzeitklassen und konkrete Algorithmusidentität beliebiger Lösungen werden nicht benotet; Aufgabe 26 zählt eine konkrete innere Anweisung bei zwei Eingabegrößen.

Rekursion ist einschließlich indirekter Aufrufe bis zur gemeinsamen Grenze von 16 gleichzeitig aktiven Aufrufen zugelassen. `AlgorithmStep.CallDepth` ergänzt den Namen des aktiven Bereichs, damit gleiche Funktionsnamen auf verschiedenen Ebenen unterscheidbar bleiben. Das Rechenbudget wird auch bei verzweigter Rekursion nicht zurückgesetzt. Testfallaufgaben verwenden `CaseInputName` (standardmäßig gewicht); dadurch kann die Mengenstaffel anzahl prüfen, ohne das skalare Antwortformat zu verändern. Katalogprüfung und Spickzettel bleiben dazu synchron.

`LearningNavigation` merkt die SQL-Aufgabenseite innerhalb der Browsersitzung. Der SQL-Spickzettel erhält sie zusätzlich als zurueck-Abfrageparameter. Nur die lokalen Routen /sql, /sql-werkstatt und ihre positiven numerischen Aufgabenpfade sind erlaubt; externe Adressen und andere Zielseiten werden verworfen. Der Parameter wird auch nach einem Neuladen berücksichtigt. Die Links im geführten Hilfebereich geben die genaue Aufgabennummer weiter, die Kopfnavigation die zuletzt besuchte SQL-Seite. Der Rückweg verändert keinen Entwurf und führt aus der Werkstatt wieder in die Werkstatt.

`Components/Learning/CodeEditor.razor` bindet den Entwurf und meldet Eingabe/Blur an die Algorithmusseite. `wwwroot/code-editor.js` hält Zeilenzählung, Scrollsynchronisation und Tab-Bearbeitung lokal am Element; ein WeakMap-Eintrag verhindert doppelte Listener nach Blazor-Rendern. Nur der Inhalt des leeren Nummern-Pre und des Statusfelds gehört JavaScript. Codezeilen brechen nicht um. Blockeinrückung bewahrt die Auswahl und das Zeichenlimit. `insertText` erhält die native Undo-Historie, bei fehlender Unterstützung greift ein Textfeld-Ersatzpfad. Esc gibt den nächsten Tab frei; der zusätzliche Shift-Tastendruck hebt diese Freigabe nicht auf. Ausgaben-/Testfallantworten verwenden weiter die normale Tab-Navigation. Kein zusätzlicher Editor- oder npm-Laufzeitbaustein erforderlich.

Gemeinsame Lernbedienung wird unter `Components/Learning` gepflegt. `MainLayout` hört auf interne Navigation und setzt den sitzungsbezogenen `LearningNavigation`-Kontext anhand fachlicher Zielpfade. Übersicht und Lernplan ändern diesen Kontext nicht. Bei einem vollständigen Neuladen einer allgemeinen Seite ist SQL wieder der Standard; direkte Algorithmus-Adressen stellen den passenden Kontext her. Es werden keine Browsercookies oder neuen Profilfelder für den Navigationskontext eingeführt. Neue gemeinsame Abstraktionen erst ergänzen, wenn ein weiteres Modul sie tatsächlich benötigt.

Die bisherigen JSON-Eigenschaften und Dateipfade für Lernstände bleiben trotz neuer Namespaces unverändert. Es werden keine .NET-Typnamen in diesen Dateien gespeichert; eine Profilmigration ist für die Umordnung nicht nötig.

## WiSo-Modul

`BusinessCatalog` lädt `questions.json`, `topics.json` und `sources.json` als eingebettete Ressourcen. `BusinessEvaluator` prüft Auswahlmengen und Reihenfolgen vollständig sowie Ergebnisfelder einzeln. Es gibt keine SQL- oder KI-Abhängigkeit. Format-, Punkte- und Quellenregeln stehen in [WiSo](wiso.md).

Die Seiten `/wiso`, `/wiso/runde` und `/wiso/spickzettel` verwenden denselben aktuellen Rundenstand. `BusinessRoundBuilder` wählt Aufgaben und mischt Optionen anhand von Runde und stabiler Kennung reproduzierbar. Die Webseite erzeugt keine eigene zweite Bewertungslogik. Die UTC-Frist der Prüfungsrunde wird aus `RunningSinceUtc` und `RemainingSeconds` berechnet; ein UI-Timer aktualisiert nur die Anzeige und wird auch nach asynchronem Laden initialisiert. Das Speichern prüft eine abgelaufene Frist zusätzlich unabhängig vom UI-Tick.

`TrainingProgress` ergänzt `BusinessQuestions`, `BusinessRound` und `BusinessHistory`, ohne bestehende Bereiche zu ersetzen. `ProgressStore.Business.cs` verwendet dieselbe Dateisperre und atomare Dateiersetzung wie SQL und Algorithmen. Runde und Revisionsnummer schützen gegen veraltete Browserfenster. Bereits geprüfte Antworten bleiben gesperrt; doppelte Abgabe erhöht keine Versuchszahl. Abgeschlossene Ergebnisse bleiben auf 20 Runden begrenzt. Alte Profile ohne WiSo-Eigenschaften bekommen leere Vorgaben; fehlerhafte vorhandene Daten werden abgewiesen.

Die Feld-Eingabe übernimmt Tastendrücke über `oninput` sofort in das Antwortmodell. `blur` und nachfolgende Aktionen speichern. Optionen und Sortierpfeile speichern unmittelbar nach einer Änderung. Komponenten und Quellen verwenden keine externen Laufzeitbausteine. Beim Pflegen der Aufgaben ContentVersion, Quellenstand, Spickzettel und unabhängige Testwerte gemeinsam prüfen. Eine inkompatible offene Runde erhält einen erklärten Neustart statt still veränderter Aufgaben.

`Business.RequestFinish` fordert nach dem Rendern den Fokus für die Abgabebestätigung an. `OnAfterRenderAsync` verwendet die `ElementReference` des aktuellen Statusbereichs und `FocusAsync` mit normalem Scrollverhalten. Eine neu abgeschlossene Runde erhält denselben Fokusauftrag für das Ergebnis. Reine Navigation innerhalb einer bereits abgeschlossenen Runde löst ihn nicht erneut aus. `tabindex="-1"` macht den Bereich programmatisch erreichbar, ohne einen zusätzlichen Tabstopp einzuführen; der sichtbare Rahmen und die beschriftete Region bleiben für Tastaturnutzer erhalten. Kein eigener Scroll-JavaScript-Helfer oder globaler Navigationswechsel nötig.

## Persönliche Konfiguration

### Projektlinks

`Project/ProjectInformation.cs` enthält die freigegebenen öffentlichen Namen und Ziele für GitHub, Issues, Releases, Nutzungsbedingungen und PayPal. `Components/Project/ProjectSupport.razor` verwendet sie auf Übersicht und Urheberseite; `MainLayout` hält Projekt- und Hilfelinks in der Fußzeile. Die Route `/ueber-das-lernwerk` ist eine allgemeine Seite und erhält den vorhandenen Lernmodulkontext. Es gibt keine neue API-, Datenbank- oder Profilabhängigkeit. README-Links und neue Screenshots zusätzlich pflegen. Architektur und Inhalte: [Projektauftritt](projektauftritt.md).

### Speicherorte

Im Quellprojekt speichert die Einrichtung unter `src/FiaeTrainer.Web/appsettings.Local.json`. Der Lernstand liegt unter `src/FiaeTrainer.Web/Data/progress/progress.json`. Beide Bereiche sind ignoriert und gehören nicht ins öffentliche Repository. Die allgemeine Verbindung in `appsettings.json` enthält kein Kennwort.

Die `.gitignore` schützt Dateipfade, durchsucht aber keine beliebigen Inhalte nach Geheimnissen und bereinigt keine bereits veröffentlichte Historie. Vor jedem Push die Änderungsliste prüfen. Quelltext und Lerninhalte enthalten ausschließlich synthetische Beispieldaten.

## Prüfen

```powershell
dotnet build FIAE_AP2_Trainer.sln
dotnet test FIAE_AP2_Trainer.sln
node --test tests/theme.test.cjs tests/reconnect.test.cjs tests/code-editor.test.cjs
```

Tests gegen einen echten SQL Server werden ausdrücklich aktiviert:

```powershell
$env:FIAE_SQL_TESTS = '1'
dotnet test FIAE_AP2_Trainer.sln
```

`FIAE_SQL_CONNECTION` erlaubt dafür eine andere Testinstanz, aber weiterhin ausschließlich die synthetische Datenbank `FIAE_AP2_Training`. Ohne Aktivierung werden diese Tests übersprungen. Für Browserprüfungen einen separaten Port, ein eigenes `Training:ProgressDirectory` und bei Einrichtungsprüfungen zusätzlich ein eigenes `LocalRelease:SettingsFile` verwenden. Den normalen Nutzerlernstand nicht für Tests ändern.

## Version und spätere Releases

Die Version steht zentral in `Directory.Build.props`. Die Updateprüfung kann nur öffentlich erreichbare Releases auswerten. Konfigurierte Quelle: `b0rnkilla/FIAE-AP2-Lernwerk`; benötigt werden ein SemVer-Tag und ein fertig hochgeladener Anhang `*-win-x64.zip`. Reine Tags und automatisch erzeugte Quellcodearchive zählen nicht. Details: [Lokale Releases](lokale-releases.md).

`Release-erstellen.ps1` hat auf ausdrücklichen Nutzerwunsch die Alpha 0.1.1-alpha.8 aus dem sauberen Commit `b4e953f` erzeugt und lädt nichts hoch. Der Tag `v0.1.1-alpha.8` gehört auf diesen inzwischen gepushten Anwendungscommit. Die nachträglich ergänzten Prüfdokumente verändern das geprüfte ZIP nicht. Es legt `release/Anleitung.md` als kurze ANLEITUNG.md und passende Versionshinweise bei; die technische Entwickleranleitung ist nicht die Benutzeranleitung im ZIP. Die README beschreibt für Anwender ausschließlich den Release-Download. Weitere Pakete nur ausdrücklich oder am vereinbarten Entwicklungsende erstellen. Die aktuelle private Projektlizenz und unabhängige Drittanbieterlizenzen müssen mitgeliefert werden. Ein früherer MIT-Entwurf des Projekts gehört nicht in neue Pakete. Die aktuelle Paketprüfung steht im [Testprotokoll](testprotokoll.md).

Weitere Vorgaben: [Coding Guideline](coding-guideline.md), [Lernkonzept](lernkonzept.md), [Testprotokoll](testprotokoll.md).

## Neue Prüfungsrunden und Aufgabenwechsel

SQL und Pseudocode besitzen separate 45-Minuten-Prüfungen. Inhalt, Teilpunkte, Routen, Profilfelder und Bewertungsgrenzen stehen unter [Prüfungsmodus](pruefungsmodus.md). Die vorhandenen 220 Lernaufgaben bleiben unverändert und werden seit Alpha 11 durch Datenbankentwurf ergänzt (ursprünglich 30, aktuell 42 Lernaufgaben); die zusätzlichen 25 SQL-/Pseudocode-Prüfungsaufgaben zählen in eigenen Ergebnissen.

Bei Listenklicks innerhalb von SQL, Werkstatt und Algorithmen merkt `task-navigation.js` ausschließlich die Gesamtseiten- und Listenposition dieses Wechsels. `TaskNavigationPosition` stellt sie nach dem fertig geladenen Zielrender wieder her. Andere Navigation, Prüfungsseiten, Browser-Zurück und WiSo-Abgaben bleiben davon getrennt. Während asynchroner Aufgabenwechsel bleibt der bisherige Inhalt für die Höhe im DOM und ist inert; `FocusOnNavigate` fokussiert auf diesen drei Lernseiten nicht mehr die Überschrift.

## Einheitliche Prüfungsinfos seit Alpha 10

Components/Exams/ExamIntroduction.razor bündelt die gemeinsam formulierten Bedienhinweise für SQL, Algorithmen und WiSo; LearningModule steuert ausschließlich die fachlichen Einleitungssätze. BusinessExamHome.razor unter /wiso/pruefung verwendet weiterhin BusinessRoundBuilder und die bestehenden BusinessRound-/BusinessHistory-Felder. Beim Aufruf wird nur gelesen. Erst die Startaktion erzeugt die 60-Minuten-Runde; bei einer offenen Lern- oder Prüfungsrunde wird deren ID erst nach Bestätigung an StartBusinessRoundAsync übergeben. Ein konkurrierend neu angelegter offener Versuch bleibt dadurch geschützt. Inhaltsrevision und Profilformat ändern sich nicht.

Die Übersicht setzt module-exam-action jeweils auf eine eigene volle Buttonzeile; die WiSo-Lernseite verwendet zwei Karten. Navigation: Übersicht, Lernplan, Modultraining, Prüfung üben und Spickzettel. Das zuletzt gebaute Windows-Paket bleibt Alpha 9. Für Alpha 10 wurden nur Quellprojekt und isolierter Browserbetrieb geprüft; kein ZIP beauftragt oder erstellt.

## Textumbrüche im Verbindungsdialog

reconnect.js setzt weiterhin ausschließlich textContent. Die sechs Zustandstexte enthalten Newlines, die CSS über #reconnect-message mit white-space: pre-line sichtbar macht; kein innerHTML erforderlich. ReconnectModal besitzt im statischen Ausgangstext ebenfalls einen Zeilenumbruch. Der Dialog bleibt unabhängig vom Blazor-Circuit bedienbar. Wiederverbindungslogik nicht für Layoutänderungen anfassen; bestehende tests/reconnect.test.cjs decken deren Verhalten ab.

## Datenbankentwurf: separater Fachbereich

Core/DatabaseDesign und Web/Components/DatabaseDesign kapseln das neue Modul. Die Seiten DatabaseDesign, DesignExamHome, DesignExam und DesignReference verbinden den Baukasten mit dem eigenen ProgressStore.Design.cs. Namen sind Darstellung; feste Attributkennungen und Spalten-/Tabellen-IDs tragen Fachbedeutung beziehungsweise Verbindungen. Keine neue NuGet-/JavaScript-Bibliothek und keine Abhängigkeit von SQL oder einer KI-Schnittstelle.

Ein gespeicherter Modellversuch besitzt Inhaltsversion und Revision. Beim Aufgabenwechsel bleiben Speichervorgänge an die zuvor erfasste Aufgabenkennung gebunden. Die atomare Profilsperre bewahrt fremde Modulfelder. Die Entwurfsprüfung speichert strukturierte Antworten und bewertet erst bei bestätigter oder erzwungener Abgabe. Details, Grenzen und Erweiterungspunkte: [Datenbankentwurf](datenbankentwurf.md).

## Heimnetz-Einstellung seit Alpha 12

`/heimnetz` wird einschließlich Routes/HeadOutlet statisch gerendert und von interaktiver Navigation ausgeschlossen. Native Formulare verwenden AntiforgeryToken; HomeNetworkEndpoints prüft zusätzlich die echte Loopback-Gegenstelle und einen lokalen Hostnamen. Weder private LAN-Adressen noch Forwarded-Header berechtigen zur Verwaltung. Externe Besucher sehen keine Verwaltungsformulare.

HomeNetworkService koordiniert die lesende Windows-Diagnose, nötigenfalls den festen erhöhten PowerShell-Helfer und SaveNetworkPreferenceAsync. Alle persönlichen Einstellungen teilen sich dieselbe atomare Dateisperre. NetworkRestart serialisiert Änderungen und beendet nach Übertragung der Antwort nur den Webhost. Program baut diesen innerhalb desselben Prozesses neu auf; Profilpfad und Port bleiben erhalten. Der Browser wartet mit home-network.js auf die bestätigte neue Betriebsart. Während des Wechsels liefert der Bereitschaftsendpunkt 503. Nur der erste Programmstart öffnet einen Browser.

Der Helfer wird als separate Inhaltsdatei neben der EXE veröffentlicht; UTF-8 **mit BOM** ist für Windows PowerShell 5.1 erforderlich. Er verändert weder Netzwerkprofile noch fremde Firewall-Regeln. Eine vorhandene passende eigene Regel vermeidet erneutes UAC; das Beenden der Freigabe benötigt keine erhöhten Rechte und lässt die eng begrenzte Regel für spätere Nutzung bestehen. Ein neuer Programmordner benötigt gegebenenfalls eine Reparatur der pfadgebundenen Regel. QRCoder 1.8.1 erzeugt PNGs vollständig lokal; seine MIT-Lizenz und Urheberhinweise werden über die vorhandene Abhängigkeitsdokumentation berücksichtigt.

Grenzen, Diagnose und Prüfnachweise: [Heimnetz](heimnetz-und-serverbetrieb.md). Kein neues Release-Paket erstellt.

## Paketnachweis Alpha 12

Die native selbstständig startbare EXE aus dem ZIP wurde ohne SDK-Aufruf auf Port 5214 betrieben. SettingsFile und ProgressDirectory lagen ausdrücklich außerhalb des entpackten Programmordners in einem eigenen synthetischen QA-Profil. Der mitgelieferte Windows-PowerShell-Helfer und QRCoder funktionieren auch im Single-File-Paket. Alle 59 Programmdateien blieben nach den Laufprüfungen bitgleich. Nicht allein aus einer allgemeinen Release-Dateiendung auf Paketportabilität schließen; der Nachweis steht in artifacts/alpha12-verification.json und im Testprotokoll. Keine Veröffentlichung durch die KI.

## 07.11.2026 – Gemeinsamer Rahmen für Datenbankentwurf

DatabaseDesign.razor verwendet TaskRail und LearningModePicker statt eigener Listen-/Modusschalter. HasOwnDraft vergleicht die strukturierte Antwort mit InitialAnswer; die gemeinsame Bestätigung löst anschließend RestartAsync aus. Die vorhandene Speichersperre, Revisionen und Unterstützungserfassung bleiben erhalten. Bestehende workspace-, mission-main-, story-note-, task-card-, editor-card- und help-card-Stile tragen die Darstellung. Nur der eingebettete Modellbaukasten erhält begrenzte zusätzliche Regeln für schmale Eingabefelder; DesignExam und task-navigation.js bleiben unverändert. Zum Übernehmen dieser kompilierten Razor-Änderung Trainer-starten.cmd neu starten; ein Neuladen des alten Prozesses genügt nicht. Kein Release-Paket erstellt.

## 07.11.2026 – Fortschrittsabhängige Lichtblick-Geschichte

DesignAssignment ergänzt PreviousContext und Discovery nach dem bestehenden Algorithmus-Vorbild. DesignCatalog verlangt vollständige Story-/Ergebnisfelder aller Lernaufgaben und Vorgängertexte ab Aufgabe 2; Prüfungsaufgaben benötigen diese Lernfelder nicht. DatabaseDesign prüft den abgeschlossenen Vorgänger vor Anzeige des Übergangs und verlinkt andernfalls den offenen Schritt. Die rechte Planungsliste verwendet nur freigeschaltete Discovery-Texte bis zur aktuellen Aufgabe; ausgelassene Schritte vor späteren Abschlüssen erhalten Links. Fachliche Explanation bleibt getrennt erhalten. Keine Profilmigration, neue Inhaltsvertragsversion oder Änderung von DesignExam nötig. Die drei neuen Komponentenprüfungen sichern offene/gelöste Vorgänger, fehlende Vorwegnahme, unveränderte gespeicherte Entwürfe und Berichtslücken einschließlich ausgeblendeter späterer Erfolge.

## 07.11.2026 – Audit des Datenbankentwurf-Lernablaufs

DatabaseDesign.razor.cs trennt die Ablaufsteuerung vom Markup. Routenparameter 0 bedeutet Fortsetzen; CurrentId und _loaded tragen die fachliche Kennung. Eine Laderevision verwirft überholte Anfragen vor Zustandsübernahme, der bisherige Hauptinhalt ist während des Ladens inert. CanEdit und die Aktionssperre verhindern doppelte Bewertungen; SaveAsync hält die ursprüngliche Aufgabenkennung fest. Moduswechsel verwerfen frühere Rückmeldungen, Erfolge bleiben in ProgressStore erhalten.

DesignLearningFeedback ist ausschließlich für Lernen, DesignFeedback ausschließlich für abgeschlossene Prüfungen zuständig. Die interne 50er-Gewichtung des Evaluators bleibt für die bestehende Prüfungsrechnung und gespeicherte Resultate erhalten; keine sichtbaren Punkte in der Lernansicht. Falsche Auswahlantworten übernehmen nicht mehr Explanation als ungefragte Lösung. DesignAnswerEditor sichert ReadOnly und Größenlimits zusätzlich in den Handlern und vergibt nach Löschungen freie Tabellennamen. 34 neue Auditfälle prüfen unter anderem alle 23 Modellaufgaben mit Umbenennung/Umordnung und unzulässigem NULL-PK, Ergebnisdarstellung, Schreibschutz, Doppelklick und gezielt verzögerten Aufgabenwechsel. Keine zusätzliche Laufzeitbibliothek oder Profilmigration.

## 07.11.2026 – Additiver Diagrammlernweg in Abschnitt 04

Aufträge 31–42 liegen unter DatabaseDesign/Content/diagram-lessons.json; Lessons verbindet sie in stabiler Reihenfolge mit dem ursprünglichen Bestand. DesignErDiagram/Entity/Relationship beschreiben eigene ER-Vorlagen, DesignDiagramSlot die begrenzten Ergänzungen, DesignEvidence synthetische Belege und Datentabellen. DesignCatalog prüft Platzhalter-/Optionszuordnung, Beziehungsendpunkte und rechteckige Unterlagen. DesignEvaluator vergleicht ER-Ergänzungen je Feld; relationale Aufträge verwenden den bisherigen Variantenprüfer.

DesignErDiagramView zeichnet eigene SVG-Elemente und eine gleichbedeutende Textliste. DesignMaterial zeigt unveränderliche Arbeitsunterlagen; DesignAnswerEditor meldet ER-Änderungen über denselben Speicherweg wie bisherige Entwürfe. DiagramValues wird additiv gespeichert. Keine neue Bibliothek, KI-Auswertung oder Profilmigration. Spickzettel-Rückwege folgen dem Katalog statt der Obergrenze 30. Prüfungsfälle sowie SQL-/Algorithmusimplementierungen bleiben unverändert.

## Abschnitt 03 seit Alpha 13

UML verwendet eigene Models/Content/Evaluation/Practice unter Core/Uml, eigene Komponenten unter Components/Uml und ProgressStore.Uml.cs. Inhaltsversion 2026-11-09, 36 Lern-IDs und drei unabhängige Prüfungsfälle. Keine SQL-Abhängigkeit, zusätzliche Bibliothek oder KI-Anbindung. Additive Profilfelder und eigene Revisionen erhalten alle bestehenden Module; Quellversion zentral in Directory.Build.props. Die Freigabe für dieses neue Modul erfolgte ausdrücklich nach der vorherigen Begrenzung auf Abschnitt 04. [Fachvertrag und Bewertungsgrenzen](uml.md).

Tests ausschließlich mit separatem Port und Training:ProgressDirectory; SQL-Integration für dieses Paket nicht erneut ausgeführt. Kein Publish/ZIP und keine Änderung der alten Archive. Die Benutzereinstiege der README beschreiben weiterhin die aktuelle App, Paketstände werden hier und unter lokale-releases.md getrennt geführt.
