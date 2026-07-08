using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using TheArchitectCup.Characters.TheArchitectCup.Cards;

namespace TheArchitectCup.Characters.TheArchitectCup.Patches;

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterDeath))]
public static class EntomancerBeeBroRewardPatch
{
    private const float DropChance = 0.6f;

    public static void Postfix(AbstractModel __instance, PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (__instance is not Entomancer entomancer)
            return;

        if (entomancer.CombatState == null || entomancer.Creature == null)
            return;

        bool allEnemiesDead = entomancer.CombatState.HittableEnemies.All(e => e.IsDead);
        if (!allEnemiesDead)
            return;

        foreach (Player player in entomancer.CombatState.RunState.Players)
        {
            if (player.RunState.Rng.CombatCardGeneration.NextFloat() < DropChance)
            {
                CardModel beeBro = player.RunState.CreateCard<BeeBroOnTheRun>(player);
                SpecialCardReward reward = new SpecialCardReward(beeBro, player);
                reward.SetCustomDescriptionEncounterSource(ModelDb.Encounter<EntomancerElite>().Id);
                if (player.RunState.CurrentRoom is CombatRoom combatRoom)
                    combatRoom.AddExtraReward(player, reward);
            }
        }
    }
}