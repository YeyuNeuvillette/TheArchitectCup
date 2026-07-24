using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Players;
using STS2RitsuLib.Combat.PlayerResources;

namespace TheArchitectCup.Characters.TheArchitectCup.Patches;

[Obsolete("Implement IPlayerResourceHookListener from STS2.RitsuLib instead.")]
public static class EnergyGainPatch
{
    public static readonly List<IEnergyGainedListener> Listeners = new();

    static EnergyGainPatch()
    {
        PlayerResourceHook.RegisterGlobalListener(new LegacyEnergyGainedAdapter());
    }

    private sealed class LegacyEnergyGainedAdapter : IPlayerResourceHookListener
    {
        public Task AfterPlayerEnergyGained(PlayerResourceGainContext context)
        {
            IEnergyGainedListener[] snapshot = Listeners.ToArray();
            for (int index = snapshot.Length - 1; index >= 0; index--)
                snapshot[index].OnEnergyGained(context.Player, context.Amount);

            return Task.CompletedTask;
        }
    }
}

[Obsolete("Implement IPlayerResourceHookListener from STS2.RitsuLib instead.")]
public interface IEnergyGainedListener
{
    void OnEnergyGained(Player player, decimal amount);
}
