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

        int expectedPos = data.lastPlayedPosition + 1;
        if (expectedPos >= data.handSnapshot.Count)
            return false;

        var expectedCard = data.handSnapshot[expectedPos];
        var currentHand = PileType.Hand.GetPile(player).Cards;
        return currentHand.Contains(card) && expectedCard == card;
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

        Godot.GD.Print($"[VakuuTeachesUToPlayPower] StartTrackingCurrentTurn, hand count={data.handSnapshot.Count}");
        for (int i = 0; i < data.handSnapshot.Count; i++)
        {
            Godot.GD.Print($"[VakuuTeachesUToPlayPower]   Snapshot pos {i}: {data.handSnapshot[i].Id}");
        }
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
        Godot.GD.Print($"[VakuuTeachesUToPlayPower] BeforeCardPlayed: card={playedCard.Id}, playedPos={playedPos}, firstCardPlayed={data.firstCardPlayed}, lastPlayedPosition={data.lastPlayedPosition}");

        if (playedPos < 0)
        {
            Godot.GD.Print($"[VakuuTeachesUToPlayPower] Card not in snapshot, disabling");
            data.canFunctionThisTurn = false;
            return Task.CompletedTask;
        }

        if (!data.firstCardPlayed)
        {
            data.firstCardPlayed = true;
            data.lastPlayedPosition = playedPos;
            Godot.GD.Print($"[VakuuTeachesUToPlayPower] First card played at position {playedPos}");
            return Task.CompletedTask;
        }

        if (playedPos != data.lastPlayedPosition + 1)
        {
            Godot.GD.Print($"[VakuuTeachesUToPlayPower] FAILED! Expected pos {data.lastPlayedPosition + 1}, got {playedPos}");
            data.canFunctionThisTurn = false;
            return Task.CompletedTask;
        }

        data.lastPlayedPosition = playedPos;
        Godot.GD.Print($"[VakuuTeachesUToPlayPower] SUCCESS! Next expected pos: {data.lastPlayedPosition + 1}");
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