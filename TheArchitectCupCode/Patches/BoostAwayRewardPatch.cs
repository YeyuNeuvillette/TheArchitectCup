using HarmonyLib;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using TheArchitectCup.Features.Rewards;

namespace TheArchitectCup.Patches;

[HarmonyPatch(typeof(RewardsSet), nameof(RewardsSet.WithRewardsFromRoom))]
internal static class BoostAwayRewardPatch
{
    [HarmonyPrefix]
    private static bool Prefix(RewardsSet __instance, AbstractRoom room, ref RewardsSet __result)
    {
        if (room is not CombatRoom combatRoom ||
            !BoostAwayRewardService.TryTakeReward(combatRoom, __instance.Player, out Reward reward))
        {
            return true;
        }

        __result = __instance.EmptyForRoom(room).WithCustomRewards([reward]);
        return false;
    }
}
