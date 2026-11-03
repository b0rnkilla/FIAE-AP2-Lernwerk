param(
    [Parameter(Mandatory)] [string]$PackageDirectory,
    [Parameter(Mandatory)] [string]$AssetsPath
)

$ErrorActionPreference = 'Stop'
$assets = Get-Content -LiteralPath $AssetsPath -Raw | ConvertFrom-Json
$packageRoots = @($assets.packageFolders.PSObject.Properties.Name)
$licenseDirectory = Join-Path $PackageDirectory 'Lizenzen'
New-Item -ItemType Directory -Path $licenseDirectory -Force | Out-Null
$notes = [System.Collections.Generic.List[string]]::new()
$notes.Add('# Mitgelieferte Abhängigkeiten und Hinweise')
$notes.Add('')
$notes.Add('Diese Liste wird aus den beim Build aufgelösten NuGet-Paketen einschließlich Buildwerkzeugen erzeugt. Die Originalhinweise der Laufzeit und vorhandene paketbezogene Lizenzdateien stehen im Ordner Lizenzen. Die Projektlizenz ist davon unabhängig.')

$packages = @($assets.libraries.PSObject.Properties | Where-Object { $_.Value.type -eq 'package' } | ForEach-Object { $_.Value.path })
$runtimePackages = @($assets.project.frameworks.'net10.0'.downloadDependencies | Where-Object { $_.name -in 'Microsoft.NETCore.App.Runtime.win-x64', 'Microsoft.AspNetCore.App.Runtime.win-x64' })
foreach ($runtime in $runtimePackages) {
    $runtimeVersion = $runtime.version.Trim([char[]]'[] ').Split(',')[0].Trim()
    $packages += ($runtime.name.ToLowerInvariant() + '/' + $runtimeVersion)
}

foreach ($relativePath in $packages | Sort-Object -Unique) {
    $packagePath = $null
    foreach ($root in $packageRoots) {
        $candidate = Join-Path $root $relativePath
        if (Test-Path -LiteralPath $candidate) { $packagePath = $candidate; break }
    }
    if ($null -eq $packagePath) { throw "Abhängigkeit fehlt im Paketcache: $relativePath" }
    $specification = Get-ChildItem -LiteralPath $packagePath -Filter '*.nuspec' | Select-Object -First 1
    [xml]$metadata = Get-Content -LiteralPath $specification.FullName
    $packageId = [string]$metadata.package.metadata.id
    $packageVersion = [string]$metadata.package.metadata.version
    $license = [string]$metadata.package.metadata.license.InnerText
    $licenseType = [string]$metadata.package.metadata.license.type
    $notes.Add('')
    $notes.Add("## $packageId $packageVersion")
    $notes.Add('')
    $notes.Add("NuGet: https://www.nuget.org/packages/$packageId/$packageVersion")
    $notes.Add('')
    $notes.Add([string]$metadata.package.metadata.copyright)
    $notes.Add('')
    if ($licenseType -eq 'expression' -and $license -eq 'MIT') {
        $notes.Add('Lizenz: MIT (vollständiger Text siehe Lizenzen/MIT.txt; Paket-Urheberhinweise stehen oben).')
    }
    elseif ($licenseType -eq 'file') {
        $licensePath = [IO.Path]::GetFullPath((Join-Path $packagePath $license))
        if (-not $licensePath.StartsWith([IO.Path]::GetFullPath($packagePath) + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) { throw 'Ungültiger Lizenzdateipfad im NuGet-Paket.' }
        Copy-Item -LiteralPath $licensePath -Destination (Join-Path $licenseDirectory "$packageId-$packageVersion-LICENSE.txt")
        $notes.Add("Lizenz: Lizenzen/$packageId-$packageVersion-LICENSE.txt")
    }
    else { throw "Neue Lizenzform für $packageId prüfen: $licenseType $license" }

    foreach ($notice in Get-ChildItem -LiteralPath $packagePath -File | Where-Object { $_.Name -match '^(LICENSE|THIRD-PARTY-NOTICES)\.' }) {
        Copy-Item -LiteralPath $notice.FullName -Destination (Join-Path $licenseDirectory "$packageId-$packageVersion-$($notice.Name)")
        if ($packageId -eq 'Microsoft.NETCore.App.Runtime.win-x64' -and $notice.Name -ieq 'LICENSE.TXT') {
            Copy-Item -LiteralPath $notice.FullName -Destination (Join-Path $licenseDirectory 'MIT.txt')
        }
    }
}
if (-not (Test-Path -LiteralPath (Join-Path $licenseDirectory 'MIT.txt'))) { throw 'Der MIT-Lizenztext der .NET-Laufzeit fehlt.' }
$notes | Set-Content -LiteralPath (Join-Path $PackageDirectory 'DRITTANBIETER.md') -Encoding utf8
