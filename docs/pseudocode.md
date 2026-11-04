# Pseudocode und Algorithmen

Stand: 03.11.2026, Entwicklungsfassung 0.1.1-alpha.4. Der erste Lernweg ist im Quellprojekt umgesetzt, nicht im unveränderten Windows-ZIP 0.1.1-alpha.3.

## Lernweg und Aufgaben

„Leitstelle im Ausnahmezustand“ erzählt eine eigenständige Wiederherstellung der Einsatzplanung mit Entwicklerin Nora Seidel. Jede Aufgabe besitzt einen eigenen Auftrag, drei freiwillige Hinweise, eine Musterlösung mit Erklärung und eine erst nach erfolgreicher Abgabe sichtbare Erkenntnis. Eine vorherige Aufgabe wird nur bei vorhandenem Abschluss als bestätigt bezeichnet; der Einsatzbericht zeigt offene Zwischenstücke statt erfundener Erfolge.

| Nr. | Aufgabe | Form | Schwerpunkt |
| --- | --- | --- | --- |
| 1 | Was passt noch hinein? | Eigene Lösung | Variablen, Subtraktion, Ausgabe |
| 2 | Ein Wert verändert sich | Schreibtischtest | Zuweisungen und Ausgabereihenfolge |
| 3 | Die letzte freie Einheit | Fehlersuche | Vergleich einschließlich Gleichheit |
| 4 | Wer muss zuerst los? | Eigene Lösung | Verschachtelte Bedingungen und ODER |
| 5 | Ein Paket fehlt | Fehlersuche | Inklusive Zählschleife, Akkumulator, Nullfall |
| 6 | Wann endet der Ladevorgang? | Schreibtischtest | SOLANGE, Abbruchbedingung und Überladung |
| 7 | Die Grenze gezielt testen | Testfälle entwerfen | Ungültige Werte und Grenzfallabdeckung |
| 8 | Die nächste Ladung im Überblick | Eigene Lösung | Liste durchlaufen, filtern, zählen und summieren |

Funktionen, weitere Listenaufgaben, Suche und Sortierung sind weitere geplante Ausbauschritte. Die ursprünglich vorgeschlagenen ungefähr 24 Aufgaben sind ein Zielumfang, derzeit nicht vollständig umgesetzt.

## Bedienung

Die Reihenfolge lautet Lernmodus, Story, Aufgabe und Editor. Neue Aufgaben beginnen in „Ohne Hilfe lösen“. Der Modus bleibt je Aufgabe gespeichert. „Geführt lernen“ ergänzt Erklärungen, drei einzeln aufdeckbare Hinweise, einen internen Spickzettel und die Musterlösung mit Ansehen/Ausblenden.

„Ablauf untersuchen“ verwendet die angezeigten Beispieleingaben. Bei freien und Reparaturaufgaben wird der eigene Code ausgeführt; beim Schreibtischtest der vorgegebene Code, bei der Testfallaufgabe die Referenz der beschriebenen Gewichtsregel. Die Schrittansicht zeigt den Zustand **nach** der ausgewählten Anweisung beziehungsweise Bedingungsprüfung. Vor und zurück verändert nur den angezeigten Zustand; die bereits begrenzt berechnete Spur wird nicht erneut ausgeführt. Eingabeänderungen verwerfen veraltete Vorschau und Bewertung.

Der Spickzettel bleibt im selben Tab. Aufgabenliste, ausblendbarer Selbsttest-Hinweis und die Möglichkeit zur Wiederherstellung im Lernplan werden gemeinsam mit SQL verwendet. Der Lernplan berücksichtigt neue, begonnene und mit Unterstützung gelöste Aufgaben aller drei Lernwege. Übersicht und Lernplan stehen in der Kopfnavigation immer vorn. Danach folgen im Algorithmus-Kontext Training und Pseudocode-Spickzettel, im SQL-Kontext Ermittlung, Werkstatt und SQL-Spickzettel. Der Kontext gilt innerhalb der Browsersitzung; ein vollständiges Neuladen einer allgemeinen Seite beginnt wieder mit SQL.

## Vereinbarte Schreibweise

Pseudocode besitzt keine universell verbindliche Grammatik. Der Trainer führt eine dokumentierte Teilmenge aus; beliebige natürliche Sprache oder C#/Java-Code werden nicht automatisch bewertet.

