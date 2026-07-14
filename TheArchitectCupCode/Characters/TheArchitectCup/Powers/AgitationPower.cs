using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class AgitationPower : BasePower
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner.Creature != Owner || Owner.Player == null)
        {
            return true;
        }
        if (autoPlayType != AutoPlayType.None)
        {
            return true;
        }
        return card.EnergyCost.GetResolved() >= PileType.Hand.GetPile(Owner.Player).Cards.Max(c => c.EnergyCost.GetResolved());
    }
}