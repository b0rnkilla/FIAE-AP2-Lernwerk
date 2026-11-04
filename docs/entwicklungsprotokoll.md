# Entwicklungsprotokoll

## Umgesetzt

### 02.11.2026 – Erste SQL-Version

* Nutzeranforderungen und die drei Vorlagendokumente ausgewertet; allgemeine Coding-Konventionen und Dokumentationsstruktur übernommen, fremde Projektinhalte abgegrenzt.
* Vorhandene .NET-SDKs und lokale SQL-Express-Instanz ermittelt; SQL-Verbindung mit Windows-Anmeldung erfolgreich geprüft.
* C#, ASP.NET Core Blazor, .NET 10 und Microsoft SQL Server als technische Grundlage festgelegt.
* Projektmappe mit Core-Klassenbibliothek, Blazor-Webanwendung und Testprojekt erstellt.
* Englische Codebezeichner, deutsche Lerninhalte und Dokumentation sowie Nullable Reference Types eingeführt.
* Projektlokale öffentliche Paketquelle konfiguriert, damit keine Anmeldung an der vorhandenen Firmenpaketquelle benötigt wird.
* Microsoft.Data.SqlClient und Microsoft ScriptDom eingebunden; eine anfänglich gemeldete verwundbare transitive Kryptografie-Abhängigkeit auf eine aktuelle Version angehoben.
* Eigenes deutsches Szenario „Fallakte Nachtzug – Die verschwundene Lieferung“ mit sechs zusammenhängenden Tabellen und synthetischen Datensätzen entworfen.
* Feste, gekennzeichnete Trainingsdatenbank mit idempotenter Initialisierung und versioniertem Schema umgesetzt.
* Zusätzlichen abweichenden Prüfdatenbestand mit weiteren Personen, Zuordnungen und Grenzfällen angelegt.
* 30 Missionen in fünf Kapiteln erstellt: Grundlagen, Filter, JOINs, Aggregation und Unterabfragen.
* Für jede Mission Lerntext, eindeutigen Auftrag, drei Hinweise, Referenzlösung, Erklärung und eine neue Erkenntnis hinterlegt.
* SQL-Syntaxprüfung über den T-SQL-Parser anstelle unsicherer Schlüsselwort- oder Textvergleiche umgesetzt.
* SQL-Ausführung auf einen Benutzer ohne Login und ohne Schreibrechte eingeschränkt; unwiderruflichen Kontextwechsel, nicht gepoolte Verbindungen, Zeitlimit und Zeilenlimit integriert.
* Ergebnisauswertung mit Erhalt von NULL, Duplikaten und relevanter Sortierung umgesetzt; numerisch kompatible Typen erlaubt, reine Textwerte nicht als Zahlen behandelt.
* Zusätzliche Bewertung auf Prüfdaten über eine gezielte Änderung echter Tabellenreferenzen im Syntaxbaum umgesetzt; Textliterale werden dabei nicht ersetzt.
* Hauptmenü mit aktivem SQL-Modul und als „In Vorbereitung“ gekennzeichneten weiteren Themen erstellt.
* Deutsche Blazor-Oberfläche mit Missionsauswahl, Tabellenübersicht, Abfrageeditor, Ergebnistabelle, Hinweisen und erspielten Erkenntnissen umgesetzt.
* Geführtes Lernen und eigenständiges Lösen mit leerem Entwurf und ohne Probeausführung getrennt.
* Lokalen Lernstand mit Entwürfen, Hinweisen, Abschlüssen und eigenständigen Wiederholungen atomar in JSON speicherbar gemacht.
* Lernplan, SQL-Spickzettel, kontrolliertes Zurücksetzen des lokalen Profils und verständliche Fehlerseiten ergänzt.
* Browserzugriff auf den lokalen Rechner beschränkt; öffentliche Bereitstellung nicht eingerichtet.
* Inhalte gegen die echte SQL-Express-Instanz getestet: alle 30 Musterlösungen bestehen beide Datenbestände.
* Unabhängige Zeilen-/Spaltenerwartungen, Aggregatwerte und die Fallauflösung in automatisierten Tests geprüft.
* Typische fachliche Fehler und alternative korrekte Formulierungen in die Tests aufgenommen.
* Zugriffsschutztests präzisiert: Öffentliche SQL-Server-Systemmetadaten sind teilweise auch ohne zusätzliche Rechte sichtbar; der Parser verbietet solche Fremdbezüge unabhängig davon. Nicht öffentliche Datenbankzugriffe und Schreibbefehle wurden zusätzlich unter dem eingeschränkten Benutzer geprüft.
* Im Browser SQL-Ausführung, erfolgreiche Bewertung, LEFT JOIN mit NULL-Ergebnissen, Hinweise, Moduswechsel und Persistenz nach Neuladen geprüft.
* Weitere Tests für Entwürfe, Hinweise, konkurrierende Fortschrittsspeicherung, beschädigte Fortschrittsdateien und isoliertes Zurücksetzen ergänzt; insgesamt 39 automatisierte Prüfungen erfolgreich.
* Desktop- und schmale Browserdarstellung sowie deutsche Fehlerhinweise bei falschen Spaltennamen geprüft.
* Startdatei, Projektsteckbrief, angepasste Coding Guideline, Architektur-/Lernkonzept und Testprotokoll erstellt.

---

### 02.11.2026 – Zweite Kontrolle und SQL-Werkstatt

* Bestehende 39 Tests vor der Erweiterung erneut erfolgreich ausgeführt.
* Zusatzprüfung für vollständig ausgeschriebene Spalten- und Sternbezüge korrigiert; Bezüge wie `fall.Sendungen.Id` werden gemeinsam mit der Tabelle auf die Prüfdaten umgeleitet.
* Numerischen Vergleich für sehr kleine Gleitkommazahlen korrigiert; ein echter Wert wie 1e-100 darf nicht durch Dezimalrundung als null gelten.
* Gemeinsame SQL-Sicherheitsprüfung eingeführt und weitere externe oder spezielle Tabellenquellen ausgeschlossen.
* Laufende Ermittlungsabfragen bei Seitenwechseln abbrechbar gemacht; verspätete Ergebnisse werden nicht der nächsten Mission zugeordnet.
* Technische Verbindungs-/Referenzfehler von fachlich falschen Antworten getrennt; sie erhöhen die fachliche Versuchszahl nicht.
* Fortschrittsdateien zusätzlich auf inhaltlich beschädigte Strukturen geprüft, etwa NULL-Missionseinträge oder ungültige Hilfestufen. Solche Dateien werden nicht still überschrieben.
* SQL-Werkstatt mit zehn freien Aufgaben für Datenänderungen, Tabellen-/Spaltenanlage, PRIMARY KEY, NOT NULL, CHECK, COMMIT und ROLLBACK umgesetzt.
* Schreibversuche auf ausschließlich sitzungseigene temporäre Tabellen umgeleitet; vorhandene permanente Tabellen und Benutzerrechte bleiben unverändert.
* Jede Werkstattausführung mit frischen Ausgangsdaten gestartet; alle logischen Tabellennamen bleiben im Editor normale SQL-Namen ohne technisches temporäres Präfix.
* Endzustände auf zwei Ausgangsdatenbeständen sowie Spalten, Datentypen und Constraints durch tatsächliche Einfügeversuche geprüft.
* Probeänderungen vor ROLLBACK gesondert erfasst, damit unveränderte oder falsch berechnete Scheinlösungen nicht als korrekte Rücknahme gelten.
* Gleichzeitige Werkstattversuche einschließlich benannter Constraints auf getrennte Sitzungen geprüft.
* Werkstattentwürfe und -fortschritte getrennt von der Ermittlungsakte gespeichert; bestehende Profilversionen bleiben lesbar.
* Hauptmenü, Navigation, SQL-Spickzettel und Lernplan um die Werkstatt ergänzt.
* Insgesamt 69 automatisierte Tests einschließlich echter SQL-Integration erfolgreich ausgeführt; alle 40 Musterlösungen bestehen beide Datenbestände.
* Aktuelle Paketprüfung durchgeführt; gemäß den verwendeten Quellen keine bekannten verwundbaren direkten oder transitiven Abhängigkeiten festgestellt.
* Browserprüfung mit separatem Testport und Test-Lernprofil durchgeführt: falsches/korrektes UPDATE, freie Tabellenanlage, Datenregeln, ROLLBACK-Zwischenstand, Moduswechsel und Wiederladen erfolgreich geprüft.
* Abbruch einer laufenden Abfrage durch Modul- sowie Missionswechsel im Browser geprüft; anschließende korrekte Abgabe mit ausgeschriebenen Spaltennamen erfolgreich.
* Schmale Werkstattdarstellung bei 390 Pixeln ohne horizontales Überlaufen geprüft.

