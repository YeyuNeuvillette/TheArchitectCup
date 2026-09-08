using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

public abstract class Phase5EqualValueExchange() : ArchitectCupCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public bool playerFromHand { get; private set; } = false;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "AlwaysReady")
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card == this && card.Pile != null && card.Pile.Type != PileType.None)
        {
            if (oldPileType == PileType.Hand)
            {
                if(card.Pile.Type == PileType.Play)
                {
                    playerFromHand = true;
                }
                else
                {
                    await ChooseAndAdd(card.Pile.Type);
                }
            }
            else if (oldPileType == PileType.Play && playerFromHand)
            {
                playerFromHand = false;
                await ChooseAndAdd(card.Pile.Type);
            }
        }
    }

    private async Task ChooseAndAdd(PileType type) //注意：使用了ThrowingPlayerChoiceContext()
    {
        CardModel? card = (await CardSelectCmd.FromCombatPile(new ThrowingPlayerChoiceContext(), type.GetPile(Owner), Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1))).FirstOrDefault();
        if(card != null)
        {
            await CardPileCmd.Add(card, PileType.Hand);
        }
    }
}

[RegisterCard(typeof(SilentCardPool))]
public class Phase5EqualValueExchangeSilent() : Phase5EqualValueExchange{}

[RegisterCard(typeof(RegentCardPool))]
public class Phase5EqualValueExchangeRegent() : Phase5EqualValueExchange{}