* Eine Anweisung pro Zeile; alternativ trennt ein Semikolon Anweisungen. Einrückung ist frei. Schlüsselwörter und Variablennamen sind unabhängig von Groß-/Kleinschreibung.
* Die im Auftrag genannten Eingaben sind bereits als Variablen gesetzt. Bevorzugte Zuweisung mit `=`, zum Beispiel `rest = kapazitaet - beladen`. Beispiele, Vorgabecode, Hinweise und Referenzlösungen verwenden diese Schreibweise. `←`, `<-` und `:=` bleiben für vorhandene Entwürfe akzeptiert. Ausgabe mit `AUSGABE ausdruck`.
* Zahlen mit Dezimalpunkt, Texte in einfachen oder doppelten Anführungszeichen ohne Escape-Syntax, Wahrheitswerte `WAHR` und `FALSCH`. Kommentare ab `//` bis zum Zeilenende.
* Rechnen mit `+`, `-`, `*`, `/`, `DIV`, `MOD` beziehungsweise `%`; `DIV` schneidet zur 0 hin ab. Keine implizite Umwandlung zwischen Text und Zahl.
* Gleichheitsvergleiche bevorzugt mit `==`, zum Beispiel `WENN anzahl == 0 DANN`; ein einzelnes `=` innerhalb einer Bedingung bleibt als Vergleich zulässig. Weitere Vergleiche mit `<>`, `!=`, `<`, `<=`, `>` und `>=`. `NICHT` bezieht sich auf den folgenden Vergleich. Priorität: Klammern, unäres Vorzeichen, Punktrechnung, Strichrechnung, Vergleiche, NICHT, UND, ODER. UND und ODER werten den rechten Teil nur bei Bedarf aus.
* `WENN bedingung DANN`, optional `SONST`, abgeschlossen mit `ENDE WENN`. Verschachtelte WENN-Blöcke besitzen jeweils einen eigenen Abschluss.
* `SOLANGE bedingung`, optional `TUE`, und `ENDE SOLANGE`. Die Bedingung wird vor jedem Durchlauf geprüft.
* `FÜR i VON start BIS ende`, optional `TUE`, und `ENDE FÜR`. Ganzzahlige Grenzen einmal beim Eintritt ausgewertet, beide eingeschlossen, Schrittweite +1; bei start > ende kein Durchlauf. Die interne Steuerung ist unabhängig von einer Zuweisung an i im Rumpf. i soll nicht im Rumpf verändert werden. Nach einer nichtleeren Schleife behält i den zuletzt im Rumpf gespeicherten Wert; bei einer leeren Schleife entsteht dadurch keine neue Variable.
* `FÜR JEDES wert IN liste` und `ENDE FÜR`. `FUER` ist statt `FÜR` möglich.
* Listenliterale wie `[10, 20]`, Lesezugriff `liste[0]`, Länge `LÄNGE(liste)` oder `LAENGE(liste)`. Indexbeginn 0. Einzelne Listeneinträge zuweisen, abwärts zählen, Funktionen und Rekursion sind noch nicht unterstützt.

Die Beispiele und Regeln stehen zusätzlich direkt in der App unter `/algorithmen/spickzettel`. Beim Schreibtischtest enthält die Antwort ausschließlich die Ausgaben, eine pro Zeile. Die Testfallaufgabe erwartet je Zeile `gewicht => erwartete Ausgabe`; Gewichte sind ganze Zahlen. Texte und Ausgabezahlen werden nach Entfernung äußerer Leerzeichen exakt verglichen; keine automatische fachliche Auslegung zusätzlicher Beschreibungen.

## Bewertung und Grenzen

Codeaufgaben werden auf mehreren festgelegten Eingaben ausgeführt. Die Sollausgaben stehen unabhängig in der Inhaltsdatei; Tests prüfen auch jede Referenz dagegen. Ein anderer Algorithmus besteht bei gleichem gefordertem Verhalten. Bei Aufgabe 5 ist deshalb auch die direkte Multiplikation korrekt; eine Schleifenpflicht ist nicht im Auftrag vorgegeben. Die kombinierte Listenaufgabe akzeptiert sowohl Listenschleifen als auch passend begrenzte Indexschleifen.

