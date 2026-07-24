using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Unlocks;
using TheArchitectCup.Content;
using TheArchitectCup.Settings;

namespace TheArchitectCup.Patches;

[HarmonyPatch(typeof(CardPoolModel), nameof(CardPoolModel.GetUnlockedCards))]
public static class CardPoolFilterPatch
{
    [HarmonyPriority(Priority.Last)]
    static void Postfix(CardPoolModel __instance, UnlockState unlockState, CardMultiplayerConstraint multiplayerConstraint, ref IEnumerable<CardModel> __result)
    {
        var runState = CardSettingsService.CurrentRunState;
        __result = __result.Where(card =>
        {
            if (!ArchitectCupCardCatalog.TryGet(card.Id.Entry, out ArchitectCupCardDefinition definition) ||
                !definition.Configurable)
                return true;

            return runState == null
                ? CardSettingsService.IsCardEnabled(card.Id.Entry)
                : CardSettingsService.IsCardEnabled(card.Id.Entry, runState);
        });
    }
}
