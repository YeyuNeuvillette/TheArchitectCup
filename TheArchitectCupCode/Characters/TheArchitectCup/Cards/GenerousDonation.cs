using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheArchitectCup.Characters.TheArchitectCup.Patches;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(SilentCardPool))]
public sealed class GenerousDonation() : ArchitectCupCard(3, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(new LocString("static_hover_tips", "AUTHOR.title"), "SkyF")
    ];

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Sly];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null || cardPlay.Target.Player == null)
            return;

        var targetPlayer = cardPlay.Target.Player;
        var maxSelect = IsUpgraded ? 999999999 : 1;
        var promptKey = IsUpgraded ? "TO_GIVE_ANY" : "TO_GIVE";
        var selectedCards = (await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(new LocString("card_selection", promptKey), 1, maxSelect),
            null,
            this)).ToList();

        foreach (var card in selectedCards)
        {
            await HandTransferPatch.GiveCardFromHandToAnotherPlayerHand(card, targetPlayer);
        }
    }
}