using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(ColorlessCardPool))]
public sealed class Phase6Haste() : ArchitectCupCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "苑风"),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> list = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 0, 999999999), c => c.IsUpgraded, this)).ToList();
        foreach (CardModel item in list)
        {
            if (!IsUpgraded)
            {
                CardCmd.Downgrade(item);
            }
            await CardCmd.AutoPlay(choiceContext, item, null);
            if (IsUpgraded)
            {
                CardCmd.Downgrade(item);
            }
        }
    }
}