using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Api;
using TheArchitectCup.Content;
using TheArchitectCup.Settings;

namespace TheArchitectCup.Characters.TheArchitectCup.Cards;

[RegisterCard(typeof(ColorlessCardPool), FullPublicEntry = ArchitectCupCardIds.Coronation)]
public sealed class Coronation() : ArchitectCupCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{

    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null)
            return;

        List<CardModel> cards = CreateChampionChoices();
        if (cards.Count == 0)
            return;

        if (IsUpgraded)
            CardCmd.Upgrade(cards, CardPreviewStyle.HorizontalLayout);

        CardModel? selectedCard = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, Owner, canSkip: true);
        if (selectedCard == null)
            return;

        selectedCard.SetToFreeThisTurn();
        await CardPileCmd.AddGeneratedCardToCombat(selectedCard, PileType.Hand, Owner);
    }

    private List<CardModel> CreateChampionChoices()
    {
        var candidates = ArchitectCupCardCatalog.ChampionCards
            .Where(IsChampionCardEnabled)
            .Select(static definition => ModelDb.GetById<CardModel>(ModelDb.GetId(definition.ModelType)))
            .Where(static card => card.CanBeGeneratedInCombat)
            .Distinct()
            .TakeRandom(3, Owner.RunState.Rng.CombatCardGeneration);

        return candidates
            .Select(card => CombatState!.CreateCard(card, Owner))
            .ToList();
    }

    private bool IsChampionCardEnabled(ArchitectCupCardDefinition definition)
    {
        if (!definition.Configurable)
            return true;

        return Owner.RunState is RunState runState
            ? CardSettingsService.IsCardEnabled(definition.Id, runState)
            : CardSettingsService.IsCardEnabled(definition.Id);
    }
}
