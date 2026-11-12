# AP2 Lernwerk lokal starten

## Erste Beta für Windows

Das Windows-x64-Paket **1.0.0-beta.1** wurde am 11.11.2026 mit PowerShell 7 aus dem sauberen Commit `ab00d0344870aa35c283229e23d242a8f22651d1` gebaut. GitHub-`master` und die Produktversion der entpackten EXE bestätigen diesen Commit. Die Anleitung und die Versionshinweise stammen ebenfalls daraus.

`AP2-Lernwerk-1.0.0-beta.1-win-x64.zip` enthält **62 Dateien**, darunter die Windows-Laufzeit und die Drittanbieterlizenzen. Größe **51.705.155 Bytes**, SHA256 `862e1a08d78f18a171c572f64eb7952cde5e394877b80f2b35c67f43ba18a7be`. Der Paketwechsel von Alpha 16 und die Starts mit und ohne SQL wurden mit einem separaten Lernprofil geprüft. [Testprotokoll](testprotokoll.md).

Für GitHub den Tag `v1.0.0-beta.1` auf den Paketcommit setzen, als Titel **AP2 Lernwerk 1.0.0-beta.1** verwenden und **Pre-release** wählen. ZIP und zugehörige `.zip.sha256` als Assets anhängen. [Versionshinweise zur ersten Beta](../release/Versionshinweise-1.0.0-beta.1.md).

## Historisches Paket Alpha 16

**Windows-Paket 0.1.1-alpha.16: am 11.11.2026 nach ausdrücklich bestätigtem Nutzer-Push gebaut und direkt aus dem ZIP geprüft.** Sauberer Code-Commit `1dbcb124859cf9dd31cda7296b02d8608bd3a2dc`, identisch mit direkt abgefragtem origin/master. Anwendungscode und Anleitung aus diesem Commit; kurze Versionshinweise anschließend ergänzt. Nachträgliche Prüfdokumentation separat committen.

**454 Lernübungen in allen acht Modulen**, 25 SQL-/Pseudocode-Prüfungsaufgaben und je drei Datenbankentwurfs-, UML-, Softwareplanungs-, Qualitäts- und Projektprüfungsfälle. Neu: Abschnitt 07 mit 54 Aufgaben, eigenen Struktur-/Netz-/Terminplänen, Wirtschaftlichkeitsrechnungen und getrennten anklickbaren Auswahlkarten.

Datei: `artifacts/releases/AP2-Lernwerk-0.1.1-alpha.16-win-x64.zip`, **51.705.122 Bytes (49,31 MiB)**, **62 Dateien**; zusätzlich `.zip.sha256`. SHA256: `7eec3a4f9d3404ad629795587065614d51fd1eb0f87aaa64b59bd71ceecc267e`. Eigenständig startbare Windows-x64-Fassung mit enthaltener Laufzeit. Keine privaten Einstellungen/Lernstände, QA-Dateien, Originalprüfungen oder Debugsymbole. Anleitung, Projektlizenz, Drittanbieter-/Runtime-/QRCoder-Lizenzen und Windows-Helfer mit UTF-8-BOM vollständig.

Native Prüfung auf 5250 mit eigenem synthetischem Altprofil: Auswahl-/Tastaturbedienung, eigener Strukturplan, alternative Ressourcenfolge, Netzplan und Museumsprüfung 50/50. Pause/EXE-Neustart bei 2684 Sekunden, Kontrollmarkierung und Antwortsperre erhalten. Alle alten Modulfelder einschließlich unsichtbarer Reasoning-Werte bewahrt. Alle 62 Paketdateien nach Lauf bitgleich; ältere Archive/Prüfsummen unverändert. Nachweis artifacts/alpha16-verification.json. Keine neue erhöhte Firewall-Einrichtung, physisches Zweitgerät oder persönliche SQL-Integration.

GitHub: Tag **v0.1.1-alpha.16**, Titel **AP2 Lernwerk 0.1.1-alpha.16**, Zielcommit **1dbcb124859cf9dd31cda7296b02d8608bd3a2dc**, **Pre-release**. ZIP und SHA256 anhängen. Kurze Beschreibung: [Versionshinweise Alpha 16](../release/Versionshinweise-0.1.1-alpha.16.md). Nutzer veröffentlicht selbst; kein automatischer Tag/Upload/Commit/Push. Geprüftes ZIP/SHA256 und ältere Pakete nicht ersetzen.

