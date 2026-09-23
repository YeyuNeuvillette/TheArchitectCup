using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Api;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

public abstract class Phase5EqualValueExchangeBase() : ArchitectCupCard(
    2, CardType.Skill, CardRarity.Uncommon, TargetType.Self,
    sharedPortraitId: ArchitectCupCardIds.Phase5EqualValueExchange)
{
    private bool _awaitingResultPile;
    private bool _resolvingChoice;

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
        if (card != this)
            return;

        PileType newPileType = card.Pile?.Type ?? PileType.None;

        if (oldPileType == PileType.Hand)
        {
            if (newPileType == PileType.Play)
            {
                _awaitingResultPile = true;
                return;
            }

            await ChooseFromDestinationPile(newPileType);
            return;
        }

        if (oldPileType == PileType.Play && _awaitingResultPile)
        {
            _awaitingResultPile = false;
            await ChooseFromDestinationPile(newPileType);
        }
    }

    private async Task ChooseFromDestinationPile(PileType pileType)
    {
        if (_resolvingChoice ||
            pileType is PileType.None or PileType.Hand or PileType.Play or PileType.Deck ||
            !LocalContext.NetId.HasValue)
            return;

        CardPile destinationPile = pileType.GetPile(Owner);
        if (destinationPile.Cards.Count == 0)
            return;

        HookPlayerChoiceContext choiceContext = new(
            this,
            LocalContext.NetId.Value,
            Owner.Creature.CombatState,
            GameActionType.Combat);
        Task task = ResolveChoice(choiceContext, destinationPile);
        await choiceContext.AssignTaskAndWaitForPauseOrCompletion(task);
    }

    private async Task ResolveChoice(PlayerChoiceContext choiceContext, CardPile destinationPile)
    {
        _resolvingChoice = true;
        try
        {
            CardModel? selected = (await CardSelectCmd.FromCombatPile(
                choiceContext,
                destinationPile,
                Owner,
                new CardSelectorPrefs(SelectionScreenPrompt, 1))).FirstOrDefault();

            if (selected != null)
                await CardPileCmd.Add(selected, PileType.Hand);
        }
        finally
        {
            _resolvingChoice = false;
        }
    }
}

[RegisterCard(typeof(SilentCardPool), FullPublicEntry = ArchitectCupCardIds.Phase5EqualValueExchange)]
public sealed class Phase5EqualValueExchange() : Phase5EqualValueExchangeBase { }

[RegisterCard(typeof(RegentCardPool), FullPublicEntry = ArchitectCupCardIds.Phase5EqualValueExchangeRegent)]
public sealed class Phase5EqualValueExchangeRegent() : Phase5EqualValueExchangeBase { }
