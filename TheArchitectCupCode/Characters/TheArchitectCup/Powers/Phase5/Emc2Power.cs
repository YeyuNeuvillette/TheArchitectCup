using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class Emc2Power : BasePower
{
    private sealed class Data
    {
        public int UsesSpentThisTurn;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Math.Max(0, Amount - GetInternalData<Data>().UsesSpentThisTurn);

    protected override object InitInternalData() => new Data();

    public bool CanConvert(CardModel card)
    {
        if (card.Owner.Creature != Owner ||
            card.Type is not (CardType.Attack or CardType.Skill) ||
            card.Pile?.Type != PileType.Hand ||
            card.Owner.PlayerCombatState == null ||
            GetInternalData<Data>().UsesSpentThisTurn >= Amount)
            return false;

        return card.EnergyCost.GetAmountToSpend() > card.Owner.PlayerCombatState.Energy;
    }

    public async Task<(int EnergySpent, int StarsSpent)> SpendWithConversion(CardModel card)
    {
        ICombatState combatState = card.CombatState
            ?? throw new InvalidOperationException("Mass-Energy Conversion requires an active combat.");
        int requiredEnergy = card.EnergyCost.GetAmountToSpend();
        int energySpent = Math.Min(requiredEnergy, card.Owner.PlayerCombatState!.Energy);
        int debrisCount = Math.Max(0, requiredEnergy - energySpent);
        int starsSpent = Math.Max(0, card.GetStarCostWithModifiers());

        Data data = GetInternalData<Data>();
        data.UsesSpentThisTurn++;
        InvokeDisplayAmountChanged();
        Flash();

        if (energySpent > 0)
        {
            CombatManager.Instance.History.EnergySpent(combatState, energySpent, card.Owner);
            card.Owner.PlayerCombatState.LoseEnergy(energySpent);
        }
        await Hook.AfterEnergySpent(combatState, card, energySpent);

        card.LastStarsSpent = starsSpent;
        if (starsSpent > 0)
        {
            card.Owner.PlayerCombatState.LoseStars(starsSpent);
            await Hook.AfterStarsSpent(combatState, starsSpent, card.Owner);
        }

        if (debrisCount > 0)
        {
            List<CardModel> debris = Enumerable.Range(0, debrisCount)
                .Select(_ => combatState.CreateCard<Debris>(card.Owner))
                .Cast<CardModel>()
                .ToList();
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(
                debris,
                PileType.Draw,
                card.Owner,
                CardPilePosition.Random));
        }

        return (energySpent, starsSpent);
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner))
        {
            GetInternalData<Data>().UsesSpentThisTurn = 0;
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }
}
