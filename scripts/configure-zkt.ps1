param([Parameter(Mandatory=$true)][string]$SourceSettings)
$ErrorActionPreference = 'Stop'
$projectRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$source = Get-Content -Raw -LiteralPath $SourceSettings | ConvertFrom-Json
if ([string]::IsNullOrWhiteSpace($source.ZkAttendance.ConnectionString) -or [string]::IsNullOrWhiteSpace($source.ConnectionStrings.OrganizationDB)) {
    throw 'The source settings must contain ZkAttendance:ConnectionString and ConnectionStrings:OrganizationDB.'
}
$localRoot = Join-Path $projectRoot '.local'
New-Item -ItemType Directory -Force $localRoot | Out-Null
@{zkConnectionString=$source.ZkAttendance.ConnectionString; organizationConnectionString=$source.ConnectionStrings.OrganizationDB} | ConvertTo-Json | Set-Content -Encoding UTF8 (Join-Path $localRoot 'zkt-source.json')
Write-Host 'Existing ZKT configuration saved locally. Run setup.ps1 to apply it; no source settings were changed.'
