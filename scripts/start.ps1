$ErrorActionPreference = 'Stop'
$projectRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$localRoot = Join-Path $projectRoot '.local'
if (-not (Test-Path -LiteralPath (Join-Path $localRoot 'settings.json'))) { & (Join-Path $PSScriptRoot 'setup.ps1') }
New-Item -ItemType Directory -Force (Join-Path $localRoot 'logs') | Out-Null
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:DOTNET_ENVIRONMENT = 'Development'
$env:ASPIRE_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS = 'true'
$env:ASPIRE_ALLOW_UNSECURED_TRANSPORT = 'true'
$env:ASPNETCORE_URLS = 'http://127.0.0.1:18880'
$env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = 'http://127.0.0.1:18881'
$hostProcess = Start-Process dotnet -ArgumentList @('run', '--no-build', '--no-launch-profile', '--project', 'src/AppHost/AppHost.csproj') -WorkingDirectory $projectRoot -WindowStyle Hidden -RedirectStandardOutput (Join-Path $localRoot 'logs/apphost.log') -RedirectStandardError (Join-Path $localRoot 'logs/apphost-error.log') -PassThru
if (Test-Path -LiteralPath (Join-Path $localRoot 'processes.json')) {
    $previous = Get-Content -Raw (Join-Path $localRoot 'processes.json') | ConvertFrom-Json
    $frontendProcess = Get-Process -Id $previous.frontend -ErrorAction SilentlyContinue
}
if (-not $frontendProcess) {
    $frontendProcess = Start-Process npm.cmd -ArgumentList @('run', 'dev') -WorkingDirectory (Join-Path $projectRoot 'frontend') -WindowStyle Hidden -RedirectStandardOutput (Join-Path $localRoot 'logs/frontend.log') -RedirectStandardError (Join-Path $localRoot 'logs/frontend-error.log') -PassThru
}
@{ apphost = $hostProcess.Id; frontend = $frontendProcess.Id } | ConvertTo-Json | Set-Content (Join-Path $localRoot 'processes.json')
Write-Host 'ARG ERP: http://localhost:3000 | Aspire dashboard: http://localhost:18880'
