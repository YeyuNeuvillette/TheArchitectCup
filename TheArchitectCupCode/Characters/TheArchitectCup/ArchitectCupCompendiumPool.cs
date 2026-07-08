using Godot;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using TheArchitectCup.Characters.TheArchitectCup.Cards;

namespace TheArchitectCup.Characters.TheArchitectCup;

[RegisterSharedCardPool]
public class ArchitectCupCompendiumPool : TypeListCardPoolModel
{
    public override string Title => "ArchitectCupCompendium";
    public override string EnergyColorName => "architect_cup";
    public override Color DeckEntryCardColor => new("d48900");

#pragma warning disable CS0618
    [Obsolete]
    protected override IEnumerable<Type> CardTypes =>
    [
        typeof(CursedHandkerchief),
        typeof(ThreeLeggedRace),
        typeof(PingPong),
        typeof(GenerousDonation),
        typeof(FindSomeoneToGetYou),
        typeof(EarthShattering),
        typeof(BurnTheMountain),
        typeof(BeeBroOnTheRun)
    ];
#pragma warning restore CS0618

    public override bool IsColorless => true;
}