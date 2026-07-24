using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace TheArchitectCup.Characters.TheArchitectCup.Patches;

[Obsolete("Use CardPileCmd.GiveToAnotherPlayer instead.")]
public static class HandTransferPatch
{
    public static Task GiveCardFromHandToAnotherPlayerHand(
        CardModel card,
        Player targetPlayer,
        CardPilePosition position = CardPilePosition.Bottom) =>
        CardPileCmd.GiveToAnotherPlayer(card, targetPlayer, PileType.Hand, position);
}
