using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class PressForwardPower : BasePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromCard<SovereignBlade>()
    ];

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner.Creature != Owner || Owner.Player == null)
        {
            return true;
        }

        if (card is SovereignBlade)
        {
            return true;
        }

        if (autoPlayType != AutoPlayType.None)
        {
            return true;
        }

        return !PileType.Hand.GetPile(Owner.Player).Cards.Any(card => card is SovereignBlade);
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (Owner.Player is { } player)
        {
            foreach (var blade in player.PlayerCombatState!.AllCards.OfType<SovereignBlade>().Where(b => !b.IsUpgraded))
            {
                CardCmd.Upgrade(blade);
                CardCmd.Preview(blade);
            }
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card.Owner.Creature == Owner && card is SovereignBlade blade && !blade.IsUpgraded)
        {
            CardCmd.Upgrade(blade);
            CardCmd.Preview(blade);
        }
        return Task.CompletedTask;
    }
}