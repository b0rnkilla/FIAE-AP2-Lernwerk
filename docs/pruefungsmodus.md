# Prüfungsrunden für SQL und Pseudocode

Stand: **05.11.2026 · Quellfassung 0.1.1-alpha.9**. Die Prüfungen sind zusätzlich zu den bisherigen 220 Lernübungen umgesetzt; sie verändern den bisherigen Aufgabenfortschritt nicht. Im eigenen Prüfungspool liegen zehn Algorithmus- und 15 SQL-Aufgaben. Das veröffentlichte Alpha-8-ZIP enthält diese neuen Funktionen noch nicht. Kein neuer Paketbau ohne Auftrag.

## Einstieg und Ablauf

Auf Übersicht und in der Modulnavigation führt **Prüfung üben** zum Einstieg. Algorithmen: `/algorithmen/pruefung`, SQL: `/sql/pruefung`. Die aktuelle Runde liegt jeweils unter `/runde`. Die Uhr startet erst mit **Prüfung starten**. Eine noch nicht bewertete Prüfung wird nur nach ausdrücklicher Bestätigung ersetzt; frühere Ergebnisse bleiben in der Historie.

Jede Runde dauert 45 Minuten und bietet 50 Trainingspunkte. Das persönliche Trainingsziel beträgt 25 Punkte. Die Zeitansätze sind Schätzungen für gezieltes Modultraining, kein nachgewiesener Ersatz für eine vollständige AP2. Vor Abgabe gibt es keine Probeausführung, Lösungskontrolle oder Musterlösung. Aufgaben dürfen frei gewählt und zur Kontrolle markiert werden. Die Auswertung erscheint nach Bestätigung beziehungsweise Fristablauf. Abgabebestätigung und Ergebnis werden sichtbar fokussiert.

**Pause** friert die Restzeit ein und blendet Aufgaben/Editoren aus. Browser schließen, zu einer anderen Seite wechseln oder die App beenden pausiert nicht. Die UTC-Frist bleibt gespeichert; nach Ablauf wird beim nächsten Öffnen/Speichern abgegeben. Offene Runden werden etwa einmal pro Sekunde auf Fristablauf geprüft. Entwürfe werden bei Änderungen ungefähr sekündlich sowie beim Verlassen des Editors, Wechseln, Pausieren und Abgeben atomar gespeichert. Bei abruptem Prozess-/Verbindungsabbruch können die letzten noch nicht gespeicherten Eingaben verloren gehen.

Der Spickzettel bleibt erreichbar. Er pausiert die Runde, merkt **mit Nachschlagen** und führt direkt zur Prüfung zurück. Nach Fristablauf kann Nachschlagen die Abgabe nicht rückgängig machen. Veraltete Fenster werden durch Revisionsprüfung am Überschreiben gehindert. In der Heimnetz-Vorführung gilt weiterhin ein gemeinsames Profil: keine getrennten Benutzerprüfungen.

## Pseudocode: zehn eigene Aufgaben

Aus dem separaten Pool werden zwei Funktions-/Korrekturaufgaben zu je 20 Punkten und eine Analyseaufgabe mit zehn Punkten gewählt. Die erste Aufgabe stammt aus 1–3, die zweite aus 4–8, die Analyse aus 9–10. Auswahl und Reihenfolge bleiben nach dem Start erhalten. Wiederholungen bei späteren Runden sind möglich. Die bisherigen 30 Lernaufgaben behalten ihre IDs und Entwürfe.

| ID | Aufgabe | Schwerpunkt |
| --- | --- | --- |
| 1 | Eine Sammelrechnung berechnen | Mehrere Bedingungen, gewichtete Summe, Rabattgrenze, Listenrückgabe |
| 2 | Gestaffelte Nutzungskosten | Angefangene Blöcke, Sonderfälle, Zuschlag, Rückgabe |
| 3 | Zusammenhängende Arbeitsplätze | Verschachtelte Listen, zusammenhängender Block, erste Fundstelle |
| 4 | Das günstigste passende Angebot | Filtern, Minimum, Gleichstand, Leerfall |
| 5 | Aufträge nach zwei Kriterien ordnen | Frei wählbares Sortierverfahren, zwei Sortierschlüssel |
| 6 | Messreihen bereinigt auswerten | Unterschiedlich lange Reihen, Gültigkeitsbereich, Aggregation |
| 7 | Eine Qualitätskontrolle reparieren | Schleifengrenzen, ODER/UND, Startwert |
| 8 | Rückgabe in jedem Entscheidungszweig | Ungültige Eingaben, Grenzwerte, vollständige Rückgabe |
| 9 | Lokale Änderungen nachvollziehen | Schreibtischtest, Parameter-Wertkopien und Ausgabereihenfolge |
| 10 | Grenzen einer Wartezeitregel prüfen | Äquivalenzklassen und konkrete Grenzfälle |

