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

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("CanFunction", 1),
        new StringVar("Card", "你不该看到这个。"),
        new StringVar("Position", "你不该看到这个。")
    ];

    protected override object InitInternalData()
    {
        return new Data()
        {
            canFunctionThisTurn = false,
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
        DynamicVars["CanFunction"].BaseValue = canFunctionThisTurn ?(nextCard.Count == 0 ? 3 :(nextCard.Count == 1 ? 1 : 2)): 0;
        if(nextCard.Count == 1)
        {
            CardModel keyCard = nextCard.First().Key;
            ((StringVar)DynamicVars["Card"]).StringValue = keyCard.Title;
            int index = PileType.Hand.GetPile(Owner.Player).Cards.IndexOf(keyCard);
            ((StringVar)DynamicVars["Position"]).StringValue = index >= 0 ? (index + 1).ToString() : "不在手牌！";
        }
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if(!GetInternalData<Data>().canFunctionThisTurn)
        {
            return Task.CompletedTask;
        }
        if(card.Pile.Type == PileType.Play || oldPileType == PileType.Play)
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
            DynamicVars["CanFunction"].BaseValue = 0;
            return Task.CompletedTask;
        }
        CardModel? card = GetInternalData<Data>().nextCard[cardPlay.Card];
        GetInternalData<Data>().nextCard.Clear();
        if(card != null)
        {
            GetInternalData<Data>().nextCard.Add(card, GetNextCard(card));
            DynamicVars["CanFunction"].BaseValue = 1;
            ((StringVar)DynamicVars["Card"]).StringValue = card.Title;
            int index = PileType.Hand.GetPile(Owner.Player).Cards.IndexOf(card);
            ((StringVar)DynamicVars["Position"]).StringValue = index >= 0 ? (index + 1).ToString() : "不在手牌！";
        }
        else
        {
            DynamicVars["CanFunction"].BaseValue = 2;
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
        DynamicVars["CanFunction"].BaseValue = 3;
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