using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace TheArchitectCup.Characters.TheArchitectCup.Patches;

[HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Exhaust))]
public static class PowerCardExhaustPatch
{
    public static bool Prefix(PlayerChoiceContext choiceContext, CardModel card, ref bool skipVisuals)
    {
        if (card.Type == CardType.Power && card.Pile?.Type == PileType.Play)
        {
            skipVisuals = true;
        }
        return true;
    }
}