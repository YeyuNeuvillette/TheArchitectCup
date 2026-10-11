using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(ColorlessCardPool), FullPublicEntry = global::TheArchitectCup.Api.ArchitectCupCardIds.Phase6Apotheosis)]
public sealed class Phase6Apotheosis() : ArchitectCupCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "登高"),
        new HoverTip(new LocString("static_hover_tips", "CHAMPION_PHASE6.title"), new LocString("static_hover_tips", "CHAMPION_PHASE6.description")),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate];

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.PlayerCombatState == null)
        {    
            return Task.CompletedTask;
        }
        if (!IsUpgraded)
        {
            foreach (CardModel card in Owner.PlayerCombatState.AllCards.ToArray())
            {
                if (card.IsUpgradable)
                {
                    CardCmd.Upgrade(card);
                }
            }
        }
        else
        {
            foreach (CardModel card in Owner.PlayerCombatState.AllCards.Where(c => c.IsUpgraded).ToArray())
            {
                CardCmd.Downgrade(card);
            }
        }
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Innate);
    }

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (!IsUpgraded)
        {
            return true;
        }
        if (card.Owner != Owner)
        {
            return true;
        }
        CardPile? pile = Pile;
        if (pile == null || pile.Type != PileType.Hand)
        {
            return true;
        }
        if (card is Phase6Apotheosis)
        {
            return true;
        }
        if (autoPlayType != AutoPlayType.None)
        {
            return true;
        }
        return false;
    }
}
