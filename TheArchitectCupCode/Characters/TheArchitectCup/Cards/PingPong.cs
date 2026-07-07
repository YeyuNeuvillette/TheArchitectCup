using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.TheArchitectCup.Powers;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(ColorlessCardPool))]
public sealed class PingPong() : ArchitectCupCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Player? teammate = Owner.Creature.CombatState?.Players.FirstOrDefault(p => p != Owner);
        if (teammate == null)
            return;

        PingPongPower? power = await PowerCmd.Apply<PingPongPower>(choiceContext, teammate.Creature, 1, Owner.Creature, this);
        if (power != null)
            power.IsUpgraded = IsUpgraded;
    }

    protected override void OnUpgrade()
    {
    }
}