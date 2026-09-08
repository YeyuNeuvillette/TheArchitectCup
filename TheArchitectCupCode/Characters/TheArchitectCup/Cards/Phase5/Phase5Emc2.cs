using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.TheArchitectCup.Powers;
using MegaCrit.Sts2.Core.Commands;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

public abstract class Phase5Emc2() : ArchitectCupCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "💤"),
        EnergyHoverTip
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Emc2Power>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}

[RegisterCard(typeof(DefectCardPool))]
public class Phase5Emc2Defect() : Phase5Emc2{}

[RegisterCard(typeof(RegentCardPool))]
public class Phase5Emc2Regent() : Phase5Emc2{}