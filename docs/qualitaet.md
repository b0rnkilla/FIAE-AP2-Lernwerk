# Tests, Qualität und Sicherheit – Abschnitt 06

Die Quellfassung **1.0.0-beta.1** umfasst 54 Lernaufgaben, acht feste lokale Testmodelle und drei Prüfungsfälle mit je drei Teilen. Der Inhaltsvertrag gilt seit **2026-11-11**. Abschnitt 06 ist seit Alpha 15 im Windows-Paket enthalten.

## Lernweg

Mira König bereitet nach der Softwareplanung den Probebetrieb im Kulturzentrum Nordhafen vor. Jeder Auftrag beschreibt seine nötigen Voraussetzungen eigenständig. Der Vorgänger erscheint nur als bestätigt, wenn er tatsächlich abgeschlossen wurde; sonst führt ein Link zum offenen Schritt. Erkenntnisse werden erst nach Erfolg gezeigt. Der Prüfbericht enthält ausschließlich bestätigte Schritte bis zur aktuellen Aufgabe und verlinkt ausgelassene Bereiche.

| Aufgaben | Kapitel | Eigene Arbeit |
| --- | --- | --- |
| 01–06 | Qualität beurteilen | Messbares Qualitätsziel, konkrete Reviewbefunde, statische/dynamische Prüfung und Grenzen eines Nachweises |
| 07–12 | Tests planen | Teststufen/-arten, vorgegebenes Risiko, Stub/Mock und Abnahme anhand konkreter Ergebnisse |
| 13–18 | Blackbox-Testfälle entwickeln | Eigene Klassen-/Grenztests, Entscheidungstabelle, Zustandsübergänge und Tarifkombinationen |
| 19–24 | Whitebox und Überdeckung | Eigene Testsätze für Anweisungen, bedingte Zweige und erreichbare Pfade; Überdeckung berechnen |
| 25–30 | Unit-Tests verbessern | Arrange–Act–Assert, unabhängige Sollwerte, wirkungslose Assertion, Isolation und fehlerhafte Bestandsdaten |
| 31–36 | Fehler und Freigaben | Bericht vervollständigen, Nachtest/Regression und Prüfkette |
| 37–42 | Daten und Zugriffe schützen | CIA-Schutzziele, Authentifizierung/Autorisierung, eigene Rechte-Matrix und negative Zugriffstests |
| 43–48 | Webanwendungen absichern | Defensive SQL-/XSS-Reviews, Parameter/Bezeichner, serverseitige CSRF-Prüfung, Sperrgrenzen und Schlüsselverlust |
| 49–54 | Kryptografie und Betrieb | Verschlüsselung/Hash/Signatur, vereinfachter hybrider Ablauf, Passwortspeicherung, Zertifikate und Restoreplan |

Sechs Arbeitsformen: Auswahl, Ausarbeitung, Reviewmarkierungen, Testwerkstatt, Berechtigungsmatrix und zusammensetzbare Abläufe. Der Ablaufbaukasten verwendet Hinzufügen, Entfernen und Früher/Später statt Ziehen. Die Aufgaben geben die entscheidenden Abhängigkeiten ausdrücklich vor; eine feste Referenzfolge wird nicht als universeller Projektablauf behauptet. Zusätzliche Freitextfelder und zugehörige Selbstkontrollkarten sind auf Nutzerwunsch vollständig entfernt.

## Testwerkstatt und Bewertungsgrenzen

Eigene Testfälle enthalten typisierte Eingaben und einen selbst bestimmten Sollwert. Es wird **keine Gleichheit mit den Zahlen der Musterlösung** verlangt: Andere geeignete Klassenvertreter sind zulässig. Explizit verlangte Grenzen bleiben verbindlich. Jeder Fall startet mit frischen lokalen Daten; Zustandsaufgaben geben den Ausgangszustand pro Zeile vor. Es gibt keine beliebige Codeausführung, keine echten Angriffe, Datenbankabfragen oder externen Netzwerkzugriffe.

