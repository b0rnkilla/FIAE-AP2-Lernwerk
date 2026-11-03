# AP2 Lernwerk starten

## Herunterladen und entpacken

1. Die [GitHub-Releases](https://github.com/b0rnkilla/FIAE-AP2-Lernwerk/releases) öffnen und die aktuelle Version wählen.
2. Unter **Assets** das Paket **AP2-Lernwerk-…-win-x64.zip** herunterladen. Die **Source code**-Archive sind nicht die fertige Anwendung.
3. Das vollständige ZIP in einen eigenen Ordner entpacken.
4. **AP2-Lernwerk.exe** starten. Alle mitgelieferten Dateien und Ordner daneben müssen erhalten bleiben.
5. Der Browser öffnet sich automatisch. Falls nicht, **http://localhost:5080** aufrufen.

Die Anwendung benötigt Windows auf einem 64-Bit-Rechner und einen aktuellen Browser. Die .NET-Laufzeit ist enthalten; Visual Studio und ein SDK sind nicht nötig. Während der Nutzung das Programmfenster geöffnet lassen. Zum Beenden darin **Strg+C** drücken oder das Fenster schließen. Das Schließen des Browsers allein beendet die Anwendung nicht.

## SQL Server einrichten

Microsoft SQL Server muss separat installiert sein, beispielsweise Express oder LocalDB. **SSMS allein ist kein Datenbankserver.** Bei Bedarf hilft der [offizielle Microsoft-Download](https://www.microsoft.com/de-de/sql-server/sql-server-downloads).

Im Fußbereich **Updates & SQL-Einrichtung** öffnen, die eigene Instanz auswählen oder deren Namen eintragen und **Verbindung prüfen & speichern** wählen. Die Anwendung verwendet deine Windows-Anmeldung und legt ausschließlich ihre eigene Datenbank **FIAE_AP2_Training** an. Dafür benötigt die Anmeldung passende SQL-Rechte. Eine gleichnamige fremde Datenbank wird nicht übernommen. Eine fehlgeschlagene Verbindung bedeutet nicht automatisch, dass SQL Server fehlt.

## Lernstand und Updates

Lernstände und persönliche Einstellungen liegen unter **%LOCALAPPDATA%\AP2-Lernwerk**, außerhalb des Programmordners. Die App verwendet ein lokales Profil für deine Browser. Unter **Lernplan** lässt sich dieses Profil zurücksetzen; die Datenbank bleibt dabei erhalten.

Die abschaltbare Updateprüfung liest öffentliche GitHub-Releases und zeigt neue passende Windows-Versionen an. Für ein Update die App beenden, das neue vollständige ZIP in einen neuen Ordner entpacken und die neue EXE starten. Es findet kein automatischer Dateiaustausch statt. Der Lernstand bleibt im Benutzerordner. Solange das Repository oder der Release privat ist, kann die anonyme Prüfung ihn nicht erkennen.

Die App zeigt nur **höhere** Versionsnummern als Update an. Zum Testen muss daher beispielsweise eine laufende 0.1.1-alpha.3 auf ein veröffentlichtes Paket 0.1.1-alpha.4 treffen. Derselbe Release wird nicht als neues Update gemeldet.

## Wenn etwas nicht funktioniert

- **Port 5080 belegt:** Möglicherweise läuft das Lernwerk bereits. Die Browseradresse öffnen oder die alte Instanz zuerst beenden.
- **Verbindung unterbrochen:** Prüfen, ob das Programm noch läuft. Der Dialog bietet Wiederverbinden beziehungsweise Neuladen an. Gespeicherte Lernstände bleiben erhalten; noch ungesicherte Eingaben können beim Neuladen verloren gehen.
- **SQL-Verbindung fehlt:** Instanzname, laufenden SQL-Dienst und Rechte der Windows-Anmeldung prüfen; anschließend erneut verbinden.

## Nutzung

Es gelten die privaten Nutzungsbedingungen in **LICENSE**. **DRITTANBIETER.md** und **Lizenzen** enthalten eigenständige Hinweise zu Bibliotheken und Laufzeiten. Deren Lizenztexte sind keine zusätzliche Freigabe des Lernwerks.
