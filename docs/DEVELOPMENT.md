# Development

## Layout

- `TheArchitectCupCode/Api`: supported API for other mods.
- `TheArchitectCupCode/Content`: canonical metadata shared by settings and card pools.
- `TheArchitectCupCode/Features`: runtime services that do not patch game methods.
- `TheArchitectCupCode/Patches`: the two remaining Harmony postfixes.
- `TheArchitectCupCode/Compatibility/Legacy`: obsolete source-compatible forwarding APIs.
- `TheArchitectCup/localization`: English, Japanese, and Simplified Chinese game tables.

## External artwork

Artwork remains out of Git and must be placed under `TheArchitectCup/images`. Required paths and dimensions are
defined in `assets/required-assets.json`. Godot `.import` files are generated locally and are not source assets.

Validate artwork before export:

```powershell
pwsh -NoProfile -File .\tools\Validate-Assets.ps1
```

PCK export runs this check automatically. A normal C# build does not require external artwork.

## Validation

```powershell
pwsh -NoProfile -File .\tools\Validate-Localization.ps1
dotnet build --no-restore -p:RunPckExport=false
dotnet build --no-restore -c Release -p:RunPckExport=false
```

Before release, run a two-client session with different local card settings and verify host settings win. Exercise
Three-Legged Race, Ping Pong, Burn the Mountain with another character mod, Entomancer rewards, full-hand card
transfers, Coupling with a changing orb queue, and Vakuu with unplayable cards between valid cards.
