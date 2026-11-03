#Requires -RunAsAdministrator
# Gibt ausschließlich den Trainerport für Verbindungen aus dem lokalen Subnetz im privaten Windows-Netzwerk frei.
$ErrorActionPreference = 'Stop'
$trainerRuleName = 'FIAE_AP2_Trainer-LAN-5080'
if (Get-NetFirewallRule -Name $trainerRuleName -ErrorAction SilentlyContinue) {
    Write-Host 'Die Trainer-Regel existiert bereits. Einstellungen in der Windows-Firewall prüfen.'
    return
}
New-NetFirewallRule -Name $trainerRuleName -DisplayName 'AP2 Lernwerk – privates Heimnetz (TCP 5080)' -Description 'Lokale Vorführung des FIAE-AP2-Trainers; keine allgemeine Internetfreigabe.' -Direction Inbound -Action Allow -Enabled True -Profile Private -Protocol TCP -LocalPort 5080 -RemoteAddress LocalSubnet -EdgeTraversalPolicy Block | Out-Null
Write-Host 'Port 5080 ist für das lokale Subnetz im privaten Netzwerk freigegeben.'
