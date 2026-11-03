param(
    [ValidatePattern('^\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?$')]
    [string]$Version
)

$ErrorActionPreference = 'Stop'
$projectDirectory = $PSScriptRoot
if (-not $PSBoundParameters.ContainsKey('Version')) {
    [xml]$buildProperties = Get-Content -LiteralPath (Join-Path $projectDirectory 'Directory.Build.props')
    $Version = [string]$buildProperties.Project.PropertyGroup.Version
    if ($Version -notmatch '^\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?$') { throw 'In Directory.Build.props fehlt eine gültige Versionsnummer.' }
}
$releaseName = "AP2-Lernwerk-$Version-win-x64"
$releaseDirectory = Join-Path $projectDirectory 'artifacts/releases'
$archivePath = Join-Path $releaseDirectory "$releaseName.zip"
if (Test-Path -LiteralPath $archivePath) {
    throw "Dieses Release existiert bereits: $archivePath. Verwende eine neue Versionsnummer."
}

# Jeder Durchlauf verwendet einen neuen Ordner. Bestehende Releases und Nutzerdateien werden nicht gelöscht.
$buildDirectory = Join-Path $projectDirectory ("artifacts/release-builds/" + [Guid]::NewGuid().ToString('N'))
$packageDirectory = Join-Path $buildDirectory $releaseName
New-Item -ItemType Directory -Path $packageDirectory -Force | Out-Null

dotnet publish (Join-Path $projectDirectory 'src/FiaeTrainer.Web/FiaeTrainer.Web.csproj') `
    -c Release -p:PublishProfile=Windows-x64 "-p:Version=$Version" `
    -p:DebugType=None -p:DebugSymbols=false -p:CopyOutputSymbolsToPublishDirectory=false -o $packageDirectory
if ($LASTEXITCODE -ne 0) { throw 'Das Windows-Paket konnte nicht erstellt werden.' }

# Ein fehlerhaft erweiterter Publish-Umfang darf keine lokalen Profile oder privaten Schlüssel verbreiten.
$privateFiles = @(Get-ChildItem -LiteralPath $packageDirectory -Recurse -File | Where-Object {
    $_.Name -eq 'progress.json' -or $_.Name -eq 'appsettings.Local.json' -or
    $_.Name -like 'appsettings.Development*' -or $_.Extension -in '.pfx', '.key', '.pem', '.log', '.user', '.pdb' -or
    $_.FullName.Substring($packageDirectory.Length) -match '[\\/](Data|artifacts)[\\/]'
})
if ($privateFiles.Count -gt 0) { throw 'Das Paket enthält lokale Dateien. Es wurde kein ZIP erzeugt.' }
$settings = Get-Content -LiteralPath (Join-Path $packageDirectory 'appsettings.json') -Raw | ConvertFrom-Json
$expectedConnection = 'Server=.\SQLEXPRESS;Database=FIAE_AP2_Training;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Connect Timeout=5'
if ($settings.ConnectionStrings.Training -cne $expectedConnection -or @($settings.ConnectionStrings.PSObject.Properties).Count -ne 1) {
    throw 'Die veröffentlichte Verbindung muss die allgemeine SQL-Express-Vorlage sein. Private Konfiguration gehört in appsettings.Local.json.'
}

Move-Item -LiteralPath (Join-Path $packageDirectory 'FiaeTrainer.Web.exe') -Destination (Join-Path $packageDirectory 'AP2-Lernwerk.exe')
$iisConfiguration = Join-Path $packageDirectory 'web.config'
if (Test-Path -LiteralPath $iisConfiguration) { Remove-Item -LiteralPath $iisConfiguration }
Copy-Item -LiteralPath (Join-Path $projectDirectory 'release/Anleitung.md') -Destination (Join-Path $packageDirectory 'ANLEITUNG.md')
Copy-Item -LiteralPath (Join-Path $projectDirectory 'release/Verbindung.example.json') -Destination $packageDirectory
$versionNotes = Join-Path $projectDirectory "release/Versionshinweise-$Version.md"
if (Test-Path -LiteralPath $versionNotes) { Copy-Item -LiteralPath $versionNotes -Destination (Join-Path $packageDirectory 'VERSIONSHINWEISE.md') }
& (Join-Path $projectDirectory 'release/Abhaengigkeiten-dokumentieren.ps1') -PackageDirectory $packageDirectory -AssetsPath (Join-Path $projectDirectory 'src/FiaeTrainer.Web/obj/project.assets.json')
if (Test-Path -LiteralPath (Join-Path $projectDirectory 'LICENSE')) {
    Copy-Item -LiteralPath (Join-Path $projectDirectory 'LICENSE') -Destination (Join-Path $packageDirectory 'LICENSE')
}
@{
    Version = $Version
    Platform = 'Windows x64'
    SelfContained = $true
    RequiresSqlServer = $true
    CreatedUtc = [DateTime]::UtcNow.ToString('o')
} | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $packageDirectory 'release-info.json') -Encoding utf8

New-Item -ItemType Directory -Path $releaseDirectory -Force | Out-Null
Compress-Archive -LiteralPath $packageDirectory -DestinationPath $archivePath -CompressionLevel Optimal
$checksum = (Get-FileHash -LiteralPath $archivePath -Algorithm SHA256).Hash.ToLowerInvariant()
Set-Content -LiteralPath "$archivePath.sha256" -Value "$checksum  $releaseName.zip" -Encoding ascii
Write-Output "Release bereit: $archivePath"
Write-Output "Programmordner: $packageDirectory"
Write-Output 'Das Paket wurde lokal vorbereitet und nicht zu GitHub hochgeladen.'
