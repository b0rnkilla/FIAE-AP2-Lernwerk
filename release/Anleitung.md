# AP2 Lernwerk starten

## Herunterladen und entpacken

1. Die [GitHub-Releases](https://github.com/b0rnkilla/FIAE-AP2-Lernwerk/releases) öffnen und die aktuelle Version wählen.
2. Unter **Assets** das Paket **AP2-Lernwerk-…-win-x64.zip** herunterladen. Die **Source code**-Archive sind nicht die fertige Anwendung.
3. Das vollständige ZIP in einen eigenen Ordner entpacken.
4. **AP2-Lernwerk.exe** starten. Alle mitgelieferten Dateien und Ordner daneben müssen erhalten bleiben.
5. Der Browser öffnet sich automatisch. Falls nicht, **http://localhost:5080** aufrufen.

Die Anwendung benötigt Windows auf einem 64-Bit-Rechner und einen aktuellen Browser. Die .NET-Laufzeit ist enthalten; Visual Studio und ein SDK sind nicht nötig. Während der Nutzung das Programmfenster geöffnet lassen. Zum Beenden darin **Strg+C** drücken oder das Fenster schließen. Das Schließen des Browsers allein beendet die Anwendung nicht.

## SQL Server einrichten

Für die SQL-Lernwege muss Microsoft SQL Server separat installiert sein, beispielsweise Express oder LocalDB. **SSMS allein ist kein Datenbankserver.** Bei Bedarf hilft der [offizielle Microsoft-Download](https://www.microsoft.com/de-de/sql-server/sql-server-downloads). Pseudocode, UML, Datenbankentwurf, Softwareplanung, Qualitätssicherung und WiSo lassen sich auch ohne SQL Server bearbeiten.

Im Fußbereich **Updates & SQL-Einrichtung** öffnen, die eigene Instanz auswählen oder deren Namen eintragen und **Verbindung prüfen & speichern** wählen. Die Anwendung verwendet deine Windows-Anmeldung und legt ausschließlich ihre eigene Datenbank **FIAE_AP2_Training** an. Dafür benötigt die Anmeldung passende SQL-Rechte. Eine gleichnamige fremde Datenbank wird nicht übernommen. Eine fehlgeschlagene Verbindung bedeutet nicht automatisch, dass SQL Server fehlt.

## Lernstand und Updates

Über die Übersicht zwischen SQL, Pseudocode & Algorithmen, UML & Modellierung, Datenbankentwurf, Softwareplanung & OOP, Tests/Qualität/Sicherheit und Wirtschafts- & Sozialkunde wählen. Die Kopfnavigation beginnt immer mit Übersicht und Lernplan; die folgenden Links passen zum gewählten Lernbereich. Neue SQL- und Pseudocode-Aufgaben starten im Modus **Ohne Hilfe lösen**. **Geführt lernen** ergänzt Erklärungen, Hinweise, Spickzettel und Musterlösung; bei Pseudocode zusätzlich eine schrittweise Ablaufansicht. Pseudocode-Beispiele verwenden **=** für Zuweisungen und **==** für Gleichheitsvergleiche.

Es gibt 30 SQL-Ermittlungsaufgaben, zehn SQL-Werkstattaufgaben, 30 Pseudocode- und 150 WiSo-Aufgaben, zusammen mit 42 Datenbankentwurfs-, 36 UML-, 48 Softwareplanungs- und 54 Qualitätsaufgaben insgesamt 400. Der Pseudocode-Lernweg reicht von Bedingungen und Schleifen über Listen, Suche und Sortierung bis zu Funktionen und Rekursion. Schlüsselwörter wie **WENN**, **DANN** und **AUSGABE** müssen großgeschrieben sein. Im nummerierten Codefeld rückt **Tab** ein, **Umschalt+Tab** aus. **Esc, danach Tab** verlässt das Codefeld; reine Ausgabe- und Testfallantworten behalten die normale Tab-Navigation. Alle Spickzettel besitzen einen Rückweg zum Lernen.

WiSo bietet Themenlernen, gemischte Zehnerrunden, Fehlerwiederholung und eine Prüfungsrunde mit 30 Aufgaben und 60 Minuten. **Prüfung üben** in Übersicht oder Kopfnavigation öffnet zuerst die Infos; erst **Prüfung starten** legt eine neue Runde an. Offene Runden lassen sich fortsetzen und werden nur nach zusätzlicher Bestätigung ersetzt. Im Training zeigt **Antwort prüfen** die Erklärung und sperrt die Antwort bis zur nächsten Runde. In der Prüfung erscheinen Lösungen erst nach Abgabe. Die Uhr läuft beim Seitenwechsel und Schließen weiter; **Pause** hält sie an. Nachschlagen im Spickzettel pausiert die Prüfung und kennzeichnet die Runde entsprechend. Teilpunkte und Prozentwert folgen eigenen Trainingsregeln, keiner amtlichen IHK-Note. Antworten und Markierungen bleiben beim Fortsetzen erhalten; Ergebnisfelder speichern beim Verlassen oder bei der nächsten Aktion.

Lernstände und persönliche Einstellungen liegen unter **%LOCALAPPDATA%\AP2-Lernwerk**, außerhalb des Programmordners. Die App verwendet ein lokales Profil für deine Browser. Unter **Lernplan** lässt sich dieses Profil zurücksetzen; die Datenbank bleibt dabei erhalten.

Die abschaltbare Updateprüfung liest öffentliche GitHub-Releases und zeigt neue passende Windows-Versionen an. Für ein Update die App beenden, das neue vollständige ZIP in einen neuen Ordner entpacken und die neue EXE starten. Es findet kein automatischer Dateiaustausch statt. Der Lernstand bleibt im Benutzerordner. Solange das Repository oder der Release privat ist, kann die anonyme Prüfung ihn nicht erkennen.

Die App zeigt nur **höhere** Versionsnummern als Update an. Zum Testen muss daher beispielsweise eine laufende 0.1.1-alpha.4 auf ein veröffentlichtes Paket 0.1.1-alpha.7 treffen. Derselbe Release wird nicht als neues Update gemeldet.

## SQL- und Pseudocode-Prüfungen

Über die Übersicht oder die jeweilige Modulnavigation **Prüfung** öffnen. SQL bietet drei Fälle mit je fünf Aufgaben; Pseudocode kombiniert drei Aufgaben aus einem eigenen Pool mit zehn Aufgaben. Jede Runde hat **45 Minuten und 50 Trainingspunkte**. Diese 25 Prüfungsaufgaben ergänzen die 400 Lernübungen und verändern deren Abschlusszähler nicht.

Die Uhr startet erst mit der Runde und läuft beim Seitenwechsel oder Schließen weiter. **Pause** hält sie an und blendet die Aufgaben aus. Der Spickzettel pausiert ebenfalls und kennzeichnet die Runde als mit Nachschlagen bearbeitet. Entwürfe und Kontrollmarkierungen werden gespeichert. Erst nach der endgültigen Abgabe erscheinen Bewertung und Lösungen; danach sind Antworten gesperrt. Bei technischen SQL-Problemen bleibt die Abgabe unbewertet erhalten und kann nach Beheben der Verbindung erneut bewertet werden. Die eigenen Trainingspunkte sind keine amtliche IHK-Note.

## Wenn etwas nicht funktioniert

- **Port 5080 belegt:** Möglicherweise läuft das Lernwerk bereits. Die Browseradresse öffnen oder die alte Instanz zuerst beenden.
- **Verbindung unterbrochen:** Prüfen, ob das Programm noch läuft. Der Dialog bietet Wiederverbinden beziehungsweise Neuladen an. Gespeicherte Lernstände bleiben erhalten; noch ungesicherte Eingaben können beim Neuladen verloren gehen.
- **SQL-Verbindung fehlt:** Instanzname, laufenden SQL-Dienst und Rechte der Windows-Anmeldung prüfen; anschließend erneut verbinden.

## Nutzung

Über die Fußzeile erreichst du **Über das Lernwerk**, die freiwillige **Unterstützung** und **Fehler oder Vorschlag melden**. Projektinhaber ist **Christopher Weismantel · b0rnkilla**. GitHub und PayPal öffnen in einem neuen Tab. Einen GitHub-Stern vergibst du dort über **Star**; Fehlermeldungen und Vorschläge werden als öffentliche GitHub-Issues angelegt. Diese Aktionen verändern deinen Lernstand nicht.

Es gelten die privaten Nutzungsbedingungen in **LICENSE**. **DRITTANBIETER.md** und **Lizenzen** enthalten eigenständige Hinweise zu Bibliotheken und Laufzeiten. Deren Lizenztexte sind keine zusätzliche Freigabe des Lernwerks.

## Datenbankentwurf

Abschnitt 04 enthält 42 Aufgaben zu Tabellen, Schlüsseln, Beziehungen und Normalisierung. Lege Tabellen und Attribute im Baukasten an, markiere PK und wähle vollständige FK-Referenzen. Gleiche UQ-Gruppen bezeichnen gemeinsame zusätzliche eindeutige Schlüssel. Tabellen- und Anzeigenamen sind frei; die Fachattribute aus dem Auftrag behalten ihre Bedeutung. Das automatisch gezeichnete Modell lässt sich vergrößern und bei Bedarf seitlich verschieben.

Ohne Hilfe lösen ist der Standard; Geführt lernen ergänzt drei Hinweise, Spickzettel und Musterlösung. Modus und Entwurf werden je Aufgabe gespeichert. Drei eigene Prüfungsfälle mit je drei Teilaufträgen verwenden 45 Minuten und 50 Trainingspunkte. Start, Pause, Nachschlagen und Abgabe funktionieren wie in den anderen Prüfungsbereichen. Die regelbasierte Bewertung folgt den ausdrücklich beschriebenen Modellannahmen und ist keine amtliche IHK-Korrektur. Vorhandene Lernstände bleiben erhalten.

## Auf Handy, Tablet oder Zweitrechner lernen

Im Fußbereich **Heimnetz** öffnen und **Im Heimnetz freigeben** wählen. Windows benötigt beim ersten Einrichten gegebenenfalls eine Administratorbestätigung für die Firewall. Deine App läuft anschließend weiterhin ohne Administratorrechte. Das Windows-Netzwerk muss als privat eingestuft sein; die Einstellungsseite hilft beim Prüfen.

Nach dem kurzen Neustart der App zeigt die Seite eine kopierbare Adresse und einen QR-Code. Öffne diese Adresse auf deinem anderen Gerät im selben Heimnetz; dort genügt der Browser. Der Gastgeber muss eingeschaltet bleiben und das Lernwerk geöffnet lassen. SQL Server wird nur dort benötigt. Die Firewall-Freigabe gilt für das lokale Subnetz, ohne einzelne Geräteadressen einzutragen. Gast-WLAN oder eine Geräteisolierung im Router können den Zugriff trotzdem verhindern.

**Alle Geräte teilen deinen Lernstand und dieselben Prüfungsrunden.** Zum Beenden der Freigabe auf dem Gastgeber `http://localhost:5080/heimnetz` öffnen und **Freigabe beenden** wählen. Ein anderer Port wird entsprechend angezeigt. Vor dem Wechsel offene Eingaben verlassen; gespeicherte Daten bleiben erhalten, laufende Prüfungsuhren laufen weiter.

Die Betriebsart bleibt gespeichert. Wird ein Update in einen neuen Ordner entpackt, kann die Seite wegen des geänderten Programmpfads **Freigabe reparieren** anbieten. Es ist weder eine zweite EXE noch eine SQL-Portfreigabe auf anderen Geräten nötig.

## UML und Modellierung

Abschnitt 03 enthält 36 eigene Aufgaben zum Kulturzentrum Nordhafen. Lies, repariere und erstelle Anwendungsfall-, Klassen-, Aktivitäts-, Sequenz- und Zustandsdiagramme. Eigene Entwürfe beginnen leer: Füge Elemente hinzu, wähle ihre Fachrollen und UML-Arten, ergänze Merkmale und verbinde die Endpunkte. Anzeigenamen sind frei; die App ordnet das Diagramm automatisch an. Früher/Später verschiebt Sequenznachrichten. Vergrößerung und Textdarstellung ergänzen die Zeichenfläche.

Ohne Hilfe lösen und Geführt lernen funktionieren wie in den anderen Lernmodulen. Drei Hinweise und ein UML-Spickzettel unterstützen auf Wunsch. Im Lernen erscheinen Rückmeldungen ohne Punkte. Drei neue Prüfungsfälle mit je drei unabhängigen Modellaufträgen bieten 45 Minuten und 50 Trainingspunkte. Erst die endgültige Abgabe zeigt Bewertung und Musterlösungen. Die Prüfung vergleicht ausdrücklich beschriebene Fachrollen und geprüfte Varianten, keine beliebigen freien UML-Entwürfe. Lernfortschritte und Prüfungsstände bleiben von den anderen Modulen getrennt.

## Softwareplanung und OOP

Abschnitt 05 bietet 48 Aufgaben in acht Kapiteln. Präzisiere Anforderungen, analysiere Objektzustände und plane Klassen, Muster, Architekturen und Schnittstellen. Schreibe JSON-/XML-Nachrichten und erstelle eigene Formularprototypen. Fachrollen behalten ihre Bedeutung; Anzeigenamen sind frei. Im Formularbaukasten ordnen **Früher/Später** die Felder. Die Vorschau lässt einfache Pflichtangaben ausprobieren, ohne echte Buchung oder Datenaustausch.

Ohne Hilfe lösen und Geführt lernen funktionieren wie in den anderen Entwurfsbereichen. Drei neue Prüfungsfälle verwenden je drei unabhängige Teilaufgaben, 45 Minuten und 50 Trainingspunkte. Pause, Nachschlagen, Kontrollmarkierung und gesperrte Abgabe bleiben erhalten. Die Aufgaben prüfen ausdrücklich vereinbarte Varianten, keine beliebige Softwarearchitektur und keine amtliche IHK-Korrektur.

## Tests, Qualität und Sicherheit (Abschnitt 06)

54 eigene Aufgaben begleiten Nordhafen in den Probebetrieb. Entwickle Testfälle mit Eingaben und erwarteten Ergebnissen, markiere Reviewbefunde, ergänze eine Rechte-Matrix und stelle Abläufe zusammen. In der Testwerkstatt sind andere geeignete Testwerte als in der Musterlösung erlaubt; die verlangten Grenzen und Kriterien stehen im Auftrag. Jeder Fall startet unabhängig. Erst **Lösung prüfen** zeigt berechnete Ergebnisse, ausgeführte Anweisungen/Zweige/Pfade und erkannte Fehlervarianten. Ein falscher Sollwert zählt nicht als gefundener Programmfehler. Abdeckungsprozente sind keine Note.

**Geführt lernen** bietet drei Hinweise, Qualitäts-Spickzettel und Musterlösung. Drei eigene Prüfungsfälle haben je drei unabhängige Teile, 45 Minuten und 50 Trainingspunkte; Pause, Markierungen, Nachschlagen und endgültige Abgabe funktionieren wie in den anderen Modulen. In Prüfungen erscheinen Ergebnisse erst nach Abgabe. Das Modul benötigt keinen SQL Server und führt weder fremden Code noch echte Angriffe aus.