## Historisches Paket Alpha 15

**Windows-Paket 0.1.1-alpha.15: am 11.11.2026 nach ausdrücklich bestätigtem Nutzer-Push gebaut und direkt aus dem ZIP geprüft.** Anwendungscode aus sauberem gepushtem Commit `3c7339f87745350d9a4b9c2a525095f7a3147775`, identisch mit direkt abgefragtem origin/master. Anleitung aus diesem Commit; kurze Versionshinweise anschließend ergänzt, Anwendungscode unverändert. Nachträgliche Prüfdokumentation separat committen.

Enthalten: **400 Lernübungen in sieben Modulen**, 25 SQL-/Pseudocode-Prüfungsaufgaben und je drei Datenbankentwurfs-, UML-, Softwareplanungs- und Qualitätsprüfungsfälle mit neun Teilen je Modul. Neu: Abschnitt 06 mit 54 Aufgaben, eigener Testwerkstatt, Reviews, Matrix und Abläufen. Zusätzliche Begründungsfelder und Selbstkontrollbereiche in 05/06 entfernt.

Datei: `artifacts/releases/AP2-Lernwerk-0.1.1-alpha.15-win-x64.zip`, **51.622.353 Bytes (49,23 MiB)**, 59 Paketdateien, zusätzlich `.zip.sha256`. SHA256: `92f1f08e2b0b5cd6df8c31cba7704fc58655801ba7e6912a4950183c641245fa`. Windows-x64-Laufzeit enthalten, keine Entwicklungswerkzeuge zum Start nötig. Keine privaten Einstellungen/Lernstände, QA-Dateien, Originalprüfungen oder Debugsymbole. Projektlizenz, Drittanbieter-/Runtime-/QRCoder-Lizenzen vollständig; Windows-Helfer UTF-8 mit BOM.

Native Paketprüfung auf Port 5244 ohne SQL mit eigenem Altprofil: Testsätze, Sollwertfehler, Reparatur 33, entfernte Zusatzfelder, Qualitätsprüfung 50/50, Pause/Hostneustart, Markierung und Antwortsperre erfolgreich. Alte Modulfelder und historische unsichtbare Notiz erhalten. Alle 59 Dateien nach Lauf unverändert, ältere Archive unverändert; Nachweis artifacts/alpha15-verification.json. Keine neue erhöhte Firewall-Einrichtung, kein physisches Zweitgerät/frischer fremder Rechner und keine neue echte SQL-Integration.

GitHub: Tag **v0.1.1-alpha.15**, Titel **AP2 Lernwerk 0.1.1-alpha.15**, Zielcommit **3c7339f87745350d9a4b9c2a525095f7a3147775**, **Pre-release**. ZIP und SHA256 anhängen. Kurze Beschreibung: [Versionshinweise Alpha 15](../release/Versionshinweise-0.1.1-alpha.15.md). Nutzer veröffentlicht selbst; kein automatischer Tag/Upload/Commit/Push. ZIP/SHA256 und ältere Pakete nicht ersetzen.

## Historisches Paket Alpha 14

**Windows-Paket 0.1.1-alpha.14: am 11.11.2026 nach ausdrücklich bestätigtem Nutzer-Push gebaut und aus dem ZIP geprüft.** Sauberer HEAD und direkt abgefragter Remote-master `33f09a7f896839962c9a29f0a9167d24ae9aa181`. Code, Anleitung und Versionshinweise aus diesem Commit; nachträgliche Prüfdokumentation separat. Enthalten: **346 Lernübungen in sechs Modulen**, 25 SQL-/Pseudocode-Prüfungsaufgaben sowie je drei Datenbankentwurfs-, UML- und Softwareplanungs-Prüfungsfälle mit neun Teilen je Modul. Abschnitt 05: 48 Aufgaben in acht Kapiteln und eigene Architektur-/API-/Formularentwürfe.

