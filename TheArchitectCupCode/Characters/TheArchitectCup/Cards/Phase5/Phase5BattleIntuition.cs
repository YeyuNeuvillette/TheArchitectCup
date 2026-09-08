using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheArchitectCup.Characters.TheArchitectCup.Powers;
using MegaCrit.Sts2.Core.Commands;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

public abstract class Phase5BattleIntuition() : ArchitectCupCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
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
        BattleIntuitionPower? power = await PowerCmd.Apply<BattleIntuitionPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        if(power != null)
        {
            power.SetEnergy(DynamicVars.Energy.BaseValue);
            power.SetCards(DynamicVars.Cards.BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1m);
    }
}

[RegisterCard(typeof(IroncladCardPool))]
public class Phase5BattleIntuitionIronclad() : Phase5BattleIntuition{}

[RegisterCard(typeof(SilentCardPool))]
public class Phase5BattleIntuitionSilent() : Phase5BattleIntuition{}