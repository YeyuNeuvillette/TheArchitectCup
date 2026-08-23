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
        public readonly List<CardBatch> cards = [];
    }

    private class CardBatch
    {
        public CardModel? card;
        public int count;
    }

    public override PowerType Type => PowerType.Buff;

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
            List<CardBatch> cards = GetInternalData<Data>().cards;
            if (cards.Count == 0)
            {
                await PowerCmd.Remove(this);
                return;
            }

            int remaining = Amount;
            foreach (CardBatch batch in cards)
            {
                if (batch.card == null || batch.count <= 0 || remaining <= 0)
                {
                    continue;
                }

                int count = Math.Min(batch.count, remaining);
                for (int i = 0; i < count; i++)
                {
                    await CardPileCmd.AddGeneratedCardToCombat(batch.card.CreateClone(), PileType.Hand, Owner.Player);
                }

                remaining -= count;
            }

            await PowerCmd.Remove(this);
        }
    }

    public void SetCard(CardModel card)
    {
        Data data = GetInternalData<Data>();
        data.cards.Clear();
        AddCard(card, Amount);
    }

    internal void AddCard(CardModel card, int count)
    {
        if (count <= 0)
        {
            return;
        }

        CardModel cardModel = card.CreateClone();
        CardCmd.ClearAffliction(cardModel);
        GetInternalData<Data>().cards.Add(new CardBatch
        {
            card = cardModel,
            count = count
        });
        ((StringVar)DynamicVars["Card"]).StringValue = cardModel.Title;
    }
}
