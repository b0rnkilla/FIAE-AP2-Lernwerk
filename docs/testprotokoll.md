# Testprotokoll

Stand: 03.11.2026. Geprüft wurde auf dem lokalen Windows-Rechner mit .NET-SDK 10.0.401 und der vorhandenen Microsoft-SQL-Express-Instanz. Alle Datenbanktests verwenden ausschließlich synthetische Trainingsdaten.

## Automatisierte Tests

**230 .NET-Prüfungen erfolgreich, 0 fehlgeschlagen, 0 übersprungen**, einschließlich der ausdrücklich eingeschalteten SQL-Integrationstests. Der Build ist ohne Warnungen und Fehler erfolgreich. Die zuletzt durchgeführte Paketprüfung meldete gemäß den verwendeten Quellen keine bekannten verwundbaren Abhängigkeiten; die Abhängigkeiten wurden für diese Änderung nicht erweitert.

| Bereich | Nachweis |
| --- | --- |
| Inhaltsstruktur | 30 fortlaufende Missionen, drei Hinweise, Texte, Aufgaben und syntaktisch gültige Musterlösungen |
| Lerninhalte auf echtem SQL Server | Alle 30 Musterlösungen auf Fall- und Prüfdatenbestand erfolgreich bewertet |
| Unabhängige Erwartungen | Erwartete Zeilen- und Spaltenzahlen für alle 30 Missionen separat festgehalten |
| Zentrale Ergebnisse | 12 Sendungen, 162000 Gesamtwert, 13500 Durchschnitt, 500 Minimum, 48000 Maximum; korrekte Zutrittsanzahlen und Fallauflösung |
| Typische Fehler | AND/OR ohne notwendige Klammern, falsche Sortierung, = NULL, falscher JOIN-Typ, COUNT auf nullable Spalten, COUNT(*) nach LEFT JOIN und fehlende Korrelation zurückgewiesen |
| Zufallstreffer | Fest eingetragene Täter-ID passt nur zu den sichtbaren Daten und wird in der Zusatzprüfung abgewiesen |
| Alternative Lösung | NOT EXISTS statt LEFT JOIN / IS NULL mit anderen Aliasnamen erfolgreich akzeptiert |
| Wertevergleich | Duplikate bleiben relevant, Sortierung nur bei Anforderung, NULL und Text verschieden, Zahlenwerte kompatibel, übergroße Gleitkommawerte ohne Konvertierungsabsturz |
| Parsergrenzen | Schreib-/Verwaltungsbefehle, mehrere Anweisungen, Fremdschemas, externe Quellen, SELECT INTO, Variablen und unerlaubte Funktionsaufrufe abgewiesen |
| Rechtebegrenzung | DELETE, CREATE TABLE, Zugriff auf interne Verwaltungstabelle sowie nicht öffentlich zugängliche andere Datenbank trotz direktem Aufruf unter TrainerReader abgewiesen; REVERT ermöglicht keine Rückkehr zu Schreibrechten |
| Ergebnisgrenze | Ein Kreuzprodukt wird auf 200 angezeigte Zeilen begrenzt und als gekürzt gekennzeichnet |
| Deutsche Fehlerhinweise | Unbekannte Spalte und unvollständige Gruppierung liefern verständliche Rückmeldung |
| Fortschrittsspeicherung | Entwürfe, Hinweise und Erfolge über Wiederladen erhalten; gleichzeitige Missionsspeicherung ohne Verlust bestehender Abschlüsse; beschädigte Dateien nicht überschrieben; Zurücksetzen ausschließlich im isolierten Testverzeichnis geprüft |

Die 30 Ermittlungslösungen und zehn Werkstattlösungen werden innerhalb von Integrationsprüfungen einzeln auf beiden Datenbeständen durchlaufen; zusätzlich bestehen alle 30 Ermittlungslösungen auch mit explizitem Schemapräfix zur Abwärtskompatibilität. Die Zahl 230 bezeichnet .NET-Testfälle und nicht die Anzahl ausgeführter SQL-Abfragen. Tests zur Datenbankprüfung werden ohne `FIAE_SQL_TESTS=1` ausdrücklich übersprungen.

## Entwicklungsfassung 0.1.1-alpha.1: Version, Updates und SQL-Einrichtung (03.11.2026)

Die vorhandene EXE 0.1.0 bleibt unverändert eine frühe Momentaufnahme. In diesem Paket wurden nur Quellprojekt-Builds und Tests ausgeführt; kein neues Release-ZIP und kein GitHub-Upload.

