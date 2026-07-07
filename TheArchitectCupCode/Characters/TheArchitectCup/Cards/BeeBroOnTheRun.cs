using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(ArchitectCupCardPool))]
public sealed class BeeBroOnTheRun() : ArchitectCupCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
    }

    protected override void OnUpgrade()
    {
    }
}