[CmdletBinding()]
param(
    [string]$RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

$ErrorActionPreference = 'Stop'
$root = [System.IO.Path]::GetFullPath($RepositoryRoot)
$rootWithSeparator = $root.TrimEnd(
    [System.IO.Path]::DirectorySeparatorChar,
    [System.IO.Path]::AltDirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar
$failures = [System.Collections.Generic.List[string]]::new()
$linkPattern = [regex]'!?(?:\[[^\]]*\])\((?<target>[^)]+)\)'

function Test-IsExternalTarget {
    param([string]$Target)

    return $Target -match '^(?i:https?|mailto|tel|ftp|data|javascript):' -or $Target.StartsWith('#')
}

function Get-LocalTargetPath {
    param(
        [string]$MarkdownPath,
        [string]$RawTarget
    )

    $target = $RawTarget.Trim()
    if ($target.StartsWith('<')) {
        $closingBracket = $target.IndexOf('>')
        if ($closingBracket -gt 0) {
            $target = $target.Substring(1, $closingBracket - 1)
        }
    }
    else {
        # Markdown titles follow the destination after whitespace. Repository paths
        # containing spaces should use %20 or the angle-bracket destination form.
        $target = ($target -split '\s+', 2)[0]
    }

    if (Test-IsExternalTarget -Target $target) {
        return $null
    }

    $target = ($target -split '#', 2)[0]
    $target = ($target -split '\?', 2)[0]
    if ([string]::IsNullOrWhiteSpace($target)) {
        return $null
    }

    $decoded = [System.Uri]::UnescapeDataString($target)
    $baseDirectory = Split-Path -Parent $MarkdownPath
    return [System.IO.Path]::GetFullPath((Join-Path $baseDirectory $decoded))
}

$markdownFiles = Get-ChildItem -Path $root -Filter '*.md' -File -Recurse | Where-Object {
    $_.FullName -notmatch '[\\/](?:bin|obj|\.git)[\\/]'
}

foreach ($file in $markdownFiles) {
    $content = Get-Content -LiteralPath $file.FullName -Raw
    foreach ($match in $linkPattern.Matches($content)) {
        $rawTarget = $match.Groups['target'].Value
        $localPath = Get-LocalTargetPath -MarkdownPath $file.FullName -RawTarget $rawTarget
        if ($null -eq $localPath) {
            continue
        }

        $isRepositoryPath = $localPath.Equals(
            $root,
            [System.StringComparison]::OrdinalIgnoreCase) -or $localPath.StartsWith(
                $rootWithSeparator,
                [System.StringComparison]::OrdinalIgnoreCase)
        if (-not $isRepositoryPath) {
            $relativeFile = [System.IO.Path]::GetRelativePath($root, $file.FullName)
            $failures.Add("${relativeFile}: link escapes repository root: $rawTarget")
            continue
        }

        if (-not (Test-Path -LiteralPath $localPath)) {
            $relativeFile = [System.IO.Path]::GetRelativePath($root, $file.FullName)
            $relativeTarget = [System.IO.Path]::GetRelativePath($root, $localPath)
            $failures.Add("${relativeFile}: missing local target '$rawTarget' -> '$relativeTarget'")
        }
    }
}

if ($failures.Count -gt 0) {
    Write-Error ("Markdown link verification failed:`n - " + ($failures -join "`n - "))
    exit 1
}

Write-Host "Verified local Markdown links across $($markdownFiles.Count) files."