---

### 02.11.2026 – Bedienkorrekturen und allgemeiner AP2-Einstieg

* Rückmeldung zum Cursor anhand des bereitgestellten Bildschirmfotos und der bestehenden Editorsteuerung untersucht.
* Ermittlungs- und Werkstatteditor während der Ausführung auf readonly statt disabled umgestellt, damit Lesbarkeit und Auswahl erhalten bleiben.
* Ausdrückliche Cursorfarbe, einen kontrastreichen I-förmigen Mauszeiger und das zum Design passende native Browserfarbschema ergänzt.
* Helle und dunkle Farbpaletten für alle Seiten, Editoren, Tabellen, Hinweise, Rückmeldungen und Hilfetexte eingeführt.
* Auswahl System/Hell/Dunkel in der Kopfzeile ergänzt; automatische Systemerkennung, Nachführung und manuelle Überschreibung umgesetzt.
* Designauswahl im Präferenzcookie gespeichert; ungültige Werte und gesperrte Cookies kontrolliert behandelt und HTTPS-Secure-Merkmal ergänzt.
* Auswahl nach erneutem Laden und nach Blazors Aktivierung der Oberfläche synchronisiert.
* Startseite als allgemeinen AP2-Lernraum mit gleichrangigem Themenkompass gestaltet; SQL-Einstieg auf eine verfügbare Themenkarte mit zwei Lernwegen konzentriert.
* Gesamtfortschritt aus Ermittlungs- und Werkstattaufgaben zusammengeführt und im separaten Testprofil mit zwei erfolgreichen Aufgaben geprüft.
* Schemapräfix fall. für bekannte Falltabellen optional gemacht; kurze Namen vor Ausführung und Zusatzprüfung gezielt im Syntaxbaum aufgelöst.
* Aliase, JOINs und Unterabfragen mit kurzen sowie gemischten Tabellennamen geprüft; fremde Schemas bleiben gesperrt.
* Alle 30 Ermittlungslösungen zusätzlich ohne Schemapräfix auf beiden Datenbeständen geprüft.
* 75 .NET-Tests und sechs zusätzliche JavaScript-Tests zur Designwahl erfolgreich ausgeführt.
* Browserprüfung mit separater Release-Fassung, Testport, Testprofil und anderem Hostnamen durchgeführt; bestehender Lernbetrieb wurde nicht beendet.
* Helle Darstellung bei dunkler Systemeinstellung, gespeicherte Auswahl nach Neuladen, Rückkehr zur Systemeinstellung, Editor während laufender Abfrage und dunkle Ergebnisanzeige erfolgreich geprüft.

---

### 02.11.2026 – Blaues Design und verständlichere Lernführung

* Helle Palette auf Blau und Dunkelpalette auf Grau-Blau umgestellt; die bisherige grüne Akzentfarbe einschließlich Cursor, Fortschritt, Ergebnissen und Symbol wurde ersetzt.
* Kräftige serifenlose Überschriften, abgerundete Karten und Schaltflächen, sichtbare Navigationsauswahl und besser lesbare Hilfstexte eingeführt. Nicht mehr genutzte Gestaltung der alten SQL-Startseite entfernt.
* Den entbehrlichen Satz zu Sendungen/fall.Sendungen aus der Tabellenübersicht entfernt; die zugrunde liegende SQL-Funktion bleibt erhalten.
* Beide SQL-Lernwege starten jetzt mit „Ohne Hilfe lösen“. Die gemeinsame Komponente LearningModePicker erklärt beide Modi und nennt die geführte Alternative ausdrücklich.
* Geführtes Lernen übernimmt den Entwurf. Ein neuer Selbsttest bietet vor dem Leeren die Wahl „Neuen Versuch starten“ oder „Entwurf behalten“.
* Unterstützung pro Versuch wird dauerhaft gespeichert. Unterstützte und ältere gespeicherte Entwürfe können durch Neuladen keinen selbstständigen Erfolg erhalten. Neue Selbsttests erhalten vorhandene Abschlüsse und Zähler.
* „Lösung prüfen“ ersetzt die endgültig klingende Abgabe; leere Lösungen können nicht versehentlich zur Prüfung gesendet werden.
* Auftrags- und Editorbereich mit Lernschritten beschriftet, Tabellenzugriff durch Sprunglinks vereinfacht und Fehleranzeige während des Ladens durch einen verständlichen Ladezustand ersetzt.
* Fünf zusätzliche .NET-Testfälle für unterstützte und selbstständige Entwürfe, ältere Fortschrittsdateien und das Beibehalten vorhandener Erfolge ergänzt.
* Browserabläufe mit separatem Testprofil geprüft: geführter Versuch, Hinweis, Entwurf behalten, bestätigter Neustart, korrekte selbstständige Abgabe sowie Werkstatt-Probeausführung und -Bewertung. Die Übersicht zeigt danach korrekt zwei selbstständige Abschlüsse.
* Helles und dunkles Design sowie schmale Darstellung bei 390 Pixeln visuell geprüft; bestehendes Nutzerprofil nicht für Tests verwendet.
* Fragmentlinks mit dem Blazor-Basispfad abgeglichen und in beide Richtungen geprüft. Moduswahl und Editorstatus mit ausdrücklichen ARIA-Zuständen versehen; zwei Renderingtests sichern die Moduswahl ab.
* Abschließender Build ohne Warnungen/Fehler; 82 .NET-Tests inklusive SQL und sechs JavaScript-Tests erfolgreich.

---

### 02.11.2026 – Anführungszeichen bei SQL-Textwerten

