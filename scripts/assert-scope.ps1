$ErrorActionPreference = 'Stop'
$projectRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$allowedServices = @('HumanResource.Service', 'Organization.Service') # Auth is hosted in Organization.
foreach ($service in Get-ChildItem -LiteralPath (Join-Path $projectRoot 'src/Services') -Directory) {
    if ($service.Name -notin $allowedServices -and (Get-ChildItem -LiteralPath $service.FullName -File -Filter '*.csproj')) { throw "Out-of-scope service: $($service.Name)" }
}
$solution = Get-Content -Raw -LiteralPath (Join-Path $projectRoot 'ARGERP.sln')
if ($solution -match 'Workflow.Service|Finance.Service|EDXS.Shared.Finance|Legacy[\\/]') { throw 'An excluded project is present in the solution.' }
$gateway = Get-Content -Raw -LiteralPath (Join-Path $projectRoot 'src/Gateway/Yarp.Gateway/appsettings.json') | ConvertFrom-Json
foreach ($cluster in $gateway.ReverseProxy.Clusters.PSObject.Properties.Name) {
    if ($cluster -notin @('hr','organization')) { throw "Out-of-scope gateway cluster: $cluster" }
}
foreach ($file in Get-ChildItem -LiteralPath (Join-Path $projectRoot 'frontend/src') -Recurse -File | Where-Object Extension -in @('.ts','.tsx')) {
    if ((Get-Content -Raw -LiteralPath $file.FullName) -match '(?i)/api/(Academics|Administration|Communication|Student|BankManagement|FinancialManagement|Transport|report)/|sm\.edu-man\.com') {
        throw "Out-of-scope API reference: $($file.FullName)"
    }
}
Write-Host 'Module scope verified: HR, Organization and Auth only.'
