using Godot;
using STS2RitsuLib.Scaffolding.Characters;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;
using TheArchitectCup.Extensions;

namespace TheArchitectCup.Characters.TheArchitectCup;

public class ArchitectCupCardPool : TypeListCardPoolModel, IModColorfulPhilosophersCardPool
{
    public override string Title => "TheArchitectCup";
    public override string EnergyColorName => "architect_cup";
    public override string BigEnergyIconPath => "charui/energy_architect_cup_big.png".ImagePath();
    public override string TextEnergyIconPath => "charui/energy_architect_cup.png".ImagePath();
    public override Material? PoolFrameMaterial => MaterialUtils.CreateHsvShaderMaterial(0.08f, 0.73f, 0.93f);
    public override Color DeckEntryCardColor => new("d48900");
    public override bool IsColorless => false;
}