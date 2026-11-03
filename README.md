# AP2 Lernwerk

Eine deutschsprachige Lernapp für die FIAE-Abschlussprüfung Teil 2. Du schreibst eigene Lösungen und erhältst direkte Rückmeldung. Der erste verfügbare Themenbereich ist **SQL & Datenbanken**; weitere AP2-Themen folgen.

**Entwicklungsstand: 0.1.1-alpha.3.** Die Anwendung läuft lokal und wird im Browser bedient. Windows-Pakete sind für die spätere Veröffentlichung vorgesehen.

## Das kannst du bereits lernen

- **Fallakte Nachtzug:** 30 aufeinander aufbauende SQL-Aufgaben mit einer eigenen Ermittlungsgeschichte – von SELECT und Filtern über JOINs und GROUP BY bis zu Unterabfragen.
- **SQL-Werkstatt:** zehn Aufgaben zu Datenänderungen, Tabellen, einfachen Constraints und Transaktionen.
- **Zwei Lernmodi:** selbstständig lösen oder mit Erklärungen, freiwilligen Tipps und Musterlösungen lernen.
- **Lernplan und Spickzettel:** offene Aufgaben fortsetzen und bereits gelöste Aufgaben wiederholen.

Lösungen werden auf Microsoft SQL Server ausgeführt und mit zwei Datenbeständen geprüft. Die App ist noch keine vollständige AP2-Vorbereitung und enthält keine Originalprüfungsaufgaben. Pseudocode, UML und weitere Bereiche sind bisher Platzhalter.

## Voraussetzungen

- Windows und ein aktueller Browser.
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) zum Starten aus dem Quellprojekt.
- [Microsoft SQL Server](https://www.microsoft.com/de-de/sql-server/sql-server-downloads), beispielsweise Express oder LocalDB. SSMS allein reicht nicht.

Für die lokale App sind ein Konto oder eine KI-API nicht erforderlich. Für das erstmalige Laden der .NET-Pakete wird Internet benötigt.

## Starten

1. Das Quellprojekt herunterladen beziehungsweise mit entsprechender Berechtigung aus dem Repository klonen.
2. Im Projektordner **Trainer-starten.cmd** öffnen.
3. Im Browser **http://localhost:5080** aufrufen, falls sich die Seite nicht automatisch öffnet.
4. Bei Bedarf über **Updates & SQL-Einrichtung** deine SQL-Instanz auswählen und die Verbindung prüfen und speichern.

Alternativ im Projektordner:

```powershell
dotnet run --project src/FiaeTrainer.Web
```

Die App verwendet Windows-Anmeldung und legt ausschließlich ihre eigene Datenbank `FIAE_AP2_Training` an. Dafür benötigt die Anmeldung die entsprechenden SQL-Rechte. Eine vorhandene gleichnamige fremde Datenbank wird nicht übernommen. Zum Beenden im Programmfenster **Strg+C** drücken.

## Lernstand und Updates

Entwürfe und Erfolge werden automatisch lokal gespeichert. Unter **Lernplan** kannst du den Lernstand zurücksetzen; die SQL-Datenbank bleibt dabei erhalten. Jede lokale Installation verwendet ein gemeinsames Profil für ihre Browser.

Das GitHub-Repository ist derzeit privat. Die vorbereitete Updateprüfung kann erst öffentlich zugängliche Releases auswerten; sie verwendet keine GitHub-Zugangsdaten. Version und Updateprüfung findest du unter **Updates & SQL-Einrichtung**. Die abschaltbare Startprüfung liest öffentliche GitHub-Releases und weist auf neue Windows-Pakete hin. Dateien werden nicht automatisch heruntergeladen oder ersetzt.

## Dokumentation

- [Entwicklung und Projektstruktur](docs/entwicklung.md) – Orientierung im Code, Debuggen und Tests.
- [Lernkonzept](docs/lernkonzept.md) – Lernmodi, Bewertung und technische Grenzen.
- [Lokale Windows-Pakete](docs/lokale-releases.md) – Voraussetzungen, Profil und Versionswechsel.
- [Projektsteckbrief](docs/projektsteckbrief.md) und [weitere Entwicklung](docs/verbesserungsplan.md).
- [Entwicklungsprotokoll](docs/entwicklungsprotokoll.md) und [Testprotokoll](docs/testprotokoll.md).

## Nutzung und Lizenz

Die aktuelle Fassung darf **ausschließlich privat und nichtkommerziell** genutzt werden. Änderungen am Programm und Weiterverbreitung sind ohne ausdrückliche Erlaubnis nicht gestattet. Eigene Übungslösungen, persönliche Einstellungen und Lernstände sind erlaubt. Statt Kopien weiterzugeben, verlinke bitte auf dieses Repository.

Es gelten die [Nutzungsbedingungen](LICENSE). Zwingende gesetzliche Rechte und eigenständige Drittanbieterlizenzen bleiben unberührt. Weitere [Erläuterungen zur privaten Nutzung](docs/lizenz.md).
