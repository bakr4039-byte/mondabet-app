@echo off
echo Stopping dotnet and node dev processes...
taskkill /F /IM dotnet.exe >nul 2>&1
taskkill /F /IM node.exe >nul 2>&1
REM VBCSCompiler.exe is the C# build server - it's a separate process from dotnet.exe and
REM keeps running (and holding file locks on obj\...\*.dll) even after the dotnet.exe
REM processes that spawned it are gone. Leaving it running is what causes the next build to
REM fail with "CSC : error CS2012: Cannot open ... for writing -- ... locked by 'VBCSCompiler'".
taskkill /F /IM VBCSCompiler.exe >nul 2>&1
echo Stopping Docker containers (data volumes are kept)...
cd /d "%~dp0"
docker compose stop
echo Done.
pause
