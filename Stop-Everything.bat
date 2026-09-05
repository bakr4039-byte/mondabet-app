@echo off
echo Stopping dotnet and node dev processes...
taskkill /F /IM dotnet.exe >nul 2>&1
taskkill /F /IM node.exe >nul 2>&1
echo Stopping Docker containers (data volumes are kept)...
cd /d "%~dp0"
docker compose stop
echo Done.
pause
