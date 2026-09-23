using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using TheArchitectCup.Api;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

public abstract class Phase5RedeployBase() : ArchitectCupCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.Self,
    sharedPortraitId: ArchitectCupCardIds.Phase5Redeploy)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "尖尖的刀"),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> cards = (await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 0, PileType.Hand.GetPile(Owner).Cards.Count),
            null,
            this)).ToList();
        int discardedEnergy = cards
            .Where(static card => !card.Keywords.Contains(CardKeyword.Unplayable))
            .Sum(static card => card.EnergyCost.GetAmountToSpend());

        await CardCmd.Discard(choiceContext, cards);

        int drawnEnergy = 0;
        while (IsUpgraded ? drawnEnergy <= discardedEnergy : drawnEnergy < discardedEnergy)
        {
            CardModel? card = await CardPileCmd.Draw(choiceContext, Owner);
            if (card == null)
                break;

            if (!card.Keywords.Contains(CardKeyword.Unplayable))
                drawnEnergy += card.EnergyCost.GetAmountToSpend();
        }
    }
}

[RegisterCard(typeof(SilentCardPool), FullPublicEntry = ArchitectCupCardIds.Phase5Redeploy)]
public sealed class Phase5Redeploy() : Phase5RedeployBase { }

[RegisterCard(typeof(DefectCardPool), FullPublicEntry = ArchitectCupCardIds.Phase5RedeployDefect)]
public sealed class Phase5RedeployDefect() : Phase5RedeployBase { }