Datei: `artifacts/releases/AP2-Lernwerk-0.1.1-alpha.14-win-x64.zip`, **51.548.922 Bytes (49,16 MiB)**, 59 Paketdateien, zusätzlich `.zip.sha256`. SHA256: `34f0a09a7de8d667960ca52c80b525f553a7295b20dff02294cc0d00d8be1ef8`. Eigenständige win-x64-Laufzeit; keine privaten Einstellungen/Profile, Originalprüfungen, QA-Dateien oder Debugsymbole. Projektlizenz, eigenständige Drittanbieter-/Runtime-/QRCoder-Lizenzen und Windows-PowerShell-BOM-Helfer vollständig.

GitHub: Tag **v0.1.1-alpha.14**, Titel **AP2 Lernwerk 0.1.1-alpha.14**, Zielcommit **33f09a7f896839962c9a29f0a9167d24ae9aa181**, **Pre-release**. ZIP und SHA256 anhängen. Beschreibung: [Versionshinweise Alpha 14](../release/Versionshinweise-0.1.1-alpha.14.md). Veröffentlichung als Alpha-14-Pre-release vom Nutzer bestätigt. Kein automatischer Upload/Commit/Push. Das geprüfte ZIP nicht ersetzen.

1.090 .NET- und 31 JavaScript-Prüfungen erfolgreich, 20 echte SQL-Integrationen ausgelassen. Native EXE auf Port 5236 mit eigenem temporärem Profil und unerreichbarem SQL-Ziel: API32, XML35 und Formular39 erfolgreich; eigene Beschriftung erhalten, Vorschau benennt fehlende Pflichtangaben. Prüfungsfall Fahrradverleih vollständig per Oberfläche beantwortet, bei 44:39/Teil2/Markierung pausiert, EXE neu gestartet und unverändert fortgesetzt; Abgabe **50/50**, Antworten gesperrt, Historie einmal. Alle alten synthetischen Modulfelder und 59 Paketdateien nach Lauf unverändert. Nachweis artifacts/alpha14-verification.json. Kein kompletter 45-Minuten-Endnutzerlauf, neues physisches Gerät oder erneute erhöhte Firewall-Einrichtung Teil dieses Tests.

## Historisches Paket Alpha 13

**Windows-Paket 0.1.1-alpha.13: am 09.11.2026 ausdrücklich beauftragt, gebaut und aus dem ZIP geprüft.** Anwendungscode aus dem sauberen und remote bestätigten Commit `b4224b0b87f3f66a742e4b25bdbb9dacf7aeb9d5`. Die Versionshinweise wurden nach dem Commit ergänzt; Anwendungscode und Version blieben unverändert. Enthalten: **298 Lernübungen in fünf Modulen**, 25 SQL-/Pseudocode-Prüfungsaufgaben, drei Datenbankentwurfs- und drei UML-Prüfungsfälle mit jeweils neun Teilaufgaben je Modul. UML mit 36 Aufgaben, Datenbankentwurf mit 42 Aufgaben einschließlich Gestaltung und Storynachlauf.

Datei: `artifacts/releases/AP2-Lernwerk-0.1.1-alpha.13-win-x64.zip`, **51.467.925 Bytes (49,08 MiB)**, 59 Paketdateien, zusätzlich `.zip.sha256`. SHA256: `dd8dcee9301b697a04727f25f0a416666d52d6068a75af26936811fe2609df33`. Laufzeit für Windows x64 enthalten. Keine privaten Einstellungen, Lernstände, Originalprüfungen, QA-Dateien oder Debugsymbole. Projektlizenz und unabhängige Drittanbieterhinweise einschließlich QRCoder vollständig. Alle 59 Dateien blieben nach dem Pakettest bitgleich; Alpha-12-Archiv samt Prüfsumme unverändert.

