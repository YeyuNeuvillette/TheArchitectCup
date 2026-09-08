using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class BattleIntuitionPower : BasePower
{
    public bool _triggeredThisTurn = false;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(0),
        new CardsVar(0)
    ];

    public void SetEnergy(decimal value)
    {
        DynamicVars.Energy.BaseValue = value;
    }

    public void SetCards(decimal value)
    {
        DynamicVars.Cards.BaseValue = value;
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner))
        {
            _triggeredThisTurn = false;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterHandEmptied(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner.Player && player.PlayerCombatState != null && IsValidPhase(player.PlayerCombatState.Phase) && !_triggeredThisTurn)
        {
            Flash();
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, player);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, player);
            _triggeredThisTurn = true;
        }
    }

    public static bool IsValidPhase(PlayerTurnPhase phase)
    {
        return (uint)(phase - 2) <= 2u;
    }
}
