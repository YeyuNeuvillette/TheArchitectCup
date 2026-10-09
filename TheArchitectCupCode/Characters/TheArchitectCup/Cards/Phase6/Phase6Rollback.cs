using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(DefectCardPool))]
public sealed class Phase6Rollback() : ArchitectCupCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "紫幽梦魇Grimm"),
        .. HoverTipFactory.FromEnchantment<Steady>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> list = (await CardSelectCmd.FromHand(choiceContext, Owner,
        new CardSelectorPrefs(SelectionScreenPrompt, 0, DynamicVars.Cards.IntValue), c => c.IsUpgraded && ModelDb.Enchantment<Steady>().CanEnchant(c), this)).ToList();
        foreach (CardModel item in list)
        {
            CardCmd.Downgrade(item);
            CardCmd.Enchant<Steady>(item, 1m);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}