GitHub: Tag **v0.1.1-alpha.13**, Titel **AP2 Lernwerk 0.1.1-alpha.13**, Zielcommit **b4224b0**, als **Pre-release** kennzeichnen. ZIP und SHA256-Datei anhängen. Beschreibung: [Versionshinweise Alpha 13](../release/Versionshinweise-0.1.1-alpha.13.md). Der Nutzer hat [Alpha 13](https://github.com/b0rnkilla/FIAE-AP2-Lernwerk/releases/tag/v0.1.1-alpha.13) am 09.11.2026 als Pre-release veröffentlicht; über die öffentliche GitHub-API am 11.11.2026 bestätigt. Kein Tag/Upload durch die KI. Nachträgliche Prüfdokumentation separat committen, das geprüfte ZIP nicht ersetzen.

1.041 .NET-Tests bestanden, 20 SQL-Integrationsprüfungen bewusst ausgelassen; 31 JavaScript-Tests bestanden. Native EXE direkt aus dem ZIP auf Port 5230 mit separatem synthetischem Profil außerhalb des Programmordners: alte Modulstände erhalten, UML-Auswahl und selbst aufgebautes Diagramm sowie ER-Ergänzung 31 ohne SQL erfolgreich, Pause/Markierung/Teilaufgabe nach Hostneustart erhalten, UML-Prüfung 50/50 und anschließend gesperrt. Prüfungsreferenzen ausschließlich im QA-Profil vorbereitet. Native Windows-Netzwerkdiagnose erfolgreich, keine Firewall-Regel verändert. Nachweis: artifacts/alpha13-verification.json. Kein neuer 45-Minuten-Endnutzerlauf, kein frischer fremder Windows-Rechner und keine neue erhöhte Firewall-Regel Teil dieses Pakettests.

## Historisches Paket Alpha 12

**Windows-Paket 0.1.1-alpha.12: am 06.11.2026 gebaut und aus dem ZIP geprüft.** Sauberer Commit `e3af6aa95c12d25df3f5fe36d80417e3506fb2ad`, identisch mit dem bestätigten GitHub-master. Anwendungscode, Anleitung und Versionshinweise stammen aus diesem Commit. Enthalten: 250 Lernübungen, 25 separate SQL-/Pseudocode-Prüfungsaufgaben sowie drei Entwurfsprüfungen mit neun Teilaufgaben; Heimnetz-Einstellung direkt in derselben EXE.

Datei: `artifacts/releases/AP2-Lernwerk-0.1.1-alpha.12-win-x64.zip`, **51.363.008 Bytes (48,98 MiB)**, 59 Paketdateien, zusätzlich `.zip.sha256`. SHA256: `bc660d417e9279ec7fa22eadea9f9b87efbc4b6147618faaf519fb50ec914402`. Windows x64, mitgelieferte Laufzeit, private Projektlizenz und eigenständige Drittanbieterhinweise einschließlich QRCoder. Keine privaten Einstellungen, Lernstände, QA-/Originalprüfungsdateien oder Debugsymbole. ZIP und SHA256 unverändert erhalten.

GitHub: Tag **v0.1.1-alpha.12**, Titel **AP2 Lernwerk 0.1.1-alpha.12**, Zielcommit **e3af6aa**, **Pre-release**. ZIP und SHA256-Datei als Assets anhängen. Veröffentlichung übernimmt der Nutzer; noch kein Upload/Tag durch die KI. Releasebeschreibung: release/Versionshinweise-0.1.1-alpha.12.md.

866 .NET-Tests einschließlich SQL und 31 JavaScript-Prüfungen erfolgreich. Entpackte native EXE auf eigenem Port 5214 und eigenem Profil geprüft: Auswahlaufgabe und freies Tabellenmodell ohne SQL jeweils 50/50, neue 45-Minuten-Prüfung mit gespeicherter Pause/Kontrollmarkierung, echter SQL-JOIN, nativer Firewall-Diagnosehelfer und lokal erzeugter QR-Code, Remoteansicht ohne Verwaltung sowie Rückwechsel zu localhost mit internem Neustart. Vorherige Modulstände und Prüfungsverläufe erhalten; alle Paketdateien nach Lauf unverändert. Nachweis artifacts/alpha12-verification.json. Neue erhöhte Firewall-Regelerstellung und ein frisch eingerichteter fremder Windows-Rechner waren nicht Teil des Pakettests; der Nutzer hat den Heimnetzbetrieb zuvor bestätigt.

## Historisches Paket Alpha 9

**Windows-Paket 0.1.1-alpha.9: gebaut und aus dem entpackten ZIP geprüft am 05.11.2026.** Anwendungscode aus dem sauberen, gepushten Commit `a6c929f846cf3d0957e15db4a74ace47ac947991`. Die Paketdokumente ANLEITUNG und VERSIONSHINWEISE wurden nach dem Commit ergänzt; am Anwendungscode wurde für den Paketbau nichts geändert. Enthalten sind 220 Lernübungen und 25 zusätzliche Prüfungsaufgaben, neue SQL-/Pseudocode-Prüfungsrunden und der WiSo-Feinschliff. Veröffentlichung führt der Nutzer selbst aus.

Datei: `artifacts/releases/AP2-Lernwerk-0.1.1-alpha.9-win-x64.zip`, **50.946.094 Bytes (48,59 MiB)**, 50 Dateien, plus `.zip.sha256`. SHA256: `2056930755bf556418fbd227f2fe664185ac7b54c8a95e365d29be3cf058be4f`. Keine privaten Einstellungen, Lernstände, QA-Dateien, Originalprüfungen oder Debugsymbole. Eigene Projektlizenz und Drittanbieterhinweise enthalten. ZIP nach Prüfung unverändert erhalten.

GitHub: Tag `v0.1.1-alpha.9`, Titel `AP2 Lernwerk 0.1.1-alpha.9`, Zielcommit `a6c929f`, **Pre-release**. ZIP und SHA256-Datei als Assets hochladen. Der Upload und die Live-Erkennung dieses neuen Releases sind noch nicht erfolgt; der Nutzer hat die Update-Erkennung früherer Veröffentlichungen bereits bestätigt.

## Historisches Paket Alpha 8

**Windows-Paket 0.1.1-alpha.8: gebaut und aus dem entpackten ZIP geprüft am 05.11.2026**, aus dem sauberen Commit `b4e953fbb628b9c39ce695538db2bf1343f13bcf`. Der Nutzer hat den Commit gepusht; der Remote-Stand wurde bestätigt. Enthalten sind 220 Übungen: 40 SQL-, 30 Algorithmus- und 150 WiSo-Aufgaben sowie der neue Projektbereich. Veröffentlichung und Upload erfolgen durch den Nutzer.

Datei: `artifacts/releases/AP2-Lernwerk-0.1.1-alpha.8-win-x64.zip`, **50.839.413 Bytes (48,48 MiB)**, plus `.zip.sha256`. SHA256: `cdd77864a8dfecf5c7b535ac662d574ca8762d670e2889e7ed18745ea7cc145c`. Selbstständig startbare Windows-x64-Fassung mit Laufzeit, Anleitung, Versionshinweisen und Lizenzen; ohne private Konfiguration, Lernstände, Protokolle oder Debugsymbole.

GitHub: Tag `v0.1.1-alpha.8`, Titel `AP2 Lernwerk 0.1.1-alpha.8`, Zielcommit `b4e953f`, als **Pre-release** kennzeichnen. ZIP und SHA256-Datei als Assets hochladen. Die neue öffentliche Version kann erst nach Veröffentlichung mit vollständig vorhandenem Windows-Asset als Update erkannt werden; dieser künftige Live-Schritt ist noch nicht geprüft.

Der Nutzer hat die Veröffentlichung von Alpha 8 bestätigt. Der neue **Quellstand Alpha 9** verbessert WiSo-Leseführung und Urheberlogo; dafür wurde kein neues Paket erstellt.

## Historisches Paket Alpha 7

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
* Für die SQL-Lernwege eine erreichbare Microsoft-SQL-Server-Instanz, beispielsweise SQL Server Express. SSMS allein enthält keinen Datenbankserver. Standard ist die lokale Instanz .\SQLEXPRESS mit Windows-Anmeldung. Pseudocode und WiSo können ohne SQL Server bearbeitet werden.
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

Der normale Aufruf in PowerShell 7 erstellt die GitHub-ZIP mit unsignierter EXE. Das a²-Symbol ist bereits beim Kompilieren eingebettet.

```powershell
.\Release-erstellen.ps1
```

Für eine signierte Ausgabe zusätzlich `signtool.exe` aus dem Windows SDK im Suchpfad bereitstellen. Das Codesignaturzertifikat des Herausgebers muss in `Cert:\CurrentUser\My` mit zugänglichem privaten Schlüssel vorhanden sein. Für den RFC-3161-Zeitstempel die URL des Zertifikatsanbieters verwenden.

```powershell
.\Release-erstellen.ps1 -Sign -SigningThumbprint 'FINGERABDRUCK_DES_ZERTIFIKATS' -TimestampServer 'URL_DES_ZEITSTEMPELDIENSTES'
```

Die Signierung wird durch `-Sign` oder einen ausdrücklich übergebenen Signierparameter aktiviert. Erst dann liest das Skript fehlende Angaben aus `AP2_SIGNING_THUMBPRINT` und `AP2_TIMESTAMP_SERVER`. Signatur und Zeitstempel verwenden jeweils SHA-256. Vor dem Erstellen einer signierten ZIP prüft das Skript die Vertrauenskette, den Fingerabdruck und den Zeitstempel. Ein Fehler beendet den angeforderten Signierlauf.

Ohne Versionsparameter verwendet das Skript die zentrale Version aus Directory.Build.props. Während der laufenden Entwicklung wird es nicht automatisch ausgeführt. Dafür sind das im Projekt festgelegte .NET-SDK und beim ersten Wiederherstellen der Windows-Laufzeitpakete Internetzugang erforderlich. Der Ablauf erstellt ein neues Buildverzeichnis und schreibt ZIP sowie SHA256-Datei nach artifacts/releases. Vorhandene gleich nummerierte ZIP-Dateien werden nicht überschrieben. Private Konfiguration, Fortschrittsdateien, QA-Profile und Protokolle werden vor dem Verpacken abgeprüft.

Zum öffentlichen GitHub-Release gehören das geprüfte ZIP, seine Prüfsumme und passende Versionshinweise. Das Skript lädt nichts hoch. Eine GitHub-Automatisierung ist noch nicht eingerichtet. Für die aktuelle Projektfassung gelten die Bedingungen zur privaten Nutzung in LICENSE. LICENSE wird mitgeliefert; eine MIT-Freigabe des Projekts ist nicht vorgesehen. Drittanbieterlizenzen bleiben unabhängig davon wirksam; siehe [Private Nutzung](lizenz.md). DRITTANBIETER.md und der Ordner Lizenzen dokumentieren die aufgelösten NuGet-Pakete und enthalten die vorhandenen Originalhinweise. Die jeweiligen Drittanbieterlizenzen gelten unabhängig von der Projektlizenz.

Die veröffentlichte Beta 1 trägt noch keine Herausgebersignatur. Bei zukünftigen signierten Downloads kann Windows den geprüften Herausgeber anzeigen. SmartScreen bewertet zusätzlich die Reputation der Datei und des Herausgebers. Deshalb kann auch beim ersten Start einer gültig signierten neuen Version ein Hinweis erscheinen. [Microsoft zu SmartScreen](https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/smartscreen-reputation).

## Öffentliche Updatequelle

Die konfigurierte Quelle ist https://github.com/b0rnkilla/FIAE-AP2-Lernwerk. Ist das Repository oder ein Release privat, kann die anonyme Prüfung es nicht erkennen. Die Prüfung verwendet keine Zugangsschlüssel und liest maximal 100 öffentliche Releases. Sie benötigt einen gültigen SemVer-Tag und einen fertig hochgeladenen Anhang *-win-x64.zip. Alpha-Fassungen berücksichtigen Vorabversionen; stabile Fassungen bleiben beim stabilen Kanal. Abfragen haben ein kurzes Timeout und werden bei Wiederholung innerhalb einer Minute zusammengefasst. Fehler werden von „keine neuere Version“ unterschieden. Unter Updates & SQL-Einrichtung kann die automatische Startprüfung ausgeschaltet werden.

## Prüfungsumfang in Alpha 9

SQL-/Pseudocode-Prüfungen sind im neuen Alpha-9-Paket enthalten und aus dem ZIP geprüft: 25 zusätzliche Prüfungsaufgaben, getrennt von den bisherigen 220 Lernübungen. Das veröffentlichte Alpha-8-Paket bleibt unverändert. Für weitere Pakete gelten weiterhin Nutzerauftrag, eigener Profiltest und passende Versionshinweise. [Funktionsstand und Grenzen](pruefungsmodus.md).

## Quellstand Alpha 10, noch kein Paket

Seit 06.11.2026 vereinheitlicht 0.1.1-alpha.10 die Übersichtsbuttons und Prüfungsinfos. WiSo besitzt einen eigenen Einstieg /wiso/pruefung und startet erst durch Prüfung starten. Quellbuild und isolierte Browserprüfung erfolgreich, kein neues ZIP beauftragt oder erstellt. Das geprüfte Alpha-9-Paket und dessen SHA256 bleiben unverändert; dessen Veröffentlichung wurde in diesem Änderungspaket nicht überprüft.

## Quellstand Alpha 11, kein neues Paket

Datenbankentwurf ist mit 30 Lernaufgaben, Baukasten und drei Prüfungsfällen hinzugekommen. Die aktuelle Quellfassung enthält 250 Lernübungen; das bisherige geprüfte Alpha-9-Paket bleibt bei seinem damaligen Umfang. Ein neuer Paketbau wurde nicht beauftragt und nicht durchgeführt. Die bisherige Alpha-9-ZIP-Prüfsumme wurde unverändert bestätigt. README und Quellanleitung beschreiben auf Nutzerwunsch die aktuelle App-Fassung; Paketstand und Prüfung bleiben hier getrennt nachvollziehbar.

## Quellstand Alpha 12: Heimnetz-Freigabe in derselben EXE

Die neue Seite /heimnetz richtet die optionale Freigabe ein und beendet sie mit einem internen Hostneustart. Kein zweites Programm und keine Entwicklungswerkzeuge nötig. Der mitveröffentlichte Helfer Networking/Heimnetz-Firewall.ps1 benötigt nur für das Ändern seiner eigenen Windows-Regel UAC. Programmpfad und Port sind Teil der Regel; neue Updateordner können eine ausdrücklich angebotene Reparatur benötigen. Der QR-Code entsteht lokal mit QRCoder 1.8.1. Abhängigkeits- und Lizenzhinweise werden wie bisher aus dem NuGet-Bestand erstellt.

Dieses Änderungspaket ist ausschließlich gebaut und geprüft; **kein neues ZIP**, kein Commit/Push/Upload. Alpha-9-Archiv und SHA256 bleiben unverändert. Es enthält weiterhin 220 Lernübungen; die aktuelle Quellfassung besitzt 250. Weitere Details: [Heimnetz](heimnetz-und-serverbetrieb.md).

## Beauftragt: Windows-ZIP Alpha 12 nach Nutzer-Push

Der Nutzer hat Paketbau und Release-Informationen für **0.1.1-alpha.12** ausdrücklich nach seinem nächsten Commit und Push beauftragt. Versionshinweise unter release/Versionshinweise-0.1.1-alpha.12.md vorbereitet. Enthalten sein sollen 250 Lernübungen, die bisherigen SQL-/Pseudocode-Prüfungen, drei Entwurfsprüfungen und die Heimnetz-Einstellung. Heimnetz vom Nutzer bestätigt, Abschnitt 04 noch nicht selbst erprobt. Zielcommit, ZIP/SHA256 und Paketprüfung werden erst nach dem Push bestimmt. Bisherige Archive bleiben unverändert; Upload/Veröffentlichung übernimmt der Nutzer.

## Quellfassung Alpha 13 und anschließender Paketauftrag

Seit 09.11.2026 ergänzt die ausdrücklich freigegebene Quellfassung Abschnitt 03 UML und Modellierung: 36 Lernaufgaben sowie drei weitere Prüfungsfälle. Zusammen mit der Entwurfserweiterung auf 42 Aufgaben sind 298 Lernübungen in fünf Modulen verfügbar. Build und Browserprüfung des Quellprojekts sind kein portables Release. Alpha-12-ZIP und Prüfsumme bleiben unverändert bei 250 Lernübungen. Anschließend wurde Alpha 13 ausdrücklich als ZIP beauftragt und am 09.11.2026 aus dem gepushten Commit b4224b0 gebaut und geprüft; Details oben. Veröffentlichung durch den Nutzer steht noch aus.

## Quellfassung Alpha 14 – Paket nach Nutzer-Push beauftragt

Abschnitt 05 Softwareplanung und OOP ergänzt 48 Lernübungen und drei eigene Prüfungsfälle; insgesamt 346 Übungen in sechs Modulen. Version zentral in Directory.Build.props auf 0.1.1-alpha.14. Nur Quellprojekt/Testausgabe gebaut und mit eigenem Profil geprüft. Das obige Alpha-13-ZIP samt SHA256 bleibt unverändert bei 298 Übungen. Änderungen an der Anleitung im Quellprojekt verändern keine bereits geprüften Archive. Am 11.11.2026 hat der Nutzer das Windows-ZIP und Release-Informationen ausdrücklich **nach seinem Commit und Push** beauftragt. Erste Nutzererprobung von Abschnitt 05 positiv, ausführliche Prüfung/Zeitkalibrierung weiter offen. Versionshinweise unter [Alpha 14](../release/Versionshinweise-0.1.1-alpha.14.md) vorbereitet. Zielcommit, ZIP/SHA256 und native Paketprüfung erst nach dem Push bestimmen. Bestehende Archive erhalten; Veröffentlichung führt der Nutzer selbst aus.

## Abschluss Alpha-14-Paketauftrag

Nach dem oben protokollierten Auftrag hat der Nutzer Commit und Push bestätigt. Paketbau und native Prüfung am 11.11.2026 erfolgreich abgeschlossen; endgültige Datei-, Commit- und Release-Angaben stehen im aktuellen Paketabschnitt oben. Ältere Beauftragt-/Noch-nicht-gebaut-Vermerke bleiben als Historie erhalten.

## Quellfassung Alpha 15 – Abschnitt 06, ausdrücklich kein ZIP

Nach Konzept und ausdrücklichem Nutzer-GO ergänzt Abschnitt 06 Tests, Qualität und Sicherheit 54 eigene Lernaufgaben und drei eigene Prüfungsfälle. Zentralversion 0.1.1-alpha.15; 400 Übungen in sieben Modulen. Ausschließlich Quellprojekt bauen und mit getrenntem Profil testen. Der Nutzer testet anschließend. Kein neues ZIP, kein Upload/Commit/Push und kein Beginn von Abschnitt 07. Das veröffentlichte Alpha-14-Paket aus 33f09a7 mit 346 Übungen, dessen SHA256 und alle älteren Archive bleiben unverändert.

## Quellfassung Alpha 16 – kein Paketauftrag

Der Nutzer bestätigt Alpha 15 als hochgeladen. Abschnitt 07 Projekte und Wirtschaftlichkeit ist nach Konzept-GO in der Quellfassung 0.1.1-alpha.16 ergänzt: 454 Übungen, acht aktive Module. Ausschließlich Quellprojekt und getrennte Testausgaben bauen. Noch kein Windows-ZIP, kein Upload/Tag/Commit/Push. Das geprüfte Alpha-15-ZIP mit 51.622.353 Bytes und SHA256 `92f1f08e2b0b5cd6df8c31cba7704fc58655801ba7e6912a4950183c641245fa` sowie ältere Archive unverändert erhalten. Weitere Pakete nur auf ausdrücklichen Auftrag.

## Startverhalten der Quellfassung nach Alpha 16

Ist der konfigurierte Port beim Start belegt, nennt das Lernwerk die Browseradresse und empfiehlt, eine bereits laufende Instanz für einen Neustart zuerst zu beenden. Der zweite Prozess endet mit Exitcode 1. Im interaktiven Release-Fenster bestätigt der Nutzer die Meldung mit der Eingabetaste. Die Vorlage der Paketanleitung nennt alle acht Lernbereiche.