| Modell | Vertrag | Beobachtbare Besonderheit |
| --- | --- | --- |
| Personenzahl | Ganze Zahl, gültig 1 bis 24 einschließlich | Ungültige Klassen und vier Bereichsnachbarn |
| Gruppenbeitrag | 1–24 Personen, 1200 Cent je Person, 20 % Nachlass nur für Mitglieder ab zehn Personen | UND-Regel und Nachlassgrenze |
| Werkzeuggebühr | 1–14 Tage, sieben Euro je Tag, einmalig 20 Euro bei Schaden | Frühe Ablehnung und optionaler Zuschlag |
| Versand | 1–5000 Gramm, bis 1000 Gramm vier Euro, darüber acht, Express plus drei | Fünf gezählte Anweisungen, sechs bedingte Zweige, fünf erreichbare Pfade |
| Objektzugriff | Verwaltung immer; Mitglied nur am eigenen Objekt; Gast nie | Verweigerte Fremdzugriffe und weitergehende Verwaltungsrolle |
| Anmeldeversuch | Ab drei vorherigen Fehlversuchen gesperrt, sonst abhängig vom richtigen Passwort | Zeilen verändern keinen gemeinsamen Zähler |
| Buchungsübergang | Angefragt bestätigen oder nicht stornierte Buchung stornieren; andere Kombinationen ablehnen | Jede Zeile nennt Zustand und Ereignis |
| Verfügbarkeit | Kapazität positiv, Buchungszahl zwischen null und Kapazität; sonst -1 | Zufällige Ergebnisgleichheit einer Fehlervariante erkennen |

Eingaben müssen im angegebenen **Testdatenbereich** liegen und den Datentyp erfüllen. Fachlich ungültige Werte innerhalb dieses Bereichs sind ausdrücklich testbar, um die Ablehnung zu prüfen. Zahlen-Sollwerte akzeptieren äquivalente Dezimalschreibweisen mit Punkt/Komma; Wahrheitswerte und benannte Textausgaben besitzen die erklärten gleichwertigen Schreibweisen. Die Oberfläche bietet die stabilen Auswahlwerte an. Unvollständige Zeilen bleiben speicherbar, bestehen aber keine vollständige Prüfung.

Eine Lösungsprüfung ermittelt getrennt:

- gültige Eingaben und fachlich richtige Sollwerte für jede Zeile;
- geforderte Klassen und Grenzen;
- tatsächlich ausgeführte gezählte S-Anweisungen;
- tatsächlich ausgeführte Wahr-/Falsch-Ausgänge der D-Entscheidungen;
- vollständige erreichbare Folgen dieser D-Ausgänge;
- durch richtige Sollwerte erkannte ausdrücklich benannte Fehlervarianten.

**Falsche Sollwerte zählen nie als Fehlernachweis.** Tatsächlich ausgeführte Marken zählen trotzdem zur Abdeckung; Abdeckung und richtige Assertions sind verschiedene Nachweise. Die Prozentanzeige nennt ihren Nenner ausdrücklich. Sie zählt hier nur markierte S-Anweisungen und bedingte D-Ausgänge, keine zusätzlichen unbedingten Kontrollflusskanten oder kurzgeschlossenen Teilbedingungen. Die Modelle sind klein und schleifenfrei; ihre Pfade sind vollständig benannt. Vollständige Überdeckung beweist keine allgemeine Fehlerfreiheit.

Beim Verfügbarkeitsmodell liefert eine fehlerhafte Subtraktion bei genau einem Platz Überbelegung zufällig dieselbe -1 wie die korrekte Ablehnung. Ein weiter überbelegter Vertreter unterscheidet beide. Diese Eigenschaft ist Teil des Auftrags und seiner Hinweise, kein versteckter Vergleich gegen eine einzige Referenzzahl.

Review/Choice prüft die vollständige Optionsmenge ohne zusätzliche oder doppelte Kennungen. Ausarbeitungen prüfen die ausdrücklich beschrifteten Fachwerte. Die Matrix prüft jede Kombination aus Rolle, Aktion und Objektbezug. Abläufe prüfen die vollständige vorgegebene Folge. Eine allgemeine Prüfung beliebiger Sicherheitskonzepte, Testprogramme oder Normkonformität ist nicht enthalten.

## Bedienung

`/qualitaet` setzt die letzte eigene Aufgabe fort; `/qualitaet/{id}` ist der Direkteinstieg. Gemeinsame linke TaskRail und LearningModePicker, Auftrag/Arbeitsbereich in der Mitte und bestätigter Prüfbericht rechts entsprechen den anderen Lernmodulen. Neue Aufgaben starten ohne Hilfe. Geführtes Lernen bietet drei Hinweise, Spickzettel mit validiertem lokalem Rückweg und bewusst aufgedeckte Musterlösung. Der Wechsel zur Unterstützung erhält den Entwurf. Ein neuer Selbsttest verlangt bei eigenem Entwurf die gemeinsame Bestätigung und setzt nur den aktuellen Versuch zurück; historische Erfolge bleiben bestehen.

