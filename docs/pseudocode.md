# Pseudocode und Algorithmen

Quellstand vom 23.11.2026, `1.0.0-beta.7`: 33 Lernaufgaben. Die bisherigen 30 Aufgaben bleiben erhalten; 31–33 ergänzen Kompression, Dekompression und eine Tabellenausgabe. Natürliche Listenschleifen und Typangaben gelten weiter. Zusätzlich ist der Prüfungsfall **Geräteausleihe für ein Kulturfest** auswählbar: drei unabhängige Aufgaben zu Verfügbarkeit, Bestandsänderung und Zeitgrenzen, 45 Minuten und 20/20/10 Trainingspunkte. Der bisherige Zufallspool aus zehn Aufgaben bleibt unverändert. [Prüfungsvertrag](pruefungsmodus.md). Das Beta-6-Windows-Paket ist aus `bfed97f` gebaut; [Paketdaten und Prüfgrenzen](lokale-releases.md).

## Lernweg und Aufgaben

„Leitstelle im Ausnahmezustand“ erzählt eine eigenständige Wiederherstellung der Einsatzplanung mit Entwicklerin Nora Seidel. Jede Aufgabe besitzt einen eigenen Auftrag, drei freiwillige Hinweise, eine Musterlösung mit Erklärung und eine erst nach erfolgreicher Abgabe sichtbare Erkenntnis. Eine vorherige Aufgabe wird nur bei vorhandenem Abschluss als bestätigt bezeichnet; der Einsatzbericht zeigt offene Zwischenstücke statt erfundener Erfolge.

| Nr. | Aufgabe | Form | Schwerpunkt |
| --- | --- | --- | --- |
| 1 | Reicht der Platz für den Auftrag? | Eigene Lösung | Zuladung und Reserve für normale und eilige Aufträge |
| 2 | Warum fehlt der letzte Funkversuch? | Schreibtischtest | Wachsende Wartezeiten, Zeitgrenze und Versuchszähler |
| 3 | Wer hat um Mitternacht Bereitschaft? | Fehlersuche | Tages- und Nachtschichten mit genauer Übergabeminute |
| 4 | Wer muss zuerst los? | Eigene Lösung | Verschachtelte Bedingungen und ODER |
| 5 | Ein Paket fehlt | Fehlersuche | Inklusive Zählschleife, Akkumulator, Nullfall |
| 6 | Wann endet der Ladevorgang? | Schreibtischtest | SOLANGE, Abbruchbedingung und Überladung |
| 7 | Die Grenze gezielt testen | Testfälle entwerfen | Ungültige Werte und Grenzfallabdeckung |
| 8 | Die nächste Ladung im Überblick | Eigene Lösung | Liste durchlaufen, filtern, zählen und summieren |
| 9 | Der höchste Messwert | Eigene Lösung | Maximum, negative Werte und Leerfall |
| 10 | Der verschwundene halbe Wert | Fehlersuche | Durchschnitt, / statt DIV, Division durch 0 vermeiden |
| 11 | Wo steht der Auftrag? | Eigene Lösung | Lineare Suche, erster Treffer und -1 |
| 12 | Darf Nora den Wartungstermin eintragen? | Eigene Funktion | Schaltjahre, Jahrhundertausnahmen und boolesche Rückgabe |
| 13 | Immer die halbe Warteliste | Schreibtischtest | Binäre Suche, Index und Wert unterscheiden |
| 14 | Wo liegt der Abholschein? | Fehlersuche | Fachnummer auf Regalreihe und Spalte abbilden, Richtungswechsel und Reihengrenzen |
| 15 | Tauschen ohne Verlust | Fehlersuche | Indexzuweisung und Hilfsvariable |
| 16 | Ein Durchlauf ist noch keine Sortierung | Schreibtischtest | Ein Bubble-Durchlauf und Anzahl der Tausche |
| 17 | Die ganze Liste in Ordnung bringen | Eigene Lösung | Sortierung, verschachtelte Schleifen, Duplikate |
| 18 | Die Auslastung je Reihe | Eigene Lösung | Verschachtelte Listen, Zeilensummen und Gleichstand |
| 19 | Welcher Wert gehört zu welchem Aufruf? | Schreibtischtest | Parameter, lokale Variablen und Rückgaben |
| 20 | Jeder Pfad braucht eine Rückgabe | Fehlersuche | Funktionsvertrag und fehlende Rückgabe |
| 21 | Eine Gewichtsregel für alle Ansichten | Eigene Funktion | Grenzwerte, frühe Rückgaben und Textwerte |
| 22 | Eine Liste als Parameter | Eigene Funktion | Listenparameter und optionale Hilfsfunktion |
| 23 | Doppelte Werte sind kein Sortierfehler | Fehlersuche | Boolesche Rückgabe, Gegenbeispiel und Gleichheit |
| 24 | Die nächste Ladung sicher zusammenstellen | Eigene Funktion | Reihenfolge, Kapazität und Listenrückgabe |
| 25 | Die Prüfziffer der Kennung | Eigene Funktion | Ziffernsumme, DIV und MOD |
| 26 | Doppelt so viele Daten – viermal so viel Arbeit? | Schreibtischtest | Konkrete Anweisungszahl und quadratisches Wachstum |
| 27 | Beide Seiten einer Grenze prüfen | Testfälle entwerfen | Äquivalenzklassen, Staffelwechsel und ungültige Eingaben |
| 28 | Wenn eine Funktion sich selbst aufruft | Schreibtischtest | Rekursiver Rückweg und lokale Aufrufebenen |
| 29 | Der Aufruf muss dem Basisfall näherkommen | Fehlersuche | Rekursionsfortschritt und Fakultät |
| 30 | Doppelte Meldungen paarweise prüfen | Eigene Funktion | Positionspaare, Schleifengrenzen und Vermeidung von Doppelzählungen |
| 31 | Wiederholungen für das Archiv komprimieren | Eigene Funktion | Zusammenhängende Zeichenfolgen, Grenze 3/4 und mehrstellige Anzahlen |
| 32 | Archivtexte vollständig wiederherstellen | Eigene Funktion | Marker, Ziffernfolge, Textindex und Ende der Eingabe |
| 33 | Kundendaten bis zum Ende ausgeben | Eigene Funktion | Vorgegebene Funktionen, Leerstring als Endsignal und Void |

