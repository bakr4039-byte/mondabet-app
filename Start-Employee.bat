@echo off
set ASPNETCORE_ENVIRONMENT=Development
cd /d "E:\psma\Mondabet-Code"
echo Starting Employee service on port 5003...
dotnet run --project "src\Services\Employee\Mondabet.Employee.Api" --urls http://localhost:5003 > "logs\Employee.out.log" 2> "logs\Employee.err.log"
echo.
echo ==================================================================
echo Employee service stopped. Check logs\Employee.err.log for errors.
echo ==================================================================
pause
