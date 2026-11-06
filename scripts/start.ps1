$ErrorActionPreference = 'Stop'
$projectRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
& (Join-Path $PSScriptRoot 'assert-scope.ps1')
$localRoot = Join-Path $projectRoot '.local'
$runningBackend = @(Get-CimInstance Win32_Process | Where-Object {
    $_.ExecutablePath -like "$projectRoot\*" -and $_.Name -in @('AppHost.exe', 'HumanResource.Service.exe', 'Organization.Service.exe', 'Yarp.Gateway.exe')
})
if ($runningBackend.Count -gt 0) { throw 'ARG backend is already running. Stop its Visual Studio debugging session or run scripts/stop.ps1 -BackendOnly before starting another instance.' }
if (-not (Test-Path -LiteralPath (Join-Path $localRoot 'settings.json'))) { & (Join-Path $PSScriptRoot 'setup.ps1') }
New-Item -ItemType Directory -Force (Join-Path $localRoot 'logs') | Out-Null
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:DOTNET_ENVIRONMENT = 'Development'
$env:ASPIRE_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS = 'true'
$env:ASPIRE_ALLOW_UNSECURED_TRANSPORT = 'true'
$env:ASPNETCORE_URLS = 'http://127.0.0.1:18880'
$env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = 'http://127.0.0.1:18881'
$initialDockerNetworks = @(docker network ls --filter 'name=default-aspire-network-' --format '{{.ID}}')
if ($LASTEXITCODE -ne 0) { throw 'Docker is not available. Start Docker Desktop before starting AppHost.' }
$hostProcess = Start-Process dotnet -ArgumentList @('run', '--no-build', '--launch-profile', 'AppHost', '--project', 'src/AppHost/AppHost.csproj') -WorkingDirectory $projectRoot -WindowStyle Hidden -RedirectStandardOutput (Join-Path $localRoot 'logs/apphost.log') -RedirectStandardError (Join-Path $localRoot 'logs/apphost-error.log') -PassThru
if (Test-Path -LiteralPath (Join-Path $localRoot 'processes.json')) {
    $previous = Get-Content -Raw (Join-Path $localRoot 'processes.json') | ConvertFrom-Json
    $frontendProcess = Get-Process -Id $previous.frontend -ErrorAction SilentlyContinue
}
if (-not $frontendProcess) {
    $frontendProcess = Start-Process npm.cmd -ArgumentList @('run', 'dev') -WorkingDirectory (Join-Path $projectRoot 'frontend') -WindowStyle Hidden -RedirectStandardOutput (Join-Path $localRoot 'logs/frontend.log') -RedirectStandardError (Join-Path $localRoot 'logs/frontend-error.log') -PassThru
}
$launchState = @{ apphost = $hostProcess.Id; frontend = $frontendProcess.Id }
$launchState | ConvertTo-Json | Set-Content (Join-Path $localRoot 'processes.json')
# Record only a uniquely observed new Aspire network from this launch; never prune shared Docker networks.
for ($attempt = 0; $attempt -lt 20; $attempt++) {
    $hostProcess.Refresh()
    if ($hostProcess.HasExited) { throw "AppHost exited during startup. See .local/logs/apphost-error.log." }
    $newNetworks = @(docker network ls --filter 'name=default-aspire-network-' --format '{{.ID}}' | Where-Object { $_ -notin $initialDockerNetworks })
    if ($newNetworks.Count -eq 1) {
        $launchState.dockerNetwork = $newNetworks[0]
        $launchState | ConvertTo-Json | Set-Content (Join-Path $localRoot 'processes.json')
        break
    }
    if ($newNetworks.Count -gt 1) { break }
    Start-Sleep -Milliseconds 500
}
Write-Host 'ARG ERP: http://localhost:3000 | Aspire dashboard: http://localhost:18880'
