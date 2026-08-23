using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class OneTwoPunchPower : BasePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner.Player)
        {
            Flash();
            int strength = Owner.HasPower<StrengthPower>() ? Owner.GetPower<StrengthPower>()!.Amount : 0;
            int dexterity = Owner.HasPower<DexterityPower>() ? Owner.GetPower<DexterityPower>()!.Amount : 0;
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, dexterity - strength, Owner, null);
            await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, strength - dexterity, Owner, null);
        }
    }
}
