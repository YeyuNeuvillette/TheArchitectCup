using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using TheArchitectCup.Characters.Base;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TheArchitectCup.Characters.TheArchitectCup.Powers;

[RegisterPower]
public class VakuuTeachesUToPlayPower : BasePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private class Data
    {
        public bool canFunctionThisTurn;
        public bool firstCardPlayed;
        public int lastPlayedPosition = -1;
        public List<CardModel> handSnapshot = [];
    }

    protected override object InitInternalData()
    {
        return new Data()
        {
            canFunctionThisTurn = false,
            firstCardPlayed = false,
            lastPlayedPosition = -1
        };
    }

    public bool ShouldGlowForVakuu(CardModel card)
    {
        var data = GetInternalData<Data>();
        if (!data.canFunctionThisTurn || !data.firstCardPlayed)
            return false;

        var player = Owner?.Player;
        if (player == null || card.Owner != player)
            return false;

        var currentHand = PileType.Hand.GetPile(player).Cards;
        if (!currentHand.Contains(card))
            return false;

        int searchPos = data.lastPlayedPosition + 1;
        while (searchPos < data.handSnapshot.Count)
        {
            var candidate = data.handSnapshot[searchPos];
            if (candidate == card)
                return true;

            if (candidate.CanPlay())
                return false;

            searchPos++;
        }

        return false;
    }

    public void StartTrackingCurrentTurn(Player player)
    {
        if (player != Owner?.Player)
            return;

        var data = GetInternalData<Data>();
        data.canFunctionThisTurn = true;
        data.firstCardPlayed = false;
        data.lastPlayedPosition = -1;
        data.handSnapshot = [.. PileType.Hand.GetPile(player).Cards];

    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        StartTrackingCurrentTurn(player);
        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        var playedCard = cardPlay?.Card;
        var player = Owner?.Player;
        var data = GetInternalData<Data>();

        if (playedCard == null || player == null || playedCard.Owner != player || !data.canFunctionThisTurn)
            return Task.CompletedTask;

        int playedPos = data.handSnapshot.IndexOf(playedCard);
        if (playedPos < 0)
        {
            data.canFunctionThisTurn = false;
            return Task.CompletedTask;
        }

        if (!data.firstCardPlayed)
        {
            data.firstCardPlayed = true;
            data.lastPlayedPosition = playedPos;
            return Task.CompletedTask;
        }

        int expectedPos = data.lastPlayedPosition + 1;
        if (playedPos == expectedPos)
        {
            data.lastPlayedPosition = playedPos;
            return Task.CompletedTask;
        }

        if (playedPos > expectedPos)
        {
            bool allUnplayable = true;
            for (int i = expectedPos; i < playedPos; i++)
            {
                var skippedCard = data.handSnapshot[i];
                if (skippedCard.CanPlay())
                {
                    allUnplayable = false;
                    break;
                }
            }

            if (allUnplayable)
            {
                data.lastPlayedPosition = playedPos;
                return Task.CompletedTask;
            }
        }

        data.canFunctionThisTurn = false;
        return Task.CompletedTask;
    }

    public override bool ShouldFlush(Player player)
    {
        return player != Owner.Player || !GetInternalData<Data>().canFunctionThisTurn;
    }

    public override bool ShouldClearBlock(Creature creature)
    {
        return Owner != creature || !GetInternalData<Data>().canFunctionThisTurn;
    }

    public override Task AfterPreventingBlockClear(AbstractModel preventer, Creature creature)
    {
        if (this == preventer)
        {
            Flash();
        }
        return Task.CompletedTask;
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];
}
