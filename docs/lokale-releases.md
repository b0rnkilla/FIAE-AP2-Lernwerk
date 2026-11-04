# AP2 Lernwerk lokal starten

Stand: Windows-Alpha 0.1.1-alpha.3. Auf ausdrücklichen Nutzerwunsch ist ein neues eigenständig startbares ZIP mit aktueller privater Projektlizenz gebaut und geprüft. Die alte Testfassung 0.1.0 ist kein Verteilungspaket für die aktuelle Fassung und soll nicht verwendet werden.

Die anschließende Quellprojektfassung 0.1.1-alpha.4 ergänzt die erhaltene Listenposition, den überarbeiteten geführten Hilfebereich, die modulabhängige Kopfnavigation und den ersten Pseudocode-Lernweg mit acht Aufgaben. Die Pseudocode-Aufgaben dieser Entwicklungsfassung benötigen keine SQL-Verbindung. Dafür wurde noch kein neues Windows-Paket erstellt; das bestehende ZIP 0.1.1-alpha.3 bleibt unverändert und enthält weiterhin nur die SQL-Lernwege.

Für das nächste Paket sind die angepasste `release/Anleitung.md` und `release/Versionshinweise-0.1.1-alpha.4.md` vorbereitet. Die Paketierung ist ausdrücklich erst nach dem vom Nutzer durchgeführten Commit und Push beauftragt. Bis dahin bleiben dies vorbereitete Unterlagen, kein Nachweis eines gebauten oder veröffentlichten Alpha-4-Pakets.

## Voraussetzungen

* Windows auf einem x64-Rechner und ein aktueller Browser.
* Eine erreichbare Microsoft-SQL-Server-Instanz, beispielsweise SQL Server Express. SSMS allein enthält keinen Datenbankserver. Standard ist die lokale Instanz .\SQLEXPRESS mit Windows-Anmeldung.
* Beim ersten Start benötigt deine Windows-Anmeldung das Recht, die eigene Datenbank FIAE_AP2_Training anzulegen. Andere Datenbanken werden nicht befüllt oder verändert. Eine gleichnamige fremde Datenbank ohne Trainer-Kennzeichnung wird abgewiesen.

