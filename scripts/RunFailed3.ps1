$ErrorActionPreference = "Continue"
$root = "E:\psma\Mondabet-Code"
Set-Location $root
$env:ASPNETCORE_ENVIRONMENT = "Development"
$logDir = Join-Path $root "logs"

# Kill only whatever is actually squatting on these 3 ports (leaves the other
# 7 healthy services + both portals completely untouched).
foreach ($port in 5003, 5007, 5008) {
    $conns = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
    foreach ($c in $conns) {
        Write-Host "Killing PID $($c.OwningProcess) holding port $port..."
        Stop-Process -Id $c.OwningProcess -Force -ErrorAction SilentlyContinue
    }
}
Start-Sleep -Seconds 3

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

Start-DotnetService -Name "Employee"     -ProjectPath "src\Services\Employee\Mondabet.Employee.Api" -Port 5003
Start-Sleep -Seconds 5
Start-DotnetService -Name "Notification" -ProjectPath "src\Services\Notification\Mondabet.Notification.Api" -Port 5007
Start-Sleep -Seconds 5
Start-DotnetService -Name "Report"       -ProjectPath "src\Services\Report\Mondabet.Report.Api" -Port 5008

Write-Host "Done launching Employee, Notification, Report in the background."
Start-Sleep -Seconds 3
