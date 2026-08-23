using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheArchitectCup.Characters.TheArchitectCup.Powers;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(NecrobinderCardPool))]
public sealed class Haunting() : ArchitectCupCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromCard<Soul>(),
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "酥润"),
        new HoverTip(new LocString("static_hover_tips", "CHAMPION_PHASE3.title"), new LocString("static_hover_tips", "CHAMPION_PHASE3.description"))
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        int previousAmount = Owner.Creature.GetPower<HauntingPower>()?.Amount ?? 0;
        HauntingPower? power = await PowerCmd.Apply<HauntingPower>(choiceContext, Owner.Creature, 3m, Owner.Creature, this);
        if (power != null)
        {
            power.AddCard(this, power.Amount - previousAmount);
        }
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card == this && CombatState != null)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(Soul.Create(Owner, DynamicVars.Cards.IntValue, CombatState), PileType.Draw, Owner, CardPilePosition.Random));
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Exhaust);
    }
}
