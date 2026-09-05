@echo off
REM Non-interactive build check: builds the whole solution and writes full
REM output (including any errors) to logs\build.log, then exits on its own
REM (no prompts, no pauses) so it's safe to run by double-click.
setlocal
cd /d "%~dp0\.."
if not exist logs mkdir logs
echo Build started: %date% %time% > logs\build.log
dotnet build Mondabet.sln -c Debug --nologo >> logs\build.log 2>&1
echo. >> logs\build.log
echo Build finished with exit code %errorlevel% >> logs\build.log
echo Done. See logs\build.log for full output.
endlocal
