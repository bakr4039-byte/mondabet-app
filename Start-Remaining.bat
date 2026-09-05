@echo off
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0scripts\RunRemaining.ps1"
echo.
echo Done. You can close this window - all services keep running in the background.
pause
