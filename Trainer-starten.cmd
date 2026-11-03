@echo off
setlocal
chcp 65001 >nul
cd /d "%~dp0"
echo.
echo AP2 Lernwerk wird gestartet.
echo Oeffne danach im Browser: http://localhost:5080
echo Zum Beenden dieses Fenster schliessen oder Strg+C druecken.
echo.
set FIAE_OPEN_BROWSER=1
dotnet run --project src\FiaeTrainer.Web --launch-profile FiaeTrainer
if errorlevel 1 pause
