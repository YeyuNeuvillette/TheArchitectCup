using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class HauntingPower : BasePower
{
    private class Data
    {
        // Please refer to NightmarePower.
        public CardModel? card;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new StringVar("Card")];

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player == Owner.Player)
        {
            CardModel? card = GetInternalData<Data>().card;
            if (card == null)
            {
                await PowerCmd.Remove(this);
                return;
            }
            for (int i = 0; i < Amount; i++)
            {
                CardModel card2 = card.CreateClone();
                await CardPileCmd.AddGeneratedCardToCombat(card2, PileType.Hand, Owner.Player);
            }
            await PowerCmd.Remove(this);
        }
    }

    public void SetCard(CardModel card)
    {
        CardModel cardModel = card.CreateClone();
        CardCmd.ClearAffliction(cardModel);
        GetInternalData<Data>().card = cardModel;
        ((StringVar)DynamicVars["Card"]).StringValue = cardModel.Title;
    }
}
