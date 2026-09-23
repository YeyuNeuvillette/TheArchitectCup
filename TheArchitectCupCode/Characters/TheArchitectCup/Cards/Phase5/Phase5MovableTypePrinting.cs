using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using TheArchitectCup.Api;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

public abstract class Phase5MovableTypePrintingBase() : ArchitectCupCard(
    2, CardType.Skill, CardRarity.Rare, TargetType.Self,
    sharedPortraitId: ArchitectCupCardIds.Phase5MovableTypePrinting)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "盐")
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel? selected = (await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1),
            null,
            this)).FirstOrDefault();
        if (selected == null)
            return;

        CardKeyword[] keywords = selected.Keywords
            .Where(static keyword => keyword != CardKeyword.None)
            .ToArray();
        if (keywords.Length == 0)
            return;

        foreach (CardModel handCard in PileType.Hand.GetPile(Owner).Cards.ToArray())
            CardCmd.ApplyKeyword(handCard, keywords);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}

[RegisterCard(typeof(SilentCardPool), FullPublicEntry = ArchitectCupCardIds.Phase5MovableTypePrinting)]
public sealed class Phase5MovableTypePrinting() : Phase5MovableTypePrintingBase { }

[RegisterCard(typeof(NecrobinderCardPool), FullPublicEntry = ArchitectCupCardIds.Phase5MovableTypePrintingNecrobinder)]
public sealed class Phase5MovableTypePrintingNecrobinder() : Phase5MovableTypePrintingBase { }
