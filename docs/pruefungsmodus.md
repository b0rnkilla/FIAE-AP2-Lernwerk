# Prüfungsrunden und modulbezogene Trainingsprüfungen

In **1.0.0-beta.1** ergänzen die Prüfungsrunden 454 Lernübungen in acht Modulen. SQL und Pseudocode haben 15 beziehungsweise zehn eigene Prüfungsaufgaben. Datenbankentwurf, UML, Softwareplanung, Qualitätssicherung und Projekte bieten jeweils drei Fälle mit je drei Teilen. WiSo nutzt seinen eigenen Fragenbestand. Lernabschlüsse und Prüfungsergebnisse werden getrennt gespeichert.

Historischer Grundstand Alpha 10: SQL-/Pseudocode-Prüfungen ergänzten 220 Lernübungen. Das damalige Alpha-8-Paket enthielt diese Runden noch nicht. Die bisherigen Prüfungsnachweise bleiben im Testprotokoll erhalten.

## Einstieg und Ablauf

Auf Übersicht und in der Modulnavigation führt **Prüfung üben** zum Einstieg. Algorithmen: `/algorithmen/pruefung`, SQL: `/sql/pruefung`. Die aktuelle Runde liegt jeweils unter `/runde`. Die Uhr startet erst mit **Prüfung starten**. Eine noch nicht bewertete Prüfung wird nur nach ausdrücklicher Bestätigung ersetzt; frühere Ergebnisse bleiben in der Historie.

Jede Runde dauert 45 Minuten und bietet 50 Trainingspunkte. 25 Punkte entsprechen 50 Prozent der Trainingspunkte. Dieser Zielwert steht weiterhin in der Auswertung, aber nicht mehr als Zusatztext in den Einstiegsinfos. Die Zeitansätze sind Schätzungen für gezieltes Modultraining, kein nachgewiesener Ersatz für eine vollständige AP2. Vor Abgabe gibt es keine Probeausführung, Lösungskontrolle oder Musterlösung. Aufgaben dürfen frei gewählt und zur Kontrolle markiert werden. Die Auswertung erscheint nach Bestätigung beziehungsweise Fristablauf. Abgabebestätigung und Ergebnis werden sichtbar fokussiert.

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

## Windows-Paket Alpha 9

Am 05.11.2026 aus Anwendungscode-Commit a6c929f als selbstständig startbares Windows-ZIP gebaut und mit eigenem Profil geprüft. Die entpackte EXE liefert in vollständigen Pseudocode- und SQL-Prüfungen jeweils 50/50; Pause und alte Lernstände bleiben erhalten, beide Ergebnisse überstehen einen App-Neustart. Paketdokumente wurden nach dem Anwendungscode-Commit ergänzt. Veröffentlichung übernimmt der Nutzer; die 45-Minuten-Zeitkalibrierung bleibt offen. [Paketdetails](lokale-releases.md).

## Einheitliche Einstiegsseiten seit Alpha 10

In der Übersicht besitzen SQL, Pseudocode und WiSo jeweils Prüfung üben ohne Zeitangabe; Pseudocode/WiSo zeigen den Button in einer eigenen Zeile unter Weiter lernen beziehungsweise ihrem Erststartbutton. SQL benennt den begonnenen Lernweg ebenfalls Weiter lernen. Die eigentlichen Prüfungszeiten bleiben auf den Infoseiten sichtbar: SQL/Pseudocode 45 Minuten, WiSo 60 Minuten.

SQL/Pseudocode verwenden eine Zeilentrennung zwischen Ausgangssituation und Arbeitskopie beziehungsweise Aufgabenarten und separatem Prüfungspool. Der Nachschlagehinweis nennt ausdrücklich den Spickzettel. Zielpunktzahl und Satz zur künftigen Zeitabstimmung entfallen ausschließlich aus der Einleitung; Bewertungsregeln und Grenzen bleiben erhalten. WiSo verwendet dieselbe gemeinsame Info-Komponente auf /wiso/pruefung und startet erst durch Prüfung starten. Seine bestehenden Runden, Historie, Zeitregeln und Speicherung bleiben getrennt von ExamRounds; siehe [WiSo-Vertrag](wiso.md).

## Ergänzung Alpha 11: Datenbankentwurf