* 51 neue Testfälle ergänzen den früheren Stand von 179 auf 230: SemVer einschließlich Alpha-Reihenfolge und Metadaten, gültige öffentliche GitHub-Quelle, geschützte Linkbildung, Auswahl fertiger Windows-Releases, Draft-/Prerelease-Filter, leere Listen, HTTP 404/403/429/502, kaputte Antworten, Timeout, Offlinebetrieb und Wiederholungssperre. Der gerenderte Updatehinweis wird mit einer künstlichen neueren Version geprüft.
* Persönliche Einrichtung: beliebige Instanznamen, festes Trainingsdatenbankziel, Windows-Anmeldung, Abweisung ganzer Connectionstrings und Steuerzeichen, konkurrierende Speicherung von SQL- und Updatewahl, Erhalt anderer JSON-Einstellungen, Schutz beschädigter Dateien und explizit getrennte QA-Einstellungsdateien. Fehlgeschlagene Auswahl beziehungsweise Speicherung ersetzt keine funktionierende Verbindung.
* Serverseitige Heimnetz-Sperre vor jedem Kontakt mit einer neu gewählten SQL-Instanz geprüft. Vorführungsbesucher können weder Datenbankziele noch persönliche Updatewahl ändern.
* Vollständiger Testbestand mit ausdrücklich aktivierter SQL-Integration auf SQLEXPRESS und zusätzlich auf der vorhandenen LocalDB ausgeführt: alle 30 Ermittlungs- und zehn Werkstattlösungen auf beiden synthetischen Datensätzen sowie Ergebnis-, Fehler- und Rechteprüfungen bestehen. Alle 230 Fälle bestehen in Debug auf SQLEXPRESS sowie LocalDB und abschließend in Release auf SQLEXPRESS, ohne übersprungene Tests, Warnungen oder Fehler. Die sechs JavaScript-Designprüfungen bestehen ebenfalls.
* Browser mit eigenem Profil auf Port 5081 und eigener appsettings.Local.json: Start mit absichtlich falschem SQL-Endpunkt zeigt einen Einrichtungseinstieg statt „SQL fehlt“. Zwei lokale Kandidaten erkannt. Die vorhandene (localdb)\MSSQLLocalDB gewählt, Trainingsdatenbank dort vorbereitet, Verbindung gespeichert und ohne App-Neustart SELECT sowie UPDATE erfolgreich bewertet.
* Nach App-Neustart: LocalDB-Verbindung automatisch übernommen, zwei gespeicherte Erfolge erhalten, ausgeschaltete Updateprüfung wiederhergestellt. Keine neue GitHub-Anfrage in diesem Teststart. Versionsanzeige in Fußbereich und Einrichtung stimmt mit 0.1.1-alpha.1 überein. Microsoft-Downloadlink geprüft. Desktop und 390 × 844 Pixel breite Ansicht ohne horizontalen Überlauf; Vorschau artifacts/version-und-sql-einrichtung.png.
* Echter öffentlicher API-Aufruf für b0rnkilla/FIAE-AP2-Lernwerk lieferte beim Test HTTP 404. Die App meldet die fehlende Erreichbarkeit, statt einen aktuellen Stand zu behaupten. Ein echtes verfügbares Remote-Update konnte daher nicht nachgewiesen werden; Auswahl und Hinweis wurden mit isolierten API-Antworten geprüft.
* .gitignore mit echten Git-Befehlen in einem neu angelegten Testrepository geprüft: 13 private Dateibeispiele ignoriert, acht öffentliche Beispiele einschließlich appsettings.json, appsettings.Release.json, LICENSE und Verbindungsvorlage aufnehmbar. Das eigentliche Projekt wurde nicht als Git-Repository initialisiert. Dateinamensregeln ersetzen keine Inhaltsprüfung und bereinigen keine Git-Historie.
* Release-Skript nur syntaktisch geprüft. Seine Standardversion kommt jetzt aus Directory.Build.props. Das vorhandene ZIP wird nicht aktualisiert; Benutzer hat weitere Paketierung bis zum Entwicklungsabschluss zurückgestellt.
* Normale Heimnetz-Instanz mit neuer Quellprojektfassung wieder gestartet: Startseite, Einrichtung und Stylesheet HTTP 200; Version 0.1.1-alpha.1 sichtbar und SQL-Verbindung aktiv. SQL-Eingabe im Heimnetz nicht ausgeliefert. Prüfsummen der privaten Quellprojektverbindung und des alten ZIPs unverändert. Das normale Lernprofil wurde nach dem Neustart erneut gespeichert und hat deshalb eine andere Prüfsumme; die 21 bisherigen Abschlüsse sind weiterhin vorhanden. Es wurde weder für Bearbeitungstests benutzt noch zurückgesetzt oder auf einen älteren Stand zurückkopiert. Alle QA-Server beendet.

## Lokale Windows-Veröffentlichung (03.11.2026)

