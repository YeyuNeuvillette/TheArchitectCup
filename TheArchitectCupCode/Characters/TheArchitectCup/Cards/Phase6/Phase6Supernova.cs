using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Commands;
using TheArchitectCup.Characters.TheArchitectCup.Powers;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(RegentCardPool))]
public sealed class Phase6Supernova() : ArchitectCupCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "血色鸢尾花"),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new StarsVar(3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.AttackAnimDelay);
        await PlayerCmd.GainStars(DynamicVars.Stars.BaseValue, Owner);
        await PowerCmd.Apply<SupernovaPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Stars.UpgradeValueBy(1m);
    }
}