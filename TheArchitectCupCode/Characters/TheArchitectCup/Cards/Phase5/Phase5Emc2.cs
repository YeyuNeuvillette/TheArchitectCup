using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.TheArchitectCup.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using TheArchitectCup.Api;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

public abstract class Phase5Emc2Base() : ArchitectCupCard(
    1, CardType.Power, CardRarity.Rare, TargetType.Self,
    sharedPortraitId: ArchitectCupCardIds.Phase5Emc2)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "💤"),
        new HoverTip(new LocString("static_hover_tips", "CHAMPION_PHASE5.title"), new LocString("static_hover_tips", "CHAMPION_PHASE5.description")),
        EnergyHoverTip,
        HoverTipFactory.FromCard<Debris>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<Emc2Power>(choiceContext, Owner.Creature, DynamicVars.Cards.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}

[RegisterCard(typeof(DefectCardPool), FullPublicEntry = ArchitectCupCardIds.Phase5Emc2)]
public sealed class Phase5Emc2() : Phase5Emc2Base { }

[RegisterCard(typeof(RegentCardPool), FullPublicEntry = ArchitectCupCardIds.Phase5Emc2Regent)]
public sealed class Phase5Emc2Regent() : Phase5Emc2Base { }
