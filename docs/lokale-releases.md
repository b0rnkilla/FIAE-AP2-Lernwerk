# AP2 Lernwerk lokal starten

**Aktuelle Entwicklung: 0.1.1-alpha.8 mit 150 WiSo-Aufgaben, insgesamt 220 Übungen. Dafür wurde noch kein Windows-Paket erstellt.** Die folgende Paketbeschreibung betrifft das unveränderte Alpha-7-ZIP ohne WiSo. `release/Anleitung.md` ist bereits die Vorlage für einen später ausdrücklich beauftragten neuen Paketbau; sie verändert kein vorhandenes ZIP.

Die README ist inzwischen freigegeben. `release/Versionshinweise-0.1.1-alpha.8.md` ist vor dem Nutzer-Commit vorbereitet; ZIP-Bau und Paketprüfung folgen erst nach Commit/Push. Noch kein Alpha-8-Paket erzeugt.

Stand: Windows-Alpha 0.1.1-alpha.7, gebaut und aus dem ZIP geprüft am 04.11.2026. Das Paket enthält die aktuelle private Projektlizenz. Die alte Testfassung 0.1.0 ist kein Verteilungspaket für die aktuelle Fassung und soll nicht verwendet werden.

Alpha 7 enthält Pseudocode-Zeilennummern, Tab-Einrückung, große Schlüsselwörter und alle 30 Algorithmus-Aufgaben mit Suche, Sortierung, verschachtelten Listen, Funktionen, Prüfziffern, Grenztests und Rekursion. Zusammen mit den SQL-Lernwegen stehen 70 Übungen bereit. SQL-Spickzettel und Werkstatt besitzen den erhaltenen Rückweg zur Aufgabe.

Datei: `artifacts/releases/AP2-Lernwerk-0.1.1-alpha.7-win-x64.zip`, 50.740.598 Bytes (48,39 MiB), plus `.zip.sha256`. SHA256: `57c1aee1c6a791fd938290c9c767aaed11569058c2b0efb4b4b4d20c3b5e0fab`. Das ZIP wurde nach der Prüfung nicht verändert. Die EXE startet mit mitgelieferter Laufzeit; Anleitung, Versionshinweise, Projektlizenz und Drittanbieterhinweise sind enthalten. Private Einstellungen und Lernstände sind ausgeschlossen.

GitHub: Tag `v0.1.1-alpha.7`, Titel `AP2 Lernwerk 0.1.1-alpha.7`, als **Pre-release** kennzeichnen. ZIP und SHA256-Datei unter Assets hochladen. Gebaut wurde auf ausdrücklichen Nutzerwunsch aus der noch uncommitteten Arbeitsfassung. Der alte Commit `b590044` ist deshalb kein passendes Release-Ziel; Änderungen zuerst committen und pushen und den Tag auf diesen neuen Commit setzen. Es wurde nichts automatisch veröffentlicht. Ältere Alpha-Fassungen können das höhere öffentliche Release nach fertigem Asset-Upload erkennen; dieser Live-Schritt ist vor Veröffentlichung nicht geprüft.

## Frühere Pakete

Alpha 4 ergänzt die erhaltene Listenposition, den überarbeiteten geführten Hilfebereich, die modulabhängige Kopfnavigation und den ersten Pseudocode-Lernweg mit acht Aufgaben. Die Pseudocode-Aufgaben benötigen keine SQL-Verbindung. Das bestehende ältere ZIP 0.1.1-alpha.3 bleibt unverändert und enthält nur die SQL-Lernwege.

Nach dem vom Nutzer durchgeführten Commit und Push wurde das Paket aus `b5900441d3bb8269f0df9ed0b1d1e4fdf94f6547` erstellt. Anleitung und Versionshinweise sind enthalten. Lokal liegen `artifacts/releases/AP2-Lernwerk-0.1.1-alpha.4-win-x64.zip` (50.701.318 Bytes) und die zugehörige `.zip.sha256` vor. SHA256: `09909501a0d6a7679b6930998db60bc6462e1d8c72c0fdff3076c9620bbcc5fc`. Das Paket ist vorbereitet, nicht automatisch veröffentlicht.

