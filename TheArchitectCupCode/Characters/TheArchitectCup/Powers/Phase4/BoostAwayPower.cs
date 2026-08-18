using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;

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
            int num = player.RunState.Rng.Niche.NextInt(3);
            // TODO
        }
    }

    
}
