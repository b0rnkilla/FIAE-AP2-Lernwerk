# Coding Guideline

Konventionen für die Pflege und Erweiterung von AP2 Lernwerk mit C#, .NET und Blazor.

## Grundsatz

* Einfache Lösungen vor unnötiger Abstraktion.
* Verständlicher Code vor maximaler Wiederverwendbarkeit.
* Konkrete Umsetzung vor generischen Hilfskonstruktionen.
* Keine neue Struktur ohne klaren Nutzen.
* Keine Interfaces, Basisklassen oder Factories ohne konkreten Bedarf.

## Sprache und Namen

* Codebezeichner sind Englisch.
* Kommentare, XML-Summaries, Bedienoberfläche, Fehlermeldungen, Aufgaben und Dokumentation sind Deutsch.
* SQL-Schlüsselwörter bleiben SQL. Das fachliche Lernschema verwendet bewusst deutsche Tabellen- und Spaltennamen.
* Übliche .NET-Namenskonventionen: private Felder `_camelCase`, öffentliche Typen und Member `PascalCase`, Interfaces mit `I`.
* Konstanten verwenden abweichend `ALL_CAPS`.

## Dokumentation

Bei jedem Änderungspaket README, Entwicklungsprotokoll, Projektsteckbrief, Testprotokoll und fachlich betroffene weitere Markdown-Dateien auf den aktuellen Stand bringen. Umgesetzte Änderungen, tatsächlich durchgeführte Prüfungen und offene Punkte klar trennen. docs/verbesserungsplan.md hält die erledigten Nutzerpunkte und die lokale Release-Vorbereitung nach; AGENTS.md verankert diese Vorgabe für weitere Arbeiten.

Summaries dokumentieren Klassen, Felder, Properties, Methoden, Enums, Services und Events. Private Methoden benötigen ein Summary bei nicht offensichtlichem Verhalten. Kommentare erklären Entscheidungen oder gliedern größere Abschnitte; sie wiederholen nicht den Code.

```csharp
/// <summary> Prüft die Abfrage gegen den zweiten Datenbestand. </summary>
```

Mehrzeilige Summaries verwenden bei einem beabsichtigten Zeilenumbruch `<br/>`. `<param>`, `<returns>` und `<exception>` werden nur ergänzt, wenn sie hilfreiche Informationen zu Einschränkungen, Seiteneffekten oder Fehlerverträgen vermitteln. Framework-Lifecycle-Methoden mit eindeutigem Namen und reine Razor-Markup-Abschnitte benötigen keine redundanten Kommentare.

## Formatierung

* Größere Klassen werden mit Regions in der Reihenfolge `Fields`, `Properties`, `Constructor`, `Methods`, `Events` strukturiert. Keine leeren Regions; Leerzeile nach Beginn und vor Ende.
* Private Elemente stehen innerhalb ihrer fachlichen Gruppe vor öffentlichen Elementen.
* Einfache Anweisungen bleiben einzeilig. Umbrüche sollen Struktur und Lesbarkeit verbessern, nicht nur eine feste Zeilenlänge einhalten.
* Einzeilige übersichtliche Methoden und Properties verwenden `=>`.
* `if` mit genau einer Anweisung benötigt keine geschweiften Klammern.
* LINQ bleibt überschaubar; komplexe Verarbeitung wird in verständliche Schritte zerlegt.
* CSS wird nach Bereich und Bildschirmgröße gegliedert, nicht als unlesbarer komprimierter Quelltext gepflegt.

## Bevorzugte Sprachfeatures

Primary Constructors für einfache Abhängigkeitsübernahme, File Scoped Namespaces, übersichtliches Pattern Matching, Collection Expressions, Nullable Reference Types und Async/Await sind erwünscht.

Asynchrone Methoden erhalten das Suffix `Async`. Keine blockierenden `.Result`-/`.Wait()`-Aufrufe und kein `async void` außerhalb echter Eventhandler. Der Null-Forgiving-Operator ist nur begründet erlaubt; bei Blazor-injizierten Properties wird `default!` verwendet, weil die Laufzeit diese vor dem Komponentenaufruf setzt.