* Alle 179 .NET-Prüfungen bestehen in Debug und Release einschließlich ausdrücklich aktivierter SQL-Integration, ohne übersprungene Fälle, Warnungen oder Fehler. Die sechs JavaScript-Designprüfungen wurden ebenfalls erneut erfolgreich ausgeführt. Acht zusätzliche Konfigurationsfälle erhöhen den Stand von 171 auf 179: stabiles Benutzerprofil, fehlende persönliche Datei ohne unerwartete Dateianlage, persönliche SQL-Konfiguration, Vorrang von Hosting-Umgebungsvariablen und Startparametern sowie tatsächlich gebundene Browseradresse mit korrektem Port und Platzhalterhost.
* Windows-x64-Paket mit eingebauter .NET-/ASP.NET-Core-Laufzeit gebaut. Die EXE aus dem ZIP wurde außerhalb ihres Programmordners gestartet, mit ungültigem DOTNET_ROOT und abgeschalteter Mehrfach-Laufzeitsuche. Dies ist keine Prüfung auf einem frisch installierten fremden Windows-Rechner; diese zusätzliche Plattformprüfung steht vor öffentlicher Verteilung noch aus.
* Endgültigen Paketinhalt separat entpackt und kontrolliert: keine persönlichen Verbindungen, Lernstände, QA-Profile, Debugsymbole oder Logs. Allgemeine SQL-Express-Vorlage, MIT-Projektlizenz, Versionsdaten und vorhandene Original-Lizenz-/Drittanbieterhinweise mitgeliefert. SHA256-Datei passt zum ZIP.
* Tatsächliche veröffentlichte EXE: Startseite, CSS und Blazor-Skript mit HTTP 200. Im Browser SELECT-Aufgabe korrekt gelöst und gespeichert. Danach neue Paketfassung in einen anderen Ordner entpackt und mit demselben separaten QA-Profil gestartet: Entwurf, Modus, Abschluss und eingeklappte Liste erhalten. Werkstatt-UPDATE korrekt bewertet und im Lernplan als weiterer Erfolg sichtbar. Vorschau: artifacts/windows-release-getestet.png.
* Negativfall aus der EXE mit absichtlich nicht erreichbarem lokalem SQL-Endpunkt: Prozess bleibt gestartet, Startseite liefert HTTP 200 und erklärt die fehlende Datenbankverbindung. Keine Lernaufgabe wird als erfolgreich vorgetäuscht.
* Das endgültige ZIP wurde nochmals neu entpackt und über ASPNETCORE_URLS auf Port 5082 gestartet; gespeicherte Erfolge und Browseroberfläche sind erhalten. Startseite, CSS und Blazor liefern HTTP 200; SHA256 passt. Die bisherige Heimnetzinstanz auf 5080 läuft wieder, ihr normaler Lernstand hat dieselbe Prüfsumme wie vor Build und Neustart. Alle QA-Instanzen wurden beendet.
* Tests verwenden getrennte Ports 5082/5083 und eigene Fortschrittsverzeichnisse. Benutzerprofil der normalen Quellprojektfassung und synthetische Startdaten wurden nicht übernommen oder zurückgesetzt. Der Standard der veröffentlichten EXE bleibt loopback; keine neue Firewallfreigabe.
* Kein GitHub-Upload, kein automatischer Updater und kein Code-Signing durchgeführt. Keine eigenständige LocalDB- oder ARM64-Prüfung. Diese Fassung ist ein lokal vorbereitetes und geprüftes Windows-x64-Paket.

## Randpfeil und Pakete B–D (03.11.2026)

Aktueller Stand: Alle elf Anpassungen sind umgesetzt. Die nachfolgenden früheren Abschnitte dokumentieren den damaligen Stand. Die sechs JavaScript-Prüfungen aus dem Designpaket waren erfolgreich; JavaScript wurde in dieser Änderung nicht verändert.

