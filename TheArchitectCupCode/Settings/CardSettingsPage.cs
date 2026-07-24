using MegaCrit.Sts2.Core.Runs;

namespace TheArchitectCup.Settings;

public static class CardSettingsPage
{
    public static void Register() => CardSettingsService.Register();

    [Obsolete("Use ArchitectCupApi.IsCardEnabled instead.")]
    public static bool IsCardEnabled(string cardId) => CardSettingsService.IsCardEnabled(cardId);

    [Obsolete("Use ArchitectCupApi.IsCardEnabled instead.")]
    public static bool IsCardEnabled(string cardId, RunState runState) =>
        CardSettingsService.IsCardEnabled(cardId, runState);

    public static void SyncLocalSettingsToRunState(RunState runState) =>
        CardSettingsService.SyncLocalSettingsToRunState(runState);

    public static bool HasSyncedSettings(RunState runState) =>
        CardSettingsService.HasSyncedSettings(runState);
}
