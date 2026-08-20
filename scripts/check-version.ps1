[CmdletBinding()]
param(
    [string]$ProjectPath = (Join-Path $PSScriptRoot '../src/ChronoDesk.App/ChronoDesk.App.csproj'),
    [string]$Tag
)

$ErrorActionPreference = 'Stop'
$resolvedProjectPath = (Resolve-Path -LiteralPath $ProjectPath).Path

function Read-ProjectXml {
    param([string]$Path)

    $resolved = (Resolve-Path -LiteralPath $Path).Path
    return [pscustomobject]@{
        Path = $resolved
        Xml = [xml](Get-Content -LiteralPath $resolved -Raw)
    }
}

function Get-RequiredProjectProperty {
    param(
        [Parameter(Mandatory)]$Project,
        [Parameter(Mandatory)][string]$Name
    )

    $node = $Project.Xml.SelectSingleNode("/Project/PropertyGroup/$Name")
    if ($null -eq $node -or [string]::IsNullOrWhiteSpace($node.InnerText)) {
        throw "Required project property '$Name' is missing from '$($Project.Path)'."
    }

    return $node.InnerText.Trim()
}

$shared = Read-ProjectXml -Path $resolvedProjectPath
$version = Get-RequiredProjectProperty -Project $shared -Name 'Version'
$packageVersion = Get-RequiredProjectProperty -Project $shared -Name 'PackageVersion'
$assemblyVersion = Get-RequiredProjectProperty -Project $shared -Name 'AssemblyVersion'
$fileVersion = Get-RequiredProjectProperty -Project $shared -Name 'FileVersion'

if ($version -notmatch '^\d+\.\d+\.\d+\.\d+$') {
    throw "ChronoDesk Version '$version' must use four numeric components: MAJOR.MINOR.PATCH.REVISION."
}

$versionParts = $version.Split('.') | ForEach-Object { [int]$_ }
if ($versionParts | Where-Object { $_ -lt 0 -or $_ -gt 65534 }) {
    throw "ChronoDesk Version '$version' contains a component outside the assembly-version range 0..65534."
}

$versionProperties = [ordered]@{
    PackageVersion = $packageVersion
    AssemblyVersion = $assemblyVersion
    FileVersion = $fileVersion
}

foreach ($entry in $versionProperties.GetEnumerator()) {
    if ($entry.Value -ne $version) {
        throw "$($entry.Key) '$($entry.Value)' does not match Version '$version'."
    }
}

$legacyPrefix = $shared.Xml.SelectSingleNode('/Project/PropertyGroup/VersionPrefix')
$legacySuffix = $shared.Xml.SelectSingleNode('/Project/PropertyGroup/VersionSuffix')
if (($null -ne $legacyPrefix -and -not [string]::IsNullOrWhiteSpace($legacyPrefix.InnerText)) -or
    ($null -ne $legacySuffix -and -not [string]::IsNullOrWhiteSpace($legacySuffix.InnerText))) {
    throw 'VersionPrefix/VersionSuffix must not be used alongside the canonical four-part Version property.'
}

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$desktop = Read-ProjectXml -Path (Join-Path $repoRoot 'src/ChronoDesk.Desktop/ChronoDesk.Desktop.csproj')
foreach ($name in @('Version', 'PackageVersion', 'AssemblyVersion', 'FileVersion')) {
    $value = Get-RequiredProjectProperty -Project $desktop -Name $name
    if ($value -ne $version) {
        throw "Desktop $name '$value' does not match canonical version '$version'."
    }
}

$android = Read-ProjectXml -Path (Join-Path $repoRoot 'src/ChronoDesk.Android/ChronoDesk.Android.csproj')
$androidDisplayVersion = Get-RequiredProjectProperty -Project $android -Name 'ApplicationDisplayVersion'
if ($androidDisplayVersion -ne $version) {
    throw "Android ApplicationDisplayVersion '$androidDisplayVersion' does not match canonical version '$version'."
}

$androidVersionCode = Get-RequiredProjectProperty -Project $android -Name 'ApplicationVersion'
if ($androidVersionCode -notmatch '^\d+$' -or [int64]$androidVersionCode -le 0) {
    throw "Android ApplicationVersion '$androidVersionCode' must be a positive integer."
}

$ios = Read-ProjectXml -Path (Join-Path $repoRoot 'src/ChronoDesk.iOS/ChronoDesk.iOS.csproj')
$iosDisplayVersion = Get-RequiredProjectProperty -Project $ios -Name 'ApplicationDisplayVersion'
$expectedIosDisplayVersion = ($versionParts[0..2] -join '.')
if ($iosDisplayVersion -ne $expectedIosDisplayVersion) {
    throw "iOS ApplicationDisplayVersion '$iosDisplayVersion' must be '$expectedIosDisplayVersion' for canonical version '$version'."
}

$iosBuildNumber = Get-RequiredProjectProperty -Project $ios -Name 'ApplicationVersion'
if ($iosBuildNumber -notmatch '^\d+$' -or [int64]$iosBuildNumber -le 0) {
    throw "iOS ApplicationVersion '$iosBuildNumber' must be a positive integer."
}

if (-not [string]::IsNullOrWhiteSpace($Tag)) {
    $expectedTag = "v$version"
    if ($Tag -ne $expectedTag) {
        throw "Release tag '$Tag' does not match application version '$expectedTag'."
    }
}

Write-Host "ChronoDesk cross-platform version metadata is consistent: $version"
Write-Host "Apple marketing version: $expectedIosDisplayVersion (build $iosBuildNumber)"
if (-not [string]::IsNullOrWhiteSpace($Tag)) {
    Write-Host "Release tag matches application version: $Tag"
}
