using Godot;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using TheArchitectCup.Content;

namespace TheArchitectCup.Characters.TheArchitectCup;

[RegisterSharedCardPool]
public class ArchitectCupPhase5Pool : TypeListCardPoolModel
{
    public override string Title => "ArchitectCupPhase5";
    public override string EnergyColorName => "architect_cup";
    public override Color DeckEntryCardColor => new("d48900");

#pragma warning disable CS0618
    [Obsolete]
    protected override IEnumerable<Type> CardTypes => ArchitectCupCardCatalog.All
        .Where(static definition => definition.Phase == 5 && definition.ShowInCompendium)
        .Select(static definition => definition.ModelType);
#pragma warning restore CS0618

    public override bool IsColorless => true;
}