* 31 zusätzliche .NET-Fälle bringen die Gesamtzahl auf 171. Prüfung in Debug und Release mit ausdrücklich eingeschalteter SQL-Integration erfolgreich; keine übersprungenen Fälle, Buildwarnungen oder -fehler.
* Schreibregeln: konkrete Schlüsselwort-/Funktionskorrekturen, einzelne und mehrteilige Anweisungen, BEGIN/COMMIT/ROLLBACK, Semikolons in Kommentaren und Texten, maskierte Anführungszeichen, Unterabfragen sowie frei geschriebene Spalten und Aliase. CAST, TRY_CAST und IIF werden zusätzlich zu gewöhnlichen Funktionen berücksichtigt.
* Bisherige semantische Fehlertests wurden um korrekte Abschlusszeichen ergänzt, damit ein fachlich falscher Filter weiterhin wegen seines Ergebnisses und nicht allein wegen der Schreibregel scheitert. Die Zusatzprüfung mit explizitem Schema bleibt als Kompatibilitätsfall erhalten.
* Alle 30 Ermittlungs- und zehn Werkstattlösungen bestehen auf beiden synthetischen Datenbeständen. Unabhängige Zeilen-/Spaltenzahlen, zentrale Datenwerte, alternative Lösungen, Zählfallen und Rechtebegrenzungen bestehen weiterhin. Eine deutsche Verbindungssprache verarbeitet die neuen Zeitliterale korrekt. Ergebnistabellen zeigen auch Mitternacht vollständig im neuen Format.
* Gerenderter Lernplan: leeres Profil, gemischte neue/offene/unterstützt gelöste Aufgaben, abgeschlossener Katalog und reine Entwürfe ohne Abgabe. Keine falsche Abschlussmeldung, keine festen Wochen. Story-/Notiztests prüfen Reihenfolge, erkennbare Lücken und verborgene ungelöste Erkenntnisse.
* Browser mit eigenem Profil auf Port 5081: Kleinbuchstaben und fehlendes Semikolon erzeugen konkrete Meldungen; korrigierte SELECT- und UPDATE-Lösungen bestehen. Datumsfilter liefert erwartete Zeilen und sichtbare Zeitwerte als 2026-10-12 20:10:00 usw. Geführter Erfolg wird zur Wiederholung angeboten; begonnene falsche Aufgabe und nächste neue Aufgabe stehen getrennt im Lernplan.
* Browser: Sprung zu Aufgabe 18 markiert die offene Vorgängeraufgabe. Fallnotizen zeigen Aufgaben 1, 2 und 18 chronologisch mit offener Lücke 3–17. Die Story-/Notiztexte wurden zusätzlich fachlich gegen die jeweiligen Aufträge geprüft, damit Ergebnisse übersprungener Aufgaben nicht als bereits bestätigt erscheinen.
* Kleiner Pfeil in dunkler Desktopansicht sowie heller mobiler Ansicht bei 390 × 844 Pixeln ein-/ausgeklappt geprüft. Temporäre Ansichts- und Farbauswahl danach zurückgestellt. Keine horizontalen Layoutprobleme sichtbar. Vorschauen: artifacts/kleiner-pfeil-und-story.png und artifacts/lernplan-im-eigenen-tempo.png.
* Normales Lernprofil nicht für Bearbeitungstests benutzt und nicht zurückgesetzt. Prüfsumme vor/nach Release-Prüfung und Neustart identisch. Heimnetz-Startseite, app.css und _framework/blazor.web.js liefern jeweils HTTP 200. Die Datenbankstruktur und synthetischen Startdaten wurden nicht geändert.

## Aufgeräumte Aufgabenansicht (03.11.2026)

Dieser Stand ersetzt die frühere Inline-Wiederanzeige aus Paket A; historische Prüfabschnitte dokumentieren den damaligen Stand.

* 140 .NET-Prüfungen in Debug und Release mit ausdrücklich eingeschalteten SQL-Integrationstests erfolgreich, keine übersprungenen Fälle, keine Buildwarnungen oder -fehler. Zwei zusätzliche Renderfälle prüfen die erreichbare Aufklappsteuerung und den versteckten Listeninhalt. Ein weiterer Test prüft gleichzeitige Speicherung von Sidebar-Einstellung, Hilfseinstellung und Aufgabenentwurf sowie das erneute Aufklappen nach Neuladen. Die vorhandenen Hilfetests verlangen jetzt ausdrücklich keine Inline-Wiederanzeige.
* Browser mit separatem QA-Profil auf Port 5081: Aufgabenliste ein-/ausgeklappt; Auswahl über Aufgabenwechsel, Neuladen und Wechsel zur Werkstatt erhalten. Richtige SQL-Abfrage bewertet; Entwurf und Modus ohne Hilfe nach Rückkehr erhalten. Geführte Aufgabe zeigt die fachliche Erklärung unter dem Editor. Kein Dein-Lernraum-Link, generischer Modustext oder Speicherhinweis mehr.
* Allgemeine Hilfe per X ausgeblendet, kein Wiederanzeige-Link an der Aufgabe. Im Lernplan reaktiviert und in der Werkstatt wieder sichtbar; erneut ausgeblendet. Desktop und schmale Ansicht mit 390 × 844 Pixeln visuell geprüft; auch mit aufgeklappter Liste kein horizontal überlaufender Arbeitsbereich sichtbar.
* Vorschau: artifacts/aufgabenansicht-aufgeraeumt.png. Normales Lernprofil weder zurückgesetzt noch für Bearbeitungstests verwendet. Keine Änderung an JavaScript oder SQL-Inhalten; die sechs JavaScript-Tests stammen aus dem früheren Designpaket.

