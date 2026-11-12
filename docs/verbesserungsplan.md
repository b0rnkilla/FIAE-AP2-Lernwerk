# Stand und weitere Entwicklung

Die Quellfassung **1.0.0-beta.1** umfasst acht Lernbereiche mit 454 Übungen. Die sechs Befunde aus dem Abschlusscheck sind behoben und nachgeprüft. Alpha 16 aus Commit `1dbcb12` ist auf GitHub veröffentlicht. [Entwicklungsprotokoll](entwicklungsprotokoll.md), [Testprotokoll](testprotokoll.md).

## Umgesetzte Rückmeldungen

Nach Beta 1 trägt die Windows-EXE das eingebettete a²-Symbol. Die GitHub-ZIP bleibt vorerst unsigniert. Die README erklärt den SmartScreen-Dialog beim ersten Start. Für die stabile 1.0.0 soll der Microsoft Store als zusätzlicher Vertriebsweg geprüft werden. [Paketbau](lokale-releases.md#für-die-veröffentlichung-vorbereiten).

| Bereich | Ergebnis |
| --- | --- |
| SQL-Schreibweise | Große Schlüsselwörter/Funktionen und Semikolon je äußerer Anweisung; konkrete Korrekturhinweise |
| Lernmodus | Ohne Hilfe als Standard, Auswahl je Aufgabe gespeichert, Rückfrage nur bei vorhandenem Entwurf |
| Übersichtlichkeit | Weniger Erklärungstext, einklappbare Aufgabenliste, ausblendbarer Bedienhinweis |
| Navigation | Einheitlich Aufgabe statt Mission, unnötige Sprunglinks entfernt |
| Modulnavigation | Übersicht und Lernplan zuerst; anschließend SQL-, Algorithmus- oder WiSo-Links passend zum besuchten Bereich |
| Pseudocode-Einstieg | Acht Aufgaben mit eigener Story, vier Aufgabenarten, lokalem Interpreter, Prüffällen und Ablaufspur |
| Pseudocode-Ausbau | Zunächst 16, anschließend sechs weitere Aufgaben; Kennzahlen, lineare/binäre Suche, Sortierung, verschachtelte Listen und Funktionen; insgesamt 30 Algorithmus-Aufgaben und zusammen mit SQL 70 Übungen; mit WiSo insgesamt 220 |
| SQL-Spickzettel-Rückweg | Einheitlicher Button zur konkreten Aufgabe aus Ermittlung oder Werkstatt; Ziel bleibt beim Neuladen erhalten |
| Funktionsbewertung | Benannte Funktion mit Argumenten und Rückgabevertrag; eigene lokale Bereiche, Wertkopien und Bereichsanzeige in der Vorschau |
| Pseudocode-Feinschliff | Technischen Zusatz auf der Themenkarte entfernt; Zuweisungen in Beispielen, Hinweisen und Lösungen mit `=`, Gleichheitsvergleiche bevorzugt mit `==` |
| Pseudocode-Editor | Nummerierte Zeilen einschließlich Leerzeilen, synchrones Scrollen, Tab-/Blockeinrückung und Ausrücken; Esc vor Tab für Tastaturnavigation |
| Pseudocode-Schreibweise | Große Schlüsselwörter, Operatorwörter, Wahrheitswerte und Längenfunktion verpflichtend; konkrete Korrektur mit Zeilennummer, Texte/Kommentare und Variablenschreibung unberührt |
| Heimnetz für Windows-Anwender | Alpha 12: eigene Einstellungsseite, Firewall-Diagnose und bei Bedarf UAC-Helfer, Kopieradresse/QR, interner Hostneustart; gemeinsames Profil. Kein neues Paket. Erhöhte Regelerstellung manuell zu bestätigen |
| Android im Heimnetz | Chrome-Zugriff nach Android-Berechtigung unter „Geräte in der Nähe“ vom Nutzer bestätigt; Anleitung ergänzt, keine Einzel-IP-Freigaben nötig |
| Scrollposition | Liste bleibt bei asynchronen Aufgabenwechseln innerhalb beider SQL-Lernwege stehen |
| Geführte Hilfen | Einheitliche Zeilen mit Beschreibung links und Aktion rechts; Spickzettel im selben Tab, Musterlösung mit Ansehen/Ausblenden |
| Lernplan | Profilbezogene nächste Schritte statt fester Wochen |
| Geschichte | Verbundene Storyschritte und chronologische Fallnotizen mit offenen Lücken |
| SQL-Beispiele | Kurze Tabellennamen und Datum/Uhrzeit als YYYY-MM-DD HH:mm:ss |
| Verbindungsfehler | Deutsche Dialoge mit lesbaren Hell-/Dunkelfarben und ausdrücklichem Neuladen |
| Darstellung | Helle blaue und dunkle grau-blaue Palette, Systemwahl und gespeicherte Präferenz |
| Einrichtung | Versionsanzeige, lokale SQL-Instanzauswahl und öffentliche GitHub-Updateprüfung |
| Projektpflege | Kurze README, eigene Entwickleranleitung, Modulordner und passende Namespaces |
| Lizenz | Ausschließlich private Nutzung; keine pauschale MIT-Freigabe, Drittanbieterlizenzen bleiben eigenständig |
| WiSo-Lernbereich | 150 eigene Fragen, acht Themen und sieben Antwortformen; nachvollziehbare Erklärungen, Merksätze, eigene Abbildungen, Quellen und Spickzettel mit Rückweg |
| WiSo-Runden | Themenlernen, gemischte Zehnerrunden, Fehler-/Unsicherheitswiederholung sowie 30 Aufgaben mit 60 Minuten und Auswertung nach Abgabe |
| WiSo-Profil | Fortsetzbare Antworten, Kontrollmarkierungen, Pausen, getrennte Fortschritte und letzte Ergebnisse; Revision schützt vor veralteten Fenstern |
| WiSo-Themenposition | Ursprüngliche Übersicht von 01 bis 08 wiederhergestellt; WiSo bleibt letzter Themenbereich |
| WiSo-Quellen | Quellentitel ohne wiederholte externe Tab-Zusätze; ein gemeinsamer Hinweis im Spickzettel beziehungsweise Quellenbereich |
| WiSo-Abgabe | Bestätigung wird nach dem Rendern fokussiert und sichtbar; anschließend auch die neue Auswertung |
| WiSo-Bedienhilfe | Erklärung zu Unsicherheit und Kontrollmarkierung oben im Spickzettel; keine zusätzliche dauerhaft eingeblendete Hilfe in den Aufgaben |

Die elf ursprünglichen SQL-Anpassungen sind abgeschlossen. Die nachfolgenden Wünsche zur Bedienung, Einrichtung, Dokumentation und Codeordnung sind ebenfalls umgesetzt. Eine Bewertung anhand zweier Datenbestände bleibt eine begrenzte fachliche Prüfung, keine allgemeine SQL-Beweisführung.

## Projektauftritt und Unterstützung – umgesetzt

Nach der positiven WiSo-Rückmeldung hat der Nutzer die Vorlage und zusätzlich den Issues-Verweis als fünften Punkt freigegeben. Alle fünf Punkte sind im Quellstand umgesetzt und mit eigenen Profilen geprüft:

1. **Freiwillige Unterstützung:** dezente Karte am Ende der Übersicht und auf der Projektseite sowie ein Fußzeilenlink. Freigegebener öffentlicher PayPal-Link: https://www.paypal.com/paypalme/b0rnkillafpv. Zahlung extern, Betrag frei wählbar.
2. **Projekt-/Urheberseite:** `/ueber-das-lernwerk` nennt Christopher Weismantel und b0rnkilla, beschreibt die Entstehung und verweist auf Nutzungsbedingungen und Drittanbieterhinweise. Das spätere Original-Logo kann die jetzige Wortmarke ergänzen.
3. **GitHub-README:** kurzer Download-Einstieg, vier neue eigene Browserbilder, kompakte Lernbereichsgalerie, drei Startschritte und Projektaktionen. Die README beschreibt auf Nutzerwunsch die aktuelle App-Fassung; Vorschau-/Paketvergleich, FIAE-Präfix im Leitsatz und der Designzusatz in der Downloadzeile sind entfernt. Technische Dokumente halten den tatsächlichen Paketstand weiter fest.
4. **GitHub-Anerkennung:** „Auf GitHub unterstützen“ mit Erklärung des einzelnen Star-Schalters. Keine erfundene Fünf-Sterne-Bewertung und keine automatische Sternvergabe.
5. **Fehler und Vorschläge:** eigener Link zur GitHub-Issues-Übersicht auf Übersicht, Projektseite und in der Fußzeile. Die Projektseite erklärt hilfreiche Angaben und die Öffentlichkeit der Meldungen; keine automatischen Uploads oder Meldungen.

Weitere Details: [Projektauftritt](projektauftritt.md). Diese Funktionen sind im geprüften Alpha-8-Paket enthalten. Weitere Release-Erstellung weiterhin nur auf ausdrücklichen Auftrag; das später nachgereichte Logo ist inzwischen in Alpha 9 eingebunden.

## Weitere Lerninhalte

1. Zusätzliche SQL-Transferaufgaben und weitere Datenbankthemen nach Bedarf.
2. Pseudocode und Algorithmen: abgeschlossener Basisumfang von 30 Aufgaben. Die sechs Ergänzungen behandeln Prüfziffern, konkreten Schleifenaufwand, weitere Grenztests, rekursiven Rückweg und Fortschritt sowie eindeutige Positionspaare. Aus der bisherigen Aufgabenplanung bleibt nichts offen; keine vollständige Katalogabdeckung zugesagt.
3. WiSo: vereinbarter Umfang mit 150 Aufgaben umgesetzt; Quellenstand, Eingabeformen und eigene Punktevergabe dokumentiert. Keine vollständige Katalogabdeckung zugesagt. Inhaltserweiterungen bei konkretem Bedarf.
4. UML: Diagramme verstehen und Fehler finden, anschließend begrenzte Modellierungsaufgaben.
5. Weitere AP2-Bereiche sowie systematische Zuordnung zum aktuellen Prüfungskatalog.

## Spätere Veröffentlichung

Alpha 7 wurde auf ausdrücklichen Wunsch aus der damaligen, noch uncommitteten Arbeitsfassung ohne WiSo gebaut und aus dem ZIP geprüft. Benutzeranleitung, Versionshinweise, aktuelle Projektlizenz und Drittanbieterhinweise liegen bei. EXE-Start, Pseudocode ohne SQL, Funktionen/Rekursion, SELECT, Werkstattänderung und Übernahme eines früheren isolierten Alpha-4-Testprofils sind geprüft; keine privaten Dateien im Paket. ZIP und SHA256-Datei liegen lokal vor. Vor dem Tag aktuelle Änderungen committen und pushen; der alte Basiscommit ist kein passendes Release-Ziel. Der Nutzer übernimmt die Veröffentlichung; es gibt keinen automatischen Upload.

Die früheren Alpha-3- und Alpha-4-Pakete bleiben unverändert. Die README führt Anwender zu Releases → Windows-ZIP → Entpacken → EXE. Das alte 0.1.0-Paket wird nicht zur Verteilung empfohlen. Ein Test auf einem weiteren frisch eingerichteten Windows-Rechner wurde hier nicht durchgeführt. Weitere Pakete bleiben ausdrücklich zu beauftragen beziehungsweise dem vereinbarten Abschluss vorbehalten.

Login, Registrierung, Benutzertabellen und öffentlicher Homeserver-/nginx-Betrieb sind gestrichen. Die Algorithmus-Bewertung benötigt keine KI-Anbindung. Weitere KI-Funktionen würden eine gesonderte Entscheidung voraussetzen.


## Release Alpha 8 vorbereitet

**Windows-Paket 0.1.1-alpha.8: gebaut und aus dem entpackten ZIP geprüft am 05.11.2026**, aus dem sauberen Commit `b4e953fbb628b9c39ce695538db2bf1343f13bcf`. Der Nutzer hat den Commit gepusht; der Remote-Stand wurde bestätigt. Enthalten sind 220 Übungen: 40 SQL-, 30 Algorithmus- und 150 WiSo-Aufgaben sowie der neue Projektbereich. Veröffentlichung und Upload erfolgen durch den Nutzer. ZIP und SHA256-Datei liegen lokal vor; Tag, Titel und Asset-Angaben stehen unter [lokale Releases](lokale-releases.md). Nachträgliche Prüfdokumentation wird separat gepflegt; das geprüfte ZIP wird nicht überschrieben. Das nachgereichte Logo ist im Quellstand Alpha 9 auf der Projektseite eingebunden. Der Nutzer hat die Veröffentlichung von Alpha 8 bestätigt; für Alpha 9 besteht noch kein Paketauftrag.

## Ergänzung Alpha 9: Mehrfachauswahl

Erledigt: Hinweis direkt vor den Optionen hervorgehoben, erforderliche Anzahl ausgeschrieben und fett. Einfachauswahl und Auswahlzähler erhalten. Build, 16 WiSo-Komponentenprüfungen und Browserdarstellung in beiden Designs sowie schmaler Ansicht erfolgreich. Kein neuer Paketauftrag.

## Alpha 9: Aufgabenwechsel und Prüfungsrunden

Umgesetzt auf ausdrückliches GO: Gesamtseitenposition bei Listenklicks in SQL, Werkstatt und Pseudocode erhalten; WiSo-Fokus auf Abgabebestätigung/Auswertung unverändert. Zusätzlich zehn eigene Pseudocode-Prüfungsaufgaben und drei neue SQL-Fälle mit je fünf Aufgaben, getrennte 45-Minuten-Runden, Teilpunkte, Pause, Historie und technische Wiederaufnahme der Bewertung.

Geprüft: 794 .NET-Tests einschließlich SQL, 26 JavaScript-Tests, Browserdurchläufe mit 50/50 Punkten in beiden Modulen, Spickzettel-Rückweg, Pause/Neuladen, Nachbereitung und Handybreite. Keine Originalprüfungen eingebettet. Offen bleibt die praktische Kalibrierung der geschätzten Bearbeitungszeit durch vollständige Endnutzerläufe. Details: [Prüfungsmodus](pruefungsmodus.md). Kein neuer ZIP-Auftrag.

## Alpha 9: Paketbau nach Commit/Push abgeschlossen

Der Nutzer hat Commit und Push bestätigt; GitHub-master entspricht a6c929f. Alpha-9-ZIP auf ausdrücklichen Auftrag gebaut, entpackt und mit eigenem Profil geprüft. Pseudocode und SQL jeweils 50/50, Pause/Neuladen, Neustart, alte Profilfelder, Logo und Paketinhalt erfolgreich kontrolliert. Versionshinweise und Anleitung nach dem Anwendungscode-Commit ergänzt. ZIP und SHA256 unverändert erhalten. Offen: Nutzer-Upload als Pre-release v0.1.1-alpha.9 und praktische Zeitkalibrierung. Paket-/Prüfdetails: [lokale Releases](lokale-releases.md), [Testprotokoll](testprotokoll.md). Kein automatischer Upload; weitere Pakete nur auf Auftrag.

## 06.11.2026 – Einheitliche Prüfungs-Einstiege erledigt

- Übersicht: SQL-Fortsetzung heißt Weiter lernen. Prüfung üben ohne Zeitangabe steht separat darunter; bei Pseudocode unter dem Lernbutton, bei WiSo neu ergänzt. Themenreihenfolge unverändert.
- SQL-/Pseudocode-Einleitung: angeforderte Sätze gestrichen, Zeilenumbruch nach Ausgangssituation beziehungsweise Analyseaufgabe, Nachschlagen im Spickzettel ausdrücklich benannt.
- WiSo: eigener Info-Einstieg /wiso/pruefung, Anlage/Uhr erst mit Prüfung starten. Offene Lern- und Prüfungsrunden erfordern eine zusätzliche Ersetzungsbestätigung und bleiben bei Abbruch unverändert. Frühere Prüfungskarte entfernt, Kopfpunkt Prüfung üben statt Deine Runde.
- Geprüft: 47 betroffene .NET-Tests ohne Fehler/Überspringen; eigener Web-Build ohne Warnungen/Fehler; Browserstart, Fortsetzen, bestätigter Wechsel, Abbruch, beide Designs und 390 × 844. README und Fachdokumente aktualisiert. Quellstand Alpha 10; Alpha-9-ZIP unverändert, kein neuer Paketauftrag.

## 06.11.2026 – Zeilenumbrüche und WiSo-Kartenabstände erledigt

Alle sechs Verbindungshinweise mit expliziten Zeilenumbrüchen, statischer Ausgangstext ebenfalls angepasst. Beide WiSo-Lernwegkarten trennen Beschreibung und Button durch 24 Pixel. Sieben Wiederverbindungstests und eigener Web-Build erfolgreich; tatsächliche Verbindungsunterbrechung auf eigenem Port 5211 und gemessene Kartenabstände bestätigt. Dokumentation nachgeführt, normale App/Lernstände unberührt, kein neuer Paketbau.

## 06.11.2026 – Abschnitt 04 Datenbankentwurf umgesetzt

Nach ausdrücklichem GO: 30 eigene Lernaufgaben in sechs Kapiteln, Modellbaukasten mit freien Beschriftungen und festen Fachattributen, PK-/FK-/UQ-Gruppen, Kardinalitäten, automatisch gezeichnetes Modell mit Vergrößerung und ausgeschriebenen Beziehungen. Individuelle Modi, drei Hinweise, Musterlösung, bestätigte Planungsschritte, Spickzettel und Lernplan-Anbindung.

Drei eigene Prüfungsfälle mit neun Teilaufgaben, 45 Minuten, Teilpunkten, Kontrollmarkierung, Pause, Nachschlagen und unveränderlicher Abgabe. Keine SQL-/KI-Abhängigkeit. Alte Profilfelder und Aufgabenkennungen erhalten; Gesamtlernzähler 250, Modulposition 04 und WiSo 08 unverändert. Bewertung begrenzt auf dokumentierte Attributverträge und geprüfte Varianten; keine vollständige AP2-Katalogabdeckung behauptet.

Geprüft: vollständiger Bestand 840 .NET-Tests einschließlich SQL erfolgreich; nach abschließender UI-Sicherung 53 betroffene Tests und fehlerfreier Build. Browser ohne SQL: eigene Beschriftungen, Modelle 5/21, Modus je Aufgabe, Rückweg, vollständige neue Prüfung 50/50 mit Nachschlagen, Pause/Neuladen, readonly nach Abgabe, Handybreite und Serverneustart. Offen: praktische Kalibrierung der geschätzten 45 Minuten durch Endnutzer. Kein neues ZIP; Details in [Datenbankentwurf](datenbankentwurf.md).

Entwicklungsstart Alpha 12 nachgeprüft: normaler CMD-/VS-Start respektiert gespeicherte Lokal-/Heimnetzwahl; keine erzwungene URL im Standardprofil. Zwei neue Regressionen und 35 betroffene Tests erfolgreich. Optionaler LAN-Starter bleibt erhalten.

Alpha 12: Heimnetz vom Nutzer als einwandfrei bestätigt. Eigene Nutzerprüfung von Abschnitt 04 noch offen. Versionshinweise vorbereitet; Windows-ZIP und Release-Informationen nach Nutzer-Commit und Push ausdrücklich beauftragt. Paketprüfung noch ausstehend.

## Alpha 12: Paketauftrag abgeschlossen

Windows-ZIP nach Commit/Push e3af6aa gebaut und am 06.11.2026 aus dem ZIP geprüft. Enthalten: 250 Lernübungen, die zusätzlichen Prüfungen und Heimnetz-Einstellung. 866 .NET-/31 JavaScript-Tests erfolgreich; isolierte native EXE mit/ohne SQL, Modellbewertung, Pause/Neustart, QR und Profilerhalt geprüft. ZIP/SHA256 vorhanden, Veröffentlichung durch Nutzer ausstehend. Eigene Nutzerprüfung von Abschnitt 04 sowie praktische Zeitkalibrierung bleiben davon getrennte offene Punkte. Keine weitere Paket-/Veröffentlichungsaktion ohne Auftrag.

## 07.11.2026 – Gestaltung von Abschnitt 04 vereinheitlicht

Nutzerwunsch umgesetzt: Datenbankentwurf verwendet den bestehenden Lernrahmen mit linker TaskRail und gemeinsamer Lernmoduswahl. Auftrag, Arbeitsbereich, Hinweise, Musterlösung, Planungsschritte und Navigation folgen SQL/Pseudocode; Baukastenfelder passen sich der schmaleren Arbeitsfläche an. 53 betroffene Tests und Browserprüfungen mit eigenem Profil erfolgreich. Anschließende Nutzerkontrolle offen. Kein neuer ZIP-Auftrag; Alpha 12 ist inzwischen auf GitHub veröffentlicht, Tag und ZIP-Digest stimmen mit e3af6aa und dem lokalen Paket überein.

## 07.11.2026 – Lichtblick-Geschichte überarbeitet

Nutzerpunkt umgesetzt: Alle 30 Entwurfsaufgaben erhalten konkrete Situationen, eine durchgehende Begleitfigur und nachvollziehbare Übergänge ähnlich SQL/Pseudocode. Bestätigte Vorgänger werden nur bei vorhandenem Abschluss aufgegriffen; übersprungene Schritte bleiben offen. Planungsergebnisse sind getrennt von Fachlösungen und nur nach richtigem Abschluss sichtbar. 56 betroffene Tests, unveränderte Fachverträge und Browseransicht auf eigenem Port geprüft. Nutzerkontrolle ausstehend; weiterhin kein neues ZIP.

## 07.11.2026 – Auswertung und Gesamtprüfung Abschnitt 04

Nutzerkritik bestätigt: Punkte und doppelte Ergebnisblöcke passten nicht in den Lernablauf. Behoben durch eigene Lernrückmeldung im bestehenden Stil und klare Trennung von Prüfungswertung. Zusätzlich Lösungsvorwegnahme bei falscher Auswahl, doppelte automatische Tabellennamen, unsaubere schnelle Ladewechsel, mehrfache Prüfklicks, veraltetes Feedback nach Moduswechsel und fehlende Fortsetzung am Moduleinstieg korrigiert. Baukasten-Schreibschutz serverseitig abgesichert. 34 neue Auditfälle; insgesamt 903 .NET-/31 JavaScript-Tests erfolgreich. Eigene Browserprüfung einschließlich Prüfungsabgabe, Pause/Neustart und Handybreite. Erneute Nutzerkontrolle offen, kein Paketauftrag.

## 07.11.2026 – Diagramme und Unterlagen in Abschnitt 04 umgesetzt

Nutzerauftrag nach Sichtung alter Prüfungen: Abschnitt 04 um prüfungsnahe Aufgabenformen ergänzen; SQL/Pseudocode vorerst abgeschlossen, in dieser Alpha keinen neuen Abschnitt beginnen. Additiv umgesetzt als Aufgaben 31–42: ER-Abbildungen gezielt vervollständigen, aus ER-Vorlagen Tabellen entwerfen, vorbereitete relationale Modelle erweitern sowie Standliste und Leihschein normalisieren und Anomalien konkret beurteilen. 42 Entwurfsaufgaben, insgesamt 262 Lernübungen; bisherige Kennungen/Erfolge und Prüfungsfälle erhalten. Vertrauter Lernrahmen, Spickzettel und Rückmeldungen ohne Lernpunkte bleiben. Eigene synthetische Fälle und Bilder. Technische Prüfungen im Testprotokoll; anschließende Nutzererprobung offen. Kein neuer Abschnitt, kein Paketauftrag.

## 09.11.2026 – Abschnitt 03 nach neuer ausdrücklicher Freigabe umgesetzt

Nach Vergleich von Abschnitt 03/05 wurde UML und Modellierung vorgeschlagen. Die Nutzerfrage zu eigenen Diagrammen ist berücksichtigt: Der Lernweg enthält nicht nur Lesen/Korrigieren, sondern einen eigenen Baukasten und 23 Aufgaben von leerer Arbeitsfläche. 36 Aufgaben in sechs Kapiteln, fünf Diagrammarten, zusammenhängende Nordhafen-Geschichte, Spickzettel und drei eigene Prüfungsfälle. Die anschließende ausdrückliche Freigabe ersetzt für dieses Modul die frühere Beschränkung auf Abschnitt 04 in Alpha 12; neue Quellfassung deshalb Alpha 13. SQL/Pseudocode bleiben abgeschlossen, Abschnitt 05 unverändert geplant.

298 Gesamtlernaufgaben, fünf aktive Module. Getrennte Lern-/Prüfungsstände, Revisionen und unveränderliche Abgabe erhalten bestehende Profile. Quellbuild und Regressionen erfolgreich, eigene Browsererprobung auf getrenntem Port; [Prüfnachweise](testprotokoll.md). Nutzererprobung und Zeitkalibrierung stehen noch aus. Kein ZIP/Commit/Push/Upload beauftragt oder durchgeführt.

## 09.11.2026 – Windows-ZIP Alpha 13 und Release-Informationen erledigt

Nach Nutzer-Commit/Push b4224b0 ausdrücklich beauftragt. Selbstständig startbares ZIP samt SHA256 aus dem unveränderten Anwendungscode gebaut, Version und Remote-Commit bestätigt, Inhalt/Lizenzen sowie entpackte native EXE mit eigenem Profil geprüft. 298 Lernübungen, fünf Module, UML-Prüfung 50/50, ER-Ergänzung und Profilerhalt erfolgreich. Versionshinweise nach dem Commit ergänzt; Prüfdokumentation separat. Nutzer lädt hoch und setzt Tag v0.1.1-alpha.13 auf b4224b0 als Pre-release. Keine automatische Veröffentlichung. Als nächster Lernbereich ist Abschnitt 05 Softwareplanung & OOP vorgesehen; zunächst sollen die fehlenden Abschnitte fertiggestellt werden. Ausführliche Nutzererprobung von UML erfolgt später.

## 09.11.2026 – Abschnitt 05 nach Konzeptfreigabe umgesetzt

Nutzerfreigabe für acht Kapitel und 48 Aufgaben umgesetzt. Der Lernweg enthält eigene Klassen-/Musterdiagramme, Architekturbausteine, API-Verträge mit frei geschriebenem JSON/XML und Formularbaukasten mit bedienbarer Vorschau. OOP-Aufrufe werden anhand konkreter Zustände/Rückgaben bearbeitet; keine neue allgemeine Programmiersprache. Freie Begründungen bleiben gespeicherte Selbstkontrolle, keine Schlüsselwortbenotung. Gemeinsamer Lernrahmen, drei Hinweise, Spickzettel, einheitliche Lernrückmeldung ohne Punkte und bestätigte Geschichte. Drei eigene Prüfungsfälle, getrennte Profile und 20/20/10-Gewichtung.

SQL/Pseudocode und die übrigen fachlichen Lernkataloge bleiben unverändert; kein Beginn von Abschnitt 06/07. Quelle Alpha 14 mit 346 Übungen. Build, automatische Prüfungen und isolierte Browsererprobung dokumentiert im Testprotokoll. Nutzer testet anschließend; Zeitkalibrierung bleibt offen. Kein ZIP, Commit, Push oder Upload beauftragt.

## 11.11.2026 – Erste Nutzererprobung positiv, README und Dokumentation nachgeführt

Der Nutzer hat Abschnitt 05 kurz erprobt und einige Aufgaben gelöst; Gestaltung und Umsetzung gefallen ihm. Vollständige Aufgaben-/Prüfungserprobung und Zeitabstimmung bleiben offen. README-Kritik umgesetzt: UML und Softwareplanung erhalten denselben kompakten Platz wie die übrigen Beispiele. Zwei Reihen zu drei Spalten in der Reihenfolge 01/02/03 und 04/05/08, alle Vorschaubilder 120 Pixel hoch und zur großen Ansicht verlinkt. Aktueller Umfang 346 Übungen, sechs aktive Module. Fach-/Entwicklungsdokumentation und Benutzeranleitung auf diesen Stand geprüft; veraltete aktuelle Kopfstände korrigiert, historische Nachweise erhalten.

Versionshinweise Alpha 14 vorbereitet. Nutzer führt Commit und Push aus; danach sind ZIP, isolierte native Paketprüfung und Release-Informationen ausdrücklich beauftragt. Vorher kein Paketbau, kein Commit/Push/Upload. Erst anschließend folgt die Planung des nächsten noch fehlenden Abschnitts.

## 11.11.2026 – Windows-ZIP Alpha 14 nach Nutzer-Push erledigt

Sauberer gepushter Commit 33f09a7 und Remote-master bestätigt. ZIP mit 346 Übungen und sechs Modulen aus unverändertem Code sowie den bereits committeten Paketdokumenten gebaut. 1.090 .NET-/31 JavaScript-Prüfungen erfolgreich, 20 echte SQL-Integrationen ausgelassen. Native EXE aus dem ZIP mit eigenem Profil: API, XML und Formularreparatur ohne SQL, eigene Architektur und neue Softwareprüfung 50/50, Pause/Hostneustart, Markierung, Antwortsperre und alte Profilfelder erfolgreich. 59 Dateien nach Lauf unverändert. ZIP 51.548.922 Bytes und SHA256 vorhanden, ältere Archive erhalten. Nutzer veröffentlicht als Pre-release v0.1.1-alpha.14 auf 33f09a7; kein automatischer Tag/Upload/Commit/Push. Als nächstes wird nach diesem Release-Schritt ein Konzept für einen noch fehlenden Abschnitt erarbeitet; in diesem Paket keiner begonnen.

## 11.11.2026 – Abschnitt 06 nach Konzept und Nutzer-GO umgesetzt

Alpha 14 ist laut Nutzer veröffentlicht. Auf das freigegebene Konzept folgen 54 eigene Aufgaben zu Tests, Qualität und Sicherheit in neun Kapiteln. Eigene Testsätze gegen acht lokale Modelle, unabhängige Sollwerte, tatsächliche S-/D-/Pfadabdeckung und konkrete Fehlervarianten bilden die praktische Grundlage. Reviews, Ausarbeitungen, eigene Berechtigungsmatrix und Ablaufbaukasten ergänzen Unit-Tests, Fehlerbericht, Freigaben und Schutzmaßnahmen. Zwei sichtbare Reparaturvorlagen, drei eigene Prüfungsfälle und eigener Spickzettel. Keine Fremdcode-/Angriffsausführung, keine automatische Benotung freier Begründungen. Der gemeinsame Lernrahmen und Lernfeedback ohne Punkte bleiben erhalten.

Quelle Alpha 15 mit 400 Übungen und sieben aktiven Modulen. Eigene Profile/Revisionsschutz und unveränderliche Prüfungsabgabe bewahren alte Modulstände. README bleibt kompakt mit sieben gleich kleinen Bildern; Pflichtdokumente/Fachvertrag/Benutzeranleitung nachgeführt. 1.147 .NET-/31 JavaScript-Prüfungen erfolgreich, 20 SQL-Integrationen bewusst ausgelassen. Browser mit eigenem Profil auf 5240 einschließlich Prüfung 50/50, Pause/Neustart und Handyansicht geprüft. Nutzer testet anschließend. Abschnitt 07 bleibt offen; SQL/Pseudocode weiterhin abgeschlossen. Ausdrücklich kein neues ZIP, Commit/Push oder Upload; Alpha-14-Archiv und Prüfsumme unverändert.

## 11.11.2026 – Zusatzfreitext auf Nutzerwunsch vollständig aus der Bedienung entfernt

Abschnitt 05/06 besitzen weder im Lernen noch in Prüfungen zusätzliche Begründungs-/Notizfelder. Zugehörige Reflexionskarten, Selbstkontrolllisten, Hinweise und verwaiste Metadaten entfernt; betroffene Aufgabentexte und Dokumentation bereinigt. Reguläre Fachangaben, Modelle und Nachrichteneditoren bleiben erhalten. Historische Reasoning-Daten werden nur zur verlustfreien Profilübernahme weiter mitgeführt, nicht angezeigt oder bearbeitet. Alle 1.147 bestehenden .NET-Prüfungen erfolgreich (20 SQL-Integrationen ausgelassen); eigener Browser mit korrekten Fachantworten und beiden Prüfungsauswertungen geprüft. Nutzer kann nach Trainerneustart weiter testen. Kein neues ZIP.

## 11.11.2026 – Windows-ZIP Alpha 15 nach Nutzer-Push erledigt

Nutzer hat Commit und Push bestätigt und ZIP/Prüfung/kurze Release-Infos ausdrücklich beauftragt. Code-Commit 3c7339f87745350d9a4b9c2a525095f7a3147775 und Remote-master stimmen überein. Paket mit 400 Übungen/sieben Modulen und dem entfernten Zusatzfreitext gebaut. 1.147 .NET-/31 JavaScript-Prüfungen erfolgreich, 20 echte SQL-Integrationen ausgelassen. Native EXE mit eigenem Altprofil: alternative Testsätze, Reparatur, Qualitätsprüfung 50/50, Pause/Neustart, gesperrte Abgabe und alte Profilfelder erfolgreich. 59 Dateien bitgleich nach Lauf, ZIP/SHA256 und ältere Archive unverändert. Kurze Neu/Verbessert/Start-und-Update-Beschreibung an den tatsächlichen vorhandenen GitHub-Releases ausgerichtet. Versionshinweise nach dem Code-Commit ergänzt, Prüfdokumentation separat. Veröffentlichung als Pre-release v0.1.1-alpha.15 auf 3c7339f durch Nutzer. Kein automatischer Upload/Tag/Commit/Push oder Beginn von Abschnitt 07.

## 11.11.2026 – Abschnitt 07 nach Nutzer-GO umgesetzt

Der Nutzer hat Alpha 15 veröffentlicht und das vorgestellte Konzept für Projekte und Wirtschaftlichkeit ausdrücklich freigegeben. 54 eigene Lernaufgaben in neun Kapiteln, eigene Struktur-/Netz-/Terminpläne, Kosten-/Wirtschaftlichkeitsrechnungen, Nutzwerttabellen und Projektsteuerung umgesetzt. Drei neue Prüfungsfälle mit 45 Minuten und 50 Trainingspunkten, gewohnter Lernrahmen und eigenes Profil. Keine Zusatznotizen/Reflexionsfelder. SQL/Pseudocode und bisherige fachliche Kataloge bleiben unverändert. Damit sind alle acht Themenkarten nutzbar; keine vollständige AP2-Abdeckung behauptet.

1.190 .NET-/31 JavaScript-Prüfungen erfolgreich; 20 persönliche SQL-Integrationen bewusst ausgelassen. Eigene Browserprüfung auf 5246: eigener Strukturplan, Netzplanzeiten, alternative Ressourcenfolge, Rechenmatrix, Hilfen, Prüfung 50/50 und Pause/Neustart. README mit acht gleich großen Modulbildern, Fachvertrag und Pflichtdokumente nachgeführt. Nutzer testet anschließend ausführlicher; Zeitkalibrierung bleibt offen. Kein ZIP, Commit/Push oder Upload.

## 11.11.2026 – Checkboxdarstellung Abschnitt 07 berichtigt

Auf Nutzerrückmeldung durchlaufende Auswahltexte in eigenständige, voll anklickbare Auswahlkarten umgestaltet. Feste Checkboxgröße, Textumbruch, Abstand und sichtbarer Tastaturfokus gelten für Auswahl- und Reviewaufgaben. Warnungsfreier Build, 17 Komponentenprüfungen und isolierter Browser auf 5248 (Desktop dunkel/Handy hell) erfolgreich. README geprüft, Darstellung der acht Module unverändert passend. Keine Inhalts-/Profilrevision und kein ZIP; Nutzer testet weiter.

## 11.11.2026 – Alpha-16-ZIP nach Nutzer-Push erledigt

Sauberer HEAD und direkt abgefragter origin/master 1dbcb12 bestätigt. Anwendungscode unverändert gepackt, kurze Versionshinweise im Stil der tatsächlichen öffentlichen Alpha15-/Alpha14-Beschreibungen ergänzt. 454 Übungen, alle acht Module, neue Projektprüfungen und korrigierte Auswahlkarten. 1.190 .NET-/31 JavaScript-Prüfungen erfolgreich, 20 SQL-Integrationen ausgelassen. Native EXE aus dem ZIP: Pläne/alternative Reihenfolge, Prüfung 50/50, Pause/Neustart und alle Altprofilfelder erhalten. 62 Dateien nach Lauf bitgleich. ZIP/SHA256 vorhanden, alte Archive unverändert; Nutzer veröffentlicht selbst mit Tag v0.1.1-alpha.16 auf 1dbcb12. Kein automatischer Tag/Upload/Commit/Push, keine weiteren Inhalte aus diesem Paketauftrag ableiten.

## Abschlusscheck nach Alpha 16

AB-01 bis AB-04 in der Quellfassung behoben und nachgeprüft. Einzelbefunde und Messergebnisse stehen im [Testprotokoll](testprotokoll.md).

Die beiden Befunde aus Schritt 3 (AB-05 und AB-06) sind in der Quellfassung behoben. Die Nachprüfung ist im [Testprotokoll](testprotokoll.md) dokumentiert.
