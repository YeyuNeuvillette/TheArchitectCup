using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using TheArchitectCup.Features.Rewards;

namespace TheArchitectCup.Patches;

[HarmonyPatch]
internal static class BoostAwayRewardToSerializablePatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(GoldReward), nameof(Reward.ToSerializable));
        yield return AccessTools.Method(typeof(CardReward), nameof(Reward.ToSerializable));
        yield return AccessTools.Method(typeof(RelicReward), nameof(Reward.ToSerializable));
    }

    [HarmonyPostfix]
    private static void Postfix(Reward __instance, ref SerializableReward __result)
    {
        if (BoostAwayRewardService.IsMarked(__instance) &&
            __instance.Player.RunState.CurrentRoom is CombatRoom combatRoom)
        {
            __result.CustomDescriptionEncounterSourceId = combatRoom.Encounter.Id;
        }
    }
}

[HarmonyPatch(typeof(Reward), nameof(Reward.FromSerializable))]
internal static class BoostAwayRewardFromSerializablePatch
{
    [HarmonyPostfix]
    private static void Postfix(SerializableReward save, Reward __result)
    {
        if (save.CustomDescriptionEncounterSourceId != ModelId.none &&
            __result is GoldReward or CardReward or RelicReward)
        {
            BoostAwayRewardService.MarkReward(__result);
        }
    }
}
