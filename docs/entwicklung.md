# Entwicklung und Projektstruktur

Quellstand und zuletzt gebautes Windows-Paket: 0.1.1-alpha.7. Das Paket wurde ausdrücklich aus der aktuellen Arbeitsfassung mit noch uncommitteten Änderungen gebaut und aus dem ZIP geprüft. `b590044` ist nur der zugrunde liegende ältere Commit, nicht der genaue Anwendungsstand dieses Pakets. Vor einem Release-Tag müssen die aktuellen Änderungen committed und gepusht werden. Diese Anleitung dient dem Projektinhaber zur Pflege und Erweiterung. Auch ein Zugriff auf die Dateien räumt keine Open-Source-Lizenz ein; es gelten die [Nutzungsbedingungen](../LICENSE).

## Orientierung

```text
src/
  FiaeTrainer.Core/
    Algorithms/
      Content/          30 JSON-Aufgaben, Modelle und Katalog
      Syntax/           Tokenisierung und begrenzte deutsche Grammatik
      Execution/        Isolierter Interpreter und Ablaufspur
      Evaluation/       Code-, Ausgabe- und Testfallprüfung
    Sql/
      Content/          JSON-Aufgaben, SQL-Startdaten, Ressourcenloader
      Database/         Datenbankanlage und SQL-Ausführung
      Investigation/    Lesender Aufgabenkatalog und Bewertung
      Results/          Ergebnistabellen und Wertevergleich
      Validation/       Syntax, Schreibregeln und Zugriffsgrenzen
      Workshop/         Schreibübungen, Modelle und Bewertung
      Lesson.cs         Gemeinsames Aufgabenmodell der SQL-Lernwege
  FiaeTrainer.Web/
    Components/
      Layout/           Kopfbereich und Seitenrahmen
      Learning/         Gemeinsame Lernbedienung
      Pages/Training/   SQL- und Algorithmus-Aufgabenseiten samt Code-behind
      Sql/              SQL-Ergebnisse, Fallnotizen und Tabellenübersicht
      Updates/          Updatehinweis
    Configuration/      Persönliche Einstellungen und Release-Profil
    Learning/           Profilmodelle und Fortschrittsspeicherung
    Networking/         Zugriffskontrolle der optionalen Vorführung
    Sql/                Verbindungsstatus und lokale Instanzerkennung
    Updates/            Version, GitHub-Quelle und Releaseprüfung
    Program.cs          Start und Dienstregistrierung
tests/
  FiaeTrainer.Tests/    Fach-, Integrations- und Komponentenprüfungen
  theme.test.cjs        Browserlogik der Designwahl
```

Namespaces folgen den Ordnern. Fachliche SQL-Typen liegen unter `FiaeTrainer.Core.Sql.*`, allgemeine Webdienste beispielsweise unter `FiaeTrainer.Web.Learning`. Öffentliche Typen haben eigene Dateien. Die beiden Teile von `TrainingDatabase` liegen gemeinsam in `Sql/Database`.

## Einstieg ins Debuggen