Die Testfallaufgabe prüft zuerst jede selbst gewählte Eingabe samt Sollausgabe gegen die beschriebene Regel. Nur korrekte Testfälle tragen zur Abdeckung bei: negatives ganzzahliges Gewicht, genau 0, genau 100 und ein Wert über 100. Gleichartige Normalfälle oder falsche Sollausgaben genügen nicht. Andere passende negative beziehungsweise übergroße Werte als in der Musterlösung sind erlaubt.

Eine endliche Prüfmenge beweist keine allgemeine Korrektheit. Es gibt keine offizielle IHK-Punktevergabe und keine KI-Bewertung. Alle Ausführungen sind lokal, isoliert und ohne Zugriff auf Dateien, Netzwerk, Prozesse oder .NET-Aufrufe. Eine Interpreter-Instanz behält zwischen Aufrufen keine Variablen.

Begrenzungen: 12.000 Zeichen Code/Antwort, 4.000 Tokens, 64 Parser-Verschachtelungen, 256 Tokens pro Ausdruck, 256 direkte Listeneinträge, begrenzte Gesamtgröße und Verschachtelung selbst gebauter Listen, 10.000 Rechenoperationen, 200 Ausgaben und 16.000 Ausgabezeichen. Die Ablaufspur endet bei 500 Zuständen oder 200.000 Zeichen Zustandsdaten; eine gekürzte Spur wird kenntlich gemacht. Ein Abbruch liefert einen deutschen Hinweis statt einer unkontrollierten Endlosschleife.

## Code und Profil

* `Core/Algorithms/Content`: eingebettete `lessons.json`, Modelle und Katalog.
* `Core/Algorithms/Syntax`: Tokenizer, Parser und interne Syntaxelemente.
* `Core/Algorithms/Execution`: Interpreter, isolierter Zustand und Ablaufspur.
* `Core/Algorithms/Evaluation`: Vergleich der Ausgaben und Prüfung der Testfallgruppen.
* `Web/Components/Pages/Training/Algorithms.razor` und `.razor.cs`: Oberfläche und Ablaufsteuerung, unabhängig vom SQL-Verbindungsstatus.
* `Web/Learning/AlgorithmProgress`: eigener Entwurf, Modus, Erfolge und protokollierte Unterstützung. `ProgressStore.SaveAlgorithmAsync` erhält parallel die bestehenden SQL-Stände.

Eine Reparatur beginnt mit dem gegebenen fehlerhaften Code. Ein gespeicherter leerer Entwurf bleibt leer; nur ein noch nicht vorhandener Entwurf erhält die Vorgabe. Ein frischer unabhängiger Reparaturversuch stellt die Vorgabe wieder her. Ein unveränderter Vorgabecode braucht beim Moduswechsel keine Löschbestätigung. Einmal verwendete Hilfen bleiben bis zum Neustart vermerkt, auch wenn die Musterlösung wieder geschlossen wird. Bisherige Erfolge bleiben beim Neustart eines Versuchs erhalten.

Ältere Profile ohne `AlgorithmLessons` bleiben lesbar; fehlende Eigenschaften erhalten leere Vorgaben und die letzte Aufgabennummer 1. Beschädigte Algorithmus-Einträge werden ebenso wie beschädigte SQL-Einträge nicht still überschrieben. Normale Nutzerprofile dürfen für Bearbeitungstests nicht verwendet werden.

## Fachliche Orientierung

Die Auswahl trainiert Codeverständnis, eigene Programmlogik und Testdaten als Teil der Anforderungen an „Entwicklung und Umsetzung von Algorithmen“. Sie ist keine vollständige Zuordnung zum aktuellen Prüfungskatalog. [IHK Regensburg: Prüfungsinformationen](https://www.ihk.de/regensburg/aus-und-weiterbildung/ausbildung/pruefungstermine-a-bis-z/fachinformatiker-in-pruefung--5882230). Struktogramm und PAP werden hier nicht neu umgesetzt; die IHK beschreibt deren Streichung im überarbeiteten Katalog. [IHK Hannover](https://www.ihk.de/hannover/hauptnavigation/ausbildung-und-weiterbildung/ausbildung/ausbildung-a-z/neuordnungen/pruefungskataloge-it-berufe-6438900).
