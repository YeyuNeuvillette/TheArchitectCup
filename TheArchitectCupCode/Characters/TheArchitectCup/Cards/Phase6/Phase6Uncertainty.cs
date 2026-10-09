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
using System.Reflection;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(SilentCardPool))]
public sealed class Phase6Uncertainty() : ArchitectCupCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "🍄🍈."),
        .. HoverTipFactory.FromEnchantment<Slither>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2)
    ];

    private static readonly FieldInfo? LocalModifiersField = typeof(CardEnergyCost).GetField("_localModifiers", BindingFlags.NonPublic | BindingFlags.Instance);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<CardModel> cards = await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 2),
        c => !(c.EnergyCost.CostsX || c.Keywords.Contains(CardKeyword.Unplayable)), this);
        if (cards.Count() != 2) return;

        CardModel? card1 = cards.FirstOrDefault();
        CardModel? card2 = cards.LastOrDefault();
        if (card1 == null || card2 == null || LocalModifiersField == null) return;

        int card1EnergyCost = card1.EnergyCost.GetWithModifiers(CostModifiers.None);
        int card2EnergyCost = card2.EnergyCost.GetWithModifiers(CostModifiers.None);
        List<LocalCostModifier>? list1 = LocalModifiersField.GetValue(card1.EnergyCost) as List<LocalCostModifier>;
        List<LocalCostModifier>? list2 = LocalModifiersField.GetValue(card2.EnergyCost) as List<LocalCostModifier>;
        card1.EnergyCost.SetCustomBaseCost(card2EnergyCost);
        card2.EnergyCost.SetCustomBaseCost(card1EnergyCost);
        LocalModifiersField.SetValue(card1.EnergyCost, list2);
        LocalModifiersField.SetValue(card2.EnergyCost, list1);

        card1EnergyCost = card1.EnergyCost.GetWithModifiers(CostModifiers.All);
        card2EnergyCost = card2.EnergyCost.GetWithModifiers(CostModifiers.All);
        if(card1EnergyCost > card2EnergyCost && ModelDb.Enchantment<Slither>().CanEnchant(card1))
        {
            CardCmd.Enchant<Steady>(card1, 1m);
        }
        if(card1EnergyCost < card2EnergyCost && ModelDb.Enchantment<Slither>().CanEnchant(card2))
        {
            CardCmd.Enchant<Steady>(card2, 1m);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}