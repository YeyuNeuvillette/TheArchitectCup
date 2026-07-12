using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib;
using STS2RitsuLib.Data;
using STS2RitsuLib.RunData;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils.Persistence;

namespace TheArchitectCup.Settings;

public sealed class CardSettingsData
{
    public bool CursedHandkerchiefEnabled { get; set; } = true;
    public bool ThreeLeggedRaceEnabled { get; set; } = true;
    public bool PingPongEnabled { get; set; } = true;
    public bool GenerousDonationEnabled { get; set; } = true;
    public bool FindSomeoneToGetYouEnabled { get; set; } = true;
    public bool EarthShatteringEnabled { get; set; } = true;
    public bool BurnTheMountainEnabled { get; set; } = true;
    public bool BeeBroOnTheRunEnabled { get; set; } = true;
}

public static class CardSettingsPage
{
    private const string DataKey = "card_settings";
    private const string RunSavedDataKey = "card_settings_sync";
    private const string Table = "settings_ui";

    private static readonly ModSettingsValueBinding<CardSettingsData, bool> CursedHandkerchiefBinding = new(
        MainFile.ModId, DataKey, SaveScope.Global,
        static s => s.CursedHandkerchiefEnabled,
        static (s, v) => s.CursedHandkerchiefEnabled = v);

    private static readonly ModSettingsValueBinding<CardSettingsData, bool> ThreeLeggedRaceBinding = new(
        MainFile.ModId, DataKey, SaveScope.Global,
        static s => s.ThreeLeggedRaceEnabled,
        static (s, v) => s.ThreeLeggedRaceEnabled = v);

    private static readonly ModSettingsValueBinding<CardSettingsData, bool> PingPongBinding = new(
        MainFile.ModId, DataKey, SaveScope.Global,
        static s => s.PingPongEnabled,
        static (s, v) => s.PingPongEnabled = v);

    private static readonly ModSettingsValueBinding<CardSettingsData, bool> GenerousDonationBinding = new(
        MainFile.ModId, DataKey, SaveScope.Global,
        static s => s.GenerousDonationEnabled,
        static (s, v) => s.GenerousDonationEnabled = v);

    private static readonly ModSettingsValueBinding<CardSettingsData, bool> FindSomeoneToGetYouBinding = new(
        MainFile.ModId, DataKey, SaveScope.Global,
        static s => s.FindSomeoneToGetYouEnabled,
        static (s, v) => s.FindSomeoneToGetYouEnabled = v);

    private static readonly ModSettingsValueBinding<CardSettingsData, bool> EarthShatteringBinding = new(
        MainFile.ModId, DataKey, SaveScope.Global,
        static s => s.EarthShatteringEnabled,
        static (s, v) => s.EarthShatteringEnabled = v);

    private static readonly ModSettingsValueBinding<CardSettingsData, bool> BurnTheMountainBinding = new(
        MainFile.ModId, DataKey, SaveScope.Global,
        static s => s.BurnTheMountainEnabled,
        static (s, v) => s.BurnTheMountainEnabled = v);

    private static readonly ModSettingsValueBinding<CardSettingsData, bool> BeeBroOnTheRunBinding = new(
        MainFile.ModId, DataKey, SaveScope.Global,
        static s => s.BeeBroOnTheRunEnabled,
        static (s, v) => s.BeeBroOnTheRunEnabled = v);

    private static readonly RunSavedData<CardSettingsData> RunSavedCardSettings =
        RunSavedDataStore.For(MainFile.ModId).Register<CardSettingsData>(
            key: RunSavedDataKey,
            defaultFactory: () => new CardSettingsData(),
            options: new RunSavedDataOptions { WritePolicy = RunSavedDataWritePolicy.AlwaysWhenRegistered });

