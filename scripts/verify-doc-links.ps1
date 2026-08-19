[CmdletBinding()]
param(
    [Parameter()]
    [string] $Root = (Join-Path $PSScriptRoot '..')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$rootPath = [System.IO.Path]::GetFullPath($Root)
$excludedDirectories = @('.git', 'bin', 'obj', 'TestResults', 'publish', 'dist')
$linkPattern = [regex]'!?(?:\[[^\]]*\])\((?<target><[^>]+>|[^\s\)]+)(?:\s+[^\)]*)?\)'
$failures = [System.Collections.Generic.List[string]]::new()
$checkedLinks = 0

function Test-IsExcludedPath {
    param([Parameter(Mandatory)][string] $Path)

    $relative = [System.IO.Path]::GetRelativePath($rootPath, $Path)
    $segments = $relative -split '[\\/]'
    foreach ($segment in $segments) {
        if ($excludedDirectories -contains $segment) {
            return $true
        }
    }

    return $false
}

function Get-LocalTargetPath {
    param(
        [Parameter(Mandatory)][string] $SourcePath,
        [Parameter(Mandatory)][string] $Target
    )

    $value = $Target.Trim()
    if ($value.StartsWith('<') -and $value.EndsWith('>')) {
        $value = $value.Substring(1, $value.Length - 2)
    }

    if ([string]::IsNullOrWhiteSpace($value)
        -or $value.StartsWith('#')
        -or $value -match '^[A-Za-z][A-Za-z0-9+.-]*:') {
        return $null
    }

    $fragmentIndex = $value.IndexOf('#')
    if ($fragmentIndex -ge 0) {
        $value = $value.Substring(0, $fragmentIndex)
    }

    $queryIndex = $value.IndexOf('?')
    if ($queryIndex -ge 0) {
        $value = $value.Substring(0, $queryIndex)
    }

    if ([string]::IsNullOrWhiteSpace($value)) {
        return $null
    }

    try {
        $value = [Uri]::UnescapeDataString($value)
    }
    catch {
        return [pscustomobject]@{
            Invalid = $true
            Value = $value
            Path = $null
        }
    }

    $candidate = if ($value.StartsWith('/')) {
        Join-Path $rootPath $value.TrimStart('/')
    }
    else {
        Join-Path (Split-Path -Parent $SourcePath) $value
    }

    [pscustomobject]@{
        Invalid = $false
        Value = $value
        Path = [System.IO.Path]::GetFullPath($candidate)
    }
}

$markdownFiles = Get-ChildItem -LiteralPath $rootPath -Recurse -File -Filter '*.md' |
    Where-Object { -not (Test-IsExcludedPath -Path $_.FullName) } |
    Sort-Object FullName

foreach ($file in $markdownFiles) {
    $content = Get-Content -LiteralPath $file.FullName -Raw
    $relativeSource = [System.IO.Path]::GetRelativePath($rootPath, $file.FullName)

    foreach ($match in $linkPattern.Matches($content)) {
        $targetText = $match.Groups['target'].Value
        $target = Get-LocalTargetPath -SourcePath $file.FullName -Target $targetText
        if ($null -eq $target) {
            continue
        }

        $checkedLinks++
        if ($target.Invalid) {
            $failures.Add("${relativeSource}: invalid URI encoding in local link '$targetText'")
            continue
        }

        if (-not (Test-Path -LiteralPath $target.Path)) {
            $failures.Add("${relativeSource}: missing local target '$($target.Value)'")
        }
    }
}

if ($failures.Count -gt 0) {
    Write-Error "Documentation link verification failed with $($failures.Count) error(s):`n$($failures -join "`n")"
    exit 1
}

Write-Host "Documentation links verified: $checkedLinks local link(s) across $($markdownFiles.Count) Markdown file(s)."