## Zuständigkeiten

* Core: Lernmodelle, Inhaltskatalog, Syntaxprüfung, Datenbankzugriff und fachliche Bewertung.
* Web: Blazor-Oberfläche, UI-Ablauf, Konfiguration und lokale Fortschrittsspeicherung.
* Tests: unabhängige Ergebnisbeispiele, Fehlerfälle und Prüfungen gegen die echte Trainingsdatenbank.

SQL-Texte werden nicht mit frei gewählten Datenbanknamen oder Zugriffen auf bestehende Nutzdaten vermischt. Normale falsche Antworten sind Ergebniswerte, keine Exceptions. Technische IO-/Datenbankfehler werden kontrolliert behandelt und verständlich gemeldet.

## Lerninhalte und Erweiterungen

Jede SQL-Aufgabe benötigt einen Auftrag mit eindeutiger Spaltenreihenfolge, drei freiwillige Hinweise, eine ausführbare Referenzlösung und eine fachliche Erklärung. Alternative korrekte SQL-Formulierungen dürfen nicht wegen abweichender Aliase oder Formatierung scheitern. Duplikate, NULL und Sortierung müssen bewusst behandelt werden.

Neue SQL-Musterlösungen verwenden große Schlüsselwörter und Funktionen, Semikolon je äußerer Anweisung, kurze Tabellennamen und Datums-/Zeitwerte als YYYY-MM-DD HH:mm:ss. Story und Entdeckungen dürfen Ergebnisse anderer ungelöster Schritte nicht als bereits bestätigt voraussetzen.

Neue Inhalts- oder Bewertungsänderungen werden mit geeigneten Tests geprüft. Eine reine Ergebnisgleichheit auf zwei Datensätzen ist keine vollständige semantische SQL-Analyse. Der begrenzte Umfang wird transparent dokumentiert.

Keine echten Firmen-, Kunden- oder Personendaten übernehmen. Keine Zugangsdaten versionieren. Keine kostenpflichtige KI-Anbindung ohne gesonderte Entscheidung des Nutzers einbauen.

Algorithmus-Aufgaben liegen getrennt von SQL unter `Core/Algorithms`. Jede benötigt einen eindeutigen Auftrag, drei Hinweise, eine geprüfte Referenz und unabhängig festgelegte Sollausgaben. Freie Lösungen anhand ihrer Ausführung beurteilen, nicht anhand der Übereinstimmung des Codewortlauts. Unterstützte Pseudocode-Syntax und Ausführungsgrenzen in `docs/pseudocode.md` und im App-Spickzettel gemeinsam pflegen. Im Profil eigene Entwürfe und Erfolge führen; keine Pseudocode-Eingaben in SQL-Feldern speichern.

Pseudocode-Beispiele, Hinweise, Reparaturvorgaben und Musterlösungen verwenden bevorzugt `=` für Zuweisungen und `==` für Gleichheitsvergleiche. Die zusätzlich akzeptierten Zuweisungszeichen bleiben für vorhandene Entwürfe erhalten. Das vorgelegte alte Prüfungsbeispiel dient als Orientierung für diese Schreibweise; dessen Aufgabe wird nicht übernommen und seine gesamte Syntax ist damit nicht automatisch Bestandteil des Interpreters.

## Ordner und Namespaces

Fachlogik wird nach Lernmodul gruppiert, derzeit unter Core/Sql. Ermittlung, Werkstatt, Validierung, Ergebnisse, Datenbankzugriff und Inhalte erhalten dort eigene Ordner. Allgemeine Webdienste liegen unter Configuration, Learning, Networking und Updates; SQL-spezifische Webdienste unter Sql. Namespaces folgen dem Ordnerpfad. Öffentliche Typen erhalten eigene Dateien; zusammengehörige Teile einer partial class bleiben im selben Ordner. Program.cs bleibt als Einstieg im Web-Projektroot. Neue Lernmodule nicht in den SQL-Bereich einordnen. Die konkrete Struktur steht in entwicklung.md.