Die Projektmappe `FIAE_AP2_Trainer.sln` in Visual Studio 2026 öffnen und `FiaeTrainer.Web` als Startprojekt wählen. Das .NET-10-SDK ist erforderlich. [Microsoft: SDK-/Visual-Studio-Kompatibilität](https://learn.microsoft.com/en-us/dotnet/core/porting/versioning-sdk-msbuild-vs).

| Aufgabe | Einstieg |
| --- | --- |
| SQL-Abgabe verfolgen | `Components/Pages/Training/Sql.razor.cs` → `QueryEvaluator.EvaluateAsync` |
| Pseudocode-Abgabe verfolgen | `Components/Pages/Training/Algorithms.razor.cs` → `AlgorithmEvaluator.Evaluate` |
| Pseudocode schrittweise ausführen | `Algorithms/Execution/AlgorithmInterpreter.cs` im Core-Projekt |
| Funktionsaufgabe bewerten | `AlgorithmLesson.FunctionName` / `FunctionInputs` → `AlgorithmInterpreter.RunFunction` |
| SQL-Zulässigkeit prüfen | `Sql/Validation/SqlQueryValidator.cs` im Core-Projekt |
| Schreibskript bewerten | `Sql/Workshop/WorkshopEvaluator.cs` im Core-Projekt |
| Verbindung einrichten | `Sql/DatabaseState.cs` und `Configuration/LocalSettingsStore.cs` im Webprojekt |
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

`Algorithms` ist als zweites Modul neben `Sql` umgesetzt. SQL-Parser und SQL-Datenbankzugriff bleiben im SQL-Modul. Die Algorithmus-Inhalte liegen eingebettet unter `Core/Algorithms/Content/lessons.json`; fachliche Referenzen und unabhängig festgelegte Sollausgaben werden automatisiert geprüft. `AlgorithmProgress` verwendet einen eigenen Entwurf statt der SQL-Eigenschaft `Sql`. Ältere JSON-Profile erhalten den fehlenden Bereich `AlgorithmLessons` mit leeren Vorgaben; `LastAlgorithmLessonId` startet bei 1. Speichern hält alle drei Bereiche und Einstellungen unter derselben Dateisperre zusammen. Details und Grenzen: [Pseudocode](pseudocode.md).

Sichtbare Pseudocode-Zuweisungen verwenden bevorzugt `=`, Gleichheitsvergleiche `==`. Die alten Zuweisungsvarianten bleiben im Parser zulässig; gespeicherte Nutzerentwürfe werden bei der Inhaltsänderung nicht automatisch ersetzt. Jede Änderung eingebetteter Aufgaben benötigt einen Neubau.

`AlgorithmSyntax.KEYWORDS` enthält die reservierten Wörter für Tokenizer und Variablennamenprüfung. Bereits beim Tokenisieren werden kleine oder gemischte Schlüsselwörter mit Zeile und großer Ersatzschreibweise zurückgewiesen; Kommentare und Textliterale durchlaufen diese Prüfung nicht. Vorschau und Abgabe verwenden denselben Interpreter.

Der Parser liefert Hauptanweisungen und Funktionsdefinitionen getrennt. Die Machine registriert alle Definitionen vor der Ausführung. Listen haben durch `CopyValue` konsistente Wertsemantik; Indexzuweisungen ersetzen Einträge einer geprüften Kopie und können keine zyklischen Aliase herstellen. `Invoke` wertet mit eigenem Variablenbereich aus und stellt den Aufrufer auch im Fehlerfall wieder her. Nicht-null-Rückgabewerte von `Execute` beenden den Aufruf und werden durch Bedingungen/Schleifen nach oben weitergereicht; null bedeutet ausschließlich „keine Rückgabe“. Mehr als 16 gleichzeitig aktive Aufrufebenen werden abgefangen; das gilt auch für Rekursion. Alle Aufrufe teilen Rechen-, Ausgabe- und Tracebudgets.

Funktionsaufgaben definieren `FunctionName` und die Eingabereihenfolge `FunctionInputs`. Der Katalog prüft deren Zuordnung zu Beispielen und Tests. Bewertung und Vorschau verwenden `RunFunction`; Hauptanweisungen sind in diesem Weg unzulässig. Der Prüfaufruf macht den Rückgabewert als Ergebnis vergleichbar, ohne einen erfundenen AUSGABE-Schritt in die Spur einzufügen. `AlgorithmStep.Scope` nennt Hauptablauf oder aktive Funktion. Das UI zeigt den Beispiel-Rückgabewert separat. Andere Parameternamen und eigene Hilfsfunktionen bleiben erlaubt.

Die bisherigen IDs und das JSON-Profilformat bleiben stabil. Neue Aufgaben benötigen keine Migration; Lernplan und Themenkarte verwenden Katalogzahlen und umfassen jetzt 70 Aufgaben. Die Endphase der Geschichte ist an Aufgabe 30 gebunden. Prüffälle enthalten ausdrücklich unabhängige Sollausgaben; generierte Such-/Sortierfälle vergleichen zusätzlich mit Bibliotheksergebnissen. Laufzeitklassen und konkrete Algorithmusidentität beliebiger Lösungen werden nicht benotet; Aufgabe 26 zählt eine konkrete innere Anweisung bei zwei Eingabegrößen.

Rekursion ist einschließlich indirekter Aufrufe bis zur gemeinsamen Grenze von 16 gleichzeitig aktiven Aufrufen zugelassen. `AlgorithmStep.CallDepth` ergänzt den Namen des aktiven Bereichs, damit gleiche Funktionsnamen auf verschiedenen Ebenen unterscheidbar bleiben. Das Rechenbudget wird auch bei verzweigter Rekursion nicht zurückgesetzt. Testfallaufgaben verwenden `CaseInputName` (standardmäßig gewicht); dadurch kann die Mengenstaffel anzahl prüfen, ohne das skalare Antwortformat zu verändern. Katalogprüfung und Spickzettel bleiben dazu synchron.

`LearningNavigation` merkt die SQL-Aufgabenseite innerhalb der Browsersitzung. Der SQL-Spickzettel erhält sie zusätzlich als zurueck-Abfrageparameter. Nur die lokalen Routen /sql, /sql-werkstatt und ihre positiven numerischen Aufgabenpfade sind erlaubt; externe Adressen und andere Zielseiten werden verworfen. Der Parameter wird auch nach einem Neuladen berücksichtigt. Die Links im geführten Hilfebereich geben die genaue Aufgabennummer weiter, die Kopfnavigation die zuletzt besuchte SQL-Seite. Der Rückweg verändert keinen Entwurf und führt aus der Werkstatt wieder in die Werkstatt.

`Components/Learning/CodeEditor.razor` bindet den Entwurf und meldet Eingabe/Blur an die Algorithmusseite. `wwwroot/code-editor.js` hält Zeilenzählung, Scrollsynchronisation und Tab-Bearbeitung lokal am Element; ein WeakMap-Eintrag verhindert doppelte Listener nach Blazor-Rendern. Nur der Inhalt des leeren Nummern-Pre und des Statusfelds gehört JavaScript. Codezeilen brechen nicht um. Blockeinrückung bewahrt die Auswahl und das Zeichenlimit. `insertText` erhält die native Undo-Historie, bei fehlender Unterstützung greift ein Textfeld-Ersatzpfad. Esc gibt den nächsten Tab frei; der zusätzliche Shift-Tastendruck hebt diese Freigabe nicht auf. Ausgaben-/Testfallantworten verwenden weiter die normale Tab-Navigation. Kein zusätzlicher Editor- oder npm-Laufzeitbaustein erforderlich.

Gemeinsame Lernbedienung wird unter `Components/Learning` gepflegt. `MainLayout` hört auf interne Navigation und setzt den sitzungsbezogenen `LearningNavigation`-Kontext anhand fachlicher Zielpfade. Übersicht und Lernplan ändern diesen Kontext nicht. Bei einem vollständigen Neuladen einer allgemeinen Seite ist SQL wieder der Standard; direkte Algorithmus-Adressen stellen den passenden Kontext her. Es werden keine Browsercookies oder neuen Profilfelder für den Navigationskontext eingeführt. Neue gemeinsame Abstraktionen erst ergänzen, wenn ein weiteres Modul sie tatsächlich benötigt.

Die bisherigen JSON-Eigenschaften und Dateipfade für Lernstände bleiben trotz neuer Namespaces unverändert. Es werden keine .NET-Typnamen in diesen Dateien gespeichert; eine Profilmigration ist für die Umordnung nicht nötig.

## Persönliche Konfiguration

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

`Release-erstellen.ps1` hat auf ausdrücklichen Nutzerwunsch die Alpha 0.1.1-alpha.7 aus der aktuellen Arbeitsfassung erzeugt und lädt nichts hoch. Die Release-Zuordnung muss nach Commit und Push auf den neuen Anwendungsstand zeigen, nicht auf den älteren Basiscommit. Es legt `release/Anleitung.md` als kurze ANLEITUNG.md und passende Versionshinweise bei; die technische Entwickleranleitung ist nicht die Benutzeranleitung im ZIP. Die README beschreibt für Anwender ausschließlich den Release-Download. Weitere Pakete nur ausdrücklich oder am vereinbarten Entwicklungsende erstellen. Die aktuelle private Projektlizenz und unabhängige Drittanbieterlizenzen müssen mitgeliefert werden. Ein früherer MIT-Entwurf des Projekts gehört nicht in neue Pakete. Die aktuelle Paketprüfung steht im [Testprotokoll](testprotokoll.md).

Weitere Vorgaben: [Coding Guideline](coding-guideline.md), [Lernkonzept](lernkonzept.md), [Testprotokoll](testprotokoll.md).