Leere Antworten sperren die Prüftaste. Nach einer Änderung verschwindet veraltetes Feedback samt Testausführung. Die Lernrückmeldung enthält genau eine Ergebniskarte **ohne Trainingspunkte, Prozentnote oder IHK-Zusatz**. Die optionale technische Auswertung der Testwerkstatt zeigt die tatsächlich berechneten Ergebnisse je Fall, erkannte Fehlervarianten, Wege und gemeinsame Abdeckung. Diese Abdeckungsprozente sind keine Lernnote. Ergebnisse erscheinen erst nach „Lösung prüfen“, Prüfungsdaten erst nach endgültiger Abgabe.

Testfälle können hinzugefügt, entfernt und umgeordnet werden. Maximalzahl je Aufgabe sechs, acht oder zwölf, Speichergrenze 16. Felder und sämtliche Handler beachten Schreibsperren und Größenlimits. Lange Überschriften brechen um; breite Matrizen und Ergebnistabellen besitzen einen eigenen Scrollbereich. Hell-/Dunkelansicht und mobile Bedienung verwenden das vorhandene Design.

## Trainingsprüfungen

`/qualitaet/pruefung` bietet **Paketstation**, **Werkzeugverleih** und **Terminportal** mit je drei unabhängig lösbaren Teilaufträgen. 45 Minuten, 50 Trainingspunkte, Gewichte **20/20/10**. Die interne Bewertung jeder Aufgabe verwendet 50 Kriterienanteile und wird entsprechend gewichtet; das ist keine amtliche IHK-Note.

Start erst nach Aktion; offene Runde nur nach Bestätigung ersetzen. UTC-Frist läuft beim Browserende weiter. Pause blendet Aufgaben und Antworten aus. Nachschlagen pausiert und kennzeichnet Unterstützung. Kontrollmarkierungen, aktueller Teilauftrag, Entwürfe und Restzeit bleiben beim Neustart erhalten. Nach Ablauf zählen nur rechtzeitig gespeicherte Antworten. Eine endgültige Abgabe wird gesperrt und zählt genau einmal in die auf zwanzig Einträge begrenzte Historie. Die automatische Speicherung erhält die Identität der bearbeiteten Testzeilen auch bei unmittelbar folgenden Eingaben.

## Technik und Profil

`Core/QualityAssurance` gliedert sich in `Models`, `Content`, `Evaluation` und `Practice`. Eingebettete `lessons.json`, `programs.json` und `exams.json`; `QualityCatalog` prüft Umfang, Kennungen, Ziele und Referenzen beim Laden. `QualitySimulator` führt acht ausdrücklich implementierte Modelle aus. `QualityEvaluator` trennt Ausführung, Sollwerte, Abdeckung und Fehlervarianten. Keine Erweiterung des Pseudocode-Interpreters oder der SQL-Ausführung.

Zusätzliche Profilfelder **QualityLessons**, **LastQualityLessonId**, **QualityExam** und **QualityHistory** liegen neben sämtlichen bisherigen Modulständen. Alte Dateien ohne diese Felder laden mit leeren Standardwerten. Das frühere Profilfeld Reasoning wird ausschließlich zur verlustfreien Übernahme alter Dateien erhalten, nicht angezeigt oder bearbeitet; sein Inhalt zählt nicht als eigener neuer Entwurf. Revisionsschutz, gemeinsame atomare Dateisperre, begrenzte gespeicherte Ausführungsdaten und unveränderliche Prüfungshistorie bleiben erhalten. Keine Inhaltsversion anderer Module und keine bestehenden Kennungen geändert.

## Quellen und Grenzen · geprüft am 11.11.2026