## Verbesserungspaket A: Punkte 3–6 und 8 (03.11.2026)

Aktueller UI-Stand: Modus je Aufgabe gespeichert, allgemeine Hilfe ausblendbar, Sprunglinks entfernt und Navigation als Aufgabe beschriftet. Frühere Prüfabschnitte bleiben als Nachweis des damaligen Änderungsstands bestehen; die neue Modusspeicherung ersetzt die frühere seitenweite Auswahl.

* Acht neue automatisierte Fälle prüfen getrennte Aufgabenmodi in beiden Lernbereichen, Standardwerte alter Dateien, Erhalt der Leistungen, gleichzeitige Speicherung von Aufgaben und Anzeigeeinstellung, Wiederherstellung und Schutz beschädigter Profile sowie die gerenderte Hilfekomponente.
* Alle 137 .NET-Tests einschließlich der ausdrücklich aktivierten SQL-Integrationstests bestehen in Debug und abschließend in Release. Keine übersprungenen Tests, keine Buildwarnungen oder -fehler. Die JavaScript-Designlogik wurde nicht geändert; ihre sechs Tests waren im früheren Designpaket erfolgreich.
* Browser: Aufgabe 1 ohne Hilfe richtig gelöst, Aufgabe 2 auf geführt gestellt, zu Aufgabe 1 zurückgekehrt. Aufgabe 1 blieb ohne Hilfe, ihr Entwurf und der selbstständige Erfolg blieben erhalten. Beide Modi über vollständiges Neuladen bestätigt.
* Browser: Eine neue Aufgabe nach einer geführten Aufgabe öffnet ohne Hilfe. Dasselbe Verhalten für beide Werkstattaufgaben geprüft. Die Aufgabenbereiche und gleich nummerierte Aufgaben beeinflussen einander nicht.
* Browser: Leerer Editor wechselt ohne Fragefenster zum Selbsttest. Bei vorhandenem Entwurf erscheint die Bestätigung; Entwurf behalten lässt Inhalt und geführten Modus unverändert.
* Browser: X blendet den Bedienhinweis aus. Die Einstellung gilt auch nach Aufgabenwechsel, Neuladen und im anderen SQL-Lernweg. Wiederherstellung im Lernplan und direkt an der Aufgabe erfolgreich.
* Browser: Keine alten Sprunglinks, Navigation Vorherige Aufgabe/Nächste Aufgabe vorhanden. X-Button und Abstand zum Auftrag visuell geprüft.
* Testinstanz mit Port 5081 und artifacts/qa-package-a-profile verwendet. Browserergebnisse: artifacts/paket-a-browserpruefung.json; Vorschau: artifacts/paket-a-selbsttesthinweis.jpg. Das normale Lernprofil wurde für keine Testabgabe verwendet.
* Noch nicht Teil dieses Pakets: Punkte 1, 2, 7, 9, 10 und 11. Einzelheiten und Reihenfolge stehen in docs/verbesserungsplan.md.
## Heimnetz-Modus (03.11.2026)

* 27 neue Tests prüfen Loopback, private IPv4-/IPv6-ULA-Adressen, IPv4-gemappte IPv6-Adressen, Grenzen der privaten Bereiche, öffentliche/linklokale/unbestimmte Quellen und fehlende Quelladressen. Nur der ausdrücklich aktive LAN-Modus erlaubt private entfernte Quellen.
* Lokaler Modus auf separatem Testport: localhost antwortet mit 200, eine Anfrage über die tatsächliche Heimnetzadresse mit 403. X-Forwarded-For: 127.0.0.1 umgeht diese Sperre nicht.
* LAN-Profil mit eigenem QA-Lernstand: Startseite, app.css, _framework/blazor.web.js und _blazor/negotiate antworten über die Heimnetzadresse mit 200. Die anfänglich fehlende Frameworkressource wurde durch das ausschließlich für LanDemo aktivierte Buildmanifest-Laden behoben.
* Im Browser wurde über die Heimnetzadresse eine SELECT-Mission erfolgreich bewertet und gespeichert. Der Hinweis auf den gemeinsamen Lernstand war sichtbar. Vorschau: artifacts/heimnetz-vorfuehrung.jpg.
* Windows-Firewallregel nach Administratorbestätigung vorhanden und geprüft: TCP 5080, eingehend erlaubt, Private, LocalSubnet, Edge Traversal Block. Netzwerkprofil WLAN: Private.
* Nach Testabschluss läuft die Vorführung auf Port 5080. Listener 0.0.0.0 und HTTP 200 für Startseite sowie Frameworkskript über die Heimnetz-IP bestätigt. Die Bearbeitungstests verwendeten artifacts/qa-network-profile, nicht das normale Lernprofil.
* Einschränkung: Die HTTP-/Browserprüfung lief auf dem Trainer-Rechner über dessen echte LAN-Adresse. Erreichbarkeit von einem zweiten physischen Rechner und dessen Netzwerksegment muss dort geprüft werden; Gastnetz-Isolation oder andere Firewalls werden damit nicht ausgeschlossen.
* Abschließender Build ohne Warnungen/Fehler, 129 .NET-Tests bestanden, 0 fehlgeschlagen, 0 übersprungen. Die sechs zuvor bestandenen JavaScript-Tests betreffen unveränderte Designlogik.
## Erfolgsrückmeldung bei übernommenem Versuch

