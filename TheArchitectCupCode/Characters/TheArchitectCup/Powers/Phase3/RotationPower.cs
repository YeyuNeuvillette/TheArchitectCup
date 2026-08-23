using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class RotationPower : BasePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Player == null)
        {
            return;
        }

        CardPile discardPile = PileType.Discard.GetPile(Owner.Player);
        int amount = Math.Min((int)Amount, discardPile.Cards.Count);
        if (amount <= 0)
        {
            return;
        }

        IEnumerable<CardModel> cards = await CardSelectCmd.FromCombatPile(choiceContext, discardPile, Owner.Player, new CardSelectorPrefs(GetSelectionPrompt(), amount));
        await CardPileCmd.Add(cards, PileType.Draw, CardPilePosition.Top);
    }

    private LocString GetSelectionPrompt()
    {
        LocString prompt = new("powers", $"{Id.Entry}.selectionScreenPrompt");
        return prompt.Exists() ? prompt : CardSelectorPrefs.DiscardSelectionPrompt;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}
