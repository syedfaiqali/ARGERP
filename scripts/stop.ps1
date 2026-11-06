param([switch]$BackendOnly)
$ErrorActionPreference = 'Stop'
$projectRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$statePath = Join-Path $projectRoot '.local/processes.json'
if (-not (Test-Path -LiteralPath $statePath)) { return }
$state = Get-Content -Raw -LiteralPath $statePath | ConvertFrom-Json
$processes = @(Get-CimInstance Win32_Process)
$roots = @($state.apphost)
if (-not $BackendOnly) { $roots += $state.frontend }
foreach ($rootId in $roots) {
    $rootProcess = $processes | Where-Object ProcessId -eq $rootId | Select-Object -First 1
    if (-not $rootProcess) { continue }
    if ($rootProcess.CommandLine -notmatch 'AppHost.csproj|npm-cli.js|npm.cmd|npm run dev') { throw "Saved process $rootId no longer matches an ARG ERP launcher; refusing to stop it." }
    $ids = [Collections.Generic.List[int]]::new()
    $ids.Add([int]$rootId)
    for ($index = 0; $index -lt $ids.Count; $index++) {
        foreach ($child in $processes | Where-Object ParentProcessId -eq $ids[$index]) { $ids.Add([int]$child.ProcessId) }
    }
    for ($index = $ids.Count - 1; $index -ge 0; $index--) { Stop-Process -Id $ids[$index] -Force -ErrorAction SilentlyContinue }
}
if ($state.dockerNetwork) {
    $networkIds = @(docker network ls --filter "id=$($state.dockerNetwork)" --format '{{.ID}}')
    if ($networkIds.Count -eq 1) {
        $network = docker network inspect $networkIds[0] | ConvertFrom-Json
        if ($network.Name -match '^default-aspire-network-[a-z0-9]+$') {
            $attached = @($network.Containers.PSObject.Properties)
            # Only detach ARG's own persistent brokers from this launch's recorded network.
            $foreign = @($attached | Where-Object { $_.Value.Name -notin @('argerp-redis','argerp-rabbitmq') })
            if ($foreign.Count -eq 0) {
                foreach ($container in $attached) { docker network disconnect $network.Id $container.Name | Out-Null }
                $updated = docker network inspect $network.Id | ConvertFrom-Json
                if (@($updated.Containers.PSObject.Properties).Count -eq 0) { docker network rm $network.Id | Out-Null }
            }
        }
    }
}
Write-Host 'ARG ERP processes stopped.'
