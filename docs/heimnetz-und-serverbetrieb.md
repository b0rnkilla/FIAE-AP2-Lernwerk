# Optionaler Heimnetz-Betrieb

Quellstand **0.1.1-alpha.12 · 06.11.2026**. Die gleiche Windows-EXE unterstützt lokale Nutzung und private Heimnetz-Freigabe. Login, Registrierung, Benutzertabellen, Homeserver und nginx bleiben gestrichen. Die Nutzung richtet sich weiterhin nach den [Nutzungsbedingungen](../LICENSE). Das vorhandene Alpha-9-ZIP wurde nicht verändert; diese Erweiterung gehört erst in ein künftig ausdrücklich beauftragtes Paket.

## Für Anwender: Freigabe in der App

1. Die EXE normal starten und **Heimnetz** in der Fußzeile öffnen.
2. Die Seite prüft das private Windows-Netzwerk und die eigene Firewall-Regel. Bei öffentlichem Profil gibt es eine Erklärung und einen Link in die Windows-Einstellungen; das Profil wird nicht automatisch geändert.
3. **Im Heimnetz freigeben** wählen. Fehlt eine passende Regel, fragt Windows nach Administratorrechten ausschließlich für den Helfer. Abbruch, fehlende Rechte oder fehlgeschlagene Prüfung speichern keine neue Betriebsart.
4. Der Webhost startet innerhalb derselben EXE kurz neu. Der Browser kehrt automatisch zurück. Bereits gespeicherte Entwürfe und Erfolge bleiben erhalten. Vorher offene Eingaben verlassen; Prüfungsuhren werden nicht automatisch pausiert.
5. Auf dem anderen Gerät die angezeigte Adresse öffnen oder den lokal erzeugten QR-Code scannen. Der Gastgeber bleibt eingeschaltet und die EXE geöffnet. Auf den anderen Geräten sind weder Installation noch SQL Server nötig.
6. **Freigabe beenden** auf dem Gastgeber schaltet nach kurzem Neustart wieder auf localhost. Dafür sind keine Administratorrechte nötig.

Verwaltung ist ausschließlich über die lokale Adresse `http://localhost:5080/heimnetz` beziehungsweise den tatsächlich verwendeten Port möglich. Beim Zugriff über die Heimnetz-IP zeigt die Seite nur den Modus und erklärt den Verwaltungsweg. Adresse/Kopierknopf/QR werden nur dem lokal zugreifenden Gastgeber angezeigt. Es gibt keine zweite EXE und keine erforderlichen Anwenderskripte.

### Speicherung und Firewall

Network:AllowLan, Urls und AllowedHosts werden unter derselben atomaren Dateisperre wie SQL-Verbindung/Updatewahl in der persönlichen Einstellungsdatei gespeichert. Windows-Releases verwenden dafür `%LOCALAPPDATA%/AP2-Lernwerk/appsettings.Local.json`. Die Wahl bleibt bei Programmstarts und Updates erhalten; bestehende Konfiguration und Lernstände werden nicht ersetzt. Explizite externe Startparameter haben beim nächsten regulären Start weiterhin Vorrang. Interne Wechsel überschreiben alte Startparameter ausdrücklich.

Die neue Regel **AP2-Lernwerk-Heimnetz-TCP-<Port>** erlaubt eingehend TCP nur für den tatsächlichen Port und den ausgeführten Programmpfad, ausschließlich im privaten Windows-Profil, RemoteAddress LocalSubnet, EdgeTraversal Block. Keine Einzel-IP-Regeln, Routerweiterleitungen oder Freigaben des SQL-Server-Ports. Die App selbst akzeptiert im Heimnetz private Quelladressen und verwirft öffentliche; der Listener verwendet IPv4. Die Adressanzeige berücksichtigt private IPv4-Adapter mit Standardgateway.

Eine passende eigene Regel wird wiederverwendet. Nach einem Update in einen neuen Programmordner kann **Freigabe reparieren** mit erneuter Windows-Bestätigung nötig sein, weil die Regel bewusst an die EXE gebunden ist. Beim Abschalten bleibt diese eingeschränkte Regel bestehen; die App lauscht dann nur lokal. Fremde und ältere Vorführungsregeln werden nicht geändert. Der Helfer unterstützt bei Bedarf das Entfernen ausschließlich seiner eigenen benannten Regel über Action Remove, Port und ProgramPath; das ist kein notwendiger Anwenderschritt.

