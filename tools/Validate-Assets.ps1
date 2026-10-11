[CmdletBinding()]
param(
    [string]$ProjectRoot = (Join-Path $PSScriptRoot '..'),
    [string]$ManifestPath = (Join-Path $PSScriptRoot '..\assets\required-assets.json')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$manifest = Get-Content -LiteralPath $ManifestPath -Raw | ConvertFrom-Json
$errors = [System.Collections.Generic.List[string]]::new()
$validatedCount = 0
$fallbackCount = 0
Add-Type -AssemblyName System.Drawing

foreach ($asset in $manifest.assets) {
    $assetPath = Join-Path $ProjectRoot ([string]$asset.path)
    if (-not (Test-Path -LiteralPath $assetPath -PathType Leaf)) {
        if ($asset.PSObject.Properties['optional'] -and $asset.optional) {
            $fallbackCount++
            Write-Warning "Missing draft artwork (runtime fallback): $($asset.path)"
            continue
        }
        $errors.Add("Missing asset: $($asset.path)")
        continue
    }

    $image = $null
    try {
        $image = [System.Drawing.Image]::FromFile($assetPath)
        $validatedCount++
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

Write-Host "Asset validation passed for $validatedCount available images; $fallbackCount draft images use runtime fallbacks."
