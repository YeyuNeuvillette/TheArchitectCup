using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using TheArchitectCup.Characters.TheArchitectCup.Powers;

namespace TheArchitectCup.Patches;

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
internal static class Emc2CanPlayPatch
{
    [HarmonyPostfix]
    private static void Postfix(PlayerCombatState __instance, CardModel card, ref UnplayableReason reason, ref bool __result)
    {
        Emc2Power? power = card.Owner.Creature.GetPower<Emc2Power>();
        if (power == null || !power.CanConvert(card))
            return;

        reason &= ~UnplayableReason.EnergyCostTooHigh;

        // Ignore the obsolete excess-energy-to-stars substitution. An intrinsic star cost still has to be paid.
        if (Math.Max(0, card.GetStarCostWithModifiers()) <= __instance.Stars)
            reason &= ~UnplayableReason.StarCostTooHigh;

        __result = reason == UnplayableReason.None;
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
internal static class Emc2SpendResourcesPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardModel __instance, ref Task<(int, int)> __result)
    {
        Emc2Power? power = __instance.Owner.Creature.GetPower<Emc2Power>();
        if (power == null || !power.CanConvert(__instance))
            return true;

        __result = power.SpendWithConversion(__instance);
        return false;
    }
}