* SQL-Server-Verhalten anhand der Microsoft-Dokumentation zu SET QUOTED_IDENTIFIER geprüft. Unter ON bezeichnen doppelte Anführungszeichen Objekt-/Spaltennamen, einfache Anführungszeichen Textwerte.
* QUOTED_IDENTIFIER ON ausdrücklich je Trainingssitzung gesetzt, passend zur bestehenden Syntaxprüfung; keine globale Servereinstellung verändert.
* Bei unbekannten Spalten und tatsächlich doppelt zitierten Spaltenbezügen erscheint ein deutscher Hinweis mit WHERE Status = 'verschwunden'. Gültige zitierte Spalten bleiben erlaubt.
* Die Erkennung verwendet den Syntaxbaum; Kommentare, Textinhalte und reine Ausgabealiase lösen den Zusatzhinweis nicht aus.
* Ermittlungsakte und SQL-Werkstatt verwenden dieselbe Fehlerrückmeldung. SQL-Spickzettel um Anführungszeichen und eingebettete Apostrophe ergänzt.
* Zwei zusätzliche SQL-Integrationstests prüfen den gemeldeten Fall, die richtige Schreibweise, zitierte Spalten und störende Anführungszeichen in anderen SQL-Elementen. Alle 84 .NET-Tests erfolgreich; keine neuen Abhängigkeiten.
* Quelle: https://learn.microsoft.com/de-de/sql/t-sql/statements/set-quoted-identifier-transact-sql?view=sql-server-ver17

---
### 02.11.2026 – Konkrete Syntaxhilfe und motivierender Einstieg