Ein weiterer eigener Prüfungseinstieg liegt unter /datenbankentwurf/pruefung. Drei unbekannte Fälle enthalten jeweils drei Teilaufgaben, 45 Minuten und 50 Trainingspunkte. Zwei Modellaufträge werden auf je 20 Punkte gewichtet, eine fachliche Auswahl auf zehn. Das Profil verwendet DesignExam und DesignHistory mit strukturierten Antworten; ExamRounds für SQL/Pseudocode und BusinessRound für WiSo bleiben unberührt. Die Uhr startet erst durch die Startaktion, Abgaben sind endgültig, Nachschlagen pausiert und kennzeichnet die Runde. [Ausführlicher Fachvertrag](datenbankentwurf.md).

## Ergänzung Alpha 13: UML und Modellierung

/uml/pruefung bietet drei neue eigene Fälle mit je drei unabhängig modellierbaren Aufträgen, 45 Minuten und 50 Trainingspunkten (20/20/10). Fünf UML-Diagrammarten werden fallabhängig kombiniert; alle Prüfungsaufträge beginnen leer. Eigene Felder UmlExam/UmlHistory neben den bisherigen Prüfungsformaten. Start, Ersetzungsbestätigung, Pause, UTC-Frist, Nachschlagen, Kontrollmarkierung und endgültige Abgabe folgen dem vorhandenen Bedienprinzip. Keine Punkte im UML-Lernen. Bewertung anhand klarer Fachrollen und geprüfter Varianten, keine allgemeine UML-Beweisführung. [UML-Fachvertrag](uml.md).

## Ergänzung Alpha 14: Softwareplanung und OOP

`/softwareplanung/pruefung` enthält drei eigene Fälle (Fahrradverleih, Bildungszentrum, Reparaturannahme), jeweils drei unabhängig lösbare Teilaufträge, 45 Minuten und 50 Trainingspunkte (20/20/10). Anforderungen, Klassen/Architektur, API-Nachrichten, Formulare und Musterentscheidungen werden fallabhängig kombiniert. `SoftwareExam`/`SoftwareHistory` bleiben zusätzlich neben allen bisherigen Modulständen. UTC-Frist, Pause, Nachschlagekennzeichnung, Kontrollmarkierung, Ersetzungsbestätigung und unveränderliche endgültige Abgabe folgen dem bestehenden Bedienprinzip. Der offizielle Bereich Planen eines Softwareproduktes dauert 90 Minuten; diese Runde ist ein begrenztes Modultraining. [Softwareplanungs-Vertrag](softwareplanung.md).

## Ergänzung Alpha 15: Tests, Qualität und Sicherheit

`/qualitaet/pruefung` bietet Paketstation, Werkzeugverleih und Terminportal. Je drei unabhängige Teile, 45 Minuten, Gewichte 20/20/10, insgesamt 50 eigene Trainingspunkte. Eigene Testfälle mit Sollwerten, konkrete Reviewbefunde, Matrix und Ablaufaufträge. Ausführungsergebnisse, Erklärungen und Referenzen bleiben bis zur endgültigen Abgabe verborgen. Offene Runde nur nach Bestätigung ersetzen; UTC-Frist, Pause, Spickzettelkennzeichnung, Kontrollmarkierungen und endgültige Sperre entsprechen den vorhandenen Modulen.

QualityExam/QualityHistory bleiben neben den bisherigen Modulprofilen. Eine abgelaufene Frist übernimmt ausschließlich zuvor gespeicherte Antworten; eine Abgabe zählt einmal. Die lokale Werkstatt prüft eigene Klassenvertreter, begrenzte S-/D-/Pfadabdeckung und ausdrücklich benannte Fehler, keine vollständige Fehlerfreiheit. Einzelheiten: [Qualitätsvertrag](qualitaet.md).

## Projekte und Wirtschaftlichkeit seit Alpha 16

`/projekte/pruefung` startet nur nach Aktion einen der drei eigenen Fälle Bibliothek, Sportverein und Museum. Drei unabhängige Teile, 45 Minuten und 50 Trainingspunkte (20/20/10). Pläne werden selbst aufgebaut oder ergänzt, Fachwerte in Tabellen eingetragen. Alle gültigen Ressourcen-/Terminvarianten innerhalb des Auftrags gelten; Musterlösungen sind Beispiele. Vor Abgabe keine Berechnung gesuchter Netzplanwerte, Prüfung oder Lösungsvorschau. Pause blendet Aufgaben aus; Nachschlagen pausiert und kennzeichnet Unterstützung. Revision, UTC-Budget, unveränderliche Abgabe und einmalige Historie unter `ProjectExam`/`ProjectHistory`. Keine Zusatzbegründung. Grenzen und Quellen: [Projekte](projekte.md).