GitHub-Angaben: Tag `v0.1.1-alpha.4`, Titel `Alpha 0.1.1-alpha.4`, Zielcommit `b590044`, Kennzeichnung als Vorabversion. ZIP und SHA256-Datei als Assets hochladen. Ein öffentliches veröffentlichtes Alpha-4-Release mit vollständig hochgeladenem Windows-ZIP kann von Alpha 3 als Update erkannt werden; derselbe Versionsstand ist kein neues Update.

## Voraussetzungen

* Windows auf einem x64-Rechner und ein aktueller Browser.
* Für die SQL-Lernwege eine erreichbare Microsoft-SQL-Server-Instanz, beispielsweise SQL Server Express. SSMS allein enthält keinen Datenbankserver. Standard ist die lokale Instanz .\SQLEXPRESS mit Windows-Anmeldung. Pseudocode kann ohne SQL Server bearbeitet werden.
* Beim ersten Start benötigt deine Windows-Anmeldung das Recht, die eigene Datenbank FIAE_AP2_Training anzulegen. Andere Datenbanken werden nicht befüllt oder verändert. Eine gleichnamige fremde Datenbank ohne Trainer-Kennzeichnung wird abgewiesen.

Das Windows-Paket enthält die benötigte .NET- und ASP.NET-Core-Laufzeit. Ein SDK, Visual Studio oder eine separate .NET-Installation sind für dieses Paket nicht erforderlich. SQL Server ist nur für SQL separat erforderlich und wird vom Lernwerk weder installiert noch aktualisiert. Das technische Feld `RequiresSqlServer` in `release-info.json` bezieht sich auf die SQL-Funktionen; es sperrt den Algorithmus-Lernweg nicht. [Microsoft: eigenständige Single-File-Veröffentlichung](https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/), [SQL Server herunterladen](https://www.microsoft.com/sql-server/sql-server-downloads).

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
.\Release-erstellen.ps1 -Version 0.1.1-alpha.4
```

Ohne Versionsparameter verwendet das Skript die zentrale Version aus Directory.Build.props. Während der laufenden Entwicklung wird es nicht automatisch ausgeführt. Dafür sind das im Projekt festgelegte .NET-SDK und beim ersten Wiederherstellen der Windows-Laufzeitpakete Internetzugang erforderlich. Der Ablauf erstellt ein neues Buildverzeichnis und schreibt ZIP sowie SHA256-Datei nach artifacts/releases. Vorhandene gleich nummerierte ZIP-Dateien werden nicht überschrieben. Private Konfiguration, Fortschrittsdateien, QA-Profile und Protokolle werden vor dem Verpacken abgeprüft.

Zum öffentlichen GitHub-Release gehören das geprüfte ZIP, seine Prüfsumme und passende Versionshinweise. Das Skript lädt nichts hoch. Eine GitHub-Automatisierung ist noch nicht eingerichtet. Für die aktuelle Projektfassung gelten die Bedingungen zur privaten Nutzung in LICENSE. LICENSE wird mitgeliefert; eine MIT-Freigabe des Projekts ist nicht vorgesehen. Drittanbieterlizenzen bleiben unabhängig davon wirksam; siehe [Private Nutzung](lizenz.md). DRITTANBIETER.md und der Ordner Lizenzen dokumentieren die aufgelösten NuGet-Pakete und enthalten die vorhandenen Originalhinweise. Die jeweiligen Drittanbieterlizenzen gelten unabhängig von der Projektlizenz.

Das erste Paket ist nicht mit einem Code-Signing-Zertifikat signiert. Ein Test auf einem frisch eingerichteten fremden Windows-Rechner bleibt vor dem öffentlichen Release sinnvoll; lokale Tests ersetzen diese Prüfung nicht.

## Öffentliche Updatequelle

Die konfigurierte Quelle ist https://github.com/b0rnkilla/FIAE-AP2-Lernwerk. Ist das Repository oder ein Release privat, kann die anonyme Prüfung es nicht erkennen. Die Prüfung verwendet keine Zugangsschlüssel und liest maximal 100 öffentliche Releases. Sie benötigt einen gültigen SemVer-Tag und einen fertig hochgeladenen Anhang *-win-x64.zip. Alpha-Fassungen berücksichtigen Vorabversionen; stabile Fassungen bleiben beim stabilen Kanal. Abfragen haben ein kurzes Timeout und werden bei Wiederholung innerhalb einer Minute zusammengefasst. Fehler werden von „keine neuere Version“ unterschieden. Unter Updates & SQL-Einrichtung kann die automatische Startprüfung ausgeschaltet werden.
