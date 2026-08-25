using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace TheArchitectCup.Features.Rewards;

internal static class BoostAwayRewardService
{
    private sealed class RewardMarker;

    private static readonly ConditionalWeakTable<Reward, RewardMarker> MarkedRewards = new();

    internal static void SetReward(CombatRoom room, Player player, Reward reward)
    {
        MarkReward(reward);
        room.AddExtraReward(player, reward);
    }

    internal static void MarkReward(Reward reward)
    {
        MarkedRewards.GetValue(reward, static _ => new RewardMarker());
    }

    internal static bool IsMarked(Reward reward) => MarkedRewards.TryGetValue(reward, out _);

    internal static bool TryTakeReward(CombatRoom room, Player player, out Reward reward)
    {
        reward = null!;
        if (!room.ExtraRewards.TryGetValue(player, out List<Reward>? rewards))
            return false;

        Reward? pendingReward = rewards.FirstOrDefault(IsMarked);

        // Compatibility for saves created before Boost Away rewards carried a
        // persistent marker. Escaping every enemy produces zero gold proportion.
        if (pendingReward is null &&
            room.IsPreFinished &&
            room.GoldProportion <= 0f &&
            rewards.Count == 1 &&
            rewards[0] is GoldReward or CardReward or RelicReward)
        {
            pendingReward = rewards[0];
        }

        if (pendingReward is null)
            return false;

        reward = pendingReward;
        MarkedRewards.Remove(reward);
        return true;
    }
}
