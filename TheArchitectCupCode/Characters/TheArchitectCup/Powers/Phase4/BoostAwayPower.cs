using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;
using TheArchitectCup.Features.Rewards;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class BoostAwayPower : BasePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }
        if (Amount > 1)
        {
            await PowerCmd.Decrement(this);
            return;
        }
        AbstractRoom? currentRoom = player.RunState.CurrentRoom;
        if (currentRoom is CombatRoom combatRoom)
        {
            if (CombatManager.Instance.IsOverOrEnding)
                return;

            Reward reward = player.RunState.Rng.Niche.NextInt(3) switch
            {
                0 => new GoldReward(combatRoom.Encounter.MinGoldReward, combatRoom.Encounter.MaxGoldReward, player),
                1 => new CardReward(
                    CardCreationOptions.ForRoom(player, combatRoom.RoomType)
                        .WithFlags(CardCreationFlags.IsFromCombat),
                    3,
                    player),
                _ => new RelicReward(player),
            };

            BoostAwayRewardService.SetReward(combatRoom, player, reward);
            Flash();
            await PowerCmd.Remove(this);

            foreach (var enemy in combatRoom.CombatState.Enemies.Where(static enemy => enemy.IsAlive).ToList())
                await CreatureCmd.Escape(enemy, removeCreatureNode: true);

            await CombatManager.Instance.CheckWinCondition();
        }
    }
}
