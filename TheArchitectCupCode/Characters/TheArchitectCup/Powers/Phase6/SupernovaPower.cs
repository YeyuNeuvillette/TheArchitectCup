using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class SupernovaPower : BasePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    [ThreadStatic]
    private static int _readingOriginalCost;

    internal static bool Converts(CardModel card) =>
        _readingOriginalCost == 0 && !card.IsCanonical &&
        card.Owner.PlayerCombatState != null && card.Pile?.Type == PileType.Hand &&
        card.Owner.Creature.GetPower<SupernovaPower>() != null;

    internal static int ConvertedCost(CardModel card)
    {
        _readingOriginalCost++;
        try
        {
            if (card.EnergyCost.CostsX)
            {
                int nativeStars = card.HasStarCostX ? 0 : Math.Max(0, card.GetStarCostWithModifiers());
                return Math.Max(0, (card.Owner.PlayerCombatState?.Stars ?? 0) - nativeStars);
            }
            return Math.Max(0, card.EnergyCost.GetWithModifiers(CostModifiers.All));
        }
        finally { _readingOriginalCost--; }
    }

    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner || !Converts(card)) return false;
        modifiedCost = 0;
        return true;
    }

    public override bool TryModifyStarCost(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner || !Converts(card)) return false;
        modifiedCost = Math.Max(0, originalCost) + ConvertedCost(card);
        return true;
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
        {
            return;
        }
        await PowerCmd.Remove(this);
    }
}
