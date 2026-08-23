using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Logging;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class DeadLockPower : BasePower
{
    public class Data
    {
        public Player? player;
    }

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new StringVar("Player")];

    protected override object InitInternalData()
    {
        return new Data();
    }

    public void SetPlayer(Player player)
    {
        GetInternalData<Data>().player = player;
        ((StringVar)DynamicVars["Player"]).StringValue = player.Creature.Name;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != GetInternalData<Data>().player)
        {
            return;
        }
        if (Owner.Monster == null)
        {
            if(Owner.Player == null)
            {
                Log.Error(Owner.Name +" is neither Monster nor Player wtf");
                return;
            }
            else
            {
                PlayerCmd.EndTurn(Owner.Player, false);
            }
        }
        else
        {
            await CreatureCmd.Stun(Owner);
        }
        await PowerCmd.Decrement(this);
    }
}
