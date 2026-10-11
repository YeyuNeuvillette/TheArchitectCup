[CmdletBinding()]
param(
    [string]$Configuration = 'Debug',
    [string]$ProjectRoot = (Join-Path $PSScriptRoot '..')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$bin = Join-Path $ProjectRoot ".godot/mono/temp/bin/$Configuration"
if (-not (Test-Path (Join-Path $bin 'TheArchitectCup.dll'))) {
    throw "Build the $Configuration assembly before running this test."
}
# Model-only checks run in a fresh PowerShell process, without launching the game
# or touching player saves. Do not use this isolated ModelDb in a running game.
Get-ChildItem $bin -Filter *.dll | ForEach-Object {
    [void][Reflection.Assembly]::LoadFrom($_.FullName)
}
$assembly = [Reflection.Assembly]::LoadFrom((Join-Path $bin 'TheArchitectCup.dll'))
$game = [Reflection.Assembly]::LoadFrom((Join-Path $bin 'sts2.dll'))
$flags = [Reflection.BindingFlags]'Public,NonPublic,Static,Instance'
$script:assertions = 0
function Assert-Equal($actual, $expected, [string]$label) {
    if ($actual -ne $expected) { throw "$label expected '$expected', got '$actual'." }
    $script:assertions++
}

$cards = $assembly.GetType('TheArchitectCup.Api.ArchitectCupApi').GetProperty('Cards').GetValue($null)
$phase6 = @($cards | Where-Object Phase -eq 6)
Assert-Equal $phase6.Count 7 'Phase 6 API count'
$catalog = $assembly.GetType('TheArchitectCup.Content.ArchitectCupCardCatalog')
$champions = @($catalog.GetProperty('ChampionCards', $flags).GetValue($null) | Where-Object Phase -eq 6)
Assert-Equal $champions.Count 1 'Phase 6 champion count'
Assert-Equal $champions[0].Id 'THE_ARCHITECT_CUP_CARD_PHASE6_APOTHEOSIS' 'Champion ID'
$settings = [Activator]::CreateInstance($assembly.GetType('TheArchitectCup.Settings.CardSettingsData'))
$db = $game.GetType('MegaCrit.Sts2.Core.Models.ModelDb').GetField('_contentById', $flags).GetValue($null)
$localKeywords = [Enum]::Parse($game.GetType('MegaCrit.Sts2.Core.Entities.Cards.KeywordSources'), 'Local')
$specs = @{
    Phase6Rollback = @(0, 'Skill', 'Uncommon', 'DefectCardPool', 'Retain', 'Cards', 2, 2)
    Phase6Haste = @(0, 'Skill', 'Uncommon', 'ColorlessCardPool', 'Innate', '', 0, 0)
    Phase6BestPartner = @(1, 'Skill', 'Uncommon', 'NecrobinderCardPool', '', 'Summon', 5, 7)
    Phase6Apotheosis = @(1, 'Skill', 'Rare', 'ColorlessCardPool', '', '', 0, 0)
    Phase6Uncertainty = @(1, 'Skill', 'Rare', 'SilentCardPool', 'Retain', 'Cards', 2, 2)
    Phase6AutomaticWeapon = @(1, 'Power', 'Rare', 'ColorlessCardPool', '', 'Cards', 2, 3)
    Phase6Supernova = @(2, 'Skill', 'Rare', 'RegentCardPool', '', 'Stars', 3, 4)
}
$models = @{}
foreach ($info in $phase6) {
    $name = $info.ModelType.Name
    $spec = $specs[$name]
    Assert-Equal $info.Configurable $true "$name configurable"
    Assert-Equal $info.ShowInCompendium $true "$name compendium"
    $property = $settings.GetType().GetProperty("${name}Enabled")
    Assert-Equal $property.GetValue($settings) $true "$name default enabled"
    $property.SetValue($settings, $false)
    Assert-Equal $property.GetValue($settings) $false "$name toggle"
    $registration = @($info.ModelType.GetCustomAttributesData() |
        Where-Object { $_.AttributeType.Name -eq 'RegisterCardAttribute' })
    Assert-Equal $registration.Count 1 "$name registration count"
    Assert-Equal $registration[0].ConstructorArguments[0].Value.Name $spec[3] "$name pool"
    $entry = @($registration[0].NamedArguments | Where-Object MemberName -eq 'FullPublicEntry')
    Assert-Equal $entry[0].TypedValue.Value $info.Id "$name stable ID"

    $canonical = [Activator]::CreateInstance($info.ModelType)
    $db.Add($canonical.Id, $canonical)
    $models[$name] = $canonical
    $card = $canonical.ToMutable()
    Assert-Equal $card.EnergyCost.GetWithModifiers(0) $spec[0] "$name cost"
    Assert-Equal $card.Type.ToString() $spec[1] "$name type"
    Assert-Equal $card.Rarity.ToString() $spec[2] "$name rarity"
    if ($spec[5]) {
        Assert-Equal $card.DynamicVars.Item($spec[5]).IntValue $spec[6] "$name base variable"
    }
    $card.UpgradeInternal()
    Assert-Equal $card.EnergyCost.GetWithModifiers(0) $spec[0] "$name upgraded cost"
    if ($spec[4]) {
        Assert-Equal (@($card.GetKeywordsWithSources($localKeywords) | ForEach-Object ToString) -contains $spec[4]) $true "$name upgraded keyword"
    }
    if ($name -eq 'Phase6Apotheosis') {
        Assert-Equal (@($card.GetKeywordsWithSources($localKeywords) | ForEach-Object ToString) -contains 'Innate') $false 'Apotheosis loses Innate'
    }
    if ($spec[5]) {
        Assert-Equal $card.DynamicVars.Item($spec[5]).IntValue $spec[7] "$name upgraded variable"
    }
    $card.DowngradeInternal()
    Assert-Equal $card.IsUpgraded $false "$name downgrade"
    Assert-Equal $card.EnergyCost.GetWithModifiers(0) $spec[0] "$name downgraded cost"
}
$pool = [Activator]::CreateInstance($assembly.GetType('TheArchitectCup.Characters.TheArchitectCup.ArchitectCupPhase6Pool'))
$types = @($pool.GetType().GetProperty('CardTypes', $flags).GetValue($pool))
Assert-Equal $types.Count 7 'Phase 6 compendium pool size'
Assert-Equal (@($types | Select-Object -Unique).Count) 7 'No duplicate compendium entries'

# Minimal isolated payment fixture: no run, combat loop, UI or save IO. Fields
# normally initialized by run setup are injected only in this fresh test process.
function Set-Field($object, [string]$name, $value) {
    $type = $object.GetType()
    while ($type) {
        $field = $type.GetField($name, $flags)
        if ($field) { $field.SetValue($object, $value); return }
        $type = $type.BaseType
    }
    throw "Missing fixture field: $name"
}
$playerType = $game.GetType('MegaCrit.Sts2.Core.Entities.Players.Player')
$stateType = $game.GetType('MegaCrit.Sts2.Core.Entities.Players.PlayerCombatState')
$player = [Runtime.CompilerServices.RuntimeHelpers]::GetUninitializedObject($playerType)
$state = [Runtime.CompilerServices.RuntimeHelpers]::GetUninitializedObject($stateType)
$creature = [Activator]::CreateInstance($game.GetType('MegaCrit.Sts2.Core.Entities.Creatures.Creature'), @($player, 50, 50))
Set-Field $player '<Creature>k__BackingField' $creature
Set-Field $player '<PlayerCombatState>k__BackingField' $state
Set-Field $state '_player' $player
Set-Field $state '_energy' 3
Set-Field $state '_stars' 5
$pileType = $game.GetType('MegaCrit.Sts2.Core.Entities.Cards.PileType')
$cardPileType = $game.GetType('MegaCrit.Sts2.Core.Entities.Cards.CardPile')
$piles = [Array]::CreateInstance($cardPileType, 5)
$pileNames = @('Hand', 'Draw', 'Discard', 'Exhaust', 'Play')
for ($i = 0; $i -lt 5; $i++) {
    $piles.SetValue([Activator]::CreateInstance($cardPileType, @([Enum]::Parse($pileType, $pileNames[$i]))), $i)
}
Set-Field $state '_piles' $piles
Set-Field $player '_runPiles' ([Array]::CreateInstance($cardPileType, 0))
$handCards = $cardPileType.GetField('_cards', $flags).GetValue($piles[0])
$powerType = $assembly.GetType('TheArchitectCup.Characters.TheArchitectCup.Powers.SupernovaPower')
$power = [Activator]::CreateInstance($powerType)
$power = $power.MutableClone()
Set-Field $power '_owner' $creature
$powers = $creature.GetType().GetField('_powers', $flags).GetValue($creature)
$powers.Add($power)
$costCard = $models['Phase6Supernova'].ToMutable()
$costCard.Owner = $player
$handCards.Add($costCard)
$convertedCost = $powerType.GetMethod('ConvertedCost', $flags)
Assert-Equal $convertedCost.Invoke($null, @($costCard)) 2 'Fixed converted cost'
$hookArgs = @($costCard, [decimal]2, [decimal]-1)
Assert-Equal $powerType.GetMethod('TryModifyEnergyCostInCombatLate').Invoke($power, $hookArgs) $true 'Energy conversion active'
Assert-Equal $hookArgs[2] 0 'Energy cost replaced with zero'
$hookArgs = @($costCard, [decimal]1, [decimal]-1)
[void]$powerType.GetMethod('TryModifyStarCost').Invoke($power, $hookArgs)
Assert-Equal $hookArgs[2] 3 'Native star cost plus converted energy'
$costCard.EnergyCost.SetThisTurnOrUntilPlayed(0, $false)
Assert-Equal $convertedCost.Invoke($null, @($costCard)) 0 'Temporary free cost stays free'
[void]$handCards.Remove($costCard)
$hookArgs = @($costCard, [decimal]2, [decimal]-1)
Assert-Equal $powerType.GetMethod('TryModifyEnergyCostInCombatLate').Invoke($power, $hookArgs) $false 'Non-hand card not converted'

$xCanonical = [Activator]::CreateInstance($game.GetType('MegaCrit.Sts2.Core.Models.Cards.Whirlwind'))
$db.Add($xCanonical.Id, $xCanonical)
$xCard = $xCanonical.ToMutable()
$xCard.Owner = $player
$handCards.Add($xCard)
Assert-Equal $convertedCost.Invoke($null, @($xCard)) 5 'Energy X uses remaining stars'
Set-Field $xCard '_baseStarCost' 2
Set-Field $xCard '_starCostSet' $true
Assert-Equal $convertedCost.Invoke($null, @($xCard)) 3 'Energy X reserves native star cost'
Set-Field $state '_stars' 0
Assert-Equal $convertedCost.Invoke($null, @($xCard)) 0 'Energy X with no stars'
$powers.Clear()
$hookArgs = @($xCard, [decimal]2, [decimal]-1)
Assert-Equal $powerType.GetMethod('TryModifyEnergyCostInCombatLate').Invoke($power, $hookArgs) $false 'Conversion ends when power removed'
Write-Host "Phase 6 model checks passed: $script:assertions assertions. In-game and two-client checks remain separate."