Der ursprüngliche Zielumfang von 24 Aufgaben ist mit sechs zusätzlichen Abnahmeaufgaben abgeschlossen: mit den drei neuen Text-/Ausgabeaufgaben insgesamt 16 freie Lösungen, acht Reparaturen, sieben Schreibtischtests und zwei Testfallaufgaben. Aus der bisherigen Planung sind damit keine weiteren Aufgaben offen. Beliebige weitere Algorithmen, Textverarbeitung oder Programmiersprachenfunktionen sind dadurch nicht vollständig abgedeckt. Grafische Modellierung hat ihren eigenen Lernbereich in Abschnitt 03.

## Bedienung

Die Reihenfolge lautet Lernmodus, Story, Aufgabe und Editor. Neue Aufgaben beginnen in „Ohne Hilfe lösen“. Der Modus bleibt je Aufgabe gespeichert. „Geführt lernen“ ergänzt Erklärungen, drei einzeln aufdeckbare Hinweise, einen internen Spickzettel und die Musterlösung mit Ansehen/Ausblenden.

Nach selbstständigem Erfolg öffnet die Moduswahl die geprüfte Antwort zum Nachlesen. Geführte Experimente verwenden eine Kopie. Für einen frischen Versuch gibt es **Neu üben**. [Moduswechsel und ältere Profile](lernkonzept.md#bedienung-und-lernmodus).

In den Arbeitsaufträgen der 33 Lern- und 13 Prüfungsaufgaben kennzeichnet Codeschrift Variablen, Funktionsaufrufe und konkrete Ausdrücke. Das gilt auch für zweideutige Wörter wie `frei`, wenn sie als Eingabename gemeint sind. Beispieleingaben sind unter denselben Namen beschriftet.

Der Editor zeigt physische Zeilennummern einschließlich Leerzeilen, passend zu Fehlern und Ablaufspur. Lange Zeilen scrollen im Editor und in der nummerierten Schrittansicht waagerecht, statt die Zuordnung durch Umbruch zu verändern; im Editor folgen die Nummern dem senkrechten Scrollen. In Code- und Reparaturaufgaben fügt Tab vier Leerzeichen ein. Markierte Zeilen werden gemeinsam eingerückt; Umschalt+Tab entfernt bis zu vier führende Leerzeichen oder einen Tab je Zeile. **Esc, anschließend Tab** verlässt das Feld vorwärts, **Esc, anschließend Umschalt+Tab** rückwärts. Der kurze Tastaturhinweis steht am Editor. Strg+Tab bleibt für den Browser reserviert. Reine Antwortfelder für Ausgaben und Testfälle behalten die normale Tab-Navigation.

Einrückungen werden wie normale Eingaben gespeichert und verwerfen veraltetes Feedback. Die native Browserbearbeitung erhält Rückgängig/Wiederholen in unterstützenden Browsern; ein Textfeld-Ersatzpfad ermöglicht Einrückung auch ohne diese ältere Bearbeitungsfunktion, kann dann aber keine native Undo-Historie zusichern. Der Editor begrenzt weiterhin auf 12.000 Zeichen und schneidet durch Einrückung keinen Code ab.

„Ablauf untersuchen“ verwendet die angezeigten Beispieleingaben. Bei freien und Reparaturaufgaben wird der eigene Code ausgeführt; beim Schreibtischtest der vorgegebene Code, bei der Testfallaufgabe die Referenz der beschriebenen Gewichtsregel. Die Schrittansicht zeigt den Zustand **nach** der ausgewählten Anweisung beziehungsweise Bedingungsprüfung. Vor und zurück verändert nur den angezeigten Zustand; die bereits begrenzt berechnete Spur wird nicht erneut ausgeführt. Eingabeänderungen verwerfen veraltete Vorschau und Bewertung.

Bei den mit einem Funktionsvertrag gekennzeichneten Aufgaben stehen Funktionsname und Argumentreihenfolge im Auftrag. Es werden ausschließlich Definitionen eingereicht; die App ruft die vereinbarte Funktion automatisch mit wechselnden Argumenten auf. Ein zusätzlicher Hauptablauf wird abgelehnt. Die Vorschau zeigt den Rückgabewert beziehungsweise beim Void-Auftrag die Ausgaben des Beispielaufrufs sowie bei jedem Schritt den aktiven Bereich, seine Aufrufebene und dessen lokale Variablen. RÜCKGABE ist keine Bildschirmausgabe; im Abschnitt „Bisherige Ausgaben“ stehen tatsächlich ausgeführte AUSGABE-Anweisungen beziehungsweise die Ausgaben der vorgegebenen Tabellenfunktionen. Einige Schreibtischtests enthalten dagegen ausdrücklich einen vorgegebenen Hauptablauf mit Aufrufen.

Der Spickzettel bleibt im selben Tab. Aufgabenliste, ausblendbarer Selbsttest-Hinweis und die Möglichkeit zur Wiederherstellung im Lernplan werden gemeinsam mit SQL verwendet. Der Lernplan berücksichtigt neue, begonnene und mit Unterstützung gelöste Aufgaben aller drei Lernwege. Übersicht und Lernplan stehen in der Kopfnavigation immer vorn. Danach folgen im Algorithmus-Kontext Training und Pseudocode-Spickzettel, im SQL-Kontext Ermittlung, Werkstatt und SQL-Spickzettel. Der Kontext gilt innerhalb der Browsersitzung; ein vollständiges Neuladen einer allgemeinen Seite beginnt wieder mit SQL.

## Vereinbarte Schreibweise

Pseudocode besitzt keine universell verbindliche Grammatik. Der Trainer führt eine dokumentierte Teilmenge aus; beliebige natürliche Sprache oder C#/Java-Code werden nicht automatisch bewertet.

* Eine Anweisung pro Zeile; alternativ trennt ein Semikolon Anweisungen. Einrückung ist frei. Schlüsselwörter, Wortoperatoren, Wahrheitswerte und `LÄNGE`/`LAENGE` müssen im Trainer großgeschrieben sein, etwa `WENN`, `DANN`, `AUSGABE`, `UND` und `WAHR`. Bei Klein-/Mischschreibung nennen Vorschau und Bewertung die ursprüngliche Zeile und die konkrete Korrektur. Variablennamen bleiben unabhängig von Groß-/Kleinschreibung; Textwerte und Kommentare werden nicht umgeschrieben oder auf große Befehle geprüft. Vorhandene Entwürfe bleiben erhalten, müssen bei einer neuen Prüfung aber diese Schreibregel erfüllen.
* Die im Auftrag genannten Eingaben sind bereits als Variablen gesetzt. Bevorzugte Zuweisung mit `=`, zum Beispiel `rest = kapazitaet - beladen`. Beispiele, Vorgabecode, Hinweise und Referenzlösungen verwenden diese Schreibweise. `←`, `<-` und `:=` bleiben für vorhandene Entwürfe akzeptiert. Ausgabe mit `AUSGABE ausdruck`.
* Zahlen mit Dezimalpunkt, Texte in einfachen oder doppelten Anführungszeichen ohne Escape-Syntax, Wahrheitswerte `WAHR` und `FALSCH`. Kommentare ab `//` bis zum Zeilenende.
* Rechnen mit `+`, `-`, `*`, `/`, `DIV`, `MOD` beziehungsweise `%`; `DIV` schneidet zur 0 hin ab. Keine implizite Umwandlung zwischen Text und Zahl.
* Gleichheitsvergleiche bevorzugt mit `==`, zum Beispiel `WENN anzahl == 0 DANN`; ein einzelnes `=` innerhalb einer Bedingung bleibt als Vergleich zulässig. Weitere Vergleiche mit `<>`, `!=`, `<`, `<=`, `>` und `>=`. `NICHT` bezieht sich auf den folgenden Vergleich. Priorität: Klammern, unäres Vorzeichen, Punktrechnung, Strichrechnung, Vergleiche, NICHT, UND, ODER. UND und ODER werten den rechten Teil nur bei Bedarf aus.
* `WENN bedingung DANN`, optional `SONST`, abgeschlossen mit `ENDE WENN`. Verschachtelte WENN-Blöcke besitzen jeweils einen eigenen Abschluss.
* `SOLANGE bedingung`, optional `TUE`, und `ENDE SOLANGE`. Die Bedingung wird vor jedem Durchlauf geprüft.
* `FÜR i VON start BIS ende`, optional `TUE`, und `ENDE FÜR`. Ganzzahlige Grenzen einmal beim Eintritt ausgewertet, beide eingeschlossen, Schrittweite +1; bei start > ende kein Durchlauf. Die interne Steuerung ist unabhängig von einer Zuweisung an i im Rumpf. i soll nicht im Rumpf verändert werden. Nach einer nichtleeren Schleife behält i den zuletzt im Rumpf gespeicherten Wert; bei einer leeren Schleife entsteht dadurch keine neue Variable.
* `FÜR name = start; bedingung; name = fortschritt` und `ENDE FÜR`: Startzuweisung einmal ausführen, Bedingung vor jedem Durchlauf prüfen, Fortschrittszuweisung nach dem Rumpf ausführen. Die drei Kopfteile dürfen auch auf drei Zeilen stehen. `FUER`, alternative Zuweisungszeichen und `TUE` nach dem dritten Teil sind ebenfalls möglich. Bei anfangs falscher Bedingung entfallen Rumpf und Fortschritt; `VERLASSE SCHLEIFE` und `RÜCKGABE` überspringen den Fortschritt. Diese Form ergänzt die Zählschleife mit `VON`/`BIS`.
* `FÜR JEDEN wert IN werte`, `FÜR JEDE meldung IN meldungen`, `FÜR JEDES gewicht IN gewichte` und `FÜR ALLE wert IN werte` sind gleichwertig. `FUER` ist im Kopf und in `ENDE FÜR` möglich. Die neuen Wörter sind nur an der Stelle des Mengenworts großzuschreiben. Durchlaufen werden die beim Eintritt vorhandenen Einträge; Änderungen der ursprünglichen Liste im Rumpf verändern nicht nachträglich diese Folge.
* Listenliterale wie `[10, 20]`, Lesezugriff `liste[0]`, Länge `LÄNGE(liste)` oder `LAENGE(liste)`. Indexbeginn 0. Schreibzugriff mit `liste[i] = wert`, auch verschachtelt `matrix[i][j] = wert`; der Index muss bereits existieren und ganzzahlig sein. Kein automatisches Vergrößern einer Liste. Eine Zuweisung erstellt im Trainer eine unabhängige Wertkopie einschließlich verschachtelter Listen. Auch die Listenschleife arbeitet mit einer Wertkopie des jeweiligen Eintrags; zum Ändern der Ursprungsliste ihren Index verwenden.
* Funktionen mit `FUNKTION name(parameter, ...)` und `ENDE FUNKTION`. Definitionen stehen nur auf oberster Ebene, vor oder nach dem Hauptablauf. Aufrufe sind Ausdrücke: `ergebnis = name(wert)`. Namen sind unabhängig von Groß-/Kleinschreibung; Schlüsselwörter wie FUNKTION bleiben groß. Argumente werden links nach rechts ausgewertet, dann nach Position zugeordnet. Doppelte Funktionsnamen oder Parameter derselben Definition sind ungültig.
* `RÜCKGABE ausdruck` beziehungsweise `RUECKGABE ausdruck` beendet nur den aktuellen Aufruf, auch aus verschachtelten Bedingungen und Schleifen. Rückgaben können Zahlen, Texte, Wahrheitswerte und Listen sein. Bei Funktionen mit Ergebnis braucht jeder ausgeführte Pfad einen Rückgabewert. AUSGABE ersetzt keine Rückgabe; im Hauptablauf ist RÜCKGABE unzulässig.
* Jeder Aufruf besitzt einen eigenen lokalen Variablenbereich ohne Zugriff auf Hauptablaufvariablen. Parameter und Rückgaben verwenden Wertkopien von Listen. Andere selbst definierte Funktionen und rekursive Selbstaufrufe sind erlaubt, auch indirekte Rekursion. Gemeinsam maximal 16 gleichzeitig aktive Aufrufebenen und 16 Parameter/Argumente je Aufruf. Basisfall und Fortschritt müssen zur erlaubten Eingabedomäne passen. Die Tiefen- und Rechengrenzen werden nicht je Selbstaufruf zurückgesetzt. Prozeduren ohne Rückgabewert sind als eigenständige Aufrufe erlaubt. Keine Klassen, frei aufrufbaren externen Funktionen oder Datei-/Netzwerkschnittstellen. Abwärtszählen erfolgt bei Bedarf mit einer SOLANGE-Schleife.

Die Beispiele und Regeln stehen zusätzlich direkt in der App unter `/algorithmen/spickzettel`. Beim Schreibtischtest enthält die Antwort ausschließlich die Ausgaben, eine pro Zeile. Die Testfallaufgabe erwartet je Zeile `gewicht => erwartete Ausgabe`; Gewichte sind ganze Zahlen. Texte und Ausgabezahlen werden nach Entfernung äußerer Leerzeichen exakt verglichen; keine automatische fachliche Auslegung zusätzlicher Beschreibungen.

## Datentypen und Funktionsköpfe

Im Lernweg sind Typangaben bei allen zwölf Funktionsaufgaben freiwillig. In jeder neuen Pseudocode-Prüfung verlangt der erste Auftrag den vollständigen typisierten Kopf. Direkt am Arbeitsauftrag steht **Typangaben erforderlich** oder **Typangaben freiwillig**. Der Hinweis ist in beiden Lernmodi sichtbar und verbraucht keine Hilfe. Früher begonnene Prüfungen behalten ihren bisherigen Vertrag.

| Wert | Gleichwertige Schreibweisen | Beispiel |
| --- | --- | --- |
| Ganze Zahl | `Integer`, `int`, `Ganzzahl` | `-2` |
| Zahl mit möglichem Nachkommateil | `Double`, `Kommazahl`, `Dezimalzahl` | `2.5` |
| Wahrheitswert | `Boolean`, `bool`, `Wahrheitswert` | `FALSCH` |
| Text | `String`, `Text`, `Zeichenkette` | `"bereit"` |
| Kein Rückgabewert | `Void`, `KeinRückgabewert`, `KeinRueckgabewert` | Nur als Rückgabetyp |
| Liste mit Elementtyp | `List<Integer>`, `Liste<Ganzzahl>`, `int[]` | `[2, 4]` |

Typnamen sind unabhängig von ihrer Großschreibung und bleiben außerhalb von Typangaben als Bezeichner nutzbar. Die Befehle behalten ihre bisherige Großschreibung. Der Trainer rechnet intern weiterhin mit Dezimalzahlen. Bei `Integer` muss der Wert ganzzahlig sein, `Double` erlaubt auch ganze Zahlen. Eine leere Liste ist für jeden Elementtyp zulässig. Bei einem typisierten Funktionskopf zählt dennoch der deklarierte Elementtyp. `Integer[][]` entspricht `Liste<Liste<int>>`. Es gelten höchstens 16 Listenebenen und die bisherigen Grenzen für Listenwerte.

```text
FUNKTION istGerade(zahl: Integer): Boolean
    rest: Ganzzahl = zahl MOD 2
    RÜCKGABE rest == 0
ENDE FUNKTION
```

`Boolean istGerade(int zahl)` ist ein gleichwertiger Kopf. `Integer rest = zahl MOD 2` ist eine gleichwertige lokale Deklaration. Bei `istGerade(6)` ist 6 das Argument, Boolean der Rückgabetyp und WAHR der konkrete Wert. Angegebene Typen am Einstieg müssen zum Arbeitsauftrag passen. Die Prüfung vergleicht die Parameter in ihrer Aufrufreihenfolge. Die Ausführung prüft übergebene Werte, Rückgaben sowie spätere Schreibzugriffe auf deklarierte Variablen und Listen. Auch bei freiwilligen Angaben muss der tatsächliche Rückgabewert zum Auftrag passen. Der Text `"42"` ersetzt keine Zahl. Eine Deklaration braucht einen Startwert. Typbindungen gelten nur im jeweiligen Funktionsaufruf bzw. Hauptablauf. Die Ablaufvorschau nutzt dieselben Regeln wie die Bewertung.

## Bewertung und Grenzen

Codeaufgaben werden auf mehreren festgelegten Eingaben ausgeführt. Die Sollausgaben stehen unabhängig in der Inhaltsdatei; Tests prüfen auch jede Referenz dagegen. Ein anderer Algorithmus besteht bei gleichem gefordertem Verhalten. Bei Aufgabe 5 ist deshalb auch die direkte Multiplikation korrekt; eine Schleifenpflicht ist nicht im Auftrag vorgegeben. Die kombinierte Listenaufgabe akzeptiert sowohl Listenschleifen als auch passend begrenzte Indexschleifen.

Funktionsaufgaben prüfen den benannten Einstieg tatsächlich mit neuen Argumenten, nicht den Wortlaut der Musterlösung. Eigene Parameternamen und zusätzliche Hilfsfunktionen sind erlaubt. Geprüft wird das zurückgegebene Ergebnis; zusätzliche AUSGABE-Zeilen innerhalb der Funktion zählen als zusätzliche Ausgaben und erfüllen den Vertrag nicht. Ein Name oder eine passende Ausgabe im Hauptablauf ersetzt die verlangte Funktion nicht.

Bei Suche und Sortierung wird funktionales Verhalten bewertet, keine garantierte Laufzeitklasse oder Identität eines Algorithmus. Selection Sort besteht die Sortieraufgabe ebenfalls. Die lineare Suche, der abschließend gemeldete Index im Schreibtischtest zur binären Suche und die Sortierreferenz werden mit 100 deterministisch erzeugten Listen gegen unabhängige Bibliotheksergebnisse geprüft. Für Aufgabe 14 decken die Prüffälle Reihenanfänge und -enden, innere Fächer in beiden Laufrichtungen, wechselnde Regalbreiten und ein einspaltiges Regal ab. Die Abschlussaufgabe arbeitet ausdrücklich in Eingabereihenfolge; sie fordert keine optimale Packung und erlaubt kein Umsortieren als Ersatz für diese Regel.

Beide Testfallaufgaben prüfen zuerst jede selbst gewählte Eingabe samt Sollausgabe gegen die beschriebene Regel. Nur korrekte Testfälle tragen zur Abdeckung bei. Aufgabe 7 verlangt negatives ganzzahliges Gewicht, genau 0, genau 100 und einen Wert über 100. Aufgabe 27 verlangt eine negative Menge sowie genau 0, 9, 10, 49 und 50. Gleichartige Normalfälle oder falsche Sollausgaben genügen nicht. Andere passende negative beziehungsweise übergroße Werte als in der Musterlösung sind erlaubt, wenn kein einzelner Grenzwert ausdrücklich verlangt wird. Das Format bleibt skalare ganze Zahl => Ausgabe; mehrere Eingabevariablen oder strukturierte Fälle werden hier nicht eingelesen.

Aufgabe 26 zählt eine ausdrücklich benannte Anweisung für zwei Eingabegrößen und erklärt den Zusammenhang zum quadratischen Wachstum. Diese konkrete Zählaufgabe bewertet keine allgemeine Big-O-Klassifikation beliebigen Codes. Die Prüfziffer in Aufgabe 25 ist eine eigens beschriebene Übungsregel und kein behauptetes reales Standardverfahren. Rekursionsaufgaben unterscheiden Aufrufweg, Basisfall und Rückweg; eine iterative Alternative mit korrektem Ergebnis bleibt zulässig.

Die Funktion `istSchaltjahr` in Aufgabe 12 benötigt einen booleschen Rückgabewert. Die Prüfung unterscheidet `WAHR` vom Text `"WAHR"` und akzeptiert gleichwertige Formulierungen der Kalenderregel.

Eine endliche Prüfmenge beweist keine allgemeine Korrektheit. Es gibt keine offizielle IHK-Punktevergabe und keine KI-Bewertung. Alle Ausführungen sind lokal, isoliert und ohne Zugriff auf Dateien, Netzwerk, Prozesse oder .NET-Aufrufe. Eine Interpreter-Instanz behält zwischen Aufrufen keine Variablen.

Begrenzungen: 12.000 Zeichen Code/Antwort, 4.000 Tokens, 64 Parser-Verschachtelungen, 256 Tokens pro Ausdruck, 256 direkte Listeneinträge, begrenzte Gesamtgröße und Verschachtelung selbst gebauter Listen, 16 Funktionsaufrufebenen, je 16 Parameter/Argumente, normalerweise 10.000 Rechenoperationen (Aufgaben 31/32: 100.000, direkt im Auftrag angezeigt), 200 Ausgaben und 16.000 Ausgabezeichen. Das Rechenbudget gilt über alle Aufrufe gemeinsam. Die Ablaufspur endet bei 500 Zuständen oder 200.000 Zeichen Zustandsdaten; eine gekürzte Spur wird kenntlich gemacht. Ein Abbruch liefert einen deutschen Hinweis statt einer unkontrollierten Endlosschleife.

## Textverarbeitung und Tabellenauftrag seit Beta 7

Aufgaben 31 und 32 sind zwei unmittelbar aufeinanderfolgende eigene Funktionen mit String-Parameter und String-Rückgabe. Originaltexte bestehen aus A–Z; `%` ist reserviert. Erst ab vier gleichen aufeinanderfolgenden Zeichen gilt `%ZEICHEN%ANZAHL`, wobei die Zahl die gesamte Länge des Laufs bezeichnet. Leerer Text bleibt leer. Die Dekompression setzt gültige Eingaben voraus. Vier Wiederholungen sind durch die Formatvorgabe nicht unbedingt kürzer gespeichert. Die Aufgabe verspricht daher keine Einsparung bei jeder Eingabe.

Textzugriffe `text[i]` beginnen bei 0 und liefern einen Text mit einem Zeichen. `LÄNGE`/`LAENGE` akzeptieren Text und Listen; Textindexzuweisungen bleiben unzulässig. `+` verknüpft zwei Texte, ohne Zahlen implizit umzuwandeln. `ALS_TEXT` akzeptiert Zahl, Wahrheitswert und Text. `ALS_GANZZAHL` akzeptiert ausschließlich nichtleeren Zifferntext mit optionalem Vorzeichen im unterstützten Zahlenbereich; keine Leerzeichen, Nachkommastellen oder Listen. Erzeugte Texte bleiben auf 16.000 Zeichen begrenzt. `VERLASSE SCHLEIFE` beendet nur die innerste Schleife derselben Funktion; Rückgaben verlassen weiterhin den gesamten Aufruf.

Die Musterlösung von Aufgabe 33 verwendet `FÜR daten = hole_daten(); daten != ""; daten = hole_daten()`. Der erste Abruf erfolgt einmal vor der ersten Bedingung, weitere Abrufe erst nach der Datensatzausgabe. Die bisherige SOLANGE-Lösung bleibt gleichwertig. Grundlagen und Mustererklärung unterscheiden die drei Phasen anhand eines anderen Zahlenbeispiels.

In Aufgabe 33 stellt ausschließlich der Auftrag vier lokale Funktionen bereit: `hole_daten(): String`, `schreibe_header(): Void`, `schreibe_datensatz(daten: String): Void` und `schreibe_footer(): Void`. Der Benutzer implementiert nur `output()` ohne Argumente. `Void`, `KeinRückgabewert` und `KeinRueckgabewert` bezeichnen keinen Wert; sie sind nur als Rückgabetyp zulässig. Eine solche Funktion endet am Blockende oder mit `RÜCKGABE` ohne Ausdruck. Eigenständige Aufrufe dürfen auch untypisiert sein. Ein fehlender Wert in einem Ausdruck oder einer Funktion mit Werttyp bleibt ein Fehler.

Jeder Prüflauf erhält eine neue isolierte Quelle mit höchstens 100 nichtleeren synthetischen Datensätzen und 12.000 Datenzeichen. `datensaetze` beschreibt nur diese Quelle und ist kein Programmparameter und keine globale Variable. Nach dem letzten Datensatz liefert der Abruf `""`. Der Validator prüft neben den sichtbaren Ausgaben den Vertrag: eine Überschrift, alle tatsächlich abgerufenen Datensätze in Reihenfolge, ein Abschluss nach erkanntem Ende. Auch leere Datenfolgen brauchen Kopf und Fuß. Überspringen, doppelte Ausgabe, vorzeitiger Abschluss, Ersatz durch AUSGABE oder Neudefinition der bereitgestellten Funktionen werden erklärt. Unterschiedliche Schleifen und eigene Hilfsfunktionen sind erlaubt; kein fester Codewortlaut wird verlangt.

Die Textaufgaben vergleichen Rückgaben einschließlich äußerer Leerzeichen exakt. Leere Rückgaben werden als `""` angezeigt. Die 32 unabhängigen Prüffälle der drei Aufgaben umfassen leere Eingaben, kurze Läufe, die Grenze 3/4, aufeinanderfolgende Marker, Randpositionen, zwei-/dreistellige Anzahlen bis 999 sowie leere, einzelne, mehrere und identische Datensätze. Geführte Grundlagen erklären die Syntax an anderen Beispielen; jeweils drei gestufte Hinweise bleiben von der vollständigen Musterlösung und ihrer Erklärung getrennt. Die Nutzervorlage der Dekompression wurde fachlich berichtigt: Zeichenwert `text[i + 1]` statt Indexzahl, durchgehend `ziffern` und vollständiges `ENDE WENN`.

## Code und Profil

* `Core/Algorithms/Content`: eingebettete `lessons.json`, Modelle und Katalog.
* `Core/Algorithms/Syntax`: Tokenizer, Parser und interne Syntaxelemente.
* `Core/Algorithms/Execution`: Interpreter, isolierter Zustand und Ablaufspur.
* `Core/Algorithms/Evaluation`: Vergleich der Ausgaben und Prüfung der Testfallgruppen.
* `Web/Components/Pages/Training/Algorithms.razor` und `.razor.cs`: Oberfläche und Ablaufsteuerung, unabhängig vom SQL-Verbindungsstatus.
* `Web/Components/Learning/CodeEditor.razor` und `wwwroot/code-editor.js` stellen den gemeinsamen nummerierten Editor für Pseudocode, SQL-Ermittlung, SQL-Werkstatt und die zugehörigen Prüfungen bereit.
* `Web/Learning/AlgorithmProgress`: eigener Entwurf, Modus, Erfolge und protokollierte Unterstützung. `ProgressStore.SaveAlgorithmAsync` erhält parallel die bestehenden SQL-Stände.

Eine Reparatur beginnt mit dem gegebenen fehlerhaften Code. Ein gespeicherter leerer Entwurf bleibt leer; nur ein noch nicht vorhandener Entwurf erhält die Vorgabe. Ein frischer unabhängiger Reparaturversuch stellt die Vorgabe wieder her. Ein unveränderter Vorgabecode braucht beim Moduswechsel keine Löschbestätigung. Einmal verwendete Hilfen bleiben bis zum Neustart vermerkt, auch wenn die Musterlösung wieder geschlossen wird. Bisherige Erfolge bleiben beim Neustart eines Versuchs erhalten.

Ältere Profile ohne `AlgorithmLessons` bleiben lesbar; fehlende Eigenschaften erhalten leere Vorgaben und die letzte Aufgabennummer 1. Beschädigte Algorithmus-Einträge werden ebenso wie beschädigte SQL-Einträge nicht still überschrieben. Normale Nutzerprofile dürfen für Bearbeitungstests nicht verwendet werden.

Die Aufgaben-IDs 1–30 bleiben stabil. Bei den fünf ausgetauschten Aufgaben 1, 2, 3, 12 und 14 beginnt ein neuer Versuch. Beim Einlesen eines älteren Profils trennt der Speicher die früheren Entwürfe, Erfolge, Versuche und Hilfestände in `Beta3AlgorithmArchive` ab. Hat ein Profil die ersten vier Aufgabenwechsel bereits mitgemacht, wird nur die frühere Aufgabe 14 ins Archiv übernommen. Die nächste reguläre Speicherung schreibt Archiv und aktuellen Stand zusammen. Der frühere Abschlussstatus und der damalige Entwurf lassen sich in der jeweiligen Aufgabe aufklappen. Übersicht, Lernplan und Einsatzbericht beziehen sich auf die aktuelle Aufgabenfassung.

## Fachliche Orientierung

Die Auswahl trainiert Codeverständnis, eigene Programmlogik und Testdaten als Teil der Anforderungen an „Entwicklung und Umsetzung von Algorithmen“. Sie ist keine vollständige Zuordnung zum aktuellen Prüfungskatalog. [IHK Regensburg: Prüfungsinformationen](https://www.ihk.de/regensburg/aus-und-weiterbildung/ausbildung/pruefungstermine-a-bis-z/fachinformatiker-in-pruefung--5882230). Struktogramm und PAP werden hier nicht neu umgesetzt; die IHK beschreibt deren Streichung im überarbeiteten Katalog. [IHK Hannover](https://www.ihk.de/hannover/hauptnavigation/ausbildung-und-weiterbildung/ausbildung/ausbildung-a-z/neuordnungen/pruefungskataloge-it-berufe-6438900).

Zur Abgrenzung: Der Prüfungsbereich umfasst neben Codeverständnis, eigener Programmierlogik und Testdaten auch grafische Darstellung und Datenabfragen. Diese App verteilt solche Themen auf getrennte Lernbereiche; 33 Pseudocode-Aufgaben sind keine vollständige AP2-Zusage. [Ausbildungsordnung: Prüfungsbereich Entwicklung und Umsetzung von Algorithmen](https://www.gesetze-im-internet.de/fiausbv/__14.html).

## Zusätzlicher Prüfungsbereich seit Alpha 9

Neben den 33 Lernaufgaben existiert ein eigener Zehnerpool ausschließlich für Prüfungen. Eine 45-Minuten-Runde wählt zwei Funktions-/Korrekturaufgaben und eine Analyseaufgabe für insgesamt 50 Trainingspunkte. Kein neuer Interpreterdialekt; bestehende Schreibregeln, Wertsemantik und Budgets gelten weiter. Bewertung, Teilpunkte, Speicherung und Grenzen stehen unter [Prüfungsmodus](pruefungsmodus.md).
