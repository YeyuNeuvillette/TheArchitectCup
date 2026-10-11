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

[RegisterCard(typeof(SilentCardPool), FullPublicEntry = global::TheArchitectCup.Api.ArchitectCupCardIds.Phase6Uncertainty)]
public sealed class Phase6Uncertainty() : ArchitectCupCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "🍄🍈."),
        .. HoverTipFactory.FromEnchantment<Slither>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2)
    ];


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.PlayerCombatState == null) return;
        CardModel[] cards = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 2),
        c => !(c.EnergyCost.CostsX || c.Keywords.Contains(CardKeyword.Unplayable)), this)).ToArray();
        if (cards.Length != 2) return;

        CardModel? card1 = cards.FirstOrDefault();
        CardModel? card2 = cards.LastOrDefault();
        if (card1 == null || card2 == null) return;

        int card1EnergyCost = card1.EnergyCost.GetWithModifiers(CostModifiers.None);
        int card2EnergyCost = card2.EnergyCost.GetWithModifiers(CostModifiers.None);
        // Keep each transferred modifier's expiration, without sharing mutable modifier instances.
        List<LocalCostModifier> list1 = card1.EnergyCost._localModifiers.Select(m => m.Clone()).ToList();
        List<LocalCostModifier> list2 = card2.EnergyCost._localModifiers.Select(m => m.Clone()).ToList();
        card1.EnergyCost.SetCustomBaseCost(card2EnergyCost);
        card2.EnergyCost.SetCustomBaseCost(card1EnergyCost);
        card1.EnergyCost._localModifiers = list2;
        card2.EnergyCost._localModifiers = list1;
        card1.InvokeEnergyCostChanged();
        card2.InvokeEnergyCostChanged();

        card1EnergyCost = card1.EnergyCost.GetWithModifiers(CostModifiers.All);
        card2EnergyCost = card2.EnergyCost.GetWithModifiers(CostModifiers.All);
        if(card1EnergyCost > card2EnergyCost && ModelDb.Enchantment<Slither>().CanEnchant(card1))
        {
            CardCmd.Enchant<Slither>(card1, 1m);
        }
        if(card1EnergyCost < card2EnergyCost && ModelDb.Enchantment<Slither>().CanEnchant(card2))
        {
            CardCmd.Enchant<Slither>(card2, 1m);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