Der bestehende Interpreter wird verwendet, einschließlich großer Schlüsselwörter, Wertsemantik, eigener Funktionsparameter und gemeinsamer Laufzeitbudgets. Es kommen keine externen Objekte oder sprachfremden Pseudocode-Befehle hinzu. Funktionsaufträge nennen Eingaben, Rückgabe und zulässige Werte. Die automatische Prüfung bewertet funktionales Verhalten auf einer endlichen Prüfmenge, keine bewiesene allgemeine Korrektheit oder Laufzeitklasse.

## SQL: drei neue Fälle

| Fall | Tabellen | Fünf Aufgaben |
| --- | --- | --- |
| Bildungszentrum | Raeume, Veranstaltungen, Buchungen | Datumsbereich, mehrstufiger JOIN, GROUP BY/HAVING, fehlende Bestätigung, UPDATE |
| Servicebetrieb | Kunden, Auftraege, Arbeitsschritte | Datums-/NULL-Filter, JOIN, Aggregation, fehlender Arbeitsschritt, INSERT VALUES |
| Ersatzteillager | Kategorien, Artikel, Lieferpositionen | Bestandsdifferenz, JOIN, LEFT JOIN/COUNT, Durchschnitts-Unterabfrage, ALTER TABLE ADD |

Die Auswahl kann zufällig oder gezielt erfolgen. Schema, Beziehungen und Beispieldaten bleiben in jeder Aufgabe sichtbar. Eine Aufgabe beeinflusst keine andere: Jede Ausführung beginnt mit frischen sitzungseigenen temporären Tabellen. Prüfungen laufen unter `TrainerReader` ohne Rückkehr zu erhöhten Rechten, ohne Verbindungspooling und mit acht Sekunden Ausführungsbudget sowie 200 Ergebniszeilen. Die bestehende Ermittlung/Werkstatt bleibt unverändert.

Erlaubt ist genau eine Anweisung der verlangten Art. Tabellenbezüge werden mit ScriptDom auf die Falltabellen begrenzt und im Syntaxbaum zu temporären Namen umgeschrieben. Literale und Aliase bleiben erhalten. Externe Quellen, andere Schemas, Verwaltungsbefehle, INTO, WITH, OPTION und Tabellenhinweise sind ausgeschlossen. INSERT verwendet VALUES, keine Prozeduraufrufe. ALTER unterstützt genau eine einfache Spalte mit int, bit, decimal oder nvarchar und NULL/NOT NULL ohne zusätzliche Eigenschaften. Alternative korrekte JOIN-/Unterabfrageformulierungen werden innerhalb dieser Grenzen akzeptiert. SQL-Schreibregeln bleiben wie im Trainer: große Befehle/Funktionen und Semikolon.

## Teilpunkte und Nachbereitung

**Pseudocode:** Die festen Prüffälle teilen sich die maximalen Punkte der Aufgabe; Restpunkte werden den ersten Fällen zugeordnet. Jeder Prüffall wird unabhängig ausgeführt, auch wenn ein anderer fehlschlägt. Beim Schreibtischtest gibt jede der fünf richtig zugeordneten Ausgaben zwei Punkte; unwichtige Leerzeichen in Listen sind erlaubt. Die Testfallaufgabe vergibt Punkte für die fest geforderten Eingabegruppen, nicht für beliebig viele zusätzliche Fälle. Falsche Formate/Sollausgaben verhindern dort Abdeckungspunkte. Nicht ausführbarer Code wird nicht wie durch einen Menschen auf richtige Ansätze bewertet.

