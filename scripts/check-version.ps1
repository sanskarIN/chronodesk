[CmdletBinding()]
param(
    [string]$ProjectPath = (Join-Path $PSScriptRoot '../src/ChronoDesk.App/ChronoDesk.App.csproj'),
    [string]$Tag
)

$ErrorActionPreference = 'Stop'
$resolvedProjectPath = (Resolve-Path -LiteralPath $ProjectPath).Path
[xml]$project = Get-Content -LiteralPath $resolvedProjectPath -Raw

function Get-RequiredProjectProperty {
    param([string]$Name)

    $node = $project.SelectSingleNode("/Project/PropertyGroup/$Name")
    if ($null -eq $node -or [string]::IsNullOrWhiteSpace($node.InnerText)) {
        throw "Required project property '$Name' is missing from '$resolvedProjectPath'."
    }

    return $node.InnerText.Trim()
}

$version = Get-RequiredProjectProperty -Name 'Version'
$packageVersion = Get-RequiredProjectProperty -Name 'PackageVersion'
$assemblyVersion = Get-RequiredProjectProperty -Name 'AssemblyVersion'
$fileVersion = Get-RequiredProjectProperty -Name 'FileVersion'

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

$legacyPrefix = $project.SelectSingleNode('/Project/PropertyGroup/VersionPrefix')
$legacySuffix = $project.SelectSingleNode('/Project/PropertyGroup/VersionSuffix')
if (($null -ne $legacyPrefix -and -not [string]::IsNullOrWhiteSpace($legacyPrefix.InnerText)) -or
    ($null -ne $legacySuffix -and -not [string]::IsNullOrWhiteSpace($legacySuffix.InnerText))) {
    throw 'VersionPrefix/VersionSuffix must not be used alongside the canonical four-part Version property.'
}

if (-not [string]::IsNullOrWhiteSpace($Tag)) {
    $expectedTag = "v$version"
    if ($Tag -ne $expectedTag) {
        throw "Release tag '$Tag' does not match application version '$expectedTag'."
    }
}

Write-Host "ChronoDesk version metadata is consistent: $version"
if (-not [string]::IsNullOrWhiteSpace($Tag)) {
    Write-Host "Release tag matches application version: $Tag"
}
