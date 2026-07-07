using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace TheArchitectCup.Characters.TheArchitectCup.Patches;

public static class HandTransferPatch
{
    public static async Task GiveCardFromHandToAnotherPlayerHand(CardModel card, Player targetPlayer, CardPilePosition position = CardPilePosition.Bottom)
    {
        if (card.Pile?.Type != PileType.Hand)
            return;

        Player sourcePlayer = card.Owner;
        bool sourceIsLocal = LocalContext.IsMe(sourcePlayer);
        bool targetIsLocal = LocalContext.IsMe(targetPlayer);

        NCard? cardNode = NCard.FindOnTable(card);
        Vector2 cardGlobalPos = cardNode?.GlobalPosition ?? Vector2.Zero;

        if (sourceIsLocal && cardNode != null)
        {
            NPlayerHand? sourceHand = NCombatRoom.Instance?.Ui?.Hand;
            if (sourceHand != null)
            {
                NCardHolder? holder = sourceHand.GetCardHolder(card);
                if (holder != null)
                {
                    holder.Clear();
                    sourceHand.RemoveCardHolder(holder);
                }
            }

            NCombatUi? combatUi = NCombatRoom.Instance?.Ui;
            if (combatUi != null && cardNode.GetParent() == null)
            {
                combatUi.AddChildSafely(cardNode);
                cardNode.GlobalPosition = cardGlobalPos;
            }
        }

        card.RemoveFromCurrentPile(silent: true);
        card.GiveToAnotherPlayer(targetPlayer);

        CardPile targetPile = PileType.Hand.GetPile(targetPlayer);
        await CardPileCmd.Add(new[] { card }, targetPile, position, null, true, true);

        bool endedInHand = card.Pile?.Type == PileType.Hand;

        if (cardNode != null && cardNode.IsValid() && cardNode.GetParent() != null)
        {
            Node? vfxContainer = card.Owner.Creature.GetVfxContainer();
            if (vfxContainer != null)
            {
                cardNode.Reparent(vfxContainer);

                if (targetIsLocal)
                {
                    PileType flyTarget = endedInHand ? PileType.Hand : PileType.Discard;
                    NCardFlyVfx? flyVfx = NCardFlyVfx.Create(cardNode, flyTarget, true, card.Owner.Character.TrailPath);
                    vfxContainer.AddChildSafely(flyVfx);

                    if (flyVfx != null && flyVfx.SwooshAwayCompletion != null)
                    {
                        await flyVfx.SwooshAwayCompletion.Task;
                    }

                    if (endedInHand)
                    {
                        NPlayerHand? targetHand = NCombatRoom.Instance?.Ui?.Hand;
                        if (targetHand != null)
                        {
                            NCard? newCardNode = NCard.Create(card);
                            if (newCardNode != null)
                            {
                                NCombatUi? combatUi = NCombatRoom.Instance?.Ui;
                                if (combatUi != null)
                                {
                                    combatUi.AddChildSafely(newCardNode);
                                    newCardNode.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
                                    targetHand.Add(newCardNode);
                                }
                            }
                        }
                    }
                }
                else
                {
                    NCardFlyVfx? flyVfx = NCardFlyVfx.Create(cardNode, targetPlayer.Creature, card.Owner.Character.TrailPath);
                    vfxContainer.AddChildSafely(flyVfx);
                }
            }
        }
        else if (targetIsLocal)
        {
            if (endedInHand)
            {
                NPlayerHand? targetHand = NCombatRoom.Instance?.Ui?.Hand;
                NCombatUi? combatUi = NCombatRoom.Instance?.Ui;
                if (targetHand != null && combatUi != null)
                {
                    NCard? newCardNode = NCard.Create(card);
                    if (newCardNode != null)
                    {
                        combatUi.AddChildSafely(newCardNode);
                        newCardNode.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
                        targetHand.Add(newCardNode);
                    }
                }
            }

            card.Pile?.InvokeCardAddFinished();
        }
    }
}