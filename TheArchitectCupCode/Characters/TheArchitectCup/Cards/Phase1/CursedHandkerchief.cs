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
        Player player = cardPlay.Player;
        int cardsPlayedThisTurn = CombatManager.Instance.History.CardPlaysStarted.Count(
            (CardPlayStartedEntry entry) =>
                entry.HappenedThisTurn(base.CombatState) && entry.CardPlay.Player == player);
        if (cardsPlayedThisTurn == 0 || cardsPlayedThisTurn % 7 != 0)
            return;

        EnergyCost.AddThisTurn(1);
        if (Pile?.Type == PileType.Hand && Owner == player)
            return;

        if (Owner == player)
            await CardPileCmd.Add(this, PileType.Hand);
        else
            await CardPileCmd.GiveToAnotherPlayer(this, player, PileType.Hand);
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
