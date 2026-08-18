using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.CardSelection;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class AnvilPower : BasePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterForge(decimal amount, Player forger, AbstractModel? source)
    {
        if (forger != Owner.Player)
        {
            return;
        }
        IEnumerable<CardModel> cards = await CardSelectCmd.FromHand(new ThrowingPlayerChoiceContext(), Owner.Player, 
        new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, Amount),c => c.IsUpgradable, this);
        foreach (CardModel card in cards)
        {
            CardCmd.Upgrade(card);
        }
    }
}