* Drei zusätzliche Renderingtests prüfen Erfolgstexte für geführtes Lernen, einen übernommenen Versuch im Modus ohne Hilfe und einen frischen Selbsttest. Im übernommenen Fall wird kein Wechsel in den bereits gewählten Modus empfohlen.
* Ermittlung und Werkstatt nutzen dieselbe Rückmeldung. Die bestehende Fortschritts- und Bewertungslogik bleibt unverändert.
* Im Browser mit eigenem älteren Testprofil: Mission 4 korrekt gelöst, übernommener Entwurf ausdrücklich erklärt, Aufgabe als abgeschlossen gespeichert. Nach Neu ohne Hilfe starten und Bestätigung: Editor leer, bisheriger Abschluss erhalten. Erneute richtige Abgabe: normaler Selbsttest-Erfolgshinweis und Independent=true.
* Der gespeicherte Abgabenzähler betrug anschließend zwei; die Prüfung griff nicht auf den normalen Nutzerlernstand zu. Vorschau: artifacts/uebernommener-versuch-hinweis.jpg.
* Abschließender automatisierter Stand: 102 .NET-Tests erfolgreich, 0 fehlgeschlagen, 0 übersprungen. Die Designlogik und ihre sechs zuvor erfolgreichen JavaScript-Tests wurden nicht verändert.
## Konkrete Syntaxhinweise und Fortsetzen

* 15 neue Testfälle prüfen offene oder gemischte Anführungszeichen, maskierte Apostrophe, zitierte Bezeichner, eckige Klammern, offene/überzählige runde Klammern, offene Kommentare und allgemeine Parserfehler. Gültige maskierte Zeichen sowie Kommentare mit Anführungszeichen bleiben akzeptiert.
* Das exakte Nutzerbeispiel mit 'Essen" liefert in beiden Validatoren eine Erklärung zu einfachen Anführungszeichen mit dem betroffenen Ausschnitt statt einer Spaltennummer.
* Browser: Die SQL-Bewertung zeigte die Meldung genau einmal. Nach der Syntaxkorrektur waren die tatsächlichen sieben Ergebniszeilen und die fachliche Rückmeldung weiterhin sichtbar.
* Browser: Werkstattbewertung erzeugte einen Feedbackblock ohne doppelte Fehlerausgabe. Die Probeausführung erzeugte weiterhin genau einen sichtbaren Warnhinweis.
* Browser: Ein leeres Testprofil zeigte Ermittlung starten; nach Bearbeitung führte Weiter ermitteln korrekt zu Mission 4 und übernahm den gespeicherten Entwurf.
* Alle 99 .NET-Tests inklusive SQL bestanden, keine übersprungen. Die sechs JavaScript-Tests betreffen die unveränderte Designlogik und wurden bei der vorherigen Designänderung erfolgreich ausgeführt.
* Getrennte Release-Testinstanz auf Port 5081 mit artifacts/qa-feedback-profile; das normale Lernprofil blieb unangetastet. Vorschauen: artifacts/sql-fehlerhinweis.jpg und artifacts/ermittlung-fortsetzen.jpg.
## Anführungszeichen bei Textwerten

* Das gemeldete SELECT mit Status = "verschwunden" erhält bei Probeausführung und Bewertung den gezielten Hinweis auf einfache Anführungszeichen. Es wird als fachlicher Fehler und nicht als technische Unterbrechung behandelt.
* Dasselbe SELECT mit Status = 'verschwunden' funktioniert; doppelt zitierte echte Tabellen-/Spaltennamen liefern dasselbe Ergebnis.
* Doppelte Anführungszeichen in Zeilen-/Blockkommentaren, korrekt begrenzten Texten und Ausgabealiasen erzeugen bei einem anderen unbekannten Spaltennamen keinen falschen Zusatzhinweis. Eckig geklammerte unbekannte Spalten behalten die normale Rückmeldung.
* Die SQL-Werkstatt liefert den Hinweis auch bei UPDATE mit einem doppelt zitierten Textwert. UPDATE mit korrekt doppelt zitierten Spalten und einfach zitiertem Text bleibt ausführbar.
* Die zwei neuen Integrationstests und alle übrigen .NET-Tests bestehen: 84 erfolgreich, 0 fehlgeschlagen, 0 übersprungen. Die sechs JavaScript-Prüfungen waren im vorherigen Stand erfolgreich; die Designlogik wurde nicht verändert.
## Blaues Design und Lernmodus als Standard