- [Ausbildungsordnung § 13](https://www.gesetze-im-internet.de/fiausbv/__13.html) und [§ 14](https://www.gesetze-im-internet.de/fiausbv/__14.html): Qualitätssicherung beziehungsweise Testdaten und Testszenarien. Offizielle schriftliche Bereiche jeweils 90 Minuten; unsere 45-Minuten-Runden sind begrenztes Modultraining.
- [IHK Hannover, Prüfungskataloge ab 2025](https://www.ihk.de/hannover/hauptnavigation/ausbildung-und-weiterbildung/ausbildung/ausbildung-a-z/neuordnungen/pruefungskataloge-it-berufe-6438900): aktuelle Katalogorientierung; keine vollständige Abdeckung behauptet.
- [ISTQB CTFL 4.0, Lehrplan 4.0.1](https://istqb.org/certifications/certified-tester-foundation-level-ctfl-v4-0/): Testentwurf, Überdeckung, Teststufen und statische Prüfung. Die vereinfachte Zählkonvention der Werkstatt ist oben ausdrücklich abgegrenzt.
- [ISO/IEC 25010:2023](https://committee.iso.org/cms/live/live/en/sites/isoorg/contents/data/standard/07/81/78176.html): aktuelle Fassung des Produktqualitätsmodells mit neun Merkmalen. Kein Vermischen mit dem älteren Acht-Merkmale-Modell oder ISO 9126, keine Wiedergabe der vollständigen Norm.
- OWASP: [Autorisierung](https://cheatsheetseries.owasp.org/cheatsheets/Authorization_Cheat_Sheet.html), [SQL-Injection-Prävention](https://cheatsheetseries.owasp.org/cheatsheets/SQL_Injection_Prevention_Cheat_Sheet.html), [XSS-Prävention](https://cheatsheetseries.owasp.org/cheatsheets/Cross_Site_Scripting_Prevention_Cheat_Sheet.html), [Passwortspeicherung](https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html). Aktuelle konkrete Passwort-Kostenparameter gehören zu einer späteren echten Implementierungsprüfung; keine eigene Passwortverwaltung im Trainer.

Bereitgestellte alte Prüfungen aus Teil 1/2 dienen ausschließlich als private Orientierung für Aufgabenformen und Lücken. Texte, Daten, Diagramme und Fälle dieses Moduls sind eigene Inhalte. Keine Originalprüfungsdateien werden eingebettet oder verteilt. Ausführliche Nutzererprobung, praktische Zeitkalibrierung und eine vollständige Katalogzuordnung bleiben offen. Durchgeführte Nachweise stehen im [Testprotokoll](testprotokoll.md).

## Nachlauf: zusätzliche Freitextfelder entfernt

Am 11.11.2026 auf Nutzerwunsch vollständig aus Lern-/Prüfungsansichten entfernt, einschließlich der zugehörigen Selbstkontrollkarten und Kriterienlisten. Aufgaben 31/54 verlangen keine zusätzliche schriftliche Begründung mehr. Bewertete Fachangaben und Inhaltsversion bleiben gleich; ältere Profile werden ohne Datenverlust übernommen. Einzelheiten und Nachweis im Testprotokoll.

## Native Paketprüfung Alpha 15

Direkt aus dem ZIP auf eigenem Port 5244 ohne SQL: eigene Klassenvertreter -8/17/39 akzeptiert, falscher Sollwert abgelehnt und alte Ausführung bei Antwortänderung verworfen. Reparatur 33 mit eigenem vierten Testfall korrekt, Ausgangskopie unverändert. Paketstationsprüfung vollständig per Oberfläche mit alternativen Versandwerten bearbeitet, bei 44:39 pausiert und nach EXE-Neustart mit Entwürfen/Markierung fortgesetzt; endgültig 50/50 und gesperrte Antworten. Alle bisherigen synthetischen Modulfelder einschließlich altem unsichtbarem Reasoning-Text erhalten. Kein Zusatzfreitext in Lernen/Prüfung; mobile Seite innerhalb der Bildschirmbreite. Einzelheiten und Grenzen: Testprotokoll. ZIP/SHA256 unverändert erhalten; Veröffentlichung übernimmt der Nutzer.

## Unveränderter Modulstand im Alpha-16-Paket

Das nach Nutzer-Push aus 1dbcb12 gebaute und geprüfte Alpha-16-ZIP enthält dieses Modul unverändert sowie Abschnitt 07 Projekte und Wirtschaftlichkeit; insgesamt 454 Übungen in acht Modulen. Historische Profilfelder blieben im isolierten nativen Pakettest erhalten. Details: [lokale Releases](lokale-releases.md), [Testprotokoll](testprotokoll.md). Alpha 16 wurde vom Nutzer veröffentlicht.
