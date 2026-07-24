using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib;
using TheArchitectCup.Characters.TheArchitectCup.Cards;

namespace TheArchitectCup.Features.Rewards;

internal static class BeeBroRewardService
{
    private const float DropChance = 0.6f;
    private static readonly HashSet<Creature> RewardedEntomancers = [];

    internal static void Register()
    {
        RitsuLibFramework.SubscribeLifecycle<CreatureDiedEvent>(OnCreatureDied, replayCurrentState: false);
        RitsuLibFramework.SubscribeLifecycle<CombatEndedEvent>(_ => RewardedEntomancers.Clear(), replayCurrentState: false);
    }

    private static void OnCreatureDied(CreatureDiedEvent evt)
    {
        if (evt.WasRemovalPrevented || evt.Creature.Monster is not Entomancer)
            return;

        if (evt.CombatState == null || evt.RunState.CurrentRoom is not CombatRoom combatRoom)
            return;

        if (evt.CombatState.HittableEnemies.Any(static enemy => !enemy.IsDead))
            return;

        if (!RewardedEntomancers.Add(evt.Creature))
            return;

        foreach (Player player in evt.CombatState.Players.OrderBy(static player => player.NetId))
        {
            if (player.RunState.Rng.CombatCardGeneration.NextFloat() >= DropChance)
                continue;

            CardModel beeBro = player.RunState.CreateCard<BeeBroOnTheRun>(player);
            var reward = new SpecialCardReward(beeBro, player);
            reward.SetCustomDescriptionEncounterSource(ModelDb.Encounter<EntomancerElite>().Id);
            combatRoom.AddExtraReward(player, reward);
        }
    }
}
