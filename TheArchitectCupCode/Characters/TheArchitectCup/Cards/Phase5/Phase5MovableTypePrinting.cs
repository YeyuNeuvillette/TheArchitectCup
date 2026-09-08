using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

public abstract class Phase5MovableTypePrinting() : ArchitectCupCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "盐")
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel? card = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this)).FirstOrDefault();
        if(card != null)
        {
            foreach(CardKeyword keyword in card.Keywords)
            {
                foreach(CardModel handCard in PileType.Hand.GetPile(Owner).Cards)
                {
                    handCard.AddKeyword(keyword);
                }
            }
        }
        
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}

[RegisterCard(typeof(SilentCardPool))]
public class Phase5MovableTypePrintingSilent() : Phase5MovableTypePrinting{}

[RegisterCard(typeof(NecrobinderCardPool))]
public class Phase5MovableTypePrintingNecrobinder() : Phase5MovableTypePrinting{}