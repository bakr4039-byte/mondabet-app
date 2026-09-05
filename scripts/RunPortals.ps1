$ErrorActionPreference = "Continue"
$root = "E:\psma\Mondabet-Code"
Set-Location $root
$logDir = Join-Path $root "logs"

# Kill whatever is stuck on 4200/4201 (including any hung "port in use?" prompt processes)
foreach ($port in 4200, 4201) {
    $conns = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
    foreach ($c in $conns) {
        Write-Host "Killing PID $($c.OwningProcess) holding port $port..."
        Stop-Process -Id $c.OwningProcess -Force -ErrorAction SilentlyContinue
    }
}
# Also kill any orphaned node.exe that might be the actual ng-serve process behind a stuck prompt
Get-Process node -ErrorAction SilentlyContinue | ForEach-Object {
    try {
        $cmdline = (Get-CimInstance Win32_Process -Filter "ProcessId=$($_.Id)").CommandLine
        if ($cmdline -match "ng serve" -or $cmdline -match "port 4200" -or $cmdline -match "port 4201") {
            Write-Host "Killing orphaned ng serve node PID $($_.Id)"
            Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
        }
    } catch {}
}
Start-Sleep -Seconds 3

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

Start-NgPortal -Name "SuperAdminPortal" -PortalPath "src\portals\super-admin" -Port 4200
Start-Sleep -Seconds 3
Start-NgPortal -Name "CustomerPortal" -PortalPath "src\portals\customer" -Port 4201

Write-Host "Done. Give it ~60s to build."
Start-Sleep -Seconds 3
