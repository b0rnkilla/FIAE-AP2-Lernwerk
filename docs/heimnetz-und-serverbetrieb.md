# Optionale Vorführung im Heimnetz

Stand: 03.11.2026. Aktuelle Nutzerentscheidung: lokale Windows-Releases über GitHub. Login, Registrierung, Benutzertabellen, Homeserver und nginx sind nicht mehr geplant. Die folgenden Netzwerkangaben gelten nur für die bereits vorhandene optionale Vorführung.

## Jetzt: Vorführung im Heimnetz

1. Einen bereits laufenden Trainer beenden, falls er Port 5080 belegt.
2. Im Projektordner **Trainer-im-Netzwerk-starten.cmd** starten und das Fenster geöffnet lassen.
3. Auf diesem Rechner bleibt **http://localhost:5080** erreichbar. Auf dem anderen Rechner die im Startfenster angezeigte Heimnetzadresse öffnen.

Bei der Einrichtung lautete die WLAN-Adresse **192.168.1.21**, entsprechend **http://192.168.1.21:5080**. DHCP, ein anderes WLAN oder ein anderer Rechner können diese Adresse ändern. localhost bezeichnet auf dem zweiten Rechner diesen zweiten Rechner, nicht den Trainer-Rechner.

Der Trainer-Rechner muss eingeschaltet, mit dem Netzwerk verbunden und wach bleiben. Eine Teams-Bildschirmfreigabe funktioniert unabhängig davon; Teilnehmer außerhalb des Heimnetzes können die private Adresse nicht allein über Teams öffnen.

**Alle Besucher teilen sich momentan denselben Lernstand**, einschließlich Entwürfen, Abschlüssen, aufgabenspezifischer Moduswahl, Anzeige der allgemeinen Bedienhilfe und Zurücksetzen. Der Heimnetz-Modus zeigt diesen Umstand auf jeder Seite. Die SQL-Ausführungen der Werkstatt bleiben trotzdem in eigenen Sitzungen isoliert. Für unabhängige Fortschritte verwendet jeder seine eigene lokale Kopie; Onlinekonten werden nicht ergänzt.

## Technische Änderungen

* Das zusätzliche Startprofil **FiaeTrainerLan** lauscht auf allen IPv4-Netzwerkschnittstellen auf TCP-Port 5080. Nur dieses Profil aktiviert `Network:AllowLan` und erlaubt wechselnde Hostnamen beziehungsweise IP-Adressen im Host-Header.
* Die eigene Zugriffskontrolle verwendet die tatsächliche TCP-Quelladresse. Loopback bleibt immer erlaubt. Im Heimnetz-Modus kommen private IPv4-Adressen aus 10/8, 172.16/12 und 192.168/16 sowie IPv6-ULA hinzu. IPv4-gemappte IPv6-Adressen werden normalisiert. Öffentliche und unbekannte Quelladressen bleiben gesperrt. Das Standardprofil bleibt auf lokale Nutzung begrenzt.
* Ungeprüfte X-Forwarded-For-Header ändern diese Entscheidung nicht. Ein Reverse-Proxy-Betrieb gehört nicht zum aktuellen Verteilungsmodell.
* Das Profil verwendet die eigene Umgebung **LanDemo**, damit keine Entwicklungsfehlerseiten aktiviert werden. Nur in dieser ausdrücklich gewählten Umgebung werden beim lokalen `dotnet run` die Buildmanifest-Ressourcen geladen, insbesondere das Blazor-Skript. Das lokale EXE-Paket verwendet veröffentlichte Dateien.
* Die Windows-Firewallregel **FIAE_AP2_Trainer-LAN-5080** erlaubt eingehend ausschließlich TCP 5080, Profil **Private**, Quellbereich **LocalSubnet**. Edge Traversal ist gesperrt. Die übrige Firewall bleibt aktiv. Es wurden keine Routerweiterleitung und keine Freigabe des SQL-Server-Ports eingerichtet.
* Beim ersten Einrichten wurde die Regel mit einer Windows-Administratorbestätigung angelegt. Das Projekt enthält hierfür **Heimnetz-Firewall-einrichten.ps1**; der normale Appstart braucht keine Administratorrechte. Ein vorhandenes gleichnamiges Regelobjekt wird vom Skript nicht überschrieben.

