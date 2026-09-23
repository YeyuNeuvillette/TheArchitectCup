using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheArchitectCup.Characters.TheArchitectCup.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using TheArchitectCup.Api;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

public abstract class Phase5BattleIntuitionBase() : ArchitectCupCard(
    2, CardType.Power, CardRarity.Uncommon, TargetType.Self,
    sharedPortraitId: ArchitectCupCardIds.Phase5BattleIntuition)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "紫幽梦魇Grimm"),
        EnergyHoverTip
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(1),
        new CardsVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        BattleIntuitionPower power = (BattleIntuitionPower)ModelDb.Power<BattleIntuitionPower>().ToMutable();
        power.Configure(DynamicVars.Energy.BaseValue, DynamicVars.Cards.BaseValue);
        await PowerCmd.Apply(choiceContext, power, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1m);
    }
}

[RegisterCard(typeof(IroncladCardPool), FullPublicEntry = ArchitectCupCardIds.Phase5BattleIntuition)]
public sealed class Phase5BattleIntuition() : Phase5BattleIntuitionBase { }

[RegisterCard(typeof(SilentCardPool), FullPublicEntry = ArchitectCupCardIds.Phase5BattleIntuitionSilent)]
public sealed class Phase5BattleIntuitionSilent() : Phase5BattleIntuitionBase { }
