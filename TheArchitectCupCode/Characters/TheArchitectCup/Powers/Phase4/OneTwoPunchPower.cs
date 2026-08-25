using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class OneTwoPunchPower : BasePower
{
    private sealed class Data
    {
        internal CardModel? SourceCard;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override object InitInternalData() => new Data();

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        GetInternalData<Data>().SourceCard = cardSource;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return;

        Data data = GetInternalData<Data>();
        if (ReferenceEquals(cardPlay.Card, data.SourceCard))
        {
            data.SourceCard = null;
            return;
        }

        int strength = Owner.GetPower<StrengthPower>()?.Amount ?? 0;
        int dexterity = Owner.GetPower<DexterityPower>()?.Amount ?? 0;
        if (strength == dexterity)
            return;

        Flash();
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, dexterity - strength, Owner, null);
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, strength - dexterity, Owner, null);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
            await PowerCmd.Remove(this);
    }
}