* Gemischte Anführungszeichen aus dem Nutzerbeispiel ('Essen") als unvollständig begrenzten Text erkannt. Gemeinsame Syntaxrückmeldung für Ermittlungsakte und Werkstatt ergänzt.
* Offene Texte, zitierte Bezeichner, eckige Klammern, verschachtelte Blockkommentare und runde Klammern erhalten konkrete deutsche Hinweise mit einem SQL-Ausschnitt. Maskierte Zeichen und Kommentare werden berücksichtigt. Allgemeine Parserfehler zeigen die Umgebung der betroffenen Stelle statt nur Zeile/Spalte.
* Die Hinweise ändern keine Eingabe und ersetzen die echte Syntax- und Sicherheitsprüfung nicht. Bei mehrdeutigen Fehlern bleiben es ausdrücklich Prüfvorschläge.
* Identische Fehlermeldungen nach der Bewertung erscheinen nur einmal; Probeausführungsfehler und echte Ergebnistabellen bleiben sichtbar. Beide SQL-Lernwege angepasst.
* Startseiten-Schaltfläche abhängig vom Ermittlungsstand in Ermittlung starten oder Weiter ermitteln geändert. Leere, beim ersten Seitenaufruf angelegte Einträge allein zählen nicht als Beginn.
* 15 neue Testfälle decken den gemeldeten Fehler, weitere Begrenzungsfehler, allgemeine Syntaxfehler und gültige maskierte Zeichen ab. Alle 99 .NET-Tests einschließlich SQL-Integrationstests erfolgreich, keine Buildwarnungen oder -fehler.
* Browserprüfung mit eigenem Port 5081 und Profil artifacts/qa-feedback-profile: genauer Nutzerfehler, nur eine Meldung, Tabelle nach Syntaxkorrektur, Start-/Fortsetzungsbeschriftung, Werkstattbewertung und Probeausführung geprüft. Normales Nutzerprofil unverändert.

---
### 02.11.2026 – Erfolgshinweis bei übernommenem Selbsttest

* Widersprüchlichen Wiederholungshinweis im Modus Ohne Hilfe lösen untersucht: Ein übernommener oder früher unterstützter Entwurf konnte weiterhin als solcher gekennzeichnet sein, obwohl der aktuelle Modus bereits ohne Hilfen lief.
* Gemeinsame Komponente AttemptOutcomeNote für Ermittlung und Werkstatt ergänzt. Sie unterscheidet geführtes Lernen, einen übernommenen Versuch im Modus ohne Hilfe und einen frischen selbstständigen Versuch.
* Hinweis unter der Moduswahl erklärt jetzt die Herkunft aus geführtem Lernen oder älteren Profilen und dass der Entwurf erst nach bestätigtem Neustart geleert wird.
* Keine gespeicherten Nutzerleistungen umgewertet oder zurückgesetzt. Die Aufgabe zählt bei richtiger Lösung weiterhin als gelöst; die zusätzliche Selbsttestwertung bleibt getrennt.
* Drei Renderingtests sichern die Texte und den konkreten gemeldeten Sonderfall ab. Alle 102 .NET-Tests einschließlich SQL-Integrationstests erfolgreich, ohne Buildwarnungen oder -fehler.
* Browserprüfung mit synthetischem älteren Profil ohne AttemptAssisted: richtige Lösung zunächst mit Completed=true und Independent=false gespeichert. Nach bestätigtem Neustart erschien der reguläre Erfolgshinweis; Completed und Independent waren beide true und der Abgabenzähler blieb erhalten.
* Separate Testinstanz auf Port 5081 mit artifacts/qa-outcome-profile verwendet. Das normale Nutzerprofil blieb unangetastet. Die zunächst am Nutzungslimit gescheiterte Browserprüfung wurde nach erneuter Verfügbarkeit erfolgreich abgeschlossen.

---
### 03.11.2026 – Vorführung im privaten Heimnetz

* Drei bisherige Sperren geprüft: reine localhost-Bindung, lokale AllowedHosts-Liste und zusätzliche Loopback-Zugriffskontrolle.
* Separates Startprofil FiaeTrainerLan und Trainer-im-Netzwerk-starten.cmd ergänzt. Der Standardstart bleibt lokal. Im LAN-Profil werden alle IPv4-Schnittstellen gebunden, Hostnamen/IP-Adressen zugelassen und ausschließlich private TCP-Quelladressen zusätzlich erlaubt.
* NetworkAccessPolicy mit expliziter LAN-Aktivierung eingeführt; IPv4-gemappte IPv6-Adressen normalisiert, private Adressgrenzen und unbekannte/öffentliche Quellen geprüft. Keine pauschale Übernahme von Forwarded-Headern.
* Sichtbaren Hinweis auf den gemeinsamen Lernstand im Heimnetz ergänzt. Keine Konten oder öffentliche Bereitstellung vorgetäuscht; nginx-/Kontenausbau in heimnetz-und-serverbetrieb.md beschrieben.
* Gezielte Windows-Firewallregel FIAE_AP2_Trainer-LAN-5080 nach Windows-Administratorbestätigung eingerichtet: Inbound TCP 5080, Private, RemoteAddress LocalSubnet, Edge Traversal Block. Andere Regeln und Routerkonfiguration unverändert. Wiederverwendbares Einrichtungsskript im Projekt abgelegt.
* Beim echten Verbindungstest fehlte zunächst das Frameworkskript außerhalb von Development. Eigenes LanDemo-Startumfeld mit auf genau dieses Umfeld begrenztem Buildmanifest-Laden ergänzt. Skript, Styles und interaktive Verbindung danach erfolgreich geprüft; keine Buildmanifest-Aktivierung für einen späteren Production-Betrieb.
* 27 neue Adressgrenzen-Testfälle ergänzt; alle 129 .NET-Tests einschließlich SQL bestanden, keine Buildwarnungen oder -fehler.
* HTTP-Prüfung: localhost 200, Heimnetzadresse im lokalen Modus 403, gefälschter Forwarded-Header weiterhin 403. Im LAN-Profil Startseite, CSS, Blazor-Skript und SignalR-Verbindungsaufbau jeweils 200.
* Browserprüfung über die echte Heimnetz-IP mit eigenem QA-Profil: Mission 1 erfolgreich gelöst, Ergebnis und Fortschritt korrekt. Keine Übungsabgaben auf dem normalen Nutzerprofil.
* Getrennte Testinstanz beendet und die geprüfte Vorführung auf Port 5080 mit dem normalen Profil gestartet. Listener auf 0.0.0.0 und Startseite/Frameworkskript über 192.168.1.21:5080 geprüft. Zugriff von einem zweiten physischen Rechner nicht durch den Agenten verifiziert.

---
### 03.11.2026 – Verbesserungspaket A: Aufgabenmodus und Bedienung

* Die elf Nutzerpunkte vor dem Homeserver-Ausbau geprüft und in docs/verbesserungsplan.md auf vier Pakete verteilt. Zunächst nur Punkte 3–6 und 8 umgesetzt; Semikolon, Großschreibung, Beispielbereinigung/Datumsformate, neuer Lernplan und Geschichte bleiben ausdrücklich offen.
* Die letzte Moduswahl als ExamMode je LessonProgress gespeichert, getrennt für Ermittlung und Werkstatt. Neue Aufgaben und alte Dateien ohne Modusfeld starten ohne Hilfe. Die Auswahl wird bei jedem Aufgabenwechsel und beim Neuladen wiederhergestellt; der Modus einer anderen Aufgabe wird nicht mehr übernommen.
* Bestehende Abschlüsse, selbstständige Erfolge, Entwürfe und Versuchshistorie erhalten. Ein späterer geführter Versuch überschreibt einen früheren selbstständigen Erfolg nicht.
* Leerer oder nur aus Leerzeichen bestehender Editor wechselt direkt zum neuen Selbsttest. Bei vorhandenem Text bleiben Neustartbestätigung und Entwurf behalten erhalten.
* Gemeinsame Komponente SelfTestHelp mit zugänglichem X-Button ergänzt. Die profileigene Anzeigeeinstellung gilt für beide SQL-Lernwege und bleibt über Aufgabenwechsel und Neuladen erhalten. Wiederherstellung direkt an der Aufgabe oder unter Lernplan → Bedienhinweise möglich.
* Profilzugriffe in ProgressStore für Aufgaben und Anzeigeeinstellung auf dieselben atomaren Lese-/Schreibmethoden geführt. Gleichzeitige Änderungen bleiben erhalten; beschädigte Dateien werden weiterhin nicht überschrieben.
* Die beiden Tabellen-/Editor-Sprunglinks und ihre ungenutzten CSS-Regeln entfernt. Sichtbare Navigation, Aufgabenliste, Zähler und weitere UI-Texte von Mission auf Aufgabe umgestellt. Inhaltliche Fallgeschichte noch nicht überarbeitet.
* Acht neue Testfälle für individuelle Modi in beiden Lernbereichen, Altprofile, konkurrierende Speicherung, Wiederherstellung, beschädigte Profile und die Hilfekomponente ergänzt. Alle 137 .NET-Tests einschließlich SQL-Integrationstests erfolgreich; abschließend auch als Release-Fassung gebaut und geprüft, ohne Warnungen/Fehler.
* Browserprüfung mit artifacts/qa-package-a-profile und Port 5081: gemeldeten A/B-Wechsel nach selbstständigem Abschluss exakt nachgestellt, neue Aufgaben, gespeicherte Auswahl nach Neuladen, leeren/vorhandenen Entwurf, X, globale Anzeigeeinstellung sowie beide Wege zur Wiederherstellung geprüft. Werkstattwechsel ebenfalls erfolgreich. Nachweis in artifacts/paket-a-browserpruefung.json und artifacts/paket-a-selbsttesthinweis.jpg.
* README, Projektsteckbrief, Lernkonzept, Testprotokoll, Heimnetz-Dokumentation und Coding Guideline aktualisiert. AGENTS.md hält die fortlaufende Markdown-Pflege als Projektvorgabe fest. Frühere Protokolleinträge bleiben unverändert als Historie.
* Separate Testinstanz beendet und die vorhandene Heimnetz-Vorführung mit der neuen Release-Fassung neu gestartet. Bearbeitungstests verwendeten ausschließlich das eigene Testprofil; der normale Lernstand wurde nicht zurückgesetzt.

---
## 03.11.2026 – Aufgabenansicht nach Nutzertest vereinfacht

* Wiederanzeige-Link für die allgemeine Selbsttest-Hilfe entfernt; Wiederherstellung ausschließlich im Lernplan. Generische Moduserklärungen, Speicherhinweise und Dein Lernraum aus beiden SQL-Lernwegen entfernt. Veraltete aria-describedby-Verweise und nicht mehr genutzte Linkstile bereinigt.
* Lernmodus, Geschichte, Auftrag und Editor rücken in den Vordergrund. Fachliche Erklärungen des geführten Lernens unter den Editor verschoben. Eine kurze Kennzeichnung mit Neustartaktion erhält den Zugang zu einem frischen Selbsttest bei übernommenen Entwürfen; Bestätigung vor tatsächlichem Entwurfsverlust bleibt bestehen.
* Gemeinsame TaskRail-Komponente ergänzt: Aufgabenliste mit zugänglicher Ein-/Ausklappsteuerung, breiterem Arbeitsbereich nach dem Einklappen und horizontaler Bedienung auf kleinen Bildschirmen. Die Einstellung wird separat und atomar im Lernprofil gespeichert; bestehende Entwürfe, Aufgabenmodi und Erfolge bleiben erhalten.
* Drei zusätzliche automatisierte Testfälle für Navigation und konkurrierende Profilspeicherung; bestehender Hilfetest an die bewusst fehlende Inline-Wiederanzeige angepasst. 140 .NET-Prüfungen einschließlich SQL-Integrationstests in Debug und Release erfolgreich. Browser: SQL-Bewertung, Modus-/Entwurfserhalt, Aufgabenwechsel, Neuladen, Auf-/Zuklappen, Lernplan-Wiederherstellung und mobile Ansicht mit separatem Testprofil geprüft.
* README, Projektsteckbrief, Lernkonzept, Verbesserungsplan, Heimnetz-Dokumentation und Testprotokoll aktualisiert. Die inhaltliche Storyüberarbeitung bleibt offen. Vorschau unter artifacts/aufgabenansicht-aufgeraeumt.png. Separate Testinstanz beendet und Heimnetz-App mit der geprüften Release-Fassung auf Port 5080 neu gestartet.

---
## 03.11.2026 – Dezentere Navigation und verbleibende Nutzerpunkte

* Textschalter der Aufgabenliste durch kleinen < / >-Pfeil am Trennrand ersetzt. Zugänglicher Name, Tooltip, Tastaturbedienung und gespeicherte Auswahl bleiben bestehen; mobil rechts am Rand.
* Paket B: SqlWritingRules ergänzt. Echte Schlüsselwort-Tokens und Funktionsknoten prüfen die Großschreibung; die letzte relevante Tokenposition jeder äußeren Anweisung prüft das Semikolon. Kommentare, Literale, Bezeichner und Aliase werden nicht pauschal verändert. Sicherheits- und Syntaxprüfungen bleiben vorgeschaltet. Bestehende Nutzerentwürfe werden nicht migriert oder umgeschrieben.
* Sichtbare Inhalte und Spickzettel von fall-Präfixen bereinigt. Referenzlösungen nutzen kurze Namen. Kompatibilitätsprüfung qualifizierter Abfragen bleibt erhalten. Datums-/Zeitbeispiele und Ergebnisse verwenden YYYY-MM-DD HH:mm:ss; beide SQL-Sitzungsarten setzen DATEFORMAT ymd. Verbindung mit deutscher Spracheinstellung separat geprüft.
* Paket C: Wochenplan durch profilbezogene Empfehlungen ersetzt. Neue Aufgaben, begonnene offene Versuche und unterstützte Abschlüsse ohne eigenständigen Erfolg getrennt. Reine Entwürfe ohne Abgabe und vollständig begonnene/gelöste Kataloge berücksichtigt.
* Paket D: Alle 30 Storyschritte und Entdeckungen überarbeitet. Zweck und Zusammenhang der Datenfragen werden erklärt; übernommene frühere Ergebnisse werden nicht bei übersprungenen Aufgaben vorausgesetzt. InvestigationStory markiert die offene Vorgängerspur, CaseNotes zeigt chronologische freigeschaltete Erkenntnisse mit Lücken und verlinkten Aufgaben.
* 31 zusätzliche automatisierte Fälle; insgesamt 171 .NET-Prüfungen einschließlich SQL-Integration in Debug und Release erfolgreich. Browserprüfungen mit eigenem Lernstand für Regeln, Korrekturen, Datumsdarstellung, Storysprünge, Notizen, Lernplan und kleine Navigation auf Desktop/mobil. Keine neuen Abhängigkeiten oder Änderungen an Datenbankschema/Startdaten.
* README, Projektsteckbrief, Lernkonzept, Coding Guideline, Verbesserungsplan und Testprotokoll aktualisiert. Bisherige Logeinträge bleiben Historie. Separate QA-Instanz beendet und normale Heimnetz-App als Release auf Port 5080 neu gestartet. Heimnetzadresse und Browserdateien mit HTTP 200 bestätigt; Prüfsumme des normalen Lernprofils unverändert. Gesamtfassung bereit für den Nutzertest; Homeserver und Benutzerkonten folgen danach.

---
## 03.11.2026 – Lokale Windows-Releases statt Onlinekonten

* Neue Nutzerentscheidung übernommen: Login, Registrierung, Benutzertabelle und öffentlicher Homeserver-/nginx-Betrieb entfallen. Die bestehenden elf SQL-Korrekturen sind positiv erprobt. AGENTS.md, README, Projektsteckbrief, Verbesserungsplan, Lernkonzept und Heimnetz-Dokumentation auf die lokale GitHub-Verteilung umgestellt; frühere Protokolleinträge bleiben historische Entscheidungen.
* Windows-x64-Publishprofil und Release-erstellen.ps1 ergänzt. Eigenständige EXE mit eingebauter .NET-/ASP.NET-Core-Laufzeit, statischen Browserdateien, nummeriertem ZIP und SHA256. SQL Server bleibt separat erforderlich. Native Bibliotheken werden bei Bedarf aus dem Bundle entpackt.
* Nur das Publish-Paket lädt appsettings.Release.json als Kennzeichnung. Inhalte werden aus dem Programmordner geladen; persönliche Konfiguration und Fortschritt verwenden LocalApplicationData/AP2-Lernwerk. Hosting-Umgebungsvariablen und Kommandozeile bleiben überschreibbar. Browserstart verwendet die tatsächliche Adresse statt eines fest eingetragenen Ports. Entwicklungs- und Heimnetzprofile behalten ihren bisherigen Lernstand.
* Private Konfiguration, Fortschrittsdateien und QA-Verzeichnisse aus dem Publish-Umfang entfernt. Paketprüfung stoppt bei privaten Dateien, Debugsymbolen oder abweichender allgemeiner SQL-Verbindung. Beim ersten Pakettest gefundene Core-Debugsymbole vor dem endgültigen Paket ausgeschlossen. Nicht benötigte IIS-Konfiguration wird nicht verteilt.
* MIT-Lizenz auf ausdrückliche Antwort des Nutzers ergänzt. Abhängigkeitshinweise aus NuGet-Metadaten und Originaldateien erzeugt, Laufzeit-Lizenzen und Drittanbieterhinweise beigefügt. Anleitung und Versionshinweise ergänzt. Kein GitHub-Upload oder Code-Signing.
* Acht zusätzliche Konfigurationstests, insgesamt 179 .NET-Prüfungen in Debug und Release sowie sechs JavaScript-Prüfungen erfolgreich. Tatsächliches ZIP entpackt und EXE auf getrennten Testports mit eigenem Lernprofil geprüft: statische Dateien, interaktives SELECT/UPDATE, gespeicherter Fortschritt nach Wechsel des Programmordners und verständlicher Start bei fehlender SQL-Verbindung. Details und Grenzen stehen im Testprotokoll. Die normale Heimnetzinstanz wurde neu gestartet; Prüfsumme ihres Lernstands unverändert. Endgültiges ZIP samt SHA256 unter artifacts/releases, alle QA-Instanzen beendet.

---
## 03.11.2026 – Frühe Alpha weiterentwickeln: Version, Updatehinweis und SQL-Auswahl

* Rückmeldung übernommen: Die Paketierung war vorgezogen. EXE 0.1.0 bleibt eine frühe Momentaufnahme, weitere Release-Pakete erst zum Abschluss beziehungsweise ausdrücklich angefordert. Zentrale Entwicklungsnummer 0.1.1-alpha.1 in Directory.Build.props; Browserfußbereich und Einrichtungsseite zeigen die tatsächlich gebaute Assemblyversion. Release-Skript liest denselben Standard, wurde hier nur syntaktisch geprüft.
* Öffentliche Updatequelle nach Nutzerantwort b0rnkilla/FIAE-AP2-Lernwerk hinterlegt. Startprüfung im Hintergrund, fünf Sekunden HTTP-Timeout, begrenzte Antwortgröße, kurze Wiederholungssperre. SemVer statt Zeichenkettenvergleich; Vorab-/Stable-Kanäle getrennt, nur veröffentlichte Releases mit fertigem Windows-ZIP. Kein Download oder Dateiaustausch ohne Benutzeraktion. Offline-, API- und Formatfehler bleiben vom Lernbetrieb getrennt. Persönliche Startprüfung abschaltbar.
* SQL-Einrichtungsseite ergänzt: registrierte lokale Instanzen beziehungsweise LocalDB als Auswahlhilfe, manuelle Namen, Windows-Anmeldung und tatsächliche Verbindungs-/Datenbankprüfung. Erst danach atomare Speicherung in einer ignorierten persönlichen Datei. Die eigene Trainingsdatenbank bleibt fest begrenzt; bestehende gültige Verbindung bleibt bei Fehlversuch erhalten. Neue Lernseiten beziehen eigene Evaluatoren aus der aktuell geprüften Datenbankinstanz.
* Konfigurationsänderung während der gemeinsamen Heimnetz-Vorführung im Service gesperrt, bevor eine andere Instanz angesprochen wird. Die lokale Erkennung verändert keine Dienste und behauptet bei fehlendem Treffer oder Verbindungsfehler nicht, SQL Server sei sicher nicht installiert. Offizieller Microsoft-Download verlinkt.
* .gitignore für private Einstellungen, temporäre Konfiguration, .env, Schlüssel, Zertifikate und Datenbankdateien ergänzt. Regeln in separatem Git-Testrepository gegen 13 private und acht öffentliche Beispiele verifiziert. Kein echtes Repository initialisiert, keine Dateien gepusht und keine Geheimnisse als Zugangsschlüssel für die App verwendet.
* Browserprüfung mit absichtlich fehlerhaftem Start und getrennten QA-Dateien: vorhandene LocalDB ausgewählt und vorbereitet, SELECT/UPDATE erfolgreich gelöst, Verbindung und Updatewahl über Neustart erhalten. Vollständige SQL-Prüfungen zusätzlich auf LocalDB; Updateanzeige mit künstlichen API-Antworten geprüft. Der echte GitHub-Zugriff lieferte derzeit 404 und wird als nicht erreichbar gemeldet.
* Insgesamt 230 .NET-Fälle in Debug auf SQLEXPRESS und LocalDB sowie in Release auf SQLEXPRESS erfolgreich, keine übersprungenen Fälle oder Buildwarnungen/-fehler. Sechs JavaScript-Prüfungen erfolgreich. README, Projektsteckbrief, Lernkonzept, Startanleitung, Heimnetz-Dokumentation, Verbesserungsplan, AGENTS.md und Testprotokoll aktualisiert. Normale Heimnetz-Instanz mit aktueller Version wieder gestartet; private Verbindung und Alpha-ZIP per Prüfsumme unverändert. Lernprofil nach dem Neustart im Appbetrieb erneut gespeichert; 21 bisherige Abschlüsse weiterhin vorhanden, kein Reset oder Rückkopieren. Alle QA-Instanzen beendet; Endergebnisse im Testprotokoll.

---
## 03.11.2026 – Modulstruktur, öffentliche Dokumentation und private Nutzungsbedingungen

* Nutzer hat das GitHub-Repository erstellt und die Projektmappe selbst gepusht. README auf Lernumfang, Voraussetzungen, Start, Lernstand, Updates und Nutzungsbedingungen gekürzt. Rechnerbezogene Einzelheiten und Entwicklungsgespräch entfernt; technische Orientierung in docs/entwicklung.md zusammengefasst. Projektsteckbrief und Verbesserungsplan auf den aktuellen Stand gestrafft. Historische Protokolle bleiben erhalten.
* MIT-Freigabe auf neue ausdrückliche Entscheidung für die Projektfassung 0.1.1-alpha.2 durch eigene private Nutzungsbedingungen ersetzt. Keine kommerzielle Nutzung, Programmänderung oder Weitergabe eingeräumt; bestimmungsgemäße Konfiguration, eigene Lösungen und persönliche Fortschritte erlaubt. Frühere MIT-Rechte, zwingende gesetzliche Rechte, GitHub-Plattformrechte und Drittanbieterlizenzen ausdrücklich ausgenommen. Ursprünglichen MIT-Text separat bewahrt. docs/lizenz.md erklärt Distribution, Reichweite und Grenzen des Wechsels; keine juristische Prüfung behauptet.
* Core-Fachlogik unter Sql gegliedert: Content, Database, Investigation, Results, Validation und Workshop. Webdienste in Configuration, Learning, Networking, Sql und Updates gegliedert. SQL-Komponenten, Aufgabenseiten und allgemeine Lernbedienung getrennt; Namespaces und Verweise angepasst. Öffentliche Modelle, Lernbereich und Updatezustände in eigene Dateien aufgeteilt. Program.cs bleibt einziger C#-Einstieg im Web-Projektroot.
* Gemeinsamen eingebetteten Ressourcenloader SqlContent aus dem Ermittlungsaufgabenkatalog gelöst. Beide Kataloge und der Datenbankzugriff verwenden ihn; keine Änderung der vier Inhaltsdateien, Datenbanknamen, Datenbankmarker oder JSON-Lernstände. Eine Profilmigration ist nicht erforderlich.
* Alle vorhandenen XML-Summaries geprüft und die alte Bezeichnung Mission in C#-Kommentaren und Testmeldungen durch Aufgabe ersetzt. Ungenaue Beschreibungen von Verbindungsprüfung, Rechteumfang, Ergebnisgrenzen, Profilreset und Updatezeitpunkt präzisiert; nicht offensichtliche Ablaufmethoden zusätzlich erklärt. Historische Protokollsprache und technische CSS-Klassen nicht umbenannt.
* 230 .NET-Fälle in Debug und Release mit echten SQL-Integrationstests sowie sechs JavaScript-Prüfungen erfolgreich. Ein isolierter Teststart benötigte Zugriff auf den lokalen Testprozess; anschließend vollständige erfolgreiche Wiederholung. Acht Seiten-/Ressourcenpfade separat über HTTP geprüft, aktuelle Version sichtbar. 27 relative Dokumentationslinks und unveränderte private Profildatei geprüft. Einzelheiten im Testprotokoll.
* Release-Skript für die Mitlieferung des historischen Lizenzhinweises ergänzt und nur syntaktisch geprüft. Keine neue Paketierung, keine Git-Indexänderung, kein Commit oder Push. Die GitHub-API lieferte beim separaten anonymen Kontrollabruf weiterhin 404; diese Momentaufnahme wird nicht mehr als allgemeiner Zustand in öffentlichen Einstiegsdokumenten ausgegeben.

---
## 03.11.2026 – Lesbare deutsche Verbindungsanzeige und präzisierte Privatnutzung

* Fehlerursache des Screenshots nachvollzogen: Das Standardfenster des Frameworks verwendet eine weiße Fläche und übernimmt die helle Textfarbe des dunklen Appdesigns. Eigenen statischen ReconnectModal außerhalb des interaktiven Lernbereichs ergänzt, dessen Text, Fläche, Rahmen und Aktionen die bestehenden semantischen Farben verwenden. Allgemeine Fehlerleiste einschließlich Linkfarbe präzisiert.
* Browserseitige Ereignisse für Verbindungsaufbau, Wiederholung, Fehlschlag, abgelehnte Sitzung, Pause und fehlgeschlagenes Fortsetzen auf deutsche Texte abgebildet. Wiederverbinden und Resume laufen ohne .NET-UI-Ereignisse; mehrere Klicks starten keine parallelen Versuche. Kein automatisches Neuladen, damit ungesicherte Eingaben nicht unbeabsichtigt verschwinden. Hinweis auf diese Grenze im Dialog.
* Klarstellung übernommen: Das Repository ist derzeit privat und hat keine Forks. Die vorherige Annahme öffentlich veröffentlichter MIT-Projektfassungen war nicht belegt. Aktuelle Lizenz Fassung 1.1 erlaubt ausschließlich die beschriebene private, nichtkommerzielle Nutzung; Änderungen, Weitergabe, Unterlizenzierung und öffentlicher/gewerblicher Betrieb nicht eingeräumt. Pauschale frühere Projekt-MIT-Freigabe und zusätzliche Fork-Klausel entfernt. Ursprünglichen Projektlizenzentwurf aus der aktuellen Dateisammlung und der Paketvorbereitung entfernt; fremde Original-Lizenzen unverändert erhalten. Historische Protokolle dienen nicht als aktuelle Lizenzfreigabe.
* Zentrale Version 0.1.1-alpha.3. Öffentliche Dokumente und Projektvorgaben benennen den privaten Repositorystand und die erst künftig mögliche anonyme Releaseprüfung, ohne Zugangsschlüssel einzubauen. README, Projektsteckbrief, Entwickleranleitung, Lernkonzept, private Nutzungsbedingungen, Verbesserungsplan und Release-Anleitung nachgeführt.
* Release-Build fehler- und warnungsfrei; 230 .NET-Tests einschließlich echter SQL-Prüfungen und 13 JavaScript-Prüfungen erfolgreich. Echten Abbruch ausschließlich am separaten Testserver geprüft, helle/dunkle Ansicht sichtbar kontrolliert und tatsächliche Textkontraste gemessen. Detailnachweise und Grenzen im Testprotokoll. Keine normale Lerninstanz beendet, keine Nutzerdaten für Tests geändert, keine Paketierung, kein Commit/Push.

---
## 03.11.2026 – Aktuelles Windows-ZIP und Einstieg über Releases

* Auf ausdrücklichen Auftrag neues Paket 0.1.1-alpha.3 mit aktueller privater Projektlizenz erstellt; das alte 0.1.0-ZIP ist kein aktuelles Verteilungspaket. Keine neue Versionsnummer allein für die Paketierung eingeführt.
* README auf Benutzerdownload über Releases/Assets, Entpacken und EXE-Start umgestellt. Aufforderung zum Klonen, Quellprojekt-Startbefehl und SDK-Voraussetzung aus dem Benutzereinstieg entfernt. Eine kurze eigene release/Anleitung.md ersetzt im ZIP die bisher kopierte technische Anleitung; passende Versionshinweise ergänzt.
* ZIP und SHA256 erzeugt. Tatsächlich entpackte EXE mit eigener QA-Konfiguration, eigenem Profil und ungültigem DOTNET_ROOT geprüft; zehn HTTP-Pfade und echte SELECT-/UPDATE-Abgaben erfolgreich. Nach Wechsel in einen zweiten Programmordner bleiben beide Testabschlüsse und Entwürfe erhalten. Paketdateien und private Projektlizenz kontrolliert, keine geprüften privaten Dateien im ZIP. Detailnachweise im Testprotokoll.
* Dokumentation und Projektvorgaben nachgeführt. Normalen Lernstand und normale App nicht für Tests verwendet; QA beendet. Kein Commit, Push, GitHub-Release oder Sichtbarkeitswechsel ausgeführt. Eine Live-Updateprüfung benötigt anschließend eine öffentlich verfügbare höhere Version als die laufende Alpha.

---
## 03.11.2026 – Aufgabenliste stehen lassen und geführte Hilfen verbessern

* Entwicklungsfassung 0.1.1-alpha.4: Bei weiteren Aufgabenwechseln nur noch den Hauptinhalt als Ladezustand austauschen. TaskRail und sein scrollender Navigationsknoten bleiben in Ermittlung und Werkstatt bestehen; die Liste springt dadurch nicht an den Anfang.
* Im geführten Hilfebereich beider SQL-Lernwege eine Spickzettel-Karte mit direktem Link in einen neuen Tab ergänzt. Der aktuelle Entwurf bleibt erhalten. Musterlösung als abgerundete Schaltfläche mit Symbol und Zustand gestaltet; Auf-/Zuklappen verändert nicht die gespeicherte Hilfenutzung. Anzeige und Unterstützung getrennt gehalten, ohne das Profilformat zu erweitern.
* Build ohne Warnungen oder Fehler, 230 .NET-Fälle einschließlich echter SQL-Integration und 13 JavaScript-Fälle erfolgreich. Tatsächliche Klicks auf gescrollte Listen in beiden Lernwegen, neuen Spickzettel-Tab, erhaltenen Testentwurf, Klappfunktion und Hilfenkennzeichnung geprüft. Helle/dunkle und schmale Ansicht ohne horizontalen Überlauf erprobt; Details im Testprotokoll.
* README, Projektsteckbrief, Lernkonzept, Entwickleranleitung, Verbesserungsplan und Release-Anleitung nachgeführt. Eigene QA-Profile verwendet und Testserver/-tabs beendet. Kein neues Paket, Commit oder Push; Windows-ZIP 0.1.1-alpha.3 bleibt unverändert.

---
## 03.11.2026 – Hilfen als einheitliche Zeilen und Spickzettel im selben Tab

* Nutzerkorrektur innerhalb der noch laufenden Entwicklungsfassung 0.1.1-alpha.4 umgesetzt: Denkanstoß, Spickzettel und Musterlösung verwenden in beiden SQL-Lernwegen dieselbe Anordnung mit Titel/Beschreibung links und Aktion rechts. Gesonderte Karten und Symbole samt ungenutzten CSS-Regeln entfernt.
* Spickzettel öffnet auf Wunsch im selben Tab statt in einem neuen. Entwürfe bleiben durch die vorhandene Speicherung bei Rückkehr erhalten. Die Musterlösung zeigt links „Musterlösung“ und rechts „Ansehen“ beziehungsweise „Ausblenden“. Hilfenkennzeichnung unverändert.
* Release-Build erfolgreich. Eigene Browserprofile: Navigation und Rückkehr mit erhaltenen Entwürfen in beiden Lernwegen, drei nacheinander geöffnete Hinweise, Ansehen/Ausblenden sowie helle/dunkle und schmale Darstellung geprüft. Keine erneute Ausführung der unveränderten Fachtests; deren vorheriger vollständiger Stand bleibt im Testprotokoll erhalten.
* README und aktuelle Fachdokumente angepasst. Vorherige neue-Tab-Ausführung bleibt im Protokoll als überholter Zwischenschritt dokumentiert. Kein neues Paket, Commit oder Push.

---
## Geplant


* Entwicklung der weiteren Lernmodule fortsetzen. Weitere EXE-Pakete erst zum Abschluss oder ausdrücklich gewünscht. Die elf SQL-Anpassungen sind erledigt; Onlinekonten und Homeserver-Ausbau gestrichen.
* Öffentliche GitHub-Erreichbarkeit und erste tatsächlich veröffentlichte Windows-Releases prüfen, sobald die Quelle verfügbar ist. Ein abschließendes Paket zusätzlich auf einem frisch eingerichteten anderen x64-Rechner testen.
* SQL-Werkstatt um weitere relevante Themen ergänzen; die ersten zehn Aufgaben sind umgesetzt.
* Zusätzliche Transferaufgaben auf einem anderen Szenario, damit die Wiederholung nicht nur den bekannten Fall abruft.
* Pseudocode-Modul mit Fehlersuche, Schreibtischtests und freien Aufgaben.
* Aktuellen vollständigen Prüfungskatalog systematisch auf die künftigen Lernmodule abbilden.

---

## Später / Optional

* Begrenzte, nachvollziehbare UML-Aufgaben mit mehreren zulässigen Lösungen.
* Weitere AP2-Module für Softwareplanung, Datenbankentwurf, Tests/Sicherheit, Projektmanagement und WiSo.
* Komfortabler Export/Import lokaler Lernstände bei weiterem Bedarf; GitHub-Releases verteilen die lokale Anwendung.
* Optionale KI-Bewertung erst nach Klärung von Konto, Kostenrahmen, Datenübertragung und fachlicher Verlässlichkeit.

---

## Bekannte Grenzen

* Aktuell 30 lesende SQL-Aufgaben und zehn schreibende Werkstattaufgaben, keine vollständige Abdeckung der AP2.
* Zwei Prüfdatenbestände verbessern die Bewertung, beweisen aber nicht die Gleichwertigkeit beliebiger SQL-Abfragen.
* Lernstand ist ein lokales Einzelbenutzerprofil. Die Erfolgsanzeige ist keine Bestehensprognose.
* Vollständige Visual-Studio-Unterstützung für .NET 10 setzt Visual Studio 2026 voraus.

### 03.11.2026 – Erster Pseudocode-Lernweg und kontextbezogene Navigation

* Den freigegebenen ersten Ausbau „Leitstelle im Ausnahmezustand“ mit acht Aufgaben umgesetzt: drei freie Codeaufgaben, zwei Reparaturen, zwei Schreibtischtests und eine Aufgabe zum Entwerfen eigener Testfälle. Eigene deutsche Story, eindeutige Aufträge, je drei Hinweise, Referenzen, Erklärungen und chronologischer Einsatzbericht mit offenen Zwischenstücken.
* Core/Algorithms in Content, Syntax, Execution und Evaluation gegliedert. Eine begrenzte deutsche Pseudocode-Grammatik wird lokal in C# geparst und interpretiert; weder KI-Dienst noch SQL-Verbindung erforderlich. Mehrere Eingaben und unabhängig hinterlegte Sollausgaben prüfen das Verhalten. Alternative Listenschleifen und Indexschleifen sind möglich; keine textuelle Gleichheit mit der Musterlösung erforderlich.
* Geführtes Lernen um eine schrittweise Ablaufansicht mit markierter Zeile, Variablen und bisherigen Ausgaben ergänzt. Eingabeänderungen verwerfen alte Bewertungen und Spuren. Ausführungs-, Ausdrucks-, Listen-, Ausgabe- und Spurgrenzen fangen übergroße Abläufe und Endlosschleifen kontrolliert ab.
* Eigene AlgorithmProgress-Einträge und LastAlgorithmLessonId ergänzt. Ältere SQL-Profile behalten ihre Entwürfe und Erfolge. Neue Aufgaben beginnen ohne Hilfe, Modi werden je Aufgabe gespeichert. Ein unveränderter Reparaturcode benötigt beim Wechsel zurück keine Löschbestätigung; ein geänderter Entwurf bleibt geschützt.
* Kopfnavigation auf Übersicht, Lernplan, fachliche Links umgestellt. Der fachliche Kontext wechselt bei SQL-/Algorithmus-Seiten und bleibt innerhalb der Browsersitzung auf allgemeinen Seiten erhalten. Auf Algorithmus-Seiten stehen Training und Pseudocode-Spickzettel, auf SQL-Seiten Ermittlung, Werkstatt und SQL-Spickzettel.
* Übersicht und Lernplan auf insgesamt 48 Übungen erweitert. Neue, begonnene und mit Unterstützung gelöste Aufgaben beider Themen werden gemeinsam korrekt zugeordnet. Der Pseudocode-Spickzettel öffnet im selben Tab und erhält den gespeicherten Entwurf bei Rückkehr.
* README, Projektsteckbrief, Lernkonzept, Entwickleranleitung, Coding Guideline, Verbesserungsliste, lokale Releasehinweise und AGENTS aktualisiert; docs/pseudocode.md beschreibt Syntax, Grenzen und spätere Ausbauschritte. Funktionen, weitere Listenaufgaben, Suche und Sortierung bleiben geplant.
* Release-Build ohne Warnungen/Fehler; abschließend 274 .NET-Prüfungen einschließlich echter SQL-Integration erfolgreich, keine übersprungen. 13 Node-Prüfungen ebenfalls erfolgreich. Alle acht Aufgaben im eigenen Browserprofil gelöst, Hilfen/Ablaufansicht/Moduswechsel und Modultausch erprobt. Zusätzlicher eigener Server mit unerreichbarer SQL-Verbindung bestätigt die unabhängige Pseudocode-Nutzung. Details im Testprotokoll.
* Quellversion bleibt 0.1.1-alpha.4. Kein neues EXE-/ZIP-Paket erzeugt; SHA256 des bestehenden Alpha-3-ZIP unverändert. Keine Veröffentlichung, kein Commit oder Push.

Nachtrag zum Pseudocode-Ausbau: Die Rückmeldung bei fehlendem Zeilenumbruch nennt jetzt direkt die notwendige eigene Zeile beziehungsweise ein trennendes Semikolon. Parser und Ausführungszustand sind zusätzlich mit Regions und Zustandskommentaren gegliedert. Anschließender Build und alle 44 gezielten Algorithmusprüfungen erfolgreich; die eigenen Browser-Prüfserver sind beendet.

### 04.11.2026 – Zuweisungen mit Gleichheitszeichen und Vorbereitung Alpha 4

* Den Zusatz „ohne SQL-Verbindung nutzbar“ von der Pseudocode-Themenkarte entfernt; dort stehen nur Geschichte und Aufgabenzahl.
* Pseudocode-Beispiele, Reparaturvorgaben, Hinweise, Musterlösungen und den Syntaxfehlerhinweis auf = als bevorzugte Zuweisung umgestellt. Der Spickzettel erklärt == als bevorzugten Gleichheitsvergleich. Bisherige Entwürfe mit ←, <- oder := bleiben gültig und werden nicht umgeschrieben. Das bereitgestellte IHK-Beispiel dient nur der Schreibweisenorientierung; keine neue Funktionalität aus der fremden Prüfungsaufgabe übernommen.
* README, Projektsteckbrief, Lernkonzept, Pseudocode-Dokumentation, Coding Guideline und Verbesserungsplan angepasst. Anleitung und Versionshinweise für Alpha 4 liegen zur gemeinsamen Versionierung bereit. Das neue Windows-ZIP ist auf Nutzerwunsch erst nach dessen Commit und Push zu erstellen.
* Release-Build am 03.11.2026 ohne Warnungen/Fehler abgeschlossen. Die anschließenden Tests und der Browserprüfserver konnten wegen des erreichten Nutzungslimits zunächst nicht automatisch freigegeben werden; dabei wurden keine Prüfungen ausgeführt. Nach Nutzerfreigabe zur Wiederholung am 04.11.2026 alle 45 gezielten Algorithmus-/Profil-/Lernplanprüfungen erfolgreich, keine übersprungen. Sie prüfen auch die acht umgestellten Referenzen und die Trennung von Zuweisung und Gleichheitsvergleich.
* Browserprüfung auf Port 5085 mit eigenem Profil und absichtlich unerreichbarer SQL-Verbindung: Themenkarte ohne Zusatz, Spickzettel mit =, erste Aufgabe mit = korrekt gelöst. Normalen SQL-Dienst und Nutzerlernstand nicht verändert. Eigener Prüfserver und Prüf-Tab anschließend geschlossen.
* Version bleibt 0.1.1-alpha.4. Kein neues ZIP/EXE-Paket, kein Commit oder Push ausgeführt. Vorhandenes Alpha-3-Paket bleibt unverändert.
