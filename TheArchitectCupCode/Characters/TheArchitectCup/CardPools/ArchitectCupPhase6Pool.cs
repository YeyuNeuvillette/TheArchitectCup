using Godot;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using TheArchitectCup.Content;

namespace TheArchitectCup.Characters.TheArchitectCup;

[RegisterSharedCardPool]
public class ArchitectCupPhase6Pool : TypeListCardPoolModel
{
    public override string Title => "ArchitectCupPhase6";
    public override string EnergyColorName => "architect_cup";
    public override Color DeckEntryCardColor => new("d48900");
    public override bool IsColorless => true;

#pragma warning disable CS0618
    [Obsolete]
    protected override IEnumerable<Type> CardTypes => ArchitectCupCardCatalog.All
        .Where(static definition => definition.Phase == 6 && definition.ShowInCompendium)
        .Select(static definition => definition.ModelType);
#pragma warning restore CS0618
}