**SQL:** Zwei Punkte für eine ausführbare, erlaubte Anweisung mit den Schreibregeln. Acht Punkte für das vollständig passende Ergebnis beziehungsweise den vollständigen Endzustand auf beiden Datenbeständen. Bei Sortieraufgaben teilen sich diese acht Punkte in sechs für Werte und zwei für die richtige Sortierung auf; Sortierpunkte setzen richtige Werte voraus. Verglichen werden auch NULL, Duplikate und Spaltenreihenfolge. Bei Änderungen zählen unbeteiligte Tabellen und Spaltenstrukturen mit. Der Vergleich zweier Datenbestände ist kein allgemeiner semantischer Beweis. Eine inhaltlich falsche, aber formal gültige SQL-Antwort kann deshalb zwei Formpunkte erhalten; das ist ausdrücklich eine Trainingsregel.

Nach Abgabe bleiben Antworten schreibgeschützt. Teilanforderungen, erreichte Punkte und Musterlösungen sind einsehbar. Codeaufgaben und der Schreibtischtest besitzen eine schrittweise Nachbereitung mit Beispielwerten; SQL kann auf frischen Ausgangsdaten untersucht werden. Die bisherigen Lernaufgaben werden durch eine Prüfungsabgabe nicht als gelöst markiert.

Fehlt SQL bei der Bewertung, bleibt die Abgabe gesperrt gespeichert und erhält keine erfundenen Nullpunkte. Nach Wiederherstellung der Verbindung ist **Auswertung erneut versuchen** möglich. Nur vollständig bewertete Runden landen in der Historie. Gespeichert werden die letzten zwanzig je Modul; der Einstieg zeigt die letzten fünf Zusammenfassungen. Es handelt sich um eine lokale Lernfunktion ohne Beaufsichtigung oder Manipulationsschutz.

## Aufbau und Pflege

* `Core/Exams`: gemeinsame Runden-/Ergebnismodelle.
* `Core/Algorithms/Exams`: eigener Zehnerpool, Auswahl und fachliche Teilprüfung.
* `Core/Sql/Exams`: drei Fallkataloge, SQL-Grenzen und Bewertung; Ausführung in `Sql/Database/TrainingDatabase.Exams.cs`.
* `Web/Exams/ExamService`: verbindet Fachmodule mit der UI.
* `Web/Components/Pages/ExamHome.razor` und `Pages/Training/Exam.razor(.cs)`: Einstieg und Prüfungsablauf.
* `Web/Components/Exams`: Nachbereitung und Spickzettel-Rückweg.
* `ProgressStore.Exams.cs`: atomare Speicherung, Fristablauf, Revisionen und Historie. Im Profil getrennte Felder `ExamRounds` und `ExamHistory`.

Prüfungsinhalte verwenden `ContentVersion = 2026-11-05`. Bei inkompatiblen Änderungen Katalog und Verträglichkeitsprüfung gemeinsam nachführen. Alte Ergebnisprotokolle bleiben erhalten; unpassende offene Aufgabenstände werden ausdrücklich abgewiesen. Neue Inhalte müssen mit unabhängigen Sollwerten, alternativen Lösungen, Grenzen, Fristablauf und gespeichertem Profil geprüft werden.

## Orientierung an den bereitgestellten Unterlagen

Die sieben privaten PDFs von Winter 2021/22, Sommer 2022, dem nicht datierten Softwareprodukt-Dokument, Winter 2023/24, Sommer 2024, Winter 2024/25 und Winter 2025/26 wurden als Form-/Umfangsorientierung ausgewertet. Wiederkehrend sind präzise Datenstrukturen/Funktionsverträge, mehrteilige Bedingungen, Listen und zweidimensionale Daten, Grenzfälle sowie relationale SQL-Aufgaben einschließlich Datenänderungen. Die neuen Aufgaben verwenden eigene Fachfälle, Verträge, Werte, Startdaten und Referenzen. Originalseiten, Aufgabenformulierungen und Lösungsschlüssel werden weder eingebettet noch verteilt. Lokale Extraktionen liegen ausschließlich unter dem ignorierten QA-Verzeichnis `artifacts/exam-source-review`.

Prüfnachweise: [Testprotokoll](testprotokoll.md). Die 45-Minuten-Einteilung ist fachlich geschätzt, aber noch nicht durch einen vollständigen 45-minütigen Endnutzerlauf kalibriert.
