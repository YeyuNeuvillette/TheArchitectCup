using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using TheArchitectCup.Api;

namespace TheArchitectCup.Features.CardGeneration;

internal static class CrossCharacterCardGenerationService
{
    internal static IReadOnlyList<CardModel> GetCandidates(Player player, CardModel sourceCard)
    {
        CharacterModel currentCharacter = player.Character;
        var context = new CrossCharacterCardFilterContext(player, sourceCard, currentCharacter);

        return ModelDb.AllCharacters
            .Where(character => character.Id != currentCharacter.Id)
            .OrderBy(character => character.Id.Entry, StringComparer.Ordinal)
            .SelectMany(character => character.CardPool.GetUnlockedCards(
                player.UnlockState,
                player.RunState.CardMultiplayerConstraint))
            .Where(candidate => CrossCharacterCardFilterRegistry.Allows(context, candidate))
            .DistinctBy(candidate => candidate.Id)
            .OrderBy(candidate => candidate.Id.Entry, StringComparer.Ordinal)
            .ToArray();
    }
}