* Ermittlungsakte und Werkstatt öffnen mit „Ohne Hilfe lösen“; die geführte Alternative erklärt Erklärungen, Hinweise und Probeausführung direkt in der Auswahl.
* Der Wechsel zur Unterstützung erhält den eingegebenen SQL-Text. Ein geöffneter Hinweis erscheint ausschließlich im geführten Modus.
* Wechsel zum Selbsttest bietet eine ausdrückliche Auswahl; „Entwurf behalten“ lässt Text und Hinweise bestehen. Erst „Neuen Versuch starten“ leert beides, erhält aber bereits gelöste Aufgaben.
* Ein geführter korrekter Versuch wurde mit Completed=true und Independent=false gespeichert. Nach dem Neuladen war der Entwurf erhalten und weiterhin als unterstützt gekennzeichnet.
* Ein anschließend bestätigter Selbsttest wurde korrekt als unabhängig gewertet. Werkstattvorschau und korrekte Werkstattabgabe funktionieren ebenso; danach zeigte die Übersicht 2/40 und zwei ohne Hilfe gelöste Aufgaben.
* Zwei weitere Renderingtests sichern die ausdrücklich als true/false ausgegebenen Zustände der Moduswahl für assistive Technik ab. Die Tabellen-Sprunglinks wurden in beide Richtungen und aus den geöffneten Fallnotizen heraus geprüft; der Entwurf blieb erhalten.
* Fünf zusätzliche automatisierte Tests prüfen die dauerhafte Kennzeichnung in beiden Lernbereichen, unterstützte Entwürfe, alte Profilformate und den Erhalt von Erfolgen beim Neustart.
* Blaues helles und grau-blaues dunkles Design wurden im Browser visuell geprüft. Die Startseite und der Werkstatteditor passten bei 390 Pixeln ohne horizontales Überlaufen; die Moduswahl und die Bestätigung blieben bedienbar.
* Alle Browserprüfungen dieser Änderung verwendeten den eigenen Port 5081 und artifacts/qa-ux-profile. Das normale Profil blieb unangetastet.

## Darstellung, Cursor und optionale Schemas

* Kurze, ausgeschriebene und gemischte Tabellenbezüge einschließlich JOINs und Unterabfragen werden akzeptiert und für die Zusatzprüfung richtig aufgelöst.
* Unbekannte Tabellen, dbo/probe und fremde Datenbankbezüge bleiben abgewiesen. Literale bleiben beim Umschreiben unverändert.
* Sechs unabhängige JavaScript-Tests prüfen die Designlogik: Systemerkennung samt simuliertem Systemwechsel, manuelle Wahl samt Neuladen, Rückkehr zu System, ungültige/gesperrte Cookies, Blazors neu eingesetzten Auswahlschalter und Secure auf HTTPS.
* Im Browser wurde eine dunkle Systempräferenz erkannt und automatisch dunkel dargestellt. Die manuelle Wahl Hell blieb nach vollständigem Neuladen erhalten; Dunkel und die Rückkehr zu System funktionierten ebenfalls.
* Während einer tatsächlich laufenden SQL-Abfrage war der Editor readonly und nicht disabled. Im hellen Modus waren Hintergrund und Textcursor kontrastreich verschieden; der eigene SVG-Mauszeiger war als Cursor hinterlegt. Im dunklen Modus wurde eine helle Cursorfarbe auf dunklem Editorhintergrund geprüft.
* Eine Abgabe ohne Schemapräfix wurde über die Oberfläche erfolgreich bewertet und im dunklen Design mit Ergebnistabelle angezeigt.
* Ein Abschluss in der Ermittlung und einer in der Werkstatt ergaben auf der neuen allgemeinen Übersicht korrekt 2 von 40 verfügbaren Übungen.
* Die schmale Startseite wurde bei 390 Pixeln ohne horizontales Überlaufen geprüft.

## Zusätzliche Prüfung des Ausbaus

