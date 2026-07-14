using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Interop.AutoRegistration;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(CurseCardPool))]
public sealed class CursedHandkerchief() : ArchitectCupCard(1, CardType.Curse, CardRarity.Curse, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "罡璧")
    ];

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override int MaxUpgradeLevel => 0;

    protected override bool ShouldGlowRedInternal => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Player player = cardPlay.Card.Owner;
        CardPile? pile = base.Pile;
        GD.Print($"[CursedHandkerchief] AfterCardPlayedLate called. PlayedCard={cardPlay.Card.Id.Entry}, PlayedCardOwner={player.NetId}, MyOwner={base.Owner?.NetId}, MyPile={pile?.Type}");

        if (pile != null && pile.Type == PileType.Hand && base.Owner == player)
        {
            GD.Print($"[CursedHandkerchief] Early return: already in hand of playing player.");
            return;
        }

        ICombatState? combatState = base.CombatState;
        GD.Print($"[CursedHandkerchief] CombatState={(combatState != null ? $"Round={combatState.RoundNumber} Side={combatState.CurrentSide}" : "null")}");

        int totalPlays = CombatManager.Instance.History.CardPlaysStarted.Count((CardPlayStartedEntry e) => e.CardPlay.Card.Owner == player);
        int CardsPlayedThisTurn = CombatManager.Instance.History.CardPlaysStarted.Count((CardPlayStartedEntry e) => e.HappenedThisTurn(base.CombatState) && e.CardPlay.Card.Owner == player);
        GD.Print($"[CursedHandkerchief] Player={player.NetId}, TotalPlays={totalPlays}, PlaysThisTurn={CardsPlayedThisTurn}, Mod7={CardsPlayedThisTurn % 7}");

        int handCount = PileType.Hand.GetPile(player).Cards.Count;
        GD.Print($"[CursedHandkerchief] HandCount={handCount}, MaxHand={CardPile.MaxCardsInHand}, Condition={CardsPlayedThisTurn % 7 == 0 && handCount < CardPile.MaxCardsInHand}");

        if (CardsPlayedThisTurn % 7 == 0 && handCount < CardPile.MaxCardsInHand)
        {
            GD.Print($"[CursedHandkerchief] TRIGGERED! Moving to {(player == base.Owner ? "same owner hand" : $"player {player.NetId} hand")}");
            EnergyCost.UpgradeBy(1);
            if (player == base.Owner)
            {
                await CardPileCmd.Add(this, PileType.Hand);
            }
            else
            {
                RemoveFromCurrentPile(silent: true);
                GiveToAnotherPlayer(player);
                await CardPileCmd.Add(
                    new CardModel[] { this },
                    PileType.Hand.GetPile(player),
                    CardPilePosition.Bottom,
                    null,
                    false,
                    true
                );
            }
        }
    }

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner != base.Owner)
        {
            return true;
        }

        CardPile? pile = base.Pile;
        if (pile == null || pile.Type != PileType.Hand)
        {
            return true;
        }

        if (card is CursedHandkerchief)
        {
            return true;
        }

        if (autoPlayType != AutoPlayType.None)
        {
            return true;
        }

        return false;
    }
}