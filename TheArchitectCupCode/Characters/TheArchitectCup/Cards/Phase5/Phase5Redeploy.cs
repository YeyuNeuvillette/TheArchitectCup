using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

public abstract class Phase5Redeploy() : ArchitectCupCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "尖尖的刀"),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> cards = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 0, 999999999), null, this)).ToList();
        int totalEnergy = cards.Where(c => !c.Keywords.Contains(CardKeyword.Unplayable)).Sum(c => c.EnergyCost.GetResolved());
        if(IsUpgraded) totalEnergy += 1;
        await CardCmd.Discard(choiceContext, cards);
        while(totalEnergy > 0)
        {
            CardModel? card = await CardPileCmd.Draw(choiceContext, Owner);
            if (card == null)
            {
                break;
            }
            if (!card.Keywords.Contains(CardKeyword.Unplayable))
            {
                totalEnergy -= card.EnergyCost.GetResolved();
            }
        }
    }
}

[RegisterCard(typeof(SilentCardPool))]
public class Phase5RedeploySilent() : Phase5Redeploy{}

[RegisterCard(typeof(DefectCardPool))]
public class Phase5RedeployDefect() : Phase5Redeploy{}