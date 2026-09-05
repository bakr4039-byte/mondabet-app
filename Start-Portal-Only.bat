@echo off
echo Starting the Super Admin portal only (visible window so we can see any error)...
cd /d "E:\psma\Mondabet-Code\src\portals\super-admin"
call npm start
echo.
echo ==================================================================
echo The npm process above stopped. If you see an error message above,
echo copy it or take a screenshot and send it back.
echo ==================================================================
pause