Die Verwaltungs-POSTs verlangen Antiforgery-Token, echte Loopback-Verbindung und einen lokalen Host-Header. Parallelaktionen erhalten 409; Bereitschaft während des Neustarts liefert 503. Externe Besucher können weder Firewall noch Betriebsart ändern. Die Windows-Firewall allein kann Gast-WLAN-Isolation, VPNs, Drittanbieterfilter und Unternehmensrichtlinien nicht aufheben.

### Gemeinsames Profil

**Alle verbundenen Geräte teilen denselben Lernstand** einschließlich Entwürfen, Modulmodi, Abschlüssen, Prüfungsrunden, Kontrollmarkierungen und Zurücksetzen. Diese Freigabe eignet sich zum Gerätewechsel und zur gemeinsamen Vorführung. Mehrere Personen lernen damit nicht unabhängig voneinander. Es werden keine Konten ergänzt.

## Entwicklungstest ohne ZIP

Trainer-starten.cmd baut und startet die aktuelle Quellfassung. Im Browser unter Heimnetz freigeben und anschließend mit einem anderen Gerät die angezeigte Adresse öffnen. Der normale Starter übernimmt die gespeicherte Betriebsart; das Profil FiaeTrainer besitzt dafür keine erzwungene applicationUrl mehr. FiaeTrainerLan beziehungsweise Trainer-im-Netzwerk-starten.cmd bleibt ein optionaler expliziter LAN-Start und wird für den normalen Einstellungsablauf nicht benötigt. Visual Studio kann den normalen Starter ersetzen. Eine Build-EXE unter bin/Release ist kein portables Windows-Paket.

## Historie: Quellprojektstarter seit Alpha 4

Die folgenden Angaben dokumentieren den weiterhin vorhandenen Entwicklungsstarter. Anwender künftiger Pakete verwenden stattdessen die Einstellungsseite oben.

## Jetzt: Vorführung im Heimnetz

1. Einen bereits laufenden Trainer beenden, falls er Port 5080 belegt.
2. Im Projektordner **Trainer-im-Netzwerk-starten.cmd** starten und das Fenster geöffnet lassen.
3. Auf diesem Rechner bleibt **http://localhost:5080** erreichbar. Auf dem anderen Rechner die im Startfenster angezeigte Heimnetzadresse öffnen.

Verwende die im Startfenster angezeigte Adresse des Trainer-Rechners mit Port 5080. Die Netzwerkadresse kann sich durch DHCP oder einen Netzwerkwechsel ändern. localhost bezeichnet auf dem zweiten Rechner diesen zweiten Rechner, nicht den Trainer-Rechner.

Der Trainer-Rechner muss eingeschaltet, mit dem Netzwerk verbunden und wach bleiben. Eine Teams-Bildschirmfreigabe funktioniert unabhängig davon; Teilnehmer außerhalb des Heimnetzes können die private Adresse nicht allein über Teams öffnen.

**Alle Besucher teilen sich momentan denselben Lernstand**, einschließlich Entwürfen, Abschlüssen, aufgabenspezifischer Moduswahl, WiSo-Runde samt Antworten und Prüfungszeit, Anzeige der allgemeinen Bedienhilfe und Zurücksetzen. Eine WiSo-Runde kann deshalb nicht von mehreren Personen unabhängig gleichzeitig bearbeitet werden; veraltete Fenster werden beim Speichern zum Neuladen aufgefordert. Der Heimnetz-Modus zeigt das gemeinsame Profil auf jeder Seite. Die SQL-Ausführungen der Werkstatt bleiben trotzdem in eigenen Sitzungen isoliert. Für unabhängige Fortschritte verwendet jeder seine eigene lokale Kopie; Onlinekonten werden nicht ergänzt.

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

### Chrome auf Android erreicht keine Geräte im Heimnetz

Wenn derselbe Trainer auf demselben Handy mit Samsung Internet funktioniert, Chrome aber auch die Routerseite nicht öffnen kann, zuerst die Android-App-Berechtigungen von Chrome prüfen. Android 17 verlangt bei entsprechend aktuellen Apps eine eigene Berechtigung für lokale Netzwerkzugriffe; sie gehört zur Gruppe „Geräte in der Nähe“. Sie ist von den Website-Einstellungen innerhalb von Chrome getrennt.