Um später nur diese Regel wieder zu entfernen, in einer administrativen PowerShell ausführen:

```powershell
Remove-NetFirewallRule -Name 'FIAE_AP2_Trainer-LAN-5080'
```

Der normale Start über **Trainer-starten.cmd** bleibt lokal. Für den Heimnetzstart müssen alle drei Ebenen passen: die gebundene Netzwerkadresse, die App-Zugriffskontrolle und die Windows-Firewall.

## Fehlersuche

* **Verbindung abgelehnt:** Trainer läuft noch nicht, falsche IP/Port oder der bisherige lokale Startmodus läuft noch. Im Startfenster steht die tatsächliche Adresse.
* **Keine Antwort:** Beide Rechner müssen miteinander kommunizieren dürfen. Gast-WLAN, Client-Isolation, zusätzliche Gerätefirewalls oder unterschiedliche Netzsegmente können das verhindern. Die eingerichtete Windows-Regel gilt für das lokale Subnetz im privaten Profil.
* **HTTP 403:** Die App läuft lokal oder sieht keine erlaubte Quelladresse.
* **Statische Seite ohne funktionierende Buttons:** Beim Start das Profil FiaeTrainerLan verwenden. Die Prüfung umfasst ausdrücklich das Blazor-Skript und eine tatsächlich abgegebene SQL-Lösung.
* **SQL-Verbindungsfehler:** Der Datenbankzugriff erfolgt vom Trainer-Rechner. Auf dem zweiten Rechner werden weder SQL Server noch SSMS benötigt. Die vorhandene SQL-Verbindung wird durch den Browserzugriff nicht ersetzt.

## Veröffentlichung ohne zentralen Server

Die frühere Planung für Homeserver, Subdomain und Onlinekonten ist auf Nutzerwunsch entfallen. Stattdessen lädt jeder ein Windows-Paket von GitHub herunter und startet es lokal. Lernstand und Verbindung liegen pro Windows-Benutzer außerhalb des Programmordners. Die technische Einrichtung eines öffentlichen Dienstes und einer Benutzerverwaltung ist daher nicht erforderlich.

Anleitung, Voraussetzungen und Update-Verhalten stehen in [Lokale Releases](lokale-releases.md). Die bestehende Vorführung wurde nicht entfernt und ihre Firewallregel nicht verändert. Die normale EXE aktiviert diese Netzwerkfreigabe nicht.

## Grundlagen

* [Microsoft: Kestrel-Endpunkte](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints?view=aspnetcore-10.0)
* [Microsoft: Windows-Firewallregeln](https://learn.microsoft.com/en-us/powershell/module/netsecurity/new-netfirewallrule?view=windowsdefender)
* [Microsoft: Statische Ressourcen außerhalb von Development](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-10.0)

## Anzeigeeinstellungen im gemeinsamen Profil

Die eingeklappte Aufgabenliste wird ebenso wie Aufgabenmodi und die ausgeblendete Selbsttest-Hilfe im lokalen Profil gespeichert. Diese Einstellungen teilen derzeit alle Besucher der Heimnetzinstanz. Der Hinweis lässt sich ausschließlich unter Lernplan → Bedienhinweise wieder aktivieren. Die separate lokale EXE verwendet stattdessen das Profil des jeweiligen Windows-Benutzers.

## Persönliche Einrichtung

Die neue Seite Updates & SQL-Einrichtung zeigt die Buildversion und den Updatezustand. SQL-Verbindung und persönliche Updatewahl lassen sich während der gemeinsam genutzten Heimnetz-Vorführung nicht ändern; die Sperre wird auch im Service geprüft. Zum Einrichten die Vorführung beenden und Trainer-starten.cmd im lokalen Profil starten. Öffentliche Updatehinweise bleiben in der Vorführung sichtbar, ohne lokale Daten an GitHub zu senden.
