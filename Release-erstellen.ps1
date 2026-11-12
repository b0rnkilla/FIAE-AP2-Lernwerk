param(
    [ValidatePattern('^\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?$')]
    [string]$Version,
    [string]$SigningThumbprint,
    [uri]$TimestampServer,
    [switch]$Sign
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

$signRelease = $Sign -or $PSBoundParameters.ContainsKey('SigningThumbprint') -or $PSBoundParameters.ContainsKey('TimestampServer')
if ($signRelease) {
    if (!$PSBoundParameters.ContainsKey('SigningThumbprint')) { $SigningThumbprint = $env:AP2_SIGNING_THUMBPRINT }
    if (!$PSBoundParameters.ContainsKey('TimestampServer')) { $TimestampServer = $env:AP2_TIMESTAMP_SERVER }
    if ($SigningThumbprint -notmatch '^[0-9a-fA-F]{40}$') {
        throw 'Für die Signierung den Fingerabdruck des Codesignaturzertifikats über -SigningThumbprint oder AP2_SIGNING_THUMBPRINT angeben.'
    }
    if (!$TimestampServer -or !$TimestampServer.IsAbsoluteUri -or $TimestampServer.Scheme -notin 'http','https') {
        throw 'Den RFC-3161-Zeitstempeldienst des Anbieters über -TimestampServer oder AP2_TIMESTAMP_SERVER angeben.'
    }
    $certificate = Get-ChildItem Cert:\CurrentUser\My -CodeSigningCert | Where-Object Thumbprint -eq $SigningThumbprint
    if (!$certificate -or !$certificate.HasPrivateKey -or $certificate.NotBefore -gt (Get-Date) -or $certificate.NotAfter -le (Get-Date)) {
        throw 'Das gewählte Codesignaturzertifikat muss im persönlichen Windows-Zertifikatsspeicher gültig und mit privatem Schlüssel verfügbar sein.'
    }
    $signTool = Get-Command signtool.exe -CommandType Application -ErrorAction SilentlyContinue | Select-Object -First 1
    if (!$signTool) { throw 'SignTool aus dem Windows SDK im Suchpfad bereitstellen, z. B. über eine Developer PowerShell.' }
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
    $_.Name -like 'appsettings.Development*' -or $_.Extension -in '.pfx', '.p12', '.snk', '.key', '.pem', '.log', '.user', '.pdb' -or
    $_.FullName.Substring($packageDirectory.Length) -match '[\\/](Data|artifacts)[\\/]'
})
if ($privateFiles.Count -gt 0) { throw 'Das Paket enthält lokale Dateien. Es wurde kein ZIP erzeugt.' }
$settings = Get-Content -LiteralPath (Join-Path $packageDirectory 'appsettings.json') -Raw | ConvertFrom-Json
$expectedConnection = 'Server=.\SQLEXPRESS;Database=FIAE_AP2_Training;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Connect Timeout=5'
if ($settings.ConnectionStrings.Training -cne $expectedConnection -or @($settings.ConnectionStrings.PSObject.Properties).Count -ne 1) {
    throw 'Die veröffentlichte Verbindung muss die allgemeine SQL-Express-Vorlage sein. Private Konfiguration gehört in appsettings.Local.json.'
}

$releaseExe = Join-Path $packageDirectory 'AP2-Lernwerk.exe'
Move-Item -LiteralPath (Join-Path $packageDirectory 'FiaeTrainer.Web.exe') -Destination $releaseExe
if ($signRelease) {
    & $signTool.Source sign /s My /sha1 $SigningThumbprint /fd SHA256 /tr $TimestampServer.AbsoluteUri /td SHA256 /d 'AP2 Lernwerk' $releaseExe
    if ($LASTEXITCODE -ne 0) { throw 'Signatur oder Zeitstempel fehlgeschlagen. Das Release wurde nicht gepackt.' }
    & $signTool.Source verify /pa /tw $releaseExe
    if ($LASTEXITCODE -ne 0) { throw 'Authenticode-Prüfung der Windows-EXE fehlgeschlagen. Das Release wurde nicht gepackt.' }
    $signature = Get-AuthenticodeSignature -LiteralPath $releaseExe
    if ($signature.Status -ne 'Valid' -or $signature.SignerCertificate.Thumbprint -ne $SigningThumbprint -or !$signature.TimeStamperCertificate) {
        throw 'Die EXE benötigt eine gültige Signatur des gewählten Herausgebers mit Zeitstempel.'
    }
}
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