1. In den **Handy-Einstellungen → Apps → Chrome → Berechtigungen** öffnen.
2. **Geräte in der Nähe** beziehungsweise **Lokales Netzwerk** wählen und bei verweigertem Zugriff **Zulassen** setzen. Die genaue Bezeichnung kann je nach Systemversion abweichen.
3. Chrome neu starten und die lokale Traineradresse erneut aufrufen.

Der Nutzer hat diesen Lösungsweg am 04.11.2026 auf einem Samsung-Gerät mit One UI 9.0 und Chrome 156.0.8078.79 erfolgreich bestätigt. Zuvor scheiterten Trainer und Router in Chrome, während Samsung Internet beide erreichte. Inkognito, gelöschte Websitedaten und ausgeschaltete mobile Daten hatten den Fehler nicht behoben.

Die vorhandene Windows-Regel mit `RemoteAddress LocalSubnet` gilt bereits für alle Geräte im direkt angeschlossenen lokalen Subnetz. Eine Regel pro Handy-IP ist nicht erforderlich. Den Browserzugriff erlaubt man einmal für die Android-App; zusätzliche Windows-Freigaben oder das Abschalten von Safe Browsing, sicherem DNS und weiteren Schutzfunktionen sind hierfür nicht erforderlich. Zum Test veränderte Schutzfunktionen anschließend wieder auf ihre bisherigen Werte stellen.

Quellen: [Android: Lokale Netzwerkberechtigung](https://developer.android.com/privacy-and-security/local-network-permission), [Chromium: Android-Berechtigung für lokale Netzwerkzugriffe](https://chromium.googlesource.com/chromium/src/+/6a6d202a1596821b35195abc88eaca7ee4ae4133%5E%21/).

## Veröffentlichung ohne zentralen Server

Die frühere Planung für Homeserver, Subdomain und Onlinekonten ist auf Nutzerwunsch entfallen. Stattdessen lädt jeder ein Windows-Paket von GitHub herunter und startet es lokal. Lernstand und Verbindung liegen pro Windows-Benutzer außerhalb des Programmordners. Die technische Einrichtung eines öffentlichen Dienstes und einer Benutzerverwaltung ist daher nicht erforderlich.

Anleitung, Voraussetzungen und Update-Verhalten stehen in [Lokale Releases](lokale-releases.md). Die bestehende Vorführung wurde nicht entfernt und ihre Firewallregel nicht verändert. Seit Alpha 12 kann die EXE die Netzwerkfreigabe ausdrücklich über die Einstellungsseite aktivieren; der erstmalige Standard bleibt lokal.

## Grundlagen

* [Microsoft: Kestrel-Endpunkte](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints?view=aspnetcore-10.0)
* [Microsoft: Windows-Firewallregeln](https://learn.microsoft.com/en-us/powershell/module/netsecurity/new-netfirewallrule?view=windowsdefender)
* [Microsoft: Statische Ressourcen außerhalb von Development](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-10.0)

## Anzeigeeinstellungen im gemeinsamen Profil

Die eingeklappte Aufgabenliste wird ebenso wie Aufgabenmodi und die ausgeblendete Selbsttest-Hilfe im lokalen Profil gespeichert. Diese Einstellungen teilen derzeit alle Besucher der Heimnetzinstanz. Der Hinweis lässt sich ausschließlich unter Lernplan → Bedienhinweise wieder aktivieren. Die EXE verwendet in beiden Betriebsarten das Profil des jeweiligen Windows-Benutzers des Gastgebers.

## Persönliche Einrichtung

Die neue Seite Updates & SQL-Einrichtung zeigt die Buildversion und den Updatezustand. SQL-Verbindung und persönliche Updatewahl lassen sich während der gemeinsam genutzten Heimnetz-Vorführung nicht ändern; die Sperre wird auch im Service geprüft. Seit Alpha 12 zum Einrichten auf dem Gastgeber unter Heimnetz die Freigabe beenden; die SQL-Einrichtung bleibt während aktiver Freigabe auch lokal gesperrt. Öffentliche Updatehinweise bleiben in der Vorführung sichtbar, ohne lokale Daten an GitHub zu senden.
