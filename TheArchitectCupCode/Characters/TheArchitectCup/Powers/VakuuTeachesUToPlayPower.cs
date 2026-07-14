using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class VakuuTeachesUToPlayPower : BasePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    private class Data
    {
        public bool canFunctionThisTurn;
        public required Dictionary<CardModel, CardModel?> nextCard;
    }

    protected override object InitInternalData()
    {
        return new Data()
        {
            nextCard = new Dictionary<CardModel, CardModel?>()
        };
    }

    public bool ShouldGlowForVakuu(CardModel card)
    {
        return GetInternalData<Data>().nextCard.ContainsKey(card);
    }

    public void SetInternalData(bool canFunctionThisTurn, Dictionary<CardModel, CardModel?> nextCard)
    {
        GetInternalData<Data>().canFunctionThisTurn = canFunctionThisTurn;
        GetInternalData<Data>().nextCard = nextCard;
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if(!GetInternalData<Data>().canFunctionThisTurn)
        {
            return Task.CompletedTask;
        }
        if(GetInternalData<Data>().nextCard.ContainsKey(card))
        {
            if(oldPileType == PileType.Hand)
            {
                CardModel? nextCard = GetInternalData<Data>().nextCard[card];
                GetInternalData<Data>().nextCard.Remove(card);
                if(nextCard != null)
                {
                    GetInternalData<Data>().nextCard.Add(nextCard, GetNextCard(nextCard));
                }
            }
            return Task.CompletedTask;
        }
        CardModel keyCard = GetInternalData<Data>().nextCard.FirstOrDefault(k => k.Value == card).Key;
        if (keyCard != null && oldPileType == PileType.Hand)
        {
            GetInternalData<Data>().nextCard[keyCard] = GetNextCard(card);
        }
        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if(!GetInternalData<Data>().canFunctionThisTurn)
        {
            return Task.CompletedTask;
        }
        if(!GetInternalData<Data>().nextCard.ContainsKey(cardPlay.Card))
        {
            GetInternalData<Data>().canFunctionThisTurn = false;
            return Task.CompletedTask;
        }
        CardModel? card = GetInternalData<Data>().nextCard[cardPlay.Card];
        GetInternalData<Data>().nextCard.Clear();
        if(card != null)
        {
            GetInternalData<Data>().nextCard.Add(card, GetNextCard(card));
        }
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }
        GetInternalData<Data>().canFunctionThisTurn = true;
        GetInternalData<Data>().nextCard.Clear();
        foreach(CardModel card in PileType.Hand.GetPile(Owner.Player).Cards)
        {
            GetInternalData<Data>().nextCard.Add(card, GetNextCard(card));
        }
    }

    private CardModel? GetNextCard(CardModel lastcard)
    {
        CardPile handPile = PileType.Hand.GetPile(Owner.Player);
        if (lastcard.Pile != handPile) return null;
        return handPile.Cards.SkipWhile(c => c != lastcard).Skip(1).FirstOrDefault(c => c.CanPlay());
    }

    public override bool ShouldFlush(Player player)
    {
        return player != Owner.Player || !GetInternalData<Data>().canFunctionThisTurn;
    }

    public override bool ShouldClearBlock(Creature creature)
    {
        return Owner != creature || !GetInternalData<Data>().canFunctionThisTurn;
    }

    public override Task AfterPreventingBlockClear(AbstractModel preventer, Creature creature)
    {
        if (this == preventer)
        {
            Flash();
        }
        return Task.CompletedTask;
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];
}