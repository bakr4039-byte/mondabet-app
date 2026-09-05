# Mondabet — start the full local dev stack unattended.
# Updated for an 8GB RAM machine: Docker core infra + all 9 backend services +
# both Angular portals. (Originally scoped down for 4GB — now everything's on.)

$ErrorActionPreference = "Continue"
$root = "E:\psma\Mondabet-Code"
Set-Location $root

# Child processes started via Start-Process inherit this (no -UseNewEnvironment passed).
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
    param([string]$Name, [string]$PortalPath, [int]$Port = 4200)
    $out = Join-Path $logDir "$Name.out.log"
    $err = Join-Path $logDir "$Name.err.log"
    Write-Host "Starting $Name (npm start on port $Port)..."
    # npm resolves to npm.cmd on Windows - Start-Process -FilePath "npm" fails with
    # "%1 is not a valid Win32 application", so go through cmd.exe instead.
    Start-Process -FilePath "cmd.exe" `
        -ArgumentList "/c", "npm start -- --port $Port" `
        -WorkingDirectory $PortalPath `
        -WindowStyle Hidden `
        -RedirectStandardOutput $out `
        -RedirectStandardError $err
}

# 1) Make sure Docker Desktop is up (best effort — safe to skip if already running)
$dockerReady = $false
try { docker info *> $null; if ($LASTEXITCODE -eq 0) { $dockerReady = $true } } catch {}
if (-not $dockerReady) {
    Write-Host "Starting Docker Desktop, waiting up to 90s..."
    Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe" -ErrorAction SilentlyContinue
    $waited = 0
    while ($waited -lt 90) {
        Start-Sleep -Seconds 5
        $waited += 5
        try { docker info *> $null; if ($LASTEXITCODE -eq 0) { $dockerReady = $true; break } } catch {}
    }
}
if (-not $dockerReady) {
    Write-Host "WARNING: Docker engine did not come up in time. Docker-dependent steps will fail."
}

# 2) Core infra (SQL Server / Redis / Keycloak). RabbitMQ/MinIO stay off unless a feature needs them:
# docker compose up -d rabbitmq minio
docker compose up -d sqlserver redis keycloak

Write-Host "Waiting ~90s for Keycloak to finish importing the realm..."
Start-Sleep -Seconds 90

# 3) Core .NET services
Start-DotnetService -Name "Identity" -ProjectPath "src\Services\Identity\Mondabet.Identity.Api" -Port 5001
Start-Sleep -Seconds 5
Start-DotnetService -Name "Gateway"  -ProjectPath "src\Gateway\Mondabet.Gateway" -Port 5000
Start-Sleep -Seconds 5
Start-DotnetService -Name "Tenant"   -ProjectPath "src\Services\Tenant\Mondabet.Tenant.Api" -Port 5002
Start-Sleep -Seconds 5

# 4) Remaining backend services
Start-DotnetService -Name "Employee"      -ProjectPath "src\Services\Employee\Mondabet.Employee.Api" -Port 5003
Start-Sleep -Seconds 4
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

Start-Sleep -Seconds 10

# 5) Both Angular portals
Start-NgPortal -Name "SuperAdminPortal" -PortalPath "src\portals\super-admin" -Port 4200
Start-NgPortal -Name "CustomerPortal"   -PortalPath "src\portals\customer" -Port 4201

Write-Host ""
Write-Host "==================================================================="
Write-Host "Started. Everything runs in the background - this window can close."
Write-Host "Logs: $logDir\<ServiceName>.out.log / .err.log"
Write-Host "Super Admin portal: http://localhost:4200  (login: sa@mondabet.sa / asdd)"
Write-Host "Customer portal:    http://localhost:4201"
Write-Host "Gateway:            http://localhost:5000"
Write-Host "Keycloak admin:     http://localhost:8180"
Write-Host "==================================================================="
Start-Sleep -Seconds 5
