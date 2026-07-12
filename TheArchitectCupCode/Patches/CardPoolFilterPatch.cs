using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Unlocks;
using TheArchitectCup.Settings;

namespace TheArchitectCup.Patches;

[HarmonyPatch(typeof(CardPoolModel), nameof(CardPoolModel.GetUnlockedCards))]
public static class CardPoolFilterPatch
{
    private static readonly HashSet<string> ManagedCardIds =
    [
        "THE_ARCHITECT_CUP_CARD_CURSED_HANDKERCHIEF",
        "THE_ARCHITECT_CUP_CARD_THREE_LEGGED_RACE",
        "THE_ARCHITECT_CUP_CARD_PING_PONG",
        "THE_ARCHITECT_CUP_CARD_GENEROUS_DONATION",
        "THE_ARCHITECT_CUP_CARD_FIND_SOMEONE_TO_GET_YOU",
        "THE_ARCHITECT_CUP_CARD_EARTH_SHATTERING",
        "THE_ARCHITECT_CUP_CARD_BURN_THE_MOUNTAIN",
        "THE_ARCHITECT_CUP_CARD_BEE_BRO_ON_THE_RUN"
    ];

    static void Postfix(CardPoolModel __instance, UnlockState unlockState, CardMultiplayerConstraint multiplayerConstraint, ref IEnumerable<CardModel> __result)
    {
        if (multiplayerConstraint == CardMultiplayerConstraint.MultiplayerOnly)
        {
            var runState = RunManager.Instance.DebugOnlyGetState();
            if (runState != null)
            {
                __result = __result.Where(c => !ManagedCardIds.Contains(c.Id.Entry) || CardSettingsPage.IsCardEnabled(c.Id.Entry, runState));
                return;
            }
        }

        __result = __result.Where(c => !ManagedCardIds.Contains(c.Id.Entry) || CardSettingsPage.IsCardEnabled(c.Id.Entry));
    }
}