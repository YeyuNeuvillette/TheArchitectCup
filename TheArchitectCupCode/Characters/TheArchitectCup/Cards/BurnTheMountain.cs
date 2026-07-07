using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.TheArchitectCup.Powers;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(IroncladCardPool))]
public sealed class BurnTheMountain() : ArchitectCupCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int amount = IsUpgraded ? 2 : 1;
        foreach (Player player in Owner.Creature.CombatState!.Players)
        {
            await PowerCmd.Apply<BurnTheMountainPower>(choiceContext, player.Creature, amount, Owner.Creature, this);
        }
    }
}