using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Combat.PlayerResources;
using TheArchitectCup.Characters.Base;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public sealed class ThreeLeggedRacePower : BasePower, IPlayerResourceHookListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    public async Task AfterPlayerEnergyGained(PlayerResourceGainContext context)
    {
        if (context.Player.Creature != Owner || Applier?.IsDead != false || Applier.Player is not { } drawingPlayer)
            return;

        await TriggerDraw(drawingPlayer, context.Amount);
    }

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner.Creature != Applier)
            return;

        if (amount <= 0)
            return;

        if (Owner.Player is not { } teammatePlayer)
            return;

        await TriggerDiscard(teammatePlayer, amount);
    }

    private async Task TriggerDraw(Player player, int count)
    {
        if (count <= 0)
            return;

        Flash();
        await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), count, player);
    }

    private async Task TriggerDiscard(Player player, int count)
    {
        if (count <= 0)
            return;

        List<CardModel> handCards = PileType.Hand.GetPile(player).Cards.ToList();
        if (handCards.Count == 0)
            return;

        int actualCount = Math.Min(count, handCards.Count);
        List<CardModel> toDiscard = handCards.Take(actualCount).ToList();

        Flash();
        await CardCmd.Discard(new ThrowingPlayerChoiceContext(), toDiscard);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (Applier != null && participants.Contains(Applier))
        {
            await PowerCmd.Remove(this);
        }
    }
}
