using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using TheArchitectCup.Characters.TheArchitectCup.Powers;

namespace TheArchitectCup.Patches;

// X-cost bypasses vanilla's energy hooks. Adapt its payment and captured value;
// fixed costs continue to use the standard energy/star cost hooks.
[HarmonyPatch(typeof(CardEnergyCost), nameof(CardEnergyCost.GetAmountToSpend))]
internal static class SupernovaXPaymentPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardEnergyCost __instance, CardModel ____card, ref int __result)
    {
        if (__instance.CostsX && SupernovaPower.Converts(____card))
            __result = 0;
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.CurrentStarCost), MethodType.Getter)]
internal static class SupernovaStarDisplayPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel __instance, ref int __result)
    {
        // Vanilla hides the star icon when the native star cost is -1.
        if (__result < 0 && SupernovaPower.Converts(__instance))
            __result = 0;
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
internal static class SupernovaCaptureXPatch
{
    [HarmonyPrefix]
    private static void Prefix(CardModel __instance, out int? __state)
    {
        __state = __instance.EnergyCost.CostsX && SupernovaPower.Converts(__instance)
            ? SupernovaPower.ConvertedCost(__instance) : null;
    }

    [HarmonyPostfix]
    private static void Postfix(CardModel __instance, int? __state, ref Task<(int, int)> __result)
    {
        if (__state.HasValue)
            __result = CaptureAfterPayment(__instance, __state.Value, __result);
    }

    private static async Task<(int, int)> CaptureAfterPayment(CardModel card, int x, Task<(int, int)> payment)
    {
        var spent = await payment;
        card.EnergyCost.CapturedXValue = Math.Min(x, Math.Max(0, spent.Item2));
        return spent;
    }
}
