using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Runs;

namespace TheArchitectCup.Infrastructure;

internal static class GameCompatibility
{
    internal static bool IsRunAuthority(RunManager? runManager = null)
    {
        RunManager manager = runManager ?? RunManager.Instance;
        return manager.IsSingleplayerOrFakeMultiplayer || manager.NetService is INetHostGameService;
    }
}
