[CmdletBinding()]
param(
    [string]$ProjectRoot = (Join-Path $PSScriptRoot '..'),
    [string]$ManifestPath = (Join-Path $PSScriptRoot '..\assets\required-assets.json')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$manifest = Get-Content -LiteralPath $ManifestPath -Raw | ConvertFrom-Json
$errors = [System.Collections.Generic.List[string]]::new()
Add-Type -AssemblyName System.Drawing

foreach ($asset in $manifest.assets) {
    $assetPath = Join-Path $ProjectRoot ([string]$asset.path)
    if (-not (Test-Path -LiteralPath $assetPath -PathType Leaf)) {
        $errors.Add("Missing asset: $($asset.path)")
        continue
    }

    $image = $null
    try {
        $image = [System.Drawing.Image]::FromFile($assetPath)
        if ($image.Width -ne [int]$asset.width -or $image.Height -ne [int]$asset.height) {
            $errors.Add(
                "Invalid dimensions: $($asset.path) is $($image.Width)x$($image.Height), expected $($asset.width)x$($asset.height)")
        }
    }
    catch {
        $errors.Add("Unreadable image: $($asset.path): $($_.Exception.Message)")
    }
    finally {
        if ($null -ne $image) { $image.Dispose() }
    }
}

if ($errors.Count -gt 0) {
    $errors | ForEach-Object { Write-Error $_ -ErrorAction Continue }
    exit 1
}

Write-Host "Asset validation passed for $($manifest.assets.Count) external images."
