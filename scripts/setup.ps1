$ErrorActionPreference = 'Stop'
$projectRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
& (Join-Path $PSScriptRoot 'assert-scope.ps1')
$localRoot = Join-Path $projectRoot '.local'
New-Item -ItemType Directory -Force $localRoot | Out-Null
$settingsPath = Join-Path $localRoot 'settings.json'
if (-not (Test-Path -LiteralPath $settingsPath)) {
    $random = [Security.Cryptography.RandomNumberGenerator]::Create()
    $keyBytes = New-Object byte[] 48
    $random.GetBytes($keyBytes)
    $passwordBytes = New-Object byte[] 15
    $random.GetBytes($passwordBytes)
    @{
        connectionString = 'Server=(localdb)\MSSQLLocalDB;Database=ARGERP_HR;Integrated Security=true;TrustServerCertificate=true;MultipleActiveResultSets=true'
        signingKey = [Convert]::ToBase64String($keyBytes)
        adminPassword = 'Arg!' + [Convert]::ToBase64String($passwordBytes)
    } | ConvertTo-Json | Set-Content -Encoding UTF8 $settingsPath
}
$settings = Get-Content -Raw -LiteralPath $settingsPath | ConvertFrom-Json
$workflowKeys = @{}
foreach ($name in @('workflow', 'humanresource', 'organization', 'auth')) {
    $hash = [Security.Cryptography.SHA256]::Create().ComputeHash([Text.Encoding]::UTF8.GetBytes($settings.signingKey + ':' + $name))
    $workflowKeys[$name] = [Convert]::ToBase64String($hash)
}
foreach ($service in @('HumanResource', 'Organization')) {
    $config = @{
        ConnectionStrings = @{ OrganizationDB = $settings.connectionString; UserManagementDB = $settings.connectionString }
        JwtSettings = @{ SecretKey = $settings.signingKey; Issuer = 'ARGERP'; Audience = 'ARGERP' }
        Workflow = @{
            Enabled = $true; SingleReplica = $true; ServiceKeys = $workflowKeys
            Services = @{ auth = 'http://127.0.0.1:5183'; organization = 'http://127.0.0.1:5183'; humanresource = 'http://127.0.0.1:5182'; workflow = 'http://127.0.0.1:5182' }
        }
        Logging = @{ LogLevel = @{ Default = 'Warning'; 'Microsoft.AspNetCore' = 'Warning' } }
    }
    $zkSettingsPath = Join-Path $localRoot 'zkt-source.json'
    if ($service -eq 'HumanResource' -and (Test-Path -LiteralPath $zkSettingsPath)) {
        $zkSettings = Get-Content -Raw -LiteralPath $zkSettingsPath | ConvertFrom-Json
        $config.ZkAttendance = @{ Enabled = $true; ConnectionString = $zkSettings.zkConnectionString; IntervalMinutes = 5 }
    }
    $config | ConvertTo-Json -Depth 5 | Set-Content -Encoding UTF8 (Join-Path $projectRoot "src/Services/$service.Service/appsettings.Local.json")
}
if (-not $settings.rabbitPassword) {
    $rabbitBytes = New-Object byte[] 32
    [Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($rabbitBytes)
    $settings | Add-Member -NotePropertyName rabbitUsername -NotePropertyValue 'argerp' -Force
    $settings | Add-Member -NotePropertyName rabbitPassword -NotePropertyValue ([Convert]::ToBase64String($rabbitBytes)) -Force
    $settings | ConvertTo-Json | Set-Content -Encoding UTF8 $settingsPath
}
@{ ConnectionStrings = @{ OrganizationDB=$settings.connectionString; UserManagementDB=$settings.connectionString }; Parameters = @{ 'arg-rabbit-user'=$settings.rabbitUsername; 'arg-rabbit-password'=$settings.rabbitPassword } } | ConvertTo-Json -Depth 4 | Set-Content -Encoding UTF8 (Join-Path $projectRoot 'src/AppHost/appsettings.Local.json')
'VITE_WORKFLOW_ENABLED=true' | Set-Content -Encoding ASCII (Join-Path $projectRoot 'frontend/.env.local')
$localDb = Join-Path $env:ProgramFiles 'Microsoft SQL Server/170/Tools/Binn/SqlLocalDB.exe'
if (Test-Path -LiteralPath $localDb) { & $localDb start MSSQLLocalDB | Out-Host }
Push-Location $projectRoot
try {
    dotnet run --project tools/DatabaseSetup/DatabaseSetup.csproj
    if ($LASTEXITCODE -ne 0) { throw 'Database setup failed.' }
} finally { Pop-Location }
Write-Host 'Database ready. Login: http://localhost:3000 | Username: admin'
Write-Host 'The generated password is saved in .local/settings.json (not tracked by Git).'