    public static void Register()
    {
        ModDataStore.For(MainFile.ModId).Register<CardSettingsData>(
            key: DataKey,
            fileName: "card_settings.json",
            scope: SaveScope.Global,
            defaultFactory: () => new CardSettingsData(),
            autoCreateIfMissing: true);

        RitsuLibFramework.RegisterModSettings(MainFile.ModId, page => page
            .WithTitle(ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_SETTINGS_TITLE", "The Architect Cup"))
            .WithModDisplayName(ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_MOD_DISPLAY_NAME", "The Architect Cup"))
            .WithVisibleOnHostSurfaces(
                ModSettingsHostSurface.MainMenu | ModSettingsHostSurface.RunPause)
            .AddSection("phase1", section => section
                .WithTitle(ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_SECTION_PHASE1", "Phase 1"))
                .AddToggle("cursed_handkerchief",
                    ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_CARD_CURSED_HANDKERCHIEF_TOGGLE", "Cursed Handkerchief"),
                    CursedHandkerchiefBinding,
                    description: ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_CARD_CURSED_HANDKERCHIEF_AUTHOR", "Author: 罡璧"))
                .AddToggle("three_legged_race",
                    ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_CARD_THREE_LEGGED_RACE_TOGGLE", "Three Legged Race"),
                    ThreeLeggedRaceBinding,
                    description: ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_CARD_THREE_LEGGED_RACE_AUTHOR", "Author: 六方最密堆积"))
                .AddToggle("ping_pong",
                    ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_CARD_PING_PONG_TOGGLE", "Ping Pong"),
                    PingPongBinding,
                    description: ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_CARD_PING_PONG_AUTHOR", "Author: 小肥"))
                .AddToggle("generous_donation",
                    ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_CARD_GENEROUS_DONATION_TOGGLE", "Generous Donation"),
                    GenerousDonationBinding,
                    description: ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_CARD_GENEROUS_DONATION_AUTHOR", "Author: SkyF"))
                .AddToggle("find_someone_to_get_you",
                    ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_CARD_FIND_SOMEONE_TO_GET_YOU_TOGGLE", "Find Someone to Get You"),
                    FindSomeoneToGetYouBinding,
                    description: ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_CARD_FIND_SOMEONE_TO_GET_YOU_AUTHOR", "Author: 花盆上屹立的不明食草兽"))
                .AddToggle("earth_shattering",
                    ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_CARD_EARTH_SHATTERING_TOGGLE", "Earth Shattering"),
                    EarthShatteringBinding,
                    description: ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_CARD_EARTH_SHATTERING_AUTHOR", "Author: absi2011"))
                .AddToggle("burn_the_mountain",
                    ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_CARD_BURN_THE_MOUNTAIN_TOGGLE", "Burn the Mountain"),
                    BurnTheMountainBinding,
                    description: ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_CARD_BURN_THE_MOUNTAIN_AUTHOR", "Author: BlueGhost"))
                .AddToggle("bee_bro_on_the_run",
                    ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_CARD_BEE_BRO_ON_THE_RUN_TOGGLE", "Bee Bro On the Run"),
                    BeeBroOnTheRunBinding,
                    description: ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_CARD_BEE_BRO_ON_THE_RUN_AUTHOR", "Author: 阿迪"))));
    }

    public static bool IsCardEnabled(string cardId) => cardId switch
    {
        "THE_ARCHITECT_CUP_CARD_CURSED_HANDKERCHIEF" => CursedHandkerchiefBinding.Read(),
        "THE_ARCHITECT_CUP_CARD_THREE_LEGGED_RACE" => ThreeLeggedRaceBinding.Read(),
        "THE_ARCHITECT_CUP_CARD_PING_PONG" => PingPongBinding.Read(),
        "THE_ARCHITECT_CUP_CARD_GENEROUS_DONATION" => GenerousDonationBinding.Read(),
        "THE_ARCHITECT_CUP_CARD_FIND_SOMEONE_TO_GET_YOU" => FindSomeoneToGetYouBinding.Read(),
        "THE_ARCHITECT_CUP_CARD_EARTH_SHATTERING" => EarthShatteringBinding.Read(),
        "THE_ARCHITECT_CUP_CARD_BURN_THE_MOUNTAIN" => BurnTheMountainBinding.Read(),
        "THE_ARCHITECT_CUP_CARD_BEE_BRO_ON_THE_RUN" => BeeBroOnTheRunBinding.Read(),
        _ => true
    };

    public static bool IsCardEnabled(string cardId, RunState runState)
    {
        if (RunSavedCardSettings.TryGet(runState, out var synced))
            return IsCardEnabledFromData(cardId, synced);

        return IsCardEnabled(cardId);
    }

    private static bool IsCardEnabledFromData(string cardId, CardSettingsData data) => cardId switch
    {
        "THE_ARCHITECT_CUP_CARD_CURSED_HANDKERCHIEF" => data.CursedHandkerchiefEnabled,
        "THE_ARCHITECT_CUP_CARD_THREE_LEGGED_RACE" => data.ThreeLeggedRaceEnabled,
        "THE_ARCHITECT_CUP_CARD_PING_PONG" => data.PingPongEnabled,
        "THE_ARCHITECT_CUP_CARD_GENEROUS_DONATION" => data.GenerousDonationEnabled,
        "THE_ARCHITECT_CUP_CARD_FIND_SOMEONE_TO_GET_YOU" => data.FindSomeoneToGetYouEnabled,
        "THE_ARCHITECT_CUP_CARD_EARTH_SHATTERING" => data.EarthShatteringEnabled,
        "THE_ARCHITECT_CUP_CARD_BURN_THE_MOUNTAIN" => data.BurnTheMountainEnabled,
        "THE_ARCHITECT_CUP_CARD_BEE_BRO_ON_THE_RUN" => data.BeeBroOnTheRunEnabled,
        _ => true
    };

    public static void SyncLocalSettingsToRunState(RunState runState)
    {
        var data = new CardSettingsData
        {
            CursedHandkerchiefEnabled = CursedHandkerchiefBinding.Read(),
            ThreeLeggedRaceEnabled = ThreeLeggedRaceBinding.Read(),
            PingPongEnabled = PingPongBinding.Read(),
            GenerousDonationEnabled = GenerousDonationBinding.Read(),
            FindSomeoneToGetYouEnabled = FindSomeoneToGetYouBinding.Read(),
            EarthShatteringEnabled = EarthShatteringBinding.Read(),
            BurnTheMountainEnabled = BurnTheMountainBinding.Read(),
            BeeBroOnTheRunEnabled = BeeBroOnTheRunBinding.Read(),
        };
        RunSavedCardSettings.Set(runState, data);
    }

    public static bool HasSyncedSettings(RunState runState)
    {
        return RunSavedCardSettings.TryGet(runState, out _);
    }
}