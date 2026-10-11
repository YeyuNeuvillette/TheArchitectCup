using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(ColorlessCardPool), FullPublicEntry = global::TheArchitectCup.Api.ArchitectCupCardIds.Phase6Haste)]
public sealed class Phase6Haste() : ArchitectCupCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "苑风"),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.PlayerCombatState == null) return;
        bool playBeforeDowngrading = IsUpgraded;
        List<CardModel> list = (await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 0, PileType.Hand.GetPile(Owner).Cards.Count),
            c => playBeforeDowngrading ? c.IsUpgraded : true, this)).ToList();
        // The base version finishes downgrading the selection before any play can change it.
        if (!playBeforeDowngrading)
            foreach (CardModel item in list.Where(c => c.IsUpgraded))
                CardCmd.Downgrade(item);
        foreach (CardModel item in list)
        {
            if (CombatManager.Instance.IsOverOrEnding) break;
            if (item.Pile?.Type != PileType.Hand) continue;
            await CardCmd.AutoPlay(choiceContext, item, null);
        }
        if (playBeforeDowngrading)
            foreach (CardModel item in list.Where(c => c.IsUpgraded))
                CardCmd.Downgrade(item);
    }
}
