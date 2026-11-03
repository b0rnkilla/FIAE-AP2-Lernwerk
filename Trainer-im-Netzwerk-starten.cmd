@echo off
setlocal
chcp 65001 >nul
cd /d "%~dp0"
echo.
echo AP2 Lernwerk wird fuer das private Heimnetz gestartet.
echo Die Netzwerkadresse erscheint nach dem Start in diesem Fenster.
echo Auf diesem Rechner: http://localhost:5080
echo Alle Besucher teilen sich aktuell denselben Lernstand.
echo Zum Beenden dieses Fenster schliessen oder Strg+C druecken.
echo Falls Port 5080 belegt ist: den bereits laufenden Trainer zuerst beenden.
echo.
set FIAE_OPEN_BROWSER=1
dotnet run --project src\FiaeTrainer.Web --launch-profile FiaeTrainerLan
if errorlevel 1 pause
