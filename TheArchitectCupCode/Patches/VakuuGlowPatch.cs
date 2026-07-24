using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using TheArchitectCup.Characters.TheArchitectCup.Powers;

namespace TheArchitectCup.Characters.TheArchitectCup.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.ShouldGlowGold), MethodType.Getter)]
public static class VakuuGlowPatch
{
    [HarmonyPriority(Priority.Last)]
    public static void Postfix(CardModel __instance, ref bool __result)
    {
        if (__result)
            return;

        var player = __instance.Owner;
        if (player == null)
            return;

        var power = player.Creature?.GetPower<VakuuTeachesUToPlayPower>();
        if (power == null)
            return;

        if (power.ShouldGlowForVakuu(__instance))
            __result = true;
    }
}
