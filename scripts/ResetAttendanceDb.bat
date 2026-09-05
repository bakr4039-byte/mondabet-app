@echo off
REM The Attendance service uses EF Core's EnsureCreatedAsync() (not formal migrations),
REM so after a schema change (new columns/tables) the dev database must be dropped —
REM it gets recreated automatically, with the new schema, the next time the
REM Attendance service starts up. This only affects the MondabetAttendance database;
REM all other services/databases are untouched.
setlocal
sqlcmd -S localhost,1433 -U sa -P "Mondabet_Dev_2024!" -C -Q "IF DB_ID('MondabetAttendance') IS NOT NULL BEGIN ALTER DATABASE MondabetAttendance SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE MondabetAttendance; END"
echo.
echo Done. MondabetAttendance will be recreated automatically next time the Attendance service starts.
endlocal
