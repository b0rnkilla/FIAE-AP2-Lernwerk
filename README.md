# AP2 Lernwerk

Eine deutschsprachige Lernapp für die FIAE-Abschlussprüfung Teil 2. Du schreibst eigene Lösungen und erhältst direkte Rückmeldung. Verfügbar sind **SQL & Datenbanken** sowie der erste Lernweg für **Pseudocode & Algorithmen**.

**Windows-Alpha: 0.1.1-alpha.7.** Die Anwendung läuft lokal und wird im Browser bedient. Das Windows-Paket ist gebaut und aus dem ZIP geprüft; die Bereitstellung über GitHub-Releases erfolgt durch den Projektinhaber.

Die Version enthält **70 Übungen**, darunter **30 Pseudocode-Aufgaben** zu Listen, Suche, Sortierung, Funktionen, Prüfziffern, Grenzwerttests und Rekursion. Der nummerierte Editor unterstützt Tab-Einrückung; Schlüsselwörter werden großgeschrieben.

## Das kannst du bereits lernen

- **Fallakte Nachtzug:** 30 aufeinander aufbauende SQL-Aufgaben mit einer eigenen Ermittlungsgeschichte – von SELECT und Filtern über JOINs und GROUP BY bis zu Unterabfragen.
- **SQL-Werkstatt:** zehn Aufgaben zu Datenänderungen, Tabellen, einfachen Constraints und Transaktionen.
- **Leitstelle im Ausnahmezustand:** 30 Pseudocode-Aufgaben mit eigenen Abläufen, Fehlersuche, Schreibtischtests und Testfällen.
- **Zwei Lernmodi:** selbstständig lösen oder mit Erklärungen, freiwilligen Tipps und Musterlösungen lernen. Im geführten Hilfebereich kannst du auch direkt im SQL-Spickzettel nachschlagen.
- **Lernplan und Spickzettel:** offene Aufgaben fortsetzen und bereits gelöste Aufgaben wiederholen. Beide Spickzettel führen mit einem Button zurück zur Aufgabe; SQL merkt sich dabei auch Ermittlung oder Werkstatt.

SQL-Lösungen werden auf Microsoft SQL Server ausgeführt und mit zwei Datenbeständen geprüft. Die App ist noch keine vollständige AP2-Vorbereitung und enthält keine Originalprüfungsaufgaben.

Ein lokaler Interpreter bewertet Pseudocode-Lösungen und zeigt im geführten Modus die Ausführung Schritt für Schritt. Beispiele verwenden `=` für Zuweisungen. Der erweiterte Lernweg führt bis zu verschachtelten Listen, Sortierung und Funktionen mit Parametern und Rückgaben. Dafür ist keine SQL-Verbindung nötig. Übersicht und Lernplan fassen beide Themen zusammen; die weiteren Navigationspunkte wechseln mit dem Lernbereich. UML und weitere Bereiche bleiben in Vorbereitung.

## Voraussetzungen

- Windows auf einem 64-Bit-Rechner und ein aktueller Browser.
- Für die SQL-Lernwege: [Microsoft SQL Server](https://www.microsoft.com/de-de/sql-server/sql-server-downloads), beispielsweise Express oder LocalDB. SSMS allein reicht nicht.

Das ZIP enthält die benötigte .NET-Laufzeit. Visual Studio, ein SDK oder eine separate .NET-Installation sind nicht erforderlich. Pseudocode funktioniert auch ohne SQL Server. Für die lokale App sind ein Konto oder eine KI-API nicht erforderlich.

## Starten

1. Im Repository rechts auf **[Releases](https://github.com/b0rnkilla/FIAE-AP2-Lernwerk/releases)** gehen und die aktuelle Version öffnen.
2. Unter **Assets** das Windows-Paket **AP2-Lernwerk-…-win-x64.zip** herunterladen. Die automatisch angebotenen **Source code**-Archive sind nicht das fertige Programm.
3. Das vollständige ZIP in einen eigenen Ordner entpacken und darin **AP2-Lernwerk.exe** starten. Die übrigen Dateien und Ordner daneben gehören dazu.
4. Im Browser **http://localhost:5080** aufrufen, falls sich die Seite nicht automatisch öffnet.
5. Bei Bedarf über **Updates & SQL-Einrichtung** deine SQL-Instanz auswählen und die Verbindung prüfen und speichern.

Bitte die EXE erst nach dem Entpacken starten. Eine kurze **ANLEITUNG.md** liegt dem Paket bei.

Die App verwendet Windows-Anmeldung und legt ausschließlich ihre eigene Datenbank `FIAE_AP2_Training` an. Dafür benötigt die Anmeldung die entsprechenden SQL-Rechte. Eine vorhandene gleichnamige fremde Datenbank wird nicht übernommen. Zum Beenden im Programmfenster **Strg+C** drücken.

## Lernstand und Updates

Entwürfe und Erfolge werden automatisch lokal gespeichert. Unter **Lernplan** kannst du den Lernstand zurücksetzen; die SQL-Datenbank bleibt dabei erhalten. Jede lokale Installation verwendet ein gemeinsames Profil für ihre Browser.

Die Updateprüfung wertet öffentlich zugängliche Releases aus; sie verwendet keine GitHub-Zugangsdaten. Version und Updateprüfung findest du unter **Updates & SQL-Einrichtung**. Die abschaltbare Startprüfung weist auf neue Windows-Pakete hin. Dateien werden nicht automatisch heruntergeladen oder ersetzt. Private Releases kann die anonyme Prüfung nicht erkennen.

## Dokumentation

- [Entwicklung und Projektstruktur](docs/entwicklung.md) – Orientierung im Code, Debuggen und Tests.
- [Lernkonzept](docs/lernkonzept.md) – Lernmodi, Bewertung und technische Grenzen.
- [Pseudocode und Algorithmen](docs/pseudocode.md) – Aufgabenübersicht und unterstützte Schreibweise.
- [Lokale Windows-Pakete](docs/lokale-releases.md) – Voraussetzungen, Profil und Versionswechsel.
- [Optionale Vorführung im Heimnetz](docs/heimnetz-und-serverbetrieb.md) – Netzwerkzugriff und Android-/Chrome-Berechtigungen.
- [Projektsteckbrief](docs/projektsteckbrief.md) und [weitere Entwicklung](docs/verbesserungsplan.md).
- [Entwicklungsprotokoll](docs/entwicklungsprotokoll.md) und [Testprotokoll](docs/testprotokoll.md).

## Nutzung und Lizenz

Die aktuelle Fassung darf **ausschließlich privat und nichtkommerziell** genutzt werden. Änderungen am Programm und Weiterverbreitung sind ohne ausdrückliche Erlaubnis nicht gestattet. Eigene Übungslösungen, persönliche Einstellungen und Lernstände sind erlaubt. Statt Kopien weiterzugeben, verlinke bitte auf dieses Repository.

Es gelten die [Nutzungsbedingungen](LICENSE). Zwingende gesetzliche Rechte und eigenständige Drittanbieterlizenzen bleiben unberührt. Weitere [Erläuterungen zur privaten Nutzung](docs/lizenz.md).
