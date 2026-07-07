using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;
using TheArchitectCup.Characters.TheArchitectCup.Cards;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public sealed class BurnTheMountainPower : BasePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IfUpgradedVar((int)Amount >= 2 ? UpgradeDisplay.Upgraded : UpgradeDisplay.Normal)
    ];

    public override bool TryModifyKeywordsInCombat(CardModel card, ISet<CardKeyword> keywords)
    {
        if (card.Owner != Owner.Player)
            return false;

        return keywords.Add(CardKeyword.Exhaust);
    }

    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(
        CardModel card, bool isAutoPlay, ResourceInfo resources,
        PileType pileType, CardPilePosition position)
    {
        if (card.Owner.Creature != Owner)
            return (pileType, position);

        return (PileType.Exhaust, position);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner)
            return;

        if (cardPlay.Card is BurnTheMountain)
            return;

        Player? player = cardPlay.Card.Owner;
        if (player == null)
            return;

        CharacterModel currentCharacter = player.Character;

        var otherCharacterCards = ModelDb.AllCharacters
            .Where(c => c != currentCharacter)
            .SelectMany(c => c.CardPool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint))
            .ToList();

        if (otherCharacterCards.Count == 0)
            return;

        var randomCards = CardFactory.GetForCombat(
            player,
            otherCharacterCards,
            1,
            player.RunState.Rng.CombatCardGeneration).ToList();

        foreach (var newCard in randomCards)
        {
            if ((int)Amount >= 2)
            {
                CardCmd.Upgrade(newCard);
            }

            await CardPileCmd.AddGeneratedCardToCombat(newCard, PileType.Hand, player);
        }
    }
}