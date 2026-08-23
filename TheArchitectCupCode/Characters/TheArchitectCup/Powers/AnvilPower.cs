using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class AnvilPower : BasePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterForge(decimal amount, Player forger, AbstractModel? source)
    {
        Player? ownerPlayer = Owner.Player;
        if (ownerPlayer == null || forger != ownerPlayer || !LocalContext.NetId.HasValue)
        {
            return;
        }

        HookPlayerChoiceContext choiceContext = new(this, LocalContext.NetId.Value, ownerPlayer.Creature.CombatState, GameActionType.Combat);
        Task task = UpgradeHandCards(choiceContext, ownerPlayer);
        await choiceContext.AssignTaskAndWaitForPauseOrCompletion(task);
    }

    private async Task UpgradeHandCards(PlayerChoiceContext choiceContext, Player player)
    {
        int amount = Math.Min((int)Amount, PileType.Hand.GetPile(player).Cards.Count(c => c.IsUpgradable));
        if (amount <= 0)
        {
            return;
        }

        IEnumerable<CardModel> cards = await CardSelectCmd.FromHand(
            choiceContext,
            player,
            new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, amount),
            c => c.IsUpgradable,
            this);

        foreach (CardModel card in cards)
        {
            CardCmd.Upgrade(card);
        }
    }
}
