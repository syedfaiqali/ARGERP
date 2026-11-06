param([string]$ArtifactsPath)
$ErrorActionPreference = 'Stop'
$projectRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
& (Join-Path $PSScriptRoot 'assert-scope.ps1')
if (-not $ArtifactsPath) {
    $running = @(Get-CimInstance Win32_Process | Where-Object {
        $_.ExecutablePath -like "$projectRoot\*" -and $_.Name -in @('AppHost.exe','HumanResource.Service.exe','Organization.Service.exe','Yarp.Gateway.exe')
    })
    if ($running.Count) { throw 'Stop ARG debugging before building, or use -ArtifactsPath .local/build to verify with isolated output.' }
}
Push-Location $projectRoot
try {
    $buildArgs = @('build','ARGERP.sln','--nologo')
    if ($ArtifactsPath) { $buildArgs += @('--artifacts-path',$ArtifactsPath) }
    & dotnet @buildArgs
    if ($LASTEXITCODE) { throw 'Backend build failed.' }
    npm --prefix frontend run build
    if ($LASTEXITCODE) { throw 'Frontend build failed.' }
} finally { Pop-Location }
