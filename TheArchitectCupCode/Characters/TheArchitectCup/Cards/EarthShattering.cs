using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Interop.AutoRegistration;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(IroncladCardPool))]
public sealed class EarthShattering() : ArchitectCupCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        base.AdditionalHoverTips.Concat([
            HoverTipFactory.FromCard<GiantRock>(IsUpgraded)
        ]);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var combatState = CombatState;
        if (combatState == null)
            return;

        foreach (var player in combatState.Players)
        {
            var attackCards = PileType.Hand.GetPile(player).Cards
                .Where(c => c != null && c.IsTransformable && c.Type == CardType.Attack)
                .ToList();

            foreach (var card in attackCards)
            {
                var giantRock = combatState.CreateCard<GiantRock>(player);
                if (IsUpgraded)
                {
                    CardCmd.Upgrade(giantRock);
                }
                await CardCmd.Transform(card, giantRock);
            }
        }
    }
}