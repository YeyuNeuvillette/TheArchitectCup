using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.TheArchitectCup.Powers;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(ColorlessCardPool))]
public sealed class VakuuTeachesUToPlay() : ArchitectCupCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Retain),
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "塔莉娜")
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        VakuuTeachesUToPlayPower? power = await PowerCmd.Apply<VakuuTeachesUToPlayPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        if(power != null)
        {
            power.SetInternalData(canFunctionThisTurn, nextCard);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }

    private bool canFunctionThisTurn;

    private Dictionary<CardModel, CardModel?> nextCard = new Dictionary<CardModel, CardModel?>();

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if(!canFunctionThisTurn)
        {
            return Task.CompletedTask;
        }
        if(card.Pile.Type == PileType.Play || oldPileType == PileType.Play)
        {
            return Task.CompletedTask;
        }
        if(nextCard.ContainsKey(card))
        {
            if(oldPileType == PileType.Hand)
            {
                CardModel? nCard = nextCard[card];
                nextCard.Remove(card);
                if(nCard != null)
                {
                    nextCard.Add(nCard, GetNextCard(nCard));
                }
            }
            return Task.CompletedTask;
        }
        CardModel keyCard = nextCard.FirstOrDefault(k => k.Value == card).Key;
        if (keyCard != null && oldPileType == PileType.Hand)
        {
            nextCard[keyCard] = GetNextCard(card);
        }
        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if(!canFunctionThisTurn)
        {
            return Task.CompletedTask;
        }
        if(!nextCard.ContainsKey(cardPlay.Card))
        {
            canFunctionThisTurn = false;
            return Task.CompletedTask;
        }
        CardModel? card = nextCard[cardPlay.Card];
        nextCard.Clear();
        if(card != null)
        {
            nextCard.Add(card, GetNextCard(card));
        }
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner)
        {
            return;
        }
        canFunctionThisTurn = true;
        nextCard.Clear();
        foreach(CardModel card in PileType.Hand.GetPile(Owner).Cards)
        {
            nextCard.Add(card, GetNextCard(card));
        }
    }

    private CardModel? GetNextCard(CardModel lastcard)
    {
        CardPile handPile = PileType.Hand.GetPile(Owner);
        if (lastcard.Pile != handPile) return null;
        return handPile.Cards.SkipWhile(c => c != lastcard).Skip(1).FirstOrDefault(c => c.CanPlay());
    }
}