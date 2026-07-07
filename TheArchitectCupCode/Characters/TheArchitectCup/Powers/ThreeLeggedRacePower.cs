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
using TheArchitectCup.Characters.Base;
using TheArchitectCup.Characters.TheArchitectCup.Patches;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class ThreeLeggedRacePower : BasePower, IEnergyGainedListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private class Data
    {
        public Creature Teammate = null!;
        public bool IsRegistered;
    }

    protected override object? InitInternalData()
    {
        return new Data();
    }

    public void SetTeammate(Creature teammate)
    {
        GetInternalData<Data>().Teammate = teammate;
    }

    public Creature GetTeammate()
    {
        return GetInternalData<Data>().Teammate;
    }

    public void RegisterListener()
    {
        Data data = GetInternalData<Data>();
        if (!data.IsRegistered)
        {
            data.IsRegistered = true;
            EnergyGainPatch.Listeners.Add(this);
        }
    }

    public void OnEnergyGained(Player player, decimal amount)
    {
        Data data = GetInternalData<Data>();
        if (data.Teammate == null || player.Creature != data.Teammate)
            return;

        if (Owner?.Player == null)
            return;

        _ = TriggerDraw(Owner.Player, (int)amount);
    }

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        Data data = GetInternalData<Data>();
        if (data.Teammate == null || card.Owner.Creature != Owner)
            return;

        if (amount <= 0)
            return;

        Player? teammatePlayer = data.Teammate.Player;
        if (teammatePlayer == null)
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
        if (participants.Contains(Owner))
        {
            Unregister();
            await PowerCmd.Remove(this);
        }
    }

    private void Unregister()
    {
        Data data = GetInternalData<Data>();
        if (data.IsRegistered)
        {
            EnergyGainPatch.Listeners.Remove(this);
            data.IsRegistered = false;
        }
    }
}