using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.CardSelection;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(NecrobinderCardPool))]
public sealed class Phase6BestPartner() : ArchitectCupCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "起司"),
        HoverTipFactory.Static(StaticHoverTip.SummonDynamic, DynamicVars.Summon),
        .. HoverTipFactory.FromEnchantment<PerfectFit>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new SummonVar(5m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(Owner.PlayerCombatState == null) return;
        await CreatureCmd.TriggerAnim(Owner.Creature, Necrobinder.GetSummonAnimIfApplicable(Owner.Character), Necrobinder.GetSummonDelayIfApplicable(Owner.Character));
        await OstyCmd.Summon(choiceContext, Owner, DynamicVars.Summon.BaseValue, this);
        IReadOnlyList<CardModel> cards = Owner.PlayerCombatState.AllCards.Where(c => c.Tags.Contains(CardTag.OstyAttack) && ModelDb.Enchantment<PerfectFit>().CanEnchant(c)).ToList();
        CardModel? card = (await CardSelectCmd.FromSimpleGrid(choiceContext, cards, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1))).FirstOrDefault();
        if (card != null)
        {
            CardCmd.Enchant<PerfectFit>(card, 1m);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Summon.UpgradeValueBy(2m);
    }
}