using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using TheArchitectCup.Characters.TheArchitectCup.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(ColorlessCardPool))]
public sealed class Phase6AutomaticWeapon() : ArchitectCupCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "AlwaysReady"),
        HoverTipFactory.FromKeyword(CardKeyword.Unplayable)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<CardModel> cards = await CardSelectCmd.FromCombatPile(choiceContext, PileType.Draw.GetPile(Owner), Owner, new CardSelectorPrefs(SelectionScreenPrompt, 0, DynamicVars.Cards.IntValue), c => c.Type == CardType.Attack);
        foreach (CardModel card in cards)
        {
            card.AddKeyword(CardKeyword.Unplayable);
        }
        await PowerCmd.Apply<AutomaticWeaponPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}