* Vollständig ausgeschriebene Spalten-/Sternbezüge werden bei der Ermittlung korrekt mit dem Tabellenschema auf die Prüfdaten umgeleitet.
* Sehr kleine Gleitkommazahlen werden nicht fälschlich als null akzeptiert.
* Syntaktisch gültiges, aber strukturell beschädigtes Fortschritts-JSON wird erkannt und unverändert erhalten.
* Alte Profilversionen bleiben lesbar; gleich nummerierte Werkstatt- und Ermittlungsaufgaben überschreiben einander nicht.
* Alle zehn Werkstattreferenzen erzeugen den gefragten Endzustand auf beiden Startdatenbeständen.
* Ungefiltertes UPDATE, fest eingetragene Artikel-Ids, feste statt relative Kontostände, fehlende/falsche CHECK-Regeln und reine SELECT-Scheinlösungen werden abgewiesen.
* Ein vor ROLLBACK unverändertes oder nur um den falschen Betrag geändertes Konto besteht die Aufgabe nicht.
* PRIMARY KEY, NOT NULL und CHECK werden durch tatsächliche gültige/ungültige Einfügeversuche überprüft.
* Alternative benannte Constraints und abweichende Groß-/Kleinschreibung von Tabellen-/Spaltennamen werden akzeptiert.
* Parallel laufende Aufgaben haben getrennte Arbeitskopien. Ein neuer Versuch sieht wieder die Ausgangsdaten.
* Auch der rohe Werkstatt-Ausführer kann keine permanenten Falltabellen verändern oder anlegen. Der Gesamtwert der Ermittlungsdaten bleibt 162000.
* Offene Transaktionen und verletzte Datenregeln liefern verständliches Feedback; folgende Versuche starten unverändert.

## Ergänzende Browserprüfung

Getestet auf einem separaten lokalen Port mit eigenem QA-Profil, ohne Verwendung des normalen Lernstands:

* Werkstatt über Hauptmenü geöffnet; falsches und korrektes UPDATE abgegeben.
* Modus ohne Hilfen mit leerem Editor und ausgeblendeter Probeausführung geprüft; korrekte Abgabe und Entwurf nach Neuladen erhalten.
* CREATE TABLE mit Schlüssel, Pflichtfeldern und Mengenregel abgegeben; Struktur und Ergebnisse der Einfügeprüfungen angezeigt.
* ROLLBACK abgegeben: Kontostand vor Rücknahme 550, nach Rücknahme wieder 600.
* Laufende große Abfrage per Modul- und Missionswechsel verlassen; neue Aufgabe ohne fremdes Feedback anschließend erfolgreich abgegeben.
* Ausschließlich die tatsächlich gelöste Ermittlungsmission erhielt einen Abschluss; Werkstattfortschritt blieb separat.
* Werkstattdarstellung bei 1440 Pixeln sowie schmal bei 390 Pixeln ohne horizontales Überlaufen geprüft; Vorschau als lokales Artefakt gesichert.

## Browserprüfung

* Hauptmenü und Kennzeichnung zukünftiger Module geöffnet.
* SELECT-Abfrage über den freien Editor ausgeführt: zwölf echte Ergebniszeilen.
* Antwort abgegeben und erfolgreiche Bewertung gegen beide Datenbestände angezeigt.
* Hinweis einzeln geöffnet; nächste Hilfestufe korrekt angeboten.
* Modus ohne Hilfen aktiviert: Entwurf leer, Erklärung/Hinweise/Probeausführung entfernt.
* Korrekte selbstständige Antwort abgegeben und gesondert erfasst.
* Seite neu geladen: gespeicherter Entwurf und Abschluss erhalten.
* LEFT JOIN über die Oberfläche abgegeben: Sendungen ohne Fahrzeug bleiben mit NULL-Kennzeichen enthalten.
* Fehlerhafter Spaltenname im freien Editor ausgeführt: deutsche Rückmeldung ohne Verbindungsabbruch.
* Desktopdarstellung bei 1440 Pixeln und schmale Darstellung bei 390 Pixeln geprüft; kein horizontales Überlaufen der Seite festgestellt.

## Grenzen der Prüfung

Die automatisierten Prüfungen belegen die genannten konkreten Abläufe. Es wurde keine universelle semantische SQL-Gleichwertigkeit, vollständige Sicherheitszertifizierung, Mehrbenutzerfähigkeit oder vollständige IHK-Themenabdeckung nachgewiesen. Die Oberfläche unterstützt lokale Nutzung und eine ausdrücklich aktivierte Vorführung im privaten Heimnetz mit gemeinsamem Profil. Eigene Benutzerkonten und öffentlicher Appbetrieb sind auf Nutzerwunsch nicht mehr geplant. Lokale Windows-Releases sind der vorgesehene Verteilungsweg. Der fachliche Abschluss einer Mission ist keine Aussage über eine spätere IHK-Note.
