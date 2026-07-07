using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;

namespace TheArchitectCup.Characters.TheArchitectCup.Patches;

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.GainEnergy))]
public static class EnergyGainPatch
{
    public static readonly List<IEnergyGainedListener> Listeners = new();

    public static void Postfix(PlayerCombatState __instance, decimal amount, Player ____player)
    {
        if (amount <= 0m)
            return;

        if (____player == null)
            return;

        for (int i = Listeners.Count - 1; i >= 0; i--)
        {
            Listeners[i].OnEnergyGained(____player, amount);
        }
    }
}

public interface IEnergyGainedListener
{
    void OnEnergyGained(Player player, decimal amount);
}