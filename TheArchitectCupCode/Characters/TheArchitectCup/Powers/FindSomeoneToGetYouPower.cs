using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;
using TheArchitectCup.Characters.TheArchitectCup.Cards;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public sealed class FindSomeoneToGetYouPower : BasePower
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player.Creature != Owner)
            return;

        int matchingPlays = CombatManager.Instance.History.CardPlaysStarted.Count(
            (CardPlayStartedEntry entry) =>
                entry.HappenedThisTurn(CombatState) &&
                entry.CardPlay.Player.Creature == Owner &&
                entry.CardPlay.Card.Id == cardPlay.Card.Id);
        if (matchingPlays < 2)
            return;

        Player player = cardPlay.Player;

        for (int index = 0; index < Amount; index++)
        {
            CardModel human = CombatState.CreateCard<Human>(player);
            await CardPileCmd.AddGeneratedCardToCombat(human, PileType.Hand, player);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}
