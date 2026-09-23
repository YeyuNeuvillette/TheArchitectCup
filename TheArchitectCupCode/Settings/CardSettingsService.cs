using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib;
using STS2RitsuLib.Data;
using STS2RitsuLib.RunData;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils.Persistence;
using TheArchitectCup.Content;
using TheArchitectCup.Infrastructure;

namespace TheArchitectCup.Settings;

internal static class CardSettingsService
{
    private const string DataKey = "card_settings";
    private const string RunSavedDataKey = "card_settings_sync";
    private const string Table = "settings_ui";

    private static readonly IReadOnlyDictionary<string, ModSettingsValueBinding<CardSettingsData, bool>> Bindings =
        ArchitectCupCardCatalog.ConfigurableCards.ToDictionary(
            definition => definition.Id,
            definition => new ModSettingsValueBinding<CardSettingsData, bool>(
                MainFile.ModId,
                DataKey,
                SaveScope.Global,
                definition.ReadEnabled!,
                definition.WriteEnabled!),
            StringComparer.Ordinal);

    private static readonly RunSavedData<CardSettingsData> RunSavedCardSettings =
        RunSavedDataStore.For(MainFile.ModId).Register<CardSettingsData>(
            key: RunSavedDataKey,
            defaultFactory: static () => new CardSettingsData(),
            options: new RunSavedDataOptions { WritePolicy = RunSavedDataWritePolicy.AlwaysWhenRegistered });

    internal static RunState? CurrentRunState { get; private set; }

    internal static void Register()
    {
        ModDataStore.For(MainFile.ModId).Register<CardSettingsData>(
            key: DataKey,
            fileName: "card_settings.json",
            scope: SaveScope.Global,
            defaultFactory: static () => new CardSettingsData(),
            autoCreateIfMissing: true);

        RitsuLibFramework.RegisterModSettings(MainFile.ModId, page =>
        {
            page.WithTitle(ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_SETTINGS_TITLE", "The Architect Cup"))
                .WithModDisplayName(ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_MOD_DISPLAY_NAME", "The Architect Cup"))
                .WithVisibleOnHostSurfaces(ModSettingsHostSurface.MainMenu | ModSettingsHostSurface.RunPause);

            AddPhaseSection(page, 1);
            AddPhaseSection(page, 2);
            AddPhaseSection(page, 3);
            AddPhaseSection(page, 4);
            AddPhaseSection(page, 5);
        });
    }

    internal static void OnRunStarted(RunState runState)
    {
        CurrentRunState = runState;
        if (GameCompatibility.IsRunAuthority())
            SyncLocalSettingsToRunState(runState);
    }

    internal static void OnRunLoaded(RunState runState, bool isMultiplayer)
    {
        CurrentRunState = runState;
        if (!GameCompatibility.IsRunAuthority())
            return;

        if (!isMultiplayer || !HasSyncedSettings(runState))
            SyncLocalSettingsToRunState(runState);
    }

    internal static void OnRunEnded() => CurrentRunState = null;

    internal static bool IsCardEnabled(string cardId)
    {
        if (!ArchitectCupCardCatalog.TryGet(cardId, out ArchitectCupCardDefinition definition) ||
            !definition.Configurable)
            return true;

        return Bindings[definition.Id].Read();
    }

    internal static bool IsCardEnabled(string cardId, RunState runState)
    {
        if (!ArchitectCupCardCatalog.TryGet(cardId, out ArchitectCupCardDefinition definition) ||
            !definition.Configurable)
            return true;

        return RunSavedCardSettings.TryGet(runState, out CardSettingsData? synced)
            ? definition.ReadEnabled!(synced)
            : IsCardEnabled(cardId);
    }

    internal static void SyncLocalSettingsToRunState(RunState runState)
    {
        var snapshot = new CardSettingsData();
        foreach (ArchitectCupCardDefinition definition in ArchitectCupCardCatalog.ConfigurableCards)
            definition.WriteEnabled!(snapshot, Bindings[definition.Id].Read());

        RunSavedCardSettings.Set(runState, snapshot);
    }

    internal static bool HasSyncedSettings(RunState runState) =>
        RunSavedCardSettings.TryGet(runState, out _);

    private static void AddPhaseSection(ModSettingsPageBuilder page, int phase)
    {
        string sectionId = $"phase{phase}";
        string sectionKey = $"THE_ARCHITECT_CUP_SECTION_PHASE{phase}";
        page.AddSection(sectionId, section =>
        {
            ArchitectCupCardDefinition[] phaseCards = ArchitectCupCardCatalog.ConfigurableCards
                .Where(definition => definition.Phase == phase)
                .ToArray();

            section.WithTitle(ModSettingsText.LocString(Table, sectionKey, $"Phase {phase}"))
                .Collapsible(startCollapsed: true);
            section.AddButton(
                $"{sectionId}_enable_all",
                ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_BULK_ACTIONS", "Bulk actions"),
                ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_ENABLE_ALL", "Enable all"),
                host => SetPhaseEnabled(phaseCards, true, host),
                ModSettingsButtonTone.Accent,
                ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_ENABLE_ALL_DESCRIPTION", "Enable every card in this phase."));
            section.AddButton(
                $"{sectionId}_disable_all",
                ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_BULK_ACTIONS", "Bulk actions"),
                ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_DISABLE_ALL", "Disable all"),
                host => SetPhaseEnabled(phaseCards, false, host),
                ModSettingsButtonTone.Normal,
                ModSettingsText.LocString(Table, "THE_ARCHITECT_CUP_DISABLE_ALL_DESCRIPTION", "Disable every card in this phase."));

            foreach (ArchitectCupCardDefinition definition in phaseCards)
            {
                section.AddToggle(
                    definition.SettingId!,
                    ModSettingsText.LocString(Table, $"{definition.Id}_TOGGLE", definition.EnglishTitle),
                    Bindings[definition.Id],
                    description: ModSettingsText.LocString(
                        Table,
                        $"{definition.Id}_AUTHOR",
                        $"Disabling this card will prevent it from appearing in card pools. Author: {definition.Author}"));
            }
        });
    }

    private static void SetPhaseEnabled(
        IEnumerable<ArchitectCupCardDefinition> phaseCards,
        bool enabled,
        IModSettingsUiActionHost host)
    {
        foreach (ArchitectCupCardDefinition definition in phaseCards)
        {
            ModSettingsValueBinding<CardSettingsData, bool> binding = Bindings[definition.Id];
            if (binding.Read() == enabled)
                continue;

            binding.Write(enabled);
            host.MarkDirty(binding);
        }

        if (CurrentRunState is not null && GameCompatibility.IsRunAuthority())
            SyncLocalSettingsToRunState(CurrentRunState);

        host.RequestRefreshAfterDataModelBatchChange();
    }
}
