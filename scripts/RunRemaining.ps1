# Starts the remaining backend services + Customer portal WITHOUT touching
# anything already running (Identity/Gateway/Tenant/Employee/SuperAdmin stay untouched).
# Safe to run once, now that the machine has 8GB RAM.

$ErrorActionPreference = "Continue"
$root = "E:\psma\Mondabet-Code"
Set-Location $root
$env:ASPNETCORE_ENVIRONMENT = "Development"

$logDir = Join-Path $root "logs"
New-Item -ItemType Directory -Force -Path $logDir | Out-Null

function Start-DotnetService {
    param([string]$Name, [string]$ProjectPath, [int]$Port)
    $out = Join-Path $logDir "$Name.out.log"
    $err = Join-Path $logDir "$Name.err.log"
    Write-Host "Starting $Name on port $Port..."
    Start-Process -FilePath "dotnet" `
        -ArgumentList "run --project `"$ProjectPath`" --urls http://localhost:$Port" `
        -WorkingDirectory $root `
        -WindowStyle Hidden `
        -RedirectStandardOutput $out `
        -RedirectStandardError $err
}

function Start-NgPortal {
    param([string]$Name, [string]$PortalPath, [int]$Port)
    $out = Join-Path $logDir "$Name.out.log"
    $err = Join-Path $logDir "$Name.err.log"
    Write-Host "Starting $Name (npm start on port $Port)..."
    Start-Process -FilePath "cmd.exe" `
        -ArgumentList "/c", "npm start -- --port $Port" `
        -WorkingDirectory $PortalPath `
        -WindowStyle Hidden `
        -RedirectStandardOutput $out `
        -RedirectStandardError $err
}

Start-DotnetService -Name "Attendance"    -ProjectPath "src\Services\Attendance\Mondabet.Attendance.Api" -Port 5004
Start-Sleep -Seconds 4
Start-DotnetService -Name "Leave"         -ProjectPath "src\Services\Leave\Mondabet.Leave.Api" -Port 5005
Start-Sleep -Seconds 4
Start-DotnetService -Name "Workflow"      -ProjectPath "src\Services\Workflow\Mondabet.Workflow.Api" -Port 5006
Start-Sleep -Seconds 4
Start-DotnetService -Name "Notification"  -ProjectPath "src\Services\Notification\Mondabet.Notification.Api" -Port 5007
Start-Sleep -Seconds 4
Start-DotnetService -Name "Report"        -ProjectPath "src\Services\Report\Mondabet.Report.Api" -Port 5008
Start-Sleep -Seconds 4
Start-DotnetService -Name "Clarification" -ProjectPath "src\Services\Clarification\Mondabet.Clarification.Api" -Port 5009

Start-Sleep -Seconds 8

Start-NgPortal -Name "CustomerPortal" -PortalPath "src\portals\customer" -Port 4201

Write-Host ""
Write-Host "==================================================================="
Write-Host "All remaining services + Customer portal launched in the background."
Write-Host "Logs: $logDir\<ServiceName>.out.log / .err.log"
Write-Host "Customer portal: http://localhost:4201"
Write-Host "==================================================================="
Start-Sleep -Seconds 5