Das Windows-Paket enthält die benötigte .NET- und ASP.NET-Core-Laufzeit. Ein SDK, Visual Studio oder eine separate .NET-Installation sind für dieses Paket nicht erforderlich. SQL Server ist separat erforderlich und wird vom Lernwerk weder installiert noch aktualisiert. [Microsoft: eigenständige Single-File-Veröffentlichung](https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/), [SQL Server herunterladen](https://www.microsoft.com/sql-server/sql-server-downloads).

## Starten und beenden

1. Das Windows-ZIP unter [GitHub-Releases](https://github.com/b0rnkilla/FIAE-AP2-Lernwerk/releases) herunterladen und vollständig in einen eigenen Ordner entpacken. Die automatisch erzeugten Source-code-Archive sind nicht das fertige Programm.
2. AP2-Lernwerk.exe starten. Die Dateien und der Ordner wwwroot daneben gehören zum Programm und müssen erhalten bleiben.
3. Der Browser öffnet http://localhost:5080 automatisch. Falls das nicht gelingt, öffne die Adresse selbst.
4. Zum Beenden das Programmfenster schließen oder darin Strg+C drücken. Das Schließen des Browserfensters allein beendet die Anwendung nicht.

Die Standardfassung ist nur auf dem eigenen Rechner erreichbar. Sie benötigt weder Konto noch Registrierung und lädt keine Lösungen zu einem KI-Dienst hoch. Sie prüft öffentliche GitHub-Releases und zeigt bei neuerer passender Version einen Hinweis. Es gibt keinen automatischen Dateiaustausch.

Läuft schon ein Trainer auf Port 5080, beende ihn vor dem Start einer neuen Fassung. Alternativ lässt sich aus einem Terminal ein anderer Port wählen:

```powershell
.\AP2-Lernwerk.exe --urls http://localhost:5082
```

## Unterbrochene Verbindung

Der deutschsprachige Dialog zeigt, ob die App eine Verbindung sucht, ein erneuter Versuch nötig ist oder die Sitzung nicht mehr verfügbar ist. **Erneut verbinden** beziehungsweise **Weiterlernen** versucht die bestehende Sitzung fortzusetzen. **Seite neu laden** beginnt eine neue Verbindung. Gespeicherte Lernstände bleiben erhalten; noch nicht gespeicherte Eingaben können beim Neuladen verloren gehen. Auch dieser Dialog folgt System/Hell/Dunkel.

## Wo bleibt mein Lernstand?

Die EXE verwendet pro Windows-Benutzer diesen Ordner:

```text
%LOCALAPPDATA%\AP2-Lernwerk\progress\progress.json
```

Entwürfe, Aufgabenmodi, gelöste Aufgaben und Anzeigeeinstellungen bleiben damit erhalten, wenn du eine neue Version in einen anderen Programmordner entpackst. Zum Sichern die Anwendung zuerst beenden und den Ordner %LOCALAPPDATA%\AP2-Lernwerk kopieren. Der Lernstand ist lokal und kein GitHub-Bestandteil.

Die im Browser gespeicherte Designwahl gehört zum Browserprofil; sie ist unabhängig vom gespeicherten Aufgabenfortschritt.

## Andere SQL-Server-Instanz verwenden

In der aktuellen Entwicklungsfassung öffnest du **Updates & SQL-Einrichtung** über den Fußbereich. Wähle einen registrierten lokalen Server oder gib einen eigenen Namen ein. **Verbindung prüfen & speichern** verwendet Windows-Anmeldung, bereitet die eigene Datenbank vor und speichert den Namen für weitere Starts. Ein fehlgeschlagener Versuch ersetzt keine bisher funktionierende Verbindung. Die registrierten Instanzen sind Hinweise, keine sichere Installations- oder Erreichbarkeitsdiagnose. Ein Microsoft-Downloadlink hilft bei tatsächlich fehlender Installation. Während einer Heimnetz-Vorführung sind Änderungen serverseitig gesperrt.

Die frühere Momentaufnahme und erweiterte SQL-Anmeldungen lassen sich weiterhin über die Datei konfigurieren:

1. Die Anwendung beenden.
2. Im Explorer %LOCALAPPDATA%\AP2-Lernwerk öffnen; den Ordner bei Bedarf selbst anlegen.
3. Die mitgelieferte Verbindung.example.json dorthin kopieren und in appsettings.Local.json umbenennen.
4. In dieser Datei ausschließlich die gewünschte Verbindung anpassen, etwa Server=MEIN-PC\\MEINE-INSTANZ innerhalb des JSON-Textes. Der Datenbankname bleibt FIAE_AP2_Training.
5. Die Anwendung erneut starten. Danach lässt sich die Verbindung auf der Übersicht prüfen.

Die mitgelieferte Beispielverbindung verwendet Windows-Anmeldung. TrustServerCertificate=True ist für eine lokale Entwicklungsinstanz mit selbst signiertem Zertifikat vorgesehen. Persönliche Verbindungen oder Zugangsdaten gehören weder in den öffentlichen Programmordner noch ins GitHub-Repository.

Eine vorhandene LocalDB-Instanz kann als Verbindung konfiguriert werden, ist aber ebenfalls eine separate Installation. Für das erste Paket wurden echte SQL-Express-Verbindungen geprüft. In der anschließenden Entwicklungsfassung wurde die vorhandene LocalDB über die Einrichtungsseite gewählt und mit dem vollständigen SQL-Integrationstestbestand geprüft. [Microsoft: SQL Server Express LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb).

## Von der bisherigen Quellprojekt-Version wechseln

Die bisherige Entwicklungsfassung speichert im Projekt unter src/FiaeTrainer.Web/Data/progress. Sie wird durch dieses Paket nicht automatisch verändert. Zur Übernahme beide Fassungen beenden und die bisherige progress.json in den oben beschriebenen Release-Profilordner kopieren. Einen dort schon vorhandenen Lernstand vorher separat sichern. Es gibt keine automatische Zusammenführung verschiedener Lernstände.

## Neue Version installieren

Die bisherige Anwendung beenden, das neue vollständige ZIP in einen neuen Ordner entpacken und die neue EXE starten. Lernstand und persönliche Verbindung bleiben im Benutzerordner. Neue Trainingsregeln können verlangen, einen alten Entwurf vor der nächsten Abgabe zu korrigieren. Die Datenbank wird bei einem normalen Versionswechsel nicht zurückgesetzt.

## Für die Veröffentlichung vorbereiten

Aus dem Quellprojekt lässt sich ein nummeriertes Windows-Paket erstellen:

```powershell
.\Release-erstellen.ps1 -Version 0.1.1-alpha.3
```

Ohne Versionsparameter verwendet das Skript die zentrale Version aus Directory.Build.props. Während der laufenden Entwicklung wird es nicht automatisch ausgeführt. Dafür sind das im Projekt festgelegte .NET-SDK und beim ersten Wiederherstellen der Windows-Laufzeitpakete Internetzugang erforderlich. Der Ablauf erstellt ein neues Buildverzeichnis und schreibt ZIP sowie SHA256-Datei nach artifacts/releases. Vorhandene gleich nummerierte ZIP-Dateien werden nicht überschrieben. Private Konfiguration, Fortschrittsdateien, QA-Profile und Protokolle werden vor dem Verpacken abgeprüft.

Zum öffentlichen GitHub-Release gehören das geprüfte ZIP, seine Prüfsumme und passende Versionshinweise. Das Skript lädt nichts hoch. Eine GitHub-Automatisierung ist noch nicht eingerichtet. Für die aktuelle Projektfassung gelten die Bedingungen zur privaten Nutzung in LICENSE. LICENSE wird mitgeliefert; eine MIT-Freigabe des Projekts ist nicht vorgesehen. Drittanbieterlizenzen bleiben unabhängig davon wirksam; siehe [Private Nutzung](lizenz.md). DRITTANBIETER.md und der Ordner Lizenzen dokumentieren die aufgelösten NuGet-Pakete und enthalten die vorhandenen Originalhinweise. Die jeweiligen Drittanbieterlizenzen gelten unabhängig von der Projektlizenz.

Das erste Paket ist nicht mit einem Code-Signing-Zertifikat signiert. Ein Test auf einem frisch eingerichteten fremden Windows-Rechner bleibt vor dem öffentlichen Release sinnvoll; lokale Tests ersetzen diese Prüfung nicht.

## Öffentliche Updatequelle

Das Repository https://github.com/b0rnkilla/FIAE-AP2-Lernwerk ist derzeit privat. Ohne öffentliche Releases ist noch keine anonyme Updatequelle verfügbar. Die vorbereitete Prüfung benötigt später öffentlich erreichbare Releases und verwendet keine Zugangsschlüssel. Die Prüfung liest maximal 100 öffentliche Releases ohne Authentifizierung, benötigt einen gültigen SemVer-Tag und einen fertig hochgeladenen Anhang *-win-x64.zip. Alpha-Fassungen berücksichtigen Vorabversionen; stabile Fassungen bleiben beim stabilen Kanal. Abfragen haben ein kurzes Timeout und werden bei Wiederholung innerhalb einer Minute zusammengefasst. Fehler werden von „keine neuere Version“ unterschieden. Unter Updates & SQL-Einrichtung kann die automatische Startprüfung ausgeschaltet werden.
