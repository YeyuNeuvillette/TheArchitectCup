using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(ColorlessCardPool))]
public sealed class BeeBroOnTheRun() : ArchitectCupCard(2, CardType.Power, CardRarity.Rare, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Innate
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<PersonalHivePower>(1m),
        new PowerVar<SlowPower>(1m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = CombatState;
        if (combatState == null)
            return;

        foreach (Creature enemy in combatState.HittableEnemies)
        {
            await PowerCmd.Apply<PersonalHivePower>(choiceContext, enemy, DynamicVars["PersonalHivePower"].BaseValue, base.Owner.Creature, this);
            await PowerCmd.Apply<SlowPower>(choiceContext, enemy, DynamicVars["SlowPower"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}