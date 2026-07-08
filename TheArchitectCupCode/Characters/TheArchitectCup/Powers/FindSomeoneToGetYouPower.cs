using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;
using TheArchitectCup.Characters.TheArchitectCup.Cards;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public sealed class FindSomeoneToGetYouPower : BasePower
{
    private class Data
    {
        public required HashSet<ModelId> recordedCardIds;
    }

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data()
        {
            recordedCardIds = new HashSet<ModelId>()
        };
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner)
            return;

        if (GetInternalData<Data>().recordedCardIds.Contains(cardPlay.Card.Id))
        {
            Player? player = cardPlay.Card.Owner;
            if (player == null)
                return;
            CardModel human = CombatState.CreateCard<Human>(player);
            await CardPileCmd.AddGeneratedCardToCombat(human, PileType.Hand, player);
        }
        else
        {
            GetInternalData<Data>().recordedCardIds.Add(cardPlay.Card.Id);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            GetInternalData<Data>().recordedCardIds.Clear();
            await PowerCmd.Remove(this);
        }
    }
}