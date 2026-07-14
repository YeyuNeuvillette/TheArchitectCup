using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;
using MegaCrit.Sts2.Core.HoverTips;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class CouplingPower : BasePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Evoke),
    ];

    public override async Task AfterOrbEvoked(PlayerChoiceContext choiceContext, OrbModel orb, IEnumerable<Creature> targets)
    {
        if (orb.Owner == Owner.Player && Owner.Player.PlayerCombatState != null)
        {
            Flash();
            for (int i = 0; i < Amount; i++)
            {
                foreach (OrbModel orbModel in Owner.Player.PlayerCombatState.OrbQueue.Orbs)
                {
                    await OrbCmd.Passive(choiceContext, orbModel, null);
                }
            }
        }
    }
}