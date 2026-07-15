using Godot;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using TheArchitectCup.Characters.TheArchitectCup.Cards;
using TheArchitectCup.Characters.TheArchitectCup.Cards.Phase2;

namespace TheArchitectCup.Characters.TheArchitectCup;

[RegisterSharedCardPool]
public class ArchitectCupPhase2Pool : TypeListCardPoolModel
{
    public override string Title => "ArchitectCupPhase2";
    public override string EnergyColorName => "architect_cup";
    public override Color DeckEntryCardColor => new("d48900");

#pragma warning disable CS0618
    [Obsolete]
    protected override IEnumerable<Type> CardTypes =>
    [
        typeof(Coupling),
        typeof(PressForward),
        typeof(VakuuTeachesUToPlay),
        typeof(LikeShadow),
        typeof(Agitation)
    ];
#pragma warning restore CS0618

    public override bool IsColorless => true;
}