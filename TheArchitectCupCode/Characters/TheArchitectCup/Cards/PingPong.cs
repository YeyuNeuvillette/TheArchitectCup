using System;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.TheArchitectCup.Powers;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(ColorlessCardPool))]
public sealed class PingPong() : ArchitectCupCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "小肥")
    ];

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        Player targetPlayer = cardPlay.Target.Player!;

        PingPongPower? power = await PowerCmd.Apply<PingPongPower>(choiceContext, targetPlayer.Creature, 1, Owner.Creature, this);
        if (power != null)
            power.IsUpgraded = IsUpgraded;
    }

    protected override void OnUpgrade()
    {
    }
}