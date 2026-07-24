using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace TheArchitectCup.Api;

/// <summary>Context supplied while filtering Burn the Mountain generation candidates.</summary>
public readonly record struct CrossCharacterCardFilterContext(
    Player Player,
    CardModel SourceCard,
    CharacterModel CurrentCharacter);
