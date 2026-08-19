[CmdletBinding()]
param(
    [Parameter()]
    [string] $Root = (Join-Path $PSScriptRoot '..')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$rootPath = [System.IO.Path]::GetFullPath($Root)
$maximumFileBytes = 2MB
$scannerRelativePath = 'scripts/verify-no-secrets.ps1'
$binaryExtensions = @(
    '.7z', '.bmp', '.dll', '.dylib', '.exe', '.gif', '.gz', '.ico', '.jar', '.jpeg', '.jpg',
    '.mp3', '.mp4', '.pdf', '.png', '.so', '.tar', '.webp', '.zip'
)

$rules = @(
    [pscustomobject]@{
        Name = 'private-key-header'
        Pattern = [regex]'-----BEGIN (?:RSA |EC |OPENSSH )?PRIVATE KEY-----'
    },
    [pscustomobject]@{
        Name = 'github-token'
        Pattern = [regex]'\bgh[pousr]_[A-Za-z0-9_]{30,}\b'
    },
    [pscustomobject]@{
        Name = 'aws-access-key-id'
        Pattern = [regex]'\bAKIA[0-9A-Z]{16}\b'
    },
    [pscustomobject]@{
        Name = 'openai-style-secret-key'
        Pattern = [regex]'\bsk-[A-Za-z0-9]{20,}\b'
    },
    [pscustomobject]@{
        Name = 'slack-token'
        Pattern = [regex]'\bxox[baprs]-[A-Za-z0-9-]{20,}\b'
    }
)

$gitOutput = & git -C $rootPath ls-files
if ($LASTEXITCODE -ne 0) {
    throw 'Unable to enumerate tracked files with git ls-files.'
}

$trackedFiles = @($gitOutput) |
    Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
    Sort-Object -Unique

$failures = [System.Collections.Generic.List[string]]::new()
$scannedFiles = 0

foreach ($relativePath in $trackedFiles) {
    $normalizedRelativePath = $relativePath.Replace('\\', '/')
    if ($normalizedRelativePath -eq $scannerRelativePath) {
        continue
    }

    $fullPath = Join-Path $rootPath $relativePath
    if (-not (Test-Path -LiteralPath $fullPath -PathType Leaf)) {
        continue
    }

    $file = Get-Item -LiteralPath $fullPath
    if ($file.Length -gt $maximumFileBytes
        -or $binaryExtensions -contains $file.Extension.ToLowerInvariant()) {
        continue
    }

    try {
        $content = Get-Content -LiteralPath $fullPath -Raw -ErrorAction Stop
    }
    catch {
        continue
    }

    $scannedFiles++
    foreach ($rule in $rules) {
        if ($rule.Pattern.IsMatch($content)) {
            $failures.Add("$normalizedRelativePath: matched high-signal secret pattern '$($rule.Name)'")
        }
    }
}

if ($failures.Count -gt 0) {
    Write-Error "Tracked-file secret verification failed with $($failures.Count) finding(s). Matched values are intentionally not printed.`n$($failures -join "`n")"
    exit 1
}

Write-Host "Tracked-file secret verification passed across $scannedFiles text file(s)."
