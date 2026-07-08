using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop;
using TheArchitectCup.Characters.TheArchitectCup;
using TheArchitectCup.Extensions;
using TheArchitectCup.Settings;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace TheArchitectCup;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "TheArchitectCup";
    public const string ResPath = $"res://{ModId}";

    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    public static void Initialize()
    {
        var assembly = Assembly.GetExecutingAssembly();
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);

        Harmony harmony = new(ModId);
        harmony.PatchAll();

        CardSettingsPage.Register();

        ModContentRegistry.For(ModId)
            .RegisterCardLibraryCompendiumSharedPoolFilter<ArchitectCupCompendiumPool>(
                "ARCHITECT_CUP_COMPENDIUM",
                "ui/the_architect_cup_one.png".ImagePath(),
                [
                    new()
                    {
                        VanillaFilterAnchorUniqueName = CardLibraryCompendiumVanillaFilterNames.ColorlessPool,
                        Relation = CardLibraryCompendiumFilterInsertRelation.After,
                    },
                ]);

        Logger.Info("TheArchitectCup mod initialized successfully");
    }
}