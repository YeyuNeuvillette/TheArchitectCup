[CmdletBinding()]
param(
    [string]$LocalizationRoot = (Join-Path $PSScriptRoot '..\TheArchitectCup\localization')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$languages = @('eng', 'jpn', 'zhs')
$errors = [System.Collections.Generic.List[string]]::new()
$tables = @{}

foreach ($language in $languages) {
    $languageRoot = Join-Path $LocalizationRoot $language
    foreach ($file in Get-ChildItem -LiteralPath $languageRoot -Filter '*.json' -File) {
        try {
            $content = Get-Content -LiteralPath $file.FullName -Raw | ConvertFrom-Json -AsHashtable
            $tables["$language/$($file.Name)"] = $content
        }
        catch {
            $errors.Add("Invalid JSON: $($file.FullName): $($_.Exception.Message)")
        }
    }
}

$tableNames = $tables.Keys |
    ForEach-Object { ($_ -split '/', 2)[1] } |
    Sort-Object -Unique

foreach ($tableName in $tableNames) {
    $allKeys = @($languages |
        ForEach-Object {
            $tableKey = "$_/$tableName"
            if ($tables.ContainsKey($tableKey)) { $tables[$tableKey].Keys }
        } |
        Sort-Object -Unique)

    foreach ($language in $languages) {
        $tableKey = "$language/$tableName"
        if (-not $tables.ContainsKey($tableKey)) {
            $errors.Add("Missing localization table: $tableKey")
            continue
        }

        $missing = @($allKeys | Where-Object { -not $tables[$tableKey].ContainsKey($_) })
        if ($missing.Count -gt 0) {
            $errors.Add("Missing keys in ${tableKey}: $($missing -join ', ')")
        }
    }
}

function Get-DynamicSignatures([string]$text) {
    return @([regex]::Matches($text, '\{(?<name>[A-Za-z][A-Za-z0-9_]*)(?::(?<operation>[A-Za-z][A-Za-z0-9_]*))?') |
        ForEach-Object {
            $operation = $_.Groups['operation'].Value
            if ($operation) { "$($_.Groups['name'].Value):$operation" } else { $_.Groups['name'].Value }
        } |
        Sort-Object)
}

foreach ($tableName in $tableNames) {
    $englishKey = "eng/$tableName"
    if (-not $tables.ContainsKey($englishKey)) { continue }

    foreach ($entryKey in $tables[$englishKey].Keys) {
        $expected = Get-DynamicSignatures ([string]$tables[$englishKey][$entryKey])
        foreach ($language in @('jpn', 'zhs')) {
            $tableKey = "$language/$tableName"
            if (-not $tables.ContainsKey($tableKey) -or -not $tables[$tableKey].ContainsKey($entryKey)) {
                continue
            }

            $actual = Get-DynamicSignatures ([string]$tables[$tableKey][$entryKey])
            if (($expected -join '|') -ne ($actual -join '|')) {
                $errors.Add("Dynamic token mismatch: $tableName/$entryKey ($language)")
            }
        }
    }
}

foreach ($tableKey in $tables.Keys) {
    foreach ($entry in $tables[$tableKey].GetEnumerator()) {
        $text = [string]$entry.Value
        foreach ($tag in @('gold', 'blue')) {
            $openCount = ([regex]::Matches($text, "\[$tag\]")).Count
            $closeCount = ([regex]::Matches($text, "\[/$tag\]")).Count
            if ($openCount -ne $closeCount) {
                $errors.Add("Unbalanced [$tag] markup: $tableKey/$($entry.Key)")
            }
        }
    }
}

foreach ($language in $languages) {
    $cards = $tables["$language/cards.json"]
    $powers = $tables["$language/powers.json"]
    $settings = $tables["$language/settings_ui.json"]

    foreach ($cardTitleKey in @($cards.Keys | Where-Object { $_ -like 'THE_ARCHITECT_CUP_CARD_*.title' })) {
        $cardId = $cardTitleKey.Substring(0, $cardTitleKey.Length - '.title'.Length)
        $suffix = $cardId.Substring('THE_ARCHITECT_CUP_CARD_'.Length)
        $powerTitleKey = "THE_ARCHITECT_CUP_POWER_${suffix}_POWER.title"
        $toggleKey = "${cardId}_TOGGLE"
        $cardTitle = [string]$cards[$cardTitleKey]

        if ($powers.ContainsKey($powerTitleKey) -and [string]$powers[$powerTitleKey] -ne $cardTitle) {
            $errors.Add("Card/power title mismatch: $language/$cardId")
        }
        if ($settings.ContainsKey($toggleKey) -and [string]$settings[$toggleKey] -ne $cardTitle) {
            $errors.Add("Card/settings title mismatch: $language/$cardId")
        }
    }
}

$cardIdsSource = Join-Path $PSScriptRoot '..\TheArchitectCupCode\Api\ArchitectCupCardIds.cs'
$declaredCardIds = @([regex]::Matches(
        (Get-Content -LiteralPath $cardIdsSource -Raw),
        'public const string [A-Za-z0-9_]+ = "(?<id>THE_ARCHITECT_CUP_CARD_[A-Z0-9_]+)"') |
    ForEach-Object { $_.Groups['id'].Value } |
    Sort-Object -Unique)
$localizedCardIds = @($tables['eng/cards.json'].Keys |
    Where-Object { $_ -like 'THE_ARCHITECT_CUP_CARD_*.title' } |
    ForEach-Object { $_.Substring(0, $_.Length - '.title'.Length) } |
    Sort-Object -Unique)

foreach ($missingId in @($localizedCardIds | Where-Object { $_ -notin $declaredCardIds })) {
    $errors.Add("Localized card ID is not declared in ArchitectCupCardIds: $missingId")
}
foreach ($unusedId in @($declaredCardIds | Where-Object { $_ -notin $localizedCardIds })) {
    $errors.Add("ArchitectCupCardIds entry has no card localization: $unusedId")
}

if ($errors.Count -gt 0) {
    $errors | ForEach-Object { Write-Error $_ -ErrorAction Continue }
    exit 1
}

Write-Host "Localization validation passed for $($languages.Count) languages and $($tableNames.Count) tables."
